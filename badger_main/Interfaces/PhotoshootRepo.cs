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
using GenericModals;

namespace badgerApi.Interfaces
{
    public interface IPhotoshootRepository
    {
        Task<Photoshoots> GetById(int id);
        Task<List<Photoshoots>> GetAll(Int32 Limit);
        Task<string> Count();
        Task<String> Create(Photoshoots NewPhotoshoot);
        Task<String> CreatePhotoshootProduct(ProductPhotoshoots NewPhotoshoot);
        Task<Boolean> Update(Photoshoots PhotoshootToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task UpdatePhotoshootForSummary(Dictionary<String, String> ValuePairs, String where);
        Task<Object> GetPhotoshootDetailsRep(Int32 id);
        Task<Object> GetPhotoshootProducts(Int32 photoshootId);
        Task<Object> GetAllPhotoshootsAndModels();
        Task<Object> GetAllPhotoshootsModels(Int32 limit);
        Task<Object> GetInprogressPhotoshoot(Int32 limit);
        Task<Object> GetSentToEditorPhotoshoot(Int32 limit);
        Task<Object> GetPhotoshootSummary();

        Task<Object> GetSkuByProduct(string product_id);
        Task<SmallestItem> GetSmallestSizeItem(int po_id, int vendor_id, int product_id);
        Task<IEnumerable<SmallestItem>> GetSmallestSkuByProduct(List<int> productIds);

    }
    public class PhotoshootRepo : IPhotoshootRepository
    {
        private readonly IConfiguration _config;
        private string product_photoshoots = "product_photoshoots";
        private string TablePhotoshoots = "photoshoots";
        private string TableProducts = "product";
        private string TableAttributes = "product_attributes";


        private string selectlimit = "30";
        public PhotoshootRepo(IConfiguration config)
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
        Developer: Mohi
        Date: 7-3-19 
        Action: Create new photoshoot and return ID of that photoshoot 
        Input: FromBody
        output: photoshoot id
        */

        public async Task<string> Create(Photoshoots NewPhotoshoot)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Photoshoots>(NewPhotoshoot);
                return result.ToString();

            }
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Create photoshoot product in product_photoshoot
        Input: FromBody
        output: 
        */
        public async Task<string> CreatePhotoshootProduct(ProductPhotoshoots NewPhotoshoot)
        {

            string photoshootExistsQuery = "SELECT * FROM product_photoshoots WHERE product_id='" + NewPhotoshoot.product_id + "';";
            using (IDbConnection conn = Connection)
            {
                var PhotoshootExists = await conn.QueryAsync<ProductPhotoshoots>(photoshootExistsQuery);

                if (PhotoshootExists == null || PhotoshootExists.Count() == 0)
                {
                    var result = await conn.InsertAsync<ProductPhotoshoots>(NewPhotoshoot);
                    return result.ToString();
                }
                else
                {
                    return PhotoshootExists.First().photoshoot_id.ToString();
                }

            }


        }

