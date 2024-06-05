using BasBE100;
using CmpBE100;
using IntBE100;
using PcmBE100;
using Primavera.Platform.Helpers;
using Serilog;
using StdBE100;
using StdPlatBE100;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using fbc_webapi.Classes.BusinessLogicLayer;
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;
using fbc_webapi.Primavera;
using UpgradeHelpers.DB;
using VndBE100;
using static StdPlatBE100.StdBETipos;

namespace fbc_webapi.Classes.DataAccessLayer
{
    public static class VendasDAL
    {


        public static async Task<List<TipoDocumentosVendas>> GetTiposDocumento(int? tipoTipoDocumento)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Documento, Descricao FROM DocumentosVenda";

                    if (tipoTipoDocumento != null)
                    {
                        cmd.CommandText += " AND TipoDocumento = @TipoDocumento";
                        cmd.Parameters.AddWithValue("@TipoDocumento", tipoTipoDocumento);
                    }

                    cmd.CommandText += " ORDER BY Descricao";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<TipoDocumentosVendas> tiposDoc = new List<TipoDocumentosVendas>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            TipoDocumentosVendas tipoDoc = new TipoDocumentosVendas
                            {
                                Codigo = (string)sqlDataReader["Documento"],
                                Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                            };

                            tiposDoc.Add(tipoDoc);
                        }

                        return tiposDoc;
                    }
                }
            }
        }

        public static async Task<TipoDocumentosVendas> GetTipoDocumento( string tipoDoc)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Documento, Descricao, TipoDocumento, DocNaoValorizado FROM DocumentosVenda WHERE Documento = @TipoDocumento";

                    cmd.Parameters.AddWithValue("@TipoDocumento", tipoDoc);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return null;

                        TipoDocumentosVendas tipoDocumentoVenda = new TipoDocumentosVendas
                        {
                            Codigo = (string)sqlDataReader["Documento"],
                            Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                            TipoTipoDocumento = sqlDataReader["TipoDocumento"] == DBNull.Value ? null : (byte?)sqlDataReader["TipoDocumento"],
                            DocumentoNaoValorizado = sqlDataReader["DocNaoValorizado"] == DBNull.Value ? null : (bool?)sqlDataReader["DocNaoValorizado"],
                        };

                        return tipoDocumentoVenda;
                    }
                }
            }
        }

        public static async Task<List<SerieVendas>> GetSeries(string tipoDocumento)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Serie, Descricao, SeriePorDefeito, Config, NumVias, Previsao FROM SeriesVendas WHERE TipoDoc = @TipoDoc ORDER BY Serie DESC";

                    cmd.Parameters.AddWithValue("@TipoDoc", tipoDocumento);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<SerieVendas> series = new List<SerieVendas>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            SerieVendas serie = new SerieVendas
                            {
                                Codigo = (string)sqlDataReader["Serie"],
                                Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                                SeriePorDefeito = (bool)sqlDataReader["SeriePorDefeito"],
                                ReportPorDefeito = sqlDataReader["Config"] == DBNull.Value ? null : (string)sqlDataReader["Config"],
                                Previsualizar = sqlDataReader["Previsao"] == DBNull.Value ? null : (bool?)sqlDataReader["Previsao"],
                            };

                            string numViasStr = sqlDataReader["NumVias"] == DBNull.Value ? null : (string)sqlDataReader["NumVias"];
                            if (!string.IsNullOrEmpty(numViasStr) && int.TryParse(numViasStr, out int nrViasInt))
                                serie.NumVias = nrViasInt;

                            series.Add(serie);
                        }

                        return series;
                    }
                }
            }
        }

        public static async Task GetDocumentos(SqlConnection conn, DocumentoVenda servico)
        {
            using (SqlCommand cmdDocumentosServicos = new SqlCommand())
            {
                cmdDocumentosServicos.Connection = conn;

                cmdDocumentosServicos.CommandText = @"
SELECT
	A.Id,
	A.FicheiroOrig,
    A.CDU_DocumentoMotorista,
    A.CDU_NumDocumentoMotorista,
    A.Data,
	DV.Descricao,
	DS.CDU_IdServico, 
	DS.CDU_DocumentoServico,
    DS.CDU_IdAnexoExterno
from TDU_DocumentosServicos DS
join Anexos A on A.CDU_NumDocumentoMotorista = DS.CDU_DocumentoServico
JOIN LinhasDoc LD on LD.Id = DS.CDU_IdServico
JOIN CabecDoc CD on CD.Id = LD.IdCabecDoc
JOIN DocumentosVenda DV on DV.Documento = CD.TipoDoc
where CD.Id = @IdServico
UNION
SELECT
	A.Id,
	A.FicheiroOrig,
    A.CDU_DocumentoMotorista,
    A.CDU_NumDocumentoMotorista,
    A.Data,
	NULL AS Descricao,
	DS.CDU_IdServico, 
	DS.CDU_DocumentoServico,
    DS.CDU_IdAnexoExterno
from TDU_DocumentosServicos DS
join Anexos A on A.Id = DS.CDU_IdAnexoExterno
JOIN LinhasDoc LD on LD.Id = DS.CDU_IdServico
JOIN CabecDoc CD on CD.Id = LD.IdCabecDoc
where CD.Id = @IdServico
";

                cmdDocumentosServicos.Parameters.AddWithValue("@IdServico", servico.Id);

                using (SqlDataReader sqlDataReader = await cmdDocumentosServicos.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                    {
                        var linha = servico.Linhas.First(f => f.Id == (Guid)sqlDataReader["CDU_IdServico"]);

                        Anexo anexo = new Anexo
                        {
                            Id = (Guid)sqlDataReader["Id"],
                            FicheiroOrig = sqlDataReader["FicheiroOrig"] == DBNull.Value ? null : (string)sqlDataReader["FicheiroOrig"],
                            DocumentoMotorista = (bool)sqlDataReader["CDU_DocumentoMotorista"],
                            NumDocumentoMotorista = sqlDataReader["CDU_NumDocumentoMotorista"] == DBNull.Value ? null : (string)sqlDataReader["CDU_NumDocumentoMotorista"],
                            Data = sqlDataReader["Data"] == DBNull.Value ? null : (DateTime?)sqlDataReader["Data"],
                            DescricaoDocumento = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"]
                        };
                    }
                }
            }
        }


        public static VndBEDocumentoVenda GetDocumentoVenda( string username, string password, Guid idDocumento)
        {
            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                VndBEDocumentoVenda documentoPrimavera = pri.BSO.Vendas.Documentos.EditaID(idDocumento.ToString());

                return documentoPrimavera;
            }
        }

        public static async Task<List<TipoDocumentosVendas>> GetTiposDocumentoEncomendaCliente()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Documento, Descricao FROM DocumentosVenda WHERE CDU_DispPortalParaEncomendas = 1 ORDER BY Descricao";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<TipoDocumentosVendas> tiposDoc = new List<TipoDocumentosVendas>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            TipoDocumentosVendas tipoDoc = new TipoDocumentosVendas
                            {
                                Codigo = (string)sqlDataReader["Documento"],
                                Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                            };

                            tiposDoc.Add(tipoDoc);
                        }

                        return tiposDoc;
                    }
                }
            }
        }

        public static async Task<int> GetProximoNumeroSerie( string tipoDocumento, string serie)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT (ISNULL(Numerador, 0) + 1) Numerador FROM SeriesVendas WHERE Serie = @Serie AND TipoDoc = @TipoDoc";

                    cmd.Parameters.AddWithValue("@TipoDoc", tipoDocumento);
                    cmd.Parameters.AddWithValue("@Serie", serie);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return 0;

                        int proximoNumero = (int)sqlDataReader["Numerador"];

                        return proximoNumero;
                    }
                }
            }
        }

        public static async Task<List<Report>> GetReportsVenda( string tipoDoc)
        {
            TipoDocumentosVendas tipoDocumentoVenda = await GetTipoDocumento(tipoDoc).ConfigureAwait(false);

            if (tipoDocumentoVenda == null)
                throw new FlorestasBemCuidadaWebApiException("Tipo de documento não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (tipoDocumentoVenda.TipoTipoDocumento == null)
                throw new FlorestasBemCuidadaWebApiException("Tipo de documento não tem o tipo configurado.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            string categoria = GetCategoriaDeTipoDocumentoVenda(tipoDocumentoVenda.TipoTipoDocumento.Value, tipoDocumentoVenda.DocumentoNaoValorizado ?? false);

            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT
	M.Mapa,
	M.Descricao
FROM
	Mapas M
WHERE
	M.Apl = 'VND'
	AND M.Categoria = @Categoria
	AND (M.Empresa = '' OR M.Empresa = @Empresa)
	AND M.Linha IN ('**', 'LP', 'LX')
ORDER BY
	M.Descricao
";

                    cmd.Parameters.AddWithValue("@Empresa", Config.Primavera_Empresa);
                    cmd.Parameters.AddWithValue("@Categoria", categoria);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<Report> reports = new List<Report>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            Report report = new Report
                            {
                                Mapa = (string)sqlDataReader["Mapa"],
                                Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                            };

                            reports.Add(report);
                        }

                        return reports;
                    }
                }
            }
        }

        private static string GetCategoriaDeTipoDocumentoVenda(byte tipoTipoDocumento, bool documentoNaoValorizado)
        {
            switch (tipoTipoDocumento)
            {
                case 0:
                    return "DocVndCt";
                case 2:
                    return "DocEncCl";
                case 3:
                    if (documentoNaoValorizado)
                        return "DocVNDNV";
                    return "DocVenda";
                default:
                    return "DocVenda";
            }
        }

        public static async Task<List<Vendedor>> GetVendedores()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Vendedor, Nome FROM Vendedores ORDER BY Nome";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<Vendedor> vendedores = new List<Vendedor>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            Vendedor vendedor = new Vendedor
                            {
                                Codigo = (string)sqlDataReader["Vendedor"],
                                Nome = sqlDataReader["Nome"] == DBNull.Value ? null : (string)sqlDataReader["Nome"],
                            };

                            vendedores.Add(vendedor);
                        }

                        return vendedores;
                    }
                }
            }
        }

        internal static async Task<Vendedor> GetVendedor( string vendedor)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Vendedor, Nome FROM Vendedores WHERE Vendedor = @Vendedor";

                    cmd.Parameters.AddWithValue("@Vendedor", vendedor);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return null;

                        Vendedor vnd = new Vendedor
                        {
                            Codigo = (string)sqlDataReader["Vendedor"],
                            Nome = sqlDataReader["Nome"] == DBNull.Value ? null : (string)sqlDataReader["Nome"],
                        };

                        return vnd;
                    }
                }
            }
        }


        public static async Task<List<DocumentoVenda>> GetPropostasAbertas(string utilizador, string menu)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
                                    SELECT
	                                    VD.Empresa,
	                                    E.IDNome,
	                                    VD.Id,
	                                    VD.Documento,
	                                    VD.TipoDoc,
	                                    VD.Serie,
	                                    VD.NumDoc,
	                                    VD.Entidade,
	                                    VD.TipoEntidade,
                                        VD.NumContribuinte,
                                        VD.RespCobranca,
                                        VD.NomeVendedor,
	                                    VD.Nome,
	                                    VD.Data,
	                                    VD.DataGravacao,
	                                    VD.Estado
	
                                    FROM
	                                    PNT_VIEW_COTDOCS VD
										LEFT JOIN PRIEMPRE.dbo.Empresas E ON E.Codigo = VD.Empresa
	                                    INNER JOIN TDU_PNT_EmpresasUtilizadores EU ON EU.CDU_CodigoEmpresa = VD.Empresa AND EU.CDU_Utilizador LIKE @Utilizador AND (EU.CDU_Inativado IS NULL OR EU.CDU_Inativado = 0)
                                    WHERE VD.CDU_MenuProposta = @MenuProposta
                                    ORDER BY
	                                    VD.DataGravacao DESC, VD.TipoDoc ASC, VD.Serie DESC, VD.NumDoc DESC
                                    ";

                    cmd.Parameters.AddWithValue("@Utilizador", utilizador);
                    cmd.Parameters.AddWithValue("@MenuProposta", menu);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<DocumentoVenda> documentos = new List<DocumentoVenda>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            DocumentoVenda documento = new DocumentoVenda
                            {
                                Id = (Guid)sqlDataReader["Id"],
                                Documento = sqlDataReader["Documento"] == DBNull.Value ? null : (string)sqlDataReader["Documento"],
                                TipoDoc = (string)sqlDataReader["TipoDoc"],
                                Serie = (string)sqlDataReader["Serie"],
                                NumDoc = (int)sqlDataReader["NumDoc"],
                                DataDoc = sqlDataReader["Data"] == DBNull.Value ? null : (DateTime?)sqlDataReader["Data"],
                                Entidade = sqlDataReader["Entidade"] == DBNull.Value ? null : (string)sqlDataReader["Entidade"],
                                Vendedor = sqlDataReader["RespCobranca"] == DBNull.Value ? null : (string)sqlDataReader["RespCobranca"],
                                NomeVendedor = sqlDataReader["NomeVendedor"] == DBNull.Value ? null : (string)sqlDataReader["NomeVendedor"],
                                NomeEntidade = sqlDataReader["Nome"] == DBNull.Value ? null : (string)sqlDataReader["Nome"],
                                Estado = sqlDataReader["Estado"] == DBNull.Value ? null : (string)sqlDataReader["Estado"],
                            };

                            documentos.Add(documento);
                        }

                        return documentos;
                    }
                }
            }
        }

        public static async Task<DocumentoVenda> GetPropostaAberta(string username, string password, Guid id, bool soCorpo = false)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                DocumentoVenda documento = null;

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT
	CD.Id,
	CD.Documento,
	CD.TipoDoc,
	CD.Serie,
	CD.NumDoc,
	CD.Data,
	CDS.Estado,
	CDS.Anulado,
	CDS.Fechado,
	CD.Entidade,
	CD.TipoEntidade,
	CD.Nome,
	V.Nome As NomeVendedor,
	CD.RespCobranca,
	CD.CondPag,
    CD.CDU_AcordoPagamento,
    CD.CDU_MenuProposta,
	CP.Descricao As NomeCondPag,
	C.LimiteCred,
	C.TotalDeb,
    CD.Zona,
    Z.Descricao  As ZonaDescricao,
	CD.Morada,
	CD.CodPostal,
	CD.CodPostalLocalidade,
	CD.CDU_TituloProposta,
	CD.CDU_ResponsabilidadeCliente,
	CD.CDU_PrazoExecucao,
	CD.CDU_DataInicio,
	CD.CDU_DataFim,
	CD.CDU_ValidadeProposta,
    CD.CDU_PrazoEntrega,
	CD.CDU_AoCuidadoDe,
	CD.CDU_OutrasInformacoes,
	CD.CDU_CaracteristicasTecnicas,
	CD.CDU_Responsabilidades,
	CD.CDU_Exclusoes,
	CD.CDU_TipoProposta,
	CD.CDU_DescricaoTipoServico,
    0 Rascunho

