using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class VDashboardcount
    {
        public int? Childid { get; set; }
        public int? Parentid { get; set; }
        public int Schoolid { get; set; }
    }
}
