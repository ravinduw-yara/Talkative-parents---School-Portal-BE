using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MSemestertestsmapping
    {
        public MSemestertestsmapping()
        {
            MOverallchildtests = new HashSet<MOverallchildtest>();
            MSemesteryearmappings = new HashSet<MSemesteryearmapping>();
            MSubjectsemesterpercentages = new HashSet<MSubjectsemesterpercentage>();
            MSubjecttestmappings = new HashSet<MSubjecttestmapping>();
            MTestsectionmappings = new HashSet<MTestsectionmapping>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? SemesterId { get; set; }
        public int BranchId { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MBranch Branch { get; set; }
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MOverallchildtest> MOverallchildtests { get; set; }
        public virtual ICollection<MSemesteryearmapping> MSemesteryearmappings { get; set; }
        public virtual ICollection<MSubjectsemesterpercentage> MSubjectsemesterpercentages { get; set; }
        public virtual ICollection<MSubjecttestmapping> MSubjecttestmappings { get; set; }
        public virtual ICollection<MTestsectionmapping> MTestsectionmappings { get; set; }
    }
}
