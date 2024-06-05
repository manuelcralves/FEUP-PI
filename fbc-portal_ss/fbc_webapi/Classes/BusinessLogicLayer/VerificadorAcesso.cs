using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using fbc_webapi.Classes.DataAccessLayer;
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;

namespace fbc_webapi.Classes.BusinessLogicLayer
{
    internal class VerificadorAcesso
    {
        private string CodigoUtilizador { get; set; }

        private Utilizador UtilizadorCache { get; set; }

        public VerificadorAcesso(string codigoUtilizador)
        {
            CodigoUtilizador = codigoUtilizador;
        }

        internal static async Task<bool> TemAcessoEmpresa(string codigoUtilizador)
        {
            if (string.IsNullOrEmpty(codigoUtilizador))
                throw new ArgumentException("Deve indicar o utilizador.", nameof(codigoUtilizador));

            return await AcessosDAL.TemAcessoEmpresa(codigoUtilizador).ConfigureAwait(false);
        }

        internal static async Task<AcessoEmpresa> GetAcessoEmpresa(string codigoUtilizador)
        {
            if (string.IsNullOrEmpty(codigoUtilizador))
                throw new ArgumentException("Deve indicar o utilizador.", nameof(codigoUtilizador));

            return await AcessosDAL.GetAcessoEmpresa(codigoUtilizador).ConfigureAwait(false);
        }

    }
}