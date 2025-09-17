using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MAdmininfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Emailid { get; set; }
        public string Phonenumber { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Statusid { get; set; }
    }
}
