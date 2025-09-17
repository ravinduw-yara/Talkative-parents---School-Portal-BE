using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class MHiEduSubSubject
    {
        public MHiEduSubSubject()
        {

        }
        public int Id { get; set; }
        public string SubSubjectName { get; set; }
        public int SubjectId { get; set; }
        public int? Weight { get; set; }
        public int? Maxmarks { get; set; }


    }
}
