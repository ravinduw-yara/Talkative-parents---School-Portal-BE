using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MStandardsectionmappingModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Parentid { get; set; }
        public int? Branchid { get; set; }
        public int? Businessunittypeid { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

    }
    public class MStandardsectionmappingUpdateModel : MStandardsectionmappingModel
    {
        public int Id { get; set; }
    }
}
