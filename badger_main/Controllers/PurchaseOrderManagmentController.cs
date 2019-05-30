using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using badgerApi.Helper;
using badgerApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderManagmentController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPurchaseOrdersRepository _PurchaseOrdersRepo;
        ILoggerFactory _loggerFactory;
        private INotesAndDocHelper _NotesAndDoc;
        private int note_type = 4;
        public PurchaseOrderManagmentController(IPurchaseOrdersRepository PurchaseOrdersRepo, ILoggerFactory loggerFactory, INotesAndDocHelper NotesAndDoc, IConfiguration config)
        {
            _config = config;
            _PurchaseOrdersRepo = PurchaseOrdersRepo;
            _loggerFactory = loggerFactory;
            _NotesAndDoc = NotesAndDoc;
        }


    }
}