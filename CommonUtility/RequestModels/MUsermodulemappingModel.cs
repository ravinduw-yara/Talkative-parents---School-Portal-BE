using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MUsermodulemappingModel
    {
        public int? Schooluserid { get; set; }
        public int? Moduleid { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }

    public class MUsermodulemappingUpdateModel : MUsermodulemappingModel
    {
        public int Id { get; set; }
    }
}
