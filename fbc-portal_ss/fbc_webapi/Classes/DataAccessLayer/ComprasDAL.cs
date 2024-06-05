using BasBE100;
using CmpBE100;
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
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;
using fbc_webapi.Primavera;
using UpgradeHelpers.DB;
using VndBE100;
using static StdPlatBE100.StdBETipos;
using IntBE100;
using Dapper;
using System.Text;

namespace fbc_webapi.Classes.DataAccessLayer
{
    public static class ComprasDAL
    {
        public static async Task<List<DocumentoCompra>> GetEncomendas()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                var documentos = new List<DocumentoCompra>();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.Transaction = null;

                    cmd.CommandText = @"
                                        SELECT
	                                        [CabecCompras].[Id],
                                            [CabecCompras].[Documento],
                                            [CabecCompras].[NumDoc],
	                                        [CabecCompras].[Serie],
	                                        [CabecCompras].[TipoDoc], 
	                                        [CabecCompras].[DataDoc] as Data,
											[CabecCompras].[Utilizador],
	                                        [CabecCompras].[ObraID] as ObraId,
                                            [COP_Obras].[Codigo] Obra,
	                                        [COP_Obras].[Descricao] NomeObra,
	                                        [CabecCompras].[Entidade] Fornecedor,
	                                        [Fornecedores].[Nome] NomeFornecedor,
	                                        [CabecCompras].[TotalMerc] Total

                                        FROM
	                                        [CabecCompras] 
	                                        LEFT JOIN [COP_Obras] ON [CabecCompras].[ObraId] = [COP_Obras].ID 
	                                        LEFT JOIN [Fornecedores] ON [CabecCompras].[Entidade] = [Fornecedores].[Fornecedor] WHERE [CabecCompras].[TipoDoc] IN (SELECT [Documento] FROM [DocumentosCompra] WHERE CDU_Encomenda_WEB = 1)

                                        ORDER BY DataDoc DESC
                                    ";


                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                      
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            var documento = new DocumentoCompra
                            {
                                Id = (Guid)sqlDataReader["Id"],
                                Documento = sqlDataReader["Documento"] == DBNull.Value ? null : (string)sqlDataReader["Documento"],
                                TipoDoc = (string)sqlDataReader["TipoDoc"],
                                Serie = (string)sqlDataReader["Serie"],
                                NumDoc = (int)sqlDataReader["NumDoc"],
                                DataDoc = sqlDataReader["Data"] == DBNull.Value ? null : (DateTime?)sqlDataReader["Data"],
                                ObraID = sqlDataReader["ObraId"] == DBNull.Value ? Guid.Empty : (Guid)sqlDataReader["ObraId"],
                                Obra = sqlDataReader["Obra"] == DBNull.Value ? null : (string)sqlDataReader["Obra"],
                                NomeObra = sqlDataReader["NomeObra"] == DBNull.Value ? null : (string)sqlDataReader["NomeObra"],
                                Entidade = sqlDataReader["Fornecedor"] == DBNull.Value ? null : (string)sqlDataReader["Fornecedor"],                             
                                NomeEntidade = sqlDataReader["NomeFornecedor"] == DBNull.Value ? null : (string)sqlDataReader["NomeFornecedor"],
                                Aprovador = sqlDataReader["Utilizador"] == DBNull.Value ? null : (string)sqlDataReader["Utilizador"],
                                TotalDocumento = sqlDataReader["Total"] == DBNull.Value ? 0 : (double)sqlDataReader["Total"],
                            };

                            documentos.Add(documento);
                        }

