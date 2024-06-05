using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Primavera.Platform.Exceptions;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Timers;
using fbc_webapi.Classes.BusinessLogicLayer;
using fbc_webapi.Classes.DataAccessLayer;
using fbc_webapi.Models;

namespace fbc_webapi.Autenticacao
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private static ConcurrentDictionary<Guid, TokenGerado> TokensPermitidos = new ConcurrentDictionary<Guid, TokenGerado>(); // controlar tokens gerados por nós. proteger contra tokens gerados por outros (se alguem conseguir a chave e algoritmo usado pelo .net poderiam conseguir gerar autenticações "válidas")
        private static Timer TokensExpirationCheckTimer = new Timer(); // limpar tokens antigos periodicamente (evitar que lista cresca muito)

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (!context.TryGetBasicCredentials(out string clientId, out string clientSecret) && !context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                context.SetError("invalid_request", "Deve enviar o ClientId no pedido do token.");
                return Task.FromResult(0);
            }

            if (string.IsNullOrEmpty(Config.ClientIdAutenticacao))
            {
                context.SetError("server_error", "Ocorreu um erro inesperado no servidor. Faltam configurações de autenticação.");
                return Task.FromResult(0);
            }

            if (string.IsNullOrEmpty(clientId) || !string.Equals(clientId, Config.ClientIdAutenticacao))
            {
                context.SetError("unauthorized_client", "Cliente não autorizado.");
                return Task.FromResult(0);
            }

            context.Validated();

            return Task.FromResult(0);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Utilizador utilizador;
             
            try
            {
                utilizador = AcessosBLL.AutenticarUtilizador(context.UserName, context.Password);

                if (utilizador == null)
                    throw new Exception($"Variável '{nameof(utilizador)}' foi devolvida a null em AutenticarUtilizador.");

                if (string.IsNullOrEmpty(utilizador.Codigo))
                    throw new Exception($"Código de utilizador é obrigatório.");

                utilizador = await AcessosDAL.GetUtilizador(utilizador.Codigo).ConfigureAwait(false);

                if (utilizador == null)
                    throw new Exception($"Variável '{nameof(utilizador)}' foi devolvida a null em GetUtilizador.");

            }
            catch (ExpectedException ex)
            {
                if (ex.Message.IndexOf("Autenticação do utilizador falhou.", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    Log.Error(ex, "Erro a validar autenticação de utilizador. Provavelmente será só devido a autenticações erradas.");

                    context.SetError("invalid_grant", "Utilizador ou password incorretos.");
                    return;
                }
                else if (ex.Message.IndexOf("Utilizador não tem permissões para aceder à empresa.", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    Log.Error(ex, "Erro a validar autenticação de utilizador. Provavelmente será só devido a permissões de acesso à empresa.");

                    context.SetError("access_denied", "Não tem permissões de acesso à empresa mestre.");
                    return;
                }
                else
                {
                    Log.Error(ex, "Erro a validar autenticação de utilizador. Pode haver configurações mal definidas ou a instalação Primavera pode ter algum problema.");

                    context.SetError("server_error", "Ocorreu um erro inesperado no servidor. Não foi possivel autenticar.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro a validar autenticação de utilizador");

                context.SetError("server_error", "Ocorreu um erro inesperado no servidor. Não foi possivel autenticar.");
                return;
            }

            Guid idToken = Guid.NewGuid();

            ClaimsIdentity identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("username", utilizador.Codigo));
            identity.AddClaim(new Claim("idToken", idToken.ToString()));
            identity.AddClaim(new Claim("role", "user"));

            AuthenticationProperties props = new AuthenticationProperties(new Dictionary<string, string>());
            props.Dictionary.Add("codigoUtilizador", utilizador.Codigo);
            props.Dictionary.Add("nomeUtilizador", utilizador.Nome);
            props.Dictionary.Add("adminUtilizador", utilizador.Administrador == true ? "S" : "N");


            AuthenticationTicket ticket = new AuthenticationTicket(identity, props);

            TokenGerado token = TokensPermitidos.GetOrAdd(idToken, new TokenGerado()
            {
                IdToken = idToken,
                Username = utilizador.Codigo,
                PasswordEncriptada = PasswordCryptorEngine.Encrypt(context.Password),
            });

            if (!string.Equals(token.Username, utilizador.Codigo))
                throw new Exception($"Não foi possísvel adicionar token a dicionário. Já existia um com o mesmo id mas username diferente. Username de tentativa: '{utilizador.Codigo}'.");

            if (TokensPermitidos.Count > 20000)
                Log.Warning("Existem {@NrTokens} tokens de autenticação em memória. Performance pode começar a ser afetada em algumas situações. Reiniciar a aplicação pode ajudar.", TokensPermitidos.Count);

            context.Validated(ticket);

            return;
        }

        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            if (context.TokenIssued)
            {
                Claim idTokenClaim = context.Identity.FindFirst("idToken");

                Guid idToken = Guid.Parse(idTokenClaim.Value);

                TokensPermitidos[idToken].DataUtcGerado = context.Properties.IssuedUtc;
                TokensPermitidos[idToken].DataUtcExpiracao = context.Properties.ExpiresUtc;

                foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
                    context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public static TokenGerado GetTokenGeradoUtilizadorAtual(IPrincipal principal)
        {
            if (!(principal is ClaimsPrincipal claimsPrincipal))
                return null;

            Claim usernameClaim = claimsPrincipal.FindFirst("username");
            Claim idTokenClaim = claimsPrincipal.FindFirst("idToken");

            if (usernameClaim == null || idTokenClaim == null)
                return null;

            if (!Guid.TryParse(idTokenClaim.Value, out Guid idToken))
                return null;

            TokenGerado token = GetTokenGerado(idToken);

            if (token != null && token.Username != usernameClaim.Value)
            {
                Log.Warning("Foi obtido um token com username no objeto diferente do username no token HTTP. Não deveria acontecer.");
                return null;
            }

            return token;
        }

        public static TokenGerado GetTokenGerado(Guid idToken)
        {
            if (TokensPermitidos.TryGetValue(idToken, out TokenGerado tokenGerado))
            {
                if (tokenGerado.IdToken != idToken)
                {
                    Log.Warning("Foi obtido um token com ID no objeto diferente do ID na chave do dicionário. Não deveria acontecer.");
                    return null;
                }

                if (tokenGerado.DataUtcExpiracao != null && tokenGerado.DataUtcExpiracao.Value < DateTimeOffset.UtcNow)
                {
                    DeleteTokenGerado(idToken);
                    return null;
                }
            }

            return tokenGerado;
        }

        public static void CheckForExpiredTokensPeriodically()
        {
            // limpar tokens antigos periodicamente (evitar que lista cresca muito)
            if (!TokensExpirationCheckTimer.Enabled)
            {
                TokensExpirationCheckTimer.Interval = new TimeSpan(0, 0, 5).TotalMilliseconds;
                TokensExpirationCheckTimer.Elapsed += TokensExpirationCheckTimer_Elapsed;
                TokensExpirationCheckTimer.Start();
            }
        }

        public static void DeleteTokenGerado(Guid idToken)
        {
            TokensPermitidos.TryRemove(idToken, out _);
        }

        public static void DeleteTokensUtilizador(string utilizador)
        {
            foreach (KeyValuePair<Guid, TokenGerado> tokenKeyValue in TokensPermitidos)
            {
                if (string.Equals(tokenKeyValue.Value.Username, utilizador, StringComparison.OrdinalIgnoreCase))
                    DeleteTokenGerado(tokenKeyValue.Key);
            }
        }

        private static void TokensExpirationCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (KeyValuePair<Guid, TokenGerado> tokenKeyValue in TokensPermitidos)
            {
                if (tokenKeyValue.Value.DataUtcExpiracao != null && tokenKeyValue.Value.DataUtcExpiracao.Value < DateTimeOffset.UtcNow)
                    DeleteTokenGerado(tokenKeyValue.Key);
            }
        }
    }
}