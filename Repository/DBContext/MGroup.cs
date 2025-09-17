using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MGroup
    {
        public MGroup()
        {
            MSchooluserroles = new HashSet<MSchooluserrole>();
            MStandardgroupmappings = new HashSet<MStandardgroupmapping>();
            TSoundingboardmessages = new HashSet<TSoundingboardmessage>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? Schoolid { get; set; }

        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MSchool School { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MSchooluserrole> MSchooluserroles { get; set; }
        public virtual ICollection<MStandardgroupmapping> MStandardgroupmappings { get; set; }
        public virtual ICollection<TSoundingboardmessage> TSoundingboardmessages { get; set; }
    }
}
