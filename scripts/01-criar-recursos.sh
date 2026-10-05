#!/usr/bin/env bash
# =====================================================================
# DimDim - cria TODA a infraestrutura na Azure via Azure CLI
#   Resource Group, Azure SQL (servidor + banco + firewall),
#   Log Analytics + Application Insights, App Service Plan + Web App,
#   App Settings e Connection String do Web App.
# =====================================================================
set -euo pipefail
cd "$(dirname "$0")"
source ./00-variaveis.sh

echo ">> Registrando os resource providers (necessário em assinatura nova)"
for ns in Microsoft.Web Microsoft.Sql Microsoft.Insights Microsoft.OperationalInsights; do
  az provider register --namespace "$ns" --wait
done
az extension add --name application-insights --upgrade --only-show-errors

echo ">> Resource Group"
az group create --name "$RG" --location "$LOCATION" -o table

echo ">> Azure SQL Server (PaaS) e banco"
az sql server create \
  --name "$SQL_SERVER" --resource-group "$RG" --location "$LOCATION" \
  --admin-user "$SQL_ADMIN" --admin-password "$SQL_ADMIN_PASSWORD" -o table

az sql db create \
  --resource-group "$RG" --server "$SQL_SERVER" --name "$SQL_DB" \
  --edition Basic --capacity 5 --backup-storage-redundancy Local -o table

echo ">> Firewall: libera serviços da Azure (o Web App) e o IP desta máquina (para rodar o DDL)"
az sql server firewall-rule create \
  --resource-group "$RG" --server "$SQL_SERVER" --name AllowAzureServices \
  --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0 -o table

MEU_IP=$(curl -s https://api.ipify.org)
az sql server firewall-rule create \
  --resource-group "$RG" --server "$SQL_SERVER" --name MeuIP \
  --start-ip-address "$MEU_IP" --end-ip-address "$MEU_IP" -o table

echo ">> Log Analytics + Application Insights"
az monitor log-analytics workspace create \
  --resource-group "$RG" --workspace-name "$LAW" --location "$LOCATION" -o table

LAW_ID=$(az monitor log-analytics workspace show -g "$RG" -n "$LAW" --query id -o tsv)
az monitor app-insights component create \
  --app "$APPINSIGHTS" --resource-group "$RG" --location "$LOCATION" \
  --kind web --application-type web --workspace "$LAW_ID" -o table

AI_CONN=$(az monitor app-insights component show -g "$RG" --app "$APPINSIGHTS" --query connectionString -o tsv)

echo ">> App Service Plan (Linux B1) e Web App .NET 10"
az appservice plan create \
  --name "$PLAN" --resource-group "$RG" --location "$LOCATION" \
  --sku B1 --is-linux -o table

az webapp create \
  --name "$WEBAPP" --resource-group "$RG" --plan "$PLAN" \
  --runtime "$RUNTIME" -o table

echo ">> App Settings: liga o Application Insights no Web App"
az webapp config appsettings set \
  --name "$WEBAPP" --resource-group "$RG" \
  --settings APPLICATIONINSIGHTS_CONNECTION_STRING="$AI_CONN" \
             ASPNETCORE_ENVIRONMENT="Production" -o none

echo ">> Connection String do banco (fica no Web App, não no código)"
SQL_CONN="Server=tcp:${SQL_SERVER}.database.windows.net,1433;Initial Catalog=${SQL_DB};User ID=${SQL_ADMIN};Password=${SQL_ADMIN_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
az webapp config connection-string set \
  --name "$WEBAPP" --resource-group "$RG" \
  --connection-string-type SQLAzure \
  --settings DimDimDb="$SQL_CONN" -o none

az webapp update --name "$WEBAPP" --resource-group "$RG" --https-only true -o none

echo
echo "Recursos criados. Web App: https://${WEBAPP}.azurewebsites.net"
echo "Próximo passo: ./02-criar-tabelas.sh"
