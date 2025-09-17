using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MChildinfo
    {
        public MChildinfo()
        {
            MChildschoolmappings = new HashSet<MChildschoolmapping>();
            MChildtestmappings = new HashSet<MChildtestmapping>();
            MOverallchildtests = new HashSet<MOverallchildtest>();
            MParentchildmappings = new HashSet<MParentchildmapping>();
            MSubjectsemesterpercentages = new HashSet<MSubjectsemesterpercentage>();
            TNoticeboardmappings = new HashSet<TNoticeboardmapping>();
            TSoundingboardmessages = new HashSet<TSoundingboardmessage>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public DateTime? Dob { get; set; }
        public string Phonenumber { get; set; }
        public int? Genderid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public string Email { get; set; }
        public string StudentImageLink { get; set; }
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MGender Gender { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MChildschoolmapping> MChildschoolmappings { get; set; }
        public virtual ICollection<MChildtestmapping> MChildtestmappings { get; set; }
        public virtual ICollection<MOverallchildtest> MOverallchildtests { get; set; }
        public virtual ICollection<MParentchildmapping> MParentchildmappings { get; set; }
        public virtual ICollection<MSubjectsemesterpercentage> MSubjectsemesterpercentages { get; set; }
        public virtual ICollection<TNoticeboardmapping> TNoticeboardmappings { get; set; }
        public virtual ICollection<TSoundingboardmessage> TSoundingboardmessages { get; set; }
    }
}
