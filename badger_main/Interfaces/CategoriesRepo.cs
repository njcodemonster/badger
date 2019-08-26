
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
    public interface ICategoriesRepository
    {
        List<Categories> allCategories { get; set; }
        Task<string> Create(Categories Categories);
        Task<Boolean> Update(Categories VendorToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
    }
    public class CategoriesRepo : ICategoriesRepository
    {
        public List<Categories> allCategories { get; set; }
        private readonly IConfiguration _config;
        private string TableName = "categories";
        public CategoriesRepo(IConfiguration config)
        {

            _config = config;
            allCategories = GetAllCategories();


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
          Date: 8-23-19 
          Action: creating new category to database
          Input: Categories model
          output: result
       */
        public async Task<string> Create(Categories NewCategories)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Categories>(NewCategories);

                return result.ToString();
            }
        }
        /*
          Developer: Hamza Haq
          Date: 8-23-19 
          Action: update category to database
          Input: Categories Model
          output: result
       */
        public async Task<Boolean> Update(Categories CategoryToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Categories>(CategoryToUpdate);
                return result;
            }

        }
        /*
         Developer: Hamza Haq
         Date: 8-23-19 
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
        /*
         Developer: Hamza Haq
         Date: 8-23-19 
         Action: get all categories
         Input: none
         output: result
      */
        public List<Categories> GetAllCategories()
        {
            IEnumerable<Categories> result = new List<Categories>();
            using (IDbConnection conn = Connection)
            {
                result = conn.GetAll<Categories>();

            }
            return result.ToList();

        }
    }
}
