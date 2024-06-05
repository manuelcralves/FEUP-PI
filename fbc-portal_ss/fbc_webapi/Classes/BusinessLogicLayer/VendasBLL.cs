using CmpBE100;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using fbc_webapi.Classes.DataAccessLayer;
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;
using fbc_webapi.Primavera;
using VndBE100;

namespace fbc_webapi.Classes.BusinessLogicLayer
{
    public static class VendasBLL
    {
        public static async Task<List<TipoDocumentosVendas>> GetTiposDocumento(int? tipoTipoDocumento)
        {

            return await VendasDAL.GetTiposDocumento(tipoTipoDocumento).ConfigureAwait(false);
        }

        public static async Task<List<SerieVendas>> GetSeries( string tipoDocumento)
        {
            if (string.IsNullOrEmpty(tipoDocumento))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o tipo de documento.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await VendasDAL.GetSeries(tipoDocumento).ConfigureAwait(false);
        }

        public static async Task<List<TipoDocumentosVendas>> GetTiposDocumentoEncomendaCliente(string utilizadorAtual)
        {
            if (!await VerificadorAcesso.TemAcessoEmpresa(utilizadorAtual).ConfigureAwait(false))
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await VendasDAL.GetTiposDocumentoEncomendaCliente().ConfigureAwait(false);
        }

        public static async Task<int> GetProximoNumeroSerie(string tipoDocumento, string serie)
        {
            if (string.IsNullOrEmpty(tipoDocumento))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o tipo de documento.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(serie))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar a serie.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await VendasDAL.GetProximoNumeroSerie(tipoDocumento, serie).ConfigureAwait(false);
        }

        public static async Task<List<Vendedor>> GetVendedores(string utilizadorAtual)
        {
            if (!await VerificadorAcesso.TemAcessoEmpresa(utilizadorAtual).ConfigureAwait(false))
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await VendasDAL.GetVendedores().ConfigureAwait(false);
        }