FROM
	CabecDoc CD
	INNER JOIN DocumentosVenda DV ON DV.Documento = CD.TipoDoc
	INNER JOIN SeriesVendas SV ON SV.TipoDoc = CD.TipoDoc AND SV.Serie = CD.Serie
	LEFT JOIN CabecDocStatus CDS ON CDS.IdCabecDoc = CD.Id
	LEFT JOIN Clientes C ON C.Cliente = CD.Entidade AND CD.TipoEntidade = 'C'
	LEFT JOIN Vendedores V ON CD.RespCobranca = V.Vendedor
    LEFT JOIN CondPag CP On CD.CondPag = CP.CondPag
    LEFT JOIN Zonas Z on Z.Zona = CD.Zona

WHERE
	CD.Id = @Id
	AND DV.CDU_TipoDocDPProposta = 1
	AND SV.CDU_SerieDPProposta = 1

UNION ALL

SELECT
	CD.Id,
	CD.Documento,
	CD.TipoDoc,
	CD.Serie,
	CD.NumDoc,
	CD.Data,
	'N' Estado,
	'FALSE' Anulado,
	'FALSE' Fechado,
	CD.Entidade,
	CD.TipoEntidade,
	CD.Nome,
	V.Nome As NomeVendedor,
	CD.RespCobranca,
	CD.CondPag,
    CD.CDU_AcordoPagamento,
    CD.CDU_MenuProposta,
	CP.Descricao As NomeCondPag,
	C.LimiteCred,
	C.TotalDeb,
    CD.Zona,
    Z.Descricao  As ZonaDescricao,
	CD.Morada,
	CD.CodPostal,
	CD.CodPostalLocalidade,
	CD.CDU_TituloProposta,
	CD.CDU_ResponsabilidadeCliente,
	CD.CDU_PrazoExecucao,
	CD.CDU_DataInicio,
	CD.CDU_DataFim,
	CD.CDU_ValidadeProposta,
    CD.CDU_PrazoEntrega,
	CD.CDU_AoCuidadoDe,
	CD.CDU_OutrasInformacoes,
	CD.CDU_CaracteristicasTecnicas,
	CD.CDU_Responsabilidades,
	CD.CDU_Exclusoes,
	CD.CDU_TipoProposta,
	CD.CDU_DescricaoTipoServico,
    1 Rascunho

FROM
	CabecDocRascunhos CD
	INNER JOIN DocumentosVenda DV ON DV.Documento = CD.TipoDoc
	INNER JOIN SeriesVendas SV ON SV.TipoDoc = CD.TipoDoc AND SV.Serie = CD.Serie
	LEFT JOIN Clientes C ON C.Cliente = CD.Entidade AND CD.TipoEntidade = 'C'
	LEFT JOIN Vendedores V ON CD.RespCobranca = V.Vendedor
    LEFT JOIN CondPag CP On CD.CondPag = CP.CondPag
    LEFT JOIN Zonas Z on Z.Zona = CD.Zona

WHERE
	CD.Id = @Id
	AND DV.CDU_TipoDocDPProposta = 1
	AND SV.CDU_SerieDPProposta = 1
";

                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            documento = new DocumentoVenda
                            {
                                Id = (Guid)sqlDataReader["Id"],
                                Documento = sqlDataReader["Documento"] == DBNull.Value ? null : (string)sqlDataReader["Documento"],
                                TipoDoc = (string)sqlDataReader["TipoDoc"],
                                Serie = (string)sqlDataReader["Serie"],
                                NumDoc = (int)sqlDataReader["NumDoc"],
                                DataDoc = sqlDataReader["Data"] == DBNull.Value ? null : (DateTime?)sqlDataReader["Data"],
                                Estado = sqlDataReader["Estado"] == DBNull.Value ? null : (string)sqlDataReader["Estado"],
                                Anulado = sqlDataReader["Anulado"] == DBNull.Value ? null : (bool?)sqlDataReader["Anulado"],
                                Fechado = sqlDataReader["Fechado"] == DBNull.Value ? null : (bool?)sqlDataReader["Fechado"],
                                Entidade = sqlDataReader["Entidade"] == DBNull.Value ? null : (string)sqlDataReader["Entidade"],
                                NomeEntidade = sqlDataReader["Nome"] == DBNull.Value ? null : (string)sqlDataReader["Nome"],
                                Vendedor = sqlDataReader["RespCobranca"] == DBNull.Value ? null : (string)sqlDataReader["RespCobranca"],
                                NomeVendedor = sqlDataReader["NomeVendedor"] == DBNull.Value ? null : (string)sqlDataReader["NomeVendedor"],
                                CondPag = sqlDataReader["CondPag"] == DBNull.Value ? null : (string)sqlDataReader["CondPag"],
                                NomeCondPag = sqlDataReader["NomeCondPag"] == DBNull.Value ? null : (string)sqlDataReader["NomeCondPag"],
                                LimiteCredito = sqlDataReader["LimiteCred"] == DBNull.Value ? null : (double?)sqlDataReader["LimiteCred"],
                                Credito = sqlDataReader["TotalDeb"] == DBNull.Value ? null : (double?)sqlDataReader["TotalDeb"],
                                Zona = sqlDataReader["Zona"] == DBNull.Value ? null : (string)sqlDataReader["Zona"],
                                ZonaDescricao = sqlDataReader["ZonaDescricao"] == DBNull.Value ? null : (string)sqlDataReader["ZonaDescricao"],
                                Morada = sqlDataReader["Morada"] == DBNull.Value ? null : (string)sqlDataReader["Morada"],
                                CodPostal = sqlDataReader["CodPostal"] == DBNull.Value ? null : (string)sqlDataReader["CodPostal"],
                                LocalidadeCodPostal = sqlDataReader["CodPostalLocalidade"] == DBNull.Value ? null : (string)sqlDataReader["CodPostalLocalidade"],

                                //DescricaoTipoServico = sqlDataReader["CDU_DescricaoTipoServico"] == DBNull.Value ? null : (string)sqlDataReader["CDU_DescricaoTipoServico"],

                                Rascunho = Convert.ToBoolean(sqlDataReader["Rascunho"])
                            };
                        }
                    }
                }

                if (documento == null)
                    return null;

                if (!soCorpo)
                {
                    await GetLinhasDocumentoVenda(id, conn, documento).ConfigureAwait(false);

                    await GetAnexosDocumentoVenda(id, conn, documento, username, password).ConfigureAwait(false);
         
                }

                return documento;
            }
        }



        public static async Task GetLinhaDocumentoVenda(Guid id, Guid idLinha, SqlConnection conn, DocumentoVenda documento)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                cmd.CommandText = @"
