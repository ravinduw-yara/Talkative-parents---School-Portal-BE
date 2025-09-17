using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class HiEdu_ChildBatchCourseMapping
    {
        public HiEdu_ChildBatchCourseMapping()
        {

        }
        public int Id { get; set; }
        public int BatchCourseMappingId { get; set; }
        public int ChildId { get; set; }

    }
}
