using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MGender
    {
        public MGender()
        {
            MAppuserinfos = new HashSet<MAppuserinfo>();
            MChildinfos = new HashSet<MChildinfo>();
            MSchooluserinfos = new HashSet<MSchooluserinfo>();
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public string Icon { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MAppuserinfo> MAppuserinfos { get; set; }
        public virtual ICollection<MChildinfo> MChildinfos { get; set; }
        public virtual ICollection<MSchooluserinfo> MSchooluserinfos { get; set; }
    }
}
