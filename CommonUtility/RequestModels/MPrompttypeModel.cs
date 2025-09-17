using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MPrompttypeModel
    {

        public string Promptypename { get; set; }
        public string Promtypefromat { get; set; }
    }
    public class MPrompttypeUpdateModel : MPrompttypeModel
    {
        public int Id { get; set; }
    }
}
