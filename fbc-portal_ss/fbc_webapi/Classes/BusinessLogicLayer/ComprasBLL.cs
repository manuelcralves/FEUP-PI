using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using fbc_webapi.Classes.DataAccessLayer;
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;
using fbc_webapi.Primavera;

namespace fbc_webapi.Classes.BusinessLogicLayer
{
    public static class ComprasBLL
    {
        public static async Task<List<DocumentoCompra>> GetEncomendas()
        {
            return await ComprasDAL.GetEncomendas().ConfigureAwait(false);
        }

        public static async Task<List<TipoDocumentosCompras>> GetTiposDocumento()
        {
            return await ComprasDAL.GetTiposDocumento().ConfigureAwait(false);
        }

        public static async Task<List<TipoDocumentosCompras>> GetTiposDocumentoEncomendaFornecedor( string utilizadorAtual)
        {
            if (!await VerificadorAcesso.TemAcessoEmpresa(utilizadorAtual).ConfigureAwait(false))
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await ComprasDAL.GetTiposDocumentoEncomendaFornecedor().ConfigureAwait(false);
        }

        public static async Task<List<SerieCompras>> GetSeries(string tipoDocumento)
        {

            if (string.IsNullOrEmpty(tipoDocumento))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o tipo de documento.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await ComprasDAL.GetSeries(tipoDocumento).ConfigureAwait(false);
        }

        public static async Task<int> GetProximoNumeroSerie( string tipoDocumento, string serie)
        {
            if (string.IsNullOrEmpty(tipoDocumento))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o tipo de documento.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(serie))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar a serie.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await ComprasDAL.GetProximoNumeroSerie(tipoDocumento, serie).ConfigureAwait(false);
        }

        public static async Task<Guid> CriarDocumento( string utilizadorAtual, DocumentoCompra documento, List<string> ficheirosAnexos)
        {

            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual);

            if (documento == null)
                new ArgumentNullException(nameof(documento));

            return await ComprasDAL.CriarDocumento(acessoEmpresa.Username, acessoEmpresa.Password, documento, ficheirosAnexos);
        }


        public static async Task<List<DocumentoCompra>> GetRascunhos(string utilizadorAtual)
        {
            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            return await ComprasDAL.GetRascunhos(null, "E").ConfigureAwait(false);
        }

        public static async Task<List<DocumentoCompra>> GetRascunhosUtilizador(string utilizadorAtual)
        {
            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            return await ComprasDAL.GetRascunhos().ConfigureAwait(false);
        }

        public static async Task<List<DocumentoInterno>> GetRascunhosAprovacao(string utilizadorAtual)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            return await ComprasDAL.GetRascunhosAprovacao(utilizadorAtual).ConfigureAwait(false);
        }

        public static async Task<DocumentoCompra> GetRascunho(string utilizadorAtual, Guid id)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            var rascunho = await ComprasDAL.GetRascunho(acessoEmpresa.Username, acessoEmpresa.Password, id).ConfigureAwait(false);

            return rascunho;
        }

        public static async Task AnularRascunho(string utilizadorAtual, Guid id)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            DocumentoCompra rascunho = await ComprasDAL.GetRascunho(acessoEmpresa.Username, acessoEmpresa.Password, id);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (rascunho == null)
                throw new FlorestasBemCuidadaWebApiException("Rascunho não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (rascunho.Estado?.Equals("R", StringComparison.OrdinalIgnoreCase) == true)
                throw new FlorestasBemCuidadaWebApiException("Rascunho já se encontra anulada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (rascunho.Estado?.Equals("F", StringComparison.OrdinalIgnoreCase) == true)
                throw new FlorestasBemCuidadaWebApiException("Rascunho já se encontra fechada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (rascunho.Estado?.Equals("T", StringComparison.OrdinalIgnoreCase) == true)
                throw new FlorestasBemCuidadaWebApiException("Rascunho já se encontra transformada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            ComprasDAL.DeleteRascunho(id);
        }

        public static async Task AlterarRascunho(string utilizadorAtual, Guid id, DocumentoCompra documento, List<string> ficheirosAnexos)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (documento == null)
                throw new ArgumentNullException(nameof(documento));

            DocumentoCompra rascunhoExistente = await ComprasDAL.GetRascunho(acessoEmpresa.Username, acessoEmpresa.Password, id, true).ConfigureAwait(false);

            if (rascunhoExistente == null)
                throw new FlorestasBemCuidadaWebApiException("Rascunho não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (!rascunhoExistente.TipoDoc.Equals(documento.TipoDoc, StringComparison.OrdinalIgnoreCase))
                throw new FlorestasBemCuidadaWebApiException("Não pode alterar o tipo de uma proposta existente.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (!rascunhoExistente.Serie.Equals(documento.Serie, StringComparison.OrdinalIgnoreCase))
                throw new FlorestasBemCuidadaWebApiException("Não pode alterar a série de uma proposta existente.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            await ComprasDAL.AlterarRascunho(acessoEmpresa.Username, acessoEmpresa.Password, id, rascunhoExistente, documento, ficheirosAnexos).ConfigureAwait(false);
        }

        public static async Task<Guid> CriarRascunho(string utilizadorAtual, DocumentoCompra documento, List<string> ficheirosAnexos)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (documento == null)
                throw new ArgumentNullException(nameof(documento));

            List<TipoDocumentosCompras> tiposDocumento = await ComprasDAL.GetTiposDocumento();

            if (!tiposDocumento.Exists(t => t.Codigo.Equals(documento.TipoDoc, StringComparison.OrdinalIgnoreCase)))
                throw new FlorestasBemCuidadaWebApiException("Tipo de documento não é válido.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            List<SerieCompras> seriesDocumento = await ComprasDAL.GetSeries(documento.TipoDoc);

            if (!seriesDocumento.Exists(s => s.Codigo.Equals(documento.Serie, StringComparison.OrdinalIgnoreCase)))
                throw new FlorestasBemCuidadaWebApiException($"Série não é válida para documentos de tipo '{documento.TipoDoc}'.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);


             return await ComprasDAL.CriarRascunho(acessoEmpresa.Username, acessoEmpresa.Password, documento, ficheirosAnexos).ConfigureAwait(false);            
        }


    }
}