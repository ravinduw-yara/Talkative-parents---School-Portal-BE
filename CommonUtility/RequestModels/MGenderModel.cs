using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MGenderModel
    {
        public string Type { get; set; }
        public string Icon { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }

    public class MGenderUpdateModel : MGenderModel
    {
        public int Id { get; set; }
    }
}
