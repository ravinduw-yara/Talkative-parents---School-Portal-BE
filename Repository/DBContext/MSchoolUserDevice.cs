using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class MSchoolUserDevice
    {
            public int Id { get; set; }
            public int? SchoolUserid { get; set; }
            public int? Groupid { get; set; }
            public string Deviceid { get; set; }
            public int? Devicetype { get; set; }
            public string Version { get; set; }
            public DateTime? Createddate { get; set; }
            public DateTime? Modifieddate { get; set; }
            public int? Createdby { get; set; }
            public int? Modifiedby { get; set; }
            public int? Statusid { get; set; }

        
    }
}
