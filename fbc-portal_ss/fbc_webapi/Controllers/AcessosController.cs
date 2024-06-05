using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using fbc_webapi.Autenticacao;
using fbc_webapi.Classes.BusinessLogicLayer;
using fbc_webapi.Models;

namespace fbc_webapi.Controllers
{
    [CustomAuthorizationFilter]
    [RoutePrefix("api/Acessos")]
    public class AcessosController : ApiController
    {


        [HttpPut]
        [Route("VerificaEmpresasUtilizador/{codigoUtil}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void))]
        public async Task<IHttpActionResult> VerificaEmpresasUtilizador(string codigoUtil)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            await AcessosBLL.VerificaEmpresasUtilizador(tokenUser.Username, codigoUtil).ConfigureAwait(false);

            return Ok();
        }

        [HttpGet]
        [Route("PermissoesUtilizadorAtual")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<string>))]
        public async Task<IHttpActionResult> GetPermissoesUtilizadorAtual()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await AcessosBLL.GetPermissoesUtilizadorAtual(tokenUser.Username).ConfigureAwait(false));
        }


        [HttpGet]
        [Route("Utilizadores")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Utilizador>))]
        public async Task<IHttpActionResult> GetUtilizadores()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await AcessosBLL.GetUtilizadores(tokenUser.Username).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Utilizadores/{codigoUtilizador}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Utilizador))]
        public async Task<IHttpActionResult> GetUtilizador(string codigoUtilizador)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await AcessosBLL.GetUtilizador(tokenUser.Username, codigoUtilizador).ConfigureAwait(false));
        }


    }
}
