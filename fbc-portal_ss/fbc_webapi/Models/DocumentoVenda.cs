using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class DocumentoVenda : DocumentoBase
    {
        public string LocalCarga { get; set; }
        public BaseMorada MoradaCarga { get; set; } = new BaseMorada();
        public string LocalDescarga { get; set; }
        public BaseMorada MoradaDescarga { get; set; } = new BaseMorada();
        public string Vendedor { get; set; }
        public string NomeVendedor { get; set; }
        public string CondPag { get; set; }
        public string NomeCondPag { get; set; }
        public double? LimiteCredito { get; set; }
        public double? Credito { get; set; }
        public string Morada { get; set; }
        public string CodPostal { get; set; }
        public string LocalidadeCodPostal { get; set; }
        public string Zona { get; set; }
        public string ZonaDescricao { get; set; }
        public string Observacoes { get; set; }
        public string Estado { get; set; }
        public bool? Anulado { get; set; }
        public bool? Fechado { get; set; }
        public EstornoDocumento DadosEstorno { get; set; }
        public string ModoExpedicao { get; set; }
        public bool Rascunho { get; set; }
        public List<LinhaDocumentoVenda> Linhas { get; set; }

        public DocumentoVenda()
        {
            Linhas = new List<LinhaDocumentoVenda>();
        }
    }
}