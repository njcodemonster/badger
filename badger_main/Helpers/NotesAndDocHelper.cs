﻿using Dapper.Contrib.Extensions;
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
        public Double created_at { get; set; }
        public Double updated_at { get; set; }
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
    }
    public class NotesAndDocHelper:INotesAndDocHelper
    {
        private String BadgerAPIURL = "";
        private readonly IConfiguration _config;
        public NotesAndDocHelper(IConfiguration config)
        {

            _config = config;
            BadgerAPIURL = _config.GetValue<string>("Services:NotesAndDoc");

        }
        private async Task<T> GenericGetAsync<T>(String _call)
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.GetAsync(BadgerAPIURL + _call, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(data, settings);

        }
        private async Task<T> GenericPostAsync<T>(T json, String _call)
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.PostAsJsonAsync(BadgerAPIURL + _call, json);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(data, settings);

        }

       
        public async Task<List<Notes>> GenericNote<T>(int Reff,int note_type, int Limit)
        {
            List<Notes> notes = new List<Notes>();
            notes = await GenericGetAsync<List<Notes>>("/notes/reff/" + Reff.ToString() + "/"+note_type.ToString()+"/"+Limit.ToString());
            return notes;
        }

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

        public async Task<string> GenericPostDoc<T>(int reffId, int docType, string URL, string notes, int createdBy, Double createdAt)
        {
            JObject newDoc = new JObject();
            newDoc.Add("ref_id", reffId);
            newDoc.Add("doc_type_id", docType);
            newDoc.Add("url", URL);
            newDoc.Add("note", notes);
            newDoc.Add("created_by", createdBy);
            newDoc.Add("created_at", createdAt);
            return await GenericPostAsync<string>(newDoc.ToString(Formatting.None), "/notes/create");
        }

        public async Task<List<Documents>> GenericGetDocAsync<T>(int Reff, int doc_type,int Limit)
        {
            List<Documents> Docs = new List<Documents>();
            Docs = await GenericGetAsync<List<Documents>>("/documents/reff/" + Reff.ToString() + "/" + doc_type.ToString() + "/" + Limit.ToString());
            return Docs;
        }
    }
}