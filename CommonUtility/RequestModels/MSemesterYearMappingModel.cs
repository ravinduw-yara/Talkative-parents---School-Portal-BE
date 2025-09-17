using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MSemesterYearMappingModel
    {
        public int? SemesterId { get; set; }
        public int? AcademicYearId { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }
    public class MSemesterYearMappingUpdateModel : MSemesterYearMappingModel
    {
        public int Id { get; set; }
    }
}
