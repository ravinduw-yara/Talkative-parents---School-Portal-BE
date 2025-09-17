using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class Appuserdevice
    {
        public int Id { get; set; }
        public int? Appuserid { get; set; }
        public int? Groupid { get; set; }
        public string Deviceid { get; set; }
        public int? Devicetype { get; set; }
        public string Version { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

        public virtual MAppuserinfo Appuser { get; set; }
        public virtual MAppuserinfo CreatedbyNavigation { get; set; }
        public virtual MAppuserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatus Status { get; set; }
    }
}
