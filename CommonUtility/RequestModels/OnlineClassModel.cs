using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CommonUtility.RequestModels
{
    class OnlineClassModel
    {
    }

    public class GCLVideoClass
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string MeetingLink { get; set; }
        //public string teacherfolderid { get; set; }
        public int schoolId { get; set; }
        public int? standardId { get; set; }
        public int? sectionId { get; set; }
        public int createdBy { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }

    public class GCLTemplate
    {
        public string gcourseid { get; set; }
        public string gcoursename { get; set; }
        public string gownerId { get; set; }
        public string teacherfolderid { get; set; }
        public int schoolId { get; set; }
        public int? standardId { get; set; }
        public int? sectionId { get; set; }
        public int createdBy { get; set; }
    }

    public class TeacherClass
    {
        public int schoolId { get; set; }
        public int teacherId { get; set; }
        public int standardId { get; set; }
        public int sectionId { get; set; }
        public int createdBy { get; set; }

    }

    public class TeacherClassandGCLRes
    {
        public int id { get; set; }
        public int standardId { get; set; }
        public string stdName { get; set; }
        public int sectionId { get; set; }
        public string sectionName { get; set; }
        public bool isApproved { get; set; }
        public int teacherid { get; set; }
        public List<GCLTemplateNew> GCL { get; set; }

    }
    public class GCLTemplateNew
    {
        public int ID { get; set; }
        public string gcourseid { get; set; }
        public string gcoursename { get; set; }
        public string gownerId { get; set; }
        public string teacherfolderid { get; set; }

    }

    public class TC_GCL_Res
    {
        public int standardId { get; set; }
        public string stdName { get; set; }
        public int sectionId { get; set; }
        public string sectionName { get; set; }
        public bool isApproved { get; set; }
        public List<GCLTemplateNew> GCL { get; set; }

    }


    public class techclassModel
    {
        public int Id { get; set; }
        public int? branchid { get; set; }
        public int? Standardsectionmappingid { get; set; }
        public int? Parentid { get; set; }
        public string Name { get; set; }
        public int? TeacherId { get; set; }
        public bool? IsApproved { get; set; }
    }

    public class TeacherEmail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
    }

    public class ChildEmail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string email { get; set; }
    }

}



