using Newtonsoft.Json;
using Serilog;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using fbc_webapi.Autenticacao;
using fbc_webapi.Classes;
using fbc_webapi.Classes.BusinessLogicLayer;
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;

namespace fbc_webapi.Controllers
{
    [CustomAuthorizationFilter]
    [RoutePrefix("api/Vendas")]
    public class VendasController : ApiController
    {
        [HttpGet]
        [Route("TiposDocumento")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TipoDocumentosVendas>))]
        public async Task<IHttpActionResult> TiposDocumento(int? tipoTipoDocumento)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await VendasBLL.GetTiposDocumento(tipoTipoDocumento).ConfigureAwait(false));
        }


        [HttpGet]
        [Route("Series")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SerieVendas>))]
        public async Task<IHttpActionResult> Series(string tipoDocumento)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await VendasBLL.GetSeries(tipoDocumento).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("TiposDocumentoEncomendaCliente")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TipoDocumentosVendas>))]
        public async Task<IHttpActionResult> TiposDocumentoEncomendaCliente()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await VendasBLL.GetTiposDocumentoEncomendaCliente(tokenUser.Username).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("ProximoNumeroSerie")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(int))]
        public async Task<IHttpActionResult> ProximoNumeroSerie(string tipoDocumento, string serie)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await VendasBLL.GetProximoNumeroSerie(tipoDocumento, serie).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Vendedores")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Vendedor>))]
        public async Task<IHttpActionResult> Vendedores()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await VendasBLL.GetVendedores(tokenUser.Username).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Vendedores/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Vendedor))]
        public async Task<IHttpActionResult> GetVendedores(string id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await VendasBLL.GetVendedor(tokenUser.Username, id).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Reports/{tipoDoc}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Report>))]
        public async Task<IHttpActionResult> GetReportsVenda(string tipoDoc)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await VendasBLL.GetReportsVenda(tokenUser.Username, tipoDoc).ConfigureAwait(false));
        }


        [HttpGet]
        [Route("PropostasAbertas/{menu}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DocumentoVenda>))]
        public async Task<IHttpActionResult> GetPropostasAbertas(string menu)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await VendasBLL.GetPropostasAbertas(tokenUser.Username, menu).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("PropostasAbertas/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(DocumentoVenda))]
        public async Task<IHttpActionResult> GetPropostaAberta(Guid id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await VendasBLL.GetPropostaAberta(tokenUser.Username, id).ConfigureAwait(false));
        }

        [HttpPost]
        [Route("AnularPropostaAberta/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void))]
        public async Task<IHttpActionResult> AnularPropostaAberta(Guid id, EstornoDocumento dadosEstorno)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            await VendasBLL.AnularPropostaAberta(tokenUser.Username, id, dadosEstorno).ConfigureAwait(false);

            return Ok();
        }

        [HttpPut]
        [Route("PropostasAbertas/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void))]
        public async Task<IHttpActionResult> PutPropostasAbertas(Guid id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            if (!Request.Content.IsMimeMultipartContent() && !Request.Content.Headers.ContentType?.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase) == true)
                throw new FlorestasBemCuidadaWebApiException("Tipo de pedido não suportado. Deve ser multipart/form-data ou application/json.", false, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);

            string documentoJson;
            List<string> ficheirosAnexos = new List<string>();

            try
            {
                if (Request.Content.IsMimeMultipartContent())
                {
                    ficheirosAnexos = await Utils.GetFicheirosEnviados("ficheirosAnexos");
                    documentoJson = HttpContext.Current.Request.Form["documento"];
                }
                else
                {
                    StreamReader bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
                    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);

                    documentoJson = bodyStream.ReadToEnd();
                }

                if (string.IsNullOrEmpty(documentoJson))
                    throw new FlorestasBemCuidadaWebApiException("Documento é obrigatório.", false, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

                DocumentoVenda documento = JsonConvert.DeserializeObject<DocumentoVenda>(documentoJson);

                await VendasBLL.AlterarPropostaAberta(tokenUser.Username, id, documento, ficheirosAnexos).ConfigureAwait(false);

                return Ok();
            }
            finally
            {
                Utils.TentarApagarFicheirosTemporarios(ficheirosAnexos);
            }
        }

        [HttpPost]
        [Route("PropostasAbertas")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Guid))]
        public async Task<IHttpActionResult> PostPropostasAbertas()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            if (!Request.Content.IsMimeMultipartContent() && !Request.Content.Headers.ContentType?.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase) == true)
                throw new FlorestasBemCuidadaWebApiException("Tipo de pedido não suportado. Deve ser multipart/form-data ou application/json.", false, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);

            string documentoJson;
            List<string> ficheirosAnexos = new List<string>();

            try
            {
                if (Request.Content.IsMimeMultipartContent())
                {
                    ficheirosAnexos = await Utils.GetFicheirosEnviados("ficheirosAnexos");
                    documentoJson = HttpContext.Current.Request.Form["documento"];
                }
                else
                {
                    StreamReader bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
                    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);

                    documentoJson = bodyStream.ReadToEnd();
                }

                if (string.IsNullOrEmpty(documentoJson))
                    throw new FlorestasBemCuidadaWebApiException("Documento é obrigatório.", false, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

                DocumentoVenda documento = JsonConvert.DeserializeObject<DocumentoVenda>(documentoJson);

                Guid idCabecDoc = await VendasBLL.CriarPropostaAberta(tokenUser.Username, documento, ficheirosAnexos).ConfigureAwait(false);

                return Ok(idCabecDoc);
            }
            finally
            {
                Utils.TentarApagarFicheirosTemporarios(ficheirosAnexos);
            }
        }

        [HttpGet]
        [Route("GerarTokenDownloadAnexoProposta/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        public async Task<IHttpActionResult> GerarTokenDownloadAnexoProposta(Guid id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            Anexo anexo = await VendasBLL.GetAnexoProposta(tokenUser.Username, id).ConfigureAwait(false);

            if (!File.Exists(anexo.FilePath))
            {
                Log.Error("Ficheiro de anexo '{IdAnexo}' não foi encontrado em '{FilePath}'. A aplicação pode não ter permissões ou ficheiro pode ter sido apagado.", id, anexo.FilePath);
                throw new FlorestasBemCuidadaWebApiException("Ficheiro de anexo não existe.", true, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);
            }

            string downloadToken = AcessoDownloadFicheiros.CriarAcessoAnexoProposta(anexo.FilePath, anexo.FicheiroOrig, tokenUser.IdToken);

            return Ok(downloadToken);
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(byte[]))]
        public IHttpActionResult DownloadAnexoProposta(string downloadToken)
        {
            TokenAcessoFicheiro tokenAcessoFicheiro = AcessoDownloadFicheiros.GetTokenAcessoAnexoProposta(downloadToken);

            AcessoDownloadFicheiros.DeleteTokenAcessoAnexoProposta(downloadToken);

            return DevolverFicheiro(tokenAcessoFicheiro, false, false);
        }

        [HttpGet]
        [Route("GerarTokenDownloadPdfProposta/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<string>))]
        public async Task<IHttpActionResult> GerarTokenDownloadPdfProposta(Guid id, string report, int nrVias)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            List<string> pdfFilePaths = await VendasBLL.GetFilePathsPdfProposta(tokenUser.Username, id, report, nrVias).ConfigureAwait(false);

            List<string> downloadTokens = new List<string>();

            foreach (string pdfFilePath in pdfFilePaths)
            {
                string downloadToken = AcessoDownloadFicheiros.CriarAcessoPdfProposta(pdfFilePath, Path.GetFileName(pdfFilePath), tokenUser.IdToken);

                downloadTokens.Add(downloadToken);
            }

            return Ok(downloadTokens);
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(byte[]))]
        public IHttpActionResult DownloadPdfProposta(string downloadToken, bool previsualizar)
        {
            TokenAcessoFicheiro tokenAcessoFicheiro = AcessoDownloadFicheiros.GetTokenAcessoPdfProposta(downloadToken);

            AcessoDownloadFicheiros.DeleteTokenAcessoPdfProposta(downloadToken, false);

            return DevolverFicheiro(tokenAcessoFicheiro, previsualizar, true);
        }

        private IHttpActionResult DevolverFicheiro(TokenAcessoFicheiro tokenAcessoFicheiro, bool previsualizar, bool deleteFile)
        {
            if (tokenAcessoFicheiro == null)
                throw new FlorestasBemCuidadaWebApiException("Token de download inválido.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGerado(tokenAcessoFicheiro.IdTokenGeradoAutenticacao);

            if (tokenUser == null)
                throw new FlorestasBemCuidadaWebApiException("Autenticação expirou.", true, EnumErrorCode.Geral, HttpStatusCode.Unauthorized);

            if (string.IsNullOrEmpty(tokenAcessoFicheiro.FilePath))
                throw new FlorestasBemCuidadaWebApiException("FilePath vazio.", false, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);

            if (string.IsNullOrEmpty(tokenAcessoFicheiro.FileName))
                throw new FlorestasBemCuidadaWebApiException("FileName vazio.", false, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);

            CustomFileStream fileStream = new CustomFileStream(tokenAcessoFicheiro.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None, deleteFile);

            try
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(fileStream),
                };

                string fileExtension = Path.GetExtension(tokenAcessoFicheiro.FileName);

                if (!previsualizar || fileExtension?.Equals(".pdf", StringComparison.OrdinalIgnoreCase) != true)
                {
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = tokenAcessoFicheiro.FileName,
                    };

                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                }
                else
                {
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
                    {
                        FileName = tokenAcessoFicheiro.FileName,
                    };

                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                }

                return ResponseMessage(response);
            }
            catch (Exception)
            {
                fileStream.Dispose();

                throw;
            }
        }


    }
}
