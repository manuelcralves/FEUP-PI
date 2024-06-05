using ErpBS100;
using Primavera.Extensibility.Engine;
using Serilog;
using StdBE100;
using StdPlatBS100;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Primavera
{
    public class PrimaveraConnection : IDisposable
    {
        public ErpBS BSO { get; set; }
        public StdPlatBS PSO { get; set; }

        public PrimaveraConnection(ConfiguracoesLigacaoPrimavera configuracoesLigacao, bool abrirPSO = false)
        {
            Abrir(configuracoesLigacao, abrirPSO);
        }

        public PrimaveraConnection(ConfiguracoesLigacaoEmpresaPrimavera configuracoesLigacaoEmpresa, bool abrirBSO = false, bool abrirPSO = false, bool carregarExtensibilidadesEmBSO = false)
        {
            Abrir(configuracoesLigacaoEmpresa, abrirBSO, abrirPSO, carregarExtensibilidadesEmBSO);
        }

        public void Abrir(ConfiguracoesLigacaoPrimavera configuracoesLigacao, bool abrirPSO)
        {
            Inicializar(configuracoesLigacao, false, abrirPSO);

            if (abrirPSO)
                AbrirPSO(configuracoesLigacao);
        }

        public void Abrir(ConfiguracoesLigacaoEmpresaPrimavera configuracoesLigacaoEmpresa, bool abrirBSO, bool abrirPSO, bool carregarExtensibilidadesEmBSO)
        {
            Inicializar(configuracoesLigacaoEmpresa, abrirBSO, abrirPSO);

            if (abrirBSO)
                AbrirBSO(configuracoesLigacaoEmpresa, carregarExtensibilidadesEmBSO);

            if (abrirPSO)
                AbrirPSO(configuracoesLigacaoEmpresa);
        }

        private void Inicializar(ConfiguracoesLigacaoPrimavera configuracoesLigacao, bool abrirBSO, bool abrirPSO)
        {
            if (configuracoesLigacao == null && (abrirPSO || abrirBSO))
                throw new ArgumentNullException(nameof(configuracoesLigacao));

            if (BSO == null)
                BSO = new ErpBS();

            if (PSO == null)
                PSO = new StdPlatBS();
        }

        public void AbrirBSO(ConfiguracoesLigacaoEmpresaPrimavera configuracoesLigacaoEmpresa, bool carregarExtensibilidadesEmBSO)
        {
            ValidarDadosIniciais(configuracoesLigacaoEmpresa);

            ValidarDadosLogin(configuracoesLigacaoEmpresa);

            if (string.IsNullOrEmpty(configuracoesLigacaoEmpresa.Empresa))
                throw new InvalidOperationException("Deve definir o código de empresa primeiro");

            StdBETransaccao dummy = new StdBETransaccao();

            BSO.AbreEmpresaTrabalho(configuracoesLigacaoEmpresa.TipoPlataforma, configuracoesLigacaoEmpresa.Empresa, configuracoesLigacaoEmpresa.Utilizador, configuracoesLigacaoEmpresa.Password, configuracoesLigacaoEmpresa.TipoLigacao, dummy, configuracoesLigacaoEmpresa.Instancia);

            if (BSO.Extensibility.Logger is StdBSExtensibilityLogger)
                (BSO.Extensibility.Logger as StdBSExtensibilityLogger).AllowInteractivity = false; // não mostrar caixas de dialogo/erros de ErpBS

            if (carregarExtensibilidadesEmBSO)
            {
                ExtensibilityService service = new ExtensibilityService();
                service.Initialize(BSO);

                if (service.IsOperational)
                    service.LoadExtensions();
            }
        }

        public void AbrirPSO(ConfiguracoesLigacaoPrimavera configuracoesLigacao)
        {
            ValidarDadosIniciais(configuracoesLigacao);

            if (configuracoesLigacao is ConfiguracoesLigacaoEmpresaPrimavera configuracoesLigacaoEmpresa)
            {
                ValidarDadosLogin(configuracoesLigacaoEmpresa);

                if (string.IsNullOrEmpty(configuracoesLigacaoEmpresa.AbvApl))
                    throw new InvalidOperationException("Deve definir a 'Apl' primeiro");

                if (string.IsNullOrEmpty(configuracoesLigacaoEmpresa.LicVersaoMinima))
                    throw new InvalidOperationException("Deve definir a 'LicVersaoMinima' primeiro");

                StdBSConfApl conf = new StdBSConfApl()
                {
                    AbvtApl = configuracoesLigacaoEmpresa.AbvApl,
                    Instancia = configuracoesLigacaoEmpresa.Instancia,
                    Utilizador = configuracoesLigacaoEmpresa.Utilizador,
                    PwdUtilizador = configuracoesLigacaoEmpresa.Password,
                    LicVersaoMinima = configuracoesLigacaoEmpresa.LicVersaoMinima,
                    TipoLigacao = configuracoesLigacaoEmpresa.TipoLigacao,
                };

                if (string.IsNullOrEmpty(configuracoesLigacaoEmpresa.Empresa))
                    PSO.AbrePlataforma(configuracoesLigacaoEmpresa.TipoPlataforma, conf);
                else
                {
                    StdBETransaccao dummy = new StdBETransaccao();

                    PSO.AbrePlataformaEmpresa(configuracoesLigacaoEmpresa.Empresa, dummy, conf, configuracoesLigacaoEmpresa.TipoPlataforma);
                }
            }
            else
            {
                PSO.AbrePlataformaEx(configuracoesLigacao.TipoPlataforma, configuracoesLigacao.Instancia);
            }
        }

        private static void ValidarDadosIniciais(ConfiguracoesLigacaoPrimavera configuracoesLigacao)
        {
            if (configuracoesLigacao == null)
                throw new ArgumentNullException(nameof(configuracoesLigacao));

            if (string.IsNullOrEmpty(configuracoesLigacao.Instancia))
                throw new InvalidOperationException("Deve definir a instância primeiro");

            if (configuracoesLigacao.TipoPlataforma != StdBETipos.EnumTipoPlataforma.tpEmpresarial && configuracoesLigacao.TipoPlataforma != StdBETipos.EnumTipoPlataforma.tpProfissional && configuracoesLigacao.TipoPlataforma != StdBETipos.EnumTipoPlataforma.tpFirst)
                throw new InvalidOperationException("O tipo de plataforma definido é inválido");
        }

        private static void ValidarDadosLogin(ConfiguracoesLigacaoEmpresaPrimavera configuracoesLigacaoEmpresa)
        {
            if (string.IsNullOrEmpty(configuracoesLigacaoEmpresa.Utilizador))
                throw new InvalidOperationException("Deve definir o login de Utilizador Primavera primeiro");

            if (string.IsNullOrEmpty(configuracoesLigacaoEmpresa.Password))
                throw new InvalidOperationException("Deve definir a Password Primavera primeiro");

            if (configuracoesLigacaoEmpresa.TipoLigacao != StdBETipos.EmunTipoLigacao.desktop && configuracoesLigacaoEmpresa.TipoLigacao != StdBETipos.EmunTipoLigacao.webapi)
                throw new InvalidOperationException("O tipo de ligação Primavera definido é inválido");
        }

        public void Fechar()
        {
            FecharBSO();

            FecharPSO();
        }

        public void FecharPSO()
        {
            if (PSO != null)
                PSO.FechaPlataformaEx();
        }

        public void FecharBSO()
        {
            if (BSO != null)
                BSO.FechaEmpresaTrabalho();
        }

        public void Dispose()
        {
            try
            {
                Fechar();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exceção ao fechar ligação a Primavera");

                throw;
            }
        }
    }
}