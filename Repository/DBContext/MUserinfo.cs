using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MUserinfo
    {
        public MUserinfo()
        {
            MBranchCreatedbyNavigations = new HashSet<MBranch>();
            MBranchModifiedbyNavigations = new HashSet<MBranch>();
            MStandardsectionmappingCreatedbyNavigations = new HashSet<MStandardsectionmapping>();
            MStandardsectionmappingModifiedbyNavigations = new HashSet<MStandardsectionmapping>();
            MBusinessunittypeCreatedbyNavigations = new HashSet<MBusinessunittype>();
            MBusinessunittypeModifiedbyNavigations = new HashSet<MBusinessunittype>();
            MChildinfoCreatedbyNavigations = new HashSet<MChildinfo>();
            MChildinfoModifiedbyNavigations = new HashSet<MChildinfo>();
            MGenderCreatedbyNavigations = new HashSet<MGender>();
            MGenderModifiedbyNavigations = new HashSet<MGender>();
            MLocationtypeCreatedbyNavigations = new HashSet<MLocationtype>();
            MLocationtypeModifiedbyNavigations = new HashSet<MLocationtype>();
            MModuleCreatedbyNavigations = new HashSet<MModule>();
            MModuleModifiedbyNavigations = new HashSet<MModule>();
            MRelationtypeCreatedbyNavigations = new HashSet<MRelationtype>();
            MRelationtypeModifiedbyNavigations = new HashSet<MRelationtype>();
            MRoleCreatedbyNavigations = new HashSet<MRole>();
            MRoleModifiedbyNavigations = new HashSet<MRole>();
            MSchoolCreatedbyNavigations = new HashSet<MSchool>();
            MSchoolModifiedbyNavigations = new HashSet<MSchool>();
            MStatusCreatedbyNavigations = new HashSet<MStatus>();
            MStatusModifiedbyNavigations = new HashSet<MStatus>();
            MStatustypeCreatedbyNavigations = new HashSet<MStatustype>();
            MStatustypeModifiedbyNavigations = new HashSet<MStatustype>();
            MUserroleCreatedbyNavigations = new HashSet<MUserrole>();
            MUserroleModifiedbyNavigations = new HashSet<MUserrole>();
            MUserroleUsers = new HashSet<MUserrole>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public string Code { get; set; }
        public string Emailid { get; set; }
        public string Password { get; set; }
        public string Phonenumber { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public DateTime? Enddate { get; set; }
        public int? Genderid { get; set; }

        public virtual MGender Gender { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MBranch> MBranchCreatedbyNavigations { get; set; }
        public virtual ICollection<MBranch> MBranchModifiedbyNavigations { get; set; }
        public virtual ICollection<MStandardsectionmapping> MStandardsectionmappingCreatedbyNavigations { get; set; }
        public virtual ICollection<MStandardsectionmapping> MStandardsectionmappingModifiedbyNavigations { get; set; }
        public virtual ICollection<MBusinessunittype> MBusinessunittypeCreatedbyNavigations { get; set; }
        public virtual ICollection<MBusinessunittype> MBusinessunittypeModifiedbyNavigations { get; set; }
        public virtual ICollection<MChildinfo> MChildinfoCreatedbyNavigations { get; set; }
        public virtual ICollection<MChildinfo> MChildinfoModifiedbyNavigations { get; set; }
        public virtual ICollection<MGender> MGenderCreatedbyNavigations { get; set; }
        public virtual ICollection<MGender> MGenderModifiedbyNavigations { get; set; }
        public virtual ICollection<MLocationtype> MLocationtypeCreatedbyNavigations { get; set; }
        public virtual ICollection<MLocationtype> MLocationtypeModifiedbyNavigations { get; set; }
        public virtual ICollection<MModule> MModuleCreatedbyNavigations { get; set; }
        public virtual ICollection<MModule> MModuleModifiedbyNavigations { get; set; }
        public virtual ICollection<MRelationtype> MRelationtypeCreatedbyNavigations { get; set; }
        public virtual ICollection<MRelationtype> MRelationtypeModifiedbyNavigations { get; set; }
        public virtual ICollection<MRole> MRoleCreatedbyNavigations { get; set; }
        public virtual ICollection<MRole> MRoleModifiedbyNavigations { get; set; }
        public virtual ICollection<MSchool> MSchoolCreatedbyNavigations { get; set; }
        public virtual ICollection<MSchool> MSchoolModifiedbyNavigations { get; set; }
        public virtual ICollection<MStatus> MStatusCreatedbyNavigations { get; set; }
        public virtual ICollection<MStatus> MStatusModifiedbyNavigations { get; set; }
        public virtual ICollection<MStatustype> MStatustypeCreatedbyNavigations { get; set; }
        public virtual ICollection<MStatustype> MStatustypeModifiedbyNavigations { get; set; }
        public virtual ICollection<MUserrole> MUserroleCreatedbyNavigations { get; set; }
        public virtual ICollection<MUserrole> MUserroleModifiedbyNavigations { get; set; }
        public virtual ICollection<MUserrole> MUserroleUsers { get; set; }
    }
}
