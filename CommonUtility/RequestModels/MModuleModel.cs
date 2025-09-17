using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MModuleModel
    {
        public string State { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        public int? Parentid { get; set; }
        public bool? Selected { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }

    public class MModuleUpdateModel : MModuleModel
    {
        public int Id { get; set; }
    }
}
