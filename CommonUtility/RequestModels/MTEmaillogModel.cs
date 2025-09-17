using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MTEmaillogModel
    {
        public string Fromemailid { get; set; }
        public int? Noticeboardmsgid { get; set; }
        public int? Emailcount { get; set; }
        public string Toemailid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

    }

    public class MTEmaillogUpdateModel : MTEmaillogModel
    {
        public int Id { get; set; }
    }
}
