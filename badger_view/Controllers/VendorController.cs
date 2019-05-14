using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace badger_view.Controllers
{
    public class VendorController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}