#!/usr/bin/env bash
# DimDim - apaga tudo (Resource Group inteiro) depois da avaliação, para não gastar crédito.
set -euo pipefail
cd "$(dirname "$0")"
SQL_ADMIN_PASSWORD="nao-usada" source ./00-variaveis.sh
az group delete --name "$RG" --yes --no-wait
echo "Exclusão de $RG solicitada."
