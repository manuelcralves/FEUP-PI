using CmpBE100;
using Dapper;
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;
using fbc_webapi.Primavera;
using IntBE100;
using Primavera.Extensibility.Engine.Containers.Modules;
using Primavera.Platform.Helpers;
using Serilog;
using StdBE100;
using StdPlatBE100;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http.Results;
using System.Xml.Linq;
using VndBE100;
using static StdPlatBE100.StdBETipos;

namespace fbc_webapi.Classes.DataAccessLayer
{
    public class InternosDAL
    {

        public static async Task<List<TipoDocumentosInternos>> GetTiposDocumento()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Documento, Descricao FROM DocumentosInternos WHERE CDU_Despesa_WEB = 1 ORDER BY Descricao";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        var tiposDoc = new List<TipoDocumentosInternos>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            var tipoDoc = new TipoDocumentosInternos
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

        public static async Task<List<SerieInternos>> GetSeries(string tipoDocumento)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Serie, Descricao, SeriePorDefeito FROM SeriesInternos WHERE TipoDoc = @TipoDoc AND CDU_Encomendas_WEB = 1 ORDER BY Serie DESC";

                    cmd.Parameters.AddWithValue("@TipoDoc", tipoDocumento);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        var series = new List<SerieInternos>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            var serie = new SerieInternos
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

