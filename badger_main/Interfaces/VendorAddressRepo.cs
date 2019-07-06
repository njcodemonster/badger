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
    public interface IVendorAddress
    {
        Task<string> Create(VendorAddress NewVendorAddress);
        Task<Boolean> Update(VendorAddress VendorToUpdate);
    }
    public class VendorAddressRepo : IVendorAddress
    {
        private readonly IConfiguration _config;
        private string TableName = "vendor";
        private string selectlimit = "30";
        public VendorAddressRepo(IConfiguration config)
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
          Action: creating new vendor address to database
          URL: 
          Request:POST
          Input: vendor address
          output: result
       */
        public async Task<string> Create(VendorAddress NewVendor)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<VendorAddress>(NewVendor);
                return result.ToString();
            }
        }
        /*
          Developer: Azeem Hassan
          Date: 7-5-19 
          Action: update vendor address to database
          URL: 
          Request:PUT
          Input: vendor address
          output: result
       */
        public async Task<Boolean> Update(VendorAddress VendorToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<VendorAddress>(VendorToUpdate);
                return result;
            }

        }
    }
}
