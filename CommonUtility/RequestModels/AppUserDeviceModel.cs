using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class AppUserDevice
    {
        public int? Appuserid { get; set; }
        public int? Groupid { get; set; }
        public string Deviceid { get; set; }
        public int? Devicetype { get; set; }
        public string Version { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }
    
    public class AppUserDeviceUpdateModel : AppUserDevice
    {
        public int Id { get; set; }
    }


    public class FeedbackModel
    {
        public string Feedback { get; set; }
    }

}
