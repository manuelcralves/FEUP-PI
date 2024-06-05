using RhpBE100;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using fbc_webapi.ErrorHandling;
using fbc_webapi.Models;
using fbc_webapi.Primavera;
using Dapper;
using System.Text;
using System.Web.Http.Results;
using System.Web.Http;
using static BasBE100.BasBETiposGcp;

namespace fbc_webapi.Classes.DataAccessLayer
{
    public static class BaseDAL
    {
        public static async Task<List<Funcionario>> GetMotoristas()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Codigo, Nome, Localidade, Naturalidade, Empresa FROM PNT_VIEW_Funcionarios";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<Funcionario> funcionarios = new List<Funcionario>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            Funcionario funcionario = new Funcionario
                            {
                                Codigo = (string)sqlDataReader["Codigo"],
                                Nome = sqlDataReader["Nome"] == DBNull.Value ? null : (string)sqlDataReader["Nome"],
                                Localidade = sqlDataReader["Localidade"] == DBNull.Value ? null : (string)sqlDataReader["Localidade"],
                                Naturalidade = sqlDataReader["Naturalidade"] == DBNull.Value ? null : (string)sqlDataReader["Naturalidade"],
                                Empresa = sqlDataReader["Empresa"] == DBNull.Value ? null : (string)sqlDataReader["Empresa"],
                            };

                            funcionarios.Add(funcionario);
                        }

                        return funcionarios;
                    }
                }
            }
        }

        public static async Task<List<Exclusao>> GetExclusoes()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT CDU_Codigo, CDU_Motivo FROM TDU_PNT_Exclusoes ORDER BY CDU_Codigo";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<Exclusao> exclusoes = new List<Exclusao>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            Exclusao exclusao = new Exclusao
                            {
                                Codigo = (string)sqlDataReader["CDU_Codigo"],
                                Motivo = (string)sqlDataReader["CDU_Motivo"],
                            };

                            exclusoes.Add(exclusao);
                        }

                        return exclusoes;
                    }
                }
            }
        }

        public static async Task<Obra> GetObra(string codObra)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT ID, Codigo, Descricao FROM [COP_Obras] WHERE Codigo = @Obra";

                    cmd.Parameters.AddWithValue("@Obra", codObra);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return null;

                        var obra = new Obra
                        { Id = (Guid)sqlDataReader["Id"],
                            Codigo = (string)sqlDataReader["Codigo"],
                            Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                        };

                        return obra;
                    }
                }
            }
        }


        public static async Task<List<Obra>> GetObras()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"SELECT ID, Codigo, Descricao FROM [COP_Obras] ORDER BY Descricao";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        var obras = new List<Obra>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            var obra = new Obra
                            {
                                Id = (Guid)sqlDataReader["Id"],
                                Codigo = (string)sqlDataReader["Codigo"],
                                Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                            };

                            obras.Add(obra);
                        }

                        return obras;
                    }
                }
            }
        }

        public static async Task<MoradaAlternativa> GetMoradaAlternativa(string cliente, string codMorada)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT 
    MoradaAlternativa, 
    Morada, 
    Localidade, 
    Cp, 
    eGAR_CodigoAPA, 
    CDU_Zona,
    Z.Descricao as ZonaDescricao
FROM MoradasAlternativasClientes 
LEFT JOIN Zonas Z on Z.Zona = CDU_Zona
WHERE Cliente = @Cliente 
AND MoradaAlternativa = @MoradaAlternativa";

                    cmd.Parameters.AddWithValue("@Cliente", cliente);
                    cmd.Parameters.AddWithValue("@MoradaAlternativa", codMorada);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return null;

                        MoradaAlternativa moradaAlternativa = new MoradaAlternativa
                        {
                            CodMoradaAlternativa = (string)sqlDataReader["MoradaAlternativa"],
                            Morada = sqlDataReader["Morada"] == DBNull.Value ? null : (string)sqlDataReader["Morada"],
                            Localidade = sqlDataReader["Localidade"] == DBNull.Value ? null : (string)sqlDataReader["Localidade"],
                            CodigoPostal = sqlDataReader["Cp"] == DBNull.Value ? null : (string)sqlDataReader["Cp"],
                            CodigoAPA = sqlDataReader["eGAR_CodigoAPA"] == DBNull.Value ? null : (string)sqlDataReader["eGAR_CodigoAPA"],
                            Zona = sqlDataReader["CDU_Zona"] == DBNull.Value ? null : (string)sqlDataReader["CDU_Zona"],
                            ZonaDescricao = sqlDataReader["ZonaDescricao"] == DBNull.Value ? null : (string)sqlDataReader["ZonaDescricao"],
                        };

                        return moradaAlternativa;
                    }
                }
            }
        }

        public static async Task<List<MoradaAlternativa>> GetMoradasAlternativas(string cliente)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT 
    MoradaAlternativa, 
    Morada, 
    Localidade, 
    Cp, 
    eGAR_CodigoAPA,
    CDU_Zona,
    Z.Descricao as ZonaDescricao
