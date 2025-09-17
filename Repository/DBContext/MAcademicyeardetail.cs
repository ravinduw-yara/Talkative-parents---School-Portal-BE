using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MAcademicyeardetail
    {
        public MAcademicyeardetail()
        {
            MChildschoolmappings = new HashSet<MChildschoolmapping>();
            MOverallchildtests = new HashSet<MOverallchildtest>();
            MSemesteryearmappings = new HashSet<MSemesteryearmapping>();
        }

        public int Id { get; set; }
        public string YearName { get; set; }
        public int SchoolId { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? Currentyear { get; set; }

        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MSchool School { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MChildschoolmapping> MChildschoolmappings { get; set; }
        public virtual ICollection<MOverallchildtest> MOverallchildtests { get; set; }
        public virtual ICollection<MSemesteryearmapping> MSemesteryearmappings { get; set; }
    }
}
