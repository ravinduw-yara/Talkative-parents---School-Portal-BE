using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MFeature
    {
        public int Id { get; set; }
        public int? Schoolid { get; set; }
        public int? Maxmsgcount { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public bool? Isgcl { get; set; }
        public string Mask { get; set; }

        public virtual MAppuserinfo CreatedbyNavigation { get; set; }
        public virtual MAppuserinfo ModifiedbyNavigation { get; set; }
        public virtual MSchool School { get; set; }
        public virtual MStatus Status { get; set; }
    }
}
