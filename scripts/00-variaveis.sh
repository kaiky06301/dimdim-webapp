#!/usr/bin/env bash
# =====================================================================
# DimDim - variáveis usadas por todos os scripts do Azure CLI
# Uso: os outros scripts fazem "source" deste arquivo.
# Troque RM pelo RM de quem vai criar os recursos (nomes de SQL Server
# e Web App precisam ser únicos no mundo inteiro).
# =====================================================================

export RM="${RM:-rm559523}"
export LOCATION="${LOCATION:-brazilsouth}"          # assinatura Students: se recusar, use mexicocentral

export RG="rg-dimdim-webapp"
export SQL_SERVER="sql-dimdim-${RM}"
export SQL_DB="dimdimdb"
export SQL_ADMIN="dimdimadmin"
export PLAN="plan-dimdim"
export WEBAPP="app-dimdim-${RM}"
export LAW="law-dimdim"                              # Log Analytics workspace (base do App Insights)
export APPINSIGHTS="appi-dimdim"
export RUNTIME="DOTNETCORE:10.0"

# A senha do banco NUNCA fica no código: vem da variável de ambiente
# SQL_ADMIN_PASSWORD ou é pedida no terminal.
if [ -z "${SQL_ADMIN_PASSWORD:-}" ]; then
  read -r -s -p "Senha do admin do Azure SQL (mín. 8 chars, maiúscula, minúscula, número e símbolo): " SQL_ADMIN_PASSWORD
  echo
fi
export SQL_ADMIN_PASSWORD
