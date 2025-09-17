using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class MHiEduCourses
    {
        public MHiEduCourses()
        {
            //  MChildschoolmappings = new HashSet<MChildschoolmapping>();
         
          //temporary commented  MHieduSemesterCourseMappings = new HashSet<MHieduSemesterCourseMapping>();
        }
        public int? Id { get; set; }
        public string Course { get; set; }
        public int SchoolId { get; set; }
        public int DepartmentId { get; set; }
        public string Duration { get; set; }

    }
}
