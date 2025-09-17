using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MChildschoolmapping
    {
        public int Id { get; set; }
        public int? Childid { get; set; }
        public int? Standardsectionmappingid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public string Registerationnumber { get; set; }
        public int? AcademicYearId { get; set; }
        public int? DRCEnable1 { get; set; }
        public int? DRCEnable2 { get; set; }
        public int? DRCEnable3 { get; set; }
        public int? Promoted { get; set; }
        


        public virtual MAcademicyeardetail AcademicYear { get; set; }
        public virtual MChildinfo Child { get; set; }
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStandardsectionmapping Standardsectionmapping { get; set; }
        public virtual MStatus Status { get; set; }
    }
}
