using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MAppuserinfoModel
    {
        public string Code { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public DateTime? Dob { get; set; }
        public string Phonenumber { get; set; }
        public int? Genderid { get; set; }
        public string Password { get; set; }
        public string Emailid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public bool? Issmsuser { get; set; }
        public bool? Isofferoptedin { get; set; }
    }
    public class MAppuserinfoUpdateModel : MAppuserinfoModel
    {
        public int Id { get; set; }
    }
}
