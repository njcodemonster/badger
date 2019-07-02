using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using badgerApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Dynamic;
using badgerApi.Helper;
using Microsoft.Extensions.Configuration;
using CommonHelper;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IVendorRepository _VendorRepo;
        ILoggerFactory _loggerFactory;
        private INotesAndDocHelper _NotesAndDoc;
        private int note_type = 3;
        private IEventRepo _eventRepo;
        private IProductRepository _productRepository;


        private int event_vendor_id = 1;
        private int event_vendor_note_create_id = 22;
        private int event_type_vendor_document_create_id = 21;
        private int event_type_vendor_update_id = 23;
        private string userEventTableName = "user_events";
        private string vendorEventTableName = "vendor_events";

        private string event_create_vendor= "Vendor created by user =%%userid%% with vendor id= %%vendorid%%";
        private string event_create_vendor_note = "Vendor note created by user =%%userid%% with note id= %%noteid%%";
        private string event_create_vendor_document = "Vendor document created by user =%%userid%% with document id= %%documentid%%";
        private string event_update_vendor = "Vendor updated by user =%%userid%% with vendor id= %%vendorid%%";






        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public VendorController(IVendorRepository VendorRepo, ILoggerFactory loggerFactory, INotesAndDocHelper NotesAndDoc, IConfiguration config, IEventRepo eventRepo,IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _eventRepo = eventRepo;
            _config = config;
            _VendorRepo = VendorRepo;
            _loggerFactory = loggerFactory;
            _NotesAndDoc = NotesAndDoc;
           
        }

        // GET: api/vendor/list
        [HttpGet("list")]
        public async Task<ActionResult<List<Vendor>>> GetAsync()
        {
            // List<Documents> notes = await _NotesAndDoc.GenericGetDocAsync<Documents>(2001, 0, 2);
            // string nnn = await _NotesAndDoc.GenericPostDoc<String>(2001,0,"testurl/url","test doc",0,254896312.2);
           
            List<Vendor> ToReturn = new List<Vendor>();
            try
            {
                return await _VendorRepo.GetAll(0);
            }
            catch(Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message"+ex.Message);
                return ToReturn;
            }
           

        }
        // GET: api/vendor/list
        [HttpGet("list/products/{id}")]
        public async Task<List<Product>> ListVendorProducts(string id)
        {
            // List<Documents> notes = await _NotesAndDoc.GenericGetDocAsync<Documents>(2001, 0, 2);
            // string nnn = await _NotesAndDoc.GenericPostDoc<String>(2001,0,"testurl/url","test doc",0,254896312.2);

            List<Product> ToReturn = new List<Product>();
            try
            {
                ToReturn = await _productRepository.GetProductsByVendorId(id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }
            return ToReturn;


        }
        // GET: api/vendor/count
        [HttpGet("count")]
        public async Task<string> CountAsync()
        {
            return await _VendorRepo.Count();

        }
        // GET: api/vendor/listpageview/10
        [HttpGet("listpageview/{limit}")]
        public async Task<object> listpageviewAsync(int limit)
        {
            dynamic vPageList = new object();
            try
            {
                vPageList = await _VendorRepo.GetVendorPageList(limit);
                string  vPageCount = await _VendorRepo.Count();
                vPageList.Count = vPageCount;
                vPageList.VendorType = await _VendorRepo.GetVendorTypes();
            }
            catch(Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);
                
            }

            return vPageList;

        }

        //GET: api/vendor/getvendornameandid
        [HttpGet("getvendorsnameandid")]
        public async Task<List<object>> GetVendorsNameAndID()
        {
            dynamic vendorDetails = new object();
            try
            {
                vendorDetails = await _VendorRepo.GetVendorsNameAndID();

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return vendorDetails;

        }


        //GET: api/vendor/detailsadressandrep/103
        [HttpGet("detailsaddressandrep/{id}")]
        public async Task<object> DetailsAddressAndRep(int id)
        {
            dynamic AdressAndrepDetails = new ExpandoObject();
            dynamic vendor = new ExpandoObject();
            dynamic vendor_address = new ExpandoObject();
            dynamic vendor_Rep = new ExpandoObject();
            dynamic vendor_Note = new ExpandoObject();
            try
            {
                vendor = await _VendorRepo.GetById(id);
                vendor_address = await _VendorRepo.GetVendorDetailsAddress(id);
                vendor_Rep = await _VendorRepo.GetVendorDetailsRep(id);
                AdressAndrepDetails.Vendor = vendor;
                AdressAndrepDetails.Addresses = vendor_address;
                AdressAndrepDetails.Reps = vendor_Rep;

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return AdressAndrepDetails;

        }

        //GET: api/vendor/getnoteanddoc/103
        [HttpGet("getnoteanddoc/{id}")]
        public async Task<object> DetailsNotesAndDoc(int id)
        {
            dynamic vendorNoteAndDoc = new ExpandoObject();
            try
            {
                vendorNoteAndDoc.note = await _NotesAndDoc.GenericNote<Notes>(id, note_type, 1);
                vendorNoteAndDoc.doc = await _NotesAndDoc.GenericGetDocAsync<Documents>(id, note_type, 1);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return vendorNoteAndDoc;

        }

        //GET: api/vendor/detailsaddress/103
        [HttpGet("detailsaddress/{id}")]
        public async Task<List<object>> DetailsAddress(int id)
        {
            dynamic AdressDetails = new object();
            try
            {
                AdressDetails = await _VendorRepo.GetVendorDetailsAddress(id);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return AdressDetails;

        }
        //GET: api/vendor/detailsrep/103
        [HttpGet("detailsRep/{id}")]
        public async Task<List<object>> DetailsRep(int id)
        {
            dynamic RepDetails = new object();
            try
            {
                RepDetails = await _VendorRepo.GetVendorDetailsRep(id);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return RepDetails;

        }
        // GET: api/vendor/list/1
        [HttpGet("list/{id}")]
        public async Task<List<Vendor>> GetAsync(int id)
        {
            List<Vendor> ToReturn = new List<Vendor>();
            try
            {
                Vendor Res =  await _VendorRepo.GetById(id);
                ToReturn.Add(Res);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);
               
            }
            return ToReturn;
        }

        // POST: api/vendor/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                Vendor newVendor = JsonConvert.DeserializeObject<Vendor>(value);
                int created_by = newVendor.created_by;
                NewInsertionID = await _VendorRepo.Create(newVendor);

                event_create_vendor = event_create_vendor.Replace("%%userid%%", created_by.ToString()).Replace("%%vendorid%%", NewInsertionID);

                _eventRepo.AddVendorEventAsync(Int32.Parse(NewInsertionID), event_vendor_id, 0, created_by, event_create_vendor, _common.GetTimeStemp(), vendorEventTableName);

                _eventRepo.AddEventAsync(event_vendor_id, created_by, Int32.Parse(NewInsertionID), event_create_vendor, _common.GetTimeStemp(), userEventTableName);
            }
            catch(Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new vendor with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // POST: api/vendor/note/create
        [HttpPost("note/create")]
        public async Task<string> NoteCreate([FromBody]   string value)
        {
            string newNoteID = "0";
            try
            {
                dynamic newVendorNote = JsonConvert.DeserializeObject<Object>(value);
                int ref_id = newVendorNote.ref_id;
                string note = newVendorNote.note;
                double created_at = _common.GetTimeStemp();
                int created_by = newVendorNote.created_by;
                newNoteID = await _NotesAndDoc.GenericPostNote<string>(ref_id, note_type, note, created_by, created_at);
                event_create_vendor_note = event_create_vendor_note.Replace("%%userid%%", created_by.ToString()).Replace("%%noteid%%", newNoteID);

                _eventRepo.AddVendorEventAsync(ref_id, event_vendor_note_create_id, Int32.Parse(newNoteID), created_by, event_create_vendor_note, _common.GetTimeStemp(), vendorEventTableName);

                _eventRepo.AddEventAsync(event_vendor_note_create_id, created_by, Int32.Parse(newNoteID), event_create_vendor_note, _common.GetTimeStemp(), userEventTableName);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new vendor with message" + ex.Message);
            }
            return newNoteID;
        }

        // POST: api/vendor/documentcreate
        [HttpPost("documentcreate")]
        public async Task<string> DocumentCreate([FromBody]   string value)
        {
            string newDocID = "0";
            try
            {
                dynamic newVendorDoc = JsonConvert.DeserializeObject<Object>(value);
                int ref_id = newVendorDoc.ref_id;
                string url = newVendorDoc.url;
                double created_at = _common.GetTimeStemp();
                int created_by = newVendorDoc.created_by;
                newDocID = await _NotesAndDoc.GenericPostDoc<string>(ref_id, note_type, url, "", created_by, created_at);
                event_create_vendor_document = event_create_vendor_document.Replace("%%userid%%", newVendorDoc.created_by.ToString()).Replace("%%documentid%%", newDocID);

                _eventRepo.AddVendorEventAsync(ref_id, event_type_vendor_document_create_id, Int32.Parse(newDocID), created_by, event_create_vendor_document, _common.GetTimeStemp(), vendorEventTableName);

                _eventRepo.AddEventAsync(event_type_vendor_document_create_id, created_by, Int32.Parse(newDocID), event_create_vendor_document, _common.GetTimeStemp(), userEventTableName);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new vendor with message" + ex.Message);
            }
            return newDocID;
        }
        // PUT: api/vendor/update/5
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                Vendor VendorToUpdate = JsonConvert.DeserializeObject<Vendor>(value);
                VendorToUpdate.vendor_id = id;
                int updated_by = VendorToUpdate.updated_by;
                UpdateProcessOutput = await _VendorRepo.Update(VendorToUpdate);
                event_update_vendor = event_update_vendor.Replace("%%userid%%", updated_by.ToString()).Replace("%%vendorid%%", id.ToString());

                _eventRepo.AddVendorEventAsync(id, event_type_vendor_update_id, id, updated_by, event_update_vendor, _common.GetTimeStemp(), vendorEventTableName);

                _eventRepo.AddEventAsync(event_type_vendor_update_id, updated_by, id, event_update_vendor, _common.GetTimeStemp(), userEventTableName);


            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  vendor with message" + ex.Message);
                UpdateResult = "Failed";
            }
            if (!UpdateProcessOutput)
            {
                UpdateResult = "Creation failed due to reason: No specific reson";
            }
            return UpdateResult;
        }

        // PUT: api/vendor/specificUpdate/5
        [HttpPut("updatespecific/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
           
            try
            {
                Vendor VendorToUpdate = JsonConvert.DeserializeObject<Vendor>(value);
                VendorToUpdate.vendor_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                if(VendorToUpdate.vendor_type != 0)
                {
                    ValuesToUpdate.Add("vendor_type", VendorToUpdate.vendor_type.ToString());
                }
                if(VendorToUpdate.vendor_name != null)
                {
                    ValuesToUpdate.Add("vendor_name", VendorToUpdate.vendor_name);
                }
                if (VendorToUpdate.vendor_description != null)
                {
                    ValuesToUpdate.Add("vendor_description", VendorToUpdate.vendor_description);
                }
                if (VendorToUpdate.corp_name != null)
                {
                    ValuesToUpdate.Add("corp_name", VendorToUpdate.corp_name);
                }
                if (VendorToUpdate.statement_name != null)
                {
                    ValuesToUpdate.Add("statement_name", VendorToUpdate.statement_name);
                }
                if (VendorToUpdate.our_customer_number != null)
                {
                    ValuesToUpdate.Add("our_customer_number", VendorToUpdate.our_customer_number);
                }
                if (VendorToUpdate.created_by != 0)
                {
                    ValuesToUpdate.Add("created_by", VendorToUpdate.created_by.ToString());
                }
                if (VendorToUpdate.updated_by != 0)
                {
                    ValuesToUpdate.Add("updated_by", VendorToUpdate.updated_by.ToString());
                }
                if (VendorToUpdate.active_status != 0)
                {
                    ValuesToUpdate.Add("active_status", VendorToUpdate.active_status.ToString());
                }
                if (VendorToUpdate.vendor_since != 0)
                {
                    ValuesToUpdate.Add("vendor_since", VendorToUpdate.vendor_since.ToString());
                }
                if (VendorToUpdate.created_at != 0)
                {
                    ValuesToUpdate.Add("created_at", VendorToUpdate.created_at.ToString());
                }
                if (VendorToUpdate.updated_at != 0)
                {
                    ValuesToUpdate.Add("updated_at", VendorToUpdate.updated_at.ToString());
                }


                await _VendorRepo.UpdateSpecific(ValuesToUpdate, "vendor_id="+id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new vendor with message" + ex.Message);
                UpdateResult = "Failed";
            }
            
            return UpdateResult;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // GET: api/vendor/skufamily/id
        [HttpGet("list/skufamily/{id}")]
        public async Task<Object> ListVendorLastSku(string id)
        {
            
            dynamic ToReturn = new object();
            try
            {
                ToReturn = await _VendorRepo.GetVendorLastSku(id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }
            return ToReturn;


        }

        //GET: api/vendor/getvendortypes
        [HttpGet("getvendortypes")]
        public async Task<List<object>> GetVendorTypes()
        {
            dynamic VendorTypes = new object();
            try
            {
                VendorTypes = await _VendorRepo.GetVendorTypes();

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return VendorTypes;

        }
    }
}
