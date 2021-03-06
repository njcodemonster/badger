﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using badger_view.Helpers;
using GenericModals.Models;
using System.Dynamic;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

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
        private CommonHelper.awsS3helper awsS3Helper = new CommonHelper.awsS3helper();
        private String UploadPath = "";
        private String S3bucket = "";
        private String S3folder = "";
        private ILoginHelper _LoginHelper;
        public VendorController(IConfiguration config, ILoginHelper LoginHelper, BadgerApiHelper badgerApiHelper)
        {
            _LoginHelper = LoginHelper;
            _config = config;
            UploadPath = _config.GetValue<string>("UploadPath:path");
            S3bucket = _config.GetValue<string>("S3config:Bucket_Name");
            S3folder = _config.GetValue<string>("S3config:Folder");
            _BadgerApiHelper = badgerApiHelper;
        }

        /*
           Developer: Azeem Hassan
           Date: 7-3-19 
           Action: getting all vendors_type vendor_list and vendor_count from badger api
           URL: index
           Input: Null
           output: dynamic ExpandoObject of VendorPageModal
       */
        [Authorize]
        public async Task<IActionResult> Index()
        {
            
           
            VendorPagerList vendorPagerList = await _BadgerApiHelper.GenericGetAsync<VendorPagerList>("/vendor/listpageview/0/0");

            dynamic VendorPageModal = new ExpandoObject();
            VendorPageModal.VendorCount = vendorPagerList.Count; 
            VendorPageModal.VendorLists = vendorPagerList.vendorInfo;
            VendorPageModal.VendorType = vendorPagerList.vendorType;
       
            return View("Index",VendorPageModal);
        }
        /*
         Developer: Azeem Hassan
         Date:  
         Action:
         URL: 
         Input: Null
         output: dynamic ExpandoObject of VendorPageModal
     */
        [Authorize]
        public async Task<IActionResult> single()
        {
            dynamic vendorDetails = new ExpandoObject();
            
            List<VendorType> getVendorTypes = await _BadgerApiHelper.GenericGetAsync<List<VendorType>>("/vendor/getvendortypes");
            vendorDetails.VendorType = getVendorTypes;
            return View("AddVendor", vendorDetails);
        }
        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get List of limit range data show by using badger api helper   
        URL: /vendor/listpagination/{start}/{limit}"
        Request: Get
        Input: int start, int limit
        output: Dynamic object of vendors
        */
        [Authorize]
        [HttpGet("vendor/listpagination/{start}/{limit}")]
        public async Task<string> ListPagination(int start, int limit)
        {
            

            VendorPagerList vendorPagerList = await _BadgerApiHelper.GenericGetAsync<VendorPagerList>("/vendor/listpageview/" + start + "/" + limit);

            dynamic VendorPageModal = new ExpandoObject();
            VendorPageModal.VendorCount = vendorPagerList.Count;
            VendorPageModal.VendorLists = vendorPagerList.vendorInfo;
            VendorPageModal.VendorType = vendorPagerList.vendorType;

            return JsonConvert.SerializeObject(VendorPageModal);
        }


        /*
            Developer: Azeem Hassan
            Date: 7-3-19 
            Action: getting all vendor details note and doc form badger api
            URL: vendor/details/id
            Input: vendor id
            output: dynamic ExpandoObject of vendorDetails
        */
        [Authorize]
        [HttpGet("vendor/details/{id}")]
        public async Task<Object> GetDetails(Int32 id)
        {
            dynamic vendorDetails = new ExpandoObject();
            
            
            VenderAdressandRep venderAdressandRep = await _BadgerApiHelper.GenericGetAsync<VenderAdressandRep>("/vendor/detailsaddressandrep/" + id.ToString());
            dynamic venderDocAndNotes = await _BadgerApiHelper.GenericGetAsync<object>("/vendor/getnoteanddoc/" + id.ToString());
            vendorDetails.venderAdressandRep = venderAdressandRep;
            vendorDetails.venderDocAndNotes = venderDocAndNotes;
            return JsonConvert.SerializeObject(vendorDetails);
        }
        /*
            Developer: Azeem Hassan
            Date: 7-3-19 
            Action: send vendor logo to badger api
            URL: vendor/newvendor_logo
            Input: vendor file data with vendor id
            output: file inserting massage
        */
        [Authorize]
        [HttpPost("vendor/newvendor_logo")]
        public async Task<String> CreateNewVendorLogo(vendorFileData vendorLogo)
        {
            
            string loginUserId = await _LoginHelper.GetLoginUserId();
            string messageDocuments = "";
            string messageAlreadyDocuments = "";
            try
            {
             
                List<IFormFile> files = vendorLogo.vendorDocuments;

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {

                        awsS3Helper.UploadToS3(formFile.FileName, formFile.OpenReadStream(), S3bucket, S3folder);
                        int ref_id = Int32.Parse(vendorLogo.Vendor_id);
                        JObject vendorDocuments = new JObject();
                        vendorDocuments.Add("vendor_id", ref_id);
                        vendorDocuments.Add("logo", formFile.FileName);
                        await _BadgerApiHelper.GenericPutAsyncString<String>(vendorDocuments.ToString(Formatting.None), "/vendor/updatespecific/" + vendorLogo.Vendor_id);
                    }
                }

                return messageDocuments + " \r\n " + messageAlreadyDocuments;
            }
            catch(Exception ex)
            {
                return "0";
            }
        }
        /*
           Developer: Azeem Hassan
           Date: 7-3-19 
           Action: delete vendor logo from upload folder and update to badger api
           URL: vendor/deletevendor_logo
           Input: vendor file data with vendor id
           output: file deleted massage
       */
        [Authorize]
        [HttpPost("vendor/deletevendor_logo")]
        public async Task<String> DeleteVendorLogo([FromBody]   JObject json)
        {
            
            try
            {

                var fileName = json.Value<string>("vendorDocuments");
                var vendor_id = json.Value<string>("Vendor_id");
                int ref_id = Int32.Parse(vendor_id);
                JObject vendorDocuments = new JObject();
                vendorDocuments.Add("vendor_id", ref_id);
                vendorDocuments.Add("logo", "");
                await _BadgerApiHelper.GenericPutAsyncString<String>(vendorDocuments.ToString(Formatting.None), "/vendor/updatespecific/" + vendor_id);
                return "file deleted successfully";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
        /*
            Developer: Azeem Hassan
            Date: 7-3-19 
            Action: parsing 3 objects of vender_details,vendor_address,vendor_repo and vendor_note data sends to badger api
            URL: vendor/newvendor
            Input: vendor form data json
            output: vendor id
        */
        [Authorize]
        [HttpPost("vendor/newvendor")]
        public  async Task<String> CreateNewVendor([FromBody]   JObject json)
        {
            
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
            vendor.Add("vendor_description", json.Value<string>("vendor_description")); 
            vendor.Add("vendor_type", json.Value<Int32>("vendor_type"));
            vendor.Add("created_by", Int32.Parse(loginUserId));
            vendor.Add("active_status", 1);
            vendor.Add("created_at", _common.GetTimeStemp());
            String newVendorID = await _BadgerApiHelper.PostAsync<string>(vendor, "/vendor/create");
            if (newVendorID != "0")
            {
                vendor_adress.Add("vendor_id", newVendorID);
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
                    JObject vendorOrderStatusNote = new JObject();
                    vendorOrderStatusNote.Add("has_note", 1);
                    await _BadgerApiHelper.GenericPutAsyncString<String>(vendorOrderStatusNote.ToString(Formatting.None), "/vendor/updatespecific/" + newVendorID);
                }
            }
            return newVendorID;

           
        }
        /*
            Developer: Azeem Hassan
            Date: 7-3-19 
            Action: parsing 3 objects of vender_details,vendor_address,vendor_repo and vendor_note sends to badger api to update vendor by id
            URL: vendor/updatevendor
            Input: vendor form data json
            output: status success/failed
        */
        [Authorize]
        [HttpPost("vendor/updatevendor/{id}")]
        public async Task<String> UpdateNewVendor(int id ,[FromBody]   JObject json)
        {
            
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
            vendor.Add("vendor_description", json.Value<string>("vendor_description"));
            vendor.Add("logo", json.Value<string>("logo"));
            vendor.Add("vendor_type", json.Value<Int32>("vendor_type"));
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
                if (vendor_notes != "sameNote")
                {
                    JObject vendorNotes = new JObject();
                    vendorNotes.Add("ref_id", id);
                    vendorNotes.Add("note", vendor_notes);
                    vendorNotes.Add("created_by", Int32.Parse(loginUserId));
                    String newNoteID = await _BadgerApiHelper.GenericPostAsyncString<String>(vendorNotes.ToString(Formatting.None), "/vendor/note/create");
                    JObject vendorOrderStatusNote = new JObject();

                    if (vendor_notes == "")
                    {
                        vendorOrderStatusNote.Add("has_note", 0);
                    }
                    else
                    {
                        vendorOrderStatusNote.Add("has_note", 1);

                    }
                    await _BadgerApiHelper.GenericPutAsyncString<String>(vendorOrderStatusNote.ToString(Formatting.None), "/vendor/updatespecific/" + id);
                }
                

            }

            return vendorStatus;

        }
        /*
            Developer: Azeem Hassan
            Date: 7-3-19 
            Action:send vendor id to badger api to get not and doc
            URL: vendor/getvendornoteanddoc/id
            Input: vendor id
            output: dynamic venderDocAndNotes
        */
        [Authorize]
        [HttpGet("vendor/getvendornoteanddoc/{id}")]
        public async Task<Object> GetNotesAndDoc(Int32 id)
        {
            
            dynamic venderDocAndNotes = await _BadgerApiHelper.GenericGetAsync<object>("/vendor/getnoteanddoc/" + id.ToString());
       
            return JsonConvert.SerializeObject(venderDocAndNotes);
        }

        /*
            Developer: Azeem Hassan
            Date: 7-3-19 
            Action:send vendor note to badger api
            URL: vendor/insertvendornote/id
            Input: vendor not and vendor id
            output: new note id
        */
        [Authorize]
        [HttpPost("vendor/insertvendornote/{id}")]
        public async Task<String> InsertVendorNote(int id, [FromBody]   JObject json)
        {
            
            string loginUserId = await _LoginHelper.GetLoginUserId();
            string vendor_notes = json.Value<string>("vendor_notes");
            String newNoteID = "0";
       
                JObject vendorNotes = new JObject();
                vendorNotes.Add("ref_id", id);
                vendorNotes.Add("note", vendor_notes);
                vendorNotes.Add("created_by", Int32.Parse(loginUserId));
                newNoteID = await _BadgerApiHelper.GenericPostAsyncString<String>(vendorNotes.ToString(Formatting.None), "/vendor/note/create");
            if (vendor_notes != "")
            {
                JObject purchaseOrderStatusNote = new JObject();
                purchaseOrderStatusNote.Add("has_note", 1);
                await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrderStatusNote.ToString(Formatting.None), "/vendor/updatespecific/" + id);
            }
            else
            {
                JObject purchaseOrderStatusNote = new JObject();
                purchaseOrderStatusNote.Add("has_note", 2);
                await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrderStatusNote.ToString(Formatting.None), "/vendor/updatespecific/" + id);
            }
            
            return newNoteID;
        }
        /*
            Developer: Azeem Hassan
            Date: 7-3-19 
            Action:send vendor id to badger api to get vendor all product
            URL: vendor/products/id
            Input: vendor id
            output: vendor products and sku
        */
        //[Authorize]
        [HttpGet("vendor/products/{id}/{product_id}/{po_id}")]
        public async Task<Object> GetVendorProducts(Int32 id,int product_id,int po_id)
        {
            
            dynamic vendorProductsandSku = new ExpandoObject();

            vendorProductsandSku.vendorProducts = await _BadgerApiHelper.GenericGetAsync<object>("/vendor/list/products/" + id.ToString()+"/"+product_id);
            vendorProductsandSku.productLineItemList = await _BadgerApiHelper.GenericGetAsync<object>("/purchaseorders/lineitems/" + product_id.ToString() + "/" + po_id.ToString());

            vendorProductsandSku.vendor = await _BadgerApiHelper.GenericGetAsync<object>("/vendor/list/" + id.ToString());
            //vendorProductsandSku.Sizes = await _BadgerApiHelper.GenericGetAsync<object>("/attributes/list/type/1");
            //vendorProductsandSku.categories = await _BadgerApiHelper.GenericGetAsync<object>("/categories/list");

            return JsonConvert.SerializeObject(vendorProductsandSku);
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Vendors list which matched by string
        URL: /vendor/autosuggest
        Request: Post
        Input: [FromBody]  string json
        output: dynamic list of venders which matched with string
        */
        [Authorize]
        [HttpPost("vendor/autosuggest")]
        public async Task<string> Autosuggest([FromBody]   JObject json)
        {
            

            string search = json.Value<string>("search");
            string columnName = json.Value<string>("columnname");
            object getVendorsNameAndId = new object();
            JObject vendorSearchObj = new JObject();
            vendorSearchObj.Add("search", search);
            vendorSearchObj.Add("columnName", columnName);
            getVendorsNameAndId = await _BadgerApiHelper.GenericPostAsync<object>(vendorSearchObj.ToString(Formatting.None), "/vendor/getvendorsbycolumnname");
            return JsonConvert.SerializeObject(getVendorsNameAndId);
        }
        /*
        Developer: Azeem
        Date: 7-11-19 
        Action: sending vendor to badger api to check vendor code existence 
        URL: vendor/vendorcodeexist
        Request: GET
        Input: vendorcode
        output: vendor exist massage
        */
        [Authorize]
        [HttpPost("vendor/vendorcodeexist")]
        public async Task<string> VendorCodeExist([FromBody]   JObject json)
        {
            string vendorcode = json.Value<string>("vendorcode");
            dynamic vendorCodeList = await _BadgerApiHelper.GenericGetAsync<Object>("/vendor/checkvendorcodeexist/"+ vendorcode);
            return JsonConvert.SerializeObject(vendorCodeList); 
        }
        /*
        Developer: Azeem
        Date: 9-24-19 
        Action: sending vendor to badger api to check vendor name existence 
        URL: vendor/vendornameexist
        Request: POST
        Input: vendorcode
        output: vendor exist massage
        */
        [Authorize]
        [HttpPost("vendor/vendornameexist")]
        public async Task<string> VendorNameExist([FromBody]   JObject json)
        {
            string vendorname = json.Value<string>("vendorname");
            JObject vendorSearchObj = new JObject();
            vendorSearchObj.Add("vendorname", vendorname);
            dynamic vendorCodeList = await _BadgerApiHelper.GenericPostAsync<object>(vendorSearchObj.ToString(Formatting.None), "/vendor/checkvendornameexist");

            return JsonConvert.SerializeObject(vendorCodeList);
        }

    }
}