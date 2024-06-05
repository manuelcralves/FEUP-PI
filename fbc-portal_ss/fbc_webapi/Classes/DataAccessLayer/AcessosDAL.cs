using AdmEngine100;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;
using fbc_webapi.Primavera;

namespace fbc_webapi.Classes.DataAccessLayer
{
    public static class AcessosDAL
    {
        public static Utilizador AutenticarUtilizador(string username, string password)
        {
            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true, abrirPSO: true))
            {
                Utilizador utilizador = new Utilizador()
                {
                    Codigo = pri.PSO.Contexto.Utilizador.Utilizador,
                    Nome = pri.PSO.Contexto.Utilizador.Nome,
                };

                return utilizador;
            }
        }

        internal static async Task<bool> TemAcessoEmpresa(string codigoUtilizador)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT TOP 1
	1
FROM
	 PRIEMPRE.dbo.Utilizadores
WHERE
	Codigo LIKE @Utilizador
";

                    cmd.Parameters.AddWithValue("@Utilizador", codigoUtilizador);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        return await sqlDataReader.ReadAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        internal static async Task<AcessoEmpresa> GetAcessoEmpresa( string codigoUtilizador)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT
	Codigo,
	Password
FROM
	 PRIEMPRE.dbo.Utilizadores
WHERE
	Codigo LIKE @Utilizador
";

                    cmd.Parameters.AddWithValue("@Utilizador", codigoUtilizador);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            AcessoEmpresa acesso = new AcessoEmpresa
                            {

                                Username = (string)sqlDataReader["Codigo"],
                                Password = sqlDataReader["Password"] == DBNull.Value ? "" : (string)sqlDataReader["Password"]
                            };

                            if (!string.IsNullOrEmpty(acesso.Password))
                            {
                                using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetAnonimo(), abrirPSO: true))
                                    acesso.Password = pri.PSO.Criptografia.Descripta(acesso.Password);
                            }

                            return acesso;
                        }
                    }
                }
            }

            return null;
        }

        public static async Task VerificaPermissoesPerfil(string perfil)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                List<string> permissoes = new List<string>();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
                                        SELECT
                                            CDU_Codigo
                                        FROM 
                                            TDU_PNT_PermissoesPortal";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {

                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            permissoes.Add((string)sqlDataReader["CDU_Codigo"]);
                        }
                    }

                    foreach ( string permissao in permissoes)
                    {

                        using (SqlCommand cmd_ = new SqlCommand())
                        {
                            cmd_.Connection = conn;

                            cmd_.CommandText = @"
	                                            BEGIN
                                                    IF NOT EXISTS (SELECT * FROM TDU_PNT_PerfisPermissoesPortal 
                                                                    WHERE CDU_PerfilPortal = @Perfil
				                                                    AND CDU_PermissaoPortal = @Permissao)
                                                    BEGIN
                                                        INSERT INTO TDU_PNT_PerfisPermissoesPortal (CDU_PerfilPortal, CDU_PermissaoPortal, CDU_Ativo)
                                                        VALUES (@Perfil,@Permissao,0)
                                                    END
                                                END";

                            cmd_.Parameters.AddWithValue("@Perfil", perfil);
                            cmd_.Parameters.AddWithValue("@Permissao", permissao);

                            await cmd_.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        public static async Task VerificaEmpresasUtilizador(string codigoUtilizador)
        {

            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                List<Empresa> empresas = new List<Empresa>();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
                                        SELECT
	                                          DISTINCT(E.Codigo)
                                        FROM
	                                         TDU_PNT_EmpresasUtilizadores EU
	                                         INNER JOIN PRIEMPRE.dbo.Empresas E ON E.Codigo = EU.CDU_CodigoEmpresa";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {

                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            Empresa empresa = new Empresa
                            {
                                Codigo = (string)sqlDataReader["Codigo"]
                            };

                            empresas.Add(empresa);
                        }
                    }

                    foreach (Empresa empresa in empresas)
                    {

                        using (SqlCommand cmd_ = new SqlCommand())
                        {
                            cmd_.Connection = conn;

                            cmd_.CommandText = @"BEGIN
                                                IF NOT EXISTS (SELECT * FROM TDU_PNT_EmpresasUtilizadores 
                                                                WHERE CDU_Utilizador = @Utilizador
				                                                AND CDU_CodigoEmpresa = @Empresa)
                                                BEGIN
                                                    INSERT INTO TDU_PNT_EmpresasUtilizadores (CDU_Utilizador, CDU_CodigoEmpresa, CDU_UtilizadorAUsar, CDU_Inativado)
                                                    VALUES (@Utilizador,@Empresa,@Utilizador,1)
                                                END
                                            END";

                            cmd_.Parameters.AddWithValue("@Utilizador", codigoUtilizador);
                            cmd_.Parameters.AddWithValue("@Empresa", empresa.Codigo);

                            await cmd_.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                    }
                }
            }

        }

        public static async Task<List<Utilizador>> GetUtilizadores()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT
	U.Codigo,
	U.Nome,
	U.Email