FROM MoradasAlternativasClientes 
LEFT JOIN Zonas Z on Z.Zona = CDU_Zona
WHERE Cliente = @Cliente 
ORDER BY MoradaPorDefeito DESC, MoradaAlternativa ASC";

                    cmd.Parameters.AddWithValue("@Cliente", cliente);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<MoradaAlternativa> moradasAlternativas = new List<MoradaAlternativa>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            MoradaAlternativa moradaAlternativa = new MoradaAlternativa
                            {
                                CodMoradaAlternativa = (string)sqlDataReader["MoradaAlternativa"],
                                Morada = sqlDataReader["Morada"] == DBNull.Value ? null : (string)sqlDataReader["Morada"],
                                Localidade = sqlDataReader["Localidade"] == DBNull.Value ? null : (string)sqlDataReader["Localidade"],
                                CodigoPostal = sqlDataReader["Cp"] == DBNull.Value ? null : (string)sqlDataReader["Cp"],
                                CodigoAPA = sqlDataReader["eGAR_CodigoAPA"] == DBNull.Value ? null : (string)sqlDataReader["eGAR_CodigoAPA"],
                                Zona = sqlDataReader["CDU_Zona"] == DBNull.Value ? null : (string)sqlDataReader["CDU_Zona"],
                                ZonaDescricao = sqlDataReader["ZonaDescricao"] == DBNull.Value ? null : (string)sqlDataReader["ZonaDescricao"],
                            };

                            moradasAlternativas.Add(moradaAlternativa);
                        }

                        return moradasAlternativas;
                    }
                }
            }
        }

        public static async Task<MoradaFornecedor> GetMoradaFornecedor(string cliente, string codMorada)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT 
	'MP000'  As Codigo,
	Morada,
	Local,
	Cp,
	eGAR_CodigoAPA,
	1 as MoradaPorDefeito
	FROM Fornecedores
Where Fornecedor = @Fornecedor and 'MP000' = @Codigo
UNION ALL
SELECT 
    MoradaAlternativa As Codigo, 
    Morada, 
    Localidade As Local, 
    Cp, 
    eGAR_CodigoAPA,
	MoradaPorDefeito
FROM MoradasAlternativasFornecedores 
WHERE Fornecedor = @Fornecedor and MoradaAlternativa = @Codigo
ORDER BY MoradaPorDefeito DESC, Morada ASC";

                    cmd.Parameters.AddWithValue("@Fornecedor", cliente);
                    cmd.Parameters.AddWithValue("@Codigo", codMorada);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return null;

                        MoradaFornecedor moradaAlternativa = new MoradaFornecedor
                        {
                            CodMoradaFornecedor = (string)sqlDataReader["Codigo"],
                            Morada = sqlDataReader["Morada"] == DBNull.Value ? null : (string)sqlDataReader["Morada"],
                            Localidade = sqlDataReader["Local"] == DBNull.Value ? null : (string)sqlDataReader["Local"],
                            CodigoPostal = sqlDataReader["Cp"] == DBNull.Value ? null : (string)sqlDataReader["Cp"],
                            CodigoAPA = sqlDataReader["eGAR_CodigoAPA"] == DBNull.Value ? null : (string)sqlDataReader["eGAR_CodigoAPA"],
                        };

                        return moradaAlternativa;
                    }
                }
            }
        }

        public static async Task<List<MoradaFornecedor>> GetMoradasFornecedores(string fornecedor)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT 
	'MP000'  As Codigo,
	Morada,
	Local,
	Cp,
	eGAR_CodigoAPA,
	1 as MoradaPorDefeito
	FROM Fornecedores
Where Fornecedor = @Fornecedor
UNION ALL
SELECT 
    MoradaAlternativa As Codigo, 
    Morada, 
    Localidade, 
    Cp, 
    eGAR_CodigoAPA,
	MoradaPorDefeito
