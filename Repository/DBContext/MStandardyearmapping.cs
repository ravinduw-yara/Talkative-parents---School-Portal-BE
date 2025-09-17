using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MStandardyearmapping
    {
        public MStandardyearmapping()
        {
            
        }

        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public int LevelId { get; set; }  
        public int GradeId { get; set; }
        public int FreezeEnable { get; set; }
        public int SemesterId { get; set; }
        public int ExamId { get; set; }

    }
}
