using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MTeacherSubjectMappingModel
    {
        public int? Teacherid { get; set; }
        public int? Subjectsectionmappingid { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? AcademicYearID { get; set; }


    }
    public class MTeacherSubjectMappingUpdateModel : MTeacherSubjectMappingModel
    {
        public int Id { get; set; }
    }
}
