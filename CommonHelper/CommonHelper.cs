using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommonHelper
{
    public class CommonHelper
    {
        
        public Double GetTimeStemp()
        {
            return (Double)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public double DateConvertToTimeStamp(string date)
        {
            Double TotalSeconds = 0;

            if (date != "") {
                string[] DateList = date.Split("/");

                DateTime newDateTime = new DateTime(Int32.Parse(DateList[2]), Int32.Parse(DateList[0]), Int32.Parse(DateList[1]), 0, 0, 0, DateTimeKind.Utc);
                DateTime secondsTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                TotalSeconds = (double)(newDateTime - secondsTime).TotalSeconds;
            }
            
            return TotalSeconds;
        }
        public string ConvertToDate(double unixtime)
        {
            return (new DateTime(1970, 1, 1).AddSeconds(unixtime)).ToString("M/d/yyyy");
        } 

        public string MultiDatePickerFormat(double starttime, double endtime)
        {
            return (new DateTime(1970, 1, 1).AddSeconds(starttime)).ToString("M/d") + "-" + (new DateTime(1970, 1, 1).AddSeconds(endtime)).ToString("M/d/yyyy");
        }

        public string NumberOfDays(double timestamp)
        {
            string NumOfDate = "0";
            if (timestamp > 0)
            {
                double TimeNow = (Double)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                DateTime StartDateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(TimeNow);
                DateTime EndDateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp);

                NumOfDate = (StartDateTime - EndDateTime).Days.ToString();
            }
            
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
