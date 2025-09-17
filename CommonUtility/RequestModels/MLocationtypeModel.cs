using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MLocationtypeModel
    {
        public string Type { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }
    public class MLocationtypeUpdateModel : MLocationtypeModel
    {
        public int Id { get; set; }
    }
}
