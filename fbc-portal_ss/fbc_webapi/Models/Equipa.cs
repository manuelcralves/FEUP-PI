using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static RhpBE100.RhpBEDeclaracaoCVMod111;

namespace fbc_webapi.Models
{
    public class Equipa
    {
        public bool ModoEdição { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Email { get; set; }
        public bool Activa { get; set; }
        public string CriadoPor { get; set; }
        public string ModificadoPor { get; set; }
        public DateTime? DataCriação { get; set; }
        public DateTime? DataUltimaActualizacao { get; set; } 
        public List<Membro> Membros { get; set; } = new List<Membro>();
        public List<Leader> Leaders { get; set; } = new List<Leader>();

    }
}