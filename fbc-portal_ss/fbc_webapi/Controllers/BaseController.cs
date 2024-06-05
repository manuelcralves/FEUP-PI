using Newtonsoft.Json;
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
using fbc_webapi.Classes.BusinessLogicLayer;
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;
using fbc_webapi.Classes;
using System.Web.Services.Description;

namespace fbc_webapi.Controllers
{
    [CustomAuthorizationFilter]
    [RoutePrefix("api/Base")]
    public class BaseController : ApiController
    {
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Empresa>))]
        public async Task<IHttpActionResult> Empresas()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetEmpresas(tokenUser.Username).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Equipas/Utilizador")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Equipa>))]
        public async Task<IHttpActionResult> GetEquipasUtilizador()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetEquipasUtilizador(tokenUser.Username).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Equipas")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Equipa>))]
        public async Task<IHttpActionResult> GetEquipas()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetEquipas(tokenUser.Username).ConfigureAwait(false));
        }


        [HttpGet]
        [Route("Equipas/{codigo}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DocumentoCompra>))]
        public async Task<IHttpActionResult> GetEquipa(string codigo)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetEquipa(tokenUser.Username, codigo).ConfigureAwait(false));
        }

        [HttpDelete]
        [Route("Equipas/{codigo}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void))]
        public async Task<IHttpActionResult> ApagarEquipa(string codigo)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            await BaseBLL.ApagarEquipa(tokenUser.Username, codigo).ConfigureAwait(false);

            return Ok();
        }

        [HttpPost]
        [Route("Equipa")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void))]
        public async Task<IHttpActionResult> CriarRequisicaoInterna(Equipa equipa)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

           await BaseBLL.CriarEquipa(tokenUser.Username, equipa).ConfigureAwait(false);

            return Ok();
        }

        [HttpPut]
        [Route("Equipa")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void))]
        public async Task<IHttpActionResult> AlteraEquipa(Equipa equipa)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            await BaseBLL.AlteraEquipa(tokenUser.Username, equipa).ConfigureAwait(false);

            return Ok();
        }
        [HttpDelete]
        [Route("Equipa/Membro/{utilizador}/{equipa}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void))]
        public async Task<IHttpActionResult> ApagarMembroEquipa(string utilizador, string equipa)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            await BaseBLL.ApagarMembroEquipa(tokenUser.Username, utilizador, equipa).ConfigureAwait(false);

            return Ok();
        }

        [HttpPost]
        [Route("Equipa/Membro")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void))]
        public async Task<IHttpActionResult> AdicionaMembroEquipa(Membro membro)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            await BaseBLL.AdicionaMembroEquipa(tokenUser.Username, membro).ConfigureAwait(false);

            return Ok();
        }

        [HttpGet]
        [Route("Exclusoes")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Exclusao>))]
        public async Task<IHttpActionResult> Exclusoes()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetExclusoes().ConfigureAwait(false));
        }

        [HttpGet]
        [Route("CondicoesPagamento")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<CondicaoPagamento>))]
        public async Task<IHttpActionResult> CondicoesPagamento()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetCondicoesPagamento(tokenUser.Username).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("CondicoesPagamento/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CondicaoPagamento))]
        public async Task<IHttpActionResult> CondicaoPagamento(string id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetCondicaoPagamento(tokenUser.Username, id).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Unidades")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Unidade>))]
        public async Task<IHttpActionResult> Unidades()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetUnidades().ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Unidades/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Unidade))]
        public async Task<IHttpActionResult> Unidade(string id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetUnidade(id).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("MoradasAlternativas/{cliente}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<MoradaAlternativa>))]
        public async Task<IHttpActionResult> MoradasAlternativas(string cliente)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetMoradasAlternativas(tokenUser.Username, cliente).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("MoradasFornecedores/{fornecedor}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<MoradaFornecedor>))]
        public async Task<IHttpActionResult> MoradasFornecedores(string fornecedor)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetMoradasFornecedores( tokenUser.Username, fornecedor).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("MoradasArmazens")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<MoradaArmazem>))]
        public async Task<IHttpActionResult> MoradasArmazens()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetMoradasArmazens(tokenUser.Username).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("MoradasAlternativas/{cliente}/{morada}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<MoradaAlternativa>))]
        public async Task<IHttpActionResult> MoradaAlternativa(string cliente, string morada)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetMoradaAlternativa(tokenUser.Username, cliente, morada).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("MoradasFornecedores/{fornecedor}/{morada}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<MoradaFornecedor>))]
        public async Task<IHttpActionResult> MoradasFornecedores(string fornecedor, string morada)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetMoradaFornecedores(tokenUser.Username, fornecedor, morada).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("MoradasArmazens/{morada}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<MoradaArmazem>))]
        public async Task<IHttpActionResult> MoradaArmazem(string morada)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetMoradaArmazem( morada).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Obras")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Obra>))]
        public async Task<IHttpActionResult> Obras()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetObras().ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Obras/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Obra))]
        public async Task<IHttpActionResult> Obra(string id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetObra(id).ConfigureAwait(false));
        }


        [HttpGet]
        [Route("Clientes")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Cliente>))]
        public async Task<IHttpActionResult> Clientes()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetClientes().ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Clientes/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Cliente))]
        public async Task<IHttpActionResult> Clientes(string id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetCliente(id).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Fornecedores")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Fornecedor>))]
        public async Task<IHttpActionResult> GetFornecedores()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetFornecedores().ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Fornecedores/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Fornecedor))]
        public async Task<IHttpActionResult> GetFornecedor(string id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetFornecedor(id).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Artigos")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Artigo>))]
        public async Task<IHttpActionResult> Artigos()
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetArtigos().ConfigureAwait(false));
        }

        [HttpGet]
        [Route("Artigos/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Artigo))]
        public async Task<IHttpActionResult> Artigos(string id)
        {
            TokenGerado tokenUser = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(RequestContext.Principal);

            return Ok(await BaseBLL.GetArtigo(id).ConfigureAwait(false));
        }


    }
}
