using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace badger_view.Helpers
{
    public class CommonHelper
    {
        private String BadgerAPIURL = "";
        private readonly IConfiguration _config;
        public CommonHelper(IConfiguration config)
        {

            _config = config;
            BadgerAPIURL = _config.GetValue<string>("Services:Badger");

        }

        public string ConvertToDate(double unixtime)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(unixtime)).ToString("M/d/yyyy");
        }

        public string NumberOfDays( double unixtime) {

            double TimeNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            DateTime StartDateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(unixtime);
            DateTime EndDateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(TimeNow);

            string NumOfDate = (EndDateTime - StartDateTime).Days.ToString();

            string TheDays = "Day";

            if (NumOfDate == "0" || NumOfDate == "1")
            {
                TheDays = "Day";
            }
            else
            {
                TheDays = "Days";
            }

            string aaa = NumOfDate + " " + TheDays;

            return aaa;
        }
    }
}
