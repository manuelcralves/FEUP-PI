using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using fbc_webapi.Classes.DataAccessLayer;
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;
using fbc_webapi.Primavera;

namespace fbc_webapi.Classes.BusinessLogicLayer
{
    public static class BaseBLL
    {
   
        public static async Task<List<Empresa>> GetEmpresas(string utilizadorAtual)
        {
            return await BaseDAL.GetEmpresas(utilizadorAtual).ConfigureAwait(false);
        }

        public static async Task<List<Exclusao>> GetExclusoes()
        {
            return await BaseDAL.GetExclusoes().ConfigureAwait(false);
        }


        public static async Task<List<CondicaoPagamento>> GetCondicoesPagamento(string utilizadorAtual)
        {
            if (!await VerificadorAcesso.TemAcessoEmpresa(utilizadorAtual).ConfigureAwait(false))
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetCondicoesPagamento().ConfigureAwait(false);
        }

        public static async Task<CondicaoPagamento> GetCondicaoPagamento(string utilizadorAtual, string codCondicaoPagamento)
        {
            if (!await VerificadorAcesso.TemAcessoEmpresa(utilizadorAtual).ConfigureAwait(false))
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(codCondicaoPagamento))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o código da condição de pagamento.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetCondicaoPagamento(codCondicaoPagamento).ConfigureAwait(false);
        }

        public static async Task<List<Unidade>> GetUnidades()
        {
            return await BaseDAL.GetUnidades().ConfigureAwait(false);
        }

        public static async Task<Unidade> GetUnidade( string codUnidade)
        {
            if (string.IsNullOrEmpty(codUnidade))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar uma unidade.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetUnidade(codUnidade).ConfigureAwait(false);
        }


