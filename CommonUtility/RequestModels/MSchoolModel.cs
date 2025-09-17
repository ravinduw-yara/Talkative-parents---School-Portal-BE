using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MSchoolModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Websitelink { get; set; }
        public string Emailid { get; set; }
        public string Ipgurl { get; set; }
        public string Primaryphonenumber { get; set; }
        public string Secondaryphonenumber { get; set; }
        public int? Staffcount { get; set; }
        public string Logo { get; set; }
        public bool? Allowcategory { get; set; }
        public bool? Issbsms { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        //public int? ismigrated { get; set; }
    }
    public class MSchoolUpdateModel : MSchoolModel
    {
        public int Id { get; set; }
    }
}
