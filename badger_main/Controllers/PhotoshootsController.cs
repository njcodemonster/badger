using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using badgerApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;

namespace badgerApi.Controllers
{
    public class PhotoshootsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}