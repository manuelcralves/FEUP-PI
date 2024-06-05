using System;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web;
using Microsoft.Owin.Cors;
using Serilog;
using System.IO;
using System.Diagnostics;
using Microsoft.Owin.Security.OAuth;
using fbc_webapi.Autenticacao;
using System.Threading.Tasks;
using System.Web.Cors;

[assembly: OwinStartup(typeof(fbc_webapi.Startup))]
namespace fbc_webapi
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

       
        public void Configuration(IAppBuilder app)
        {
            try
            {
                app.Use<OwinExceptionHandlerMiddleware>();

                app.UseCors(CorsOptions.AllowAll);

                ConfigureAuth(app);

                HttpConfiguration config = new HttpConfiguration();

                WebApiConfig.Register(config);

                app.UseWebApi(config);

                Config.Carregar();

                Log.Debug("Carregado configurações.");
            }
            catch (Exception ex)
            {
                TentarGravarExcecaoArranque(ex);
            }
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the application for OAuth based flow
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/Token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(12),
                Provider = new SimpleAuthorizationServerProvider(),
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            SimpleAuthorizationServerProvider.CheckForExpiredTokensPeriodically();
            AcessoDownloadFicheiros.CheckForExpiredTokensPeriodically();
        }

        public static void TentarGravarExcecaoArranque(Exception ex)
        {
            try
            {
                Log.Debug(ex, "Erro no carregamento inicial da aplicação. Ver detalhes.\r\n");
                Log.Fatal(ex, "Erro no carregamento inicial da aplicação. Ver detalhes.\r\n");
            }
            catch (Exception) { }

            try
            {
                string tempLog = Path.Combine(Path.GetTempPath(), "fbc_webapi", "log-erros-arranque.txt");
                File.AppendAllText(tempLog, $"[{DateTime.Now.ToString("yyyy-MM-dd")}] Erro no carregamento inicial da aplicação: {ex}\r\n\r\n");
            }
            catch (Exception) { }

            try
            {
                Debug.WriteLine("Erro no carregamento inicial da aplicação: " + ex.ToString());
            }
            catch (Exception) { }
        }
    }
}