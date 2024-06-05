
CREATE TABLE [dbo].[LinhasRasCompras](
	[Id] [uniqueidentifier] NOT NULL,
	[IdCabecRasCompras] [uniqueidentifier] NOT NULL,
	[NumLinha] [int] NOT NULL,
	[Artigo] [nvarchar](48) NULL,
	[Descricao] [varchar](512) NULL,
	[Quantidade] [float] NULL,
	[Unidade] [nvarchar](5) NULL,
	[Armazem] [nvarchar](5) NULL,
	[Localizacao] [varchar](30) NULL,
	[Lote] [varchar](20) NULL,
	[Observacoes] [nvarchar](4000) NULL,	
	[Preco] [float] NULL,
	[Total] [float] NULL,
	[ObraId] [uniqueidentifier] NULL,
	[Data] [datetime] NULL,
	[DataEntrega] [datetime] NULL,
	[Estado] [varchar](1) NULL,
	[Anulado] [bit] NULL,
	[Fechado] [bit] NULL,


 CONSTRAINT [LinhasRasCompras01] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LinhasRasCompras] ADD  CONSTRAINT [LinhasRasCompras_Id_DF]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[LinhasRasCompras] ADD  CONSTRAINT [LinhasRasCompras_NumLinha_DF]  DEFAULT ((0)) FOR [NumLinha]
GO

ALTER TABLE [dbo].[LinhasRasCompras] ADD  CONSTRAINT [LinhasRasCompras_Quantidade_DF]  DEFAULT (0) FOR [Quantidade]
GO

ALTER TABLE [dbo].[LinhasRasCompras] ADD  CONSTRAINT [LinhasRasCompras_Preco_DF]  DEFAULT (0) FOR [Preco]
GO

ALTER TABLE [dbo].[LinhasRasCompras] ADD  CONSTRAINT [LinhasRasCompras_Data_DF]  DEFAULT (getdate()) FOR [Data]
GO

ALTER TABLE [dbo].[LinhasRasCompras] ADD  CONSTRAINT [LinhasRasCompras_Anulado_DF]  DEFAULT (0) FOR [Anulado]
GO

ALTER TABLE [dbo].[LinhasRasCompras] ADD  CONSTRAINT [LinhasRasCompras_Fechado_DF]  DEFAULT (0) FOR [Fechado]
GO

ALTER TABLE [dbo].[LinhasRasCompras] ADD  CONSTRAINT [LinhasRasCompras_Total_DF]  DEFAULT (0) FOR [Total]
GO

ALTER TABLE [dbo].[LinhasRasCompras]  WITH CHECK ADD  CONSTRAINT [LinhasRasCompras_ArmazemLocalizacoes_FK] FOREIGN KEY([Armazem], [Localizacao])
REFERENCES [dbo].[ArmazemLocalizacoes] ([Armazem], [Localizacao])
GO

ALTER TABLE [dbo].[LinhasRasCompras] CHECK CONSTRAINT [LinhasRasCompras_ArmazemLocalizacoes_FK]
GO

ALTER TABLE [dbo].[LinhasRasCompras]  WITH CHECK ADD  CONSTRAINT [LinhasRasCompras_Armazens_FK] FOREIGN KEY([Armazem])
REFERENCES [dbo].[Armazens] ([Armazem])
GO

ALTER TABLE [dbo].[LinhasRasCompras] CHECK CONSTRAINT [LinhasRasCompras_Armazens_FK]
GO

ALTER TABLE [dbo].[LinhasRasCompras]  WITH CHECK ADD  CONSTRAINT [LinhasRasCompras_Artigo_FK] FOREIGN KEY([Artigo])
REFERENCES [dbo].[Artigo] ([Artigo])
GO

ALTER TABLE [dbo].[LinhasRasCompras] CHECK CONSTRAINT [LinhasRasCompras_Artigo_FK]
GO

ALTER TABLE [dbo].[LinhasRasCompras]  WITH CHECK ADD  CONSTRAINT [LinhasRasCompras_CabecInternos_FK] FOREIGN KEY([IdCabecRasCompras])
REFERENCES [dbo].[CabecRasCompras] ([Id])
GO

ALTER TABLE [dbo].[LinhasRasCompras] CHECK CONSTRAINT [LinhasRasCompras_CabecInternos_FK]
GO

ALTER TABLE [dbo].[LinhasRasCompras]  WITH CHECK ADD  CONSTRAINT [LinhasRasCompras_COP_Obras_FK] FOREIGN KEY([ObraId])
REFERENCES [dbo].[COP_Obras] ([ID])
GO

ALTER TABLE [dbo].[LinhasRasCompras] CHECK CONSTRAINT [LinhasRasCompras_COP_Obras_FK]
GO


ALTER TABLE [dbo].[LinhasRasCompras]  WITH CHECK ADD  CONSTRAINT [LinhasRasCompras_Unidades_FK] FOREIGN KEY([Unidade])
REFERENCES [dbo].[Unidades] ([Unidade])
GO

ALTER TABLE [dbo].[LinhasRasCompras] CHECK CONSTRAINT [LinhasRasCompras_Unidades_FK]
GO




