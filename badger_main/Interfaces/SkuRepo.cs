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
using System.Dynamic;

namespace badgerApi.Interfaces
{
    public interface ISkuRepo
    {
        Task<Sku> GetById(int id);
        Task<List<Sku>> GetAll(Int32 Limit);
        Task<String> Create(Sku NewSku);
        Task<Boolean> Update(Sku SkuToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<List<Sku>> CheckSkuExist(string sku);
        Task<Object> GetSku(string sku);
    }

    public class SkuRepo : ISkuRepo
    {
        private readonly IConfiguration _config;
        private string TableName = "sku";
        private string selectlimit = "30";
        public SkuRepo(IConfiguration config)
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
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: insert sku to database
        Input: new sku data
        output: string of sku id
        */
        public async Task<string> Create(Sku NewSku)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Sku>(NewSku);
                return result.ToString();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: get all sku data from database
        Input: int limit
        output: list of sku
        */
        public async Task<List<Sku>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Sku> result = new List<Sku>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<Sku>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<Sku>();
                }
                return result.ToList();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: get sku by id from database
        Input: int id
        output: list of sku
        */
        public async Task<Sku> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<Sku>(id);
                return result;
            }
        }


        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update sku by id  to database
        Input: sku data
        output: Boolean
        */
        public async Task<Boolean> Update(Sku SkuToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Sku>(SkuToUpdate);
                return result;
            }

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update specific sku by id to database
        Input: fields value and where to update
        output: Boolean
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
        Developer: Sajid Khan
        Date: 7-19-19 
        Action: Check Sku already Exist data from database
        Input: string sku
        output: list of sku check
        */
        public async Task<List<Sku>> CheckSkuExist(string sku)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Sku> result = new List<Sku>();

                string squery = "Select * from " + TableName + " WHERE sku = '" + sku + "';";

                result = await conn.QueryAsync<Sku>(squery);

                return result.ToList();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 08-09-19 
        Action: Get All sku by sku from database 
        Input: string sku
        output: dynamic list of sku data
        */
        public async Task<Object> GetSku(string sku)
        {
            dynamic skuDetails = new ExpandoObject();

            string sQuery = "SELECT sku.sku_id AS value,sku.sku AS label,product.product_vendor_image AS image,'sku' AS type FROM sku, product WHERE sku.product_id = product.product_id AND LOWER(sku.sku) LIKE '" + sku.ToLower()+"%';";

            using (IDbConnection conn = Connection)
            {
                skuDetails = await conn.QueryAsync<object>(sQuery);

            }
            return skuDetails;
        }

    }
}