FROM MoradasAlternativasFornecedores 
WHERE Fornecedor = @Fornecedor
ORDER BY MoradaPorDefeito DESC, Morada ASC";

                    cmd.Parameters.AddWithValue("@Fornecedor", fornecedor);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<MoradaFornecedor> moradasAlternativas = new List<MoradaFornecedor>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            MoradaFornecedor moradaAlternativa = new MoradaFornecedor
                            {
                                CodMoradaFornecedor = (string)sqlDataReader["Codigo"],
                                Morada = sqlDataReader["Morada"] == DBNull.Value ? null : (string)sqlDataReader["Morada"],
                                Localidade = sqlDataReader["Local"] == DBNull.Value ? null : (string)sqlDataReader["Local"],
                                CodigoPostal = sqlDataReader["Cp"] == DBNull.Value ? null : (string)sqlDataReader["Cp"],
                                CodigoAPA = sqlDataReader["eGAR_CodigoAPA"] == DBNull.Value ? null : (string)sqlDataReader["eGAR_CodigoAPA"],
                            };

                            moradasAlternativas.Add(moradaAlternativa);
                        }

                        return moradasAlternativas;
                    }
                }
            }
        }

        public static async Task<List<MoradaArmazem>> GetMoradasArmazens()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"SELECT Armazem, Morada, Localidade, Cp FROM Armazens ORDER BY Armazem ASC";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<MoradaArmazem> moradasArmazens = new List<MoradaArmazem>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            MoradaArmazem moradaArmazem = new MoradaArmazem
                            {
                                Armazem = (string)sqlDataReader["Armazem"],
                                Morada = sqlDataReader["Morada"] == DBNull.Value ? null : (string)sqlDataReader["Morada"],
                                Localidade = sqlDataReader["Localidade"] == DBNull.Value ? null : (string)sqlDataReader["Localidade"],
                                CodigoPostal = sqlDataReader["Cp"] == DBNull.Value ? null : (string)sqlDataReader["Cp"],
                            };

                            moradasArmazens.Add(moradaArmazem);
                        }

                        return moradasArmazens;
                    }
                }
            }
        }

        public static async Task<MoradaArmazem> GetMoradaArmazem(string codMorada)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"SELECT Armazem, Morada, Localidade, Cp FROM Armazens WHERE Armazem = @Armazem";

                    cmd.Parameters.AddWithValue("@Armazem", codMorada);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        MoradaArmazem moradasArmazens = new MoradaArmazem();
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return null;

                        MoradaArmazem moradaArmazem = new MoradaArmazem
                        {
                            Armazem = (string)sqlDataReader["Armazem"],
                            Morada = sqlDataReader["Morada"] == DBNull.Value ? null : (string)sqlDataReader["Morada"],
                            Localidade = sqlDataReader["Localidade"] == DBNull.Value ? null : (string)sqlDataReader["Localidade"],
                            CodigoPostal = sqlDataReader["Cp"] == DBNull.Value ? null : (string)sqlDataReader["Cp"],
                        };

                        return moradaArmazem;
                    }
                }
            }
        }

        public static async Task<List<Empresa>> GetEmpresas(string utilizador)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT
	E.Codigo,
	E.IDNome

FROM
	TDU_PNT_EmpresasUtilizadores EU
	INNER JOIN PRIEMPRE.dbo.Empresas E ON E.Codigo = EU.CDU_CodigoEmpresa

WHERE
	EU.CDU_Utilizador LIKE @Utilizador
	AND (EU.CDU_Inativado IS NULL OR EU.CDU_Inativado = 0)

ORDER BY
	E.IDNome";

                    cmd.Parameters.AddWithValue("@Utilizador", utilizador);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<Empresa> empresas = new List<Empresa>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            Empresa empresa = new Empresa
                            {
                                Codigo = (string)sqlDataReader["Codigo"],
                                Nome = sqlDataReader["IDNome"] == DBNull.Value ? null : (string)sqlDataReader["IDNome"]
                            };

                            empresas.Add(empresa);
                        }

                        return empresas;
                    }
                }
            }
        }

        public static async Task<List<MotivoEstorno>> GetMotivosEstorno(string empresa)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa(empresa)))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Motivo, Descricao FROM MotivosEstorno ORDER BY Descricao";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<MotivoEstorno> motivos = new List<MotivoEstorno>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            MotivoEstorno motivo = new MotivoEstorno
                            {
                                Codigo = (string)sqlDataReader["Motivo"],
                                Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                            };

                            motivos.Add(motivo);
                        }

                        return motivos;
                    }
                }
            }
        }
        public static async Task<List<CondicaoPagamento>> GetCondicoesPagamento()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT CondPag, Descricao, Dias FROM CondPag ORDER BY Descricao";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<CondicaoPagamento> condicoesPagamento = new List<CondicaoPagamento>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            CondicaoPagamento condicaoPagamento = new CondicaoPagamento
                            {
                                Codigo = (string)sqlDataReader["CondPag"],
                                Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                                Dias = sqlDataReader["Dias"] == DBNull.Value ? (short)0 : (short)sqlDataReader["Dias"]
                            };

                            condicoesPagamento.Add(condicaoPagamento);
                        }

                        return condicoesPagamento;
                    }
                }
            }
        }

        public static async Task<CondicaoPagamento> GetCondicaoPagamento(string codCondicaoPagamento)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT CondPag, Descricao, Dias FROM CondPag WHERE CondPag = @CondPag";

                    cmd.Parameters.AddWithValue("@CondPag", codCondicaoPagamento);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return null;

                        CondicaoPagamento condicaoPagamento = new CondicaoPagamento
                        {
                            Codigo = (string)sqlDataReader["CondPag"],
                            Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                            Dias = sqlDataReader["Dias"] == DBNull.Value ? (short)0 : (short)sqlDataReader["Dias"]
                        };

                        return condicaoPagamento;
                    }
                }
            }
        }

        public static async Task<List<Unidade>> GetUnidades()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "select Unidade, Descricao from Unidades ORDER BY Descricao";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<Unidade> unidades = new List<Unidade>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            Unidade unidade = new Unidade
                            {
                                Codigo = (string)sqlDataReader["Unidade"],
                                Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                            };

                            unidades.Add(unidade);
                        }

                        return unidades;
                    }
                }
            }
        }

        public static async Task<Unidade> GetUnidade(string codUnidade)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "select Unidade, Descricao from Unidades WHERE Unidade = @Unidade";

                    cmd.Parameters.AddWithValue("@Unidade", codUnidade);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return null;

                        Unidade unidade = new Unidade
                        {
                            Codigo = (string)sqlDataReader["Unidade"],
                            Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                        };

                        return unidade;
                    }
                }
            }
        }


        public static async Task<List<Cliente>> GetClientes()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT 
    C.Cliente, 
    C.Nome, 
    C.Fac_Mor, 
    C.Fac_Cp, 
    C.Fac_Cploc, 
    C.Fac_Local, 
    C.CondPag, 
    C.LimiteCred, 
    C.TotalDeb, 
    C.Vendedor, 
    C.Zona, 
    Z.Descricao as ZonaDescricao,
    C.eGAR_CodigoAPA 
