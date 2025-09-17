using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MLocationtype
    {
        public MLocationtype()
        {
            MLocations = new HashSet<MLocation>();
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MLocation> MLocations { get; set; }
    }
}
