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
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
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
        /*
            Developer: Azeem Hassan
            Date: 7-8-19 
            Action: insert NewAttributeValue data to database
            Input: new attribute value
            output: attribute id
         */
        public async Task<string> Create(AttributeValues NewAttributeValue)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<AttributeValues>(NewAttributeValue);
                return result.ToString();
            }
        }
        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: getting AttributeValues from database
            Input: limit
            output: AttributeValues list
         */
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


        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: getting AttributeValues by id from database
            Input: int id
            output: AttributeValues
         */
        public async Task<AttributeValues> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<AttributeValues>(id);
                return result;
            }
        }

        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: Update AttributeValues to database
            Input: AttributeValuesToUpdate
            output: result
         */
        public async Task<Boolean> Update(AttributeValues AttributeValuesToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<AttributeValues>(AttributeValuesToUpdate);
                return result;
            }

        }
        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: UpdateSpecific records to database
            Input: Dictionary<String, String> ValuePairs, String where condition
            output: bolean
         */
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
