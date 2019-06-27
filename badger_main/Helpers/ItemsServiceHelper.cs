using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace badgerApi.Helper
{

   
    [Table("items")]
    public partial class Items
    {
        [Key]
        public int item_id { get; set; }
        public decimal barcode { get; set; }
        public int? slot_number { get; set; }
        public int? bag_code { get; set; }
        public int item_status_id { get; set; }
        public int ra_status { get; set; }
        public string sku { get; set; }
        public short sku_id { get; set; }
        public int product_id { get; set; }
        public int vendor_id { get; set; }
        public string sku_family { get; set; }
        public int? PO_id { get; set; }
        public int? published { get; set; }
        public int? published_by { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }

    public interface IItemServiceHelper
    {
        Task<List<Items>> GetItemsByOrder(int PO_id);
        Task<Object> GetAllStatus();
        Task<T> GenericPostAsync<T>(T json, String _call);
    }
        public class ItemsServiceHelper:IItemServiceHelper
    {
        private String ItemApiUrl = "";

        private readonly IConfiguration _config;
        public ItemsServiceHelper(IConfiguration config)
        {

            _config = config;
            ItemApiUrl = _config.GetValue<string>("Services:ItemsService");

        }

        public async Task<List<Items>> GetItemsByOrder(int PO_id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(ItemApiUrl + "/item/list/listforPO/"+PO_id.ToString(), HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<List<Items>>(data, settings);
        }

        public async Task<object> GetAllStatus()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(ItemApiUrl + "/item/status/list", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return data;
        }
        public async Task<T> GenericPostAsync<T>(T json, String _call)
        {
            var client = new HttpClient();
            var response = await client.PostAsJsonAsync(ItemApiUrl + _call, json);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(data, settings);

        }
    }
}
