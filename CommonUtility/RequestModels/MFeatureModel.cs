using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MFeatureModel
    {
        public int? Schoolid { get; set; }
        public int? Maxmsgcount { get; set; }
        //public DateTime? Createddate { get; set; }
        //public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

    }

    public class MFeatureUpdateModel : MFeatureModel
    {
        public int Id { get; set; }
    }
}
