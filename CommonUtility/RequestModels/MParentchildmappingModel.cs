using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MParentchildmappingModel
    {
        public int? Childid { get; set; }
        public int? Appuserid { get; set; }
        public int? Relationtypeid { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }

    public class MParentchildmappingUpdateModel : MParentchildmappingModel
    {
        public int Id { get; set; }
    }
}
