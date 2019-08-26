
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
    public interface IVendorProductRepository
    {

        Task<string> Create(VendorProducts VendorProduct);
        Task<Boolean> Update(VendorProducts VendorToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
    }
    public class VendorProductRepo : IVendorProductRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "vendor_products";
        public VendorProductRepo(IConfiguration config)
        {

            _config = config;

        }
        public IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("ProductsDatabase"));
            }
        }
        /*
          Developer: Hamza Haq
          Date: 8-20-19 
          Action: creating new vendor product to database
          Input: vendor repo
          output: result
       */
        public async Task<string> Create(VendorProducts NewVendorProduct)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<VendorProducts>(NewVendorProduct);

                return result.ToString();
            }
        }
        /*
          Developer: Hamza Haq
          Date: 8-20-19 
          Action: update vendor product to database
          Input: vendor products 
          output: result
       */
        public async Task<Boolean> Update(VendorProducts VendorToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<VendorProducts>(VendorToUpdate);
                return result;
            }

        }
        /*
         Developer: Hamza Haq
         Date: 8-20-19 
         Action: updating specific fields to database
         Input: fields value and where to repair
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
