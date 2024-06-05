-- CDU_Report| Compras
ALTER TABLE DocumentosCompra ADD
	CDU_Report nvarchar(100) NULL
GO

INSERT INTO StdCamposVar (Tabela, Campo, Descricao, Query, Texto, Visivel, ValorDefeito, DadosSensiveis, ExportarTTE, Ordem) 
VALUES ('DocumentosCompra', 'CDU_Report',
		SUBSTRING('Mapa Impressão', 0, 80), '',
		SUBSTRING('Mapa Impressão', 0, 20), 1, null, 0, 0,
		(SELECT ISNULL((SELECT TOP 1 Ordem + 1 FROM StdCamposVar WHERE Tabela = 'DocumentosCompra' ORDER BY Ordem DESC), 1)))
GO

-- CDU_Report| Internos
ALTER TABLE DocumentosInternos ADD
	CDU_Report nvarchar(100) NULL
GO

INSERT INTO StdCamposVar (Tabela, Campo, Descricao, Query, Texto, Visivel, ValorDefeito, DadosSensiveis, ExportarTTE, Ordem) 
VALUES ('DocumentosInternos', 'CDU_Report',
		SUBSTRING('Mapa Impressão', 0, 80), '',
		SUBSTRING('Mapa Impressão', 0, 20), 1, null, 0, 0,
		(SELECT ISNULL((SELECT TOP 1 Ordem + 1 FROM StdCamposVar WHERE Tabela = 'DocumentosInternos' ORDER BY Ordem DESC), 1)))
GO