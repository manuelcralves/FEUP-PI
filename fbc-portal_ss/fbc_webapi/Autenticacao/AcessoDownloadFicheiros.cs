using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Security;
using fbc_webapi.Classes;

namespace fbc_webapi.Autenticacao
{
    public static class AcessoDownloadFicheiros
    {
        private static ConcurrentDictionary<string, TokenAcessoFicheiro> TokensAcessoAnexosPropostas = new ConcurrentDictionary<string, TokenAcessoFicheiro>();
        private static ConcurrentDictionary<string, TokenAcessoFicheiro> TokensAcessoPdfsPropostas = new ConcurrentDictionary<string, TokenAcessoFicheiro>();
        private static ConcurrentDictionary<string, TokenAcessoFicheiro> TokensAcessoAnexosPedidosServico = new ConcurrentDictionary<string, TokenAcessoFicheiro>();
        private static Timer TokensExpirationCheckTimer = new Timer(); // limpar tokens antigos periodicamente (evitar que lista cresca muito)

        public static string CriarAcessoAnexoProposta(string filePath, string fileName, Guid idTokenGeradoAutenticacao)
        {
            string downloadToken = Membership.GeneratePassword(32, 5);

            TokenAcessoFicheiro token = TokensAcessoAnexosPropostas.GetOrAdd(downloadToken, new TokenAcessoFicheiro()
            {
                IdTokenGeradoAutenticacao = idTokenGeradoAutenticacao,
                FilePath = filePath,
                FileName = fileName,
                DataUtcExpiracao = DateTimeOffset.UtcNow.AddMinutes(30),
            });

            if (!string.Equals(token.FilePath, filePath) || token.IdTokenGeradoAutenticacao != idTokenGeradoAutenticacao)
                throw new Exception($"Não foi possível adicionar token de download de anexo de proposta a dicionário. Já existia um com o mesmo id mas ficheiro ou utilizador diferentes. Ficheiro de tentativa: '{filePath}'.");

            if (TokensAcessoAnexosPropostas.Count > 20000)
                Log.Warning("Existem {@NrTokens} tokens de download de anexos de propostas em memória. Performance pode começar a ser afetada em algumas situações. Reiniciar a aplicação pode ajudar.", TokensAcessoAnexosPropostas.Count);

            return downloadToken;
        }

        public static TokenAcessoFicheiro GetTokenAcessoAnexoProposta(string downloadToken)
        {
            TokensAcessoAnexosPropostas.TryGetValue(downloadToken, out TokenAcessoFicheiro tokenAcessoFicheiro);

            if (tokenAcessoFicheiro != null && tokenAcessoFicheiro.DataUtcExpiracao < DateTimeOffset.UtcNow)
            {
                DeleteTokenAcessoAnexoProposta(downloadToken);
                return null;
            }

            return tokenAcessoFicheiro;
        }

        public static void DeleteTokenAcessoAnexoProposta(string downloadToken)
        {
            TokensAcessoAnexosPropostas.TryRemove(downloadToken, out _);
        }

        public static string CriarAcessoPdfProposta(string filePath, string fileName, Guid idTokenGeradoAutenticacao)
        {
            string downloadToken = Membership.GeneratePassword(32, 5);

            TokenAcessoFicheiro token = TokensAcessoPdfsPropostas.GetOrAdd(downloadToken, new TokenAcessoFicheiro()
            {
                IdTokenGeradoAutenticacao = idTokenGeradoAutenticacao,
                FilePath = filePath,
                FileName = fileName,
                DataUtcExpiracao = DateTimeOffset.UtcNow.AddMinutes(30),
            });

            if (!string.Equals(token.FilePath, filePath) || token.IdTokenGeradoAutenticacao != idTokenGeradoAutenticacao)
                throw new Exception($"Não foi possível adicionar token de download de PDF de proposta a dicionário. Já existia um com o mesmo id mas ficheiro ou utilizador diferentes. Ficheiro de tentativa: '{filePath}'.");

            if (TokensAcessoPdfsPropostas.Count > 20000)
                Log.Warning("Existem {@NrTokens} tokens de download de PDFs de propostas em memória. Performance pode começar a ser afetada em algumas situações. Reiniciar a aplicação pode ajudar.", TokensAcessoPdfsPropostas.Count);

            return downloadToken;
        }

