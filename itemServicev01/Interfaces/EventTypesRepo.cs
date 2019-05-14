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

namespace itemService.Interfaces
{
    public interface IItemTypeRepository
    {
        Task<event_type> GetByID(int id);
        Task<List<event_type>> GetAllAsync();

        Task<List<ponkaquery>> GetPonka();
        
    }
    public class ItemTypeRepo : IItemTypeRepository
    {
        private readonly IConfiguration _config;

        public ItemTypeRepo(IConfiguration config)
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
        public async Task<List<event_type>> GetAllAsync()
        {
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                var result = await conn.GetAllAsync<event_type>();
                return result.ToList(); 
            }
        }

       

        public async Task<event_type> GetByID(int id)
        {
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                var result = conn.Get<event_type>(id);
                return result;
            }
        }

        public async Task<List<ponkaquery>> GetPonka()
        {
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                var result =  conn.Query<ponkaquery>("select event_type_id as id , event_type_name as name from event_types where(event_type_id=1);");
                return result.ToList<ponkaquery>();
            }
        }
    }

}
