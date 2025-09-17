using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MChildtestmapping
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public int SubjectTestMappingId { get; set; }
        public int? Marks { get; set; }
        public string Comments { get; set; }
        public double? Percentage { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public bool IsAbsent { get; set; }

        public virtual MChildinfo Child { get; set; }
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual MSubjecttestmapping SubjectTestMapping { get; set; }
    }
}
