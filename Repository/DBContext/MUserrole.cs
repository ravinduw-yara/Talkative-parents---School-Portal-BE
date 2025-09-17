using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MUserrole
    {
        public int Id { get; set; }
        public int? Userid { get; set; }
        public int? Roleid { get; set; }
        public int? Businessunitid { get; set; }
        public int? Childid { get; set; }
        public int? Schoolid { get; set; }
        public int? Relationtypeid { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MStandardsectionmapping Standardsectionmapping { get; set; }
        public virtual MChildinfo Child { get; set; }
        public virtual MUserinfo CreatedbyNavigation { get; set; }
        public virtual MUserinfo ModifiedbyNavigation { get; set; }
        public virtual MRelationtype Relationtype { get; set; }
        public virtual MRole Role { get; set; }
        public virtual MSchool School { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual MUserinfo User { get; set; }
    }
}
