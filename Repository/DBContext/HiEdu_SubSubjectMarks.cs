using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class HiEdu_SubSubjectMarks
{
        public HiEdu_SubSubjectMarks()
        {

        }
        public int Id { get; set; }
        public int? CourseExamMarksId { get; set; }
        public int SubSubjectId { get; set; }
        public int SubSubjectMarks { get; set; }
        public string Comment { get; set; }
        public int? IsAbsent { get; set; }

    }
}

