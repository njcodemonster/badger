using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace badgerApi.Helper
{
    [Table("notes")]
    public partial class Notes
    {
        [Key]
        public int note_id { get; set; }
        public int ref_id { get; set; }
        public int note_type_id { get; set; }
        public string note { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }

    [Table("documents")]
    public class Documents
    {
        [Key]
        public int doc_id { get; set; }
        public int ref_id { get; set; }
        public int doc_type_id { get; set; }
        public string url { get; set; }
        public string notes { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
    public interface INotesAndDocHelper
    {
        Task<List<Notes>> GenericNote<T>(int Reff ,int note_type, int Limit);
        Task<String> GenericPostNote<T>(int reffId, int noteType,string note, int createdBy, Double createdAt);
        Task<String> GenericPostDoc<T>(int reffId,int docType,string URL ,string notes  ,int createdBy, Double createdAt);
        Task<List<Documents>> GenericGetDocAsync<T>(int Reff ,int doc_type, int Limit);
        Task<List<Notes>> GenericNotes<T>(string Reffs, int note_type);
    }
    public class NotesAndDocHelper:INotesAndDocHelper
    {
        private String NOtesApiUrl = "";
        private readonly IConfiguration _config;
        public NotesAndDocHelper(IConfiguration config)
        {

            _config = config;
            NOtesApiUrl = _config.GetValue<string>("Services:NotesAndDoc");

        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get note json data  
        URL: 
        Request: Get
        Input:  string url
        output: json data of note
        */
        private async Task<T> GenericGetAsync<T>(String _call)
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.GetAsync(NOtesApiUrl + _call, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(data, settings);

        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Create new note json data  
        URL: 
        Request: Post
        Input:  type json, string url
        output: json data of note
        */
        private async Task<T> GenericPostAsync<T>(T json, String _call)
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.PostAsJsonAsync(NOtesApiUrl + _call, json);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(data, settings);

        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get notes by multiple ids with comma seperate and note type
        URL: 
        Request: Get
        Input:  string ids, int note type
        output: list of notes
        */
        public async Task<List<Notes>> GenericNotes<T>(string Reffs, int note_type)
        {
            List<Notes> notes = new List<Notes>();
            notes = await GenericGetAsync<List<Notes>>("/notes/reff/" + Reffs + "/" + note_type.ToString());
            return notes;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get note by id,note type and limit
        URL: 
        Request: Get
        Input:  int id, int note type,int limit
        output: list of notes
        */
        public async Task<List<Notes>> GenericNote<T>(int Reff,int note_type, int Limit)
        {
            List<Notes> notes = new List<Notes>();
            notes = await GenericGetAsync<List<Notes>>("/notes/reff/" + Reff.ToString() + "/"+note_type.ToString()+"/"+Limit.ToString());
            return notes;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: note data send
        URL: 
        Request: Post
        Input:  int id, int note type, string note, int createdby, double createdat
        output: string of note id
        */
        public async Task<string> GenericPostNote<T>(int reffId, int noteType, string note, int createdBy,Double createdAt)
        {
            JObject newNote = new JObject();
            newNote.Add("ref_id", reffId);
            newNote.Add("note_type_id", noteType);
            newNote.Add("note", note);
            newNote.Add("created_by", createdBy);
            newNote.Add("created_at", createdAt);
            return await GenericPostAsync<string>(newNote.ToString(Formatting.None), "/notes/create");
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: document data send
        URL: 
        Request: Post
        Input:  int id, int document type, string document url,string notes, int createdby, double createdat
        output: string of document id
        */
        public async Task<string> GenericPostDoc<T>(int reffId, int docType, string URL, string notes, int createdBy, Double createdAt)
        {
            JObject newDoc = new JObject();
            newDoc.Add("ref_id", reffId);
            newDoc.Add("doc_type_id", docType);
            newDoc.Add("url", URL);
            newDoc.Add("note", notes);
            newDoc.Add("created_by", createdBy);
            newDoc.Add("created_at", createdAt);
            return await GenericPostAsync<string>(newDoc.ToString(Formatting.None), "/documents/create");
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get documents by id
        URL: 
        Request: Get
        Input:  int id, int doc type, int limit
        output: list of documents
        */
        public async Task<List<Documents>> GenericGetDocAsync<T>(int Reff, int doc_type,int Limit)
        {
            List<Documents> Docs = new List<Documents>();
            Docs = await GenericGetAsync<List<Documents>>("/documents/reff/" + Reff.ToString() + "/" + doc_type.ToString() + "/" + Limit.ToString());
            return Docs;
        }
    }
}
