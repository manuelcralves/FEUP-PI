using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace fbc_webapi.Autenticacao
{
    public class CustomAuthorizationFilterAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (!base.IsAuthorized(actionContext))
                return false;

            // controlar tokens gerados por nós. proteger contra tokens gerados por outros (se alguem conseguir a chave e algoritmo usado pelo .net poderiam conseguir gerar autenticações "válidas")
            IPrincipal principal = actionContext.ControllerContext.RequestContext.Principal;

            TokenGerado tokenPermitido = SimpleAuthorizationServerProvider.GetTokenGeradoUtilizadorAtual(principal);

            if (tokenPermitido == null)
                return false;
            else
                return true;
        }
    }
}