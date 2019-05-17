using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using itemService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using CommonHelper;

namespace itemService.Interfaces
{
    public interface IItemStatusRepository
    {
        Task<List<ItemStatus>> GetAllStatus();
        Task<List<ItemStatus>> GetStatusByID(int id);
        
    }
    public class ItemStatusRepo : IItemStatusRepository
    {
        private readonly IConfiguration _config;

        public ItemStatusRepo(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("ItemsDatabase"));
            }
        }
        public async Task<List<ItemStatus>> GetStatusByID(int id)
        {
            try
            {
                List<ItemStatus> ToRetrun = new List<ItemStatus>();


                int itemId = Convert.ToInt32(id);
                using (IDbConnection conn = Connection)
                {
                    var result = conn.Get<ItemStatus>(itemId);
                    ToRetrun.Add(result);
                }

                return ToRetrun;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<ItemStatus>> GetAllStatus()
        {
            try
            {
                    List<ItemStatus> ToRetrun = new List<ItemStatus>();

                        using (IDbConnection conn = Connection)
                        {
                            var result = await conn.GetAllAsync<ItemStatus>();
                            return result.ToList();
                        }
                    
                  
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       

    }
}
