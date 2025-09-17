using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MStatus
    {
        public MStatus()
        {
            Appuserdevices = new HashSet<Appuserdevice>();
            MAcademicyeardetails = new HashSet<MAcademicyeardetail>();
            MAppuserinfos = new HashSet<MAppuserinfo>();
            MBranches = new HashSet<MBranch>();
            MBusinessunittypes = new HashSet<MBusinessunittype>();
            MCategories = new HashSet<MCategory>();
            MChildinfos = new HashSet<MChildinfo>();
            MChildschoolmappings = new HashSet<MChildschoolmapping>();
            MChildtestmappings = new HashSet<MChildtestmapping>();
            MFeatures = new HashSet<MFeature>();
            MGenders = new HashSet<MGender>();
            MGroups = new HashSet<MGroup>();
            MLocations = new HashSet<MLocation>();
            MLocationtypes = new HashSet<MLocationtype>();
            MModules = new HashSet<MModule>();
            MOverallchildtests = new HashSet<MOverallchildtest>();
            MParentchildmappings = new HashSet<MParentchildmapping>();
            MRelationtypes = new HashSet<MRelationtype>();
            MRoles = new HashSet<MRole>();
            MSchoolDamappings = new HashSet<MSchoolDamapping>();
            MSchools = new HashSet<MSchool>();
            MSchooluserinfos = new HashSet<MSchooluserinfo>();
            MSchooluserroles = new HashSet<MSchooluserrole>();
            MSemestertestsmappings = new HashSet<MSemestertestsmapping>();
            MSemesteryearmappings = new HashSet<MSemesteryearmapping>();
            MStandardgroupmappings = new HashSet<MStandardgroupmapping>();
            MStandardsectionmappings = new HashSet<MStandardsectionmapping>();
            MSubjects = new HashSet<MSubject>();
            MSubjectsectionmappings = new HashSet<MSubjectsectionmapping>();
            MSubjectsemesterpercentages = new HashSet<MSubjectsemesterpercentage>();
            MTeachersubjectmappings = new HashSet<MTeachersubjectmapping>();
            MTestsectionmappings = new HashSet<MTestsectionmapping>();
            MUsermodulemappings = new HashSet<MUsermodulemapping>();
            TCalendereventdetails = new HashSet<TCalendereventdetail>();
            TEmaillogs = new HashSet<TEmaillog>();
            TGclteacherclasses = new HashSet<TGclteacherclass>();
            TGclvedioclasses = new HashSet<TGclvedioclass>();
            TGoogleclasses = new HashSet<TGoogleclass>();
            TNoticeboardmappings = new HashSet<TNoticeboardmapping>();
            TNoticeboardmessages = new HashSet<TNoticeboardmessage>();
            TSoundingboardmessages = new HashSet<TSoundingboardmessage>();
            TTokens = new HashSet<TToken>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Statustypeid { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }

        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatustype Statustype { get; set; }
        public virtual ICollection<Appuserdevice> Appuserdevices { get; set; }
        public virtual ICollection<MAcademicyeardetail> MAcademicyeardetails { get; set; }
        public virtual ICollection<MAppuserinfo> MAppuserinfos { get; set; }
        public virtual ICollection<MBranch> MBranches { get; set; }
        public virtual ICollection<MBusinessunittype> MBusinessunittypes { get; set; }
        public virtual ICollection<MCategory> MCategories { get; set; }
        public virtual ICollection<MChildinfo> MChildinfos { get; set; }
        public virtual ICollection<MChildschoolmapping> MChildschoolmappings { get; set; }
        public virtual ICollection<MChildtestmapping> MChildtestmappings { get; set; }
        public virtual ICollection<MFeature> MFeatures { get; set; }
        public virtual ICollection<MGender> MGenders { get; set; }
        public virtual ICollection<MGroup> MGroups { get; set; }
        public virtual ICollection<MLocation> MLocations { get; set; }
        public virtual ICollection<MLocationtype> MLocationtypes { get; set; }
        public virtual ICollection<MModule> MModules { get; set; }
        public virtual ICollection<MOverallchildtest> MOverallchildtests { get; set; }
        public virtual ICollection<MParentchildmapping> MParentchildmappings { get; set; }
        public virtual ICollection<MRelationtype> MRelationtypes { get; set; }
        public virtual ICollection<MRole> MRoles { get; set; }
        public virtual ICollection<MSchoolDamapping> MSchoolDamappings { get; set; }
        public virtual ICollection<MSchool> MSchools { get; set; }
        public virtual ICollection<MSchooluserinfo> MSchooluserinfos { get; set; }
        public virtual ICollection<MSchooluserrole> MSchooluserroles { get; set; }
        public virtual ICollection<MSemestertestsmapping> MSemestertestsmappings { get; set; }
        public virtual ICollection<MSemesteryearmapping> MSemesteryearmappings { get; set; }
        public virtual ICollection<MStandardgroupmapping> MStandardgroupmappings { get; set; }
        public virtual ICollection<MStandardsectionmapping> MStandardsectionmappings { get; set; }
        public virtual ICollection<MSubject> MSubjects { get; set; }
        public virtual ICollection<MSubjectsectionmapping> MSubjectsectionmappings { get; set; }
        public virtual ICollection<MSubjectsemesterpercentage> MSubjectsemesterpercentages { get; set; }
        public virtual ICollection<MTeachersubjectmapping> MTeachersubjectmappings { get; set; }
        public virtual ICollection<MTestsectionmapping> MTestsectionmappings { get; set; }
        public virtual ICollection<MUsermodulemapping> MUsermodulemappings { get; set; }
        public virtual ICollection<TCalendereventdetail> TCalendereventdetails { get; set; }
        public virtual ICollection<TEmaillog> TEmaillogs { get; set; }
        public virtual ICollection<TGclteacherclass> TGclteacherclasses { get; set; }
        public virtual ICollection<TGclvedioclass> TGclvedioclasses { get; set; }
        public virtual ICollection<TGoogleclass> TGoogleclasses { get; set; }
        public virtual ICollection<TNoticeboardmapping> TNoticeboardmappings { get; set; }
        public virtual ICollection<TNoticeboardmessage> TNoticeboardmessages { get; set; }
        public virtual ICollection<TSoundingboardmessage> TSoundingboardmessages { get; set; }
        public virtual ICollection<TToken> TTokens { get; set; }
    }
}