        public static List<string> GetFilePathsPdf(PrimaveraConnection pri, string username, string password, Guid idDoc, string report, int nrVias, string path = null)
        {
            if (nrVias <= 0 || nrVias > 6)
                throw new FlorestasBemCuidadaWebApiException("Número de vias deve ser entre 1 e 6.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            StdBECampos campos = pri.BSO.Internos.Documentos.DaValorAtributosID(idDoc.ToString(), "Filial", "TipoDoc", "Serie", "NumDoc");

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
                bool impresso = pri.BSO.Internos.Documentos.ImprimeDocumento(tipoDoc, serie, numDoc, filial, nrVias, report ?? "", false, filePath);

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

        public static async Task<Guid> CriaDocumento(string username, string password, DocumentoInterno documentoModel, List<string> ficheirosAnexos)
        {
            List<string> ficheirosAnexosGravados = new List<string>();

            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                try
                {
                    pri.BSO.IniciaTransaccao();

                    AdicionaObraLinhas(ref documentoModel);

                    documentoModel.Aprovador = username;

                    IntBEDocumentoInterno documento = await CriarDocumento(pri, documentoModel, ficheirosAnexos, ficheirosAnexosGravados); 
                    
                    string filePath = IPrimavera.Utilitarios.DaCaminhoDocumento(pri, documento);

                    pri.BSO.TerminaTransaccao();

                    ImprimeDocumento(username, password, documento.ID, filePath);
                   
                    Utils.EnviaNotificacaoAprovado(username,documentoModel, documento, filePath);      

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

        public static async Task<int> GetProximoNumeroSerie(string tipoDocumento, string serie)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT MAX(NumDoc) +1 AS Numerador FROM CabecRasInternos WHERE Serie = @Serie AND TipoDoc = @TipoDoc";
                    //cmd.CommandText = "SELECT (ISNULL(Numerador, 0) + 1) Numerador FROM SeriesInternos WHERE Serie = @Serie AND TipoDoc = @TipoDoc";

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


        public static IntBEDocumentoInterno GetDocumentoCompra(string username, string password, Guid idDocumento)
        {
            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                var documentoPrimavera = pri.BSO.Internos.Documentos.EditaID(idDocumento.ToString());

                return documentoPrimavera;
            }
        }

        public static async Task<IntBEDocumentoInterno> CriarDocumento(PrimaveraConnection pri, DocumentoInterno documentoModel, List<string> ficheirosAnexos, List<string> ficheirosAnexosGravados)
        {
            var documentoPrimavera = new IntBEDocumentoInterno();

            DefinirCamposDoc(documentoModel, pri, documentoPrimavera);

            foreach (LinhaDocumentoInterno linhaModel in documentoModel.Linhas)
                await AdicionarLinha(linhaModel, pri, documentoPrimavera);

            pri.BSO.Internos.Documentos.Actualiza(documentoPrimavera);

            if (documentoModel.ObraID != null)
                pri.BSO.DSO.Plat.Registos.ExecutaComando($"UPDATE CabecInternos SET ObraId = '{documentoModel.ObraID}' WHERE Id = @Id", new List<SqlParameter>() { new SqlParameter("@Id", documentoPrimavera.ID) });

            GravarAnexosDocumento(pri, documentoPrimavera.ID.Trim(new char[] { '{', '}' }), documentoModel.Anexos, ficheirosAnexos, ficheirosAnexosGravados);

            return documentoPrimavera;
        }

        public static bool ImprimeDocumento(string username, string password, string Id, string filePath)
        {
            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true, abrirPSO: true))
            {
                try
                {
                    pri.BSO.IniciaTransaccao();

                    IntBEDocumentoInterno documento = pri.BSO.Internos.Documentos.EditaID(Id);

                    if (documento == null)
                        throw new FlorestasBemCuidadaWebApiException("ERP Primavera não conseguiu encontrar o documento.", true, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);


                    var NomeReport = DaMapaImpressao(documento.Tipodoc);

                    bool impresso = pri.BSO.Internos.Documentos.ImprimeDocumento(documento.Tipodoc, documento.Serie, documento.NumDoc, documento.Filial, 1, NomeReport ?? "GCPILS01", false, filePath);

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

                    cmd.CommandText = @" SELECT CDU_Report FROM [DocumentosInternos] WHERE Documento = @Documento";

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



        private static void AdicionaObraLinhas(ref DocumentoInterno documentoModel)
        {
            if (documentoModel.ObraID == null)
                return;

            foreach (var Linha in documentoModel.Linhas)
                Linha.ObraId = documentoModel.ObraID.ToString();
        }

        private static async Task AdicionarLinha(LinhaDocumentoInterno linhaModel, PrimaveraConnection pri, IntBEDocumentoInterno documentoPrimavera)
        {
            pri.BSO.Internos.Documentos.AdicionaLinha(documentoPrimavera, linhaModel.Artigo, linhaModel.Armazem, linhaModel.Localizacao, linhaModel.Lote, linhaModel.Preco ?? 0, 0, linhaModel.Quantidade.Value );

            var linhaPrimavera = documentoPrimavera.Linhas.GetEdita(documentoPrimavera.Linhas.NumItens);

            await DefinirCamposLinha(linhaModel, pri, documentoPrimavera, linhaPrimavera);

            if (!string.IsNullOrEmpty(linhaPrimavera.IdLinha))
                linhaModel.Id = new Guid(linhaPrimavera.IdLinha);

            linhaModel.NumLinha = documentoPrimavera.Linhas.NumItens;
        }

        private static void DefinirCamposDoc(DocumentoInterno documentoModel, PrimaveraConnection pri, IntBEDocumentoInterno documentoPrimavera)
        {
            if (!documentoPrimavera.EmModoEdicao)
            {
                documentoPrimavera.Tipodoc = documentoModel.TipoDoc;
                documentoPrimavera.Serie = documentoModel.Serie;
            }

            documentoPrimavera.TipoEntidade = documentoModel.TipoEntidade;
            documentoPrimavera.Entidade = documentoModel.Entidade;

            if (!documentoPrimavera.EmModoEdicao)
                pri.BSO.Internos.Documentos.PreencheDadosRelacionados(documentoPrimavera);


            if (documentoModel.DataDoc != null)
            {
                DateTime dataDoc = documentoModel.DataDoc.Value;

                if (documentoPrimavera.EmModoEdicao)
                    dataDoc = new DateTime(dataDoc.Year, dataDoc.Month, dataDoc.Day, documentoPrimavera.Data.Hour, documentoPrimavera.Data.Minute, documentoPrimavera.Data.Second);

                documentoPrimavera.Data = dataDoc;
            }

            if (documentoModel.DescFinanceiro > 0)
                documentoPrimavera.DescFinanceiro = documentoModel.DescFinanceiro;

            if (documentoModel.DescFornecedor > 0)
                documentoPrimavera.DescEntidade = documentoModel.DescFornecedor;

            if (documentoModel.DataVenc != null)
                documentoPrimavera.DataVencimento = (DateTime)documentoModel.DataVenc;

            if (documentoModel.Observacoes != null)
                documentoPrimavera.Observacoes = documentoModel.Observacoes;

            if (documentoModel.Aprovador != null)
                documentoPrimavera.Utilizador = documentoModel.Aprovador;

            DefinirCDU(documentoPrimavera.CamposUtil, "CDU_ObraID", documentoModel.ObraID);
        }

        private static async Task DefinirCamposLinha(LinhaDocumentoInterno linhaModel, PrimaveraConnection pri, IntBEDocumentoInterno documentoPrimavera, IntBELinhaDocumentoInterno linhaPrimavera)
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
                linhaPrimavera.ObraID = linhaModel.ObraId;

            DefinirCDU(linhaPrimavera.CamposUtil, "CDU_Observacoes", linhaModel.Observacoes);
        }

        private static void DefinirCDU(StdBECampos camposUtil, string nomeCampo, object valor)
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
                ValidaAnexos(pri, anexos, idDocumento);
                 
                // apagar anexos
                // aviso: se houver vários anexos a apagar, e segundo (ou superior) anexo a apagar falhar, primeiro(s) anexo(s) vão continuar com ficheiro apagado.
                // rollback de transação não consegue restaurar ficheiros apagados.

                StdBEAnexos anexosPrimavera = pri.BSO.DSO.Plat.Anexos.ListaAnexosTabela(EnumTabelaAnexos.anxInternos, idDocumento);

                for (int i = 1; i <= anexosPrimavera.NumItens; i++)
                {
                    StdBEAnexo anexoPrimavera = anexosPrimavera.GetEdita(i);

                    // se anexo não existir no model, é porque deve ser apagado
                    if (!anexos.Any(a => a.Id == new Guid(anexoPrimavera.IdAnexo)))
                        RemoverAnexo(pri, anexoPrimavera);
                }
            }
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
            StdBEAnexos anexosPrimavera = pri.BSO.DSO.Plat.Anexos.ListaAnexosTabela(EnumTabelaAnexos.anxInternos, chave);

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
                Tabela = EnumTabelaAnexos.anxInternos,
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

        public static async Task<List<DocumentoInterno>> GetDespesas()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
                                        SELECT
	                                        [CabecInternos].[Id],
	                                        [CabecInternos].[NumDoc],
	                                        [CabecInternos].[Serie],
	                                        [CabecInternos].[TipoDoc],
	                                        [CabecInternos].[Data],
	                                        [CabecInternos].[Documento],
	                                        [CabecInternos].[Estado],
											[CabecInternos].[Utilizador],
	                                        [CabecInternos].[ObraId] ObraId,
                                            [COP_Obras].[Codigo] Obra,
	                                        [COP_Obras].[Descricao] NomeObra,
	                                        [CabecInternos].[Entidade] Fornecedor,
	                                        [Fornecedores].[Nome] NomeFornecedor,
	                                        [CabecInternos].[TotalMerc] Total
                                        FROM 
	                                        [CabecInternos]
	                                        LEFT JOIN [COP_Obras] ON [CabecInternos].[ObraId] = [COP_Obras].[ID] 
	                                        LEFT JOIN [Fornecedores] ON [CabecInternos].[Entidade] = [Fornecedores].[Fornecedor] 
                                        WHERE
	                                        [CabecInternos].[TipoDoc] IN (SELECT [Documento] FROM [DocumentosInternos] WHERE CDU_Despesa_WEB = 1) 

                                        ORDER BY Data DESC
                                    ";


                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        var documentos = new List<DocumentoInterno>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            var documento = new DocumentoInterno
                            {
                                Id = (Guid)sqlDataReader["Id"],
                                Documento = sqlDataReader["Documento"] == DBNull.Value ? null : (string)sqlDataReader["Documento"],
                                TipoDoc = (string)sqlDataReader["TipoDoc"],
                                Serie = (string)sqlDataReader["Serie"],
                                NumDoc = (int)sqlDataReader["NumDoc"],
                                Entidade = sqlDataReader["Fornecedor"] == DBNull.Value ? null : (string)sqlDataReader["Fornecedor"],
                                NomeEntidade = sqlDataReader["NomeFornecedor"] == DBNull.Value ? null : (string)sqlDataReader["NomeFornecedor"],
                                ObraID = sqlDataReader["ObraId"] == DBNull.Value ? Guid.Empty : (Guid)sqlDataReader["ObraId"],
                                Obra = sqlDataReader["Obra"] == DBNull.Value ? null : (string)sqlDataReader["Obra"],
                                NomeObra = sqlDataReader["NomeObra"] == DBNull.Value ? null : (string)sqlDataReader["NomeObra"],
                                DataDoc = sqlDataReader["Data"] == DBNull.Value ? null : (DateTime?)sqlDataReader["Data"],
                                Aprovador = sqlDataReader["Utilizador"] == DBNull.Value ? null : (string)sqlDataReader["Utilizador"],
                                Estado = sqlDataReader["Estado"] == DBNull.Value ? null : (string)sqlDataReader["Estado"],
                                TotalDocumento = sqlDataReader["Total"] == DBNull.Value ? 0 : (double)sqlDataReader["Total"],
                            };

                            documentos.Add(documento);
                        }

                        return documentos;
                    }
                }
            }
        }

        public static async Task<DocumentoInterno> GetDespesa(Guid id, bool soCorpo = false)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
                                        SELECT
	                                        [CabecInternos].[Id],
	                                        [CabecInternos].[NumDoc],
	                                        [CabecInternos].[Serie],
	                                        [CabecInternos].[TipoDoc],
	                                        [CabecInternos].[Data],
	                                        [CabecInternos].[Documento],
	                                        [CabecInternos].[Estado],
	                                        [CabecInternos].[ObraId] Obra,
	                                        [COP_Obras].[Descricao] NomeObra,
	                                        [CabecInternos].[Entidade] Fornecedor,
	                                        [Fornecedores].[Nome] NomeFornecedor,
	                                        [CabecInternos].[TotalMerc] Total
                                        FROM 
	                                        [CabecInternos]
	                                        LEFT JOIN [COP_Obras] ON [CabecInternos].[ObraId] = [COP_Obras].[ID] 
	                                        LEFT JOIN [Fornecedores] ON [CabecInternos].[Entidade] = [Fornecedores].[Fornecedor] 
                                        WHERE
	                                        [CabecInternos].[TipoDoc] IN (SELECT [Documento] FROM [DocumentosInternos] WHERE CDU_Despesa_WEB = 1) 
                                            AND  [CabecInternos].[Id] = @Id

                                        ORDER BY Data DESC
                                    ";


                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await sqlDataReader.ReadAsync().ConfigureAwait(false) == false)
                            return null;

                        var documento = new DocumentoInterno
                        {
                            Id = (Guid)sqlDataReader["Id"],
                            Documento = sqlDataReader["Documento"] == DBNull.Value ? null : (string)sqlDataReader["Documento"],
                            TipoDoc = (string)sqlDataReader["TipoDoc"],
                            Serie = (string)sqlDataReader["Serie"],
                            NumDoc = (int)sqlDataReader["NumDoc"],
                            Entidade = sqlDataReader["Fornecedor"] == DBNull.Value ? null : (string)sqlDataReader["Fornecedor"],
                            NomeEntidade = sqlDataReader["NomeFornecedor"] == DBNull.Value ? null : (string)sqlDataReader["NomeFornecedor"],
                            ObraID = sqlDataReader["Obra"] == DBNull.Value ? Guid.Empty : (Guid)sqlDataReader["Obra"],
                            NomeObra = sqlDataReader["NomeObra"] == DBNull.Value ? null : (string)sqlDataReader["NomeObra"],
                            DataDoc = sqlDataReader["Data"] == DBNull.Value ? null : (DateTime?)sqlDataReader["Data"],
                            Estado = sqlDataReader["Estado"] == DBNull.Value ? null : (string)sqlDataReader["Estado"],
                            TotalDocumento = sqlDataReader["Total"] == DBNull.Value ? 0 : (double)sqlDataReader["Total"],
                        };

                        return documento;
                    }
                }
            }
        }

        public static async Task AlterarRascunho(string username, string password, DocumentoInterno documentoModel, Guid id, List<string> ficheirosAnexos)
        {
            var ficheirosAnexosGravados = new List<string>();

            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                try
                {
                    DocumentoInterno documentoRascunho = await GetRascunho(id);

                    if (documentoRascunho == null)
                        throw new FlorestasBemCuidadaWebApiException("Documento não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

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
                        foreach (LinhaDocumentoInterno linhaModel in documentoModel.Linhas)
                        {
                            if (linhaModel.Id != null)
                            {
                                LinhaDocumentoInterno linhaRascunho = documentoRascunho.Linhas.Where(l => l.Id == linhaModel.Id).FirstOrDefault();

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
                        foreach (LinhaDocumentoInterno linhaModel in documentoModel.Linhas)
                        {
                            if (linhaModel.Id == null)
                                AdicionarLinhaRascunho(linhaModel, documentoRascunho);
                        }

                        // reordenar linhas Primavera pela mesma ordem que Model
                        var backupLinhasRascunho = documentoRascunho.Linhas.ToList();

                        documentoRascunho.Linhas.Clear();

                        foreach (LinhaDocumentoInterno linhaModel in documentoModel.Linhas)
                        {
                            LinhaDocumentoInterno linhaPrimavera = backupLinhasRascunho.Where(l => l.Id == linhaModel.Id).First();

                            documentoRascunho.Linhas.Add(linhaPrimavera);
                        }
                    }

                    UpdateRascunho(documentoRascunho, linhasApagadas);

                    GravarAnexosDocumento(pri, documentoRascunho.Id.ToString().Trim(new char[] { '{', '}' }), documentoModel.Anexos, ficheirosAnexos, ficheirosAnexosGravados);

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

        private static void DefinirCamposRascunho(DocumentoInterno documentoModel, ref DocumentoInterno documentoRascunho)
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

        private static void AdicionarLinhaRascunho(LinhaDocumentoInterno linhaModel, DocumentoInterno documentoRascunho)
        {
            var linhaRascunho = new LinhaDocumentoInterno();

            if (documentoRascunho.Id != null)
                linhaRascunho.IdCabecRasInternos = documentoRascunho.Id;

            DefinirCamposLinhaRascunho(linhaModel, linhaRascunho);

            documentoRascunho.Linhas.Add(linhaRascunho);
        }


        private static void DefinirCamposLinhaRascunho(LinhaDocumentoInterno linhaModel, LinhaDocumentoInterno linhaRascunho)
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


        public static async Task<Guid> CriarRascunho( string username, string password, DocumentoInterno documentoModel, List<string> ficheirosAnexos)
        {
            List<string> ficheirosAnexosGravados = new List<string>();

            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                try
                {
                    pri.BSO.IniciaTransaccao();

                    DocumentoInterno documento = await CriaRascunho(pri, documentoModel, ficheirosAnexos, ficheirosAnexosGravados, username);

                    Utils.EnviaNotificacaoLeaders(username, documento);

                    pri.BSO.TerminaTransaccao();

                    return (Guid)documento.Id;
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

        private static async Task ValidaDocumentoInterno(PrimaveraConnection pri, DocumentoInterno documentoModel)
        {
            string erros = "";
            var documentoPrimavera = new IntBEDocumentoInterno();

            DefinirCamposDoc(documentoModel, pri, documentoPrimavera);

            foreach (LinhaDocumentoInterno linhaModel in documentoModel.Linhas)
                await AdicionarLinha(linhaModel, pri, documentoPrimavera);

            pri.BSO.Internos.Documentos.ValidaActualizacao(documentoPrimavera, ref erros);

            if (!string.IsNullOrWhiteSpace(erros))
                throw new FlorestasBemCuidadaWebApiException(erros,true);
        }

        private static async Task<DocumentoInterno> CriaRascunho(PrimaveraConnection pri, DocumentoInterno documentoModel, List<string> ficheirosAnexos, List<string> ficheirosAnexosGravados, string username)
        {
            try
            {
                var documentoRascunho = new DocumentoInterno();

                documentoRascunho.Id = Guid.NewGuid();

                documentoRascunho.NumDoc = await GetProximoNumeroSerie(documentoModel.TipoDoc, documentoModel.Serie);

                documentoModel.Utilizador = username;

                DefinirCamposRascunho(documentoModel, ref documentoRascunho);

                foreach (LinhaDocumentoInterno linhaModel in documentoModel.Linhas)
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

        public static void DeleteLinhaRascunho(Guid id, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                conn.Execute(@"DELETE FROM LinhasRasInternos WHERE Id = @Id", new { Id = id }, tran);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task DeleteRascunho(Guid id)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute(@"DELETE FROM LinhasRasInternos WHERE IdCabecRasInternos = @Id", new { Id = id }, tran);

                        conn.Execute(@"DELETE FROM CabecRasInternos WHERE Id = @Id", new { Id = id }, tran);

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

        public static void UpdateLinhaRascunho(LinhaDocumentoInterno Linha, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                conn.Execute(@"
                    UPDATE 
                        [LinhasRasInternos]
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

        public static void UpdateCabecRascunho(DocumentoInterno documentoModel, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                conn.Execute(@"
                    UPDATE 
                        CabecRasInternos
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

        public static void UpdateRascunho(DocumentoInterno documentoModel, List<Guid> LinhasApagadas)
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
                         
                            foreach(var linha in LinhasApagadas)
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

        public static void InsertLinhaRascunho(LinhaDocumentoInterno Linha, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                conn.Execute(@"INSERT INTO LinhasRasInternos (IdCabecRasInternos, Artigo, Descricao, Quantidade, Unidade, [Preco], [Total], [ObraId], [DataEntrega], Observacoes) 
                                                VALUES(@IdCabecRasInternos, @Artigo, @Descricao, @Quantidade, @Unidade, @Preco, @Total, @ObraId, @DataEntrega, @Observacoes)", Linha, tran);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void InsertRascunho(DocumentoInterno documentoModel)
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
                             conn.Execute(@"INSERT INTO CabecRasInternos (Id, Documento, TipoDoc, Serie, NumDoc, Estado, DataDoc, DataVenc, TipoEntidade, Entidade, NomeEntidade, DescFornecedor, DescFinanceiro, ObraId, Obra, NomeObra, Utilizador, Observacoes) 
                                            VALUES(@Id, @Documento, @TipoDoc, @Serie, @NumDoc, @Estado, @DataDoc, @DataVenc, @TipoEntidade, @Entidade, @NomeEntidade, @DescFornecedor, @DescFinanceiro, @ObraId, @Obra, @NomeObra, @Utilizador, @Observacoes)", documentoModel, tran);

                            foreach (var Linha in documentoModel.Linhas)
                                InsertLinhaRascunho(Linha,conn,tran);
                            
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


        public static async Task<DocumentoInterno> GetRascunho( string username, string password, Guid id, bool soCorpo = false)
        {
            DocumentoInterno documento =  await GetRascunho(id, soCorpo);

            await GetAnexosDocumentoInternos(id.ToString().Trim(new char[] { '{', '}' }), documento, username, password).ConfigureAwait(false);

            return documento;
        }

        public static async Task<DocumentoInterno> GetRascunho(Guid id, bool soCorpo = false)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
                {
                    var documento =  await conn.QueryAsync<DocumentoInterno>("SELECT * FROM CabecRasInternos WHERE Id = @Id", new { Id = id });

                    if (documento == null)
                        return null;

                    var linhas = await conn.QueryAsync<LinhaDocumentoInterno>("SELECT * FROM LinhasRasInternos Where IdCabecRasInternos = @IdCabecRasInternos", new { IdCabecRasInternos = id });

                    if (linhas != null)
                        documento.First().Linhas = linhas.ToList();

   
                    return documento.First();
                }
            }
            catch (Exception){   
                
                throw;
            }

        }

    
        private static async Task GetAnexosDocumentoInternos(string id, DocumentoInterno documento, string username, string password)
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

        private static void PreencherTamanhoAnexos(DocumentoInterno documento, string username, string password)
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



        public static async Task<List<DocumentoInterno>> GetRascunhosAprovacao(string utilizador)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                var query = new StringBuilder();

                query.Append(@"SELECT
	                                CRI.Id,
	                                CRI.TipoDoc,
	                                CRI.Serie,
	                                CRI.NumDoc,
	                                CRI.DataDoc,
	                                CRI.Obra,
	                                CRI.NomeObra,
	                                CRI.Entidade,
	                                CRI.NomeEntidade,
	                                CRI.Documento,
	                                CRI.TotalDocumento,
	                                CRI.Utilizador,
									E.Descricao As Equipa

                                FROM
	                                CabecRasInternos CRI
                                    JOIN [PRIEMPRE].[dbo].[MembrosEquipa] ME ON CRI.Utilizador = ME.Utilizador
                                    JOIN [PRIEMPRE].[dbo].[LeadersEquipa] LE ON ME.CodEquipa = LE.CodEquipa 
                                    JOIN [PRIEMPRE].[dbo].[Equipas] E ON E.Codigo = LE.CodEquipa

                                WHERE
	                                E.Activa = 1
	                                AND LE.Utilizador = @Utilizador
	                                AND CRI.Estado NOT IN ('A','R')");


                var documentos = await conn.QueryAsync<DocumentoInterno>(query.ToString(), new { Utilizador = utilizador});

                return documentos.ToList();
            }
        }

        public static async Task<List<DocumentoInterno>> GetRascunhos(string utilizador = null, string estado = null)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                var query = new StringBuilder();

                query.Append(@"SELECT * FROM CabecRasInternos WHERE 1=1 ");

                if (!string.IsNullOrWhiteSpace(utilizador))
                    query.AppendLine($"AND Utilizador = @Utilizador ");

                if (!string.IsNullOrWhiteSpace(estado))
                    query.AppendLine($"AND Estado = @Estado ");

                var documentos = await conn.QueryAsync<DocumentoInterno>(query.ToString(), new { Utilizador = utilizador, Estado = estado });

                return documentos.ToList();
            }
        }


    }
}