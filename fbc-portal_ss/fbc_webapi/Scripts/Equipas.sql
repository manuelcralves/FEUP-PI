

CREATE TABLE [dbo].[Equipas](
	[Codigo] [nvarchar](20) NOT NULL,
	[Descricao] [nvarchar](100) NULL,
	[Email] [nvarchar](255) NULL,
	[Activa] [bit] NOT NULL,
	[DataCriacao] [datetime] NULL,
	[CriadoPor] [varchar](20) NULL,
	[DataModificacao] [datetime] NULL,
	[ModificadoPor] [varchar](20) NULL,
 CONSTRAINT [PK_Equipas] PRIMARY KEY CLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Equipas] ADD  CONSTRAINT [Equipas_Activa_DF]  DEFAULT ((1)) FOR [Activa]
GO

ALTER TABLE [dbo].[Equipas] ADD  CONSTRAINT [Equipas_DataCriacao_DF]  DEFAULT (getdate()) FOR [DataCriacao]
GO


