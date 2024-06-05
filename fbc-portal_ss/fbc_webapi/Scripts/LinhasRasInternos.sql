
CREATE TABLE [dbo].[LinhasRasInternos](
	[Id] [uniqueidentifier] NOT NULL,
	[IdCabecRasInternos] [uniqueidentifier] NOT NULL,
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


 CONSTRAINT [LinhasRasInternos01] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LinhasRasInternos] ADD  CONSTRAINT [LinhasRasInternos_Id_DF]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[LinhasRasInternos] ADD  CONSTRAINT [LinhasRasInternos_NumLinha_DF]  DEFAULT ((0)) FOR [NumLinha]
GO

ALTER TABLE [dbo].[LinhasRasInternos] ADD  CONSTRAINT [LinhasRasInternos_Quantidade_DF]  DEFAULT (0) FOR [Quantidade]
GO

ALTER TABLE [dbo].[LinhasRasInternos] ADD  CONSTRAINT [LinhasRasInternos_Preco_DF]  DEFAULT (0) FOR [Preco]
GO

ALTER TABLE [dbo].[LinhasRasInternos] ADD  CONSTRAINT [LinhasRasInternos_Data_DF]  DEFAULT (getdate()) FOR [Data]
GO

ALTER TABLE [dbo].[LinhasRasInternos] ADD  CONSTRAINT [LinhasRasInternos_Anulado_DF]  DEFAULT (0) FOR [Anulado]
GO

ALTER TABLE [dbo].[LinhasRasInternos] ADD  CONSTRAINT [LinhasRasInternos_Fechado_DF]  DEFAULT (0) FOR [Fechado]
GO

ALTER TABLE [dbo].[LinhasRasInternos] ADD  CONSTRAINT [LinhasRasInternos_Total_DF]  DEFAULT (0) FOR [Total]
GO

ALTER TABLE [dbo].[LinhasRasInternos]  WITH CHECK ADD  CONSTRAINT [LinhasRasInternos_ArmazemLocalizacoes_FK] FOREIGN KEY([Armazem], [Localizacao])
REFERENCES [dbo].[ArmazemLocalizacoes] ([Armazem], [Localizacao])
GO

ALTER TABLE [dbo].[LinhasRasInternos] CHECK CONSTRAINT [LinhasRasInternos_ArmazemLocalizacoes_FK]
GO

ALTER TABLE [dbo].[LinhasRasInternos]  WITH CHECK ADD  CONSTRAINT [LinhasRasInternos_Armazens_FK] FOREIGN KEY([Armazem])
REFERENCES [dbo].[Armazens] ([Armazem])
GO

ALTER TABLE [dbo].[LinhasRasInternos] CHECK CONSTRAINT [LinhasRasInternos_Armazens_FK]
GO

ALTER TABLE [dbo].[LinhasRasInternos]  WITH CHECK ADD  CONSTRAINT [LinhasRasInternos_Artigo_FK] FOREIGN KEY([Artigo])
REFERENCES [dbo].[Artigo] ([Artigo])
GO

ALTER TABLE [dbo].[LinhasRasInternos] CHECK CONSTRAINT [LinhasRasInternos_Artigo_FK]
GO

ALTER TABLE [dbo].[LinhasRasInternos]  WITH CHECK ADD  CONSTRAINT [LinhasRasInternos_CabecInternos_FK] FOREIGN KEY([IdCabecRasInternos])
REFERENCES [dbo].[CabecRasInternos] ([Id])
GO

ALTER TABLE [dbo].[LinhasRasInternos] CHECK CONSTRAINT [LinhasRasInternos_CabecInternos_FK]
GO

ALTER TABLE [dbo].[LinhasRasInternos]  WITH CHECK ADD  CONSTRAINT [LinhasRasInternos_COP_Obras_FK] FOREIGN KEY([ObraId])
REFERENCES [dbo].[COP_Obras] ([ID])
GO

ALTER TABLE [dbo].[LinhasRasInternos] CHECK CONSTRAINT [LinhasRasInternos_COP_Obras_FK]
GO


ALTER TABLE [dbo].[LinhasRasInternos]  WITH CHECK ADD  CONSTRAINT [LinhasRasInternos_Unidades_FK] FOREIGN KEY([Unidade])
REFERENCES [dbo].[Unidades] ([Unidade])
GO

ALTER TABLE [dbo].[LinhasRasInternos] CHECK CONSTRAINT [LinhasRasInternos_Unidades_FK]
GO




