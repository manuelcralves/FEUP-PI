

CREATE TABLE [dbo].[LeadersEquipa](
	[Id] [uniqueidentifier] NOT NULL,
	[CodEquipa] [nvarchar](20) NOT NULL,
	[Utilizador] [varchar](20) NOT NULL,
	[Nome] [nvarchar](100) NULL,

 CONSTRAINT [PK_LeadersEquipa] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[CodEquipa] ASC,
	[Utilizador] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LeadersEquipa] ADD  CONSTRAINT [LeadersEquipa_Id_DF]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[LeadersEquipa]  WITH CHECK ADD  CONSTRAINT [LeadersEquipa_Equipas_FK] FOREIGN KEY([CodEquipa])
REFERENCES [dbo].[Equipas] ([Codigo])
GO

ALTER TABLE [dbo].[LeadersEquipa]  WITH CHECK ADD  CONSTRAINT [LeadersEquipa_Utilizadores_FK] FOREIGN KEY([Utilizador])
REFERENCES [dbo].[Utilizadores] ([Codigo])
GO