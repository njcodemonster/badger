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

namespace badgerApi.Interfaces
{ 
    public interface IVendorRepository
    {
        Task<Vendor> GetByID(int id);
        Task<List<Vendor>> GetAll(Int32 Limit);
    }
    public class VendorRepo : IVendorRepository
    {
        private readonly IConfiguration _config;

        public VendorRepo(IConfiguration config)
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
      

        public async Task<List<Vendor>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                IEnumerable<Vendor> result = new List<Vendor>();
                if(Limit > 0)
                {
                    result = await conn.QueryAsync<Vendor>("Select * from vendor Limit 30;");
                }
                else
                {
                    result = await conn.GetAllAsync<Vendor>();
                }
                return result.ToList();
            }
        }

       

        public async Task<Vendor> GetByID(int id)
        {
            using (IDbConnection conn = Connection)
            {
                
                conn.Open();
                var result = await conn.GetAsync<Vendor>(id);
                return result;
            }
        }
    }
}

