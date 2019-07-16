using System;
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
        Task<List<Notes>> GetAllNotesByReffs(string reffs, int note_type);
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: insert note to database
        Input: new note data
        output: string of note id
        */
        public async Task<string> Create(Notes NewNote)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Notes>(NewNote);
                return result.ToString();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: List of notes by refference id from database
        Input: int reff,int note_type, int Limit
        output: Dynamic List of Notes
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Multiple Lists of notes by refference ids with comma seperate from database
        Input: string reffs,int note_type
        output: Dynamic List of Notes
        */
        public async Task<List<Notes>> GetAllNotesByReffs(string reffs, int note_type)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Notes> result = new List<Notes>();
               
                result = await conn.QueryAsync<Notes>("SELECT * from " + TableName + " where note_id IN(SELECT MAX(note_id) FROM " + TableName + " where (ref_id IN (" + reffs.ToString() + ") and note_type_id = " + note_type.ToString() + " ) group by ref_id) order by note_id DESC ");
                
                return result.ToList();
            }
        }

        /*
       Developer: Sajid Khan
       Date: 7-13-19 
       Action: List of notes by id from database
       Input: int id
       output: Dynamic List of Notes
       */
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
