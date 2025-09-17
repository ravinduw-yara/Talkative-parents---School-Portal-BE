using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MSchool
    {
        public MSchool()
        {
            MAcademicyeardetails = new HashSet<MAcademicyeardetail>();
            MBranches = new HashSet<MBranch>();
            MFeatures = new HashSet<MFeature>();
            MGroups = new HashSet<MGroup>();
            MRoles = new HashSet<MRole>();
            MSchoolDamappings = new HashSet<MSchoolDamapping>();
            TCalendereventdetails = new HashSet<TCalendereventdetail>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Websitelink { get; set; }
        public string Emailid { get; set; }
        public string Ipgurl { get; set; }
        public string Primaryphonenumber { get; set; }
        public string Secondaryphonenumber { get; set; }
        public int? Staffcount { get; set; }
        public string Logo { get; set; }
        public bool? Allowcategory { get; set; }
        public bool? Issbsms { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? pdfmodel { get; set; }
        public int? drcmodel { get; set; }
        public int? eduloanstatus { get; set; }
        
        /// <summary>
        //public int? ismigrated { get; set; }
        /// </summary>
        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<MAcademicyeardetail> MAcademicyeardetails { get; set; }
        public virtual ICollection<MBranch> MBranches { get; set; }
        public virtual ICollection<MFeature> MFeatures { get; set; }
        public virtual ICollection<MGroup> MGroups { get; set; }
        public virtual ICollection<MRole> MRoles { get; set; }
        public virtual ICollection<MSchoolDamapping> MSchoolDamappings { get; set; }
        public virtual ICollection<TCalendereventdetail> TCalendereventdetails { get; set; }
    }
}
