using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MSubSubjectMarksModel
    {
        public int ChildTestMappingId { get; set; }
        public int SubSubjectId { get; set; }
        public int Marks { get; set; }
        public string Comment { get; set; }
        public double? SubPrecentage { get; set; }
        
    }
    public class MSubSubjectMarksUpdateModel : MSubSubjectMarksModel
    {
        public int Id { get; set; }
    }
}
