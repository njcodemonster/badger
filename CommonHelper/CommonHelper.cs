using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommonHelper
{
    public class CommonHelper
    {
        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Now Today Timestamp
        URL: 
        Request: Get
        Input:  
        output: double timesteamp
        */
        public Double GetTimeStemp()
        {
            return (Double)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static Double GetTimeStamp()
        {
            return (Double)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: date convert to timestamp
        URL: 
        Request: Get
        Input:  string date
        output: double timesteamp
        */
        public double DateConvertToTimeStamp(string date)
        {
            Double TotalSeconds = 0;

            if (date != "") {
                string[] DateList = date.Split("/");

                 int month = Int32.Parse(DateList[0]);
                   int day = Int32.Parse(DateList[1]);
                  int year = Int32.Parse(DateList[2]);
                
                DateTime newDateTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
                DateTime secondsTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                TotalSeconds = (double)(newDateTime - secondsTime).TotalSeconds;
            }
            
            return TotalSeconds;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: date convert to timestamp
        URL: 
        Request: Get
        Input:  string date
        output: double timesteamp
        */
        public string ConvertToDate(double unixtime)
        {
            return (new DateTime(1970, 1, 1).AddSeconds(unixtime)).ToString("M/d/yyyy");
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Convert Multi Date Picker Format 
        URL: 
        Request: Get
        Input:  double starttime, double endtime
        output: string multidateformat
        */
        public string MultiDatePickerFormat(double starttime, double endtime)
        {
            return (new DateTime(1970, 1, 1).AddSeconds(starttime)).ToString("M/d") + "-" + (new DateTime(1970, 1, 1).AddSeconds(endtime)).ToString("M/d/yyyy");
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Total number of days from now today date time
        URL: 
        Request: Get
        Input:  double date
        output: Number of day or days
        */
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
