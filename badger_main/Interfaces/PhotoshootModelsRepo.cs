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
    public interface IPhotoshootModelRepository
    {
        Task<PhotoshootModels> GetById(int id);
        Task<List<PhotoshootModels>> GetAll(Int32 Limit);
        Task<String> Create(PhotoshootModels NewModel);
        Task<Boolean> Update(PhotoshootModels ModelToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where); 
    }
    public class PhotoshootModelRepo : IPhotoshootModelRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "photoshoot_models";
        private string selectlimit = "30";
        public PhotoshootModelRepo(IConfiguration config)
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
        Date: 7-5-19 
        Action: Insert new PhotoshootModel data into database
        URL: 
        Request: Post
        Input: PhotoshootModel data
        output: string of PhotoshootModel data
        */
        public async Task<string> Create(PhotoshootModels NewModel)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<PhotoshootModels>(NewModel);
                return result.ToString() ;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all PhotoshootModel data from database
        URL: 
        Request: Get
        Input: int limit
        output: list of PhotoshootModel data
        */
        public async Task<List<PhotoshootModels>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<PhotoshootModels> result = new List<PhotoshootModels>();
                if(Limit > 0)
                {
                    result = await conn.QueryAsync<PhotoshootModels>("Select * from "+TableName+" Limit "+ Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<PhotoshootModels>();
                }
                return result.ToList();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get PhotoshootModel data by id from database
        URL: 
        Request: Get
        Input: int id
        output: list of PhotoshootModel data
        */
        public async Task<PhotoshootModels> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                
                var result = await conn.GetAsync<PhotoshootModels>(id);
                return result;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Update PhotoshootModel data by id from database
        URL: 
        Request: Put
        Input: PhotoshootModel data
        output: Boolean
        */
        public async Task<Boolean> Update(PhotoshootModels ModelToUpdate)
        {
            
            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<PhotoshootModels>(ModelToUpdate);
                return result;
            }
           
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Update PhotoshootModel data with specific fields by id from database
        URL: 
        Request: Put
        Input: Dictionary<String , String> ValuePairs, String where condition
        output: Boolean
        */
        public async Task UpdateSpecific(Dictionary<String , String> ValuePairs, String where)
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

