using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

public class DbHelper
{
    private readonly string _connectionString;
    private readonly bool _isMySql;

    public DbHelper(string connectionString, bool isMySql)
    {
        _connectionString = connectionString;
        _isMySql = isMySql;
    }

    public async Task<int> ExecuteAsync(string sql, params (string, object)[] parameters)
    {
        if (_isMySql)
        {
            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conn);
            foreach (var (name, value) in parameters)
            {
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
            }

            return await cmd.ExecuteNonQueryAsync();
        }
        else
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            foreach (var (name, value) in parameters)
            {
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
            }

            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
