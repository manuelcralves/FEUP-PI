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
    [RoutePrefix("api/Internos")]
    public class InternosController : ApiController
    {

        [HttpPost]
        [Route("Documento")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Guid))]
        public async Task<IHttpActionResult> CriarDocumento()
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

                DocumentoInterno documento = JsonConvert.DeserializeObject<DocumentoInterno>(documentoJson);

                Guid idCabecDoc = await InternosBLL.CriaDocumento(tokenUser.Username, documento, ficheirosAnexos).ConfigureAwait(false);

                return Ok(idCabecDoc);
            }
            finally
            {
                Utils.TentarApagarFicheirosTemporarios(ficheirosAnexos);
            }
        }


        [HttpGet]
        [Route("TiposDocumento")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TipoDocumentosInternos>))]
        public async Task<IHttpActionResult> TiposDocumento()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await InternosBLL.GetTiposDocumento().ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Series")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SerieInternos>))]
        public async Task<IHttpActionResult> Series(string tipoDocumento)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await InternosBLL.GetSeries(tipoDocumento).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("ProximoNumeroSerie")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(int))]
        public async Task<IHttpActionResult> ProximoNumeroSerie(string tipoDocumento, string serie)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await InternosBLL.GetProximoNumeroSerie(tipoDocumento, serie).ConfigureAwait(false));
        }
        
        [HttpGet]
        [Route("Despesas")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DocumentoInterno>))]
        public async Task<IHttpActionResult> GetDespesas()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await InternosBLL.GetDespesas().ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Despesas/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DocumentoInterno>))]
        public async Task<IHttpActionResult> GetDespesas(Guid id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await InternosBLL.GetDespesa(id).ConfigureAwait(false));
        }


        [HttpGet]
        [Route("Rascunhos")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DocumentoInterno>))]
        public async Task<IHttpActionResult> GetRascunhos()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await InternosBLL.GetRascunhos(tokenUser.Username).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Rascunhos/Aprovacao")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DocumentoInterno>))]
        public async Task<IHttpActionResult> GetRascunhosAprovacao()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await InternosBLL.GetRascunhosAprovacao(tokenUser.Username).ConfigureAwait(false));
        }


        [HttpGet]
        [Route("Rascunhos/Utilizador")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DocumentoInterno>))]
        public async Task<IHttpActionResult> GetRascunhosUtilizador()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await InternosBLL.GetRascunhosUtilizador(tokenUser.Username).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Rascunhos/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DocumentoInterno>))]
        public async Task<IHttpActionResult> GetRascunho(Guid id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await InternosBLL.GetRascunho(tokenUser.Username, id).ConfigureAwait(false));
        }
        

        [HttpDelete]
        [Route("Rascunhos/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void))]
        public async Task<IHttpActionResult> AnularRascunho(Guid id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            await InternosBLL.AnularRascunho(tokenUser.Username, id).ConfigureAwait(false);

            return Ok();
        }

        [HttpPut]
        [Route("Rascunhos/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void))]
        public async Task<IHttpActionResult> AlteraRascunho(Guid id)
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

                DocumentoInterno documento = JsonConvert.DeserializeObject<DocumentoInterno>(documentoJson);

                await InternosBLL.AlterarRascunho(tokenUser.Username, id, documento, ficheirosAnexos).ConfigureAwait(false);

                return Ok();
            }
            finally
            {
                Utils.TentarApagarFicheirosTemporarios(ficheirosAnexos);
            }
        }

        [HttpPost]
        [Route("Rascunhos")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Guid))]
        public async Task<IHttpActionResult> PostRascunho()
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

                DocumentoInterno documento = JsonConvert.DeserializeObject<DocumentoInterno>(documentoJson);

                Guid idCabecDoc = await InternosBLL.CriarRascunho(tokenUser.Username, documento, ficheirosAnexos).ConfigureAwait(false);

                return Ok(idCabecDoc);
            }
            finally
            {
                Utils.TentarApagarFicheirosTemporarios(ficheirosAnexos);
            }
        }

    }
}