                        return documentos;
                    }
                }
            }
        }


        public static async Task<List<TipoDocumentosCompras>> GetTiposDocumentoEncomendaFornecedor()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Documento, Descricao FROM DocumentosCompra WHERE CDU_DispPortalParaEncomendas = 1 ORDER BY Descricao";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<TipoDocumentosCompras> tiposDoc = new List<TipoDocumentosCompras>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            TipoDocumentosCompras tipoDoc = new TipoDocumentosCompras
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

        public static async Task<List<TipoDocumentosCompras>> GetTiposDocumento()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Documento,Descricao FROM DocumentosCompra WHERE CDU_Encomenda_WEB = 1 ORDER BY Descricao";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<TipoDocumentosCompras> tiposDoc = new List<TipoDocumentosCompras>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            TipoDocumentosCompras tipoDoc = new TipoDocumentosCompras
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

        public static async Task<List<SerieCompras>> GetSeries(string tipoDocumento)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Serie, Descricao, SeriePorDefeito FROM SeriesCompras WHERE TipoDoc = @TipoDoc AND CDU_Encomendas_WEB = 1 ORDER BY Serie DESC";

                    cmd.Parameters.AddWithValue("@TipoDoc", tipoDocumento);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<SerieCompras> series = new List<SerieCompras>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            SerieCompras serie = new SerieCompras
                            {
                                Codigo = (string)sqlDataReader["Serie"],
                                Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                                SeriePorDefeito = (bool)sqlDataReader["SeriePorDefeito"],
                            };

                            series.Add(serie);
                        }

                        return series;
                    }
                }
            }
        }

        public static List<string> GetFilePathsPdf(PrimaveraConnection pri,  string username, string password, Guid idDoc, string report, int nrVias, string path = null)
        {
            if (nrVias <= 0 || nrVias > 6)
                throw new FlorestasBemCuidadaWebApiException("Número de vias deve ser entre 1 e 6.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            StdBECampos campos = pri.BSO.Compras.Documentos.DaValorAtributosID(idDoc.ToString(), "Filial", "TipoDoc", "Serie", "NumDoc");

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
                bool impresso = pri.BSO.Compras.Documentos.ImprimeDocumento(tipoDoc, serie, numDoc, filial, nrVias, report ?? "", false, filePath);

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
                        string filePathVia = BaseDAL.GetFilePathVia(fileFolder, fileNameWithoutExtension, fileExtension, via);

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

        public static async Task<Guid> CriarDocumento( string username, string password, DocumentoCompra documentoModel, List<string> ficheirosAnexos)
        {
            var ficheirosAnexosGravados = new List<string>();

            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                try
                {
                    pri.BSO.IniciaTransaccao();
                    
                    AdicionaObraLinhas(ref documentoModel);

                    documentoModel.Aprovador = username;

                    var documento = await CriarDocumento(pri, documentoModel, ficheirosAnexos, ficheirosAnexosGravados);

                    string filePath = IPrimavera.Utilitarios.DaCaminhoDocumento(pri, documento);

                    pri.BSO.TerminaTransaccao();

                    ImprimeDocumento(username, password, documento.ID, filePath);

                    Utils.EnviaNotificacaoAprovado(username, documentoModel, documento, filePath); 

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
        }

        public static bool ImprimeDocumento(string username, string password, string Id, string filePath)
        {
            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true, abrirPSO: true))
            {
                try
                {
                    pri.BSO.IniciaTransaccao();

                    var documento = pri.BSO.Compras.Documentos.EditaID(Id);

                    if (documento == null)
                        throw new FlorestasBemCuidadaWebApiException("ERP Primavera não conseguiu encontrar o documento.", true, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);


                    var NomeReport = DaMapaImpressao(documento.Tipodoc);

                    bool impresso = pri.BSO.Compras.Documentos.ImprimeDocumento(documento.Tipodoc, documento.Serie, documento.NumDoc, documento.Filial, 1, NomeReport ?? "GCPCLS01", false, filePath);

                    if (!impresso)
                        throw new FlorestasBemCuidadaWebApiException("ERP Primavera não conseguiu imprimir o documento. Erro desconhecido.", true, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);


                    if (!File.Exists(filePath))
                    {
                        Log.Error("Ficheiro PDF '{IdDoc}' não foi encontrado em '{FilePath}'. A aplicação pode não ter permissões ou ficheiro pode ter sido apagado.", Id, filePath);
                        throw new FlorestasBemCuidadaWebApiException("O ficheiro PDF gerado não foi encontrado.", true, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);
                    }

                    pri.BSO.TerminaTransaccao();

                    return impresso;
                }
                catch (Exception)
                {
                    Utils.TentarApagarFicheiroTemporario(filePath);
                    pri.BSO.DSO.Plat.DesfazTransaccao();
                    pri.BSO.DesfazTransaccao();

                    throw;
                }
            }
        }

        private static string DaMapaImpressao(string Documento)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.Transaction = null;

                    cmd.CommandText = @" SELECT CDU_Report FROM [DocumentosCompra] WHERE Documento = @Documento";

                    cmd.Parameters.AddWithValue("@Documento", Documento);

                    using (SqlDataReader sqlDataReader = cmd.ExecuteReader())
                    {
                        if (!sqlDataReader.Read())
                            return null;

                        string NomeReport = sqlDataReader["CDU_Report"] == DBNull.Value ? null : (string)sqlDataReader["CDU_Report"];

                        return NomeReport;
                    }
                }
            }
        }



        private static void AdicionaObraLinhas( ref DocumentoCompra documentoModel)
        {
            if (documentoModel.ObraID == null)
                return;
         
            foreach (var Linha in documentoModel.Linhas)
                Linha.ObraId = documentoModel.ObraID.ToString();
        }

        public static async Task<int> GetProximoNumeroSerie( string tipoDocumento, string serie)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT MAX(NumDoc) +1 AS Numerador FROM CabecRasCompras WHERE Serie = @Serie AND TipoDoc = @TipoDoc";
                    //cmd.CommandText = "SELECT (ISNULL(Numerador, 0) + 1) Numerador FROM SeriesCompras WHERE Serie = @Serie AND TipoDoc = @TipoDoc";

                    cmd.Parameters.AddWithValue("@TipoDoc", tipoDocumento);
                    cmd.Parameters.AddWithValue("@Serie", serie);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return 0;

                        int proximoNumero = sqlDataReader["Numerador"] == DBNull.Value ? 1 : (int)sqlDataReader["Numerador"];

                        return proximoNumero;
                    }
                }
            }
        }

        private static void AnulaDocumento(PrimaveraConnection pri, string id, EstornoDocumento dadosEstorno)
        {
            string avisos = "";
            pri.BSO.Compras.Documentos.AnulaDocumentoID(id, dadosEstorno.MotivoEstorno, dadosEstorno.Observacoes, ref avisos);

            if (!string.IsNullOrEmpty(avisos))
                Log.Warning("Avisos ao anular documento '{Id}' em '{Empresa}': {Avisos}", id, pri.BSO.Contexto.CodEmp, avisos);
        }

        public static CmpBEDocumentoCompra GetDocumentoCompra(string username, string password, Guid idDocumento)
        {
            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                CmpBEDocumentoCompra documentoPrimavera = pri.BSO.Compras.Documentos.EditaID(idDocumento.ToString());

                return documentoPrimavera;
            }
        }

        public static async Task<CmpBEDocumentoCompra> CriarDocumento(PrimaveraConnection pri, DocumentoCompra documentoModel, List<string> ficheirosAnexos, List<string> ficheirosAnexosGravados)
        {

            CmpBEDocumentoCompra documentoPrimavera = new CmpBEDocumentoCompra();

            DefinirCamposDoc(documentoModel, pri, documentoPrimavera);

            foreach (LinhaDocumentoCompra linhaModel in documentoModel.Linhas)
                await AdicionarLinha(linhaModel, pri, documentoPrimavera);

            pri.BSO.Compras.Documentos.Actualiza(documentoPrimavera);  
               
            if(documentoModel.ObraID != null)
                pri.BSO.DSO.Plat.Registos.ExecutaComando($"UPDATE CabecCompras SET ObraId = '{documentoModel.ObraID}' WHERE Id = @Id", new List<SqlParameter>() { new SqlParameter("@Id", documentoPrimavera.ID) });

            GravarAnexosDocumento(pri, documentoPrimavera.ID.Trim(new char[] { '{', '}' }), documentoModel.Anexos, ficheirosAnexos, ficheirosAnexosGravados);

            return documentoPrimavera;
        }

        private static async Task AdicionarLinha(LinhaDocumentoCompra linhaModel, PrimaveraConnection pri, CmpBEDocumentoCompra documentoPrimavera)
        {
            double quantidade = 0;
            string armazem = "";
            string localizacao = "";

            if (linhaModel.Quantidade != null)
                quantidade = linhaModel.Quantidade.Value;

            pri.BSO.Compras.Documentos.AdicionaLinha(documentoPrimavera, linhaModel.Artigo, ref quantidade, ref armazem, ref localizacao, linhaModel.Preco ?? 0);

            CmpBELinhaDocumentoCompra linhaPrimavera = documentoPrimavera.Linhas.GetEdita(documentoPrimavera.Linhas.NumItens);

            await DefinirCamposLinha(linhaModel, pri, documentoPrimavera, linhaPrimavera);

            if (!string.IsNullOrEmpty(linhaPrimavera.IdLinha))
                linhaModel.Id = new Guid(linhaPrimavera.IdLinha);

            linhaModel.NumLinha = documentoPrimavera.Linhas.NumItens;
        }

        private static void DefinirCamposDoc(DocumentoCompra documentoModel, PrimaveraConnection pri, CmpBEDocumentoCompra documentoPrimavera)
        {
            if (!documentoPrimavera.EmModoEdicao)
            {
                documentoPrimavera.Tipodoc = documentoModel.TipoDoc;
                documentoPrimavera.Serie = documentoModel.Serie;
                documentoPrimavera.NumDocExterno = "1";
            }

           
            documentoPrimavera.TipoEntidade = documentoModel.TipoEntidade;
            documentoPrimavera.Entidade = documentoModel.Entidade;

            if (!documentoPrimavera.EmModoEdicao)
                pri.BSO.Compras.Documentos.PreencheDadosRelacionados(documentoPrimavera);


            if (documentoModel.DataDoc != null)
            {
                DateTime dataDoc = documentoModel.DataDoc.Value;

                if (documentoPrimavera.EmModoEdicao)
                    dataDoc = new DateTime(dataDoc.Year, dataDoc.Month, dataDoc.Day, documentoPrimavera.DataDoc.Hour, documentoPrimavera.DataDoc.Minute, documentoPrimavera.DataDoc.Second);

                documentoPrimavera.DataDoc = dataDoc;
            }

            if (documentoModel.DescFinanceiro > 0)
                documentoPrimavera.DescFinanceiro = documentoModel.DescFinanceiro;

            if (documentoModel.DescFornecedor > 0)
                documentoPrimavera.DescFornecedor = documentoModel.DescFornecedor;

            if (documentoModel.DataVenc != null)
                documentoPrimavera.DataVenc = (DateTime)documentoModel.DataVenc;

            if (documentoModel.Observacoes != null)
                documentoPrimavera.Observacoes = documentoModel.Observacoes;

            if (documentoModel.Aprovador != null)
                documentoPrimavera.Utilizador = documentoModel.Aprovador;

        }

        private static async Task DefinirCamposLinha(LinhaDocumentoCompra linhaModel, PrimaveraConnection pri, CmpBEDocumentoCompra documentoPrimavera, CmpBELinhaDocumentoCompra linhaPrimavera)
        {
            if (documentoPrimavera.EmModoEdicao)
            {
                linhaPrimavera.Artigo = linhaModel.Artigo;

                if (linhaModel.Preco != null)
                    linhaPrimavera.PrecUnit = linhaModel.Preco.Value;

                if (linhaModel.Quantidade != null)
                    linhaPrimavera.Quantidade = linhaModel.Quantidade.Value;
            }

            if (linhaModel.Descricao != null)
                linhaPrimavera.Descricao = linhaModel.Descricao;

            if (linhaModel.DataEntrega != null)
                linhaPrimavera.DataEntrega = (DateTime)linhaModel.DataEntrega;

            if (linhaModel.ObraId != null)
                linhaPrimavera.IDObra = linhaModel.ObraId;

            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Observacoes", linhaModel.Observacoes);

        }

        private static void DefinirCDU(StdBECampos camposUtil, string nomeCampo, object valor)
        {
            if (!camposUtil.Existe(nomeCampo))
                camposUtil.Add(new StdBECampo() { Nome = nomeCampo });

            camposUtil[nomeCampo].Valor = valor;
        }


        public static async Task<List<DocumentoInterno>> GetRascunhosAprovacao(string utilizador)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                var query = new StringBuilder();

                query.Append(@"SELECT
	                                CRC.Id,
	                                CRC.TipoDoc,
	                                CRC.Serie,
	                                CRC.NumDoc,
	                                CRC.DataDoc,
	                                CRC.Obra,
	                                CRC.NomeObra,
	                                CRC.Entidade,
	                                CRC.NomeEntidade,
	                                CRC.Documento,
	                                CRC.TotalDocumento,
	                                CRC.Utilizador,
									E.Descricao As Equipa

                                FROM
	                                CabecRasCompras CRC
                                    JOIN [PRIEMPRE].[dbo].[MembrosEquipa] ME ON CRC.Utilizador = ME.Utilizador
                                    JOIN [PRIEMPRE].[dbo].[LeadersEquipa] LE ON ME.CodEquipa = LE.CodEquipa 
                                    JOIN [PRIEMPRE].[dbo].[Equipas] E ON E.Codigo = LE.CodEquipa

                                WHERE
	                                E.Activa = 1
	                                AND LE.Utilizador = @Utilizador
	                                AND CRC.Estado NOT IN ('A','R')");


                var documentos = await conn.QueryAsync<DocumentoInterno>(query.ToString(), new { Utilizador = utilizador });

                return documentos.ToList();
            }
        }


        public static async Task<List<DocumentoCompra>> GetRascunhos(string utilizador =  null, string estado = null)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                var query = new StringBuilder();

                query.Append(@"SELECT * FROM CabecRasCompras WHERE 1=1");

                if (!string.IsNullOrWhiteSpace(utilizador))
                    query.AppendLine($"AND Utilizador = @Utilizador ");

                if (!string.IsNullOrWhiteSpace(estado))
                    query.AppendLine($"AND Estado = @Estado ");

                var documentos = await conn.QueryAsync<DocumentoCompra>(query.ToString(), new { Utilizador = utilizador, Estado = estado });

                return documentos.ToList();
            }
        }

        private static async Task GetAnexosDocumentoCompras(string id, DocumentoCompra documento, string username, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
                {
                    var anexos = await conn.QueryAsync<Anexo>(@"SELECT  A.Id, A.Chave, A.FicheiroOrig FROM Anexos A WHERE A.Chave = @IdCabecDoc ORDER BY A.Data", new { IdCabecDoc = id });

                    documento.Anexos = (List<Anexo>)anexos;

                    PreencherTamanhoAnexos(documento, username, password);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void PreencherTamanhoAnexos(DocumentoCompra documento, string username, string password)
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

        public static async Task<DocumentoCompra> GetRascunho(string username, string password, Guid id, bool soCorpo = false)
        {
            DocumentoCompra documento = await GetRascunho(id, soCorpo);

            await GetAnexosDocumentoCompras(id.ToString().Trim(new char[] { '{', '}' }), documento, username, password).ConfigureAwait(false);

            return documento;
        }

        public static async Task<DocumentoCompra> GetRascunho(Guid id, bool soCorpo = false)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                var documento = await conn.QueryAsync<DocumentoCompra>("SELECT * FROM CabecRasCompras WHERE Id = @Id", new { Id = id});

                if (documento == null)
                    return null;

                var linhas = await conn.QueryAsync<LinhaDocumentoCompra>("SELECT * FROM LinhasRasCompras Where IdCabecRasCompras = @IdCabecRasCompras", new { IdCabecRasCompras = id });

                if (linhas != null)
                    documento.First().Linhas = linhas.ToList();

                return documento.First();
            }
        }

        public static async void DeleteRascunho(Guid id)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute(@"DELETE FROM LinhasRasCompras WHERE IdCabecRasCompras = @Id", new { Id = id }, tran);

                        conn.Execute(@"DELETE FROM CabecRasCompras WHERE Id = @Id", new { Id = id }, tran);

                        tran.Commit();

                    }
                    catch (Exception)
                    {
                        tran.Rollback();

                        throw;
                    }
                }
            }

        }

        public static void UpdateLinhaRascunho(LinhaDocumentoCompra Linha, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                conn.Execute(@"
                    UPDATE 
                        [LinhasRasCompras]
                    SET
	                    Artigo = @Artigo,
	                    Descricao = @Descricao,
	                    Quantidade = @Quantidade,
	                    Unidade = @Unidade,
	                    Armazem = @Armazem,
	                    Localizacao = @Localizacao,
	                    Observacoes = @Observacoes,	
	                    Preco = @Preco,
	                    Total = @Total,
	                    ObraId = @ObraId,
	                    DataEntrega = @DataEntrega

                    WHERE 
                        Id = @Id ", Linha, tran);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void UpdateCabecRascunho(DocumentoCompra documentoModel, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                conn.Execute(@"
                    UPDATE 
                        CabecRasCompras
                    SET
                        Documento= @Documento,
                        TipoDoc = @TipoDoc,
                        Serie = @Serie,
                        NumDoc = @NumDoc,                       
                        DataDoc = @DataDoc, 
                        DataVenc = @DataVenc,
                        TipoEntidade = @TipoEntidade,
                        Entidade = @Entidade, 
                        NomeEntidade = @NomeEntidade,
                        DescFornecedor = @DescFornecedor, 
                        DescFinanceiro = @DescFinanceiro, 
                        ObraId = @ObraId, 
                        Obra = @Obra, 
                        NomeObra = @NomeObra,
                        Utilizador = @Utilizador,
                        Estado = @Estado,
                        Aprovador = @Aprovador, 
                        DataAprovacao = @DataAprovacao, 
                        MotivoRejeicao = @MotivoRejeicao, 
                        Observacoes = @Observacoes,
                        DataUltimaActualizacao = getdate()
                    WHERE 
                        Id = @Id ", documentoModel, tran);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void DeleteLinhaRascunho(Guid id, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                conn.Execute(@"DELETE FROM LinhasRasCompras WHERE Id = @Id", new { Id = id }, tran);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void UpdateRascunho(DocumentoCompra documentoModel, List<Guid> LinhasApagadas)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
                {
                    conn.Open();

                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            UpdateCabecRascunho(documentoModel, conn, tran);

                            foreach (var Linha in documentoModel.Linhas)
                            {

                                if (Linha.Id != null)
                                    UpdateLinhaRascunho(Linha, conn, tran);
                                else
                                    InsertLinhaRascunho(Linha, conn, tran);
                            }

                            foreach (var linha in LinhasApagadas)
                                DeleteLinhaRascunho(linha, conn, tran);

                            tran.Commit();
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void InsertLinhaRascunho(LinhaDocumentoCompra Linha, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                conn.Execute(@"INSERT INTO LinhasRasCompras (IdCabecRasCompras, Artigo, Descricao, Quantidade, Unidade, [Preco], [Total], [ObraId], [DataEntrega], Observacoes) 
                                                VALUES(@IdCabecRasCompras, @Artigo, @Descricao, @Quantidade, @Unidade, @Preco, @Total, @ObraId, @DataEntrega, @Observacoes)", Linha, tran);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void InsertRascunho(DocumentoCompra documentoModel)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
                {
                    conn.Open();

                    using (var tran = conn.BeginTransaction())
                    {

                        try
                        {
                            conn.Execute(@"INSERT INTO CabecRasCompras (Id, Documento, TipoDoc, Serie, NumDoc, Estado, DataDoc, DataVenc, TipoEntidade, Entidade, NomeEntidade, DescFornecedor, DescFinanceiro, ObraId, Obra, NomeObra, Utilizador, Observacoes) 
                                            VALUES(@Id, @Documento, @TipoDoc, @Serie, @NumDoc, @Estado, @DataDoc, @DataVenc, @TipoEntidade, @Entidade, @NomeEntidade, @DescFornecedor, @DescFinanceiro, @ObraId, @Obra, @NomeObra, @Utilizador, @Observacoes)", documentoModel, tran);

                            foreach (var Linha in documentoModel.Linhas)
                                InsertLinhaRascunho(Linha, conn, tran);

                            tran.Commit();
                        }
                        catch (Exception)
                        {
                            tran.Rollback();

                            throw;
                        }
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task AlterarRascunho(string username, string password, Guid id, DocumentoCompra documentoRascunho, DocumentoCompra documentoModel, List<string> ficheirosAnexos)
        {
            List<string> ficheirosAnexosGravados = new List<string>();

            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                try
                {
                    DefinirCamposRascunho(documentoModel, ref documentoRascunho);

                    var linhasApagadas = new List<Guid>();

                    if (documentoModel.Linhas != null)
                    {
                        // apagar linhas
                        foreach (var linha in documentoRascunho.Linhas)
                        {
                            // se linha não existir no pedido enviado, é porque deve ser apagada
                            if (!documentoModel.Linhas.Any(l => l.Id == linha.Id))
                                linhasApagadas.Add((Guid)linha.Id);
                        }

                        foreach (var linha in linhasApagadas)
                            documentoRascunho.Linhas.RemoveAll(l => l.Id == linha);

                        // alterar linhas existentes
                        foreach (LinhaDocumentoCompra linhaModel in documentoModel.Linhas)
                        {
                            if (linhaModel.Id != null)
                            {
                                LinhaDocumentoCompra linhaRascunho = documentoRascunho.Linhas.Where(l => l.Id == linhaModel.Id).FirstOrDefault();

                                // se linha enviada tem ID definido mas documento Primavera não tem linha com esse ID, é porque foi definido manualmente por utilizador
                                // limpar/ignorar ID para adicionar mais tarde
                                if (linhaRascunho == null)
                                {
                                    linhaModel.Id = null;
                                    linhaModel.NumLinha = null;
                                    continue;
                                }

                                DefinirCamposLinhaRascunho(linhaModel, linhaRascunho);

                                // atualizar NumLinha do model se Primavera o tiver alterado
                                linhaModel.NumLinha = linhaRascunho.NumLinha;
                            }
                        }

                        // adicionar linhas novas
                        foreach (LinhaDocumentoCompra linhaModel in documentoModel.Linhas)
                        {
                            if (linhaModel.Id == null)
                                AdicionarLinhaRascunho(linhaModel, documentoRascunho);
                        }

                        // reordenar linhas Primavera pela mesma ordem que Model
                        var backupLinhasRascunho = documentoRascunho.Linhas.ToList();

                        documentoRascunho.Linhas.Clear();

                        foreach (LinhaDocumentoCompra linhaModel in documentoModel.Linhas)
                        {
                            LinhaDocumentoCompra linhaPrimavera = backupLinhasRascunho.Where(l => l.Id == linhaModel.Id).First();

                            documentoRascunho.Linhas.Add(linhaPrimavera);
                        }
                    }

                    GravarAnexosDocumento(pri, documentoRascunho.Id.ToString().Trim(new char[] { '{', '}' }), documentoModel.Anexos, ficheirosAnexos, ficheirosAnexosGravados);

                    UpdateRascunho(documentoRascunho, linhasApagadas);

                    if (documentoRascunho.Estado == "R")
                         Utils.EnviaNotificacaoRejeitado(username, documentoRascunho);

                }
                catch (SqlException exception)
                {

                    Utils.TentarApagarFicheiros(ficheirosAnexosGravados);

                    throw;
                }
            }
        }

        private static void DefinirCamposRascunho(DocumentoCompra documentoModel, ref DocumentoCompra documentoRascunho)
        {
            if (documentoModel.NumDoc != null)
                documentoRascunho.NumDoc = documentoModel.NumDoc;

            if (documentoModel.TipoDoc != null)
                documentoRascunho.TipoDoc = documentoModel.TipoDoc;

            if (documentoModel.Serie != null)
                documentoRascunho.Serie = documentoModel.Serie;

            if (documentoModel.TipoEntidade != null)
                documentoRascunho.TipoEntidade = documentoModel.TipoEntidade;

            if (documentoModel.Entidade != null)
                documentoRascunho.Entidade = documentoModel.Entidade;

            if (documentoModel.NomeEntidade != null)
                documentoRascunho.NomeEntidade = documentoModel.NomeEntidade;

            if (documentoModel.DataDoc != null)
                documentoRascunho.DataDoc = documentoModel.DataDoc;

            if (documentoModel.DescFinanceiro > 0)
                documentoRascunho.DescFinanceiro = documentoModel.DescFinanceiro;

            if (documentoModel.DescFornecedor > 0)
                documentoRascunho.DescFornecedor = documentoModel.DescFornecedor;

            if (documentoModel.DataVenc != null)
                documentoRascunho.DataVenc = (DateTime)documentoModel.DataVenc;

            if (documentoModel.Observacoes != null)
                documentoRascunho.Observacoes = documentoModel.Observacoes;

            if (documentoModel.Estado != null)
                documentoRascunho.Estado = documentoModel.Estado;

            if (documentoModel.Utilizador != null)
                documentoRascunho.Utilizador = documentoModel.Utilizador;

            if (documentoModel.Aprovador != null)
                documentoRascunho.Aprovador = documentoModel.Aprovador;

            if (documentoModel.DataAprovacao != null)
                documentoRascunho.DataAprovacao = documentoModel.DataAprovacao;

            if (documentoModel.TipoDoc != null && documentoModel.Serie != null && documentoModel.NumDoc != null)
                documentoRascunho.Documento = $"{documentoModel.TipoDoc} {documentoModel.Serie}/{documentoModel.NumDoc}";

            documentoRascunho.MotivoRejeicao = documentoModel.MotivoRejeicao;

            documentoRascunho.ObraID = documentoModel.ObraID;

            documentoRascunho.Obra = documentoModel.Obra;

            documentoRascunho.NomeObra = documentoModel.NomeObra;
        }

        public static async Task<Guid> CriarRascunho(string username, string password, DocumentoCompra documentoModel, List<string> ficheirosAnexos)
        {
            List<string> ficheirosAnexosGravados = new List<string>();

            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {

                try
                {
                    pri.BSO.IniciaTransaccao();

                    DocumentoCompra documento = await CriaRascunho(pri, documentoModel, ficheirosAnexos, ficheirosAnexosGravados, username);

                    Utils.EnviaNotificacaoLeaders(username, documento);

                    pri.BSO.TerminaTransaccao();

                    return (Guid)documento.Id;
                }
                catch (Exception)
                {

                    Utils.TentarApagarFicheiros(ficheirosAnexosGravados);

                    throw;
                }
            }
        }

        private static void DefinirCamposLinhaRascunho(LinhaDocumentoCompra linhaModel, LinhaDocumentoCompra linhaRascunho)
        {
            if (linhaModel.Artigo != null)
                linhaRascunho.Artigo = linhaModel.Artigo;

            if (linhaModel.Descricao != null)
                linhaRascunho.Descricao = linhaModel.Descricao;

            if (linhaModel.Preco != null)
                linhaRascunho.Preco = linhaModel.Preco.Value;

            if (linhaModel.Quantidade != null)
                linhaRascunho.Quantidade = linhaModel.Quantidade.Value;

            if (linhaModel.Unidade != null)
                linhaRascunho.Unidade = linhaModel.Unidade;

            if (linhaModel.Total != null)
                linhaRascunho.Total = linhaModel.Total;

            if (linhaModel.DataEntrega != null)
                linhaRascunho.DataEntrega = (DateTime)linhaModel.DataEntrega;

            linhaRascunho.ObraId = linhaModel.ObraId;

            linhaRascunho.Observacoes = linhaModel.Observacoes;
        }


        private static void AdicionarLinhaRascunho(LinhaDocumentoCompra linhaModel, DocumentoCompra documentoRascunho)
        {
            var linhaRascunho = new LinhaDocumentoCompra();

            if (documentoRascunho.Id != null)
                linhaRascunho.IdCabecRasCompras = documentoRascunho.Id;

            DefinirCamposLinhaRascunho(linhaModel, linhaRascunho);

            documentoRascunho.Linhas.Add(linhaRascunho);
        }

        private static async Task<DocumentoCompra> CriaRascunho(PrimaveraConnection pri, DocumentoCompra documentoModel, List<string> ficheirosAnexos, List<string> ficheirosAnexosGravados, string username)
        {
            try
            {
                var documentoRascunho = new DocumentoCompra();

                documentoRascunho.Id = Guid.NewGuid();

                documentoRascunho.NumDoc = await GetProximoNumeroSerie(documentoModel.TipoDoc, documentoModel.Serie);

                documentoModel.Utilizador = username;

                DefinirCamposRascunho(documentoModel, ref documentoRascunho);

                foreach (LinhaDocumentoCompra linhaModel in documentoModel.Linhas)
                    AdicionarLinhaRascunho(linhaModel, documentoRascunho);

                InsertRascunho(documentoRascunho);

                GravarAnexosDocumento(pri, documentoRascunho.Id.ToString().Trim(new char[] { '{', '}' }), documentoModel.Anexos, ficheirosAnexos, ficheirosAnexosGravados);

                return documentoRascunho;
            }
            catch (Exception)
            {

                throw;
            }
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
                ValidaAnexos(pri, anexos, idDocumento);

                // apagar anexos
                // aviso: se houver vários anexos a apagar, e segundo (ou superior) anexo a apagar falhar, primeiro(s) anexo(s) vão continuar com ficheiro apagado.
                //        rollback de transação não consegue restaurar ficheiros apagados.

                StdBEAnexos anexosPrimavera = pri.BSO.DSO.Plat.Anexos.ListaAnexosTabela(EnumTabelaAnexos.anxCompras, idDocumento);

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
                Tabela = EnumTabelaAnexos.anxCompras,
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

        private static void ValidaAnexos(PrimaveraConnection pri, List<Anexo> anexos, string idDocumento)
        {
            if (anexos.Count == 0)
                return;

            var Chave = anexos.FirstOrDefault().Chave;

            if (Chave == null)
                return;

            if (!string.Equals(Chave, idDocumento))
                UpdateAnexos(pri, Chave, idDocumento);
        }

        private static void UpdateAnexos(PrimaveraConnection pri, string chave, string idDocumento)
        {
            StdBEAnexos anexosPrimavera = pri.BSO.DSO.Plat.Anexos.ListaAnexosTabela(EnumTabelaAnexos.anxCompras, chave);

            for (int i = 1; i <= anexosPrimavera.NumItens; i++)
            {
                StdBEAnexo anexoPrimavera = anexosPrimavera.GetEdita(i);

                if (string.Equals(anexoPrimavera.Chave, idDocumento) == false)
                {
                    anexoPrimavera.Chave = idDocumento;

                    pri.BSO.DSO.Plat.Anexos.Actualiza(anexoPrimavera);
                };
            }
        }



    }
}