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
    public interface IPhotoshootsRepository
    {
        Task<Photoshoots> GetById(int id);
        Task<List<Photoshoots>> GetAll(Int32 Limit);
        Task<string> Count();
        Task<String> Create(Photoshoots NewPhotoshoot);
        Task<Boolean> Update(Photoshoots PhotoshootToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<Object> GetPhotoshootsDetailsRep(Int32 id);
    }
    public class PhotoshootsRepo : IPhotoshootsRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "product_photoshoots";
        private string TableProducts = "product";
        private string TableAttributes = "product_attributes";
        

        private string selectlimit = "30";
        public PhotoshootsRepo(IConfiguration config)
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
                    result = await conn.QueryAsync<Photoshoots>("Select * from "+TableName+ " where photoshoot_id = 0 Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<Photoshoots>();
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

        public async Task<Object> GetPhotoshootsDetailsRep(Int32 Limit)
        {
            dynamic photoshootsDetails = new ExpandoObject();
            string sQuery = "";
            if (Limit > 0)
            {
                 sQuery = "SELECT a.* , b.product_shoot_status_id , c.* FROM  photoshoots a , product_photoshoots b  , product c where a.photoshoot_id =  b.photoshoot_id and b.product_id=c.product_id  and b.photoshoot_id =0 Limit " + Limit.ToString() + " ;)";
            }
            else
            {
                 sQuery = "SELECT a.* , b.product_shoot_status_id , c.* FROM  photoshoots a , product_photoshoots b  , product c where a.photoshoot_id =  b.photoshoot_id and b.product_id=c.product_id  and b.photoshoot_id =0 ;)";
            }

            using (IDbConnection conn = Connection)
            {
                photoshootsDetails = await conn.QueryAsync<object>(sQuery);

            }
            return photoshootsDetails;
        }
    }
}

