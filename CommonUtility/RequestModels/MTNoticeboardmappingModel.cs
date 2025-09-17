using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MTNoticeboardmappingModel
    {
        public int? Noticeboardmsgid { get; set; }
        public int? Schooluserid { get; set; }
        public int? Groupbusinessunitmappingid { get; set; }
        public int? Appuserid { get; set; }

        //public DateTime? Createddate { get; set; }
        //public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }

    public class MTNoticeboardmappingUpdateModel : MTNoticeboardmappingModel
    {
        public int Id { get; set; }
    }
}