        /*
        Developer: 
        Date: 
        Action: 
        Input: 
        output:
        */
        public async Task<string> Count()
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync<String>("select count(photoshoot_id) from " + product_photoshoots + " where photoshoot_id = 0;");
                return result.FirstOrDefault();
            }
        }

        /*
        Update Developer: Mohi
        Date: 7-3-19 
        Action: List of all not started photoshoot with the given limit 
        Input: FromBody
        output: photoshoot id
        */
        public async Task<List<Photoshoots>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Photoshoots> result = new List<Photoshoots>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<Photoshoots>("Select * from " + product_photoshoots + " where photoshoot_id = 0 OR product_shoot_status_id = 0  Limit " + Limit.ToString() + ";");
                }
                else
                {
                    //result = await conn.GetAllAsync<Photoshoots>();
                    result = await conn.QueryAsync<Photoshoots>("Select * from " + product_photoshoots + " where photoshoot_id = 0 OR product_shoot_status_id = 0 ;");
                }
                return result.ToList();
            }
        }


        /*
        Update Developer: Mohi
        Date: 7-3-19 
        Action: select photoshoot by ID and return photoshoot model
        Input: int PhotoshootID
        output: Photoshoot Model
        */
        public async Task<Photoshoots> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<Photoshoots>(id);
                return result;
            }
        }

        /*
        Update Developer: Mohi
        Date: 7-3-19 
        Action: update photoshoots all values using common functon UpdateAsync 
        Input: Photoshoots Model
        output: boolean return
        */
        public async Task<Boolean> Update(Photoshoots PhotoshootsToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Photoshoots>(PhotoshootsToUpdate);
                return result;
            }

        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: update photoshoots all values using common functon UpdateAsync 
        Input: Dictionary<String , String> ValuePairs, String where condition
        output: 
        */
        public async Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where)
        {
            QueryHelper qHellper = new QueryHelper();
            string UpdateQuery = qHellper.MakeUpdateQuery(ValuePairs, product_photoshoots, where);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync(UpdateQuery);

            }

        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: List of all photoshoot product with the product details which are not started
        Input: Limit int
        output: Object photoshoots
        */
        public async Task<Object> GetPhotoshootDetailsRep(Int32 Limit)
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";
            if (Limit > 0)
            {
                sQuery = "SELECT  ps.product_shoot_status_id, p.product_id, p.product_name, p.vendor_color_name AS color, p.product_vendor_image, p.sku_family, v.`vendor_name`, pui.`po_id`, pos.`po_status_name` as po_status, p.vendor_id FROM product_photoshoots ps, product p, vendor v, product_used_in pui, purchase_orders po, purchase_order_status pos WHERE p.product_id = ps.product_id AND (ps.photoshoot_id = 0 OR product_shoot_status_id = 0) AND p.`vendor_id`  = v.`vendor_id` AND po.`po_id` = pui.`po_id` AND p.`product_id` = pui.`product_id` AND po.`po_status` = pos.`po_status_id` AND pos.`po_status_id` <> 2  Limit " + Limit.ToString() + " ;";
            }
            else
            {
                sQuery = "SELECT  ps.product_shoot_status_id, p.product_id, p.product_name, p.vendor_color_name AS color, p.product_vendor_image, p.sku_family, v.`vendor_name`, pui.`po_id`, pos.`po_status_name` as po_status, p.vendor_id FROM product_photoshoots ps, product p, vendor v, product_used_in pui, purchase_orders po, purchase_order_status pos WHERE p.product_id = ps.product_id AND (ps.photoshoot_id = 0 OR product_shoot_status_id = 0) AND p.`vendor_id`  = v.`vendor_id` AND po.`po_id` = pui.`po_id` AND p.`product_id` = pui.`product_id` AND po.`po_status` = pos.`po_status_id` AND pos.`po_status_id` <> 2; ";
            }

            using (IDbConnection conn = Connection)
            {
                // photoshootsDetails = await conn.QueryAsync<object>(sQuery);
                IEnumerable<object> photoshootsInfo = await conn.QueryAsync<object>(sQuery);
                photoshootsDetails.photoshootsInfo = photoshootsInfo;

            }
            return photoshootsDetails;
        }

        /*
       Developer: Mohi
       Date: 7-3-19 
       Action: List of all photoshoot products by photoshoot ID with the product details which are in-progress
       Input: int PhotoshootId
       output: Object photoshoots
       */
        public async Task<Object> GetPhotoshootProducts(Int32 photoshootId)
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";
            sQuery = "SELECT  ps.photoshoot_id, ps.product_shoot_status_id ,p.vendor_color_name as color, p.product_id, p.product_name, p.product_vendor_image, p.sku_family, v.`vendor_name`, u.name AS username FROM users u ,product_photoshoots ps  , product p, vendor v, photoshoots sh WHERE  p.product_id = ps.product_id  AND ps.photoshoot_id = " + photoshootId + " AND p.`vendor_id` = v.`vendor_id` AND sh.created_by = u.user_id AND ps.product_shoot_status_id = 1 AND sh.photoshoot_id = ps.photoshoot_id; ";


            using (IDbConnection conn = Connection)
            {
                // photoshootsDetails = await conn.QueryAsync<object>(sQuery);
                IEnumerable<object> photoshootsInfo = await conn.QueryAsync<object>(sQuery);
                photoshootsDetails.photoshootsInfo = photoshootsInfo;

            }
            return photoshootsDetails;
        }

        /*
       Developer: Mohi
       Date: 7-3-19 
       Action: List of all photoshoots and models
       Input: int PhotoshootId
       output: Object photoshoots & models
       */
        public async Task<Object> GetAllPhotoshootsAndModels()
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";
            string sQuery2 = "";


            sQuery = " SELECT p.`photoshoot_id`, p.`photoshoot_name` FROM `photoshoots` p, `product_photoshoots` pp" +
                        " WHERE p.`photoshoot_id` = pp.`photoshoot_id` AND pp.product_shoot_status_id IN(1, 2) " +
                        " GROUP BY p.`photoshoot_id` ORDER BY p.`photoshoot_id`  ";

            sQuery2 = "SELECT  model_id, model_name FROM photoshoot_models ";


            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> photoshootsList = await conn.QueryAsync<object>(sQuery);
                photoshootsDetails.photoshootsList = photoshootsList;

                IEnumerable<object> photoshootsModelList = await conn.QueryAsync<object>(sQuery2);
                photoshootsDetails.photoshootsModelList = photoshootsModelList;

            }
            return photoshootsDetails;
        }

        /*
       Developer: Mohi
       Date: 7-3-19 
       Action: Returns list of photoshoot models according to the given limit
       Input: int limit
       output: Object photoshoot models
       */
        public async Task<Object> GetAllPhotoshootsModels(Int32 Limit)
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";
            if (Limit > 0)
            {
                sQuery = "SELECT model_id, model_name FROM photoshoot_models Limit " + Limit.ToString() + " ;";
            }
            else
            {
                sQuery = "SELECT model_id, model_name FROM photoshoot_models ";
            }

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> photoshootsModelList = await conn.QueryAsync<object>(sQuery);
                photoshootsDetails.photoshootsModelList = photoshootsModelList;
            }
            return photoshootsDetails;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Returns list of inprogress photoshoots according to the given limit
        Input: int limit
        output: Object photoshoot models
        */
        public async Task<Object> GetInprogressPhotoshoot(Int32 Limit)
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";
            if (Limit > 0)
            {
                sQuery = "SELECT photoshoots.photoshoot_name, `product_photoshoots`.`photoshoot_id` FROM photoshoots, `product_photoshoots` WHERE `product_photoshoots`.photoshoot_id = photoshoots.`photoshoot_id` AND  product_photoshoots.product_shoot_status_id = 1 GROUP BY photoshoots.`photoshoot_id` Limit " + Limit.ToString();
            }
            else
            {
                sQuery = "SELECT photoshoots.photoshoot_name, `product_photoshoots`.`photoshoot_id` FROM photoshoots, `product_photoshoots` WHERE `product_photoshoots`.photoshoot_id = photoshoots.`photoshoot_id` AND  product_photoshoots.product_shoot_status_id = 1 GROUP BY photoshoots.`photoshoot_id`";
            }

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> photoshootsModelList = await conn.QueryAsync<object>(sQuery);
                photoshootsDetails.photoshootsInprogress = photoshootsModelList;
            }
            return photoshootsDetails;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Returns the sku object of the given product id 
        Input: int productId
        output: ExpandoObject SkuDetails
        */
        public async Task<Object> GetSkuByProduct(string product_id)
        {
            dynamic SkuDetails = new ExpandoObject();
            string sQuery = "";
            sQuery = "Select * from `sku` where `product_id` IN (" + product_id + ")";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> skuResult = await conn.QueryAsync<object>(sQuery);
                SkuDetails = skuResult;
            }
            return SkuDetails;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Returns sent to editor photoshoot products according to the given limit
        Input: int Limit
        output: ExpandoObject Photoshoot
        */
        public async Task<Object> GetSentToEditorPhotoshoot(Int32 Limit)
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";
            if (Limit > 0)
            {
                sQuery = "SELECT  ps.product_shoot_status_id, p.product_id, p.product_name, p.vendor_color_name AS color, p.product_vendor_image, p.sku_family, v.`vendor_name`, pui.`po_id`, pos.`po_status_name` as po_status FROM product_photoshoots ps, product p, vendor v, product_used_in pui, purchase_orders po, purchase_order_status pos WHERE p.product_id = ps.product_id AND  ps.product_shoot_status_id = 2 AND p.`vendor_id`  = v.`vendor_id` AND po.`po_id` = pui.`po_id` AND p.`product_id` = pui.`product_id` AND po.`po_status` = pos.`po_status_id` AND pos.`po_status_id` <> 2 Limit " + Limit.ToString();
            }
            else
            {
                sQuery = "SELECT  ps.product_shoot_status_id, p.product_id, p.product_name, p.vendor_color_name AS color, p.product_vendor_image, p.sku_family, v.`vendor_name`, pui.`po_id`, pos.`po_status_name` as po_status FROM product_photoshoots ps, product p, vendor v, product_used_in pui, purchase_orders po, purchase_order_status pos WHERE p.product_id = ps.product_id AND  ps.product_shoot_status_id = 2 AND p.`vendor_id`  = v.`vendor_id` AND po.`po_id` = pui.`po_id` AND p.`product_id` = pui.`product_id` AND po.`po_status` = pos.`po_status_id` AND pos.`po_status_id` <> 2;  ";
            }

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> photoshootsModelList = await conn.QueryAsync<object>(sQuery);
                photoshootsDetails.photoshootSendToEditor = photoshootsModelList;
            }
            return photoshootsDetails;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Returns photoshoots which are in-progress
        Input: null
        output: ExpandoObject Photoshoots
        */

        public async Task<Object> GetPhotoshootSummary()
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";

            sQuery = "SELECT pp.`photoshoot_id`, COUNT(pp.`photoshoot_id`) AS styles, p.`shoot_start_date` AS scheduled_date, p.`model_id`, pp.product_shoot_status_id FROM `photoshoots` p, `product_photoshoots` pp WHERE p.`photoshoot_id` = pp.`photoshoot_id` AND pp.product_shoot_status_id IN (1) GROUP BY p.`photoshoot_id` ORDER BY p.`photoshoot_id` DESC";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> photoshootsModelList = await conn.QueryAsync<object>(sQuery);
                photoshootsDetails.productPhotoshootSummary = photoshootsModelList;
            }
            return photoshootsDetails;
        }

        /*
        Developer: Mohi
        Date: 7-16-19 
        Action: update photoshoots specific values using common function UpdateAsync 
        Input: Dictionary<String , String> ValuePairs, String where condition
        output: 
*/
        public async Task UpdatePhotoshootForSummary(Dictionary<String, String> ValuePairs, String where)
        {
            QueryHelper qHellper = new QueryHelper();
            string UpdateQuery = qHellper.MakeUpdateQuery(ValuePairs, TablePhotoshoots, where);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync(UpdateQuery);

            }

        }

        public async Task<SmallestItem> GetSmallestSizeItem(int po_id, int vendor_id, int product_id)
        {
            string query = @"SELECT poli.line_item_id, poli.po_id,poli.vendor_id,poli.sku,sku.sku_id,poli.product_id
                            FROM purchase_order_line_items poli
                            JOIN sku ON poli.sku = sku.sku" +
                           $" WHERE poli.po_id = {po_id} AND poli.vendor_id = {vendor_id} AND poli.product_id = {product_id} " +
                           @" ORDER BY poli.sku ASC
                            LIMIT 1";
            using (IDbConnection conn = Connection)
            {
                return await conn.QueryFirstOrDefaultAsync<SmallestItem>(query);
            }
        }

        public async Task<IEnumerable<SmallestItem>> GetSmallestSkuByProduct(List<int> productIds)
        {
            string query = $"SELECT MIN(sk1.sku) AS sku,sku_id,sk1.vendor_id,sk1.product_id FROM `productdb`.`sku` sk1 " +
                            $"WHERE sk1.product_id IN({string.Join(',', productIds)}) " +
                            "GROUP BY sk1.vendor_id,sk1.product_id ";
            using (IDbConnection conn = Connection)
            {
                return await conn.QueryAsync<SmallestItem>(query);
            }
        }
    }
}