FROM Clientes C
LEFT JOIN Zonas Z on Z.Zona = C.Zona
WHERE ISNULL(C.ClienteAnulado, 0) = 0 
ORDER BY C.Nome";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<Cliente> clientes = new List<Cliente>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            Cliente cliente = BuildClienteFromReader(sqlDataReader);

                            clientes.Add(cliente);
                        }

                        return clientes;
                    }
                }
            }
        }

        public static async Task<Cliente> GetCliente(string codCliente)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT 
    C.Cliente, 
    C.Nome, 
    C.Fac_Mor, 
    C.Fac_Cp, 
    C.Fac_Cploc, 
    C.Fac_Local, 
    C.CondPag, 
    C.LimiteCred, 
    C.TotalDeb, 
    C.Vendedor, 
    C.Zona, 
    Z.Descricao as ZonaDescricao,
    C.eGAR_CodigoAPA 
FROM Clientes C
LEFT JOIN Zonas Z on Z.Zona = C.Zona
WHERE C.Cliente = @Cliente";

                    cmd.Parameters.AddWithValue("@Cliente", codCliente);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return null;

                        Cliente cliente = BuildClienteFromReader(sqlDataReader);

                        return cliente;
                    }
                }
            }
        }

        private static Cliente BuildClienteFromReader(SqlDataReader sqlDataReader)
        {
            return new Cliente
            {
                Codigo = (string)sqlDataReader["Cliente"],
                Nome = sqlDataReader["Nome"] == DBNull.Value ? null : (string)sqlDataReader["Nome"],
                Morada = sqlDataReader["Fac_Mor"] == DBNull.Value ? null : (string)sqlDataReader["Fac_Mor"],
                CodPostal = sqlDataReader["Fac_Cp"] == DBNull.Value ? null : (string)sqlDataReader["Fac_Cp"],
                LocalidadeCodPostal = sqlDataReader["Fac_Cploc"] == DBNull.Value ? null : (string)sqlDataReader["Fac_Cploc"],
                Localidade = sqlDataReader["Fac_Local"] == DBNull.Value ? null : (string)sqlDataReader["Fac_Local"],
                Zona = sqlDataReader["Zona"] == DBNull.Value ? null : (string)sqlDataReader["Zona"],
                ZonaDescricao = sqlDataReader["ZonaDescricao"] == DBNull.Value ? null : (string)sqlDataReader["ZonaDescricao"],
                CondPag = sqlDataReader["CondPag"] == DBNull.Value ? null : (string)sqlDataReader["CondPag"],
                LimiteCredito = sqlDataReader["LimiteCred"] == DBNull.Value ? 0d : (double)sqlDataReader["LimiteCred"],
                TotalDebito = sqlDataReader["TotalDeb"] == DBNull.Value ? 0d : (double)sqlDataReader["TotalDeb"],
                Vendedor = sqlDataReader["Vendedor"] == DBNull.Value ? null : (string)sqlDataReader["Vendedor"],
                CodigoAPA = sqlDataReader["eGAR_CodigoAPA"] == DBNull.Value ? null : (string)sqlDataReader["eGAR_CodigoAPA"]
            };
        }

        public static async Task<List<Fornecedor>> GetFornecedores()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Fornecedor, Nome FROM Fornecedores WHERE ISNULL(FornecedorAnulado, 0) = 0 AND CDU_Encomenda_Despesa_WEB = 1 ORDER BY Nome";

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<Fornecedor> fornecedores = new List<Fornecedor>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            Fornecedor fornecedor = BuildFornecedorFromReader(sqlDataReader);

                            fornecedores.Add(fornecedor);
                        }

                        return fornecedores;
                    }
                }
            }
        }

        public static RhpBEFuncionario GetFuncionario(string username, string password, string codFuncionario)
        {
            using (PrimaveraConnection pri = new PrimaveraConnection(ConfiguracoesLigacaoPrimaveraFactory.GetEmpresa(username, password), abrirBSO: true))
            {
                return pri.BSO.RecursosHumanos.Funcionarios.Edita(codFuncionario);
            }
        }

        public static async Task<Fornecedor> GetFornecedor(string codFornecedor)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT Fornecedor, Nome FROM Fornecedores WHERE Fornecedor = @Fornecedor";

                    cmd.Parameters.AddWithValue("@Fornecedor", codFornecedor);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                            return null;

                        Fornecedor fornecedor = BuildFornecedorFromReader(sqlDataReader);

                        return fornecedor;
                    }
                }
            }
        }

        private static Fornecedor BuildFornecedorFromReader(SqlDataReader sqlDataReader)
        {
            return new Fornecedor
            {
                Codigo = (string)sqlDataReader["Fornecedor"],
                Nome = sqlDataReader["Nome"] == DBNull.Value ? null : (string)sqlDataReader["Nome"],
            };
        }

        public static async Task<List<Artigo>> GetArtigos()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = @"
