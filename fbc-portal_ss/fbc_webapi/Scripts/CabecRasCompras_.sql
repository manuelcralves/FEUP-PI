
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CabecRasInternos](
	[Id] [uniqueidentifier] NOT NULL,
	[Documento] [nvarchar](50) NULL,
	[TipoDoc] [nvarchar](5) NOT NULL,
	[Serie] [nvarchar](5) NOT NULL,
	[NumDoc] [int] NOT NULL,
	[Estado] [nvarchar](1) NULL,
	[DataDoc] [datetime] NULL,
	[DataVenc] [datetime] NULL,
	[TipoEntidade] [nvarchar](1) NULL,
	[Entidade] [nvarchar](12) NULL,
	[NomeEntidade] [nvarchar](150) NULL,
	[NumContribuinte] [nvarchar](20) NULL,
	[DescFornecedor] [real] NULL,	
	[DescFinanceiro] [real] NULL,	
	[ObraID] [uniqueidentifier] NULL,
	[NomeObra] [nvarchar](150) NULL,
	[Utilizador] [nvarchar](20) NULL,
	[TotalDocumento] [float] NOT NULL,
	[Observacoes] [ntext] NULL,
	[DataGravacao] [datetime] NOT NULL,
	[DataUltimaActualizacao] [datetime] NULL,

 CONSTRAINT [CabecRasInternos01] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


ALTER TABLE [dbo].[CabecRasInternos] ADD  CONSTRAINT [CabecRasInternos_DataUltimaActualizacao_DF]  DEFAULT (getdate()) FOR [DataUltimaActualizacao]
GO

ALTER TABLE [dbo].[CabecRasInternos] ADD  CONSTRAINT [CabecRasInternos_Id_DF]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[CabecRasInternos] ADD  CONSTRAINT [CabecRasInternos_DataGravacao_DF]  DEFAULT (getdate()) FOR [DataGravacao]
GO

ALTER TABLE [dbo].[CabecRasInternos] ADD  CONSTRAINT [CabecRasInternos_TotalDocumento_DF]  DEFAULT ((0)) FOR [TotalDocumento]
GO

ALTER TABLE [dbo].[CabecRasInternos]  WITH CHECK ADD  CONSTRAINT [CabecRasInternos_DocumentosCompra_FK] FOREIGN KEY([TipoDoc])
REFERENCES [dbo].[DocumentosCompra] ([Documento])
GO

ALTER TABLE [dbo].[CabecRasInternos] CHECK CONSTRAINT [CabecRasInternos_DocumentosCompra_FK]
GO

ALTER TABLE [dbo].[CabecRasInternos]  WITH CHECK ADD  CONSTRAINT [CabecRasInternos_SeriesCompras_FK] FOREIGN KEY([TipoDoc], [Serie])
REFERENCES [dbo].[SeriesCompras] ([TipoDoc], [Serie])
GO

ALTER TABLE [dbo].[CabecRasInternos] CHECK CONSTRAINT [CabecRasInternos_SeriesCompras_FK]
GO



