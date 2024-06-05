using fbc_webapi.Classes.DataAccessLayer;
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;
using fbc_webapi.Primavera;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using VndBE100;

namespace fbc_webapi.Classes.BusinessLogicLayer
{
    public class InternosBLL
    {
        public static async Task<List<TipoDocumentosInternos>> GetTiposDocumento()
        {
            return await InternosDAL.GetTiposDocumento().ConfigureAwait(false);
        }

        public static async Task<List<SerieInternos>> GetSeries(string tipoDocumento)
        {
            if (string.IsNullOrEmpty(tipoDocumento))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o tipo de documento.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await InternosDAL.GetSeries(tipoDocumento).ConfigureAwait(false);
        }

        public static async Task<int> GetProximoNumeroSerie(string tipoDocumento, string serie)
        {
            if (string.IsNullOrEmpty(tipoDocumento))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o tipo de documento.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(serie))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar a serie.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await InternosDAL.GetProximoNumeroSerie(tipoDocumento, serie).ConfigureAwait(false);
        }

        public static async Task<Guid> CriaDocumento(string utilizadorAtual, DocumentoInterno documento, List<string> ficheirosAnexos)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual);

            if (documento == null)
                new ArgumentNullException(nameof(documento));

            return await InternosDAL.CriaDocumento(acessoEmpresa.Username, acessoEmpresa.Password, documento, ficheirosAnexos);
        }


        public static async Task<List<DocumentoInterno>> GetDespesas()
        {
            return await InternosDAL.GetDespesas().ConfigureAwait(false);
        }

        public static async Task<DocumentoInterno> GetDespesa(Guid id)
        {
            if (id == null)
                new ArgumentNullException(nameof(id));

            var proposta = await InternosDAL.GetDespesa(id).ConfigureAwait(false);

            return proposta;
        }

        public static async Task<List<DocumentoInterno>> GetRascunhosUtilizador(string utilizadorAtual)
        {
            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            return await InternosDAL.GetRascunhos(utilizadorAtual).ConfigureAwait(false);
        }

        public static async Task<List<DocumentoInterno>> GetRascunhosAprovacao(string utilizadorAtual)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            return await InternosDAL.GetRascunhosAprovacao(utilizadorAtual).ConfigureAwait(false);
        }


        public static async Task<List<DocumentoInterno>> GetRascunhos(string utilizadorAtual)
        {
            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);


            return await InternosDAL.GetRascunhos(null,"E").ConfigureAwait(false);
        }

        public static async Task<DocumentoInterno> GetRascunho(string utilizadorAtual, Guid id)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            var rascunho = await InternosDAL.GetRascunho(acessoEmpresa.Username, acessoEmpresa.Password, id).ConfigureAwait(false);

            return rascunho;
        }

        public static async Task AnularRascunho(string utilizadorAtual, Guid id)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            DocumentoInterno rascunho = await InternosDAL.GetRascunho(id, true);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (rascunho == null)
                throw new FlorestasBemCuidadaWebApiException("Rascunho não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            /*if (rascunho.Estado?.Equals("R", StringComparison.OrdinalIgnoreCase) == true)
                throw new FlorestasBemCuidadaWebApiException("Rascunho já se encontra anulado.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (rascunho.Estado?.Equals("F", StringComparison.OrdinalIgnoreCase) == true)
                throw new FlorestasBemCuidadaWebApiException("Rascunho já se encontra fechado.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (rascunho.Estado?.Equals("T", StringComparison.OrdinalIgnoreCase) == true)
                throw new FlorestasBemCuidadaWebApiException("Rascunho já se encontra transformado.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);*/

            InternosDAL.DeleteRascunho(id);
        }

        public static async Task AlterarRascunho(string utilizadorAtual, Guid id, DocumentoInterno documento, List<string> ficheirosAnexos)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (documento == null)
                throw new ArgumentNullException(nameof(documento));

            DocumentoInterno rascunhoExistente = await InternosDAL.GetRascunho(id, true).ConfigureAwait(false);

            if (rascunhoExistente == null)
                throw new FlorestasBemCuidadaWebApiException("Rascunho não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            await InternosDAL.AlterarRascunho(acessoEmpresa.Username, acessoEmpresa.Password, documento, id, ficheirosAnexos).ConfigureAwait(false);
        }

        public static async Task<Guid> CriarRascunho(string utilizadorAtual, DocumentoInterno documento, List<string> ficheirosAnexos)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (documento == null)
                throw new ArgumentNullException(nameof(documento));

            List<TipoDocumentosInternos> tiposDocumento = await InternosDAL.GetTiposDocumento();

            if (!tiposDocumento.Exists(t => t.Codigo.Equals(documento.TipoDoc, StringComparison.OrdinalIgnoreCase)))
                throw new FlorestasBemCuidadaWebApiException("Tipo de documento não é válido para internos.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            List<SerieInternos> series = await InternosDAL.GetSeries(documento.TipoDoc);

            if (!series.Exists(s => s.Codigo.Equals(documento.Serie, StringComparison.OrdinalIgnoreCase)))
                throw new FlorestasBemCuidadaWebApiException($"Série não é válida para documentos do tipo '{documento.TipoDoc}'.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);


            return await InternosDAL.CriarRascunho(acessoEmpresa.Username, acessoEmpresa.Password, documento, ficheirosAnexos).ConfigureAwait(false);
            
        }



    }
}