using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MRelationtypeModel
    {
        public string Type { get; set; }
        public string Icon { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }

    public class MRelationtypeUpdateModel : MRelationtypeModel
    {
        public int Id { get; set; }
    }
}
