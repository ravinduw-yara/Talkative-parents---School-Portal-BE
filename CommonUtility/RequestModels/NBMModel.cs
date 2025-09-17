using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class NBMModel
    {
        public int Id { get; set; } //Previously GUID was used
        public int Token { get; set; }
        public int? SchoolUserId { get; set; }
        public int? BranchId { get; set; }
        public int? SchoolId { get; set; }
        public int? Standardsectionmappingid { get; set; }
        public string Attachments { get; set; }
        public bool? IsEntireSchool { get; set; }
        public bool? IsParticularClass { get; set; }
        public bool? IsParticularParent { get; set; }
        public bool? IsParticularSection { get; set; }
        public bool? Isemail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public List<ClassList> Classes { get; set; }
        public int ParentCount { get; set; }
        public bool? Ispriority { get; set; }
        public string Sms { get; set; }
        //public string OneSignal { get; set; }
        public string SchoolName { get; set; }
        public string filename { get; set; }
        public string base64 { get; set; }

    }
    public class ClassList
    {
        public int StdId { get; set; }
        public List<Section> sectionModel { get; set; }
    }

    public class Section
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public bool? selected { get; set; }
        public List<ParentList> Parents { get; set; }
    }
    public class ParentList
    {
        public int Id { get; set; } //Parent Id
        public string Parent { get; set; } //Appuserid
        public string ChildRelation { get; set; } //Relationtypeid
        public int ChildId { get; set; }
    }
}
