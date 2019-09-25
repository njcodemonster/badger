using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using static Dapper.SqlMapper;

namespace badgerApi.Helpers
{
    public class DataAccessLayer
    {
        private string _dbName;
        private readonly IConfiguration _config;
        public IDbConnection Connection => new MySqlConnection(_config.GetConnectionString(_dbName));
        public DataAccessLayer(IConfiguration configuration,string dbName)
        {
            _config = configuration;
            _dbName = dbName;
        }

        public async Task<IEnumerable<T>> ExecuteProcedureAsync<T>(string procedureName, DynamicParameters mySqlParameter)
        {
            using (IDbConnection conn = Connection)
            {
                return await conn.QueryAsync<T>(procedureName, mySqlParameter, null, null, CommandType.StoredProcedure);
            }
        }

        public async Task<(IEnumerable<T1> list,T2 count)> GetByMultiQuery<T1,T2>(string query)
        {
            using (IDbConnection conn = Connection)
            {
                var data = await conn.QueryMultipleAsync(query);
               var t1 = await data.ReadAsync<T1>();
                var t2 = await data.ReadSingleAsync<T2>();
                return (t1, t2);
            }
        }
    }
}
