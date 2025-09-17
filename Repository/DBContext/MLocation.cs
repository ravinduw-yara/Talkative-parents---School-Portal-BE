using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MLocation
    {
        public MLocation()
        {
            MBranches = new HashSet<MBranch>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? Parentid { get; set; }
        public int? Locationtypeid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MLocationtype Locationtype { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MBranch> MBranches { get; set; }
    }
}
