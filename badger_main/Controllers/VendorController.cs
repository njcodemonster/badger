using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using GenericModals.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Dynamic;
using badgerApi.Helper;
using Microsoft.Extensions.Configuration;
using CommonHelper;
using GenericModals;
using GenericModals.Event;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IVendorRepository _VendorRepo;
        private readonly IVendorProductRepository _vendorproductRepository;
        ILoggerFactory _loggerFactory;
        private INotesAndDocHelper _NotesAndDoc;
        private int note_type = 3;
        private IEventRepo _eventRepo;
        private IProductRepository _productRepository;
        private string userEventTableName = "user_events";
        private string vendorEventTableName = "vendor_events";
        string create_vendor = "create_vendor";
        string create_vendor_product = "create_vendor_product";
        string create_vendor_note = "create_vendor_note";
        string create_vendor_document = "create_vendor_document";
        string update_vendor = "update_vendor";
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public VendorController(IVendorRepository VendorRepo, ILoggerFactory loggerFactory, INotesAndDocHelper NotesAndDoc, IConfiguration config, IEventRepo eventRepo,IProductRepository productRepository , IVendorProductRepository vendorproductRepository)
        {
            _productRepository = productRepository;
            _eventRepo = eventRepo;
            _config = config;
            _VendorRepo = VendorRepo;
            _loggerFactory = loggerFactory;
            _NotesAndDoc = NotesAndDoc;
            _vendorproductRepository = vendorproductRepository;


        }
        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: getting list of all vendors
            URL: api/vendor/list
            Request GET
            Input: null
            output: vendor
        */
        // GET: api/vendor/list
        [HttpGet("list")]
        public async Task<ActionResult<List<Vendor>>> GetAsync()
        {  
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
        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: getting vendor products by id
           URL: api/list/products/id
           Request GET
           Input: vendor id
           output: vendor products
       */
        // GET: api/list/products/id
        [HttpGet("list/products/{id}/{product_id}")]
        public async Task<List<Product>> ListVendorProducts(string id,int product_id)
        {

            List<Product> ToReturn = new List<Product>();
            try
            {
                ToReturn = await _productRepository.GetProductsByVendorId(id, product_id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }
            return ToReturn;


        }
        /*
          Developer: Azeem Hassan
          Date: 7-5-19 
          Action: getting vendor count
          URL:  api/vendor/count
          Request GET
          Input: null
          output: vendor count
      */
        // GET: api/vendor/count
        [HttpGet("count")]
        public async Task<string> CountAsync()
        {
            return await _VendorRepo.Count();

        }

        /*
          Developer: Azeem Hassan
          Date: 7-5-19 
          Action: getting vendor count
          URL:  api/vendor/listpageview/start/limit
          Request GET
          Input: null
          output: vendor count
        */
        // GET: api/vendor/listpageview/10
        [HttpGet("listpageview/{start}/{limit}")]
        public async Task<object> listpageviewAsync(int start, int limit)
        {
            dynamic vPageList = new object();
            try
            {
                vPageList = await _VendorRepo.GetVendorPageList(start,limit);
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

        /*
          Developer: Azeem Hassan
          Date: 7-5-19 
          Action: getting vendor name and id
          URL:  api/vendor/getvendornameandid
          Request GET
          Input: null
          output: list of vendor_name and id
        */
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

        /*
         Developer: Azeem Hassan
         Date: 7-5-19 
         Action: getting vendor details address and repo by vendor id
         URL:  api/vendor/detailsadressandrep/103
         Request GET
         Input: vendor id
         output: dynamic AdressAndrepDetails object
       */
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

        /*
         Developer: Azeem Hassan
         Date: 7-5-19 
         Action: getting vendor note and document by vendor id
         URL:  api/vendor/getnoteanddoc/103
         Request GET
         Input: vendor id
         output: dynamic ExpandoObject vendorNoteAndDoc
       */
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

        /*
         Developer: Azeem Hassan
         Date: 7-5-19 
         Action: getting vendor address details by vendor id
         URL:  api/vendor/detailsaddress/103
         Request GET
         Input: vendor id
         output: dynamic AdressDetails object
       */
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

        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: getting vendor detailsrep by vendor id
            URL:  api/vendor/detailsrep/103
            Request GET
            Input: vendor id
            output: dynamic RepDetails object
        */
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

        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: getting single vendor by vendor id
            URL:  api/vendor/list/1
            Request GET
            Input: vendor id
            output: vendor
        */
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

        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: create new vendor with vendor and user event
            URL:  api/vendor/create
            Request POST
            Input: vendor form data
            output: vendor_id
        */
        // POST: api/vendor/create
        [HttpPost("create")]
        public async Task<ResponseModel> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            ResponseModel response;
            Vendor newVendor = JsonConvert.DeserializeObject<Vendor>(value);
            int created_by = newVendor.created_by;
            NewInsertionID = await _VendorRepo.Create(newVendor);
            response = new ResponseModel { Data = NewInsertionID, Status = System.Net.HttpStatusCode.OK, Message = "Success" };
            var eventModel = new EventModel(vendorEventTableName)
            {
                EventName = create_vendor,
                EntityId = Int32.Parse(NewInsertionID),
                RefrenceId = 0,
                UserId = created_by,
                EventNoteId = Int32.Parse(NewInsertionID)
            };
            await _eventRepo.AddEventAsync(eventModel);

            var userEvent = new EventModel(userEventTableName)
            {
                EventName = create_vendor,
                EntityId = created_by,
                RefrenceId = Convert.ToInt32(NewInsertionID),
                UserId = created_by,
                EventNoteId = Int32.Parse(NewInsertionID)
            };
            await _eventRepo.AddEventAsync(userEvent);
            return response;
        }

        /*
           Developer: Hamza Haq
           Date: 8-20-19 
           Action: create new vendor products with vendor and user event
           URL:  api/vendor/createvendorproduct
           Request POST
           Input: VendorProducts form data
           output: product_id
       */
        // POST: api/vendor/createendorproduct
        [HttpPost("createvendorproduct")]
        public async Task<string> PostAsyncVendorProduct([FromBody] string value)
        {
            string NewInsertionID = "0";
            try
            {
                VendorProducts newVendor = JsonConvert.DeserializeObject<VendorProducts>(value);
                int created_by = newVendor.created_by;
                NewInsertionID = await _vendorproductRepository.Create(newVendor);
                var eventModel = new EventModel(vendorEventTableName)
                {
                    EventName = create_vendor_product,
                    EntityId = newVendor.vendor_id,
                    RefrenceId = Int32.Parse(NewInsertionID),
                    UserId = created_by,
                    EventNoteId = Int32.Parse(NewInsertionID)
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EventName = create_vendor_product,
                    EntityId = created_by,
                    RefrenceId = Convert.ToInt32(NewInsertionID),
                    UserId = created_by,
                    EventNoteId = Int32.Parse(NewInsertionID)
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new vendor product with message" + ex.Message);
            }
            return NewInsertionID;

         
        }

        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: create vendor note with vendor and user event
           URL:  api/vendor/note/create
           Request POST
           Input: vendor note form data
           output: note id
        */
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
                var eventModel = new EventModel(vendorEventTableName)
                {
                    EventName = create_vendor_note,
                    EntityId = ref_id,
                    RefrenceId = Int32.Parse(newNoteID),
                    UserId = created_by,
                    EventNoteId = Int32.Parse(newNoteID)
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EventName = create_vendor_note,
                    EntityId = created_by,
                    RefrenceId = Convert.ToInt32(newNoteID),
                    UserId = created_by,
                    EventNoteId = Int32.Parse(newNoteID)
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new vendor with message" + ex.Message);
            }
            return newNoteID;
        }

        /*
          Developer: Azeem Hassan
          Date: 7-5-19 
          Action: create vendor document with vendor and user event
          URL:  api/vendor/documentcreate
          Request POST
          Input: vendor document form data
          output: document id
       */
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
                var eventModel = new EventModel(vendorEventTableName)
                {
                    EventName = create_vendor_document,
                    EntityId = ref_id,
                    RefrenceId = Int32.Parse(newDocID),
                    UserId = created_by,
                    EventNoteId = Int32.Parse(newDocID)
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EventName = create_vendor_document,
                    EntityId = created_by,
                    RefrenceId = Convert.ToInt32(newDocID),
                    UserId = created_by,
                    EventNoteId = Int32.Parse(newDocID)
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new vendor with message" + ex.Message);
            }
            return newDocID;
        }

        /*
          Developer: Azeem Hassan
          Date: 7-5-19 
          Action: update vendor with vendor and user event by vendor id
          URL:  api/vendor/update/5
          Request PUT
          Input: vendor_id and vendor updated form data
          output: update result
       */
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
                var eventModel = new EventModel(vendorEventTableName)
                {
                    EventName = update_vendor,
                    EntityId = id,
                    RefrenceId = id,
                    UserId = updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EventName = update_vendor,
                    EntityId = id,
                    RefrenceId = Convert.ToInt32(id),
                    UserId = updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(userEvent);

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
                if (VendorToUpdate.logo != null)
                {
                    ValuesToUpdate.Add("logo", VendorToUpdate.logo);
                }
                if (VendorToUpdate.has_note != 0)
                {
                    ValuesToUpdate.Add("has_note", VendorToUpdate.has_note.ToString());
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

        /*
           Developer: Hamza Haq
           Date: 8-22-19 
           Action: create new vendor products with vendor and user event
           URL:  api/vendor/vendorproductUpdatespecific
           Request PUT
           Input: VendorProducts form data
           output: product_id
       */
        // POST: api/vendor/vendorproductUpdatespecific
        [HttpPut("vendorproductUpdatespecific")]
        public async Task<string> vendorproductUpdatespecific([FromBody] string value)
        {

            string UpdateResult = "Success";

            try
            {
                VendorProducts VendorToUpdate = JsonConvert.DeserializeObject<VendorProducts>(value);
                VendorToUpdate.vendor_id = VendorToUpdate.vendor_id;
                VendorToUpdate.product_id = VendorToUpdate.product_id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
               
                if (!string.IsNullOrEmpty(VendorToUpdate.vendor_color_code))
                {
                    ValuesToUpdate.Add("vendor_color_code", VendorToUpdate.vendor_color_code);
                }
                if (!string.IsNullOrEmpty(VendorToUpdate.vendor_color_name))
                {
                    ValuesToUpdate.Add("vendor_color_name", VendorToUpdate.vendor_product_code);
                }
                if (!string.IsNullOrEmpty(VendorToUpdate.vendor_product_code))
                {
                    ValuesToUpdate.Add("vendor_product_code", VendorToUpdate.vendor_product_code);
                }
                if (!string.IsNullOrEmpty(VendorToUpdate.vendor_product_name))
                {
                    ValuesToUpdate.Add("vendor_product_name", VendorToUpdate.vendor_product_name);
                }
                
                await _vendorproductRepository.UpdateSpecific(ValuesToUpdate, "product_id ="+ VendorToUpdate.product_id + " and vendor_id=" + VendorToUpdate.vendor_id);
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

        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: get vendor last sku by vendor id
           URL:  api/vendor/skufamily/id
           Request GET
           Input: vendor_id
           output: vendor product
        */
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

        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: get all vendors types
           URL:  api/vendor/getvendortypes
           Request GET
           Input: null
           output: vendor types
        */
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

        /*
          Developer: Sajid Khan
          Date: 7-12-19 
          Action: Getting vendor name and id by search string
          URL:  api/vendor/getvendorsbycolumnname/columnname/search
          Request GET
          Input: string columnName, string search 
          output: list of vendor_name and id
        */
        [HttpGet("getvendorsbycolumnname/{columnName}/{search}")]
        public async Task<List<object>> GetVendorsByColumnName(string columnName, string search)
        {
            dynamic vendorDetails = new object();
            try
            {
                vendorDetails = await _VendorRepo.GetVendorsByColumnName(columnName, search);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return vendorDetails;

        }

        /*
          Developer: Azeem Hassan
          Date: 7-11-19 
          Action: checking vendor existance
          URL:  checkvendorcodeexist/vendorcode
          Request POST
          Input: vendorcode
          output: vendor existance massage
        */
        //GET: api/vendor/getvendornameandid
        [HttpGet("checkvendorcodeexist/{vendorcode}")]
        public async Task<List<object>> CheckVendorCodeExist(string vendorcode)
        {
            dynamic vendorExist = new object();
            try
            {
                vendorExist = await _VendorRepo.CheckVendorCodeExist(vendorcode);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return vendorExist;

        }

        /*
        Developer: Sajid Khan
        Date: 08-09-19 
        Action: Getting vendor data by vendor name
        URL:  api/vendor/GetVendor/vendor_name
        Request GET
        Input: string vendor
        output: dynamic list of vendor data 
        */
        [HttpGet("getvendor/{vendor}")]
        public async Task<List<object>> GetVendor(string vendor)
        {
            dynamic vendorDetails = new object();
            try
            {
                vendorDetails = await _VendorRepo.GetVendor(vendor);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return vendorDetails;

        }

        /*
       Developer: Sajid Khan
       Date: 08-09-19 
       Action: Getting product data by stylenumber
       URL:  api/vendor/getstylenumber/stylenumber
       Request GET
       Input: string vendor
       output: dynamic list of product data 
       */
        [HttpGet("getstylenumber/{stylenumber}")]
        public async Task<List<object>> GetStyleNumber(string stylenumber)
        {
            dynamic vendorDetails = new object();
            try
            {
                vendorDetails = await _VendorRepo.GetStyleNumber(stylenumber);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return vendorDetails;

        }
        
    }
}
