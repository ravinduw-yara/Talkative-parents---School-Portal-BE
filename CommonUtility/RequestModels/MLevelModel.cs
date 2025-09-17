using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MLevelModel
    {

        public string levels { get; set; }
        public int schoolid { get; set; }
    }
    public class MLevelUpdateModel : MLevelModel
    {
        public int Id { get; set; }
    }
}
