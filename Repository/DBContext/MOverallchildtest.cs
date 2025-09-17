using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MOverallchildtest
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public int TestId { get; set; }
        public int SectionId { get; set; }
        public double? OverallPercentage { get; set; }
        public string OverallPosition { get; set; }
        public string OverallComments { get; set; }
        public string OverallCommentstwo { get; set; }
        public string OverallCommenthree { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? AcademicYearId { get; set; }

        public virtual MAcademicyeardetail AcademicYear { get; set; }
        public virtual MChildinfo Child { get; set; }
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStandardsectionmapping Section { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual MSemestertestsmapping Test { get; set; }
    }
}
