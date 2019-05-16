using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace badger_view.Helpers
{
    public class BadgerApiHelper
    {
        private readonly IConfiguration _config;
        private String BadgerAPIURL = "";
       
        public BadgerApiHelper(IConfiguration config)
        {

            _config = config;
            BadgerAPIURL = _config.GetValue<string>("Services:Badger");

        }
        public async Task<T> GenericGetAsync<T>(String _call)
        {
            var client = new HttpClient();
           // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.GetAsync(BadgerAPIURL + _call, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(data);

        }

    }
}
