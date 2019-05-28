﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using notesService.Models;

namespace notesService.Interfaces
{
    public interface INotesRepository
    {
        Task<Notes> GetByID(int id);
        Task<List<Notes>> GetAllByReff(int reff,int type,int Limit);
        Task<String> Create(Notes NewNote);

    }
    public class NotesRepo : INotesRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "notes";
        private string selectlimit = "30";
        public NotesRepo(IConfiguration config)
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
        public async Task<string> Create(Notes NewNote)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Notes>(NewNote);
                return result.ToString();
            }
        }

        public async Task<List<Notes>> GetAllByReff(int reff,int note_type, int Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Notes> result = new List<Notes>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<Notes>("Select * from " + TableName + " where (ref_id=" + reff.ToString() + " and note_type_id = "+ note_type.ToString()+" ) order by note_id DESC Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.QueryAsync<Notes>("Select * from " + TableName + " where (ref_id=" + reff.ToString() + " and note_type_id = " + note_type.ToString() + " ) order by note_id DESC Limit " + selectlimit + ";");
                }
                return result.ToList();
            }
        }

        public async Task<Notes> GetByID(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<Notes>(id);
                return result;
            }
        }
    }
}