using System;

namespace fbc_webapi.Models
{
    public class Anexo
    {
        public Guid Id { get; set; }
        public string FicheiroOrig { get; set; }
        public long TamanhoBytes { get; set; }
        public bool DocumentoMotorista { get; set; }
        public DateTime? Data { get; set; }
        public string NumDocumentoMotorista { get; set; }
        internal string FilePath { get; set; }
        public string Chave { get; set; }
        public string DescricaoDocumento { get; set; }
        public string MenuProposta { get; set; }
        public string FicheiroAssinatura { get; set; }
    }
}