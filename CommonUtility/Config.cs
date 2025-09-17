using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility
{
    public static class Config
    {
        //Notification
       
          //public static string osAppId = "ae27641b-5e56-4d18-887c-c81f96cdc197";
          //public static string osAuth = "ODU4MGQzYTgtNDhiMi00OGZhLWIyYmQtNjNiMTE4OTQzZDY4";
        public static string osAppId = "8c09536b-71d0-4ee5-b085-dfe25387cdb9"; //old onsignal
      //  public static string osAuth = "MjIxZmM5MDEtYjY4NS00ZmJkLTlkOWEtMTZmNWI0YmU5OWMz";// old onsignal
        //public static string osAuth = "NjljZDQwNGEtZDJkNi00MjRiLWFiOGYtMjBjZTFlYjFiZmU1";// old onsignal 24/8/2024
        public static string osAuth = "YWVmYmY5NjYtZTdkNS00NjBmLWE3ZTItNzMxMDg2ZjJkZDc3";// old onsignal 24/8/2024

        public static string smsusers = "";
        public static string emailusers = "";
        public static bool isErrorFound = false;
        public static string mask = "GETTALKTIVE";
        public static string strOneSignalResponse = "";

        public static void ResetAll()
        {
            smsusers = "";
            emailusers = "";
            isErrorFound = false;
            strOneSignalResponse = "";
        }
    }
}
