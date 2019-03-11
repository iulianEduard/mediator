using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace TransactionsProcessor.Data
{
    public interface IDatabase
    {
        Task<IEnumerable<TResult>> Query<TResult>(string sql, object args = null, bool isStoreProcedure = true, int timeout = 30);

        Task<TResult> QuerySingle<TResult>(string sql, object args = null, bool isStoreProcedure = true, int timeout = 30);

        Task<bool> Execute(string sql, object args = null, bool isStoreProcedure = true, int timeout = 30);
    }

    public class Database : IDatabase
    {
        private readonly IDatabaseConnection _connection;

        public Database(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<TResult>> Query<TResult>(string sql, object args = null, bool isStoreProcedure = true, int timeout = 30)
        {
            using (var con = await _connection.Open())
            {
                return await con.QueryAsync<TResult>(sql, args, commandType: isStoreProcedure ? CommandType.StoredProcedure : CommandType.Text, commandTimeout: timeout);
            }
        }

        public async Task<TResult> QuerySingle<TResult>(string sql, object args = null, bool isStoreProcedure = true, int timeout = 30)
        {
            using (var con = await _connection.Open())
            {
                return await con.QueryFirstOrDefaultAsync<TResult>(sql, args, commandType: isStoreProcedure ? CommandType.StoredProcedure : CommandType.Text, commandTimeout: timeout);
            }
        }

        public async Task<bool> Execute(string sql, object args = null, bool isStoreProcedure = true, int timeout = 30)
        {
            using (var con = await _connection.Open())
            {
                await con.ExecuteAsync(sql, args, commandType: isStoreProcedure ? CommandType.StoredProcedure : CommandType.Text, commandTimeout: timeout);
            }

            return true;
        }
    }

    public interface IDatabaseConnection
    {
        Task<IDbConnection> Open();
    }

    public class DatabaseConnection : IDatabaseConnection
    {
        private SqlConnection _conection;

        // ReSharper disable once MemberCanBePrivate.Global
        public string ConnectionString { get; }

        public DatabaseConnection()
            : this(string.Empty)
        { }

        public DatabaseConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<IDbConnection> Open()
        {
            _conection = new SqlConnection(ConnectionString);

            await _conection.OpenAsync();
            return _conection;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            _conection?.Dispose();
        }
    }
}
