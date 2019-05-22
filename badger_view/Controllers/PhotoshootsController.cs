using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace badger_view.Controllers
{
    public class PhotoshootsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult inProgress()
        {
            return View("inprogress");
        }

        public IActionResult shootInProgress(int photoshootId)
        {
            return View("shootInProgress");
        }

        public IActionResult sendToEditor(int photoshootId)
        {
            return View("sendToEditor");
        }

    }
}