using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class TClassCalenderevents
    {
        public int Id { get; set; }
        public int? sectionId { get; set; }
        public int? schoolid { get; set; }
        public int? calendereventid { get; set; }
    }
}
