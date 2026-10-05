-- =====================================================================
-- DimDim - DDL das tabelas (Azure SQL Database)
-- CLIENTE (1) ----< CONTA (N)
-- Pode ser executado no Query Editor do portal ou via sqlcmd
-- (scripts/02-criar-tabelas.sh). É idempotente: só cria o que não existe.
-- =====================================================================

IF OBJECT_ID('dbo.CLIENTE', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CLIENTE (
        ID_CLIENTE   INT IDENTITY(1,1) NOT NULL,
        NOME         NVARCHAR(100)     NOT NULL,
        CPF          CHAR(11)          NOT NULL,
        EMAIL        NVARCHAR(120)     NOT NULL,
        TELEFONE     NVARCHAR(20)      NULL,
        DT_CADASTRO  DATETIME2         NOT NULL CONSTRAINT DF_CLIENTE_DT DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_CLIENTE PRIMARY KEY (ID_CLIENTE),
        CONSTRAINT UQ_CLIENTE_CPF UNIQUE (CPF)
    );
END;
GO

IF OBJECT_ID('dbo.CONTA', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CONTA (
        ID_CONTA     INT IDENTITY(1,1) NOT NULL,
        AGENCIA      CHAR(4)           NOT NULL,
        NUMERO       VARCHAR(12)       NOT NULL,
        TIPO         VARCHAR(10)       NOT NULL,
        SALDO        DECIMAL(15,2)     NOT NULL CONSTRAINT DF_CONTA_SALDO DEFAULT 0,
        DT_ABERTURA  DATETIME2         NOT NULL CONSTRAINT DF_CONTA_DT DEFAULT SYSUTCDATETIME(),
        ID_CLIENTE   INT               NOT NULL,
        CONSTRAINT PK_CONTA PRIMARY KEY (ID_CONTA),
        CONSTRAINT UQ_CONTA_AG_NUM UNIQUE (AGENCIA, NUMERO),
        CONSTRAINT CK_CONTA_TIPO CHECK (TIPO IN ('CORRENTE', 'POUPANCA', 'SALARIO')),
        CONSTRAINT CK_CONTA_SALDO CHECK (SALDO >= 0),
        CONSTRAINT FK_CONTA_CLIENTE FOREIGN KEY (ID_CLIENTE)
            REFERENCES dbo.CLIENTE (ID_CLIENTE) ON DELETE CASCADE
    );
    CREATE INDEX IX_CONTA_CLIENTE ON dbo.CONTA (ID_CLIENTE);
END;
GO