SELECT
	A.Artigo,
	A.Descricao,
	A.Marca,
	A.Modelo,
	A.CodBarras,
    A.UnidadeVenda,
	AM.PVP1
FROM
	Artigo A
	LEFT JOIN ArtigoMoeda AM ON A.Artigo = AM.Artigo AND AM.Moeda = ISNULL((SELECT MoedaBase FROM PRIEMPRE.dbo.Empresas WHERE Codigo = @Empresa), 'EUR')
WHERE 
	A.CDU_Encomenda_Despesa_WEB = 1
ORDER BY
	A.Descricao";

                    cmd.Parameters.AddWithValue("@Empresa", Config.Primavera_Empresa);

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<Artigo> artigos = new List<Artigo>();
                        while (await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        {
                            Artigo artigo = BuildArtigoFromReader(sqlDataReader);

                            artigos.Add(artigo);
                        }

                        return artigos;
                    }
                }
            }
        }

        public static async Task<Artigo> GetArtigo(string codArtigo)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringEmpresa()))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                return await GetArtigo(conn, null, codArtigo).ConfigureAwait(false);
            }
        }

        public static async Task<Artigo> GetArtigo(SqlConnection conn, SqlTransaction trans, string codArtigo)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.Transaction = trans;

                cmd.CommandText = @"
SELECT
	A.Artigo,
	A.Descricao,
	A.Marca,
	A.Modelo,
	A.CodBarras,
    A.UnidadeVenda,
	AM.PVP1
FROM
	Artigo A
	LEFT JOIN ArtigoMoeda AM ON A.Artigo = AM.Artigo AND AM.Moeda = ISNULL((SELECT MoedaBase FROM PRIEMPRE.dbo.Empresas WHERE Codigo = @Empresa), 'EUR')
