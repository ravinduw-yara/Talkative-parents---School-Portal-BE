using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
       public partial class HiEdu_OverallChildTests
    {
        public HiEdu_OverallChildTests()
        {

        }
        public int Id { get; set; }
        public int ChildId { get; set; }
        public int BatchCourseMappingId { get; set; }
        public int OverallBatchPostition { get; set; }
        public decimal OverallSemesterExamPrecentage { get; set; }
        public string OverallComment { get; set; }

    }
}
