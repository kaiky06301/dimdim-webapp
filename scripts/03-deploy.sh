#!/usr/bin/env bash
# =====================================================================
# DimDim - build (dotnet publish) + deploy automatizado com
#          "az webapp deploy" (pacote zip) no Azure Web App.
# Não precisa de senha: usa só a sessão do "az login".
# =====================================================================
set -euo pipefail
cd "$(dirname "$0")/.."
SQL_ADMIN_PASSWORD="nao-usada-no-deploy" source ./scripts/00-variaveis.sh

rm -rf publish app.zip
echo ">> dotnet publish (Release)"
dotnet publish src/DimDim.Web/DimDim.Web.csproj -c Release -o publish

echo ">> Empacotando em app.zip"
if command -v zip >/dev/null 2>&1; then
  (cd publish && zip -qr ../app.zip .)
else
  (cd publish && python3 -c "import shutil; shutil.make_archive('../app', 'zip', '.')")
fi

echo ">> az webapp deploy"
az webapp deploy \
  --resource-group "$RG" --name "$WEBAPP" \
  --src-path app.zip --type zip --restart true

echo
echo "Deploy concluído: https://${WEBAPP}.azurewebsites.net"
