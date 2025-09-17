using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class TNoticeboardmessage
    {
        public TNoticeboardmessage()
        {
            TEmaillogs = new HashSet<TEmaillog>();
            TNoticeboardmappings = new HashSet<TNoticeboardmapping>();
        }

        public int Id { get; set; }
        public int? Schooluserid { get; set; }
        public int? Branchid { get; set; }
        public int? Standardsectionmappingid { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Attachments { get; set; }
        public string Sms { get; set; }
        public bool? Ispriority { get; set; }
        public bool? Isemail { get; set; }
        public bool? IsEntireSchool { get; set; }
        public bool? IsParticularClass { get; set; }
        public bool? IsParticularSection { get; set; }
        public bool? IsParticularParent { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public string OneSignal { get; set; }

        public virtual MBranch Branch { get; set; }
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MSchooluserinfo Schooluser { get; set; }
        public virtual MStandardsectionmapping Standardsectionmapping { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<TEmaillog> TEmaillogs { get; set; }
        public virtual ICollection<TNoticeboardmapping> TNoticeboardmappings { get; set; }
    }
}
