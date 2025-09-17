using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class HiEdu_SemesterCourseMapping
    {
        public HiEdu_SemesterCourseMapping()
        {

        }
        public int Id { get; set; }
        public string SemesterName { get; set; }
        public int? CourseId { get; set; }

    }
}
