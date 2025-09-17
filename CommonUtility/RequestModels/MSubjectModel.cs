using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MSubjectModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Branchid { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? Schoolid { get; set; }
        public int? DRCSubjectOrder { get; set; }
        
    }
    public class MSubjectUpdateModel : MSubjectModel
    {
        public int Id { get; set; }
    }
}
