using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class TToken
    {
        public Guid Id { get; set; }
        public int? Referenceid { get; set; }
        public string Usertype { get; set; }
        public DateTime? Ttl { get; set; }
        public int? Statusid { get; set; }
        public string Ipaddress { get; set; }

        public virtual MStatus Status { get; set; }
    }
}
