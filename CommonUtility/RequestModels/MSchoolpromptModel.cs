using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MSchoolpromptModel
    {

        public int SchoolId { get; set; }
        public int Prompttype1 { get; set; }
        public int Prompttype2 { get; set; }
        public int Prompttype3 { get; set; }
        public int Prompttype4 { get; set; }
        public int Prompttype5 { get; set; }
    }
    public class MSchoolpromptUpdateModel : MSchoolpromptModel
    {
        public int Id { get; set; }
    }
}
