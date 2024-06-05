using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using fbc_webapi.Autenticacao;
using fbc_webapi.Classes.DataAccessLayer;
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;

namespace fbc_webapi.Classes.BusinessLogicLayer
{
    public static class AcessosBLL
    {
        public static Utilizador AutenticarUtilizador(string username, string password)
        {
            return AcessosDAL.AutenticarUtilizador(username, password);
        }


        public static async Task VerificaEmpresasUtilizador(string utilizadorAtual, string codigoUtilizador)
        {


            if (string.IsNullOrEmpty(codigoUtilizador))
                throw new ArgumentException("Deve indicar o utilizador.", nameof(codigoUtilizador));

            Utilizador utilizadorOriginal =  await AcessosDAL.GetUtilizador(codigoUtilizador);


            if (utilizadorOriginal == null)
                throw new FlorestasBemCuidadaWebApiException($"O utilizador '{codigoUtilizador}' não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);


            await AcessosDAL.VerificaEmpresasUtilizador(codigoUtilizador).ConfigureAwait(false);

        }

        public static async Task<List<string>> GetPermissoesUtilizadorAtual(string utilizadorAtual)
        {
            if (string.IsNullOrEmpty(utilizadorAtual))
                throw new ArgumentException("Deve indicar o utilizador.", nameof(utilizadorAtual));

            Utilizador utilizador = await AcessosDAL.GetUtilizador(utilizadorAtual);

            if (utilizador == null)
                throw new FlorestasBemCuidadaWebApiException($"O utilizador '{utilizadorAtual}' não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            var Permissoes = new List<string>();

            if (BaseDAL.isTeamLeader(utilizador.Codigo) == true)
                Permissoes.Add("TEAM");

            if (utilizador.Administrador)
                Permissoes.Add("ADMIN");

            return Permissoes;
        }



        public static async Task<List<Utilizador>> GetUtilizadores(string utilizadorAtual)
        {

            return await AcessosDAL.GetUtilizadores().ConfigureAwait(false);
        }

        public static async Task<Utilizador> GetUtilizador(string utilizadorAtual, string codigoUtilizador)
        {

            if (string.IsNullOrEmpty(codigoUtilizador))
                throw new ArgumentException("Deve indicar o utilizador.", nameof(codigoUtilizador));

            return await AcessosDAL.GetUtilizador(codigoUtilizador).ConfigureAwait(false);
        }

        private static string GerarPassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

    }
}