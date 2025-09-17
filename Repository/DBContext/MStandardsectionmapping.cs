using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MStandardsectionmapping
    {
        public MStandardsectionmapping()
        {
            MChildschoolmappings = new HashSet<MChildschoolmapping>();
            MOverallchildtests = new HashSet<MOverallchildtest>();
            MSchooluserroles = new HashSet<MSchooluserrole>();
            MStandardgroupmappings = new HashSet<MStandardgroupmapping>();
            MSubjectsectionmappings = new HashSet<MSubjectsectionmapping>();
            MSubjectsemesterpercentages = new HashSet<MSubjectsemesterpercentage>();
            MTestsectionmappings = new HashSet<MTestsectionmapping>();
            TCalendereventdetails = new HashSet<TCalendereventdetail>();
            TGclteacherclasses = new HashSet<TGclteacherclass>();
            TGclvedioclasses = new HashSet<TGclvedioclass>();
            TGoogleclasses = new HashSet<TGoogleclass>();
            TNoticeboardmessages = new HashSet<TNoticeboardmessage>();
            TSoundingboardmessages = new HashSet<TSoundingboardmessage>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Parentid { get; set; }
        public int? Branchid { get; set; }
        public int? LevelID { get; set; }
        public int? Businessunittypeid { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MBranch Branch { get; set; }
        public virtual MBusinessunittype Businessunittype { get; set; }
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MChildschoolmapping> MChildschoolmappings { get; set; }
        public virtual ICollection<MOverallchildtest> MOverallchildtests { get; set; }
        public virtual ICollection<MSchooluserrole> MSchooluserroles { get; set; }
        public virtual ICollection<MStandardgroupmapping> MStandardgroupmappings { get; set; }
        public virtual ICollection<MSubjectsectionmapping> MSubjectsectionmappings { get; set; }
        public virtual ICollection<MSubjectsemesterpercentage> MSubjectsemesterpercentages { get; set; }
        public virtual ICollection<MTestsectionmapping> MTestsectionmappings { get; set; }
        public virtual ICollection<TCalendereventdetail> TCalendereventdetails { get; set; }
        public virtual ICollection<TGclteacherclass> TGclteacherclasses { get; set; }
        public virtual ICollection<TGclvedioclass> TGclvedioclasses { get; set; }
        public virtual ICollection<TGoogleclass> TGoogleclasses { get; set; }
        public virtual ICollection<TNoticeboardmessage> TNoticeboardmessages { get; set; }
        public virtual ICollection<TSoundingboardmessage> TSoundingboardmessages { get; set; }
    }
}
