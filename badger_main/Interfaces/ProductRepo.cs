
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
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> GetAll(Int32 Limit);
        Task<String> Create(Product NewProduct);
        Task<bool> UpdateAsync(Product ProductToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<String> CreateProductAttribute(ProductAttributes NewProductAttribute);
        Task<String> CreateAttributeValues(ProductAttributeValues NewProductAttributeValues);
        Task<List<Product>> GetProductsByVendorId(String Vendor_id);
        Task<IEnumerable<ProductProperties>> GetProductProperties(string id);
        Task<IEnumerable<Productpairwith>> GetProductpairwiths(string id);
        Task<IEnumerable<Productcolorwith>> GetProductcolorwiths(string id);
        Task<IEnumerable<ProductImages>> GetProductImages(string id);
        Task<IEnumerable<ProductDetails>> GetProductDetails(string id);
        Task<IEnumerable<AllColors>> GetAllProductColors();
        Task<IEnumerable<AllTags>> GetAllProductTags();
        Task<Int32> GetProductShootStatus(string id);
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

        public async Task<string> Create(Product NewProduct)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Product>(NewProduct);
                return result.ToString();
            }
        }

        public async Task<List<Product>> GetAll(int Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Product> result = new List<Product>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<Product>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<Product>();
                }
                return result.ToList();
            }
        }

        public async Task<Product> GetByIdAsync(int id)
        {

            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<Product>(id);
                return result;

            }
        }

        public async Task<bool>  UpdateAsync(Product ProductToUpdate)
        {
           
            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Product>(ProductToUpdate);
                return result;
            }
        }

        public async Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where)
        {
            QueryHelper qHellper = new QueryHelper();
            string UpdateQuery = qHellper.MakeUpdateQuery(ValuePairs, TableName, where);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync(UpdateQuery);

            }

        }
        public async Task<List<Product>> GetProductsByVendorId(String Vendor_id)
        {
            IEnumerable<Product> toReturn;
            //List<Product> toReturn = new List<Product>();
            using(IDbConnection conn = Connection)
            {
                toReturn = await conn.QueryAsync<Product>("Select * from product where vendor_id=" + Vendor_id);
            }
            return toReturn.ToList();
        }
        public async Task<string> CreateProductAttribute(ProductAttributes NewProductAttributes)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<ProductAttributes>(NewProductAttributes);
                return result.ToString();
            }
        }
        public async Task<IEnumerable<Productpairwith>> GetProductpairwiths(string id)
        {
            IEnumerable<Productpairwith> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<Productpairwith>("select pairing_product_id,paired_product_id from product_pair_with where pairing_product_id= '" + id + "'");
            }
            return productProperties;
        }
        public async Task<IEnumerable<Productcolorwith>> GetProductcolorwiths(string id)
        {
            IEnumerable<Productcolorwith> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<Productcolorwith>("select product_id,same_color_product_id from product_color_with where product_id= '" + id + "'");
            }
            return productProperties;
        }
        public async Task<IEnumerable<ProductImages>> GetProductImages(string id)
        {
            IEnumerable<ProductImages> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<ProductImages>("select * from product_images where product_id= '" + id + "'");
            }
            return productProperties;
        }
        public async Task<IEnumerable<ProductProperties>> GetProductProperties (string id)
        {
            IEnumerable<ProductProperties> productProperties;
            using (IDbConnection conn = Connection)
            {
                 productProperties = await conn.QueryAsync<ProductProperties>("select A.attribute_id,A.sku,A.product_id,C.attribute_type_id,C.attribute,C.attribute_display_name,D.value from product_attributes as A  , attributes as C ,attribute_values as D where (A.product_id = "+id+" and A.attribute_id= C.attribute_id and D.product_id = A.product_id and D.attribute_id= A.attribute_id) ");
                
            }
            return productProperties;

        }
        public async Task<IEnumerable<ProductDetails>> GetProductDetails(string id)
        {
            IEnumerable<ProductDetails> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<ProductDetails>("select * from product_page_details where product_id= '" + id + "'");

            }
            return productProperties;

        }
        public async Task<Int32> GetProductShootStatus(string id)
        {
            Int32 shootstatus = 0;
            using (IDbConnection conn = Connection)
            {
                shootstatus =  conn.QueryAsync<Int32>("select product_shoot_status_id from product_photoshoots where product_id= '" + id + "'").Result.First();

            }
            return shootstatus;

        }
        public async Task<IEnumerable<AllColors>> GetAllProductColors()
        {
            IEnumerable<AllColors> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<AllColors>("select distinct(value_id),value from attribute_values where attribute_id= 1");

            }
            return productProperties;

        }
        public async Task<IEnumerable<AllTags>> GetAllProductTags()
        {
            IEnumerable<AllTags> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<AllTags>("select attribute_id,attribute,attribute_display_name,sub_heading from attributes where attribute_type_id=4 order by sub_heading" );

            }
            return productProperties;

        }
        public async Task<string> CreateAttributeValues(ProductAttributeValues NewProductAttributeValues)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<ProductAttributeValues>(NewProductAttributeValues);
                return result.ToString();
            }

        }

    }
}


