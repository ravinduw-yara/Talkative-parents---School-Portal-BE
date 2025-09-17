using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class TGoogleclass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gcourseid { get; set; }
        public string Gownerid { get; set; }
        public string Teacherfolderid { get; set; }
        public int? Standardsectionmappingid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStandardsectionmapping Standardsectionmapping { get; set; }
        public virtual MStatus Status { get; set; }
    }
}
