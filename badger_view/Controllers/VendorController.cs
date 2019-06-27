using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using badger_view.Helpers;
using badger_view.Models;
using System.Dynamic;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace badger_view.Controllers
{
    public class vendorFileData
    {
        public List<IFormFile> vendorDocuments { get; set; }
        public string Vendor_id { get; set; }
    }
    public class VendorController : Controller
    {
        
        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        private String UploadPath = "";
        private ILoginHelper _LoginHelper;
        public VendorController(IConfiguration config, ILoginHelper LoginHelper)
        {
            _LoginHelper = LoginHelper;
            _config = config;
            UploadPath = _config.GetValue<string>("UploadPath:path");

        }
        private void SetBadgerHelper()
        {
            if (_BadgerApiHelper == null)
            {
                _BadgerApiHelper = new BadgerApiHelper(_config);
            }
        }
       
        public async Task<IActionResult> Index()
        {
            SetBadgerHelper();
           
            VendorPagerList vendorPagerList = await _BadgerApiHelper.GenericGetAsync<VendorPagerList>("/vendor/listpageview/200");
            dynamic VendorPageModal = new ExpandoObject();
            VendorPageModal.VendorCount = vendorPagerList.Count; 
            VendorPageModal.VendorLists = vendorPagerList.vendorInfo;
           // VenderAdressandRep venderAdressandRep = await _BadgerApiHelper.GenericGetAsync<VenderAdressandRep>("/Vendor/detailsaddressandrep/103");
          
            //VendorPageModal.Reps = venderAdressandRep.Reps;
            return View("Index",VendorPageModal);
        }
        [HttpGet("vendor/details/{id}")]
        public async Task<Object> GetDetails(Int32 id)
        {
            dynamic vendorDetails = new ExpandoObject();
            SetBadgerHelper();
            
            VenderAdressandRep venderAdressandRep = await _BadgerApiHelper.GenericGetAsync<VenderAdressandRep>("/vendor/detailsaddressandrep/" + id.ToString());
            dynamic venderDocAndNotes = await _BadgerApiHelper.GenericGetAsync<object>("/vendor/getnoteanddoc/" + id.ToString());
            vendorDetails.venderAdressandRep = venderAdressandRep;
            vendorDetails.venderDocAndNotes = venderDocAndNotes;
            return JsonConvert.SerializeObject(vendorDetails);
        }
        [HttpPost("vendor/newvendor_doc")]
        public async Task<String> CreateNewVendorDoc(vendorFileData vendorDoc)
        {
            SetBadgerHelper();
            string loginUserId = await _LoginHelper.GetLoginUserId();
            string messageDocuments = "";
            string messageAlreadyDocuments = "";
            try
            {
                //string Fill_path = vendorDoc.vendorDocuments.FileName;
                //Fill_path = UploadPath + Fill_path;
                //using (var stream = new FileStream(Fill_path, FileMode.Create))
                //{
                //    await vendorDoc.vendorDocument.CopyToAsync(stream);
                //    int ref_id = Int32.Parse(vendorDoc.Vendor_id);
                //    JObject vendorDocuments = new JObject();
                //    vendorDocuments.Add("ref_id", ref_id);
                //    vendorDocuments.Add("url", Fill_path);
                //    await _BadgerApiHelper.GenericPostAsyncString<String>(vendorDocuments.ToString(Formatting.None), "/vendor/documentcreate");
                //}
                //return Fill_path;


                List<IFormFile> files = vendorDoc.vendorDocuments;

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        string Fill_path = formFile.FileName;
                        Fill_path = UploadPath + Fill_path;

                        if (System.IO.File.Exists(Fill_path))
                        {
                            messageAlreadyDocuments += "File Already Exists: " + Fill_path + " \r\n";
                        }
                        else
                        {
                            using (var stream = new FileStream(Fill_path, FileMode.Create))
                            {
                                messageDocuments += Fill_path + " \r\n";

                                await formFile.CopyToAsync(stream);

                                int ref_id = Int32.Parse(vendorDoc.Vendor_id);
                                JObject vendorDocuments = new JObject();
                                vendorDocuments.Add("ref_id", ref_id);
                                vendorDocuments.Add("created_by", Int32.Parse(loginUserId));
                                vendorDocuments.Add("url", Fill_path);
                                await _BadgerApiHelper.GenericPostAsyncString<String>(vendorDocuments.ToString(Formatting.None), "/vendor/documentcreate");


                            }
                        }
                    }
                }

                return messageDocuments + " \r\n " + messageAlreadyDocuments;
            }
            catch(Exception ex)
            {
                return "0";
            }
        }
        [HttpPost("vendor/newvendor")]
        public  async Task<String> CreateNewVendor([FromBody]   JObject json)
        {
            SetBadgerHelper();
            string loginUserId = await _LoginHelper.GetLoginUserId();
            //string newVendorID = "12";
            JObject vendor = new JObject();
            JObject vendor_adress = new JObject();
            //List<JObject> vendor_reps = new List<JObject>();
            vendor.Add("vendor_name", json.Value<string>("vendor_name"));
            vendor.Add("corp_name", json.Value<string>("corp_name"));
            vendor.Add("statement_name", json.Value<string>("statement_name"));
            vendor.Add("vendor_code", json.Value<string>("vendor_code"));
            vendor.Add("our_customer_number", json.Value<string>("our_customer_number"));
            vendor.Add("created_by", Int32.Parse(loginUserId));
            vendor.Add("active_status", 1);
            vendor.Add("created_at", _common.GetTimeStemp());
            String newVendorID = await _BadgerApiHelper.GenericPostAsyncString<String>(vendor.ToString(Formatting.None), "/vendor/create");
            vendor_adress.Add("vendor_id",newVendorID);
            vendor_adress.Add("vendor_street", json.Value<string>("vendor_street"));
            vendor_adress.Add("vendor_suite_number", json.Value<string>("vendor_suite_number"));
            vendor_adress.Add("vendor_city", json.Value<string>("vendor_city"));
            vendor_adress.Add("vendor_zip", json.Value<string>("vendor_zip"));
            vendor_adress.Add("vendor_state", json.Value<string>("vendor_state"));
            vendor_adress.Add("created_by", Int32.Parse(loginUserId));
            vendor_adress.Add("created_at", _common.GetTimeStemp());
            String newVendorAdressID = await _BadgerApiHelper.GenericPostAsyncString<String>(vendor_adress.ToString(Formatting.None), "/VendorAddress/create");
            JObject allData = JObject.Parse(json.ToString());
            JArray vendor_reps_data = (JArray)allData["vendor_reps"];

            for (int i = 0; i < vendor_reps_data.Count; i++)
            {
                JObject vendor_rep = new JObject();
                vendor_rep.Add("vendor_id", newVendorID);
                vendor_rep.Add("first_name", vendor_reps_data[i].Value<string>("Rep_first_name"));
                vendor_rep.Add("full_name", vendor_reps_data[i].Value<string>("Rep_full_name"));
                vendor_rep.Add("phone1", vendor_reps_data[i].Value<string>("Rep_phone1"));
                vendor_rep.Add("phone2", vendor_reps_data[i].Value<string>("Rep_phone2"));
                vendor_rep.Add("email", vendor_reps_data[i].Value<string>("Rep_email"));
                vendor_rep.Add("main", vendor_reps_data[i].Value<string>("main"));
                vendor_rep.Add("created_by", Int32.Parse(loginUserId));
                vendor_rep.Add("created_at", _common.GetTimeStemp());
                String newVendorRepID = await _BadgerApiHelper.GenericPostAsyncString<String>(vendor_rep.ToString(Formatting.None), "/VendorRep/create");
            }
            string vendor_notes = json.Value<string>("vendor_notes");
            if (vendor_notes != "")
            {
                JObject vendorNotes = new JObject();
                vendorNotes.Add("ref_id", newVendorID);
                vendorNotes.Add("note", vendor_notes);
                vendorNotes.Add("created_by", Int32.Parse(loginUserId));
                String newNoteID = await _BadgerApiHelper.GenericPostAsyncString<String>(vendorNotes.ToString(Formatting.None), "/vendor/note/create");

            }
            return newVendorID;

           
        }
        [HttpPost("vendor/updatevendor/{id}")]
        public async Task<String> UpdateNewVendor(int id ,[FromBody]   JObject json)
        {
            SetBadgerHelper();
            string loginUserId = await _LoginHelper.GetLoginUserId();
            //string newVendorID = "12";
            JObject vendor = new JObject();
            JObject vendor_adress = new JObject();
            //List<JObject> vendor_reps = new List<JObject>();
            vendor.Add("vendor_name", json.Value<string>("vendor_name"));
            vendor.Add("corp_name", json.Value<string>("corp_name"));
            vendor.Add("statement_name", json.Value<string>("statement_name"));
            vendor.Add("vendor_code", json.Value<string>("vendor_code"));
            vendor.Add("our_customer_number", json.Value<string>("our_customer_number"));
            vendor.Add("updated_by", Int32.Parse(loginUserId));
            vendor.Add("active_status", 1);
            vendor.Add("updated_at", _common.GetTimeStemp());
            String vendorStatus = await _BadgerApiHelper.GenericPutAsyncString<String>(vendor.ToString(Formatting.None), "/vendor/update/"+id.ToString());
            if (vendorStatus == "Success")
            {
                int address_id = json.Value<int>("address_id");
                vendor_adress.Add("vendor_id", id);
                vendor_adress.Add("vendor_street", json.Value<string>("vendor_street"));
                vendor_adress.Add("vendor_suite_number", json.Value<string>("vendor_suite_number"));
                vendor_adress.Add("vendor_city", json.Value<string>("vendor_city"));
                vendor_adress.Add("vendor_zip", json.Value<int>("vendor_zip"));
                vendor_adress.Add("vendor_state", json.Value<string>("vendor_state"));
                vendor_adress.Add("updated_by", Int32.Parse(loginUserId));
                vendor_adress.Add("updated_at", _common.GetTimeStemp());
                String VendorAdressStatus = await _BadgerApiHelper.GenericPutAsyncString<String>(vendor_adress.ToString(Formatting.None), "/VendorAddress/update/" + address_id.ToString());
                JObject allData = JObject.Parse(json.ToString());
                JArray vendor_reps_data = (JArray)allData["vendor_reps"];

                for (int i = 0; i < vendor_reps_data.Count; i++)
                {
                    JObject vendor_rep = new JObject();
                    vendor_rep.Add("vendor_id", id);
                    vendor_rep.Add("first_name", vendor_reps_data[i].Value<string>("Rep_first_name"));
                    vendor_rep.Add("full_name", vendor_reps_data[i].Value<string>("Rep_full_name"));
                    vendor_rep.Add("phone1", vendor_reps_data[i].Value<string>("Rep_phone1"));
                    vendor_rep.Add("phone2", vendor_reps_data[i].Value<string>("Rep_phone2"));
                    vendor_rep.Add("email", vendor_reps_data[i].Value<string>("Rep_email"));
                    vendor_rep.Add("main", vendor_reps_data[i].Value<string>("main"));
                    string repo_id =  vendor_reps_data[i].Value<string>("repo_id");
                    if(repo_id == "0")
                    {
                        vendor_rep.Add("created_by", Int32.Parse(loginUserId));
                        vendor_rep.Add("created_at", _common.GetTimeStemp());
                        String newVendorRepID = await _BadgerApiHelper.GenericPostAsyncString<String>(vendor_rep.ToString(Formatting.None), "/VendorRep/create");

                    }
                    else
                    {
                        vendor_rep.Add("updated_by", Int32.Parse(loginUserId));
                        vendor_rep.Add("updated_at", _common.GetTimeStemp());
                        String newVendorRepID = await _BadgerApiHelper.GenericPutAsyncString<String>(vendor_rep.ToString(Formatting.None), "/VendorRep/update/" + repo_id.ToString());

                    }
                }
                string vendor_notes = json.Value<string>("vendor_notes");
                if (vendor_notes != "")
                {
                    JObject vendorNotes = new JObject();
                    vendorNotes.Add("ref_id", id);
                    vendorNotes.Add("note", vendor_notes);
                    vendorNotes.Add("created_by", Int32.Parse(loginUserId));
                    String newNoteID = await _BadgerApiHelper.GenericPostAsyncString<String>(vendorNotes.ToString(Formatting.None), "/vendor/note/create");

                }

            }

            return vendorStatus;

        }
        [HttpGet("vendor/products/{id}")]
        public async Task<Object> GetVendorProducts(Int32 id)
        {
            SetBadgerHelper();
            dynamic vendorProductsandSku = new ExpandoObject();
            dynamic vendorProducts = new ExpandoObject();
            dynamic vendorSkufamily = new ExpandoObject();
            vendorProductsandSku.vendorProducts = await _BadgerApiHelper.GenericGetAsync<object>("/vendor/list/products/" + id.ToString());
            vendorProductsandSku.vendorSkufamily = await _BadgerApiHelper.GenericGetAsync<object>("/vendor/list/skufamily/" + id.ToString());
            //vendorProducts.skufamily = vendorSkufamily;
            return JsonConvert.SerializeObject(vendorProductsandSku);
        }
    }
}