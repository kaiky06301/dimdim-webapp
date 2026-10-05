#!/usr/bin/env bash
# =====================================================================
# DimDim - executa o DDL (ddl.sql) no Azure SQL Database com sqlcmd.
# Alternativa sem sqlcmd: abrir o banco no portal > Query editor,
# colar o conteúdo de scripts/ddl.sql e clicar em Run.
# =====================================================================
set -euo pipefail
cd "$(dirname "$0")"
source ./00-variaveis.sh

sqlcmd -S "tcp:${SQL_SERVER}.database.windows.net,1433" -d "$SQL_DB" \
  -U "$SQL_ADMIN" -P "$SQL_ADMIN_PASSWORD" -N -l 60 \
  -i ddl.sql

echo ">> Tabelas criadas:"
sqlcmd -S "tcp:${SQL_SERVER}.database.windows.net,1433" -d "$SQL_DB" \
  -U "$SQL_ADMIN" -P "$SQL_ADMIN_PASSWORD" -N -l 60 \
  -Q "SET NOCOUNT ON; SELECT name AS tabela FROM sys.tables ORDER BY name"
