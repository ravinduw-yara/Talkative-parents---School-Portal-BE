using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class MSportschildmapping
    {
        public MSportschildmapping()
        {

        }

        public int Id { get; set; }

        public int ChildId { get; set; }

        public string ChildName { get; set; }


        public int SchoolId { get; set; }

        public int SportId { get; set; }

        public int SportGroupId { get; set; }

        public int levelId { get; set; }

        public int StandardId { get; set; }

        public int SectionId { get; set; }

        public int StatusId { get; set; }

    }


}


