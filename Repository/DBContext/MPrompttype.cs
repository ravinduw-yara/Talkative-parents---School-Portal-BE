using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Repository.DBContext
{
    public partial class MPrompttype
    {
        public MPrompttype()
        {

        }

        public int Id { get; set; }
        public string Promptypename { get; set; }
        public string Promtypefromat { get; set; }
    }



}


