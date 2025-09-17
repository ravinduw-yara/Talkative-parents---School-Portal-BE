using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class MHiEduCourseSemesterExam
    {
        public MHiEduCourseSemesterExam()
        {
            //  MChildschoolmappings = new HashSet<MChildschoolmapping>();
            //temporary commented   MHieduSubjectExamMappings = new HashSet<MHieduSubjectExamMapping>();
        }
        public int Id { get; set; }
        public string Exam { get; set; }
        public int? SemesterCourseMappingId { get; set; }

        //temporary commented   public virtual MHieduSemesterCourseMapping SemesterCourseMapping { get; set; }
        //temporary commented  public virtual ICollection<MHieduSubjectExamMapping> MHieduSubjectExamMappings { get; set; }
    }
}
