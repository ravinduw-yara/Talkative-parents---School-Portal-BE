using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MBusinessunit
    {
        public MBusinessunit()
        {
            MChildschoolmappings = new HashSet<MChildschoolmapping>();
            MGroups = new HashSet<MGroup>();
            MSchooluserroles = new HashSet<MSchooluserrole>();
            TCalendereventdetails = new HashSet<TCalendereventdetail>();
            TGclvedioclasses = new HashSet<TGclvedioclass>();
            TGoogleclasses = new HashSet<TGoogleclass>();
            TNoticeboardmappings = new HashSet<TNoticeboardmapping>();
            TSoundingboardmessages = new HashSet<TSoundingboardmessage>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Parentid { get; set; }
        public int? Branchid { get; set; }
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
        public virtual ICollection<MGroup> MGroups { get; set; }
        public virtual ICollection<MSchooluserrole> MSchooluserroles { get; set; }
        public virtual ICollection<TCalendereventdetail> TCalendereventdetails { get; set; }
        public virtual ICollection<TGclvedioclass> TGclvedioclasses { get; set; }
        public virtual ICollection<TGoogleclass> TGoogleclasses { get; set; }
        public virtual ICollection<TNoticeboardmapping> TNoticeboardmappings { get; set; }
        public virtual ICollection<TSoundingboardmessage> TSoundingboardmessages { get; set; }
    }
}
