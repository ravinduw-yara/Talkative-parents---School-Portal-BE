using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MLevel
    {
        public MLevel()
        {
            // MSubjectsectionmappings = new HashSet<MSubjectsectionmapping>();
        }

        public int Id { get; set; }
        public string levels { get; set; }
        public virtual MSchool School { get; set; }
        public int schoolid { get; set; }
        // public virtual ICollection<MSubjectsectionmapping> MSubjectsectionmappings { get; set; }
    }
}
