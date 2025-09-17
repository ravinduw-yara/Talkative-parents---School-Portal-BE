using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MSchooluserinfo
    {
        public MSchooluserinfo()
        {
            MAcademicyeardetailCreatedbyNavigations = new HashSet<MAcademicyeardetail>();
            MAcademicyeardetailModifiedbyNavigations = new HashSet<MAcademicyeardetail>();
            MAppuserinfoCreatedbyNavigations = new HashSet<MAppuserinfo>();
            MAppuserinfoModifiedbyNavigations = new HashSet<MAppuserinfo>();
            MBranchCreatedbyNavigations = new HashSet<MBranch>();
            MBranchModifiedbyNavigations = new HashSet<MBranch>();
            MBusinessunittypeCreatedbyNavigations = new HashSet<MBusinessunittype>();
            MBusinessunittypeModifiedbyNavigations = new HashSet<MBusinessunittype>();
            MCategoryCreatedbyNavigations = new HashSet<MCategory>();
            MCategoryModifiedbyNavigations = new HashSet<MCategory>();
            MChildinfoCreatedbyNavigations = new HashSet<MChildinfo>();
            MChildinfoModifiedbyNavigations = new HashSet<MChildinfo>();
            MChildschoolmappingCreatedbyNavigations = new HashSet<MChildschoolmapping>();
            MChildschoolmappingModifiedbyNavigations = new HashSet<MChildschoolmapping>();
            MChildtestmappingCreatedbyNavigations = new HashSet<MChildtestmapping>();
            MChildtestmappingModifiedbyNavigations = new HashSet<MChildtestmapping>();
            MGenderCreatedbyNavigations = new HashSet<MGender>();
            MGenderModifiedbyNavigations = new HashSet<MGender>();
            MGroupCreatedbyNavigations = new HashSet<MGroup>();
            MGroupModifiedbyNavigations = new HashSet<MGroup>();
            MLocationCreatedbyNavigations = new HashSet<MLocation>();
            MLocationModifiedbyNavigations = new HashSet<MLocation>();
            MLocationtypeCreatedbyNavigations = new HashSet<MLocationtype>();
            MLocationtypeModifiedbyNavigations = new HashSet<MLocationtype>();
            MModuleCreatedbyNavigations = new HashSet<MModule>();
            MModuleModifiedbyNavigations = new HashSet<MModule>();
            MOverallchildtestCreatedbyNavigations = new HashSet<MOverallchildtest>();
            MOverallchildtestModifiedbyNavigations = new HashSet<MOverallchildtest>();
            MParentchildmappingCreatedbyNavigations = new HashSet<MParentchildmapping>();
            MParentchildmappingModifiedbyNavigations = new HashSet<MParentchildmapping>();
            MRelationtypeCreatedbyNavigations = new HashSet<MRelationtype>();
            MRelationtypeModifiedbyNavigations = new HashSet<MRelationtype>();
            MRoleCreatedbyNavigations = new HashSet<MRole>();
            MRoleModifiedbyNavigations = new HashSet<MRole>();
            MSchoolCreatedbyNavigations = new HashSet<MSchool>();
            MSchoolDamappingCreatedbyNavigations = new HashSet<MSchoolDamapping>();
            MSchoolDamappingModifiedbyNavigations = new HashSet<MSchoolDamapping>();
            MSchoolModifiedbyNavigations = new HashSet<MSchool>();
            MSchooluserroleCreatedbyNavigations = new HashSet<MSchooluserrole>();
            MSchooluserroleModifiedbyNavigations = new HashSet<MSchooluserrole>();
            MSchooluserroleSchoolusers = new HashSet<MSchooluserrole>();
            MSemestertestsmappingCreatedbyNavigations = new HashSet<MSemestertestsmapping>();
            MSemestertestsmappingModifiedbyNavigations = new HashSet<MSemestertestsmapping>();
            MSemesteryearmappingCreatedbyNavigations = new HashSet<MSemesteryearmapping>();
            MSemesteryearmappingModifiedbyNavigations = new HashSet<MSemesteryearmapping>();
            MStandardgroupmappingCreatedbyNavigations = new HashSet<MStandardgroupmapping>();
            MStandardgroupmappingModifiedbyNavigations = new HashSet<MStandardgroupmapping>();
            MStandardsectionmappingCreatedbyNavigations = new HashSet<MStandardsectionmapping>();
            MStandardsectionmappingModifiedbyNavigations = new HashSet<MStandardsectionmapping>();
            MStatusCreatedbyNavigations = new HashSet<MStatus>();
            MStatusModifiedbyNavigations = new HashSet<MStatus>();
            MStatustypeCreatedbyNavigations = new HashSet<MStatustype>();
            MStatustypeModifiedbyNavigations = new HashSet<MStatustype>();
            MSubjectCreatedbyNavigations = new HashSet<MSubject>();
            MSubjectModifiedbyNavigations = new HashSet<MSubject>();
            MSubjectsectionmappingCreatedbyNavigations = new HashSet<MSubjectsectionmapping>();
            MSubjectsectionmappingModifiedbyNavigations = new HashSet<MSubjectsectionmapping>();
            MSubjectsemesterpercentageCreatedbyNavigations = new HashSet<MSubjectsemesterpercentage>();
            MSubjectsemesterpercentageModifiedbyNavigations = new HashSet<MSubjectsemesterpercentage>();
            MTeachersubjectmappingCreatedbyNavigations = new HashSet<MTeachersubjectmapping>();
            MTeachersubjectmappingModifiedbyNavigations = new HashSet<MTeachersubjectmapping>();
            MTeachersubjectmappingTeachers = new HashSet<MTeachersubjectmapping>();
            MTestsectionmappingCreatedbyNavigations = new HashSet<MTestsectionmapping>();
            MTestsectionmappingModifiedbyNavigations = new HashSet<MTestsectionmapping>();
            MUsermodulemappingCreatedbyNavigations = new HashSet<MUsermodulemapping>();
            MUsermodulemappingModifiedbyNavigations = new HashSet<MUsermodulemapping>();
            MUsermodulemappingSchoolusers = new HashSet<MUsermodulemapping>();
            TCalendereventdetailCreatedbyNavigations = new HashSet<TCalendereventdetail>();
            TCalendereventdetailModifiedbyNavigations = new HashSet<TCalendereventdetail>();
            TGclteacherclassCreatedbyNavigations = new HashSet<TGclteacherclass>();
            TGclteacherclassModifiedbyNavigations = new HashSet<TGclteacherclass>();
            TGclvedioclassCreatedbyNavigations = new HashSet<TGclvedioclass>();
            TGclvedioclassModifiedbyNavigations = new HashSet<TGclvedioclass>();
            TGoogleclassCreatedbyNavigations = new HashSet<TGoogleclass>();
            TGoogleclassModifiedbyNavigations = new HashSet<TGoogleclass>();
            TNoticeboardmappingCreatedbyNavigations = new HashSet<TNoticeboardmapping>();
            TNoticeboardmappingModifiedbyNavigations = new HashSet<TNoticeboardmapping>();
            TNoticeboardmessageCreatedbyNavigations = new HashSet<TNoticeboardmessage>();
            TNoticeboardmessageModifiedbyNavigations = new HashSet<TNoticeboardmessage>();
            TNoticeboardmessageSchoolusers = new HashSet<TNoticeboardmessage>();
        }

        public int Id { get; set; }
        public string Salutation { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public string Code { get; set; }
        public string Emailid { get; set; }
        public int? Genderid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phonenumber { get; set; }
        public string Profilephoto { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? Branchid { get; set; }
        public int? Subjectteacher { get; set; }
        public int? Isquantaenabled { get; set; }

        public virtual MBranch Branch { get; set; }
        public virtual MGender Gender { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MAcademicyeardetail> MAcademicyeardetailCreatedbyNavigations { get; set; }
        public virtual ICollection<MAcademicyeardetail> MAcademicyeardetailModifiedbyNavigations { get; set; }
        public virtual ICollection<MAppuserinfo> MAppuserinfoCreatedbyNavigations { get; set; }
        public virtual ICollection<MAppuserinfo> MAppuserinfoModifiedbyNavigations { get; set; }
        public virtual ICollection<MBranch> MBranchCreatedbyNavigations { get; set; }
        public virtual ICollection<MBranch> MBranchModifiedbyNavigations { get; set; }
        public virtual ICollection<MBusinessunittype> MBusinessunittypeCreatedbyNavigations { get; set; }
        public virtual ICollection<MBusinessunittype> MBusinessunittypeModifiedbyNavigations { get; set; }
        public virtual ICollection<MCategory> MCategoryCreatedbyNavigations { get; set; }
        public virtual ICollection<MCategory> MCategoryModifiedbyNavigations { get; set; }
        public virtual ICollection<MChildinfo> MChildinfoCreatedbyNavigations { get; set; }
        public virtual ICollection<MChildinfo> MChildinfoModifiedbyNavigations { get; set; }
        public virtual ICollection<MChildschoolmapping> MChildschoolmappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MChildschoolmapping> MChildschoolmappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MChildtestmapping> MChildtestmappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MChildtestmapping> MChildtestmappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MGender> MGenderCreatedbyNavigations { get; set; }
        public virtual ICollection<MGender> MGenderModifiedbyNavigations { get; set; }
        public virtual ICollection<MGroup> MGroupCreatedbyNavigations { get; set; }
        public virtual ICollection<MGroup> MGroupModifiedbyNavigations { get; set; }
        public virtual ICollection<MLocation> MLocationCreatedbyNavigations { get; set; }
        public virtual ICollection<MLocation> MLocationModifiedbyNavigations { get; set; }
        public virtual ICollection<MLocationtype> MLocationtypeCreatedbyNavigations { get; set; }
        public virtual ICollection<MLocationtype> MLocationtypeModifiedbyNavigations { get; set; }
        public virtual ICollection<MModule> MModuleCreatedbyNavigations { get; set; }
        public virtual ICollection<MModule> MModuleModifiedbyNavigations { get; set; }
        public virtual ICollection<MOverallchildtest> MOverallchildtestCreatedbyNavigations { get; set; }
        public virtual ICollection<MOverallchildtest> MOverallchildtestModifiedbyNavigations { get; set; }
        public virtual ICollection<MParentchildmapping> MParentchildmappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MParentchildmapping> MParentchildmappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MRelationtype> MRelationtypeCreatedbyNavigations { get; set; }
        public virtual ICollection<MRelationtype> MRelationtypeModifiedbyNavigations { get; set; }
        public virtual ICollection<MRole> MRoleCreatedbyNavigations { get; set; }
        public virtual ICollection<MRole> MRoleModifiedbyNavigations { get; set; }
        public virtual ICollection<MSchool> MSchoolCreatedbyNavigations { get; set; }
        public virtual ICollection<MSchoolDamapping> MSchoolDamappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MSchoolDamapping> MSchoolDamappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MSchool> MSchoolModifiedbyNavigations { get; set; }
        public virtual ICollection<MSchooluserrole> MSchooluserroleCreatedbyNavigations { get; set; }
        public virtual ICollection<MSchooluserrole> MSchooluserroleModifiedbyNavigations { get; set; }
        public virtual ICollection<MSchooluserrole> MSchooluserroleSchoolusers { get; set; }
        public virtual ICollection<MSemestertestsmapping> MSemestertestsmappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MSemestertestsmapping> MSemestertestsmappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MSemesteryearmapping> MSemesteryearmappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MSemesteryearmapping> MSemesteryearmappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MStandardgroupmapping> MStandardgroupmappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MStandardgroupmapping> MStandardgroupmappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MStandardsectionmapping> MStandardsectionmappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MStandardsectionmapping> MStandardsectionmappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MStatus> MStatusCreatedbyNavigations { get; set; }
        public virtual ICollection<MStatus> MStatusModifiedbyNavigations { get; set; }
        public virtual ICollection<MStatustype> MStatustypeCreatedbyNavigations { get; set; }
        public virtual ICollection<MStatustype> MStatustypeModifiedbyNavigations { get; set; }
        public virtual ICollection<MSubject> MSubjectCreatedbyNavigations { get; set; }
        public virtual ICollection<MSubject> MSubjectModifiedbyNavigations { get; set; }
        public virtual ICollection<MSubjectsectionmapping> MSubjectsectionmappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MSubjectsectionmapping> MSubjectsectionmappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MSubjectsemesterpercentage> MSubjectsemesterpercentageCreatedbyNavigations { get; set; }
        public virtual ICollection<MSubjectsemesterpercentage> MSubjectsemesterpercentageModifiedbyNavigations { get; set; }
        public virtual ICollection<MTeachersubjectmapping> MTeachersubjectmappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MTeachersubjectmapping> MTeachersubjectmappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MTeachersubjectmapping> MTeachersubjectmappingTeachers { get; set; }
        public virtual ICollection<MTestsectionmapping> MTestsectionmappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MTestsectionmapping> MTestsectionmappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MUsermodulemapping> MUsermodulemappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MUsermodulemapping> MUsermodulemappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MUsermodulemapping> MUsermodulemappingSchoolusers { get; set; }
        public virtual ICollection<TCalendereventdetail> TCalendereventdetailCreatedbyNavigations { get; set; }
        public virtual ICollection<TCalendereventdetail> TCalendereventdetailModifiedbyNavigations { get; set; }
        public virtual ICollection<TGclteacherclass> TGclteacherclassCreatedbyNavigations { get; set; }
        public virtual ICollection<TGclteacherclass> TGclteacherclassModifiedbyNavigations { get; set; }
        public virtual ICollection<TGclvedioclass> TGclvedioclassCreatedbyNavigations { get; set; }
        public virtual ICollection<TGclvedioclass> TGclvedioclassModifiedbyNavigations { get; set; }
        public virtual ICollection<TGoogleclass> TGoogleclassCreatedbyNavigations { get; set; }
        public virtual ICollection<TGoogleclass> TGoogleclassModifiedbyNavigations { get; set; }
        public virtual ICollection<TNoticeboardmapping> TNoticeboardmappingCreatedbyNavigations { get; set; }
        public virtual ICollection<TNoticeboardmapping> TNoticeboardmappingModifiedbyNavigations { get; set; }
        public virtual ICollection<TNoticeboardmessage> TNoticeboardmessageCreatedbyNavigations { get; set; }
        public virtual ICollection<TNoticeboardmessage> TNoticeboardmessageModifiedbyNavigations { get; set; }
        public virtual ICollection<TNoticeboardmessage> TNoticeboardmessageSchoolusers { get; set; }
    }
}
