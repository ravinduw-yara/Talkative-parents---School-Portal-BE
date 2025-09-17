using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MHieduChildInfoStudentModel
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

    public class MChildInfoStudentUpdateModel : MHieduChildInfoStudentModel
    {
        public int Id { get; set; }
    }


    public class GetStudentUserStdSec
    {
        public int Id { get; set; }
        public int? schooluserid { get; set; }
        public int? schoolid { get; set; }
        public int? standardid { get; set; }
        public int? sectionid { get; set; }

    }
    public class GetStudentAuthReport
    {
        public bool? parentexists { get; set; }
        public string Error { get; set; }
        public string phonenumber { get; set; }

    }

    public class GetStudentParentsListModel
    {
        public int id { get; set; }
        public int? ChildId { get; set; }
        // public int? ChildSectionId { get; set; }

        /*public string MotherName { get; set; }
        public string FatherName { get; set; }

        public string MotherEmail { get; set; }
        public string FatherEmail { get; set; }
        public string MotherContactNo { get; set; }
        public string FatheerContactNo { get; set; }*/
        public string ChildFirstName { get; set; }
        public string ChildMiddletName { get; set; }
        public string ChildLastName { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string childphone { get; set; }
        public string nic { get; set; }
        public string childbatch { get; set; }
        public string childcourse { get; set; }
        public string childadmissinno { get; set; }

        public string childHomeaddress { get; set; }
        public string childHouse { get; set; }

        public string childHosteler { get; set; }
        public string childReligion { get; set; }
        public string childMedium { get; set; }
        public string childAdmissionYear { get; set; }
        public string childLeavingYear { get; set; }
        public string BloodGroup { get; set; }
        public string MedicalConditions { get; set; }
        public string SpecialNeeds { get; set; }

        public string clubname { get; set; }
        public string Other { get; set; }
        public string Contact1 { get; set; }
        public string Relationship1 { get; set; }
        public string MobileNo1 { get; set; }
        public string Contact2 { get; set; }
        public string Relationship2 { get; set; }
        public string MobileNo2 { get; set; }
        public string aluminifather { get; set; }
        public string aluminimother { get; set; }
        public string Parentmothername { get; set; }
        public string Parentmothecontectno { get; set; }
        public string Parentmotheremail { get; set; }
        public string Parentfathername { get; set; }
        public string Parentfathercontectno { get; set; }

        public string Parentfatheremail { get; set; }
        public string GuardianName { get; set; }
        public string GuardianContactNo { get; set; }
        public string GuardianEmail { get; set; }
        public string IsMotherPastStudent { get; set; }
        public string IsFatherPastStudent { get; set; }
        public string Scholarship { get; set; }
        public string Discipline { get; set; }
        public string SportName { get; set; }
        public string OtherParentEmailAddress { get; set; }
        public string OtherParentPhoneNumber
        {
            get; set;
        }
        public bool? IsOtherParentSMSUser { get; set; }
        public string otherchildsectionname { get; set; }
        public string OtherChildId { get; set; }
        public string otherchildStandardname
        {
            get; set;

        }
        public string OtherParentFirstName { get; set; }
        public string OtherParentLastName { get; set; }
        public string Childsgender { get; set; }
        public string Parentsgender { get; set; }
        public string Parentsrelation { get; set; }
        public string Schoolid { get; set; }
        public string childemail { get; set; }


        /*old today
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
        //  public string ChildName { get; set; }
        public string ChildGender { get; set; }
        public int? ChildGenderId { get; set; }
        public string ParentsRelation { get; set; }
        public int? Schoolid { get; set; }
        public string School { get; set; }
        public int? StandardId { get; set; }
        public string Standard { get; set; }
        public int? SectionId { get; set; }
        public string Section { get; set; }
        public DateTime? Parentcreateddate { get; set; }
        public DateTime? Childcreateddate { get; set; }
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
        public string AdminssionNumber { get; set; }
        public string Medium { get; set; }
        public string OtherParentEmailAddress { get; set; }
        public string OtherParentPhoneNumber
        {
            get; set;
        }
        public bool? IsOtherParentSMSUser { get; set; }
        public string otherchildsectionname { get; set; }
        public string OtherChildId { get; set; }
        public string otherchildStandardname { get; set; }
        public string schoollogo { get; set; }
        public bool? ParentPortalEnabled { get; set; }
*/


    }

    public partial class StudentParentsUpdateModel
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

    //app api
    public class ChildStudentModel
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

}
