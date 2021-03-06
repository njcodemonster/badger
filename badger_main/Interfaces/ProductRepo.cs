﻿
using badgerApi.Models;
using CommonHelper;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using GenericModals.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using CommonHelper;
using Newtonsoft.Json;

using System.Dynamic;
using GenericModals.PurchaseOrder;

namespace badgerApi.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> GetAll(Int32 Limit);
        Task<object> GetProductsPairWithSearch(string ProdcutId);
        Task<String> Create(Product NewProduct);
        Task<bool> UpdateAsync(Product ProductToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task AttributeUpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<String> CreateProductAttribute(ProductAttributes NewProductAttribute);
        Task<String> CreateAttributeValues(ProductAttributeValues NewProductAttributeValues);
        Task<List<Product>> GetProductsByVendorId(String Vendor_id, int product_id);
        Task<IEnumerable<ProductProperties>> GetProductProperties(string id);
        Task<string> CreatePOLineitems(PurchaseOrderLineItems NewLineitem);
        Task<string> UpdatePoLineItems(PurchaseOrderLineItems NewLineitem);
        Task<IEnumerable<Productpairwith>> GetProductpairwiths(string id);
        Task<IEnumerable<Productcolorwith>> GetProductcolorwiths(string id);
        Task<IEnumerable<ProductImages>> GetProductImages(string id);
        Task<IEnumerable<ProductDetails>> GetProductDetails(string id);
        Task<IEnumerable<AllColors>> GetAllProductColors();
        Task<IEnumerable<AllTags>> GetAllProductTags();
        Task<List<PhotoshootModels>> GetPhotoshootModels();
        Task<List<ProductCategories>> GetProductCategories(string productID);
        Task<Int32> GetProductShootStatus(string id);
        Task<Int32> GetProductPhotoshootModel(string id);
        Task<string> CreateProductUsedIn(ProductUsedIn NewUsedIn);
        Task<string> CreateProductImages(Productimages NewProductImages);
        Task<string> AddEditProductPageDetails(string product_id, string product_detail_type, string value, string userid, string dateTime);
        Task<bool> UpadateImagePrimary(int product_image_id, int is_primary);
        Task<Object> GetProduct(string product_name);
        Task<string> AddPairWithProduct(string product_id, string pair_product_id, string userid, string dateTime);
        Task<string> RemovePairWithProduct(string product_id, string pair_product_id);
        Task<string> AddOtherColorProducts(string product_id, string pair_product_id, string userid, string dateTime);
        Task<string> RemoveOtherColorProducts(string product_id, string same_color_product_id);
        Task<Object> GetProductIdsByPurchaseOrder();
        Task<Object> GetPublishedProductIds(string poids);
        Task<bool> DeleteProduct(string product_id, string po_id);
        Task<Object> GetProductsbyVendorAutoSuggest(int vendor_id, string productName);
    }
    public class ProductRepo : IProductRepository
    {

        private readonly IConfiguration _config;
        private string TableName = "product";
        private string TableProductAttributes = "product_attributes";
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

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Insert new Product data into database
        Input: Product data
        output: string of Product data
        */
        public async Task<string> Create(Product NewProduct)
        {
            using (IDbConnection conn = Connection)
            {
                long result = conn.Insert<Product>(NewProduct);
                return result.ToString();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get all Product data from database
        Input: int limit
        output: list of Product data
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Product data by id from database
        Input: int id
        output: list of Product data
        */
        public async Task<Product> GetByIdAsync(int id)
        {

            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<Product>(id);
                return result;

            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Update Product data by id from database
        Input: Product data
        output: Boolean
        */
        public async Task<bool> UpdateAsync(Product ProductToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Product>(ProductToUpdate);
                return result;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Update Product data with specific fields by id from database
        Input: Dictionary<String , String> ValuePairs, String where condition
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
        Developer: Mohi
        Date: 9-7-19 
        Action: Update data with specific fields by id 
        Input: Dictionary<String , String> ValuePairs, String where condition, tablename
        output: Boolean
        */
        public async Task UpdateSpecificData(Dictionary<String, String> ValuePairs, String where, String table_name)
        {
            QueryHelper qHellper = new QueryHelper();
            string UpdateQuery = qHellper.MakeUpdateQuery(ValuePairs, table_name, where);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync(UpdateQuery);

            }

        }


        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Products data by vendor id from database
        Input:string vendor id
        output: List of products by vendor id
        */
        public async Task<List<Product>> GetProductsByVendorId(String Vendor_id, int product_id)
        {
            IEnumerable<Product> toReturn;
            //List<Product> toReturn = new List<Product>();
            using (IDbConnection conn = Connection)
            {

                string querytoRun = @"SELECT product.product_id ,product.product_type_id,product.vendor_id ,product.product_availability ,product.published_at ,product.product_vendor_image,product.product_name ,product.product_url_handle     
                                      ,product.product_description ,product.vendor_color_name ,product.sku_family ,product.size_and_fit_id  ,product.wash_type_id  ,product.product_discount ,product.product_cost  ,product.product_retail ,product.published_status ,product.is_on_site_status  ,product.created_by          
                                      ,product.updated_by ,product.updated_at  ,product.created_at   ,vendor_products.vendor_color_code,vendor_products.vendor_product_code ,
                                      (SELECT CAST(CONCAT('[', GROUP_CONCAT(JSON_OBJECT('product_category_id', pc.product_category_id, 'category_id', pc.category_id)), ']') AS JSON) AS productCategories  FROM product_categories pc WHERE pc.product_id = product.product_id) productCategories
                                      ,CASE WHEN pa.product_attribute_id IS NOT NULL THEN CAST(CONCAT('[',GROUP_CONCAT(JSON_OBJECT('attribute_id', pa.attribute_id ,'sku', pa.sku,'vendor_size',av.value)),']') AS JSON) ELSE '[]' END AS skulist  
                                      FROM product 
                                      INNER JOIN vendor_products ON  product.vendor_id = vendor_products.vendor_id  
                                      LEFT JOIN product_attributes pa ON product.product_id = pa.product_id  AND ISNULL(pa.sku)=0 AND pa.sku <> '' 
                                      LEFT JOIN attribute_values av ON pa.value_id=av.value_id
                                      WHERE product.product_id = vendor_products.product_id 
                                      AND product.vendor_id=" + Vendor_id + " and product.product_id=" + product_id;
                querytoRun += @" GROUP BY 
                 product.product_id ,product.product_type_id,product.vendor_id  ,product.product_availability   ,
                 product.published_at           ,product.product_vendor_image   ,product.product_name           ,
                 product.product_url_handle     ,product.product_description    ,product.vendor_color_name      ,
                 product.sku_family             ,product.size_and_fit_id        ,product.wash_type_id           ,
                 product.product_discount       ,product.product_cost           ,product.product_retail         ,
                 product.published_status       ,product.is_on_site_status      ,product.created_by             ,
                 product.updated_by             ,product.updated_at             ,product.created_at             ,
                 vendor_products.vendor_color_code,vendor_products.vendor_product_code";

                toReturn = await conn.QueryAsync<Product>(querytoRun);
            }


            return toReturn.ToList();
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create new product attributes into databaset
        Input: product attributes data
        output: string of product attributes
        */
        public async Task<string> CreateProductAttribute(ProductAttributes NewProductAttributes)
        {

            string ProductAttributesExistsQuery = "SELECT * FROM product_attributes WHERE sku='" + NewProductAttributes.sku + "' and  attribute_id='" + NewProductAttributes.attribute_id + "' and product_id='" + NewProductAttributes.product_id + "';";
            using (IDbConnection conn = Connection)
            {
                var ProductAttributesExist = await conn.QueryAsync<ProductAttributes>(ProductAttributesExistsQuery);

                if (ProductAttributesExist == null || ProductAttributesExist.Count() == 0)
                {
                    var result = await conn.InsertAsync<ProductAttributes>(NewProductAttributes);
                    return result.ToString();
                }
                else
                {
                    return ProductAttributesExist.First().product_attribute_id.ToString();
                }

            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Product pair with from database
        Input: string id 
        output: list of Product pair with
        */
        public async Task<IEnumerable<Productpairwith>> GetProductpairwiths(string id)
        {
            IEnumerable<Productpairwith> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<Productpairwith>("select product_pair_with.pairing_product_id,product_pair_with.paired_product_id, product.product_name from product,product_pair_with    where product.product_id = product_pair_with.paired_product_id  AND pairing_product_id= '" + id + "'");
            }
            return productProperties;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Product color with from database
        Input: string id 
        output: list of Product color with
        */
        public async Task<IEnumerable<Productcolorwith>> GetProductcolorwiths(string id)
        {
            IEnumerable<Productcolorwith> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<Productcolorwith>(" SELECT product_color_with.same_color_product_id, product_color_with.product_id, product.product_name  FROM product, product_color_with  WHERE product.product_id = product_color_with.same_color_product_id AND product_color_with.product_id  = '" + id + "'");
            }
            return productProperties;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Product image from database
        Input: string id 
        output: list of Product image
        */
        public async Task<IEnumerable<ProductImages>> GetProductImages(string id)
        {
            IEnumerable<ProductImages> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<ProductImages>("select * from product_images where product_id= '" + id + "'");
            }
            return productProperties;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Product properties from database
        Input: string id 
        output: list of Product properties
        */
        public async Task<IEnumerable<ProductProperties>> GetProductProperties(string id)
        {
            IEnumerable<ProductProperties> productProperties;
            IEnumerable<ProductProperties> productProperties2;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<ProductProperties>("SELECT A.attribute_id,A.sku,A.product_id,C.attribute_type_id,C.attribute,C.attribute_display_name,D.value FROM product_attributes AS A  , attributes AS C ,attribute_values AS D,product_attribute_values AS E WHERE (A.product_id = " + id + " AND A.attribute_id= C.attribute_id AND E.attribute_id =  A.attribute_id AND D.value_id= E.value_id AND E.product_id=" + id + ")  ");
                productProperties2 = await conn.QueryAsync<ProductProperties>("SELECT A.attribute_id,A.sku,A.product_id,B.attribute_type_id,B.attribute,B.attribute_display_name,NULL AS 'value' FROM product_attributes AS A, attributes AS B WHERE A.product_id = " + id + " AND A.attribute_id = B.attribute_id AND B.attribute_type_id =4");

            }
            return productProperties.Concat(productProperties2);

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Product details from database
        Input: string id 
        output: list of Product details
        */
        public async Task<IEnumerable<ProductDetails>> GetProductDetails(string id)
        {
            IEnumerable<ProductDetails> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<ProductDetails>("select * from product_page_details where product_id= '" + id + "'");

            }
            return productProperties;

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Productshoot status from database
        Input: string id 
        output: list of Productshoot status
        */
        public async Task<Int32> GetProductShootStatus(string id)
        {
            Int32 shootstatus = 0;
            using (IDbConnection conn = Connection)
            {
                try
                {
                    shootstatus = conn.QueryAsync<Int32>("select product_shoot_status_id from product_photoshoots where product_id= " + id).Result.First();
                }
                catch (Exception ex)
                {
                    return shootstatus;
                }
            }
            return shootstatus;

        }


        /*
        Developer: Mohi
        Date: 8-20-19 
        Action: Get Product photoshoot model from database
        Input: string id 
        output: Product photoshoot model ID
        */
        public async Task<Int32> GetProductPhotoshootModel(string id)
        {
            Int32 photoshootModel = 0;
            using (IDbConnection conn = Connection)
            {
                try
                {
                    photoshootModel = conn.QueryAsync<Int32>("SELECT photoshoots.model_id FROM `photoshoots`, `product_photoshoots` WHERE  `product_photoshoots`.photoshoot_id = `photoshoots`.photoshoot_id    AND  `product_photoshoots`.`product_id` =  " + id).Result.First();
                }
                catch (Exception ex)
                {
                    return photoshootModel;
                }
            }
            return photoshootModel;

        }


        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all Product colors from database
        Input: 
        output: list of Product colors
        */
        public async Task<IEnumerable<AllColors>> GetAllProductColors()
        {
            IEnumerable<AllColors> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<AllColors>("select distinct(value_id),value from attribute_values where attribute_id= 1");

            }
            return productProperties;

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all Product tags from database
        Input: 
        output: list of Product tags
        */
        public async Task<IEnumerable<AllTags>> GetAllProductTags()
        {
            IEnumerable<AllTags> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<AllTags>("select attribute_id,attribute,attribute_display_name,sub_heading from attributes where attribute_type_id=4 order by sub_heading");

            }
            return productProperties;

        }

        /*
        Developer: Mohi
        Date: 8-8-19 
        Action: Get all Photoshoot models from database
        Input: 
        output: list of photoshoot models
        */
        public async Task<List<PhotoshootModels>> GetPhotoshootModels()
        {
            IEnumerable<PhotoshootModels> productPhotoshootModels;
            using (IDbConnection conn = Connection)
            {
                productPhotoshootModels = await conn.QueryAsync<PhotoshootModels>("select * from photoshoot_models where active_status = 1 ");

            }
            return productPhotoshootModels.ToList();
        }


        /*
        Developer: Hamza
        Date: 16-9-19 
        Action: Get categories rm database
        Input: 
        output: list of photoshoot models
        */
        public async Task<List<ProductCategories>> GetProductCategories(string productID)
        {
            IEnumerable<ProductCategories> _productCategories;
            using (IDbConnection conn = Connection)
            {
                _productCategories = await conn.QueryAsync<ProductCategories>("SELECT pc.product_category_id ,pc.category_id FROM product_categories pc WHERE pc.product_id =" + productID);

            }
            return _productCategories.ToList();
        }
        /*Developer: ubaid
        Date:5-7-19
        Action:get AttributeValues Model from controller and insert the AttributeValues
        Input: AttributeValues Model 
        output: New AttributeValues id
        */
        public async Task<string> CreateAttributeValues(ProductAttributeValues NewProductAttributeValues)
        {
            string productAttirubteExistsQuery = "SELECT * FROM product_attribute_values WHERE product_id='" + NewProductAttributeValues.product_id + "' and attribute_id='" + NewProductAttributeValues.attribute_id + "';";
            using (IDbConnection conn = Connection)
            {


                var productAttributeExists = await conn.QueryAsync<ProductAttributeValues>(productAttirubteExistsQuery);

                if (productAttributeExists == null || productAttributeExists.Count() == 0)
                {
                    var result = await conn.InsertAsync<ProductAttributeValues>(NewProductAttributeValues);
                    return result.ToString();
                }
                else
                {
                    return productAttributeExists.First().product_attribute_value_id.ToString();
                }


            }

        }


        /*Developer: ubaid
        Date:5-7-19
        Action:get PurchaseOrderLineItems Model from controller and insert the PurchaseOrderLineItems
        Input: PurchaseOrderLineItems Model 
        output: New PO LineItem id
        */
        public async Task<string> CreatePOLineitems(PurchaseOrderLineItems NewLineitem)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<PurchaseOrderLineItems>(NewLineitem);
                return result.ToString();
            }

        }
        /*Developer: Hamza Haq
        Date:31-8-19
        Action:get PurchaseOrderLineItems Model from controller and Update the PurchaseOrderLineItems
        Input: PurchaseOrderLineItems Model 
        output: old PO LineItem id
        */
        public async Task<string> UpdatePoLineItems(PurchaseOrderLineItems NewLineitem)
        {
            using (IDbConnection conn = Connection)
            {
                String updateQuery = "";
                if ( NewLineitem.line_item_id ==0)
                {
                    updateQuery = "update purchase_order_line_items set line_item_ordered_quantity = " + NewLineitem.line_item_ordered_quantity + " , updated_by = "+NewLineitem.updated_by+ " , updated_at = " + NewLineitem.updated_at + " where vendor_id='" + NewLineitem.vendor_id + "' and  sku='" + NewLineitem.sku + "' and  po_id = " + NewLineitem.po_id;

                }
                else
                {
                   updateQuery = "update purchase_order_line_items set line_item_ordered_quantity = " + NewLineitem.line_item_ordered_quantity + " , updated_by = " + NewLineitem.updated_by + " , updated_at = " + NewLineitem.updated_at + " where line_item_id=" + NewLineitem.line_item_id + "";

                }
                var result = await conn.QueryAsync(updateQuery);
                return result.ToString();
            }

        }

        /*Developer: ubaid
       Date:5-7-19
       Action:get ProductAttributes values from controller and update the spesific Row
       Input: ProductAttributes values and where clause 
       output: None
       */
        public async Task AttributeUpdateSpecific(Dictionary<String, String> ValuePairs, String where)
        {
            QueryHelper qHellper = new QueryHelper();
            string UpdateQuery = qHellper.MakeUpdateQuery(ValuePairs, TableProductAttributes, where);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync(UpdateQuery);

            }

        }
        /*Developer: ubaid
       Date:13-7-19
       Action:get ProductUsedIn Model from controller and insert the ProductUsedIn
       Input: ProductUsedIn Model 
       output: New product UsedIn id
       */
        public async Task<string> CreateProductUsedIn(ProductUsedIn NewUsedIn)
        {


            string ProductUsedInExistsQuery = "SELECT * FROM product_used_in WHERE po_id='" + NewUsedIn.po_id + "' and product_id='" + NewUsedIn.product_id + "';";
            using (IDbConnection conn = Connection)
            {
                var ProductUsedInExists = await conn.QueryAsync<ProductUsedIn>(ProductUsedInExistsQuery);

                if (ProductUsedInExists == null || ProductUsedInExists.Count() == 0)
                {
                    var result = await conn.InsertAsync<ProductUsedIn>(NewUsedIn);
                    return result.ToString();
                }
                else
                {
                    return ProductUsedInExists.First().product_used_in_id.ToString();
                }

            }


        }
        /*
        Developer: Azeem hassan
        Date: 7-28-19 
        Action: insert product image data to db
        Input: image data
        output: insertion id
        */
        public async Task<string> CreateProductImages(Productimages NewProductImages)
        {
            using (IDbConnection conn = Connection)
            {
                long result = conn.Insert<Productimages>(NewProductImages);
                return result.ToString();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 08-09-19 
        Action: get product data by product name to db
        Input: image data
        output: insertion id
        */
        public async Task<Object> GetProduct(string product_name)
        {
            dynamic productDetails = new ExpandoObject();
            string sQuery = "SELECT product_id as value, product_name as label, product_vendor_image as image,'product' as type FROM " + TableName + " WHERE product_name LIKE '%" + product_name + "%';";
            using (IDbConnection conn = Connection)
            {
                productDetails = await conn.QueryAsync<object>(sQuery);

            }
            return productDetails;
        }

        /*
        Developer: Mohi
        Date: 08-21-19 
        Action: get product with vendor for pair with search
        Input: Prodcut Id
        output: List of products with vendor
        */
        public async Task<object> GetProductsPairWithSearch(string ProdcutId)
        {
            dynamic productDetails = new ExpandoObject();
            string sQuery = "SELECT product.sku_family as value, product.product_id, product.product_name as title, vendor.`vendor_name` as vendor FROM `product`, vendor WHERE `product`.`vendor_id` = vendor.`vendor_id` and `product`.`product_id` <> " + ProdcutId + " GROUP BY(product.`sku_family`)";
            using (IDbConnection conn = Connection)
            {
                productDetails = await conn.QueryAsync<object>(sQuery);
            }
            return productDetails;
        }

        /*
        Developer: Sajid Khan
        Date: 08-09-19 
        Action: update product image as primary in database
        Input: int product_image_id, int is_primary
        output: boolean
        */
        public async Task<bool> UpadateImagePrimary(int product_image_id, int is_primary)
        {
            Boolean res = false;
            try
            {
                using (IDbConnection conn = Connection)
                {
                    String updateQuery = "update product_images set isprimary = " + is_primary + " where product_image_id = " + product_image_id;
                    var updateResult = await conn.QueryAsync<object>(updateQuery);
                    res = true;
                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        /*
        Developer: Sajid Khan
        Date: 24-08-19 
        Action: get product ids by poid from db
        Input: string poids
        output: dynamic list of object product
        */
        public async Task<Object> GetProductIdsByPurchaseOrder()
        {
            dynamic productDetails = new ExpandoObject();
            string sQuery = "SELECT purchase_orders.po_id, product_used_in.product_id  FROM purchase_orders, product_used_in WHERE purchase_orders.po_status != 2 AND purchase_orders.po_status != 4 AND purchase_orders.po_id = product_used_in.po_id order by ra_flag DESC, FIELD(a.po_status, 3, 6, 5) asc";
            using (IDbConnection conn = Connection)
            {
                productDetails = await conn.QueryAsync<object>(sQuery);

            }
            return productDetails;
        }

        /*
        Developer: Sajid Khan
        Date: 24-08-19 
        Action: get published product ids by product ids from db
        Input: string poids
        output: dynamic list of object published product ids
        */
        public async Task<Object> GetPublishedProductIds(string poids)
        {
            dynamic productDetails = new ExpandoObject();
            string sQuery = "SELECT product_id FROM product WHERE published_status= 1 AND product_id IN (" + poids + ")";
            using (IDbConnection conn = Connection)
            {
                productDetails = await conn.QueryAsync<object>(sQuery);

            }
            return productDetails;
        }

        /*
        Developer: Rizwan Ali
        Date: 08-09-19 
        Action: delete product's al traces in database
        Input: int product_id
        output: boolean
      */
        public async Task<bool> DeleteProduct(string product_id, string po_id)
        {
            bool res = false;
            try
            {
                using (IDbConnection conn = Connection)
                {
                    String DeleteQuery = "delete FROM purchase_order_line_items WHERE product_id= " + product_id + " AND po_id = " + po_id;
                    var updateResult = await conn.QueryAsync<object>(DeleteQuery);

                    DeleteQuery = "delete FROM product_used_in WHERE product_id= " + product_id + " AND po_id = " + po_id;
                    updateResult = await conn.QueryAsync<object>(DeleteQuery);
                    res = true;
                }
            }
            catch (Exception ex)
            {
                res = false;
            }
            return res;
        }

        /*
       Developer: Mohi
       Date: 8-16-19 
       Action: check product details if itis already added then it update and otherwise insert it
       Input: product id, product detail
       output: string 
       */

        public async Task<string> AddEditProductPageDetails(string product_id, string product_detail_type, string value, string userid, string dateTime)
        {
            dynamic productDetails = new ExpandoObject();
            string sQuery = "";
            string toReturn = "success";
            sQuery = "SELECT * FROM product_page_details where product_id = " + product_id + " And product_detail_type =  " + product_detail_type;

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> productPageDetail = await conn.QueryAsync<object>(sQuery);
                int checkData = productPageDetail.Count();
                if (checkData > 0)
                {
                    String InsertQuery = "update product_page_details set `product_detail_value` = \"" + value + "\", `updated_by` = " + userid + ", updated_at = " + dateTime + "  where `product_id` = " + product_id + " and `product_detail_type`  = " + product_detail_type;
                    await conn.QueryAsync<object>(InsertQuery);
                }
                else
                {
                    String InsertQuery = "insert into product_page_details (`product_id`, `product_detail_type`, `product_detail_value`, `created_by`, `created_at`) " +
                        "values (" + product_id + "," + product_detail_type + ", \"" + value + "\", " + userid + " , " + dateTime + ")";
                    var result = await conn.QueryAsync<object>(InsertQuery);
                }

            }
            return toReturn;
        }

        /*
       Developer: Mohi
       Date: 8-16-19 
       Action: check product details if it is already added then leave it and otherwise insert it
       Input: product id, pair product id 
       output: string 
       */

        public async Task<string> AddPairWithProduct(string product_id, string pair_product_id, string userid, string dateTime)
        {
            dynamic productDetails = new ExpandoObject();
            string sQuery = "";
            string toReturn = "success";
            sQuery = "SELECT * FROM product_pair_with where pairing_product_id = " + product_id + " And paired_product_id =  " + pair_product_id;

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> productPairWith = await conn.QueryAsync<object>(sQuery);
                int checkData = productPairWith.Count();
                if (checkData > 0)
                {
                }
                else
                {
                    String InsertQuery = "insert into product_pair_with (`pairing_product_id`, `paired_product_id`, `created_by`, `created_at`) " +
                        "values (" + product_id + "," + pair_product_id + ", " + userid + " , " + dateTime + ")";
                    var result = await conn.QueryAsync<object>(InsertQuery);
                }

            }
            return toReturn;
        }


        /*
       Developer: Mohi
       Date: 8-16-19 
       Action: check product details if it is already added then leave it and otherwise insert it
       Input: product id, pair product id 
       output: string 
       */

        public async Task<string> AddOtherColorProducts(string product_id, string same_color_product_id, string userid, string dateTime)
        {
            dynamic productDetails = new ExpandoObject();
            string sQuery = "";
            string toReturn = "success";
            sQuery = "SELECT * FROM product_color_with where same_color_product_id = " + same_color_product_id + " And product_id =  " + product_id;

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> productPairWith = await conn.QueryAsync<object>(sQuery);
                int checkData = productPairWith.Count();
                if (checkData > 0)
                {

                }
                else
                {
                    String InsertQuery = "insert into product_color_with (`product_id`, `same_color_product_id`, `created_by`, `created_at`) " +
                        "values (" + product_id + "," + same_color_product_id + ", " + userid + " , " + dateTime + ")";
                    var result = await conn.QueryAsync<object>(InsertQuery);
                }

            }
            return toReturn;
        }


        /*
        Developer: Mohi
        Date: 8-16-19 
        Action: remove pair with product
        Input: product id, pair product id 
        output: string 
        */
        public async Task<string> RemovePairWithProduct(string product_id, string pair_product_id)
        {
            string toReturn = "success";
            string sQuery = "Delete FROM product_pair_with where pairing_product_id = " + product_id + " And paired_product_id =  " + pair_product_id;

            using (IDbConnection conn = Connection)
            {
                await conn.QueryAsync<object>(sQuery);
            }
            return toReturn;
        }

        public async Task<string> RemoveOtherColorProducts(string product_id, string same_color_product_id)
        {
            dynamic productDetails = new ExpandoObject();
            string sQuery = "";
            string toReturn = "success";
            sQuery = "Delete FROM product_color_with where same_color_product_id = " + same_color_product_id + " And product_id =  " + product_id;
            using (IDbConnection conn = Connection)
            {
                await conn.QueryAsync<object>(sQuery);
            }
            return toReturn;
        }

        /*
        Developer: Hamza Haq
        Date: 9-3-19 
        Action: Get All products by vendor and product name
        Input: vendor id and product name
        output: list of product list
        */
        public async Task<Object> GetProductsbyVendorAutoSuggest(int vendor_id, string productName)
        {
            dynamic vendorDetails = new ExpandoObject();
            string sQuery = "SELECT product_id as value, CONCAT(product_name, ' (', sku_family, ')')   as label FROM product WHERE vendor_id=" + vendor_id + " and product_name LIKE '%" + productName + "%';";
            using (IDbConnection conn = Connection)
            {
                vendorDetails = await conn.QueryAsync<object>(sQuery);

            }
            return vendorDetails;
        }


    }
}


