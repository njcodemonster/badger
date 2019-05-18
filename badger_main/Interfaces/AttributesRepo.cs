using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using badgerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using CommonHelper;

namespace badgerApi.Interfaces
{
    public interface IAttributesRepository
    {
        Task<Attributes> GetById(int id);
        Task<List<Attributes>> GetAll(Int32 Limit);
        Task<String> Create(Attributes NewAttribute);
        Task<Boolean> Update(Attributes AttributesToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
    }

    public class AttributesRepo : IAttributesRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "attributes";
        private string selectlimit = "30";
        public AttributesRepo(IConfiguration config)
        {

            _config = config;
            selectlimit = _config.GetValue<string>("configs:Default_select_Limit");

        }
        public IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("ProductsDatabase"));
            }
        }

        public async Task<string> Create(Attributes NewAttribute)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Attributes>(NewAttribute);
                return result.ToString();
            }
        }

        public async Task<List<Attributes>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Attributes> result = new List<Attributes>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<Attributes>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<Attributes>();
                }
                return result.ToList();
            }
        }



        public async Task<Attributes> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<Attributes>(id);
                return result;
            }
        }

        public async Task<Boolean> Update(Attributes AttributesToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Attributes>(AttributesToUpdate);
                return result;
            }

        }
        public async Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where)
        {
            QueryHelper qHellper = new QueryHelper();
            string UpdateQuery = qHellper.MakeUpdateQuery(ValuePairs, TableName, where);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync(UpdateQuery);

            }

        }
    }
}
