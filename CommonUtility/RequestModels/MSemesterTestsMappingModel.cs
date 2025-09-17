using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MSemesterTestsMappingModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Semesterid { get; set; }
        public int? Branchid { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? Schoolid { get; set; }
        public int? AcademicYearId { get; set; }
    }
    public class MSemesterTestsMappingUpdateModel : MSemesterTestsMappingModel
    {
        public int Id { get; set; }
    }
    public class SemesterModel
    {
        public string SemesterName { get; set; }
        public int SemesterId { get; set; }
        public string Description { get; set; }
        public int? Branchid { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? Schoolid { get; set; }
        public int? AcademicYearId { get; set; }
        public List<TestModel> Tests { get; set; }
        public List<dataAYear> Years { get; set; }
        public SemesterModel()
        {
            Years = new List<dataAYear>();
            Tests = new List<TestModel>();
        }
    }

    public class TestModel
    {
        public string TestName { get; set; }
        public int TestId { get; set; }
        public List<StdModel> Stds { get; set; }
        public TestModel()
        {
            Stds = new List<StdModel>();
        }
    }

    public class StdModel
    {
        public int StandardId { get; set; }
        public string StandardName { get; set; }
        public List<SecModel> Sections { get; set; }
        public StdModel()
        {
            Sections = new List<SecModel>();
        }

    }

    public class SecModel
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
    }
}
