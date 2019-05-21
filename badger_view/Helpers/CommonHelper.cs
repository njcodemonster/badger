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
        private readonly IConfiguration _config;
        public CommonHelper(IConfiguration config)
        {

            _config = config;

        }

        public Double GetTimeStemp()
        {
          return  (Double)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public string ConvertToDate(double unixtime)
        {
            return (new DateTime(1970, 1, 1).AddSeconds(unixtime)).ToString("M/d/yyyy");
        }

        public string MultiDatePickerFormat(double starttime, double endtime)
        {
            return (new DateTime(1970, 1, 1).AddSeconds(starttime)).ToString("M/d")+"-"+ (new DateTime(1970, 1, 1).AddSeconds(endtime)).ToString("M/d/yyyy");
        }

        public string NumberOfDays( double timestamp) {

            double TimeNow = (Double)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            DateTime StartDateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(TimeNow);
            DateTime EndDateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp);

            string NumOfDate = (StartDateTime - EndDateTime).Days.ToString();

            string TheDays = "Day";

            if (NumOfDate == "0" || NumOfDate == "1")
            {
                TheDays = "Day";
            }
            else
            {
                TheDays = "Days";
            }

            return NumOfDate + " " + TheDays;
        }
    }
}
