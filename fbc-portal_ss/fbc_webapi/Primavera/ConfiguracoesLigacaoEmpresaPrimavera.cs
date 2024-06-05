using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static StdBE100.StdBETipos;

namespace fbc_webapi.Primavera
{
    public class ConfiguracoesLigacaoEmpresaPrimavera : ConfiguracoesLigacaoPrimavera
    {
        public string Empresa { get; set; }
        public string Utilizador { get; set; }
        public string Password { get; set; }
        public EmunTipoLigacao TipoLigacao { get; set; }

        public string AbvApl { get; set; } // só usado para abrir PSO/StdPlatBS. Alguns valores possiveis: "ERP", "ADM", "COP", "CUB", "PWS", "INA", "GPP". Parece haver alguns comportamentos diferentes dependendo da aplicação que indicamos, como onde vai buscar preferencias de utilizador, textos usados (em erros por exemplo), verificação de permissões e possivelmente outras coisas.
        public string LicVersaoMinima { get; set; } // só usado para abrir PSO/StdPlatBS. Parece ser usado na verifiação de licenciamento.

        public ConfiguracoesLigacaoEmpresaPrimavera() : base()
        {
            TipoLigacao = EmunTipoLigacao.webapi; // não mostrar UI de upgrade de módulos/base dados

            AbvApl = "ERP";
            LicVersaoMinima = "10.00";
        }

        public ConfiguracoesLigacaoEmpresaPrimavera(EnumTipoPlataforma tipoPlataforma, string empresa, string utilizador, string password) : this()
        {
            TipoPlataforma = tipoPlataforma;
            Empresa = empresa;
            Utilizador = utilizador;
            Password = password;
        }

        public ConfiguracoesLigacaoEmpresaPrimavera(EnumTipoPlataforma tipoPlataforma, string empresa, string utilizador, string password, string instancia) : this(tipoPlataforma, empresa, utilizador, password)
        {
            Instancia = instancia;
        }

        public ConfiguracoesLigacaoEmpresaPrimavera(EnumTipoPlataforma tipoPlataforma, string empresa, string utilizador, string password, string instancia, EmunTipoLigacao tipoLigacao) : this(tipoPlataforma, empresa, utilizador, password, instancia)
        {
            TipoLigacao = tipoLigacao;
        }

        public ConfiguracoesLigacaoEmpresaPrimavera(EnumTipoPlataforma tipoPlataforma, string empresa, string utilizador, string password, string instancia, EmunTipoLigacao tipoLigacao, string abvApl, string licVersaoMinima) : this(tipoPlataforma, empresa, utilizador, password, instancia, tipoLigacao)
        {
            AbvApl = abvApl;
            LicVersaoMinima = licVersaoMinima;
        }
    }
}