SELECT
	LD.Id,
	LD.NumLinha,
	LD.Artigo,
	LD.Descricao,
	LD.PrecUnit,
	LD.Quantidade,
	LD.TotalIliquido,
	LD.DataEntrega,
	LD.CDU_MoradaAlternativa,
	LD.CDU_MoradaAlternativaDescricao,
    LD.CDU_Origem,
	LD.CDU_Destino,
    LD.CDU_LocalCarga,
	LD.CDU_MoradaCarga,
	LD.CDU_LocalDescarga,
	LD.CDU_MoradaDescarga,
	LD.CDU_Transportador,
	LD.CDU_CodigoLer,
	LD.CDU_CodigoLerDescricao,
	LD.CDU_AplicavelQtd,
	LD.CDU_AplicavelValor,
	LD.CDU_Operacao,
	LD.CDU_Observacoes,
	LD.CDU_EstadoAgendamento,
    LD.CDU_Origem,
	LD.CDU_DataHoraServico,
	LD.CDU_DataHoraFimServico,
    LD.CDU_PrecoCompra,
    LD.CDU_Viatura,
	MAC.eGAR_CodigoAPA,
	VA.ArtigoPaiZonas,
    LD.CDU_Volumetria,
    LD.CDU_VolumetriaDescricao,
    LD.CDU_Periodicidade,
    LD.CDU_Carencia

FROM
	LinhasDoc LD
	INNER JOIN CabecDoc CD ON CD.Id = LD.IdCabecDoc
	LEFT JOIN MoradasAlternativasClientes MAC ON MAC.MoradaAlternativa = LD.CDU_MoradaAlternativa AND MAC.Cliente = CD.Entidade AND CD.TipoEntidade = 'C'
	LEFT JOIN PNT_VIEW_Artigo VA ON VA.Artigo = LD.Artigo

WHERE
	LD.IdCabecDoc = @IdCabecDoc
AND LD.Id = @IdLinha

ORDER BY
	LD.NumLinha
";

                cmd.Parameters.AddWithValue("@IdCabecDoc", id);
                cmd.Parameters.AddWithValue("@IdLinha", idLinha);

                using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                    {

                        if (documento.Linhas.Exists(l => l.Id == idLinha))
                            continue;

                        LinhaDocumentoVenda linha = new LinhaDocumentoVenda
                        {
                            Id = idLinha,
                            NumLinha = (short)sqlDataReader["NumLinha"],
                            Artigo = sqlDataReader["Artigo"] == DBNull.Value ? null : (string)sqlDataReader["Artigo"],
                            Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                            Preco = sqlDataReader["PrecUnit"] == DBNull.Value ? null : (double?)sqlDataReader["PrecUnit"],                            
                            Quantidade = sqlDataReader["Quantidade"] == DBNull.Value ? null : (double?)sqlDataReader["Quantidade"],
                            Total = sqlDataReader["TotalIliquido"] == DBNull.Value ? null : (double?)sqlDataReader["TotalIliquido"],
                            DataEntrega = sqlDataReader["DataEntrega"] == DBNull.Value ? null : (DateTime?)sqlDataReader["DataEntrega"],
                            Observacoes = sqlDataReader["CDU_Observacoes"] == DBNull.Value ? null : (string)sqlDataReader["CDU_Observacoes"],
                        };

                        documento.Linhas.Add(linha);
                    }
                }
            }
        }

        private static async Task GetLinhasDocumentoVenda(Guid id, SqlConnection conn, DocumentoVenda documento)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                cmd.CommandText = @"
SELECT
	LD.Id,
	LD.NumLinha,
	LD.Artigo,
	LD.Descricao,
	LD.PrecUnit,
	LD.Quantidade,
	LD.Desconto1,
    LD.Unidade,
	LD.TotalIliquido,
	LD.DataEntrega,
	LD.CDU_MoradaAlternativa,
	LD.CDU_MoradaAlternativaDescricao,
	LD.CDU_LocalCarga,
	LD.CDU_MoradaCarga,
	LD.CDU_LocalDescarga,
	LD.CDU_MoradaDescarga,
    LD.CDU_Origem,
    ZO.Descricao As OrigemDescricao,
    LD.CDU_Destino,
    ZD.Descricao As DestinoDescricao,
	LD.CDU_DataHoraServico,
	LD.CDU_DataHoraFimServico,
	LD.CDU_Transportador,
	LD.CDU_CodigoLer,
	LD.CDU_CodigoLerDescricao,
	LD.CDU_AplicavelQtd,
	LD.CDU_AplicavelValor,
	LD.CDU_Operacao,
	LD.CDU_Observacoes,
    LD.CDU_ObservacoesAgendamento,
	LD.CDU_EstadoAgendamento,
    LD.CDU_PrecoCompra,
	MAC.eGAR_CodigoAPA,
	VA.ArtigoPaiZonas,
    LD.CDU_Viatura,
    LD.CDU_Volumetria,
    LD.CDU_VolumetriaDescricao,
    LD.CDU_EstadoServico,
    LD.CDU_Tarefa,
    LD.CDU_Motorista,
    LD.CDU_EmpresaMotorista,
    LD.CDU_Periodicidade,
    LD.CDU_Carencia,
    LD.CDU_ArtigoServico,
    LD.CDU_ArmCarga,
    LD.CDU_ArmDescarga,
	A.Descricao AS ArtigoServicoDescricao,
	A.CodBarras As ArtigoServicoNumero,
    LD.CDU_MovimentaStock,
    LD.CDU_DocumentoServico,
    LD.CDU_TipoServico
FROM
	LinhasDoc LD
	INNER JOIN CabecDoc CD ON CD.Id = LD.IdCabecDoc
	LEFT JOIN MoradasAlternativasClientes MAC ON MAC.MoradaAlternativa = LD.CDU_MoradaAlternativa AND MAC.Cliente = CD.Entidade AND CD.TipoEntidade = 'C'
	LEFT JOIN PNT_VIEW_Artigo VA ON VA.Artigo = LD.Artigo
	LEFT JOIN Artigo A ON A.Artigo = LD.CDU_ArtigoServico
    LEFT JOIN Zonas ZO on ZO.Zona = LD.CDU_Origem
    LEFT JOIN Zonas ZD on ZD.Zona = LD.CDU_Destino

WHERE
	LD.IdCabecDoc = @IdCabecDoc

UNION ALL

	SELECT
	LD.Id,
	LD.NumLinha,
	LD.Artigo,
	LD.Descricao,
	LD.PrecUnit,
	LD.Quantidade,
	LD.Desconto1,
    LD.Unidade,
	LD.TotalIliquido,
	LD.DataEntrega,
	LD.CDU_MoradaAlternativa,
	LD.CDU_MoradaAlternativaDescricao,
	LD.CDU_LocalCarga,
	LD.CDU_MoradaCarga,
	LD.CDU_LocalDescarga,
	LD.CDU_MoradaDescarga,
    LD.CDU_Origem,
    ZO.Descricao As OrigemDescricao,
    LD.CDU_Destino,
    ZD.Descricao As DestinoDescricao,
	LD.CDU_DataHoraServico,
	LD.CDU_DataHoraFimServico,
	LD.CDU_Transportador,
	LD.CDU_CodigoLer,
	LD.CDU_CodigoLerDescricao,
	LD.CDU_AplicavelQtd,
	LD.CDU_AplicavelValor,
	LD.CDU_Operacao,
	LD.CDU_Observacoes,
    LD.CDU_ObservacoesAgendamento,
	LD.CDU_EstadoAgendamento,
    LD.CDU_PrecoCompra,
	MAC.eGAR_CodigoAPA,
	VA.ArtigoPaiZonas,
    LD.CDU_Viatura,
    LD.CDU_Volumetria,
    LD.CDU_VolumetriaDescricao,
    LD.CDU_EstadoServico,
    LD.CDU_Tarefa,
    LD.CDU_Motorista,
    LD.CDU_EmpresaMotorista,
    LD.CDU_Periodicidade,
    LD.CDU_Carencia,
    LD.CDU_ArtigoServico,
    LD.CDU_ArmCarga,
    LD.CDU_ArmDescarga,
	A.Descricao AS ArtigoServicoDescricao,
	A.CodBarras As ArtigoServicoNumero,
    LD.CDU_MovimentaStock,
    LD.CDU_DocumentoServico,
    LD.CDU_TipoServico
FROM
	LinhasDocRascunhos LD
	INNER JOIN CabecDocRascunhos CD ON CD.Id = LD.IdCabecDoc
	LEFT JOIN MoradasAlternativasClientes MAC ON MAC.MoradaAlternativa = LD.CDU_MoradaAlternativa AND MAC.Cliente = CD.Entidade AND CD.TipoEntidade = 'C'
	LEFT JOIN PNT_VIEW_Artigo VA ON VA.Artigo = LD.Artigo
	LEFT JOIN Artigo A ON A.Artigo = LD.CDU_ArtigoServico
    LEFT JOIN Zonas ZO on ZO.Zona = LD.CDU_Origem
    LEFT JOIN Zonas ZD on ZD.Zona = LD.CDU_Destino

WHERE
	LD.IdCabecDoc = @IdCabecDoc

ORDER BY
	NumLinha

