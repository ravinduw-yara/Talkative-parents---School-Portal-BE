using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class TSoundingboardmessage
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public bool? Isparentreplied { get; set; }
        public bool? Isstaffreplied { get; set; }
        public string Attachments { get; set; }
        public int? Commentscount { get; set; }
        public int? Standardsectionmappingid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? Appuserinfoid { get; set; }
        public int? Childinfoid { get; set; }
        public int? Categoryid { get; set; }
        public int? Groupid { get; set; }
        public bool? Didread { get; set; }

        public virtual MAppuserinfo Appuserinfo { get; set; }
        public virtual MCategory Category { get; set; }
        public virtual MChildinfo Childinfo { get; set; }
        public virtual MAppuserinfo CreatedbyNavigation { get; set; }
        public virtual MGroup Group { get; set; }
        public virtual MAppuserinfo ModifiedbyNavigation { get; set; }
        public virtual MStandardsectionmapping Standardsectionmapping { get; set; }
        public virtual MStatus Status { get; set; }
    }
}
