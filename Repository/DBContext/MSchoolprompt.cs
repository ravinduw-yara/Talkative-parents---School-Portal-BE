using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Repository.DBContext
{
    public partial class MSchoolprompt
    {
        public MSchoolprompt()
        {

        }

        public int Id { get; set; }
        public int SchoolId { get; set; }
        public int Prompttype1 { get; set; }
        public int Prompttype2 { get; set; }
        public int Prompttype3 { get; set; }
        public int Prompttype4 { get; set; }
        public int Prompttype5 { get; set; }

    }
}

