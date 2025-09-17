using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MSubjectsectionmapping
    {
        public MSubjectsectionmapping()
        {
            MSubjectsemesterpercentages = new HashSet<MSubjectsemesterpercentage>();
            MSubjecttestmappings = new HashSet<MSubjecttestmapping>();
            MTeachersubjectmappings = new HashSet<MTeachersubjectmapping>();
        }

        public int Id { get; set; }
        public int SubjectId { get; set; }
      
        public int SectionId { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStandardsectionmapping Section { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual MSubject Subject { get; set; }
        public virtual ICollection<MSubjectsemesterpercentage> MSubjectsemesterpercentages { get; set; }
        public virtual ICollection<MSubjecttestmapping> MSubjecttestmappings { get; set; }
        public virtual ICollection<MTeachersubjectmapping> MTeachersubjectmappings { get; set; }
    }
}
