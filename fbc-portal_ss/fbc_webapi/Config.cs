using Microsoft.Extensions.Configuration;
using Serilog;
using StdBE100;
using StdPlatBS100;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using fbc_webapi.Primavera;
using fbc_webapi.Properties;

namespace fbc_webapi
{
    public static class Config
    {
        public static string Primavera_Instancia { get; set; }
        public static EnumTipoPlataformaConfig Primavera_Plataforma { get; set; }
        public static string Primavera_Apl { get; set; }
        public static string Primavera_Versao { get; set; }
        public static string Primavera_Empresa { get; set; }
        public static string ClientIdAutenticacao { get; set; }

        private static string ConnectionStringPrimaveraSemBD { get; set; }
        private static string ConnectionStringPrimaveraEmpresa { get; set; }
        private static string ConnectionStringPrimaveraPRIEMPRE { get; set; }

        public static void Carregar()
        {
            try
            {
                Primavera_Instancia = Settings.Default.Primavera_Instancia;
                Primavera_Plataforma = Settings.Default.Primavera_Plataforma;
                Primavera_Apl = Settings.Default.Primavera_Apl;
                Primavera_Versao = Settings.Default.Primavera_Versao;
                Primavera_Empresa = Settings.Default.Primavera_Empresa;
                ClientIdAutenticacao = Settings.Default.ClientIdAutenticacao;

                CarregarConnectionStringsPrimavera();
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Erro no carregamento das configurações primavera. Ver detalhes.\r\n");
            }
        }

        public static void CarregarConnectionStringsPrimavera()
        {
           
            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetAnonimo(), abrirPSO: true))
            {

                StdBELoginDisco loginDisco = pri.PSO.LoginDisco.Edita(Primavera_Instancia);

                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = loginDisco.Servidor,
                    ConnectTimeout = loginDisco.TimeOut,
                };

                string executingAssembly = Assembly.GetExecutingAssembly()?.GetName()?.Name;
                if (!string.IsNullOrEmpty(executingAssembly))
                    sqlConnectionStringBuilder.ApplicationName = executingAssembly;

                if (loginDisco.AutenticacaoWindows == 1)
                    sqlConnectionStringBuilder.IntegratedSecurity = true;
                else
                {
                    sqlConnectionStringBuilder.UserID = loginDisco.Login;
                    sqlConnectionStringBuilder.Password = loginDisco.Password;
                    //sqlConnectionStringBuilder.Password = "adminsb";
                }

                ConnectionStringPrimaveraSemBD = sqlConnectionStringBuilder.ConnectionString;

                sqlConnectionStringBuilder.InitialCatalog = pri.PSO.BaseDados.DaNomeBDdaEmpresa(Primavera_Empresa);

                ConnectionStringPrimaveraEmpresa = sqlConnectionStringBuilder.ConnectionString;

                Log.Debug("Carregada ConnectionString Empresa {Primavera_Empresa}.", Primavera_Empresa);

                sqlConnectionStringBuilder.InitialCatalog = pri.PSO.BaseDados.DaNomeBDdoPRIEMPRE();

                ConnectionStringPrimaveraPRIEMPRE = sqlConnectionStringBuilder.ConnectionString;

                Log.Debug("Carregada ConnectionString Priempre.");
            }
        }

        public static string GetConnectionStringEmpresa(string empresa)
        {
            if (string.IsNullOrEmpty(ConnectionStringPrimaveraSemBD))
                throw new InvalidOperationException("A SQL Connection String base não se encontra carregada");

            if (string.IsNullOrEmpty(empresa))
                throw new ArgumentException("Deve indicar a empresa", nameof(empresa));

            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionStringPrimaveraSemBD);

            sqlConnectionStringBuilder.InitialCatalog = "PRI" + empresa;

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public static string GetConnectionStringBaseDados(string baseDados)
        {
            if (string.IsNullOrEmpty(ConnectionStringPrimaveraSemBD))
                throw new InvalidOperationException("A SQL Connection String base não se encontra carregada");

            if (string.IsNullOrEmpty(baseDados))
                throw new ArgumentException("Deve indicar a base de dados", nameof(baseDados));

            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionStringPrimaveraSemBD);

            sqlConnectionStringBuilder.InitialCatalog = baseDados;

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public static string GetConnectionStringPRIEMPRE()
        {
            if (string.IsNullOrEmpty(ConnectionStringPrimaveraPRIEMPRE))
                throw new InvalidOperationException("A SQL Connection String da PRIEMPRE não foi carregada");

            return ConnectionStringPrimaveraPRIEMPRE;
        }

        public static string GetConnectionStringEmpresa()
        {
            if (string.IsNullOrEmpty(ConnectionStringPrimaveraEmpresa))
                throw new InvalidOperationException("A SQL Connection String da empresa Master Data não foi carregada");

            return ConnectionStringPrimaveraEmpresa;
        }


        public static string GetConnectionStringBase()
        {
            if (string.IsNullOrEmpty(ConnectionStringPrimaveraSemBD))
                throw new InvalidOperationException("A SQL Connection String base não se encontra carregada");

            return ConnectionStringPrimaveraSemBD;
        }
    }
}