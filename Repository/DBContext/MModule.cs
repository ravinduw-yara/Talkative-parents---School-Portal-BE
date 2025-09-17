using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MModule
    {
        public MModule()
        {
            MUsermodulemappings = new HashSet<MUsermodulemapping>();
        }

        public int Id { get; set; }
        public string State { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int? Parentid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public bool? Selected { get; set; }
        public string Type { get; set; }

        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MUsermodulemapping> MUsermodulemappings { get; set; }
    }
}
