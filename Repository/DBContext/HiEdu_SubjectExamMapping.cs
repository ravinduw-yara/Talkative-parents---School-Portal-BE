using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class HiEdu_SubjectExamMapping
    {
        public HiEdu_SubjectExamMapping()
        {

        }
        public int Id { get; set; }
        public string Subject { get; set; }
        public int CourseSemesterExamId { get; set; }

    }
}