WHERE
    A.CDU_Encomenda_Despesa_WEB = 1
	A.Artigo = @Artigo";

                cmd.Parameters.AddWithValue("@Artigo", codArtigo);
                cmd.Parameters.AddWithValue("@Empresa", Config.Primavera_Empresa);

                using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (!await sqlDataReader.ReadAsync().ConfigureAwait(false))
                        return null;

                    Artigo artigo = BuildArtigoFromReader(sqlDataReader);

                    return artigo;
                }
            }
        }

        private static Artigo BuildArtigoFromReader(SqlDataReader sqlDataReader)
        {
            return new Artigo
            {
                Codigo = (string)sqlDataReader["Artigo"],
                Descricao = sqlDataReader["Descricao"] == DBNull.Value ? null : (string)sqlDataReader["Descricao"],
                Marca = sqlDataReader["Marca"] == DBNull.Value ? null : (string)sqlDataReader["Marca"],
                Modelo = sqlDataReader["Modelo"] == DBNull.Value ? null : (string)sqlDataReader["Modelo"],
                CodBarras = sqlDataReader["CodBarras"] == DBNull.Value ? null : (string)sqlDataReader["CodBarras"],
                UnidadeVenda = sqlDataReader["UnidadeVenda"] == DBNull.Value ? null : (string)sqlDataReader["UnidadeVenda"],
                PVP1 = sqlDataReader["PVP1"] == DBNull.Value ? null : (double?)sqlDataReader["PVP1"],
            };
        }

        public static async Task<List<Equipa>> GetEquipasMembro(string utilizador)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
            {
                conn.Open();

                var query = @"SELECT
	                                Distinct E.Codigo 
                                FROM 
	                                Equipas E

								JOIN MembrosEquipa ME ON ME.CodEquipa = E.Codigo

                                WHERE
	                                ME.Utilizador = @Utilizador
	                                AND E.Activa = 1";

                var equipas = conn.Query<Equipa>(query, new { Utilizador = utilizador });

                return equipas.ToList();
            }
        }


        public static async Task<List<Equipa>> GetEquipasLeader(string utilizador)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
            {
                conn.Open();

                var query = @"SELECT
	                                * 
                                FROM 
	                                Equipas E

                                JOIN LeadersEquipa LE ON E.Codigo = LE.CodEquipa

                                WHERE
	                                LE.Utilizador = @Utilizador
	                                AND E.Activa = 1";

                var equipas = conn.Query<Equipa>(query, new { Utilizador = utilizador });

                return equipas.ToList();
            }
        }


        public static async Task<List<Equipa>> GetEquipas()
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
            {
                var query = new StringBuilder();

                query.Append(@"SELECT * FROM [Equipas]");

                var equipas = await conn.QueryAsync<Equipa>(query.ToString());

                return equipas.ToList();
            }
        }

        public static async Task<Equipa> GetEquipa(string codEquipa, string utilizador = null)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
            {
                var equipa = await conn.QueryAsync<Equipa>("SELECT * FROM [Equipas] WHERE Codigo = @Codigo", new { Codigo = codEquipa });

                var leaders = await conn.QueryAsync<Leader>("SELECT * FROM [LeadersEquipa] Where CodEquipa = @CodEquipa", new { CodEquipa = codEquipa });

                var membros = await conn.QueryAsync<Membro>("SELECT * FROM [MembrosEquipa] Where CodEquipa = @CodEquipa", new { CodEquipa = codEquipa });

                if (equipa == null)
                    return null;

                if (leaders != null)
                    equipa.First().Leaders = leaders.ToList();

                if (membros != null)
                    equipa.First().Membros = membros.ToList();

                return equipa.First();
            }
        }

        public static async void DeleteEquipa(string codEquipa)
        {
            using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {

                        DeleteMembrosEquipa(codEquipa, conn, tran);

                        DeleteLeadersEquipa(codEquipa, conn, tran);

                        conn.Execute(@"DELETE FROM [Equipas] WHERE Codigo = @CodEquipa", new { CodEquipa = codEquipa }, tran);

                        tran.Commit();

                    }
                    catch (Exception)
                    {
                        tran.Rollback();

                        throw;
                    }
                }
            }

        }

        public static async Task AlteraEquipa(string utilizador, Equipa equipa, Equipa modelEquipa)
        {
            try
            {
                equipa.ModoEdição = true;

                equipa.ModificadoPor = utilizador;

                DefinirCamposEquipa(modelEquipa, ref equipa);

                AlteraMembrosEquipa(modelEquipa, ref equipa, out List<string> membrosApagados);

                AlteraLeadersEquipa(modelEquipa, ref equipa, out List<string> leadersApagados);

                UpdateEquipa(equipa, leadersApagados, membrosApagados);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task AdicionaMembroEquipa(Membro membro)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
                {
                    conn.Open();

                    using (var tran = conn.BeginTransaction())
                    {
                        InsertMembroEquipa(membro, conn, tran);

                        tran.Commit();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static async Task ApagarMembroEquipa(string utilizador, string equipa)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
                {
                    conn.Open();

                    using (var tran = conn.BeginTransaction())
                    {
                        DeleteMembrosEquipa(equipa, conn, tran, utilizador);

                        tran.Commit();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void AlteraLeadersEquipa(Equipa modelEquipa, ref Equipa Equipa, out List<string> leadersApagados)
        {
            leadersApagados = new List<string>();

            // apagar leaders
            foreach (var leader in Equipa.Leaders)
            {
                // se leader não existir no pedido enviado, é porque deve ser apagado
                if (!modelEquipa.Leaders.Any(l => l.Utilizador == leader.Utilizador))
                    leadersApagados.Add(leader.Utilizador);
            }

            foreach (var leader in leadersApagados)
                Equipa.Leaders.RemoveAll(l => l.Utilizador == leader);


            foreach (Leader leaderModel in modelEquipa.Leaders)
            {
                var leader = Equipa.Leaders.Where(l => l.Utilizador == leaderModel.Utilizador).FirstOrDefault();

                if (leader != null)
                    DefinirCamposLeaderEquipa(leaderModel, leader);
                else
                    AdicionarLeaderEquipa(leaderModel, Equipa);

            }
        }

        public static void AlteraMembrosEquipa(Equipa modelEquipa, ref Equipa Equipa, out List<string> membrosApagados)
        {
            membrosApagados = new List<string>();

            // apagar membros
            foreach (var membro in Equipa.Membros)
            {
                // se membro não existir no pedido enviado, é porque deve ser apagado
                if (!modelEquipa.Membros.Any(l => l.Utilizador == membro.Utilizador))
                    membrosApagados.Add(membro.Utilizador);
            }

            foreach (var membro in membrosApagados)
                Equipa.Membros.RemoveAll(l => l.Utilizador == membro);


            foreach (Membro membroModel in modelEquipa.Membros)
            {
                var membro = Equipa.Membros.Where(l => l.Utilizador == membroModel.Utilizador).FirstOrDefault();

                if (membro != null)
                    DefinirCamposMembroEquipa(membroModel, membro);
                else
                    AdicionarMembroEquipa(membroModel, Equipa);
            }

        }

        public static async Task CriarEquipa(string utilizador, Equipa modelEquipa)
        {
            try
            {
                var novaEquipa = new Equipa();

                novaEquipa.ModoEdição = false;

                novaEquipa.CriadoPor = utilizador;

                DefinirCamposEquipa(modelEquipa, ref novaEquipa);

                foreach (Leader leader in modelEquipa.Leaders)
                    AdicionarLeaderEquipa(leader, novaEquipa);

                foreach (Membro membro in modelEquipa.Membros)
                    AdicionarMembroEquipa(membro, novaEquipa);

                InsertEquipa(novaEquipa);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void AdicionarMembroEquipa(Membro membroModal, Equipa novaEquipa)
        {
            var membroEquipa = new Membro();

            membroEquipa.ModoEdição = false;

            DefinirCamposMembroEquipa(membroModal, membroEquipa);

            novaEquipa.Membros.Add(membroEquipa);
        }

        private static void DefinirCamposMembroEquipa(Membro membroModal, Membro membroEquipa)
        {
            if (membroEquipa.ModoEdição == false)
            {
                membroEquipa.CodEquipa = membroModal.CodEquipa;
                membroEquipa.Utilizador = membroModal.Utilizador;
            }
        }

        private static void AdicionarLeaderEquipa(Leader leaderModal, Equipa novaEquipa)
        {
            var leaderEquipa = new Leader();

            leaderEquipa.ModoEdição = false;

            DefinirCamposLeaderEquipa(leaderModal, leaderEquipa);

            novaEquipa.Leaders.Add(leaderEquipa);
        }

        private static void DefinirCamposLeaderEquipa(Leader leaderModal, Leader leaderEquipa)
        {
            if (leaderEquipa.ModoEdição == false)
            {
                leaderEquipa.CodEquipa = leaderModal.CodEquipa;
                leaderEquipa.Utilizador = leaderModal.Utilizador;
            }
        }

        private static void DefinirCamposEquipa(Equipa modelEquipa, ref Equipa Equipa)
        {

            if (Equipa.ModoEdição == false)
            {
                Equipa.Codigo = modelEquipa.Codigo;
            }

            if (modelEquipa.Descricao != null)
                Equipa.Descricao = modelEquipa.Descricao;

            if (modelEquipa.Email != null)
                Equipa.Email = modelEquipa.Email;

            Equipa.Activa = modelEquipa.Activa;

        }

        public static void UpdateEquipa(Equipa equipaModel, List<string> LeadersApagados, List<string> MembrosApagados)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
                {
                    conn.Open();

                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            conn.Execute(@"UPDATE [Equipas] SET Codigo = @Codigo, Descricao = @Descricao, Email = @Email, Activa = @Activa, ModificadoPor = @ModificadoPor, DataModificacao = GETDATE() WHERE Codigo = @Codigo", equipaModel, tran);

                            foreach (var Membro in equipaModel.Membros)
                            {
                                if (Membro.ModoEdição)
                                    UpdateMembroEquipa(Membro, conn, tran);
                                else
                                    InsertMembroEquipa(Membro, conn, tran);
                            }

                            foreach (var Leader in equipaModel.Leaders)
                            {
                                if (Leader.ModoEdição)
                                    UpdateLeaderEquipa(Leader, conn, tran);
                                else
                                    InsertLeaderEquipa(Leader, conn, tran);
                            }

                            foreach (var membro in MembrosApagados)
                                DeleteMembrosEquipa(equipaModel.Codigo, conn, tran, membro);

                            foreach (var leader in LeadersApagados)
                                DeleteLeadersEquipa(equipaModel.Codigo, conn, tran, leader);

                            tran.Commit();
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void DeleteLeadersEquipa(string codEquipa, SqlConnection conn, SqlTransaction tran, string utilizador = null)
        {
            try
            {
                var query = new StringBuilder();

                query.Append(@"DELETE FROM [LeadersEquipa] WHERE CodEquipa = @CodEquipa");

                if (!string.IsNullOrWhiteSpace(utilizador))
                    query.AppendLine($" AND Utilizador = @Utilizador ");

                conn.Execute(query.ToString(), new { CodEquipa = codEquipa, Utilizador = utilizador }, tran); ;

            }
            catch (Exception)
            {
                tran.Rollback();

                throw;
            };
        }

        private static void DeleteMembrosEquipa(string codEquipa, SqlConnection conn, SqlTransaction tran, string utilizador = null)
        {
            try
            {
                var query = new StringBuilder();

                query.Append(@"DELETE FROM [MembrosEquipa] WHERE CodEquipa = @CodEquipa");

                if (!string.IsNullOrWhiteSpace(utilizador))
                    query.AppendLine($" AND Utilizador = @Utilizador ");

                conn.Execute(query.ToString(), new { CodEquipa = codEquipa, Utilizador = utilizador }, tran); ;

            }
            catch (Exception)
            {
                tran.Rollback();

                throw;
            };
        }

        public static void InsertEquipa(Equipa equipa)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
                {
                    conn.Open();

                    using (var tran = conn.BeginTransaction())
                    {

                        try
                        {
                            conn.Execute(@"INSERT INTO [Equipas] (Codigo, Descricao, Email, Activa, CriadoPor) 
                                            VALUES(@Codigo, @Descricao, @Email, @Activa, @CriadoPor)", equipa, tran);

                            foreach (var leader in equipa.Leaders)
                                InsertLeaderEquipa(leader, conn, tran);

                            foreach (var membro in equipa.Membros)
                                InsertMembroEquipa(membro, conn, tran);

                            tran.Commit();
                        }
                        catch (Exception)
                        {
                            tran.Rollback();

                            throw;
                        }
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void UpdateMembroEquipa(Membro membro, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                conn.Execute(@"UPDATE [MembrosEquipa] SET Nome = @Nome WHERE Id = @Id", membro, tran);
            }
            catch (Exception)
            {
                tran.Rollback();

                throw;
            }
        }

        private static void InsertMembroEquipa(Membro membro, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                conn.Execute(@"INSERT INTO [MembrosEquipa] (CodEquipa, Utilizador, Nome) 
                                    VALUES(@CodEquipa, @Utilizador, @Nome)", membro, tran);
            }
            catch (Exception)
            {
                tran.Rollback();

                throw;
            }
        }

        private static void UpdateLeaderEquipa(Leader leader, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                conn.Execute(@"UPDATE [LeadersEquipa] SET Nome = @Nome WHERE Id = @Id", leader, tran);
            }
            catch (Exception)
            {
                tran.Rollback();

                throw;
            }
        }

        private static void InsertLeaderEquipa(Leader leader, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                conn.Execute(@"INSERT INTO [LeadersEquipa] (CodEquipa, Utilizador, Nome) 
                                    VALUES(@CodEquipa, @Utilizador, @Nome)", leader, tran);
            }
            catch (Exception)
            {
                tran.Rollback();

                throw;
            }
        }

        public static string GetFilePathVia(string fileFolder, string fileNameWithoutExtension, string fileExtension, int via)
        {
            return Path.Combine(fileFolder, fileNameWithoutExtension + "_" + via + fileExtension);
        }

        public static void GerarPathFicheiroPdfTemporario(string tipoDoc, string serie, int numDoc, out string tempFilePath, out string fileTempFolder, out string fileNameWithoutExtension, out string fileExtension, string tempFolder = null)
        {
            if (tempFolder == null)
                tempFolder = Utils.GetTempFilesFolder();

            fileTempFolder = "";
            int tentativas = 0;
            while (string.IsNullOrEmpty(fileTempFolder) || (Directory.Exists(fileTempFolder) && tentativas < 5))
            {
                fileTempFolder = Path.Combine(tempFolder, Guid.NewGuid().ToString());
                tentativas++;

                if (tentativas == 5)
                    throw new FlorestasBemCuidadaWebApiException("Não foi possivel gerar pasta temporária depois de 5 tentativas.", false, EnumErrorCode.Geral, HttpStatusCode.InternalServerError);
            }

            Directory.CreateDirectory(fileTempFolder);

            try
            {
                fileNameWithoutExtension = $"{tipoDoc}_{serie}_{numDoc}";
                fileNameWithoutExtension = fileNameWithoutExtension.Replace("<", "").Replace(">", "").Replace(":", "").Replace("\"", "").Replace("/", "").Replace("\\", "").Replace("|", "").Replace("?", "").Replace("*", "");

                fileExtension = ".pdf";

                tempFilePath = Path.Combine(fileTempFolder, fileNameWithoutExtension + fileExtension);

                using (FileStream fileStream = File.Create(tempFilePath)) { }

                Utils.TentarApagarFicheiro(tempFilePath);
            }
            catch (Exception)
            {
                try
                {
                    Directory.Delete(fileTempFolder);
                }
                catch (Exception) { }

                throw;
            }
        }

        internal static bool isTeamLeader(string utilizador)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.GetConnectionStringPRIEMPRE()))
                {
                    conn.Open();

                    var teams = conn.QueryAsync<string>(@"SELECT CodEquipa FROM LeadersEquipa LE JOIN Equipas E ON E.Codigo = LE.CodEquipa WHERE E.Activa = 1 AND Utilizador = @Utilizador", new {Utilizador = utilizador});

                    if (teams.Result.Count() > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception) 
            {
                throw;
            }
        }
    }
}