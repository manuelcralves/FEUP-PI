using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static StdBE100.StdBETipos;

namespace fbc_webapi.Primavera
{
    public static class ConfiguracoesLigacaoPrimaveraFactory
    {
        public static ConfiguracoesLigacaoPrimavera GetAnonimo()
        {
            return new ConfiguracoesLigacaoPrimavera((EnumTipoPlataforma)Config.Primavera_Plataforma, Config.Primavera_Instancia);
        }

        public static ConfiguracoesLigacaoEmpresaPrimavera GetEmpresa(string utilizador, string password)
        {
            return new ConfiguracoesLigacaoEmpresaPrimavera((EnumTipoPlataforma)Config.Primavera_Plataforma, Config.Primavera_Empresa, utilizador, password, Config.Primavera_Instancia, EmunTipoLigacao.webapi, Config.Primavera_Apl, Config.Primavera_Versao);
        }
    }
}