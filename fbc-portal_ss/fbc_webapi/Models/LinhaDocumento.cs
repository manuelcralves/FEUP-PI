using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class LinhaDocumento
    {
        public Guid? Id { get; set; }
        public int? NumLinha { get; set; }
        public string Artigo { get; set; }
        public string Descricao { get; set; }
        public double? Quantidade { get; set; }
        public string Unidade { get; set; }
        public string Armazem { get; set; } 
        public string Localizacao { get; set; }
        public string Observacoes { get; set; }
        public double? Preco { get; set; }
        public double? Total { get; set; }
        public string Lote { get; set; }
        public string ObraId { get; set; }  
    }
}