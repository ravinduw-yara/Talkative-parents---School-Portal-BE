using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MTNoticeboardmessageModel
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Attachments { get; set; }
        public string Sms { get; set; }
        public bool? Ispriority { get; set; }
        public bool? Isemail { get; set; }

        //public DateTime? Createddate { get; set; }
        //public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }

    public class GetParents
    {
        public int Id { get; set; }
        public string Parent { get; set; }
        public string ChildRelation { get; set; }
        public string ChildEmail { get; set; }
        public int ChildId { get; set; }
        public int ChildSchoolMappingId { get; set; }
        public string firstName { get; set; }
        public int StandardId { get; set; }
        public string Standard { get; set; }
        public int SectionId { get; set; }
        public string Section { get; set; }
        public int SecondParent { get; set; }
        public string ParentName2 { get; set; }
        public string RegistrationNumber { get; set; }
        public int? StatusId { get; set; }  // 27/2/2024 Sanduni
        // public string ParentToSchoolCategoryChannel { get; set; }
        // public string SchoolToParent { get; set; }
    }

    public class GetSchoolMessage
    {
        public int Id { get; set; }
        public int SchoolUserId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool? IsEntireSchool { get; set; }
        public bool? IsParticularClass { get; set; }
        public bool? IsParticularSection { get; set; }
        public bool? IsParticularParent { get; set; }
        public string Attachments { get; set; }
        public int? StandardId { get; set; }
        public int? SectionId { get; set; }
        public List<ParticularParent> Parents { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public string OneSignal { get; set; }
        public int ChildId { get; set; }
        public int ParCount { get; set; }
        public int? Relation { get; set; }
        public string standardname { get; set; }
        public string sectionname { get; set; }
        public bool? IsPriority { get; set; }
        public string SMS { get; set; }
        public bool? isemail { get; set; }
        public DateTime? Datetimesent { get; set; }
        public string schoolusername { get; set; }
    }

    public class ParticularParent
    {
        public int? ParentId { get; set; }
        public int? ChildId { get; set; }
        public string ChildName { get; set; }
        public string ParentName { get; set; }
        public int? Relation { get; set; }

    }


    public class EMAIL_Message_Template
    {
        public static string template = "<p><span >MESSAGE_BODY</span></p>"
        + "<p></p>"
        + "<p><span></span></p>"
        + "<p><span style = 'font-family: Arial, Helvetica, sans-serif; font-size: 10px; color: #808080; background-color: #ffffff;' >"
        + "<i>You have received this email because the School has subscribed to the Talkative Parents communication platform. Please note that Talkative Parents does not take responsibility for the message content. You may contact the school for clarifications as required. "
               + "If you are not the intended recipient of this message, please email <a href = 'mailto:support@yaratechnologies.com' style= 'color: #808080; background-color: #ffffff;' > support@yaratechnologies.com</a> to be removed.</i></span></p>";
    }


    //APP
    public class SchoolMessageModel
    {
        public int Id { get; set; }
        public int SchoolUserId { get; set; }
        public int SchoolId { get; set; }
        public string Attachments { get; set; }
        public bool? IsEntireSchool { get; set; }
        public bool? IsParticularClass { get; set; }
        public bool? IsParticularParent { get; set; }
        public bool? IsParticularSection { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int? StandardId { get; set; }
        public int? SectionId { get; set; }
        public bool? ShouldDelete { get; set; }
        public DateTime? DateTimeSent { get; set; }
        public string SchoolName { get; set; }
        public bool? IsPriority { get; set; }
        public string SMS { get; set; }
    }
    public class UserDetailsModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public int RoleID { get; set; }
        public int UserSpecializationCategoryId { get; set; }
        public int Rank { get; set; }
        public string RankName { get; set; }
    }

}
