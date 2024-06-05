using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static StdBE100.StdBETipos;

namespace fbc_webapi.Primavera
{
    public class ConfiguracoesLigacaoPrimavera
    {
        public EnumTipoPlataforma TipoPlataforma { get; set; }
        public string Instancia { get; set; }

        public ConfiguracoesLigacaoPrimavera()
        {
            TipoPlataforma = EnumTipoPlataforma.tpEmpresarial;
            Instancia = "DEFAULT";
        }

        public ConfiguracoesLigacaoPrimavera(EnumTipoPlataforma tipoPlataforma) : this()
        {
            TipoPlataforma = tipoPlataforma;
        }

        public ConfiguracoesLigacaoPrimavera(EnumTipoPlataforma tipoPlataforma, string instancia) : this(tipoPlataforma)
        {
            Instancia = instancia;
        }
    }
}