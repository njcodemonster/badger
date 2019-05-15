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
        Task<EventTypes> GetByID(int id);
        Task<List<EventTypes>> GetAllAsync();
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
        public async Task<List<EventTypes>> GetAllAsync()
        {
            using (IDbConnection conn = Connection)
            {
               
                var result = await conn.GetAllAsync<EventTypes>();
                return result.ToList(); 
            }
        }

       

        public async Task<EventTypes> GetByID(int id)
        {
            using (IDbConnection conn = Connection)
            {
               
                var result = conn.Get<EventTypes>(id);
                return result;
            }
        }

        public async Task<List<ponkaquery>> GetPonka()
        {
            using (IDbConnection conn = Connection)
            {
               
                var result =  conn.Query<ponkaquery>("select event_type_id as id , event_type_name as name from event_types where(event_type_id=1);");
                return result.ToList<ponkaquery>();
            }
        }
    }

}