        public static async Task<List<MoradaAlternativa>> GetMoradasAlternativas(string utilizadorAtual, string cliente)
        {
            if (!await VerificadorAcesso.TemAcessoEmpresa(utilizadorAtual).ConfigureAwait(false))
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(cliente))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o cliente.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetMoradasAlternativas(cliente).ConfigureAwait(false);
        }

        public static async Task<List<MoradaFornecedor>> GetMoradasFornecedores(string utilizadorAtual, string fornecedor)
        {
            if (!await VerificadorAcesso.TemAcessoEmpresa(utilizadorAtual).ConfigureAwait(false))
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(fornecedor))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o fornecedor.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetMoradasFornecedores(fornecedor).ConfigureAwait(false);
        }

        public static async Task<List<MoradaArmazem>> GetMoradasArmazens(string utilizadorAtual)
        {
            if (!await VerificadorAcesso.TemAcessoEmpresa(utilizadorAtual).ConfigureAwait(false))
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetMoradasArmazens().ConfigureAwait(false);
        }

        public static async Task<MoradaAlternativa> GetMoradaAlternativa(string utilizadorAtual, string cliente, string codMorada)
        {
            if (!await VerificadorAcesso.TemAcessoEmpresa(utilizadorAtual).ConfigureAwait(false))
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(cliente))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o cliente.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(codMorada))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o código da morada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetMoradaAlternativa(cliente, codMorada).ConfigureAwait(false);
        }

        public static async Task<MoradaFornecedor> GetMoradaFornecedores(string utilizadorAtual, string fornecedor, string codMorada)
        {
            if (!await VerificadorAcesso.TemAcessoEmpresa(utilizadorAtual).ConfigureAwait(false))
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(fornecedor))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o fornecedor.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(codMorada))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o código da morada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetMoradaFornecedor(fornecedor, codMorada).ConfigureAwait(false);
        }

        public static async Task<MoradaArmazem> GetMoradaArmazem(string codMorada)
        {
            if (string.IsNullOrEmpty(codMorada))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o código da morada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetMoradaArmazem(codMorada).ConfigureAwait(false);
        }

        public static async Task<List<Cliente>> GetClientes()
        {
            return await BaseDAL.GetClientes().ConfigureAwait(false);
        }

        public static async Task<Cliente> GetCliente( string codCliente)
        {

            if (string.IsNullOrEmpty(codCliente))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o cliente.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetCliente(codCliente).ConfigureAwait(false);
        }

        public static async Task<List<Fornecedor>> GetFornecedores()
        {
            return await BaseDAL.GetFornecedores().ConfigureAwait(false);
        }

        public static async Task<Fornecedor> GetFornecedor(string codFornecedor)
        {

            if (string.IsNullOrEmpty(codFornecedor))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o fornecedor.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetFornecedor(codFornecedor).ConfigureAwait(false);
        }

        public static async Task<List<Artigo>> GetArtigos()
        {
            return await BaseDAL.GetArtigos().ConfigureAwait(false);
        }

        public static async Task<Artigo> GetArtigo(string codArtigo)
        {
            if (string.IsNullOrEmpty(codArtigo))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o artigo.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetArtigo(codArtigo).ConfigureAwait(false);
        }

        public static async Task<List<Obra>> GetObras()
        {
            return await BaseDAL.GetObras().ConfigureAwait(false);
        }

        public static async Task<Obra> GetObra(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar a obra.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await BaseDAL.GetObra(id).ConfigureAwait(false);
        }

        public static async Task<List<Equipa>> GetEquipasUtilizador(string utilizadorAtual)
        {
            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            return await BaseDAL.GetEquipasLeader(utilizadorAtual).ConfigureAwait(false);
        }

        public static async Task<List<Equipa>> GetEquipas(string utilizadorAtual)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            return await BaseDAL.GetEquipas().ConfigureAwait(false);
        }

        public static async Task<Equipa> GetEquipa(string utilizadorAtual, string codigo)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            var rascunho = await BaseDAL.GetEquipa(codigo, utilizadorAtual).ConfigureAwait(false);

            return rascunho;
        }

        public static async Task ApagarEquipa(string utilizadorAtual, string codigo)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            var equipa = await BaseDAL.GetEquipa(codigo);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (equipa == null)
                throw new FlorestasBemCuidadaWebApiException("Equipa não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            BaseDAL.DeleteEquipa(codigo);
        }

        public static async Task CriarEquipa(string utilizadorAtual, Equipa equipa)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (equipa == null)
                throw new ArgumentNullException(nameof(equipa));


            await BaseDAL.CriarEquipa(acessoEmpresa.Username, equipa).ConfigureAwait(false);
        }

        public static async Task AlteraEquipa(string utilizadorAtual, Equipa equipa)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (equipa == null)
                throw new ArgumentNullException(nameof(equipa));

            Equipa equipaExistente = await BaseDAL.GetEquipa(equipa.Codigo).ConfigureAwait(false);

            if (equipaExistente == null)
                throw new FlorestasBemCuidadaWebApiException("Equipa não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);


            await BaseDAL.AlteraEquipa(acessoEmpresa.Username, equipaExistente, equipa).ConfigureAwait(false);
        }

        public static async Task ApagarMembroEquipa(string utilizadorAtual, string utilizador, string equipa)
        {

            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (utilizador == null)
                throw new ArgumentNullException(nameof(utilizador));

            if (equipa == null)
                throw new ArgumentNullException(nameof(equipa));

            Utilizador utilizadorExistente = await AcessosDAL.GetUtilizador(utilizador).ConfigureAwait(false);

            if (utilizadorExistente == null)
                throw new FlorestasBemCuidadaWebApiException("Utilizador não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            Equipa equipaExistente = await BaseDAL.GetEquipa(equipa).ConfigureAwait(false);

            if (equipaExistente == null)
                throw new FlorestasBemCuidadaWebApiException("Equipa não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);


            await BaseDAL.ApagarMembroEquipa(utilizador, equipa).ConfigureAwait(false);
        }

        public static async Task AdicionaMembroEquipa(string utilizadorAtual, Membro membro)
        {

            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (membro == null)
                throw new ArgumentNullException(nameof(membro));

            Utilizador utilizadorExistente = await AcessosDAL.GetUtilizador(membro.Utilizador);

            if (utilizadorExistente == null)
                throw new FlorestasBemCuidadaWebApiException("Utilizador não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            Equipa equipaExistente = await BaseDAL.GetEquipa(membro.CodEquipa).ConfigureAwait(false);

            if (equipaExistente == null)
                throw new FlorestasBemCuidadaWebApiException("Equipa não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            await BaseDAL.AdicionaMembroEquipa(membro).ConfigureAwait(false);
        }

    }
}