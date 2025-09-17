using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class MHiEduBatch
    {
        public MHiEduBatch()
        {
            //  MChildschoolmappings = new HashSet<MChildschoolmapping>();
        }
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public string Batch { get; set; }
        public int Created_Year { get; set; }
        public int Created_Month { get; set; }
        public int Created_Date { get; set;}
        public DateTime Batch_Created_Date { get; set; }
        public int StudentCount { get; set; }
        public string Status { get; set; }
        public virtual MHiEduCourses Course { get; set; }
    }
}
