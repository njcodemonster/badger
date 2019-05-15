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
    public interface IVendorRepository
    {
        Task<Vendor> GetById(int id);
        Task<List<Vendor>> GetAll(Int32 Limit);
        Task<String> Create(Vendor NewVendor);
        Task<Boolean> Update(Vendor VendorToUpdate);
        Task UpdateSpeific(Dictionary<String, String> ValuePairs, String where);
    }
    public class VendorRepo : IVendorRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "vendor";
        private string selectlimit = "30";
        public VendorRepo(IConfiguration config)
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

        public async Task<string> Create(Vendor NewVendor)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Vendor>(NewVendor);
                return result.ToString() ;
            }
        }

        public async Task<List<Vendor>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Vendor> result = new List<Vendor>();
                if(Limit > 0)
                {
                    result = await conn.QueryAsync<Vendor>("Select * from "+TableName+" Limit "+selectlimit+";");
                }
                else
                {
                    result = await conn.GetAllAsync<Vendor>();
                }
                return result.ToList();
            }
        }

       

        public async Task<Vendor> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                
                var result = await conn.GetAsync<Vendor>(id);
                return result;
            }
        }

        public async Task<Boolean> Update( Vendor VendorToUpdate)
        {
            
            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Vendor>(VendorToUpdate);
                return result;
            }
           
        }
        public async Task UpdateSpeific(Dictionary<String , String> ValuePairs, String where)
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

