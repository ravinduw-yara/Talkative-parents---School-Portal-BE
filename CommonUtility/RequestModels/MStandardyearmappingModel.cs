using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MStandardyearmappingModel
    {
        public int AcademicYearId { get; set; }
        public int LevelId { get; set; }
        public int GradeId { get; set; }
        public int FreezeEnable { get; set; }
        public int SemesterId { get; set; }
        public int ExamId { get; set; }
    }
    public class MStandardyearmappingUpdateModel : MStandardyearmappingModel
    {
        public int Id { get; set; }
    }
}
