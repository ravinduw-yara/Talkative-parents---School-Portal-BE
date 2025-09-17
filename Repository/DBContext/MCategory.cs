using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MCategory
    {
        public MCategory()
        {
            MSchooluserroles = new HashSet<MSchooluserrole>();
            TSoundingboardmessages = new HashSet<TSoundingboardmessage>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Roleid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MRole Role { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MSchooluserrole> MSchooluserroles { get; set; }
        public virtual ICollection<TSoundingboardmessage> TSoundingboardmessages { get; set; }
    }
}
