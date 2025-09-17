using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MHiEduCoursesModel
    {
        public int? CourseId { get; set; }
        public string Course { get; set; }
        public int SchoolId { get; set; }
        public int DepartmentId { get; set; }
        public string Duration { get; set; }

    }
    public class MHieducoursesUpdateModel : MHiEduCoursesModel
    {
        public int Id { get; set; }
    }
}