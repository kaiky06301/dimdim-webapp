-- Consultas para mostrar a persistência no vídeo (Query Editor do portal)
-- Rodar depois de cada operação do CRUD (Create, Read, Update, Delete) em cada tabela.

SELECT * FROM dbo.CLIENTE ORDER BY ID_CLIENTE;

SELECT * FROM dbo.CONTA ORDER BY ID_CONTA;

-- Relacionamento: contas com o nome do cliente dono
SELECT c.ID_CONTA, c.AGENCIA, c.NUMERO, c.TIPO, c.SALDO, cl.ID_CLIENTE, cl.NOME
FROM dbo.CONTA c
JOIN dbo.CLIENTE cl ON cl.ID_CLIENTE = c.ID_CLIENTE
ORDER BY c.ID_CONTA;
