using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class TEmaillog
    {
        public int Id { get; set; }
        public string Fromemailid { get; set; }
        public int? Noticeboardmsgid { get; set; }
        public int? Emailcount { get; set; }
        public string Toemailid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MAppuserinfo CreatedbyNavigation { get; set; }
        public virtual MAppuserinfo ModifiedbyNavigation { get; set; }
        public virtual TNoticeboardmessage Noticeboardmsg { get; set; }
        public virtual MStatus Status { get; set; }
    }
}
