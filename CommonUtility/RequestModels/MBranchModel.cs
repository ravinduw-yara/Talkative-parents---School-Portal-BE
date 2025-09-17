using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MBranchModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Schoolid { get; set; }
        public string Principalname { get; set; }
        public string Address { get; set; }
        public int? Pincode { get; set; }
        public int? Locaionid { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }

    public class MBranchUpdateModel : MBranchModel
    {
        public int Id { get; set; }
    }
}
