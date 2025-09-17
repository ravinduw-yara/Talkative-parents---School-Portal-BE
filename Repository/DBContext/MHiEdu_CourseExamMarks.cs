using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class MHiEdu_CourseExamMarks
    {
        public MHiEdu_CourseExamMarks()
        {

        }
        public int Id { get; set; }
        public decimal Marks { get; set; }
        public int ChildId { get; set; }
        public int? SubjectExamMappingId { get; set; }

    }
}
