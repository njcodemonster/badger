using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.S3;
namespace badger_view.Helpers
{
    public class BadgerApiHelper
    {
       
        private String BadgerAPIURL = "";
        private readonly IConfiguration _config;
        public BadgerApiHelper(IConfiguration config)
        {

            _config = config;
            BadgerAPIURL = _config.GetValue<string>("Services:Badger");

        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: data sends to badger api
        Request: Get 
        Input: Any Type and URL
        output: json object 
        */
        public async Task<T> GenericGetAsync<T>(String _call)
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
            return JsonConvert.DeserializeObject<T>(data,settings);

        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: data sends to badger api
        Request: Post 
        Input: Json Type, URL
        output: json object 
        */
        public async Task<T> GenericPostAsync<T>(T json,String _call)
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

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: data sends to badger api
        Request: Post 
        Input: Json data, URL
        output: string data
        */
        public async Task<String> GenericPostAsyncString<T>(T json, String _call)
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.PostAsJsonAsync(BadgerAPIURL + _call, json);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return data.ToString();

        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: data sends to badger api
        Request: Put 
        Input: Json Type, URL
        output: string data
        */
        public async Task<String> GenericPutAsyncString<T>(T json, String _call)
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.PutAsJsonAsync(BadgerAPIURL + _call, json);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return data.ToString();

        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: string data convert to json
        Request: Get 
        Input: string data
        output: json data
        */
        public async Task<T> ForceConvert<T>(String data)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return  JsonConvert.DeserializeObject<T>(data, settings);
        }
    }
}
