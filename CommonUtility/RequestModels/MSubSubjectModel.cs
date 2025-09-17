using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MSubSubjectModel
    {

        public string SubSubjectName { get; set; }
        public int? SubjectId { get; set; }
        public int? Percentage { get; set; }
        public int? SubMaxMarks { get; set; }
        public int? ExcelSheetOrder { get; set; }
        



    }
    public class MSubSubjectUpdateModel : MSubSubjectModel
    {
        public int Id { get; set; }
    }
}
