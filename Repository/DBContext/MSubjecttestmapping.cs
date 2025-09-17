using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MSubjecttestmapping
    {
        public MSubjecttestmapping()
        {
            MChildtestmappings = new HashSet<MChildtestmapping>();
        }

        public int Id { get; set; }
        public int SubjectSectionMappingId { get; set; }
        public int TestId { get; set; }
        public int? MaxMarks { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? SemesterYearMappingId { get; set; }

        public virtual MSemesteryearmapping SemesterYearMapping { get; set; }
        public virtual MSubjectsectionmapping SubjectSectionMapping { get; set; }
        public virtual MSemestertestsmapping Test { get; set; }
        public virtual ICollection<MChildtestmapping> MChildtestmappings { get; set; }
    }
}
