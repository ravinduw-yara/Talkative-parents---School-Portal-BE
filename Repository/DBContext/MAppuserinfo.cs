using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.DBContext
{
    public partial class MAppuserinfo
    {
        public MAppuserinfo()
        {
            AppuserdeviceAppusers = new HashSet<Appuserdevice>();
            AppuserdeviceCreatedbyNavigations = new HashSet<Appuserdevice>();
            AppuserdeviceModifiedbyNavigations = new HashSet<Appuserdevice>();
            MFeatureCreatedbyNavigations = new HashSet<MFeature>();
            MFeatureModifiedbyNavigations = new HashSet<MFeature>();
            MParentchildmappings = new HashSet<MParentchildmapping>();
            TEmaillogCreatedbyNavigations = new HashSet<TEmaillog>();
            TEmaillogModifiedbyNavigations = new HashSet<TEmaillog>();
            TNoticeboardmappings = new HashSet<TNoticeboardmapping>();
            TSoundingboardmessageAppuserinfos = new HashSet<TSoundingboardmessage>();
            TSoundingboardmessageCreatedbyNavigations = new HashSet<TSoundingboardmessage>();
            TSoundingboardmessageModifiedbyNavigations = new HashSet<TSoundingboardmessage>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public DateTime? Dob { get; set; }
        public string Phonenumber { get; set; }
        public int? Genderid { get; set; }
        public string Password { get; set; }
        public string Emailid { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public bool? Issmsuser { get; set; }
        public bool? Isofferoptedin { get; set; }
        public bool? EmailRegistered { get; set; }
        public string Otp { get; set; }


        public virtual MSchooluserinfo CreatedbyNavigation { get; set; }
        public virtual MGender Gender { get; set; }
        public virtual MSchooluserinfo ModifiedbyNavigation { get; set; }
        public virtual MStatus Status { get; set; }
        public virtual ICollection<Appuserdevice> AppuserdeviceAppusers { get; set; }
        public virtual ICollection<Appuserdevice> AppuserdeviceCreatedbyNavigations { get; set; }
        public virtual ICollection<Appuserdevice> AppuserdeviceModifiedbyNavigations { get; set; }
        public virtual ICollection<MFeature> MFeatureCreatedbyNavigations { get; set; }
        public virtual ICollection<MFeature> MFeatureModifiedbyNavigations { get; set; }
        public virtual ICollection<MParentchildmapping> MParentchildmappings { get; set; }
        public virtual ICollection<TEmaillog> TEmaillogCreatedbyNavigations { get; set; }
        public virtual ICollection<TEmaillog> TEmaillogModifiedbyNavigations { get; set; }
        public virtual ICollection<TNoticeboardmapping> TNoticeboardmappings { get; set; }
        public virtual ICollection<TSoundingboardmessage> TSoundingboardmessageAppuserinfos { get; set; }
        public virtual ICollection<TSoundingboardmessage> TSoundingboardmessageCreatedbyNavigations { get; set; }
        public virtual ICollection<TSoundingboardmessage> TSoundingboardmessageModifiedbyNavigations { get; set; }
    }
}
