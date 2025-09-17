using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? SchoolId { get; set; }
        //public int? Createdby { get; set; }
        //public int? Modifiedby { get; set; }
        //public int? Statusid { get; set; }

        [NotMapped]
        public ICollection<SectionalGradeMappingModel> SelectedGrades { get; set; }

    }

    public class SectionalGradeMappingModel
    {
        //public int? Groupid { get; set; }
        //public int? Standardsectionmappingid { get; set; }
        public int? StandardId { get; set; }
    }


    public class SectionalGradeMappingModelForUpdate
    {
        public int? Groupid { get; set; }
        public int? Standardsectionmappingid { get; set; }
        public DateTime? Modifieddate { get; set; }
    }

    //public class MGroupUpdateModel : MGroupModel
    //{
    //    public int Id { get; set; }
    //}


    public class SectionalGradeModelGet
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? SchoolId { get; set; }

        [NotMapped]
        public ICollection<SectionalGradeMappingModelGet> SelectedGrades { get; set; }

        public SectionalGradeModelGet()
        {
            SelectedGrades = new List<SectionalGradeMappingModelGet>();
        }
    }

    public class SectionalGradeMappingModelGet
    {
        public int? Id { get; set; }
        public int? SectionalGradeId { get; set; }
        public int? StandardId { get; set; }
        public string StandardName { get; set; }
    }

}
