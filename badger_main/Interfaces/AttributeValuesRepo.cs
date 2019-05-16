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
    public interface IAttributeValuesRepository
    {
        Task<AttributeValues> GetById(int id);
        Task<List<AttributeValues>> GetAll(Int32 Limit);
        Task<String> Create(AttributeValues NewAttributeValue);
        Task<Boolean> Update(AttributeValues AttributeValuesToUpdate);
        Task UpdateSpeific(Dictionary<String, String> ValuePairs, String where);
    }
    public class AttributeValuesRepo : IAttributeValuesRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "attribute_values";
        private string selectlimit = "30";
        public AttributeValuesRepo(IConfiguration config)
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

        public async Task<string> Create(AttributeValues NewAttributeValue)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<AttributeValues>(NewAttributeValue);
                return result.ToString();
            }
        }

        public async Task<List<AttributeValues>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<AttributeValues> result = new List<AttributeValues>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<AttributeValues>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<AttributeValues>();
                }
                return result.ToList();
            }
        }



        public async Task<AttributeValues> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<AttributeValues>(id);
                return result;
            }
        }

        public async Task<Boolean> Update(AttributeValues AttributeValuesToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<AttributeValues>(AttributeValuesToUpdate);
                return result;
            }

        }
        public async Task UpdateSpeific(Dictionary<String, String> ValuePairs, String where)
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
