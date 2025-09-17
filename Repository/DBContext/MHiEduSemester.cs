using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MHiEduSemester
    {
        public MHiEduSemester()
        {
            //      MSchools = new HashSet<MSchool>();
            //      MSubjectsectionmappings = new HashSet<MSubjectsectionmapping>();
        }

        public int Id { get; set; }

        public string SemesterName { get; set; }
        public int? CourseId { get; set; }



        //  public virtual MHiEduCourses MHieducourses { get; set; }

        //   public virtual ICollection<Mlevelsectionmapping> Mlevelsectionmappings { get; set; }
        //    public virtual ICollection<MSchool> MSchools { get; set; }

        //  public virtual MStatus Status { get; set; }
        //    public virtual ICollection<MSubjectsectionmapping> MSubjectsectionmappings { get; set; }
    }
}
