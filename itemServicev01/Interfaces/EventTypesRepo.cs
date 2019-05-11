using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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
                string sQuery = "SELECT event_type_id as 'EventTypeId' ,event_type_name as 'EventTypeName',event_type_description as 'EventTypeDescription' ,created_at as 'CreatedAt' from event_types";
                conn.Open();
                var result =  conn.Query<EventTypes>(sQuery);
                return result.ToList();
            }
        }

       

        public Task<EventTypes> GetByID(int id)
        {
            throw new NotImplementedException();
        }
    }

}
