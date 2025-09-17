using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MSchooluserrole
    {
        public int Id { get; set; }
        public int? Schooluserid { get; set; }
        public int? Standardsectionmappingid { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? Categoryid { get; set; }
        public int? Groupid { get; set; }

        public virtual MCategory Category { get; set; }
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MGroup Group { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MSchooluserinfo Schooluser { get; set; }
        public virtual MStandardsectionmapping Standardsectionmapping { get; set; }
        public virtual MStatus Status { get; set; }
    }
}
