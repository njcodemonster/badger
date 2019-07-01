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
    public interface IPhotoshootRepository
    {
        Task<Photoshoots> GetById(int id);
        Task<List<Photoshoots>> GetAll(Int32 Limit);
        Task<string> Count();
        Task<String> Create(Photoshoots NewPhotoshoot);
        Task<Boolean> Update(Photoshoots PhotoshootToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<Object> GetPhotoshootDetailsRep(Int32 id);
        Task<Object> GetPhotoshootProducts(Int32 photoshootId);
        Task<Object> GetAllPhotoshootsAndModels();
        Task<Object> GetAllPhotoshootsModels(Int32 limit);
        Task<Object> GetInprogressPhotoshoot(Int32 limit);
        Task<Object> GetSentToEditorPhotoshoot(Int32 limit);

        Task<Object> GetSkuByProduct(string product_id);


    }
    public class PhotoshootRepo : IPhotoshootRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "product_photoshoots";
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

        public async Task<string> Create(Photoshoots NewPhotoshoot)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Photoshoots>(NewPhotoshoot);
                return result.ToString() ;
            }
        }
        public async Task<string> Count()
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync<String>("select count(photoshoot_id) from " + TableName+ " where photoshoot_id = 0;");
                return result.FirstOrDefault();
            }
        }
        public async Task<List<Photoshoots>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Photoshoots> result = new List<Photoshoots>();
                if(Limit > 0)
                {
                    result = await conn.QueryAsync<Photoshoots>("Select * from "+TableName+ " where photoshoot_id = 0 OR product_shoot_status_id = 0  Limit " + Limit.ToString() + ";");
                }
                else
                {
                    //result = await conn.GetAllAsync<Photoshoots>();
                    result = await conn.QueryAsync<Photoshoots>("Select * from " + TableName + " where photoshoot_id = 0 OR product_shoot_status_id = 0 ;");
                }
                return result.ToList();
            }
        }

       

        public async Task<Photoshoots> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                
                var result = await conn.GetAsync<Photoshoots>(id);
                return result;
            }
        }

        public async Task<Boolean> Update(Photoshoots PhotoshootsToUpdate)
        {
            
            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Photoshoots>(PhotoshootsToUpdate);
                return result;
            }
           
        }
        public async Task UpdateSpecific(Dictionary<String , String> ValuePairs, String where)
        {
            QueryHelper qHellper = new QueryHelper();
            string UpdateQuery = qHellper.MakeUpdateQuery(ValuePairs, TableName, where);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync(UpdateQuery);
               
            }

        }

        public async Task<Object> GetPhotoshootDetailsRep(Int32 Limit)
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";
            if (Limit > 0)
            {
                 sQuery = "  SELECT  ps.product_shoot_status_id , p.product_id, p.product_vendor_image, p.product_name, p.sku_family, v.`vendor_name` FROM product_photoshoots ps  , product p, vendor v WHERE  p.product_id = ps.product_id  AND (ps.photoshoot_id = 0 OR product_shoot_status_id = 0 )  AND p.`vendor_id` = v.`vendor_id` Limit " + Limit.ToString() + " ;";
            }
            else
            {
                 sQuery = "SELECT  ps.product_shoot_status_id , p.product_id, p.product_name, p.product_vendor_image, p.sku_family, v.`vendor_name` FROM product_photoshoots ps  , product p, vendor v WHERE  p.product_id = ps.product_id  AND (ps.photoshoot_id = 0 OR product_shoot_status_id = 0 ) AND p.`vendor_id` = v.`vendor_id`; ";
            }

            using (IDbConnection conn = Connection)
            {
               // photoshootsDetails = await conn.QueryAsync<object>(sQuery);
                IEnumerable<object> photoshootsInfo = await conn.QueryAsync<object>(sQuery);
                photoshootsDetails.photoshootsInfo = photoshootsInfo;

            }
            return photoshootsDetails;
        }

        public async Task<Object> GetPhotoshootProducts(Int32 photoshootId)
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";
            sQuery = "SELECT  ps.photoshoot_id, ps.product_shoot_status_id , p.product_id, p.product_name, p.product_vendor_image, p.sku_family, v.`vendor_name`, u.name AS username FROM users u ,product_photoshoots ps  , product p, vendor v, photoshoots sh WHERE  p.product_id = ps.product_id  AND ps.photoshoot_id = " + photoshootId + " AND p.`vendor_id` = v.`vendor_id` AND sh.created_by = u.user_id AND ps.product_shoot_status_id = 1 AND sh.photoshoot_id = ps.photoshoot_id; ";
            

            using (IDbConnection conn = Connection)
            {
                // photoshootsDetails = await conn.QueryAsync<object>(sQuery);
                IEnumerable<object> photoshootsInfo = await conn.QueryAsync<object>(sQuery);
                photoshootsDetails.photoshootsInfo = photoshootsInfo;

            }
            return photoshootsDetails;
        }


        public async Task<Object> GetAllPhotoshootsAndModels()
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";
            string sQuery2 = "";

            
                sQuery  = "SELECT  photoshoot_id, photoshoot_name FROM photoshoots ";
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

        public async Task<Object> GetSkuByProduct(string product_id)
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";
            sQuery = "Select * from `sku` where `product_id` IN ("+ product_id + ")";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> photoshootsModelList = await conn.QueryAsync<object>(sQuery);
                photoshootsDetails = photoshootsModelList;
            }
            return photoshootsDetails;
        }

        public async Task<Object> GetSentToEditorPhotoshoot(Int32 Limit)
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";
            if (Limit > 0)
            {
                sQuery = "SELECT  ps.photoshoot_id, ps.product_shoot_status_id , p.product_id, p.product_name, p.product_vendor_image, p.sku_family, v.`vendor_name` FROM product_photoshoots ps  , product p, vendor v WHERE  p.product_id = ps.product_id  AND  p.`vendor_id` = v.`vendor_id` AND ps.product_shoot_status_id = 2 Limit " + Limit.ToString(); 
            }
            else
            {
                sQuery = "SELECT  ps.photoshoot_id, ps.product_shoot_status_id , p.product_id, p.product_name, p.product_vendor_image, p.sku_family, v.`vendor_name` FROM product_photoshoots ps  , product p, vendor v WHERE  p.product_id = ps.product_id  AND  p.`vendor_id` = v.`vendor_id` AND ps.product_shoot_status_id = 2 ";
            }

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> photoshootsModelList = await conn.QueryAsync<object>(sQuery);
                photoshootsDetails.photoshootSendToEditor = photoshootsModelList;
            }
            return photoshootsDetails;
        }
    }
}

