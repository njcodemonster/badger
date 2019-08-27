
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
using System.Dynamic;

namespace badgerApi.Interfaces
{
    public interface IProductCategoriesRepository
    {

        Task<string> Create(ProductCategories ProductCategories);
        Task<Boolean> Update(ProductCategories VendorToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<string> Delete(ProductCategories DeleteProductCategory);
    }
    public class ProductCategoriesRepo : IProductCategoriesRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "product_categories";
        public ProductCategoriesRepo(IConfiguration config)
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
          Date: 8-23-19 
          Action: creating new product category to database
          Input: ProductCategories model
          output: result
       */
        public async Task<string> Create(ProductCategories NewProductCategories)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<ProductCategories>(NewProductCategories);

                return result.ToString();
            }
        }
        /*
        Developer: Hamza Haq
        Date: 8-26-19 
        Action: Delete product category in database
        Input: ProductCategories model
        output: result
     */
        public async Task<string> Delete(ProductCategories deleteProductCategory)
        {
            using (IDbConnection conn = Connection)
            {
                String DeleteQuery = "Delete From product_categories WHERE product_id=" + deleteProductCategory.product_id + " and  category_id =" + deleteProductCategory.category_id.ToString();
                var result = await conn.QueryAsync(DeleteQuery);
                return result.ToString();
            }
        }

        /*
          Developer: Hamza Haq
          Date: 8-23-19 
          Action: update product category to database
          Input: ProductCategories model
          output: result
       */
        public async Task<Boolean> Update(ProductCategories CategoryToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<ProductCategories>(CategoryToUpdate);
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

    }
}
