using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class Mlevelsectionmapping
    {
        public Mlevelsectionmapping()
        {
        }

        public int Id { get; set; }

        public int? levelId { get; set; }
        public int? standardsectionmappingId { get; set; }


        public virtual MLevel Level { get; set; }

    }
}