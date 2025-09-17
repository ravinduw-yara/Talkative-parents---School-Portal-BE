using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class HiEdu_BatchCourseMapping
    {
        public HiEdu_BatchCourseMapping()
        {

        }
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int CourseId { get; set; }

    }
}
