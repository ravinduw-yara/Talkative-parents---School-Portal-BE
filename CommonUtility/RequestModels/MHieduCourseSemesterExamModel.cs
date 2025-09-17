using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MHiEduCourseSemesterExamModel
    {


        public string Exam { get; set; }
        public int? SemesterCourseMappingId { get; set; }



    }
    public class MHieduCourseSemesterExamUpdateModel : MHiEduCourseSemesterExamModel
    {
        public int Id { get; set; }
    }
}