        internal static async Task<Vendedor> GetVendedor( string utilizadorAtual, string vendedor)
        {
            if (!await VerificadorAcesso.TemAcessoEmpresa(utilizadorAtual).ConfigureAwait(false))
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(vendedor))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o vendedor.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await VendasDAL.GetVendedor(vendedor).ConfigureAwait(false);
        }

        public static async Task<List<Report>> GetReportsVenda(string utilizadorAtual, string tipoDoc)
        {
            if (!await VerificadorAcesso.TemAcessoEmpresa(utilizadorAtual).ConfigureAwait(false))
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(tipoDoc))
                throw new FlorestasBemCuidadaWebApiException("Deve indicar o tipo de documento.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            return await VendasDAL.GetReportsVenda( tipoDoc).ConfigureAwait(false);
        }


        private static void DrawImage(XGraphics gfx, string jpegSamplePath, int x, int y, int width, int height)
        {
            using (XImage image = XImage.FromFile(jpegSamplePath))
            {
                gfx.DrawImage(image, x, y, width, height);
            }
        }

        public static async Task<List<DocumentoVenda>> GetPropostasAbertas(string utilizadorAtual, string menu)
        {
            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            return await VendasDAL.GetPropostasAbertas(utilizadorAtual, menu).ConfigureAwait(false);
        }

        public static async Task<DocumentoVenda> GetPropostaAberta(string utilizadorAtual, Guid id)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            var proposta = await VendasDAL.GetPropostaAberta( acessoEmpresa.Username, acessoEmpresa.Password, id).ConfigureAwait(false);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            return proposta;
        }

        private static async void CopiaLinhaManualmente(PrimaveraConnection pri, VndBEDocumentoVenda documentoVenda, LinhaDocumentoVenda linha)
        {
            pri.BSO.Vendas.Documentos.AdicionaLinha(documentoVenda, linha.Artigo);

            VndBELinhaDocumentoVenda linhaNova = documentoVenda.Linhas.GetEdita(documentoVenda.Linhas.NumItens);

            await VendasDAL.DefinirCamposLinha(linha, pri, documentoVenda, linhaNova).ConfigureAwait(false);
        }


        public static async Task AnularPropostaAberta(string utilizadorAtual, Guid id, EstornoDocumento dadosEstorno)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (dadosEstorno == null)
                throw new ArgumentNullException(nameof(dadosEstorno));

            DocumentoVenda proposta = await VendasDAL.GetPropostaAberta( acessoEmpresa.Username, acessoEmpresa.Password, id, true);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (proposta == null)
                throw new FlorestasBemCuidadaWebApiException("Proposta não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (proposta.Estado?.Equals("R", StringComparison.OrdinalIgnoreCase) == true || proposta.Anulado == true)
                throw new FlorestasBemCuidadaWebApiException("Proposta já se encontra anulada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (proposta.Estado?.Equals("F", StringComparison.OrdinalIgnoreCase) == true || proposta.Fechado == true)
                throw new FlorestasBemCuidadaWebApiException("Proposta já se encontra fechada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (proposta.Estado?.Equals("T", StringComparison.OrdinalIgnoreCase) == true)
                throw new FlorestasBemCuidadaWebApiException("Proposta já se encontra transformada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VendasDAL.AnulaDocumento( acessoEmpresa.Username, acessoEmpresa.Password, id, dadosEstorno);
        }
        public static async Task AlterarPropostaAberta(string utilizadorAtual, Guid id, DocumentoVenda documento, List<string> ficheirosAnexos)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (documento == null)
                throw new ArgumentNullException(nameof(documento));

            DocumentoVenda propostaExistente = await VendasDAL.GetPropostaAberta( acessoEmpresa.Username, acessoEmpresa.Password, id, true).ConfigureAwait(false);

            if (propostaExistente == null)
                throw new FlorestasBemCuidadaWebApiException("Proposta não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (!propostaExistente.TipoDoc.Equals(documento.TipoDoc, StringComparison.OrdinalIgnoreCase))
                throw new FlorestasBemCuidadaWebApiException("Não pode alterar o tipo de uma proposta existente.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            if (!propostaExistente.Serie.Equals(documento.Serie, StringComparison.OrdinalIgnoreCase))
                throw new FlorestasBemCuidadaWebApiException("Não pode alterar a série de uma proposta existente.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            await VendasDAL.AlterarProposta( acessoEmpresa.Username, acessoEmpresa.Password, id, documento, ficheirosAnexos).ConfigureAwait(false);
        }
        public static async Task<Guid> CriarPropostaAberta(string utilizadorAtual, DocumentoVenda documento, List<string> ficheirosAnexos)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);


            VerificadorAcesso verificadorAcesso = new VerificadorAcesso(utilizadorAtual);

            if (documento == null)
                throw new ArgumentNullException(nameof(documento));

            List<TipoDocumentosVendas> tiposDocumentoProposta = await VendasDAL.GetTiposDocumento(null);

            if (!tiposDocumentoProposta.Exists(t => t.Codigo.Equals(documento.TipoDoc, StringComparison.OrdinalIgnoreCase)))
                throw new FlorestasBemCuidadaWebApiException("Tipo de documento não é válido para propostas.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            List<SerieVendas> seriesProposta = await VendasDAL.GetSeries(documento.TipoDoc);

            if (!seriesProposta.Exists(s => s.Codigo.Equals(documento.Serie, StringComparison.OrdinalIgnoreCase)))
                throw new FlorestasBemCuidadaWebApiException($"Série não é válida para propostas de tipo '{documento.TipoDoc}'.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(acessoEmpresa.Username, acessoEmpresa.Password), abrirBSO: true))
            {
                return await VendasDAL.CriarProposta(pri,  acessoEmpresa.Username, acessoEmpresa.Password, documento, ficheirosAnexos).ConfigureAwait(false);
            }
        }

        public static async Task<Anexo> GetAnexoProposta(string utilizadorAtual, Guid idAnexo)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa( utilizadorAtual).ConfigureAwait(false);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            var anexo = await VendasDAL.GetAnexoProposta(acessoEmpresa.Username, acessoEmpresa.Password, idAnexo).ConfigureAwait(false);

            return anexo;
        }

        public static async Task<List<string>> GetFilePathsPdfProposta(string utilizadorAtual, Guid idDoc, string report, int nrVias)
        {
            AcessoEmpresa acessoEmpresa = await VerificadorAcesso.GetAcessoEmpresa(utilizadorAtual);

            if (acessoEmpresa == null)
                throw new FlorestasBemCuidadaWebApiException("Não tem acesso à empresa indicada.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

            var filePaths = VendasDAL.GetFilePathsPdfProposta( acessoEmpresa.Username, acessoEmpresa.Password, idDoc, report, nrVias, out string menu);

            return filePaths;
        }

        public static async Task AtualizarCamposUtilAnexos(PrimaveraConnection pri, Guid idAnexo, string documento)
        {
            pri.BSO.DSO.Plat.Registos.ExecutaComando(@"
                                update Anexos 
                                set CDU_DocumentoMotorista = 1,
	                                CDU_NumDocumentoMotorista = @CDU_NumDocumentoMotorista
                                where Id = @Id
                                ", new List<SqlParameter>() { new SqlParameter("@CDU_NumDocumentoMotorista", documento),
                                   new SqlParameter("@Id", idAnexo) });

        }

    }
}