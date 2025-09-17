using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public partial class MAcademicyeardetailModel
    {


        public string YearName { get; set; }
        public int SchoolId { get; set; }
        public int? CurrentYear { get; set; }
        //public DateTime? Createddate { get; set; }
        //public DateTime? Modifieddate { get; set; }
        //public int? Createdby { get; set; }
        //public int? Modifiedby { get; set; }
        //public int? Statusid { get; set; }

        public class MAcademicyeardetailUpdateModel : MAcademicyeardetailModel
        {
            public int Id { get; set; }
        }




    }
}

