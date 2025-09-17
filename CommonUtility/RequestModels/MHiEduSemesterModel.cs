using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MHiEduSemesterModel
    {
        public string SemesterName { get; set; }
        public int? CourseId { get; set; }

    }
    public class MHiEduSemesterUpdateModel : MHiEduDepartmentModel
    {
        public int Id { get; set; }
    }
}
