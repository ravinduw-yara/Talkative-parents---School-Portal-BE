using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MBuisnessunittypeModel
    {
        public string Type { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }
    public class MBuisnessunittypeUpdateModel : MBuisnessunittypeModel
    {
        public int Id { get; set; }
    }

}
