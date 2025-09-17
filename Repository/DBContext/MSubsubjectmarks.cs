using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MSubsubjectmarks
    {
        public MSubsubjectmarks()
        {
           // MSubjectsectionmappings = new HashSet<MSubjectsectionmapping>();
        }
        public int Id { get; set; }
        public int? ChildTestMappingId { get; set; }
        public int SubSubjectId { get; set; }
        public int Marks { get; set; }
        public string Comment { get; set; }
        public double? SubPrecentage { get; set; }
        public int? Absent { get; set; }
        




    }
}
