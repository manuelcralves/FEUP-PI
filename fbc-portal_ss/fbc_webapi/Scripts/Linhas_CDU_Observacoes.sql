-- CDU_Observacoes| Compras
ALTER TABLE LinhasCompras ADD
	CDU_Observacoes nvarchar(4000) NULL
GO

INSERT INTO StdCamposVar (Tabela, Campo, Descricao, Query, Texto, Visivel, ValorDefeito, DadosSensiveis, ExportarTTE, Ordem) 
VALUES ('LinhasCompras', 'CDU_Observacoes',
		SUBSTRING('Observacoes', 0, 80), '',
		SUBSTRING('Observacoes', 0, 20), 1, null, 0, 0,
		(SELECT ISNULL((SELECT TOP 1 Ordem + 1 FROM StdCamposVar WHERE Tabela = 'LinhasCompras' ORDER BY Ordem DESC), 1)))
GO

-- CDU_Observacoes| Internos
ALTER TABLE LinhasInternos ADD
	CDU_Observacoes nvarchar(4000) NULL
GO

INSERT INTO StdCamposVar (Tabela, Campo, Descricao, Query, Texto, Visivel, ValorDefeito, DadosSensiveis, ExportarTTE, Ordem) 
VALUES ('LinhasInternos', 'CDU_Observacoes',
		SUBSTRING('Observacoes', 0, 80), '',
		SUBSTRING('Observacoes', 0, 20), 1, null, 0, 0,
		(SELECT ISNULL((SELECT TOP 1 Ordem + 1 FROM StdCamposVar WHERE Tabela = 'LinhasInternos' ORDER BY Ordem DESC), 1)))
GO