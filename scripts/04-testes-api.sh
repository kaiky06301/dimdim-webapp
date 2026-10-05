#!/usr/bin/env bash
# =====================================================================
# DimDim - testa o CRUD das duas tabelas pela API JSON do Web App
# (GET, POST, PUT, DELETE). Os corpos usados estão em /api-json.
# Depois de cada passo, conferir no Query editor com scripts/consultas.sql
# =====================================================================
set -euo pipefail
cd "$(dirname "$0")/.."
SQL_ADMIN_PASSWORD="nao-usada" source ./scripts/00-variaveis.sh
API="https://${WEBAPP}.azurewebsites.net/api"
pausa() { read -r -p "   [Enter para continuar] " _; }

echo ">> POST /clientes";  curl -s -X POST "$API/clientes" -H "Content-Type: application/json" -d @api-json/cliente-post.json; echo; pausa
ID_CLIENTE=$(curl -s "$API/clientes" | python3 -c "import sys,json; print(json.load(sys.stdin)[-1]['id'])")
echo ">> GET /clientes/$ID_CLIENTE"; curl -s "$API/clientes/$ID_CLIENTE"; echo; pausa
echo ">> PUT /clientes/$ID_CLIENTE"; curl -s -o /dev/null -w "HTTP %{http_code}\n" -X PUT "$API/clientes/$ID_CLIENTE" -H "Content-Type: application/json" -d @api-json/cliente-put.json; pausa

echo ">> POST /contas"
sed "s/\"clienteId\": *[0-9]*/\"clienteId\": $ID_CLIENTE/" api-json/conta-post.json | curl -s -X POST "$API/contas" -H "Content-Type: application/json" -d @-; echo; pausa
ID_CONTA=$(curl -s "$API/contas" | python3 -c "import sys,json; print(json.load(sys.stdin)[-1]['id'])")
echo ">> GET /contas/$ID_CONTA"; curl -s "$API/contas/$ID_CONTA"; echo; pausa
echo ">> PUT /contas/$ID_CONTA"
sed "s/\"clienteId\": *[0-9]*/\"clienteId\": $ID_CLIENTE/" api-json/conta-put.json | curl -s -o /dev/null -w "HTTP %{http_code}\n" -X PUT "$API/contas/$ID_CONTA" -H "Content-Type: application/json" -d @-; pausa

echo ">> DELETE /contas/$ID_CONTA";     curl -s -o /dev/null -w "HTTP %{http_code}\n" -X DELETE "$API/contas/$ID_CONTA"; pausa
echo ">> DELETE /clientes/$ID_CLIENTE"; curl -s -o /dev/null -w "HTTP %{http_code}\n" -X DELETE "$API/clientes/$ID_CLIENTE"
