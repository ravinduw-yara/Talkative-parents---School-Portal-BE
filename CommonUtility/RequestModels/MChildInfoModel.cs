using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace CommonUtility.RequestModels
{
    //Teacher
    public class SingleTeacherDetailsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }


    }
    public class TeacherDropdownDetailsViewModel
    {
        public string TeacherId { get; set; }
        public string TeacherName { get; set; }


    }
    public class TeacherDetailsViewModel
    {
        public string TeacherName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int GradeId { get; set; }
        public int SectionId { get; set; }
    }
    //Teacher Add
    public class TeacherAddModel
    {
        public string Salutation { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public int? Genderid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phonenumber { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public List<QiMenuAccess> Students { get; set; }

    }
    public class QiMenuAccess
    {
        public int Studentanalytics { get; set; }
        public int Teacheranalytics { get; set; }
        public int Gradeanalytics { get; set; }
        public int Academictrends { get; set; }
        public int Digitalreportcard { get; set; }
        public int SubjectTeacherId { get; set; }
        public int StudentDetails { get; set; }
        public int ManageUser { get; set; }
        public int Intelligence { get; set; }
        public int DrcSettings { get; set; }
        public int TeacherDetails { get; set; }
    }
    //SchoolStudentAdd
    public class SchoolStudentAddModel
    {
        public string Code { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public DateTime? Dob { get; set; }
        public string Phonenumber { get; set; }
        public int? Genderid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public string Email { get; set; }
        public string Nic { get; set; }
        public int StandardsectionmappingId { get; set; }
        public int AcademicYearId { get; set; }
        public string registerationnumber { get; set; }

        // Parent details
        public string ParentFirstname { get; set; }
        public string ParentLastname { get; set; }
        public string ParentMiddlename { get; set; }
        public DateTime? ParentDob { get; set; }
        public string ParentPhonenumber { get; set; }
        public int? ParentGenderid { get; set; }
        public string ParentEmailid { get; set; }
        public string ParentPassword { get; set; }
        public bool ParentIssmsuser { get; set; }
        public bool ParentIshigheduser { get; set; }


        // Second Parent details
        public string Parent2Firstname { get; set; }
        public string Parent2Lastname { get; set; }
        public string Parent2Middlename { get; set; }
        public DateTime? Parent2Dob { get; set; }
        public string Parent2Phonenumber { get; set; }
        public int? Parent2Genderid { get; set; }
        public string Parent2Emailid { get; set; }
        public string Parent2Password { get; set; }
        public bool Parent2Issmsuser { get; set; }
        public bool Parent2Ishigheduser { get; set; }



    }
    //Academic Trend
    public class StudentCountData
    {
        public string Year { get; set; }
        public string SubjectName { get; set; }
        public int NumberOfStudents { get; set; }
    }
    public class PromoteStudentModel
    {
        public int AccedemicYearID { get; set; }
        public int? SchoolUserId { get; set; }
        public int? BranchId { get; set; }
        public int? SchoolId { get; set; }
        public int? Standardsectionmappingid { get; set; }
        public int? StandardId { get; set; }
        public int SectionId { get; set; }
        public List<PromoteStudentsList> students { get; set; }
        public int StudentCount { get; set; }

    }
    public class PromoteStudentsList
    {
        public int ChildId { get; set; }
        public string AdmissionId { get; set; }
    }
    //Jaliya - Student Add to batch
    public class HiEduStudentAddModel
    {
        public string Code { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public DateTime? Dob { get; set; }
        public string Phonenumber { get; set; }
        public int? Genderid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public string Email { get; set; }
        public string Nic { get; set; }
        public int BatchCourseMappingId { get; set; }

    }
    //Jaliya - Student Get
    public class StudentDetailsModel
    {
        public int ChildId { get; set; }
        public string ChildFirstName { get; set; }
        public string ChildLastName { get; set; }
        public string CourseName { get; set; }
        public string BatchName { get; set; }
    }
    public class MChildInfoModel
    {
        public string Code { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public DateTime? Dob { get; set; }
        public string Phonenumber { get; set; }
        public int? Genderid { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }

    }
    public class MChildInfoUpdateModel:MChildInfoModel
    {
        public int Id { get; set; }
    }


    public class GetUserStdSec
    {
        public int Id { get; set; }
        public int? schooluserid { get; set; }
        public int? schoolid { get; set; }
        public int? standardid { get; set; }
        public int? sectionid { get; set; }

    }
    public class GetAuthReport
    {
        public bool? parentexists { get; set; }
        public string Error { get; set; }
        public string phonenumber { get; set; }

    }
    public class GetParentsListModel
    {
        public int id { get; set; }
        public int? ChildId { get; set; }
        public int? ChildSectionId { get; set; }
        public string PhoneNumber { get; set; }
        public string ParentsName { get; set; }
        public string UserName { get; set; }
        public bool? Issmsuser { get; set; }
        public string EmailAddress { get; set; }
        public string ParentsGender { get; set; }
        public string ChildFirstName { get; set; }
        public string ChildLastName { get; set; }
        public string FatherFirstName { get; set; }
        public string FatherLastName { get; set; }
        public string MotherFirstName { get; set; }
        public string MotherLastName { get; set; }
        //  public string ChildName { get; set; }
        public string ChildGender { get; set; }
        public int? ChildGenderId { get; set; }
        public string StudentImageLink { get; set; }
        public IFormFile StudentImageFile { get; set; }
        public string ParentsRelation { get; set; }
        public int? Schoolid { get; set; }
        public string School { get; set; }
        public int? StandardId { get; set; }
        public string Standard { get; set; }
        public int? SectionId { get; set; }
        public string Section { get; set; }
        public int? Inactivestudent { get; set; }
        //public DateTime? Parentcreateddate { get; set; }
        //public DateTime? Childcreateddate { get; set; }
        public string SportTypeID { get; set; }
        public string SportName { get; set; }
        public string BloodGroup { get; set; }
        public string MedicalConditions { get; set; }
        public string SpecialNeeds { get; set; }
        public string ClubName { get; set; }
        public string RegisterationNumber { get; set; }
        public string HomeAddress { get; set; }
        public string House { get; set; }
        public string Hosteler { get; set; }
        public string Religion { get; set; }
        public string Contact2 { get; set; }
        public string AdmissionYear { get; set; }
        public string LeavingYear { get; set; }
        public string Contact1 { get; set; }
        public string Relationship1 { get; set; }
        public string MobileNo1 { get; set; }
        public string Relationship2 { get; set; }
        public string MobileNo2 { get; set; }
        public string MotherContactNo { get; set; }
        public string FatheerContactNo { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string FatherEmail { get; set; }
        public string MotherEmail { get; set; }
        public string GuardianName { get; set; }
        public string GuardianContactNo { get; set; }
        public string GuardianEmail { get; set; }
        public string Prefectship { get; set; }
        public string Scholarship { get; set; }
        public string Discipline { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Other { get; set; }
        public string IsMotherPastStudent { get; set; }
        public string IsFatherPastStudent { get; set; }
        public string Picture { get; set; }
        public string IsFatherAppUser { get; set; }
        public string IsMotherAppUser { get; set; }
        public string otherchildFirstName { get; set; }
        public string otherchildLastName { get; set; }
        public string OtherParentFirstName { get; set; }
        public string OtherParentLastName { get; set; }
        public string childemail { get; set; }
        //public string AdminssionNumber { get; set; }
        public string Medium { get; set; }
        public string OtherparentId { get; set; }
        public string OtherParentEmailAddress { get; set; }
        public string OtherParentPhoneNumber { get; set; }
        public bool? IsOtherParentSMSUser { get; set; }
        public string otherchildsectionname { get; set; }
        public string OtherChildId { get; set; }
        public string otherchildStandardname { get; set; }
        public string schoollogo { get; set; }
        public bool? ParentPortalEnabled { get; set; }
        public string AcademicYearName { get; set; }
        public int? AcademicYearId { get; set; }
        public int? DRCEnable1 { get; set; }
        public int? DRCEnable2 { get; set; }
        public int? DRCEnable3 { get; set; }
    }
    //public class GetParentsListModel
    //{
    //    public int id { get; set; }
    //    public int? ChildId { get; set; }
    //    public int? ChildSectionId { get; set; }
    //    public string PhoneNumber { get; set; }
    //    public string ParentsName { get; set; }
    //    public string UserName { get; set; }
    //    public bool? Issmsuser { get; set; }
    //    public string EmailAddress { get; set; }
    //    public string ParentsGender { get; set; }
    //    public string ChildFirstName { get; set; }
    //    public string ChildLastName { get; set; }
    //    //  public string ChildName { get; set; }
    //    public string ChildGender { get; set; }
    //    public int? ChildGenderId { get; set; }
    //    public string StudentImageLink { get; set; }
    //    public IFormFile StudentImageFile { get; set; }
    //    public string ParentsRelation { get; set; }
    //    public int? Schoolid { get; set; }
    //    public string School { get; set; }
    //    public int? StandardId { get; set; }
    //    public string Standard { get; set; }
    //    public int? SectionId { get; set; }
    //    public string Section { get; set; }
    //    public int? Inactivestudent { get; set; }
    //    //public DateTime? Parentcreateddate { get; set; }
    //    //public DateTime? Childcreateddate { get; set; }
    //    public string SportTypeID { get; set; }
    //    public string SportName { get; set; }
    //    public string BloodGroup { get; set; }
    //    public string MedicalConditions { get; set; }
    //    public string SpecialNeeds { get; set; }
    //    public string ClubName { get; set; }
    //    public string RegisterationNumber { get; set; }
    //    public string HomeAddress { get; set; }
    //    public string House { get; set; }
    //    public string Hosteler { get; set; }
    //    public string Religion { get; set; }
    //    public string Contact2 { get; set; }
    //    public string AdmissionYear { get; set; } 
    //    public string LeavingYear { get; set; }
    //    public string Contact1 { get; set; }
    //    public string Relationship1 { get; set; }
    //    public string MobileNo1 { get; set; }
    //    public string Relationship2 { get; set; }
    //    public string MobileNo2 { get; set; }
    //    public string MotherContactNo { get; set; }
    //    public string FatheerContactNo { get; set; }
    //    public string FatherName { get; set; }
    //    public string MotherName { get; set; }
    //    public string FatherEmail { get; set; }
    //    public string MotherEmail { get; set; }
    //    public string GuardianName { get; set; }
    //    public string GuardianContactNo { get; set; }
    //    public string GuardianEmail { get; set; }
    //    public string Prefectship { get; set; }
    //    public string Scholarship { get; set; }
    //    public string Discipline { get; set; }
    //    public DateTime? DateOfBirth { get; set; }
    //    public string Other { get; set; }
    //    public string IsMotherPastStudent { get; set; }
    //    public string IsFatherPastStudent { get; set; }
    //    public string Picture { get; set; }
    //    public string IsFatherAppUser { get; set; }
    //    public string IsMotherAppUser { get; set; }
    //    public string otherchildFirstName { get; set; }
    //    public string otherchildLastName { get; set; }
    //    public string OtherParentFirstName { get; set; }
    //    public string OtherParentLastName { get; set; }
    //    public string childemail { get; set; }
    //    //public string AdminssionNumber { get; set; }
    //    public string Medium { get; set; }
    //    public string OtherparentId { get; set; }
    //    public string OtherParentEmailAddress { get; set; }
    //    public string OtherParentPhoneNumber { get; set; }
    //    public bool? IsOtherParentSMSUser { get; set; }
    //    public string otherchildsectionname { get; set; }
    //    public string OtherChildId { get; set; }
    //    public string otherchildStandardname { get; set; }
    //    public string schoollogo { get; set; }
    //    public bool? ParentPortalEnabled { get; set; }
    //    public string AcademicYearName { get; set; }
    //    public int? AcademicYearId { get; set; }
    //    public int? DRCEnable1 { get; set; }
    //    public int? DRCEnable2 { get; set; }
    //    public int? DRCEnable3 { get; set; }
    //}

    public partial class ParentsUpdateModel
    {
        public int id { get; set; }
        public int childid { get; set; }
        public int childsectionid { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string username { get; set; }
        public Nullable<bool> Issmsuser { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<int> parentgender { get; set; }
        public string childfirstname { get; set; }
        public string childlastname { get; set; }
        //public Nullable<int> childgender { get; set; }
        public int? ChildGenderId { get; set; }
        public Nullable<int> parentrelation { get; set; }
        public Nullable<int> schoolid { get; set; }
        public string school { get; set; }
        public string standard { get; set; }
        public int standardid { get; set; }
        public string section { get; set; }
        public int sectionid { get; set; }
        //public int standardsectionmappingid { get; set; }
        public Nullable<System.DateTime> parentcreateddate { get; set; }
        public Nullable<System.DateTime> childcreateddate { get; set; }
    }
    public class ChildUpdateModel
    {
        public int AcademicYearId { get; set; }
        public List<childdrcsemesterlist> parentdrcenable { get; set; }

    }
    public class UpdatePromptModel
    {
        public int SchoolId { get; set; }

        public int? PromptTypeId1 { get; set; }
        public string? PromptType1 { get; set; }
        public string? PromptTypeFormat1 { get; set; }

        public int? PromptTypeId2 { get; set; }
        public string? PromptType2 { get; set; }
        public string? PromptTypeFormat2 { get; set; }

        public int? PromptTypeId3 { get; set; }
        public string? PromptType3 { get; set; }
        public string? PromptTypeFormat3 { get; set; }

        public int? PromptTypeId4 { get; set; }
        public string? PromptType4 { get; set; }
        public string? PromptTypeFormat4 { get; set; }

        public int? PromptTypeId5 { get; set; }
        public string? PromptType5 { get; set; }
        public string? PromptTypeFormat5 { get; set; }
    }

    public class AddPromptModel
    {
        public int SchoolId { get; set; }
        public string? PromptType1 { get; set; }
        public string? PromptTypeFormat1 { get; set; }
        public string? PromptType2 { get; set; }
        public string? PromptTypeFormat2 { get; set; }
        public string? PromptType3 { get; set; }
        public string? PromptTypeFormat3 { get; set; }
        public string? PromptType4 { get; set; }
        public string? PromptTypeFormat4 { get; set; }
        public string? PromptType5 { get; set; }
        public string? PromptTypeFormat5 { get; set; }
    }

    public class ParentAddModel
    {
        public int ChildId { get; set; }
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public string RelationType { get; set; } // 'Mother' or 'Father'
        public string ParentMobileNumber { get; set; }
        public string ParentEmail { get; set; }
        public bool? IsSmsUser { get; set; }
        public bool? IsHighEduUser { get; set; }
    }
    public class ChildBulkDeleteModel
    {
        public int[] ChildIds { get; set; }
    }
    public class ChildUpdateInactiveModel
    {
        public int AcademicYearId { get; set; }
        public int StandardId { get; set; }
        public int SectionId { get; set; }
        public int[] ChildIds { get; set; }
    }
    public class childdrcsemesterlist
    {
        public int ChildId { get; set; }
        public int? drcenable1 { get; set; }
        public int? drcenable2 { get; set; }
        public int? drcenable3 { get; set; }
    }
    //app api
    public class ChildModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int ChildId { get; set; }
        public string Picture { get; set; }
        [Required]
        public int RelationId { get; set; }
        [Required]
        public int Gender { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
        [Required]
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string Tag { get; set; }
        [Required]
        public int StandardId { get; set; }
        [Required]
        public int SectionId { get; set; }
        [Required]
        public string StandardName { get; set; }
        [Required]
        public string SectionName { get; set; }
        [Required]
        public string SBNotificationCount { get; set; }
        public string Logo { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }

    }
    public class GenerateQuetionPapersBulkDeleteModel
    {
        public List<GenerateQuestionPaper> Papers { get; set; }
    }

    public class GenerateQuestionPaper
    {
        public int Id { get; set; }
        public int AcademicYearFromId { get; set; }
        public int AcademicYearToId { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }

    }

}
