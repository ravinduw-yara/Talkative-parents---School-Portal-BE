using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MSubjectsemesterpercentage
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public int SubjectSectionMappingId { get; set; }
        public int StandardId { get; set; }
        public int SemesterId { get; set; }
        public double? SubjectSemesterPercentage { get; set; }
        public string OverallSemesterPosition { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MChildinfo Child { get; set; }
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MSemestertestsmapping Semester { get; set; }
        public virtual MStandardsectionmapping Standard { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual MSubjectsectionmapping SubjectSectionMapping { get; set; }
    }
}