FROM
	Utilizadores U

ORDER BY
	U.Nome
";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<Utilizador> utilizadores = new List<Utilizador>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            Utilizador utilizador = new Utilizador
                            {
                                Codigo = (string)sqlDataReader["Codigo"],
                                Nome = sqlDataReader["Nome"] == DBNull.Value ? null : (string)sqlDataReader["Nome"],
                                Email = sqlDataReader["Email"] == DBNull.Value ? null : (string)sqlDataReader["Email"]
                            };

                            utilizadores.Add(utilizador);
                        }

                        return utilizadores;
                    }
                }
            }
        }

        public static async Task<Utilizador> GetUtilizador(string codigoUtilizador)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT
	U.Codigo,
	U.Nome,
	U.Email,
    U.Administrador

FROM
	Utilizadores U

WHERE
	U.Codigo LIKE @Utilizador
";

                    cmd.Parameters.AddWithValue("@Utilizador", codigoUtilizador);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            Utilizador utilizador = new Utilizador
                            {
                                Codigo = (string)sqlDataReader["Codigo"],
                                Nome = sqlDataReader["Nome"] == DBNull.Value ? null : (string)sqlDataReader["Nome"],
                                Email = sqlDataReader["Email"] == DBNull.Value ? null : (string)sqlDataReader["Email"],
                                Administrador = (bool)sqlDataReader["Administrador"]
                            };

                            return utilizador;
                        }
                    }
                }
            }

            return null;
        }

        public static async Task<List<string>> GetUtilizadoresPerfil(string codigoPerfil)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT
	U.Codigo

FROM
	Utilizadores U

WHERE
	U.CDU_PerfilPortal = @CodigoPerfil

ORDER BY
	U.Codigo
";

                    cmd.Parameters.AddWithValue("@CodigoPerfil", codigoPerfil);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<string> utilizadores = new List<string>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            utilizadores.Add((string)sqlDataReader["Codigo"]);

                        return utilizadores;
                    }
                }
            }
        }


        public static void AlterarEnviarPasswordUtilizador(string username, string password, string codigoUtilizador, string novaPassw)
        {
            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                try
                {
                    pri.BSO.DSO.Plat.IniciaTransaccao();

                    clsUtilizador utilizador = pri.BSO.DSO.Plat.Administrador.ListaUtilizadores.Edita(codigoUtilizador);

                    if (utilizador == null)
                        throw new FlorestasBemCuidadaWebApiException("Utilizador não existe.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

                    if (string.IsNullOrEmpty(utilizador.Email))
                        throw new FlorestasBemCuidadaWebApiException("Utilizador não tem email configurado. Por favor use o Administrador para definir um.", true, EnumErrorCode.Geral, HttpStatusCode.BadRequest);

                    utilizador.Password = novaPassw;

                    pri.BSO.DSO.Plat.Administrador.ListaUtilizadores.Actualiza(utilizador);

                    pri.BSO.DSO.Plat.Registos.ExecutaComando(
                        "UPDATE PRIEMPRE.dbo.Utilizadores SET CDU_PortalDataModificacao = GETDATE(), CDU_PortalModificadoPor = @UtilizadorAtual WHERE Codigo = @CodigoUtilizador",
                        new List<SqlParameter>() { new SqlParameter("@UtilizadorAtual", username), new SqlParameter("@CodigoUtilizador", codigoUtilizador) }
                    );

                    string assunto = "Nova palavra-passe Primavera";
                    string mensagem = $@"Uma nova palavra-passe foi gerada pela administração do Portal Transucatas.

Utilizador: {codigoUtilizador}
Nova palavra-passe: {novaPassw}";

                    pri.BSO.DSO.Plat.Registos.ExecutaComando(
                        "INSERT INTO PRIEMPRE.dbo.MensagensEmail ([To], Assunto, Mensagem, Formato, Utilizador) VALUES (@To, @Assunto, @Mensagem, 0, @UtilizadorAtual)",
                        new List<SqlParameter>() { new SqlParameter("@To", utilizador.Email), new SqlParameter("@Assunto", assunto), new SqlParameter("@Mensagem", mensagem), new SqlParameter("@UtilizadorAtual", username) }
                    );

                    pri.BSO.DSO.Plat.TerminaTransaccao();

                    Log.Information("Nova password gerada para {CodigoUtilizador} a pedido de {UtilizadorAtual}.", codigoUtilizador, username);
                }
                catch (Exception)
                {
                    pri.BSO.DSO.Plat.DesfazTransaccao();

                    throw;
                }
            }
        }


    }
}