﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.S3;
using GenericModals;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Net.Http.Formatting;
using Microsoft.Extensions.Options;
using CommonHelper;

namespace badger_view.Helpers
{
    public class BadgerApiHelper
    {

        private String BadgerAPIURL = "";
        private BadgerApiHelper _BadgerApiHelper;

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
        public async Task<string> GenericGetsAsync(String _call)
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.GetAsync(BadgerAPIURL + _call, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return data;

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
            return JsonConvert.DeserializeObject<T>(data, settings);

        }

        public async Task<T> GetAsync<T>(string uri) where T : class
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.GetAsync(BadgerAPIURL + uri, HttpCompletionOption.ResponseHeadersRead);
            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var responseData = JsonConvert.DeserializeObject<ResponseModel>(data, settings);
            ThorwException(responseData);
            return CommonHelper.CommonHelper.IsJson(responseData.Data.ToString()) ?
                JsonConvert.DeserializeObject<T>(responseData.Data.ToString(), settings)
                : responseData.Data as T;
        }

        private static void ThorwException(ResponseModel responseData)
        {
            if (responseData.Status != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responseData.Message);
            }
        }

        public async Task<TReturn> PostAsync<TReturn>(object json, string uri) where TReturn : class
        {
            var client = new HttpClient();
            HttpResponseMessage response;
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);

            if (json is JObject)
            {
                response = await client.PostAsJsonAsync(BadgerAPIURL + uri, json.ToString());
            }
            else
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(json, Formatting.Indented), Encoding.UTF8, "application/json");
                response = await client.PostAsync(BadgerAPIURL + uri, content);
            }

            var data = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var responseData = JsonConvert.DeserializeObject<ResponseModel>(data, settings);
            ThorwException(responseData);
            return CommonHelper.CommonHelper.IsJson(responseData.Data.ToString()) ?
                JsonConvert.DeserializeObject<TReturn>(responseData.Data.ToString(), settings) 
                : responseData.Data as TReturn;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: data sends to badger api
        Request: Post 
        Input: Json Type, URL
        output: json object 
        */
        public async Task<T> GenericPostAsync<T>(T json, String _call)
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
            return JsonConvert.DeserializeObject<T>(data, settings);
        }

        /*
       Developer: hamza Haq
       Date: 7-7-19 
       Action: data sends to badger api
       Request: Put 
       Input: Json Type, URL
       output: string data
       */
        public async Task<String> GenericDeleteAsyncString<T>(String _call)
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.DeleteAsync(BadgerAPIURL + _call);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return data.ToString();

        }
    }
}