        public static TokenAcessoFicheiro GetTokenAcessoPdfProposta(string downloadToken)
        {
            TokensAcessoPdfsPropostas.TryGetValue(downloadToken, out TokenAcessoFicheiro tokenAcessoFicheiro);

            if (tokenAcessoFicheiro != null && tokenAcessoFicheiro.DataUtcExpiracao < DateTimeOffset.UtcNow)
            {
                DeleteTokenAcessoPdfProposta(downloadToken, true);
                return null;
            }

            return tokenAcessoFicheiro;
        }

        public static void DeleteTokenAcessoPdfProposta(string downloadToken, bool apagarFicheiro)
        {
            TokensAcessoPdfsPropostas.TryRemove(downloadToken, out TokenAcessoFicheiro tokenAcessoFicheiro);

            if (apagarFicheiro && tokenAcessoFicheiro != null)
                Utils.TentarApagarFicheiroTemporario(tokenAcessoFicheiro.FilePath);
        }

        public static string CriarAcessoAnexoPedidoServico(string filePath, string fileName, Guid idTokenGeradoAutenticacao)
        {
            string downloadToken = Membership.GeneratePassword(32, 5);

            TokenAcessoFicheiro token = TokensAcessoAnexosPedidosServico.GetOrAdd(downloadToken, new TokenAcessoFicheiro()
            {
                IdTokenGeradoAutenticacao = idTokenGeradoAutenticacao,
                FilePath = filePath,
                FileName = fileName,
                DataUtcExpiracao = DateTimeOffset.UtcNow.AddMinutes(30),
            });

            if (!string.Equals(token.FilePath, filePath) || token.IdTokenGeradoAutenticacao != idTokenGeradoAutenticacao)
                throw new Exception($"Não foi possível adicionar token de download de anexo de pedido de serviço a dicionário. Já existia um com o mesmo id mas ficheiro ou utilizador diferentes. Ficheiro de tentativa: '{filePath}'.");

            if (TokensAcessoAnexosPedidosServico.Count > 20000)
                Log.Warning("Existem {@NrTokens} tokens de download de anexos de pedidos de serviço em memória. Performance pode começar a ser afetada em algumas situações. Reiniciar a aplicação pode ajudar.", TokensAcessoAnexosPedidosServico.Count);

            return downloadToken;
        }

        public static TokenAcessoFicheiro GetTokenAcessoAnexoPedidoServico(string downloadToken)
        {
            TokensAcessoAnexosPedidosServico.TryGetValue(downloadToken, out TokenAcessoFicheiro tokenAcessoFicheiro);

            if (tokenAcessoFicheiro != null && tokenAcessoFicheiro.DataUtcExpiracao < DateTimeOffset.UtcNow)
            {
                DeleteTokenAcessoAnexoPedidoServico(downloadToken);
                return null;
            }

            return tokenAcessoFicheiro;
        }

        public static void DeleteTokenAcessoAnexoPedidoServico(string downloadToken)
        {
            TokensAcessoAnexosPedidosServico.TryRemove(downloadToken, out _);
        }

        public static void CheckForExpiredTokensPeriodically()
        {
            // limpar tokens antigos periodicamente (evitar que lista cresca muito)
            if (!TokensExpirationCheckTimer.Enabled)
            {
                TokensExpirationCheckTimer.Interval = new TimeSpan(0, 0, 5).TotalMilliseconds;
                TokensExpirationCheckTimer.Elapsed += TokensExpirationCheckTimer_Elapsed;
                TokensExpirationCheckTimer.Start();
            }
        }

        private static void TokensExpirationCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (KeyValuePair<string, TokenAcessoFicheiro> tokenKeyValue in TokensAcessoAnexosPropostas)
            {
                if (tokenKeyValue.Value.DataUtcExpiracao != null && tokenKeyValue.Value.DataUtcExpiracao < DateTimeOffset.UtcNow)
                    DeleteTokenAcessoAnexoProposta(tokenKeyValue.Key);
            }

            foreach (KeyValuePair<string, TokenAcessoFicheiro> tokenKeyValue in TokensAcessoPdfsPropostas)
            {
                if (tokenKeyValue.Value.DataUtcExpiracao != null && tokenKeyValue.Value.DataUtcExpiracao < DateTimeOffset.UtcNow)
                    DeleteTokenAcessoPdfProposta(tokenKeyValue.Key, true);
            }

            foreach (KeyValuePair<string, TokenAcessoFicheiro> tokenKeyValue in TokensAcessoAnexosPedidosServico)
            {
                if (tokenKeyValue.Value.DataUtcExpiracao != null && tokenKeyValue.Value.DataUtcExpiracao < DateTimeOffset.UtcNow)
                    DeleteTokenAcessoAnexoPedidoServico(tokenKeyValue.Key);
            }
        }
    }
}