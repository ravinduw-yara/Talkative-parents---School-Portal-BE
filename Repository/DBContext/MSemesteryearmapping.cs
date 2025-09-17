using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MSemesteryearmapping
    {
        public MSemesteryearmapping()
        {
            MSubjecttestmappings = new HashSet<MSubjecttestmapping>();
        }

        public int Id { get; set; }
        public int SemesterId { get; set; }
        public int AcademicYearId { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MAcademicyeardetail AcademicYear { get; set; }
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MSemestertestsmapping Semester { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MSubjecttestmapping> MSubjecttestmappings { get; set; }
    }
}
