using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace badger_view.Controllers
{
    public class PreferenceController : Controller
    {
      
        private readonly IConfiguration _config;
        private String JsonFilePath = "";
        public IActionResult Index()
        {
            Read("preference.json");
            return View();
        }


        public PreferenceController(IConfiguration config) {
            _config = config;
             JsonFilePath = _config.GetValue<string>("JsonPath:preference");
        }

        public string Read(string fileName)
        {
            //string root = "wwwroot";
            string FileFullPath = JsonFilePath + fileName;



            string jsonResult;

            using (StreamReader streamReader = new StreamReader(FileFullPath))
            {
                jsonResult = streamReader.ReadToEnd();
            }
            return jsonResult;
        }

        public void Write(string fileName, string location, string jSONString)
        {
            string FileFullPath = JsonFilePath + fileName;

          ///  using (var streamWriter = File.CreateText(FileFullPath))
         //  {
           //     streamWriter.Write(jSONString);
           //}
        }
    }
}



