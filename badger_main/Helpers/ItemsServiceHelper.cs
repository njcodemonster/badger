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
        Task<string> ItemUpdateById(int id, string json);
        Task<string> SkuUpdateById(int id, string json);
        Task<T> GenericPostAsync<T>(T json, String _call);
        Task<string> SetProductItemStatusForPhotoshootAsync(string json, int status);
        Task<Boolean> CheckBarcodeExist(int barcode);

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


        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get Items by purchase order id
        URL: 
        Request: Get
        Input:  int poid
        output: dynamic object of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get All status of items
        URL: 
        Request: Get
        Input:  
        output: dynamic object of items status
        */
        public async Task<object> GetAllStatus()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(ItemApiUrl + "/item/status/list", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Item update by id
        URL: 
        Request: Put
        Input:  int id, string data
        output: string of items data
        */
        public async Task<string> ItemUpdateById(int id, string json)
        {
            var client = new HttpClient();
            var response = await client.PutAsJsonAsync(ItemApiUrl + "/item/update/"+id.ToString(), json);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return data.ToString();
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: SKU Item update by id
        URL: 
        Request: Put
        Input:  int id, string data
        output: string of items data
        */
        public async Task<string> SkuUpdateById(int id, string json)
        {
            var client = new HttpClient();
            var response = await client.PutAsJsonAsync(ItemApiUrl + "/item/update/" + id.ToString(), json);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return data.ToString();
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Create New Item data
        URL: 
        Request: Post
        Input:  type json, string data
        output: dynamic type of items data
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get Product Items by small sku
        URL: 
        Request: Get
        Input:  string product_id
        output: dynamic object of items product data of small sku
        */
        public async Task<object> GetProductItemsSmallSku(string product_id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(ItemApiUrl + "/item/GetProductItemsSmallSku/"+ product_id, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Update Product Items photoshoot status
        URL: 
        Request: Post
        Input:  string json , string status
        output: string of items product data of small sku
        */
        public async Task<string> SetProductItemStatusForPhotoshootAsync(string json , int status)
        {
            var client = new HttpClient();
            var response = await client.PostAsJsonAsync(ItemApiUrl + "/item/UpdateProductItemForPhotoshoot/"+ status, json );
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        /*
       Developer: Sajid Khan
       Date: 7-20-19 
       Action: Check Barcode already Exist by barcode 
       Request: Get
       Input: int barcode
       output: Boolean
       */
        public async Task<Boolean> CheckBarcodeExist(int barcode)
        {
            Boolean result = false;
            var client = new HttpClient();
            var response = await client.GetAsync(ItemApiUrl + "/item/checkbarcodeexist/"+barcode.ToString(), HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            if (data == "true")
            {
                result = true;
            }
            return result;
         }

    }
}
