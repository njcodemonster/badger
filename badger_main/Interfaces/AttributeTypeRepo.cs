using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using GenericModals.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using CommonHelper;

namespace badgerApi.Interfaces
{
    public interface IAttributeTypeRepository
    {
        Task<AttributeType> GetById(int id);
        Task<List<AttributeType>> GetAll(Int32 Limit);
        Task<String> Create(AttributeType NewAttributeType);
        Task<Boolean> Update(AttributeType AttributeTypeToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
    }
    public class AttributeTypeRepo : IAttributeTypeRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "attribute_type";
        private string selectlimit = "30";
        public AttributeTypeRepo(IConfiguration config)
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
           Action: creating new attribute type to database
           Input: new attribute
           output: attribute id
        */
        public async Task<string> Create(AttributeType NewAttributeType)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<AttributeType>(NewAttributeType);
                return result.ToString();
            }
        }

        /*
           Developer: Azeem Hassan
           Date: 7-8-19 
           Action: getting list of attributes from database 
           Input: limit
           output: list of attribute
        */
        public async Task<List<AttributeType>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<AttributeType> result = new List<AttributeType>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<AttributeType>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<AttributeType>();
                }
                return result.ToList();
            }
        }


        /*
          Developer: Azeem Hassan
          Date: 7-8-19 
          Action: getting attributes by id from database 
          Input: int id
          output: attribute
       */
        public async Task<AttributeType> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<AttributeType>(id);
                return result;
            }
        }
        /*
            Developer: Azeem Hassan
            Date: 7-8-19 
            Action: update AttributeType to database
            Input: AttributeTypeToUpdate
            output: result
        */
        public async Task<Boolean> Update(AttributeType AttributeTypeToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<AttributeType>(AttributeTypeToUpdate);
                return result;
            }

        }
        /*
           Developer: Azeem Hassan
           Date: 7-8-19 
           Action: UpdateSpecific to database
           Input: Dictionary<String, String> ValuePairs, String where
           output: result
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
