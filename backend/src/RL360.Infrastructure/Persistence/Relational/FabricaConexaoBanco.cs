using System.Data;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace RL360.Infrastructure.Persistence.Relational;

public interface IFabricaConexaoBanco
{
    IDbConnection Criar();
}

public sealed class FabricaConexaoSqlServer(string connectionString) : IFabricaConexaoBanco
{
    public IDbConnection Criar() => new SqlConnection(connectionString);
}

public sealed class FabricaConexaoPostgres(string connectionString) : IFabricaConexaoBanco
{
    public IDbConnection Criar() => new NpgsqlConnection(connectionString);
}
