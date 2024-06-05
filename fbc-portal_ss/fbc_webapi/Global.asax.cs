using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using fbc_webapi.Primavera;

namespace fbc_webapi
{
    public class WebApiApplication : HttpApplication
    {
        private static string _pathPrimaveraV10 = "";

        protected void Application_Start()
        {
            try
            {
                CarregarSerilog();

                Log.Information("Aplicação iniciada.\r\nNome: {AssemblyName}. Versão: {Version}. A correr como Utilizador: {UserName}.", Assembly.GetExecutingAssembly()?.GetName().Name, Assembly.GetExecutingAssembly()?.GetName().Version, WindowsIdentity.GetCurrent()?.Name);

                _pathPrimaveraV10 = PrimaveraUtils.FindPrimaveraV10Path();

                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

                Log.Debug("Application_Start terminado.");
            }
            catch (Exception ex)
            {
                Startup.TentarGravarExcecaoArranque(ex);
            }
        }

        protected void Application_End()
        {
            Log.Information("A aplicação terminou");
            Log.CloseAndFlush();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Exception ex = Server.GetLastError();

                if (ex != null)
                {
                    if (ex is ThreadAbortException || ex is OperationCanceledException) return;

                    if (ex is HttpException && (ex.Message == "O ficheiro não existe." || ex.Message == "File does not exist.")) return;

                    Log.Error(ex, "Exceção não tratada. Pedido (se preenchido): '{@UrlPedido}'", HttpContext.Current?.Request?.Url?.PathAndQuery);
                }
            }
            catch (Exception) { }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyFullName = "";

            try
            {
                if (string.IsNullOrEmpty(_pathPrimaveraV10))
                    _pathPrimaveraV10 = PrimaveraUtils.FindPrimaveraV10Path();

                if (!string.IsNullOrEmpty(_pathPrimaveraV10))
                {
                    AssemblyName assemblyName = new AssemblyName(args.Name);
                    assemblyFullName = Path.Combine(_pathPrimaveraV10, assemblyName.Name + ".dll");

                    if (File.Exists(assemblyFullName))
                        return Assembly.LoadFile(assemblyFullName);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro a carregar assembly '{@AssemblyFullName}' - '{@ArgsName}'", assemblyFullName, args.Name);
            }

            return null;
        }

        private void CarregarSerilog()
        {
            Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
