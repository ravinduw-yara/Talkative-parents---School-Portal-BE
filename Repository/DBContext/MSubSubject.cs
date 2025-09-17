using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MSubSubject
    {
        public MSubSubject()
        {
           
        }

        public int Id { get; set; }
        public string SubSubject { get; set; }
        public int? SubjectId { get; set; }
        public int? Precentage { get; set; }
        public int? SubMaxMarks { get; set; }
        public int? ExcelSheetOrder { get; set; }
        

    }
}
