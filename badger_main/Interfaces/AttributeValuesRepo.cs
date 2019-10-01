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
    public interface IAttributeValuesRepository
    {
        Task<AttributeValues> GetById(int id);
        Task<List<AttributeValues>> GetAll(Int32 Limit);
        Task<String> Create(AttributeValues NewAttributeValue);
        Task<Boolean> Update(AttributeValues AttributeValuesToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<List<AttributeValues>> GetAttributesbyProductID(int productID, int attribute_type_id);
        Task<bool> DeleteById(int id);
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
                var result = conn.Insert<AttributeValues>(NewAttributeValue);

                //var sQuery = "Select value_id from attribute_values where attribute_id=" + NewAttributeValue.attribute_id + " and product_id=" + NewAttributeValue.Product_id + "";
                //var _NewAttributeValue = conn.QuerySingle<AttributeValues>(sQuery);

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


        /*
        Developer: Hamza Haq
        Date: 9-23-19 
        Action: Get 
        Input: productID id and attributeTypeid
        output: list of attributes
        */
        public async Task<List<AttributeValues>> GetAttributesbyProductID(int productID, int attribute_type_id)
        {
            List<AttributeValues> _attributeValues = new List<AttributeValues>();
            string sQuery = @"SELECT a.attribute_display_name as attribute_Name ,av.value_id ,a.attribute_id AS attribute_id , VALUE AS value FROM attributes AS a 
                            INNER JOIN attribute_values AS av ON a.attribute_id=av.attribute_id
                            WHERE attribute_type_id=@attributeTypeID AND av.product_id=@productID";

            sQuery = sQuery.Replace("@attributeTypeID", attribute_type_id.ToString());
            sQuery = sQuery.Replace("@productID", productID.ToString());

            using (IDbConnection conn = Connection)
            {
                var queryResult = await conn.QueryAsync<AttributeValues>(sQuery);
                _attributeValues = queryResult.ToList();

            }
            return _attributeValues;



        }

        /*
           Developer: Hamza Haq
           Date: 9-23-19 
           Action: getting AttributeValues by id from database
           Input: int id
           output: AttributeValues
        */
        public async Task<bool> DeleteById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                AttributeValues _attributeValues = new AttributeValues();
                _attributeValues.value_id = id;
                var result = await conn.DeleteAsync<AttributeValues>(_attributeValues);
                return result;
            }
        }
    }
}
