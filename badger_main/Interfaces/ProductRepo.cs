
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
    public interface IProductRepository
    {
        Task<Product> GetById(int id);
        Task<List<Product>> GetAll(Int32 Limit);
        Task<Product> Create(Vendor NewVendor);
        Task<Product> Update(Vendor VendorToUpdate);
        Task UpdateSpeific(Dictionary<String, String> ValuePairs, String where);
    }
    public class ProductRepo : IProductRepository
    {

        private readonly IConfiguration _config;
        private string TableName = "product";
        private string selectlimit = "";
        public ProductRepo(IConfiguration config)
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

        public Task<Product> Create(Vendor NewVendor)
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> GetAll(int Limit)
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> Update(Vendor VendorToUpdate)
        {
            throw new NotImplementedException();
        }

        public Task UpdateSpeific(Dictionary<string, string> ValuePairs, string where)
        {
            throw new NotImplementedException();
        }
    }
}


