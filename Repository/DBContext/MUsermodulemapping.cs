using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MUsermodulemapping
    {
        public int Id { get; set; }
        public int? Schooluserid { get; set; }
        public int? Moduleid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MModule Module { get; set; }
        public virtual MSchooluserinfo Schooluser { get; set; }
        public virtual MStatus Status { get; set; }
    }
}
