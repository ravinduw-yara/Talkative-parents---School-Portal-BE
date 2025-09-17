using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class SchoolUserModel
    {
        public int Id { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public string EmailAddress { get; set; }
        public int? BranchId { get; set; }
        public int? Gender { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Enddate { get; set; }
        public string Profilephoto { get; set; }
        public int? Statusid { get; set; }
        [NotMapped]
        public ICollection<SchooluserSBViewPermission> selectedPermission { get; set; }
        public ICollection<Schooluserrole> selectedClass { get; set; }

    }

    public class UserPermissionModel
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Isquantaenabled { get; set; }
        public List<GetUserClass> selectedClass { get; set; }
        public List<GetModulePermissions> selectedPermission { get; set; }

        public UserPermissionModel()
        {
            selectedPermission = new List<GetModulePermissions>();
            selectedClass = new List<GetUserClass>();
        }

    }


    public class Schooluserrole
    {
        public int? StandardId { get; set; }
        public int? SectionId { get; set; }
        public int? UserSpecializationCategoryId { get; set; }
        public int? SectionalGradeId { get; set; }

    }



    public class SchooluserSBViewPermission
    {
        public int id { get; set; }
    }

    #region more readble model
    //public class Schooluserrole
    //{
    //    public int? Standardsectionmappingid { get; set; }
    //    public int? CategoryId { get; set; }
    //}

    //public class SchooluserSBViewPermission
    //{
    //    public int ModuleId { get; set; } 
    //    //public int? SectionalGradeId { get; set; }
    //    //public int UserSpecializationCategoryId { get; set; }
    //}
    #endregion


    public partial class GetSchoolUser
    {
        public int Id { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public string EmailAddress { get; set; }
        public int? BranchId { get; set; }
        public int? Gender { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Enddate { get; set; }


        public List<GetSbAccessPermission> selectedSbAccessPermission { get; set; }
        public List<GetModulePermissions> selectedClass { get; set; }

        public GetSchoolUser()
        {
            selectedSbAccessPermission = new List<GetSbAccessPermission>();
            selectedClass = new List<GetModulePermissions>();
        }

    }


    public class GetModulePermissions
    {
        public string icon { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public bool selected { get; set; }
        public string state { get; set; }
        public string type { get; set; }

        public List<GetModuleChildren> children { get; set; }
        public GetModulePermissions()
        {
            children = new List<GetModuleChildren>();
        }

    }

    public class GetModuleChildren
    {
        public string name { get; set; }
        public string state { get; set; }
    }

    public class GetSbAccessPermission
    {
        public int Id { get; set; }
        public int? schooluserid { get; set; }
        public int? schoolid { get; set; }
        public int? standardid { get; set; }
        public int? sectionid { get; set; }
        public int? sectionalGradeId { get; set; }
        public int? UserSpecializationCategoryId { get; set; }
        public int? Rank { get; set; }

    }

    public class GetSbAccessPermissionForSP
    {
        public int Id { get; set; }
        public int? schooluserid { get; set; }
        public int? schoolid { get; set; }
        public int? standardid { get; set; }
        public int? sectionid { get; set; }
        public int? sectionalGradeId { get; set; }
        public int? UserSpecializationCategoryId { get; set; }
        public int? Rank { get; set; }
        public int? parentid { get; set; }
        public int? standandardsectionmappingid { get; set; }

    }

    //AuthServices
    public class AuthenticationModel
    {
        public int Id { get; set; }
        public Guid Token { get; set; }
        public string Salutation { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public string Code { get; set; }
        public string Emailid { get; set; }
        public string Gender { get; set; }
        public int GenderId { get; set; }
        public string Username { get; set; }
        public string OSAppId { get; set; }
        public string OSAuth { get; set; }
        public string Password { get; set; }
        public string Phonenumber { get; set; }
        public string Profilephoto { get; set; }
        public int? Pincode { get; set; }
        public int? Staffcount { get; set; }
        public bool? Allowcategory { get; set; }
        public bool? Issbsms { get; set; }
        public string Logo { get; set; }
        public string School { get; set; }
        public int SchoolId { get; set; }
        public int AllowedCriticalCount { get; set; }
        public int usedcount { get; set; }
        public string Branch { get; set; }
        public int BranchId { get; set; }
        public bool IsSchoolAdminUser { get; set; }
        public bool Isschooluser { get; set; }
        public DateTime? Enddate { get; set; }
        public int? Statusid { get; set; }
        public bool parentexists { get; set; }
        public string error { get; set; }



        #region commented for now. remove it later if its not required 
        //public List<GetUserClass> selectedClass { get; set; }
        //public List<GetModulePermissions> selectedPermission { get; set; }

        //public AuthenticationModel()
        //{
        //    selectedPermission = new List<GetModulePermissions>();
        //    selectedClass = new List<GetUserClass>();
        //}
        #endregion
    }

    public class GetUserClass
    {
        public int? Schoolid { get; set; }
        public int? Sectionid { get; set; }
        public int? Standardid { get; set; }
        public int? UserSpecializationCategoryId { get; set; }
        public int schooluserid { get; set; }
    }


    public class MUserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public string Code { get; set; }
        public string Emailid { get; set; }
        public string Password { get; set; }
        public string Phonenumber { get; set; }
        public int? Statusid { get; set; }
        public int? Genderid { get; set; }

    }

    //app
    public class UserModelPreLogin
    {
        [Required]
        public string UserName { get; set; }

        public string Code { get; set; }

        public string Password { get; set; }

        public string Salutation { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }
        public DateTime? Dob { get; set; }

        public int Gender { get; set; }

        public string EmailAddress { get; set; }
        public int? ismigrated { get; set; }

    }

    public class UserModelValidate
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ValidateModel
    {
        public int Status { get; set; }
        public Guid TokenId { get; set; }
    }


    public class ValidateModel1
    {
        public int Status { get; set; }
        public string TokenId { get; set; }
    }

    public class PreloginModel
    {
        public bool Status { get; set; }
    }

}