";

                cmd.Parameters.AddWithValue("@IdCabecDoc", id);

                using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                    {
                        Guid idLinha = (Guid)sqlDataReader["Id"];

                        if (documento.Linhas.Exists(l => l.Id == idLinha))
                            continue;

                        LinhaDocumentoVenda linha = new LinhaDocumentoVenda
                        {
                            Id = idLinha,
                            NumLinha = (short)sqlDataReader["NumLinha"],
                            Artigo = sqlDataReader["Artigo"] == DBNull.Value ? null : (string)sqlDataReader["Artigo"],
                            Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                            Preco = sqlDataReader["PrecUnit"] == DBNull.Value ? null : (double?)sqlDataReader["PrecUnit"],
                            Desconto = (float)sqlDataReader["Desconto1"],                        
                            Quantidade = sqlDataReader["Quantidade"] == DBNull.Value ? null : (double?)sqlDataReader["Quantidade"],
                            Unidade = sqlDataReader["Unidade"] == DBNull.Value ? null : (string)sqlDataReader["Unidade"],
                            Total = sqlDataReader["TotalIliquido"] == DBNull.Value ? null : (double?)sqlDataReader["TotalIliquido"],
                            DataEntrega = sqlDataReader["DataEntrega"] == DBNull.Value ? null : (DateTime?)sqlDataReader["DataEntrega"],                        
                            Observacoes = sqlDataReader["CDU_Observacoes"] == DBNull.Value ? null : (string)sqlDataReader["CDU_Observacoes"],
                        };

                        documento.Linhas.Add(linha);
                    }
                }
            }
        }

        public static async Task GetLinhasDocumentoVenda(Guid id, DocumentoVenda documento)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                await GetLinhasDocumentoVenda(id, conn, documento).ConfigureAwait(false);
            }
        }

        private static async Task GetAnexosDocumentoVenda(Guid id, SqlConnection conn, DocumentoVenda documento, string username, string password)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                cmd.CommandText = @"
SELECT
	A.Id,
	A.FicheiroOrig
	
FROM
	Anexos A

WHERE
	A.Chave = @IdCabecDoc
	AND A.Tabela = 41

ORDER BY
	A.Data
