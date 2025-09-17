using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MSubjectSectionMappingModel
    {
        public int SubjectId { get; set; }
        public int SchoolId { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public int StatusId { get; set; }
        public List<SubSectionsModel> Sections { get; set; }
        //public List<RemovedSubSectionsModel> RemovedSections { get; set; }

        public MSubjectSectionMappingModel()
        {
            Sections = new List<SubSectionsModel>();
            //RemovedSections = new List<RemovedSubSectionsModel>();
        }
    }

    public class RemovedSubSectionsModel
    {
        public string RemovedSectionName { get; set; }
        public Guid RemovedSectionId { get; set; }
    }
    public class SubSecMapDisplayModel
    {
        public string SubjectName { get; set; }
        public int SubjectId { get; set; }
        public List<SubStandardDisplayModel> Standards { get; set; }

        public SubSecMapDisplayModel()
        {
            Standards = new List<SubStandardDisplayModel>();
        }

    }
    public class SubStandardDisplayModel
    {
        public string StandardName { get; set; }
        public int StandardId { get; set; }
        public List<SubSectionsModel> Sections { get; set; }

        public SubStandardDisplayModel()
        {
            Sections = new List<SubSectionsModel>();
        }
    }
}
