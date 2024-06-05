using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class DocumentoBase
    {
        public Guid? Id { get; set; }
        public string Documento { get; set; }
        public string TipoDoc { get; set; }
        public string Serie { get; set; }
        public int? NumDoc { get; set; }
        public string Estado { get; set; }
        public DateTime? DataDoc { get; set; }
        public DateTime? DataVenc { get; set; }
        public string TipoEntidade { get; set; }
        public string Entidade { get; set; }
        public string NomeEntidade { get; set; }
        public double DescFornecedor { get; set; }
        public double DescFinanceiro { get; set; }
        public Guid? ObraID { get; set; }
        public string Obra { get; set; }
        public string NomeObra { get; set; }
        public string Utilizador { get; set; }
        public double? TotalDocumento { get; set; }
        public string Aprovador { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public string MotivoRejeicao { get; set; }
        public string Observacoes { get; set; }
        public string Equipa { get; set; }
        public List<Anexo> Anexos { get; set; }

        public DocumentoBase()
        {
            Anexos = new List<Anexo>();
        }

    }
}