";

                cmd.Parameters.AddWithValue("@IdCabecDoc", id);

                using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                    {
                        Anexo anexo = new Anexo
                        {
                            Id = (Guid)sqlDataReader["Id"],
                            FicheiroOrig = sqlDataReader["FicheiroOrig"] == DBNull.Value ? null : (string)sqlDataReader["FicheiroOrig"],
                        };

                        documento.Anexos.Add(anexo);
                    }

                    PreencherTamanhoAnexos(documento, username, password);
                }
            }
        }

        private static void PreencherTamanhoAnexos(DocumentoVenda documento, string username, string password)
        {
            try
            {
                using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirPSO: true))
                {
                    foreach (Anexo anexo in documento.Anexos)
                    {
                        try
                        {
                            string pathFicheiroAnexo = pri.PSO.Anexos.DaPercursoAnexo(anexo.Id.ToString(), null, anexo.FicheiroOrig);

                            if (File.Exists(pathFicheiroAnexo))
                                anexo.TamanhoBytes = new FileInfo(pathFicheiroAnexo).Length;
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Erro a consultar tamanho de ficheiro de anexo '{IdAnexo}' ('{NomeAnexo}') de documento '{Documento}'.", anexo.Id, anexo.FicheiroOrig, documento.Documento);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro a consultar tamanho de ficheiros de anexos de documento '{Documento}'.", documento.Documento);
            }
        }


        public static void AnulaDocumento( string username, string password, Guid id, EstornoDocumento dadosEstorno)
        {
            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                if (!pri.BSO.Vendas.Documentos.ExisteID(id.ToString()))
                    throw new FlorestasBemCuidadaWebApiException("Documento não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

                AnulaDocumento(pri, id.ToString(), dadosEstorno);
            }
        }


        private static void AnulaDocumento(PrimaveraConnection pri, string id, EstornoDocumento dadosEstorno)
        {
            string avisos = "";
            pri.BSO.Vendas.Documentos.AnulaDocumentoID(id, dadosEstorno.MotivoEstorno, dadosEstorno.Observacoes, ref avisos);

            if (!string.IsNullOrEmpty(avisos))
                Log.Warning("Avisos ao anular documento '{Id}' em '{Empresa}': {Avisos}", id, pri.BSO.Contexto.CodEmp, avisos);
        }

        public static void ApagarDocumento( string username, string password, Guid id)
        {
            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                if (!pri.BSO.Vendas.Documentos.ExisteID(id.ToString()))
                    throw new FlorestasBemCuidadaWebApiException("Documento não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

                StdBECampos campos = pri.BSO.Vendas.Documentos.DaValorAtributosID(id.ToString(), "Filial", "TipoDoc", "Serie", "NumDoc");

                string filial = campos["Filial"].Valor as string;
                string tipoDoc = campos["TipoDoc"].Valor as string;
                string serie = campos["Serie"].Valor as string;
                int numDoc = campos["NumDoc"].Valor as int? ?? 0;

                pri.BSO.Vendas.Documentos.Remove(filial, tipoDoc, serie, numDoc);
            }
        }

        public static void GravarAnexosDocumento( string username, string password, Guid id, List<Anexo> anexos, List<string> ficheirosAnexos)
        {
            List<string> ficheirosAnexosGravados = new List<string>();

            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                try
                {
                    string idDocumentoStr = id.ToString();

                    pri.BSO.IniciaTransaccao();

                    if (!pri.BSO.Vendas.Documentos.ExisteID(idDocumentoStr))
                        throw new FlorestasBemCuidadaWebApiException("Documento não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

                    GravarAnexosDocumento(pri, idDocumentoStr, anexos, ficheirosAnexos, ficheirosAnexosGravados);

                    pri.BSO.TerminaTransaccao();
                }
                catch (Exception)
                {
                    pri.BSO.DSO.Plat.DesfazTransaccao();
                    pri.BSO.DesfazTransaccao();

                    Utils.TentarApagarFicheiros(ficheirosAnexosGravados);

                    throw;
                }
            }
        }

        public static async Task AlterarProposta( string username, string password, Guid id, DocumentoVenda documentoModel, List<string> ficheirosAnexos)
        {
            List<string> ficheirosAnexosGravados = new List<string>();

            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                try
                {
                    pri.BSO.IniciaTransaccao();

                    VndBEDocumentoVenda documentoPrimavera = pri.BSO.Vendas.Documentos.EditaID(id.ToString());

                    bool rascunho = false;

                    if (documentoPrimavera == null)
                    {
                        documentoPrimavera = pri.BSO.Vendas.Documentos.EditaRascunhoID(id.ToString());

                        rascunho = true;
                    }

                    string estadoOriginal = documentoPrimavera.Estado;

                    var estadoEnviado = documentoModel.Estado;

                    if (estadoEnviado?.Equals("D", StringComparison.OrdinalIgnoreCase) == true && !string.Equals(estadoOriginal, documentoModel.Estado, StringComparison.OrdinalIgnoreCase))
                    {
                        documentoModel.Estado = "G";

                        if (rascunho == true)
                        {
                            documentoModel.Estado = "N";
                        }

                        await CriarProposta(pri,username, password, documentoModel, ficheirosAnexos).ConfigureAwait(false);

                        documentoModel.Estado = estadoOriginal;
                    }

                    if (estadoEnviado?.Equals("S", StringComparison.OrdinalIgnoreCase) == true && !string.Equals(estadoOriginal, documentoModel.Estado, StringComparison.OrdinalIgnoreCase))
                    {

                        if (documentoModel.DadosEstorno != null && !string.IsNullOrEmpty(documentoModel.DadosEstorno.MotivoEstorno))
                        {
                            AnulaDocumento(pri, documentoPrimavera.ID, documentoModel.DadosEstorno);

                            if (estadoEnviado.Equals("S", StringComparison.OrdinalIgnoreCase) == true)
                                pri.BSO.DSO.Plat.Registos.ExecutaComando("UPDATE CabecDocStatus SET Estado = 'S' WHERE IdCabecDoc = @Id", new List<SqlParameter>() { new SqlParameter("@Id", documentoPrimavera.ID) });
                        }

                        documentoModel.Estado = documentoPrimavera.Estado;

                        documentoModel.DadosEstorno = null;

                        await CriarProposta(pri, username, password, documentoModel, ficheirosAnexos).ConfigureAwait(false);
                    }

                    if (!estadoEnviado.Equals("S") && !estadoOriginal.Equals("R"))
                    {

                        if (documentoPrimavera == null)
                            throw new FlorestasBemCuidadaWebApiException("Documento não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);


                        DefinirCamposDoc(documentoModel, pri, documentoPrimavera);

                        List<String> linhasApagadas = new List<string>();

                        if (documentoModel.Linhas != null)
                        {

                            // apagar linhas
                            for (int i = documentoPrimavera.Linhas.NumItens; i >= 1; i--)
                            {
                                VndBELinhaDocumentoVenda linhaPrimavera = documentoPrimavera.Linhas.GetEdita(i);

                                // se linha não existir no pedido enviado, é porque deve ser apagada
                                if (!documentoModel.Linhas.Any(l => l.Id == new Guid(linhaPrimavera.IdLinha)))
                                {
                                    linhasApagadas.Add(documentoPrimavera.Linhas.GetEdita(i).IdLinha);

                                    documentoPrimavera.Linhas.Remove(i);
                                }
                            }

                            // alterar linhas existentes
                            foreach (LinhaDocumentoVenda linhaModel in documentoModel.Linhas)
                            {
                                if (linhaModel.Id != null)
                                {
                                    VndBELinhaDocumentoVenda linhaPrimavera = documentoPrimavera.Linhas.Where(l => new Guid(l.IdLinha) == linhaModel.Id).FirstOrDefault();

                                    // se linha enviada tem ID definido mas documento Primavera não tem linha com esse ID, é porque foi definido manualmente por utilizador
                                    // limpar/ignorar ID para adicionar mais tarde
                                    if (linhaPrimavera == null)
                                    {
                                        linhaModel.Id = null;
                                        linhaModel.NumLinha = null;
                                        continue;
                                    }

                                    await DefinirCamposLinha(linhaModel, pri, documentoPrimavera, linhaPrimavera);

                                    // atualizar NumLinha do model se Primavera o tiver alterado
                                    linhaModel.NumLinha = linhaPrimavera.NumLinha;
                                }
                            }

                            // adicionar linhas novas
                            foreach (LinhaDocumentoVenda linhaModel in documentoModel.Linhas)
                            {
                                if (linhaModel.Id == null)
                                    await AdicionarLinha(linhaModel, pri, documentoPrimavera, username, password);
                            }

                            // reordenar linhas Primavera pela mesma ordem que Model
                            List<VndBELinhaDocumentoVenda> backupLinhasPrimavera = documentoPrimavera.Linhas.ToList();
                            List<string> backupLinhasRemovidas = documentoPrimavera.Linhas.Removidas.ToList();

                            documentoPrimavera.Linhas.RemoveTodos();
                            documentoPrimavera.Linhas.Removidas.RemoveTodos();

                            foreach (string linhaRemovida in backupLinhasRemovidas)
                                documentoPrimavera.Linhas.Removidas.Insere(linhaRemovida);

                            foreach (LinhaDocumentoVenda linhaModel in documentoModel.Linhas)
                            {
                                VndBELinhaDocumentoVenda linhaPrimavera = backupLinhasPrimavera.Where(l => new Guid(l.IdLinha) == linhaModel.Id).First();

                                documentoPrimavera.Linhas.Insere(linhaPrimavera);
                            }
                        }

                        if (documentoModel.Estado == "N")
                        {
                            pri.BSO.Vendas.Documentos.ActualizaRascunho(documentoPrimavera);
                        }
                        else
                        {
                            pri.BSO.Vendas.Documentos.Actualiza(documentoPrimavera);
                        }

                        pri.BSO.DSO.Plat.Registos.ExecutaComando("UPDATE CabecDoc SET AlteradoPorPortalTransucatas = GETDATE() WHERE Id = @Id", new List<SqlParameter>() { new SqlParameter("@Id", documentoPrimavera.ID) });

                        GravarAnexosDocumento(pri, documentoPrimavera.ID.Trim(new char[] { '{', '}' }), documentoModel.Anexos, ficheirosAnexos, ficheirosAnexosGravados);

                        if (documentoModel.DadosEstorno != null && !string.IsNullOrEmpty(documentoModel.DadosEstorno.MotivoEstorno))
                        {
                            AnulaDocumento(pri, documentoPrimavera.ID, documentoModel.DadosEstorno);
                        }
                       
                    }

                    pri.BSO.TerminaTransaccao();
                }
                catch (SqlException exception)
                {
                    pri.BSO.DSO.Plat.DesfazTransaccao();
                    pri.BSO.DesfazTransaccao();

                    Utils.TentarApagarFicheiros(ficheirosAnexosGravados);

                    if (exception.Number == 2601)
                    {
                        throw new FlorestasBemCuidadaWebApiException($"As regras de preço estão duplicadas.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);
                    }

                    throw;
                }
                catch (Exception)
                {
                    pri.BSO.DSO.Plat.DesfazTransaccao();
                    pri.BSO.DesfazTransaccao();

                    Utils.TentarApagarFicheiros(ficheirosAnexosGravados);

                    throw;
                }
            }
        }


        public static async Task<Guid> CriarProposta(PrimaveraConnection pri,  string username, string password, DocumentoVenda documentoModel, List<string> ficheirosAnexos)
        {
            List<string> ficheirosAnexosGravados = new List<string>();

            try
            {
                pri.BSO.IniciaTransaccao();

                VndBEDocumentoVenda documento = await CriarDocumento(pri, documentoModel, ficheirosAnexos, ficheirosAnexosGravados, username, password);

                pri.BSO.TerminaTransaccao();

                return new Guid(documento.ID);
            }
            catch (Exception)
            {
                pri.BSO.DSO.Plat.DesfazTransaccao();
                pri.BSO.DesfazTransaccao();

                Utils.TentarApagarFicheiros(ficheirosAnexosGravados);

                throw;
            }
        }


        private static async Task<VndBEDocumentoVenda> CriarDocumento(PrimaveraConnection pri, DocumentoVenda documentoModel,  List<string> ficheirosAnexos, List<string> ficheirosAnexosGravados, string username, string password)
        {
            VndBEDocumentoVenda documentoPrimavera = new VndBEDocumentoVenda();

            DefinirCamposDoc(documentoModel, pri, documentoPrimavera);

            foreach (LinhaDocumentoVenda linhaModel in documentoModel.Linhas)
                await AdicionarLinha(linhaModel, pri, documentoPrimavera, username, password);

            if (documentoModel.Estado == "N")
            {
                pri.BSO.Vendas.Documentos.ActualizaRascunho(documentoPrimavera);
            }
            else
            {
                pri.BSO.Vendas.Documentos.Actualiza(documentoPrimavera);
            }

            pri.BSO.DSO.Plat.Registos.ExecutaComando("UPDATE CabecDoc SET CriadoPorPortalTransucatas = 1 WHERE Id = @Id", new List<SqlParameter>() { new SqlParameter("@Id", documentoPrimavera.ID) });

            GravarAnexosDocumento(pri, documentoPrimavera.ID.Trim(new char[] { '{', '}' }), documentoModel.Anexos, ficheirosAnexos, ficheirosAnexosGravados);

            if (documentoModel.Estado?.Equals("S", StringComparison.OrdinalIgnoreCase) == true)
                DuplicarDocumento(pri, documentoPrimavera);

            if (documentoModel.DadosEstorno != null && !string.IsNullOrEmpty(documentoModel.DadosEstorno.MotivoEstorno))
            {
                AnulaDocumento(pri, documentoPrimavera.ID, documentoModel.DadosEstorno);

                if (documentoModel.Estado?.Equals("S", StringComparison.OrdinalIgnoreCase) == true)
                    pri.BSO.DSO.Plat.Registos.ExecutaComando("UPDATE CabecDocStatus SET Estado = 'S' WHERE IdCabecDoc = @Id", new List<SqlParameter>() { new SqlParameter("@Id", documentoPrimavera.ID) });
            }

            return documentoPrimavera;
        }

        public static void DefinirCamposDoc(DocumentoVenda documentoModel, PrimaveraConnection pri, VndBEDocumentoVenda documentoPrimavera)
        {
            if (!documentoPrimavera.EmModoEdicao)
            {
                documentoPrimavera.Tipodoc = documentoModel.TipoDoc;
                documentoPrimavera.Serie = documentoModel.Serie;
            }

            documentoPrimavera.Entidade = documentoModel.Entidade;

            documentoPrimavera.Rascunho = documentoModel.Rascunho;

            if (!documentoPrimavera.EmModoEdicao)
                pri.BSO.Vendas.Documentos.PreencheDadosRelacionados(documentoPrimavera);

            if (documentoModel.DataDoc != null)
            {
                DateTime dataDoc = documentoModel.DataDoc.Value;

                if (documentoPrimavera.EmModoEdicao)
                    dataDoc = new DateTime(dataDoc.Year, dataDoc.Month, dataDoc.Day, documentoPrimavera.DataDoc.Hour, documentoPrimavera.DataDoc.Minute, documentoPrimavera.DataDoc.Second);

                documentoPrimavera.DataDoc = dataDoc;
            }

            if (!string.IsNullOrEmpty(documentoModel.Vendedor) && !pri.BSO.Vendas.Vendedores.Existe(documentoModel.Vendedor))
                throw new FlorestasBemCuidadaWebApiException($"O vendedor '{documentoModel.Vendedor}' não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (!string.IsNullOrEmpty(documentoModel.CondPag) && !pri.BSO.Base.CondsPagamento.Existe(documentoModel.CondPag))
                throw new FlorestasBemCuidadaWebApiException($"A condição de pagamento '{documentoModel.CondPag}' não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (documentoModel.Zona != null)
                documentoPrimavera.Zona = documentoModel.Zona;

            if (documentoModel.Vendedor != null)
                documentoPrimavera.Responsavel = documentoModel.Vendedor;

            if (documentoModel.CondPag != null)
                documentoPrimavera.CondPag = documentoModel.CondPag;

            if (documentoModel.Morada != null)
                documentoPrimavera.Morada = documentoModel.Morada;

            if (documentoModel.Morada != null)
                documentoPrimavera.Morada = documentoModel.Morada;

            if (documentoModel.CodPostal != null)
                documentoPrimavera.CodigoPostal = documentoModel.CodPostal;

            if (documentoModel.LocalidadeCodPostal != null)
                documentoPrimavera.LocalidadeCodigoPostal = documentoModel.LocalidadeCodPostal;

            if (documentoModel.Observacoes != null)
                documentoPrimavera.Observacoes = documentoModel.Observacoes;

            if (documentoModel.ModoExpedicao != null)
            {
                documentoPrimavera.ModoExp = documentoModel.ModoExpedicao;
            }

            if (documentoModel.Estado != null && (documentoModel.DadosEstorno == null || string.IsNullOrEmpty(documentoModel.DadosEstorno.MotivoEstorno)))
                documentoPrimavera.Estado = documentoModel.Estado;

            if (!documentoPrimavera.Fechado && documentoPrimavera.Estado?.Equals("F", StringComparison.OrdinalIgnoreCase) == true)
                documentoPrimavera.Fechado = true;

            //DefinirCDU(documentoPrimavera.CamposUtil, "CDU_ZonaCliente", documentoModel.Zona);

        }

        private static async Task AdicionarLinha(LinhaDocumentoVenda linhaModel, PrimaveraConnection pri, VndBEDocumentoVenda documentoPrimavera, string username, string password)
        { 
            double quantidade = 0;
            string armazem = "";
            string localizacao = "";

            if (linhaModel.Quantidade != null)
                quantidade = linhaModel.Quantidade.Value;

            pri.BSO.Vendas.Documentos.AdicionaLinha(documentoPrimavera, linhaModel.Artigo, ref quantidade, ref armazem, ref localizacao, linhaModel.Preco ?? 0, linhaModel.Desconto ?? 0);

            VndBELinhaDocumentoVenda linhaPrimavera = documentoPrimavera.Linhas.GetEdita(documentoPrimavera.Linhas.NumItens);

            await DefinirCamposLinha(linhaModel, pri, documentoPrimavera, linhaPrimavera);

            if (!string.IsNullOrEmpty(linhaPrimavera.IdLinha))
                linhaModel.Id = new Guid(linhaPrimavera.IdLinha);

            linhaModel.NumLinha = documentoPrimavera.Linhas.NumItens;
        }

        public static async Task DefinirCamposLinha(LinhaDocumentoVenda linhaModel, PrimaveraConnection pri, VndBEDocumentoVenda documentoPrimavera, VndBELinhaDocumentoVenda linhaPrimavera)
        {
            if (documentoPrimavera.EmModoEdicao)
            {
                linhaPrimavera.Artigo = linhaModel.Artigo;

                if (linhaModel.Preco != null)
                    linhaPrimavera.PrecUnit = linhaModel.Preco.Value;

                if (linhaModel.Desconto != null)
                    linhaPrimavera.Desconto1 = linhaModel.Desconto.Value;

                if (linhaModel.Quantidade != null)
                    linhaPrimavera.Quantidade = linhaModel.Quantidade.Value;
            }

            if (linhaModel.Descricao != null)
                linhaPrimavera.Descricao = linhaModel.Descricao;

            if (!string.IsNullOrEmpty(linhaModel.Unidade))
                linhaPrimavera.Unidade = linhaModel.Unidade;

            if (linhaModel.DataEntrega != null)
                linhaPrimavera.DataEntrega = (DateTime)linhaModel.DataEntrega;

            if (linhaModel.ObraId != null)
                linhaPrimavera.IDObra = linhaModel.ObraId;


            /*DefinirCDU(linhaPrimavera.CamposUtil, "CDU_MoradaAlternativa", linhaModel.MoradaAlternativa);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_MoradaAlternativaDescricao", linhaModel.MoradaAlternativaDescricao);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_LocalCarga", linhaModel.LocalCarga);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_MoradaCarga", linhaModel.MoradaCarga);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_LocalDescarga", linhaModel.LocalDescarga);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_MoradaDescarga", linhaModel.MoradaCarga);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Origem", linhaModel.Origem);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Destino", linhaModel.Destino);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Transportador", linhaModel.Transportador);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Periodicidade", linhaModel.Periodicidade);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Carencia", linhaModel.Carencia);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Operacao", linhaModel.Operacao);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Tarefa", linhaModel.Tarefa);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_CodigoLer", linhaModel.CodigoLer);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_CodigoLerDescricao", linhaModel.CodigoLerDescricao);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_AplicavelQtd", linhaModel.AplicavelQtd);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_AplicavelValor", linhaModel.AplicavelValor);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Observacoes", linhaModel.Observacoes);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_PrecoCompra", linhaModel.Despesa);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_DataHoraServico", linhaModel.DataHoraServico);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_DataHoraFimServico", linhaModel.DataHoraFimServico);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_EstadoAgendamento", linhaModel.EstadoAgendamento);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Motorista", linhaModel.Motorista);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_EmpresaMotorista", linhaModel.EmpresaMotorista);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Viatura", linhaModel.Viatura);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Volumetria", linhaModel.Volumetria);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_VolumetriaDescricao", linhaModel.VolumetriaDescricao);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_NotificarMotorista", linhaModel.NotificarMotorista);
            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_ObservacoesAgendamento", linhaModel.ObservacoesAgendamento);*/

        }



        private static VndBEDocumentoVenda DuplicarDocumento(PrimaveraConnection pri, VndBEDocumentoVenda documentoOriginal)
        {
            VndBEDocumentoVenda documentoNovo = new VndBEDocumentoVenda
            {
                Tipodoc = documentoOriginal.Tipodoc,
                Serie = documentoOriginal.Serie,
                Entidade = documentoOriginal.Entidade,
                TipoEntidade = documentoOriginal.TipoEntidade
            };

            pri.BSO.Vendas.Documentos.PreencheDadosRelacionados(documentoNovo);

            documentoNovo.DataDoc = documentoOriginal.DataDoc;
            documentoNovo.DataVenc = documentoOriginal.DataVenc;
            documentoNovo.DescEntidade = documentoOriginal.DescEntidade;
            documentoNovo.DescFinanceiro = documentoOriginal.DescFinanceiro;
            documentoNovo.Nome = documentoOriginal.Nome;
            documentoNovo.Morada = documentoOriginal.Morada;
            documentoNovo.Morada2 = documentoOriginal.Morada2;
            documentoNovo.Localidade = documentoOriginal.Localidade;
            documentoNovo.Distrito = documentoOriginal.Distrito;
            documentoNovo.CodigoPostal = documentoOriginal.CodigoPostal;
            documentoNovo.LocalidadeCodigoPostal = documentoOriginal.LocalidadeCodigoPostal;
            documentoNovo.Pais = documentoOriginal.Pais;
            documentoNovo.NumContribuinte = documentoOriginal.NumContribuinte;
            documentoNovo.Referencia = documentoOriginal.Referencia;
            documentoNovo.ModoPag = documentoOriginal.ModoPag;
            documentoNovo.CondPag = documentoOriginal.CondPag;
            documentoNovo.ModoExp = documentoOriginal.ModoExp;
            documentoNovo.Moeda = documentoOriginal.Moeda;
            documentoNovo.MoedaDaUEM = documentoOriginal.MoedaDaUEM;
            documentoNovo.ContaDomiciliacao = documentoOriginal.ContaDomiciliacao;
            documentoNovo.Responsavel = documentoOriginal.Responsavel;
            documentoNovo.GeraPendentePorLinha = documentoOriginal.GeraPendentePorLinha;
            documentoNovo.Zona = documentoOriginal.Zona;
            documentoNovo.TipoOperacao = documentoOriginal.TipoOperacao;
            documentoNovo.Seccao = documentoOriginal.Seccao;
            documentoNovo.Origem = documentoOriginal.Origem;
            documentoNovo.EspacoFiscal = documentoOriginal.EspacoFiscal;
            documentoNovo.RegimeIva = documentoOriginal.RegimeIva;
            documentoNovo.RegimeIvaReembolsos = documentoOriginal.RegimeIvaReembolsos;
            documentoNovo.LocalOperacao = documentoOriginal.LocalOperacao;
            documentoNovo.TipoTerceiro = documentoOriginal.TipoTerceiro;
            documentoNovo.DE_IL = documentoOriginal.DE_IL;
            documentoNovo.CAE = documentoOriginal.CAE;
            documentoNovo.RefTipoDocOrig = documentoOriginal.RefTipoDocOrig;
            documentoNovo.RefSerieDocOrig = documentoOriginal.RefSerieDocOrig;
            documentoNovo.RefDocOrig = documentoOriginal.RefDocOrig;
            documentoNovo.TipoEntidadeFac = documentoOriginal.TipoEntidadeFac;
            documentoNovo.EntidadeFac = documentoOriginal.EntidadeFac;
            documentoNovo.Nome = documentoOriginal.Nome;
            documentoNovo.MoradaFac = documentoOriginal.MoradaFac;
            documentoNovo.Morada2Fac = documentoOriginal.Morada2Fac;
            documentoNovo.LocalidadeFac = documentoOriginal.LocalidadeFac;
            documentoNovo.DistritoFac = documentoOriginal.DistritoFac;
            documentoNovo.CodigoPostalFac = documentoOriginal.CodigoPostalFac;
            documentoNovo.LocalidadeCodigoPostalFac = documentoOriginal.LocalidadeCodigoPostalFac;
            documentoNovo.PaisFac = documentoOriginal.PaisFac;
            documentoNovo.NumContribuinteFac = documentoOriginal.NumContribuinteFac;
            documentoNovo.LocalCarga = documentoOriginal.LocalCarga;
            documentoNovo.EntidadeDescarga = documentoOriginal.EntidadeDescarga;
            documentoNovo.LocalDescarga = documentoOriginal.LocalDescarga;
            documentoNovo.CargaDescarga.MoradaCarga = documentoOriginal.CargaDescarga.MoradaCarga;
            documentoNovo.CargaDescarga.Morada2Carga = documentoOriginal.CargaDescarga.Morada2Carga;
            documentoNovo.CargaDescarga.LocalidadeCarga = documentoOriginal.CargaDescarga.LocalidadeCarga;
            documentoNovo.CargaDescarga.DistritoCarga = documentoOriginal.CargaDescarga.DistritoCarga;
            documentoNovo.CargaDescarga.CodPostalCarga = documentoOriginal.CargaDescarga.CodPostalCarga;
            documentoNovo.CargaDescarga.CodPostalLocalidadeCarga = documentoOriginal.CargaDescarga.CodPostalLocalidadeCarga;
            documentoNovo.CargaDescarga.PaisCarga = documentoOriginal.CargaDescarga.PaisCarga;
            documentoNovo.CargaDescarga.LocalCarga = documentoOriginal.CargaDescarga.LocalCarga;
            documentoNovo.CargaDescarga.Matricula = documentoOriginal.CargaDescarga.Matricula;
            documentoNovo.CargaDescarga.EntidadeDescarga = documentoOriginal.CargaDescarga.EntidadeDescarga;
            documentoNovo.CargaDescarga.EntidadeEntrega = documentoOriginal.CargaDescarga.EntidadeEntrega;
            documentoNovo.CargaDescarga.TipoEntidadeEntrega = documentoOriginal.CargaDescarga.TipoEntidadeEntrega;
            documentoNovo.CargaDescarga.NomeEntrega = documentoOriginal.CargaDescarga.NomeEntrega;
            documentoNovo.CargaDescarga.MoradaAlternativaEntrega = documentoOriginal.CargaDescarga.MoradaAlternativaEntrega;
            documentoNovo.CargaDescarga.UsaMoradaAlternativaEntrega = documentoOriginal.CargaDescarga.UsaMoradaAlternativaEntrega;
            documentoNovo.CargaDescarga.MoradaEntrega = documentoOriginal.CargaDescarga.MoradaEntrega;
            documentoNovo.CargaDescarga.Morada2Entrega = documentoOriginal.CargaDescarga.Morada2Entrega;
            documentoNovo.CargaDescarga.LocalidadeEntrega = documentoOriginal.CargaDescarga.LocalidadeEntrega;
            documentoNovo.CargaDescarga.DistritoEntrega = documentoOriginal.CargaDescarga.DistritoEntrega;
            documentoNovo.CargaDescarga.CodPostalEntrega = documentoOriginal.CargaDescarga.CodPostalEntrega;
            documentoNovo.CargaDescarga.CodPostalLocalidadeEntrega = documentoOriginal.CargaDescarga.CodPostalLocalidadeEntrega;
            documentoNovo.CargaDescarga.PaisEntrega = documentoOriginal.CargaDescarga.PaisEntrega;
            documentoNovo.CargaDescarga.LocalDescarga = documentoOriginal.CargaDescarga.LocalDescarga;
            documentoNovo.Grupo = documentoOriginal.Grupo;
            documentoNovo.Observacoes = documentoOriginal.Observacoes;
            documentoNovo.Fluxo = documentoOriginal.Fluxo;
            documentoNovo.IdOportunidade = documentoOriginal.IdOportunidade;
            documentoNovo.IdContrato = documentoOriginal.IdContrato;

            for (int i = 0; i < documentoOriginal.CamposUtil.NumItens; i++)
            {
                StdBECampo campoOriginal = documentoOriginal.CamposUtil[i];
                DefinirCDU(documentoNovo.CamposUtil, campoOriginal.Nome, campoOriginal.Valor);
            }

            for (int i = 1; i <= documentoOriginal.Linhas.NumItens; i++)
            {
                VndBELinhaDocumentoVenda linhaOriginal = documentoOriginal.Linhas.GetEdita(i);

                pri.BSO.Vendas.Documentos.AdicionaLinha(documentoNovo, linhaOriginal.Artigo);

                VndBELinhaDocumentoVenda linhaNova = documentoNovo.Linhas.GetEdita(documentoNovo.Linhas.NumItens);

                linhaNova.Armazem = linhaOriginal.Armazem;
                linhaNova.CodigoBarras = linhaOriginal.CodigoBarras;
                linhaNova.CodIva = linhaOriginal.CodIva;
                linhaNova.CodIvaEcotaxa = linhaOriginal.CodIvaEcotaxa;
                linhaNova.CodIvaIEC = linhaOriginal.CodIvaIEC;
                linhaNova.Comissao = linhaOriginal.Comissao;
                linhaNova.ContratoID = linhaOriginal.ContratoID;
                linhaNova.DataEntrega = linhaOriginal.DataEntrega;
                linhaNova.Desconto1 = linhaOriginal.Desconto1;
                linhaNova.Desconto2 = linhaOriginal.Desconto2;
                linhaNova.Desconto3 = linhaOriginal.Desconto3;
                linhaNova.DescontoComercial = linhaOriginal.DescontoComercial;
                linhaNova.Descricao = linhaOriginal.Descricao;
                linhaNova.IdContrato = linhaOriginal.IdContrato;
                linhaNova.IDObra = linhaOriginal.IDObra;
                linhaNova.IvaRegraCalculo = linhaOriginal.IvaRegraCalculo;
                linhaNova.Localizacao = linhaOriginal.Localizacao;
                linhaNova.Lote = linhaOriginal.Lote;
                linhaNova.PrecUnit = linhaOriginal.PrecUnit;
                linhaNova.ProcessoID = linhaOriginal.ProcessoID;
                linhaNova.Quantidade = linhaOriginal.Quantidade;
                linhaNova.QuantReservada = linhaOriginal.QuantReservada;
                linhaNova.RegimeIva = linhaOriginal.RegimeIva;
                linhaNova.RegraCalculoIncidencia = linhaOriginal.RegraCalculoIncidencia;
                linhaNova.TipoLinha = linhaOriginal.TipoLinha;
                linhaNova.TipoOperacao = linhaOriginal.TipoOperacao;
                linhaNova.Unidade = linhaOriginal.Unidade;
                linhaNova.Vendedor = linhaOriginal.Vendedor;

                for (int j = 1; j <= linhaOriginal.NumerosSerie.NumItens; j++)
                {
                    BasBENumeroSerie nrSerieOriginal = linhaOriginal.NumerosSerie.GetEdita(j);

                    BasBENumeroSerie nrSerieNovo = new BasBENumeroSerie
                    {
                        IdNumeroSerie = nrSerieOriginal.IdNumeroSerie,
                        Manual = nrSerieOriginal.Manual,
                        Modulo = nrSerieOriginal.Modulo,
                        NumeroSerie = nrSerieOriginal.NumeroSerie
                    };

                    linhaNova.NumerosSerie.Insere(nrSerieNovo);
                }

                for (int j = 0; j < linhaOriginal.CamposUtil.NumItens; j++)
                {
                    StdBECampo campoOriginal = linhaOriginal.CamposUtil[j];
                    DefinirCDU(linhaNova.CamposUtil, campoOriginal.Nome, campoOriginal.Valor);
                }
            }

            pri.BSO.Vendas.Documentos.Actualiza(documentoNovo);

            pri.BSO.DSO.Plat.Registos.ExecutaComando("UPDATE CabecDoc SET CriadoPorPortalTransucatas = 1 WHERE Id = @Id", new List<SqlParameter>() { new SqlParameter("@Id", documentoNovo.ID) });

            StdBEAnexos anexosOriginais = pri.BSO.DSO.Plat.Anexos.ListaAnexosTabela(EnumTabelaAnexos.anxVendas, documentoOriginal.ID);

            for (int i = 1; i <= anexosOriginais.NumItens; i++)
            {
                StdBEAnexo anexoOriginal = anexosOriginais.GetEdita(i);

                StdBEAnexo anexoNovo = new StdBEAnexo
                {
                    IdAnexo = StdSub32.GetGUID(true),
                    Tabela = EnumTabelaAnexos.anxVendas,
                    Chave = documentoNovo.ID,
                    FicheiroOrigem = anexoOriginal.FicheiroOrigem,
                    Descricao = anexoOriginal.Descricao,
                    Data = DateTime.Now,
                    Utilizador = pri.BSO.Contexto.UtilizadorActual,
                    ExportarTTE = anexoOriginal.ExportarTTE,
                    FicheiroDestinoWeb = anexoOriginal.FicheiroDestinoWeb,
                    Idioma = anexoOriginal.Idioma,
                    Tipo = anexoOriginal.Tipo,
                    Web = anexoOriginal.Web
                };

                string ficheiroAnexoOriginal = pri.BSO.DSO.Plat.Anexos.DaPercursoAnexo(anexoOriginal.IdAnexo, anexoOriginal, anexoOriginal.FicheiroOrigem);

                if (File.Exists(ficheiroAnexoOriginal))
                {
                    anexoNovo.FicheiroOrigem = ficheiroAnexoOriginal;

                    pri.BSO.DSO.Plat.Anexos.Actualiza(anexoNovo);

                    pri.BSO.DSO.Plat.Registos.ExecutaComando("UPDATE Anexos SET FicheiroOrig = @FicheiroOrig WHERE Id = @Id", new List<SqlParameter>() { new SqlParameter("@FicheiroOrig", anexoOriginal.FicheiroOrigem), new SqlParameter("@Id", anexoNovo.IdAnexo) });
                }
                else
                    Log.Warning("Ficheiro de anexo original com ID '{IdAnexo}' não encontrado em '{PathFicheiro}'. Não irá ser duplicado/gravado para novo documento.", anexoOriginal.IdAnexo, ficheiroAnexoOriginal);
            }

            return documentoNovo;
        }

        public static void DefinirCDU(StdBECampos camposUtil, string nomeCampo, object valor)
        {
            if (!camposUtil.Existe(nomeCampo))
                camposUtil.Add(new StdBECampo() { Nome = nomeCampo });

            camposUtil[nomeCampo].Valor = valor;
        }



        private static void GravarAnexosDocumento(PrimaveraConnection pri, string idDocumento, List<Anexo> anexos, List<string> ficheirosAnexos, List<string> ficheirosAnexosGravados)
        {
            foreach (string ficheiroAnexo in ficheirosAnexos)
            {
                AdicionarAnexoDocumento(pri, ficheiroAnexo, idDocumento, out string ficheiroAnexoGravado, anexos);

                if (!string.IsNullOrEmpty(ficheiroAnexoGravado))
                    ficheirosAnexosGravados.Add(ficheiroAnexoGravado);
            }

            if (anexos != null)
            {
                // apagar anexos
                // aviso: se houver vários anexos a apagar, e segundo (ou superior) anexo a apagar falhar, primeiro(s) anexo(s) vão continuar com ficheiro apagado.
                //        rollback de transação não consegue restaurar ficheiros apagados.

                StdBEAnexos anexosPrimavera = pri.BSO.DSO.Plat.Anexos.ListaAnexosTabela(EnumTabelaAnexos.anxVendas, idDocumento);

                for (int i = 1; i <= anexosPrimavera.NumItens; i++)
                {
                    StdBEAnexo anexoPrimavera = anexosPrimavera.GetEdita(i);

                    // se anexo não existir no model, é porque deve ser apagado
                    if (!anexos.Any(a => a.Id == new Guid(anexoPrimavera.IdAnexo)))
                        RemoverAnexo(pri, anexoPrimavera);
                }
            }
        }

        private static void RemoverAnexo(PrimaveraConnection pri, StdBEAnexo anexoPrimavera)
        {
            pri.BSO.DSO.Plat.Anexos.Remove(anexoPrimavera.IdAnexo);

            string pathFicheiroAnexo = pri.BSO.DSO.Plat.Anexos.DaPercursoAnexo(anexoPrimavera.IdAnexo, anexoPrimavera, anexoPrimavera.FicheiroOrigem);

            if (!string.IsNullOrEmpty(pathFicheiroAnexo) && File.Exists(pathFicheiroAnexo))
            {
                FileInfo fileInfo = new FileInfo(pathFicheiroAnexo);

                if (fileInfo.IsReadOnly)
                    fileInfo.IsReadOnly = false;

                File.Delete(pathFicheiroAnexo);
            }

            if (pri.BSO.DSO.Plat.PrefUtilStd.StorageProvider != "DISK")
                Log.Warning("Tipo de storage Primavera '{StorageProvider}' não é suportado. Ficheiro de anexo '{IdAnexo}' (de nome '{FicheiroOrigem}') pode não ter sido apagado.", pri.BSO.DSO.Plat.PrefUtilStd.StorageProvider, anexoPrimavera.IdAnexo, anexoPrimavera.FicheiroOrigem);
        }

        public static StdBEAnexo AdicionarAnexoDocumento(PrimaveraConnection pri, string ficheiroAnexo, string idCabecDoc, out string ficheiroAnexoGravado, List<Anexo> anexosDocumento = null)
        {
            StdBEAnexo anexoPrimavera = new StdBEAnexo
            {
                IdAnexo = StdSub32.GetGUID(true),
                Tabela = EnumTabelaAnexos.anxVendas,
                Chave = idCabecDoc,
                FicheiroOrigem = ficheiroAnexo,
                Descricao = "Anexo sem descrição",
                Data = DateTime.Now,
                Utilizador = pri.BSO.Contexto.UtilizadorActual,

            };

            pri.BSO.DSO.Plat.Anexos.Actualiza(anexoPrimavera);

            ficheiroAnexoGravado = pri.BSO.DSO.Plat.Anexos.DaPercursoAnexo(anexoPrimavera.IdAnexo, anexoPrimavera, anexoPrimavera.FicheiroOrigem);

            if (!File.Exists(ficheiroAnexoGravado))
            {
                Log.Warning("Gravado anexo em Primavera com ID '{IdAnexo}' mas esta aplicação não consegue encontra-lo em '{PathFicheiro}'. Pode não ter sido gravado corretamente ou esta aplicação pode não ter permissões para vê-lo.", anexoPrimavera.IdAnexo, ficheiroAnexoGravado);
                ficheiroAnexoGravado = "";
            }

            if (anexosDocumento != null)
            {
                anexosDocumento.Add(new Anexo()
                {
                    Id = new Guid(anexoPrimavera.IdAnexo),
                    FicheiroOrig = anexoPrimavera.FicheiroOrigem,
                    FilePath = ficheiroAnexoGravado,
                });
            }

            return anexoPrimavera;
        }

        public static async Task<Anexo> GetAnexoProposta( string username, string password, Guid idAnexo)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT
	A.Id,
	A.FicheiroOrig,
	A.Chave,
    CD.CDU_MenuProposta

FROM
	Anexos A
	INNER JOIN CabecDoc CD ON CD.Id = A.Chave
	INNER JOIN DocumentosVenda DV ON DV.Documento = CD.TipoDoc
	INNER JOIN SeriesVendas SV ON SV.TipoDoc = CD.TipoDoc AND SV.Serie = CD.Serie

WHERE
	A.Id = @IdAnexo
	AND A.Tabela = 41
	AND DV.CDU_TipoDocDPProposta = 1
	AND SV.CDU_SerieDPProposta = 1
";

                    cmd.Parameters.AddWithValue("@IdAnexo", idAnexo);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            throw new FlorestasBemCuidadaWebApiException("Anexo não existe ou não é de uma Proposta.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

                        Anexo anexo = BuildAnexoFromReader(sqlDataReader);

                        anexo.MenuProposta = sqlDataReader["CDU_MenuProposta"] == DBNull.Value ? null : (string)sqlDataReader["CDU_MenuProposta"];

                        if (string.IsNullOrEmpty(anexo.FicheiroOrig))
                            throw new FlorestasBemCuidadaWebApiException("Anexo não tem nome de ficheiro definido.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

                        using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirPSO: true))
                        {
                            StdBEAnexo anexoPrimavera = new StdBEAnexo()
                            {
                                IdAnexo = anexo.Id.ToString(),
                                Chave = anexo.Chave,
                                FicheiroOrigem = anexo.FicheiroOrig,
                            };

                            anexo.FilePath = await pri.PSO.Anexos.ObtemAnexo(anexoPrimavera, true, false);
                        }

                        if (string.IsNullOrEmpty(anexo.FilePath))
                            throw new FlorestasBemCuidadaWebApiException("Primavera devolveu um caminho de ficheiro vazio.", true, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);

                        return anexo;
                    }
                }
            }
        }

        private static Anexo BuildAnexoFromReader(SqlDataReader sqlDataReader)
        {
            return new Anexo
            {
                Id = (Guid)sqlDataReader["Id"],
                FicheiroOrig = sqlDataReader["FicheiroOrig"] == DBNull.Value ? null : (string)sqlDataReader["FicheiroOrig"],
                Chave = sqlDataReader["Chave"] == DBNull.Value ? null : (string)sqlDataReader["Chave"],
            };
        }

        public static List<string> GetFilePathsPdfProposta( string username, string password, Guid idDoc, string report, int nrVias, out string menu, string path = null)
        {
            if (nrVias <= 0 || nrVias > 6)
                throw new FlorestasBemCuidadaWebApiException("Número de vias deve ser entre 1 e 6.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                StdBECampos campos = pri.BSO.Vendas.Documentos.DaValorAtributosID(idDoc.ToString(), "Filial", "TipoDoc", "Serie", "NumDoc", "CDU_MenuProposta");

                if (campos == null)
                    throw new FlorestasBemCuidadaWebApiException("Documento não encontrado.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

                string filial = campos["Filial"].Valor as string;
                string tipoDoc = campos["TipoDoc"].Valor as string;
                string serie = campos["Serie"].Valor as string;
                int numDoc = campos["NumDoc"].Valor as int? ?? 0;
                menu = campos["CDU_MenuProposta"].Valor as string;

                BaseDAL.GerarPathFicheiroPdfTemporario(tipoDoc, serie, numDoc, out string filePath, out string fileFolder, out string fileNameWithoutExtension, out string fileExtension, path);

                List<string> filesPaths = new List<string>();

                try
                {
                    bool impresso = pri.BSO.Vendas.Documentos.ImprimeDocumento(tipoDoc, serie, numDoc, filial, nrVias, report ?? "", false, filePath, 1);

                    if (!impresso)
                        throw new FlorestasBemCuidadaWebApiException("ERP Primavera não conseguiu imprimir o documento. Erro desconhecido.", true, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);

                    if (nrVias == 1)
                    {
                        if (!File.Exists(filePath))
                        {
                            Log.Error("Ficheiro PDF de proposta '{IdDoc}' não foi encontrado em '{FilePath}'. A aplicação pode não ter permissões ou ficheiro pode ter sido apagado.", idDoc, filePath);
                            throw new FlorestasBemCuidadaWebApiException("O ficheiro PDF gerado não foi encontrado.", true, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);
                        }

                        filesPaths.Add(filePath);
                    }
                    else
                    {
                        for (int via = 1; via <= nrVias; via++)
                        {
                            string filePathVia = GetFilePathVia(fileFolder, fileNameWithoutExtension, fileExtension, via);

                            if (!File.Exists(filePathVia))
                            {
                                Log.Error("Ficheiro PDF de via {Via} de proposta '{IdDoc}' não foi encontrado em '{FilePath}'. A aplicação pode não ter permissões, o ficheiro pode ter sido apagado ou ERP pode ter gerado com outro nome (verificar ficheiros na pasta).", via, idDoc, filePathVia);
                                throw new FlorestasBemCuidadaWebApiException($"O ficheiro PDF da via {via} não foi encontrado.", true, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);
                            }

                            filesPaths.Add(filePathVia);
                        }
                    }
                }
                catch (Exception)
                {
                    Utils.TentarApagarFicheiroTemporario(filePath);

                    throw;
                }

                return filesPaths;
            }
        }

        public static List<string> GetFilePathsPdf(PrimaveraConnection pri,  string username, string password, Guid idDoc, string report, int nrVias, string path = null)
        {
            if (nrVias <= 0 || nrVias > 6)
                throw new FlorestasBemCuidadaWebApiException("Número de vias deve ser entre 1 e 6.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            StdBECampos campos = pri.BSO.Vendas.Documentos.DaValorAtributosID(idDoc.ToString(), "Filial", "TipoDoc", "Serie", "NumDoc");

            if (campos == null)
                throw new FlorestasBemCuidadaWebApiException("Documento não encontrado.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            string filial = campos["Filial"].Valor as string;
            string tipoDoc = campos["TipoDoc"].Valor as string;
            string serie = campos["Serie"].Valor as string;
            int numDoc = campos["NumDoc"].Valor as int? ?? 0;

            BaseDAL.GerarPathFicheiroPdfTemporario(tipoDoc, serie, numDoc, out string filePath, out string fileFolder, out string fileNameWithoutExtension, out string fileExtension, path);

            List<string> filesPaths = new List<string>();

            try
            {
                bool impresso = pri.BSO.Vendas.Documentos.ImprimeDocumento(tipoDoc, serie, numDoc, filial, nrVias, report ?? "", false, filePath, 1);

                if (!impresso)
                    throw new FlorestasBemCuidadaWebApiException("ERP Primavera não conseguiu imprimir o documento. Erro desconhecido.", true, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);

                if (nrVias == 1)
                {
                    if (!File.Exists(filePath))
                    {
                        Log.Error("Ficheiro PDF de proposta '{IdDoc}' não foi encontrado em '{FilePath}'. A aplicação pode não ter permissões ou ficheiro pode ter sido apagado.", idDoc, filePath);
                        throw new FlorestasBemCuidadaWebApiException("O ficheiro PDF gerado não foi encontrado.", true, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);
                    }

                    filesPaths.Add(filePath);
                }
                else
                {
                    for (int via = 1; via <= nrVias; via++)
                    {
                        string filePathVia = GetFilePathVia(fileFolder, fileNameWithoutExtension, fileExtension, via);

                        if (!File.Exists(filePathVia))
                        {
                            Log.Error("Ficheiro PDF de via {Via} de proposta '{IdDoc}' não foi encontrado em '{FilePath}'. A aplicação pode não ter permissões, o ficheiro pode ter sido apagado ou ERP pode ter gerado com outro nome (verificar ficheiros na pasta).", via, idDoc, filePathVia);
                            throw new FlorestasBemCuidadaWebApiException($"O ficheiro PDF da via {via} não foi encontrado.", true, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);
                        }

                        filesPaths.Add(filePathVia);
                    }
                }
            }
            catch (Exception)
            {
                Utils.TentarApagarFicheiroTemporario(filePath);

                throw;
            }

            return filesPaths;
        }

        private static string GetFilePathVia(string fileFolder, string fileNameWithoutExtension, string fileExtension, int via)
        {
            return Path.Combine(fileFolder, fileNameWithoutExtension + "_" + via + fileExtension);
        }


        // TODO: Remover quando RuiF disser que dados as linhas devem ter
        private static async Task<string> GetArtigoTeste(string empresa)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa(empresa)))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT TOP 1
    A.Artigo
FROM
    Artigo A
WHERE
    A.ArtigoAnulado = 0
	AND EXISTS(SELECT 1 FROM TiposArtigosPermissoes TAP WHERE TAP.TipoArtigo = A.TipoArtigo AND TAP.Permitido = 1 AND TAP.Modulo = 'C' AND TAP.TipoDocumento = 2)
	AND EXISTS(SELECT 1 FROM TiposArtigosPermissoes TAP WHERE TAP.TipoArtigo = A.TipoArtigo AND TAP.Permitido = 1 AND TAP.Modulo = 'V' AND TAP.TipoDocumento = 2)
ORDER BY
    A.Artigo";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return null;

                        return (string)sqlDataReader["Artigo"];
                    }
                }
            }
        }
    }
}