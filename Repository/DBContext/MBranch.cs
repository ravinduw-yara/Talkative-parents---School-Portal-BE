using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MBranch
    {
        public MBranch()
        {
            MSchooluserinfos = new HashSet<MSchooluserinfo>();
            MSemestertestsmappings = new HashSet<MSemestertestsmapping>();
            MStandardsectionmappings = new HashSet<MStandardsectionmapping>();
            MSubjects = new HashSet<MSubject>();
            TGclteacherclasses = new HashSet<TGclteacherclass>();
            TNoticeboardmessages = new HashSet<TNoticeboardmessage>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Schoolid { get; set; }
        public string Principalname { get; set; }
        public string Address { get; set; }
        public int? Pincode { get; set; }
        public int? Locaionid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MLocation Locaion { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MSchool School { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MSchooluserinfo> MSchooluserinfos { get; set; }
        public virtual ICollection<MSemestertestsmapping> MSemestertestsmappings { get; set; }
        public virtual ICollection<MStandardsectionmapping> MStandardsectionmappings { get; set; }
        public virtual ICollection<MSubject> MSubjects { get; set; }
        public virtual ICollection<TGclteacherclass> TGclteacherclasses { get; set; }
        public virtual ICollection<TNoticeboardmessage> TNoticeboardmessages { get; set; }
    }
}
