using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MTeachersubjectmapping
    {
        public int Id { get; set; }
        public int? TeacherId { get; set; }
        public int? SubjectSectionId { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? AcademicYearID { get; set; }
        public int? SubjectId { get; set; }
        public int? SectionId { get; set; }


        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual MSubjectsectionmapping SubjectSection { get; set; }
        public virtual MSchooluserinfo Teacher { get; set; }
    }
}
