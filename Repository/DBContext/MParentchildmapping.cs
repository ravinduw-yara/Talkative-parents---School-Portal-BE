using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MParentchildmapping
    {
        public int Id { get; set; }
        public int? Childid { get; set; }
        public int? Appuserid { get; set; }
        public int? Relationtypeid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MAppuserinfo Appuser { get; set; }
        public virtual MChildinfo Child { get; set; }
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MRelationtype Relationtype { get; set; }
        public virtual MStatus Status { get; set; }
    }
}
