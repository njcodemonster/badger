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
using GenericModals;

namespace badgerApi.Interfaces
{
    public interface IAttributesRepository
    {
        Task<Attributes> GetById(int id);
        Task<List<Attributes>> GetByTypeId(int attribute_type_id);
        Task<List<AutoComplete>> GetByTypeId(int attribute_type_id,string name);
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
        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: insert new sttribute data to database
           Input: new Attribute data
           output: Attribute id
        */
        public async Task<string> Create(Attributes NewAttribute)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Attributes>(NewAttribute);
                return result.ToString();
            }
        }
        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: getting attributes with limit from database
            Input: limit
            output: attributes list
         */
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

        /*
           Developer: Ubaid
           Date: 5-8-19 
           Action: getting attributes with type id from database
           Input: type id
           output: attributes list
        */
        public async Task<List<Attributes>> GetByTypeId(Int32 attribute_type_id)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Attributes> result = new List<Attributes>();
                
                    result = await conn.QueryAsync<Attributes>("Select * from " + TableName + " Where attribute_type_id=" + attribute_type_id.ToString() + ";");
                
               
                return result.ToList();
            }
        }

        /*
       Developer: Hamza Haq
       Date: 21-8-19 
       Action: getting attributes with type id and name for autocomplete from database
       Input: type id
       output: attributes list
        */
        public async Task<List<AutoComplete>> GetByTypeId(Int32 attribute_type_id,string name)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<AutoComplete> result = new List<AutoComplete>();

                string _query = "Select  attribute_id as value , attribute_display_name as label from " + TableName + " Where attribute like '%" + name + "%' and attribute_type_id=" + attribute_type_id.ToString() + ";";
                result = await conn.QueryAsync<AutoComplete>(_query);
                return result.ToList();
            }
        }
        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: getting attribute by id from database
            Input: int id
            output: attribute
         */
        public async Task<Attributes> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<Attributes>(id);
                return result;
            }
        }

        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: update attribute to database
           Input: AttributesToUpdate
           output: result
        */
        public async Task<Boolean> Update(Attributes AttributesToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Attributes>(AttributesToUpdate);
                return result;
            }

        }

        /*
          Developer: Azeem Hassan
          Date: 7-5-19 
          Action: update specific attribute to database
          Input: value and where
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
