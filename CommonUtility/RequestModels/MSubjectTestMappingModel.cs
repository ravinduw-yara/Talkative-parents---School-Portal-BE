using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MSubjectTestMappingModel
    {
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public List<SubTestsModel> Tests { get; set; }
        public List<SubSectionsModel> Sections { get; set; }
        public MSubjectTestMappingModel()
        {
            Tests = new List<SubTestsModel>();
            Sections = new List<SubSectionsModel>();
        }
    }
    public class SubTestsModel
    {
        public string TestName { get; set; }
        public int TestId { get; set; }
    }
    public class SubSectionsModel
    {
        public string SectionName { get; set; }
        public int SectionId { get; set; }
    }
    public class SubTestMapModel
    {
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public List<SubSectionsModel> Sections { get; set; }
        public List<TestsModel> Tests { get; set; }
        //public List<SubSectionsModel> Sections { get; set; }

        public SubTestMapModel()
        {
            Sections = new List<SubSectionsModel>();
            Tests = new List<TestsModel>();
        }

    }
    public class TestsModel
    {
        public string TestName { get; set; }
        public int? TestId { get; set; }
        public int? MaxMakrs { get; set; }
    }
    public class GetSubjectTestMappingModel
    {
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public List<SubStandardModel> Standards { get; set; }
        public GetSubjectTestMappingModel()
        {
            Standards = new List<SubStandardModel>();
        }
    }
    public class SubStandardModel
    {
        public string StandardName { get; set; }
        public int StandardId { get; set; }
        public List<SubSectionDisplayModel> Sections { get; set; }

        public SubStandardModel()
        {
            Sections = new List<SubSectionDisplayModel>();
        }
    }
    public class SubSectionDisplayModel
    {
        public string SectionName { get; set; }
        public int SectionId { get; set; }
        public List<SubSemesterModel> Semesters { get; set; }
        public SubSectionDisplayModel()
        {
            Semesters = new List<SubSemesterModel>();
        }
    }
    public class SubSemesterModel
    {
        public string SemesterName { get; set; }
        public int SemesterId { get; set; }
        public List<SubTestsModel> Tests { get; set; }

        public SubSemesterModel()
        {
            Tests = new List<SubTestsModel>();
        }
    }
}
