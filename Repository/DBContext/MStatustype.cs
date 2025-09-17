using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MStatustype
    {
        public MStatustype()
        {
            MStatuses = new HashSet<MStatus>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public DateTime? Enddate { get; set; }

        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual ICollection<MStatus> MStatuses { get; set; }
    }
}
