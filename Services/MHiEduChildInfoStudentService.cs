
using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Services.MHiEduChildInfoStudentService;

namespace Services
{
    public interface IMHiEduChildInfoStudentService : ICommonService
    {
        Task<int> AddEntity(MChildinfo entity);
        Task<MChildinfo> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MChildinfo entity);
        Task<object> GetParentsListForAPI(Guid token, int nuofRows, int pageNumber, string standard = "", string section = "", string childlastname = "", string searchstring = "");
        Task<object> UpdateParentInfoForAPI(Guid token, ParentsUpdateModel model);
        Task<object> UpdateHieduStudentInfoAPI(Guid token, GetStudentParentsListModel model);
        Task<object> GetParentsListNewForAPI(Guid token, int nuofRows, int pageNumber, string standard = "", string section = "", string childlastname = "", string searchstring = "");
        Task<object> GetHiEduStudentDetailsByChildIdForAPI(Guid token, int schoolid, int childid, int nuofRows = 10, int pageNumber = 1);
        Task<object> GetStudentDetailsByParentIdAPI(int parentid, int nuofRows, int pageNumber);
        Task<object> GetsblingsListAPI(int parentId, int childId, int Schoolid);
        Task<object> GetStudentDetailsParentByChildIdAPI(int parentid, int childid, int nuofRows, int pageNumber);
        Task<object> DeleteHiEduStudentByChildId(int childid);



    }
    public class MHiEduChildInfoStudentService : IMHiEduChildInfoStudentService
    {
        private readonly IRepository<MChildinfo> repository;
        private DbSet<MChildinfo> localDBSet;
        private readonly IConfiguration configuration;
        readonly TpContext db = new TpContext();
        private readonly IMChildschoolmappingService mChildschoolmappingService;
        private readonly IMParentchildmappingService mParentchildmappingService;
        private readonly IMAppuserinfoService mAppuserinfoService;

        public MHiEduChildInfoStudentService(
            IRepository<MChildinfo> repository,
            IConfiguration configuration,
            IMChildschoolmappingService _mChildschoolmappingService,
            IMParentchildmappingService _mParentchildmappingService,
            IMAppuserinfoService _mAppuserinfoService
            )
        {
            this.repository = repository;
            this.configuration = configuration;
            mChildschoolmappingService = _mChildschoolmappingService;
            mParentchildmappingService = _mParentchildmappingService;
            mAppuserinfoService = _mAppuserinfoService;
        }

        #region base services
        private async Task AllEntityValue() => localDBSet = (DbSet<MChildinfo>)await this.repository.GetAll();

        private static Object Mapper(MChildinfo x) => new
        {
            x.Id,
            x.Code,
            x.Dob,
            x.Firstname,
            x.Middlename,
            x.Lastname,
            Gender = x.Genderid,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };


        private async Task<IQueryable<MChildinfo>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.MParentchildmappings)
            .Include(x => x.Gender)
            .Include(x => x.MChildschoolmappings)
            .Include(x => x.TSoundingboardmessages)
            .Include(x => x.Status);
        }



        public async Task<int> AddEntity(MChildinfo entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MChildinfo> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.Firstname.Equals(EntityName.Trim())).Select(x => Mapper(x));


        public async Task<int> UpdateEntity(MChildinfo entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        #endregion


        private async Task<IQueryable<GetParentsListModel>> GetParentsListSP(int? schoolid, string standard, string section, string childlastname, string searchstring, int? standardid, int? sectionid)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetParentsListModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetParentsListSP, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                command.Parameters["@schoolid"].Value = schoolid;
                command.Parameters.Add(new SqlParameter("@standard", SqlDbType.NVarChar));
                command.Parameters["@standard"].Value = standard;
                command.Parameters.Add(new SqlParameter("@section", SqlDbType.NVarChar));
                command.Parameters["@section"].Value = section;
                command.Parameters.Add(new SqlParameter("@childlastname", SqlDbType.NVarChar));
                command.Parameters["@childlastname"].Value = childlastname;
                command.Parameters.Add(new SqlParameter("@searchstring", SqlDbType.NVarChar));
                command.Parameters["@searchstring"].Value = searchstring;
                command.Parameters.Add(new SqlParameter("@standardid", SqlDbType.Int));
                command.Parameters["@standardid"].Value = standardid;
                command.Parameters.Add(new SqlParameter("@sectionid", SqlDbType.Int));
                command.Parameters["@sectionid"].Value = sectionid;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new GetParentsListModel()
                            {
                                id = (int)reader["id"],
                                ChildId = reader["childid"] != DBNull.Value ? (int?)reader["childid"] : null,
                                ChildSectionId = reader["childsectionid"] != DBNull.Value ? (int?)reader["childsectionid"] : null,
                                PhoneNumber = reader["phonenumber"].ToString(),
                                ParentsName = reader["parentsname"].ToString(),
                                UserName = reader["phonenumber"].ToString(),
                                Issmsuser = reader["Issmsuser"] != DBNull.Value ? (bool?)reader["Issmsuser"] : false,
                                EmailAddress = reader["emailid"].ToString(),
                                ParentsGender = reader["parentgender"].ToString(),
                                ChildFirstName = reader["firstname"].ToString(),
                                ChildLastName = reader["lastname"].ToString(),
                                ChildGender = reader["childgender"].ToString(),
                                ChildGenderId = reader["childgenderid"] != DBNull.Value ? (int?)reader["childgenderid"] : null,
                                ParentsRelation = reader["relationtype"].ToString(),
                                Schoolid = reader["schoolid"] != DBNull.Value ? (int?)reader["schoolid"] : null,
                                School = reader["schoolname"].ToString(),
                                StandardId = reader["standardid"] != DBNull.Value ? (int?)reader["standardid"] : null,
                                Standard = reader["standardname"].ToString(),
                                SectionId = reader["sectionid"] != DBNull.Value ? (int?)reader["sectionid"] : null,
                                Section = reader["sectionname"].ToString(),
                               // Parentcreateddate = reader["parentcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                               // Childcreateddate = reader["childcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                            }));

                        }
                    }
                }
                return res.AsQueryable().OrderByDescending(x => x.ChildFirstName);
            }
        }

        //Dinidu Modify  by 
        private async Task<IQueryable<GetStudentParentsListModel>> GetHiEduStudentDetailsByChildIdListSP(int? schoolid, int? childid, int nuofRows = 10, int pageNumber = 1)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetStudentParentsListModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetHieduStudentDetailsParentByChildIdAPIListSP, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                command.Parameters["@schoolid"].Value = schoolid;
                command.Parameters.Add(new SqlParameter("@childid", SqlDbType.Int));
                command.Parameters["@childid"].Value = childid;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new GetStudentParentsListModel()
                            {
                                /* id = (int)reader["id"],
                                 ChildId = reader["childids"] != DBNull.Value ? (int?)reader["childids"] : null,*/

                                /* MotherName = reader["mothername"].ToString(),
                                 FatherName = reader["fathername"].ToString(),
                                 MotherEmail = reader["motheremail"].ToString(),
                                 FatherEmail = reader["FatherEmail"].ToString(),
                                 MotherContactNo = reader["mothrephone"].ToString(),
                                 FatheerContactNo = reader["fatherphone"].ToString(),
                                 ChildFirstName = reader["childfname"].ToString(),
                                 ChildMiddletName = reader["childmname"].ToString(),
                                 ChildLastName = reader["childlastname"].ToString(),
                                 DateOfBirth = reader["dob"] != DBNull.Value ? (DateTime?)reader["dob"] : null,
                                 childphone = reader["phonenumber"].ToString(),
                                 nic = reader["nic"].ToString(),
                                 childbatch = reader["batch"].ToString(),
                                 childcourse = reader["course"].ToString(),
                                 childadmissinno = reader["admissinno"].ToString(),
                                 childHomeaddress = reader["Homeaddress"].ToString(),
                                 childHouse = reader["House"].ToString(),
                                 childHosteler = reader["Hosteler"].ToString(),
                                 childReligion = reader["Religion"].ToString(),
                                 childMedium = reader["Medium "].ToString(),
                                 childAdmissionYear = reader["AdmissionYear "].ToString(),
                                 childLeavingYear = reader["LeavingYear "].ToString(),
                                 BloodGroup = reader["Bloodgroup"].ToString(),
                                 MedicalConditions = reader["medicalcondition"].ToString(),
                                 SpecialNeeds = reader["specialneeds"].ToString(),
                                 clubname = reader["clubname"].ToString(),
                                 Other = reader["clubother"].ToString(),
                                 Contact1 = reader["contact1"].ToString(),
                                 Relationship1 = reader["realationship1"].ToString(),
                                 MobileNo1 = reader["mobile1"].ToString(),
                                 Contact2 = reader["contact2"].ToString(),
                                 Relationship2 = reader["realationship2"].ToString(),
                                 MobileNo2 = reader["mobile2"].ToString(),
                                 aluminifather = reader["aluminifather"].ToString(),
                                 aluminimother = reader["aluminimother"].ToString(),
                                 Parentmothername = reader["parentmothername"].ToString(),
                                 Parentmothecontectno = reader["parentmothecontectno"].ToString(),
                                 Parentmotheremail = reader["parentmotheremail"].ToString(),
                                 Parentfathername = reader["parentfathername"].ToString(),
                                 Parentfathercontectno = reader["parentfathercontectno"].ToString(),
                                 Parentfatheremail = reader["parentfatheremail"].ToString(),
                                 GuardianName = reader["GuardianName"].ToString(),
                                 GuardianContactNo = reader["GuardianContactNo"].ToString(),
                                 GuardianEmail = reader["GuardianEmail"].ToString(),
                                 IsMotherPastStudent = reader["IsMotherPastStudent"].ToString(),
                                 IsFatherPastStudent = reader["IsFatherPastStudent"].ToString(),
                                 Scholarship = reader["Scholarship"].ToString(),
                                 Discipline = reader["Discipline"].ToString(),

                                 SportName = reader["sportname"].ToString(),*/

                                //    id = (int)reader["id"],
                                //       ChildId = reader["childids"] != DBNull.Value ? (int?)reader["childids"] : null,
                                //  ChildSectionId = reader["SectionId"] != DBNull.Value ? (int?)reader["SectionId"] : null,
                                //  PhoneNumber = reader["phonenumber"].ToString(),*/
                                //     id = (int)reader["id"],
                                ChildId = reader["childid"] != DBNull.Value ? (int?)reader["childid"] : null,
                                //   MotherName = reader["mothername"].ToString(),
                                //     FatherName = reader["fathername"].ToString(),
                                //    MotherEmail = reader["motheremail"].ToString(),
                                //     FatherEmail = reader["FatherEmail"].ToString(),
                                //    MotherContactNo = reader["motherphone"].ToString(),
                                //      FatheerContactNo = reader["fatherphone"].ToString(),
                                ChildFirstName = reader["childfname"].ToString(),
                                ChildMiddletName = reader["childmname"].ToString(),
                                ChildLastName = reader["childmname"].ToString(),
                                DateOfBirth = reader["dob"] != DBNull.Value ? (DateTime?)reader["dob"] : null,
                                childphone = reader["phonenumber"].ToString(),
                                nic = reader["nic"].ToString(),
                                childbatch = reader["batch"].ToString(),
                                childcourse = reader["course"].ToString(),
                                childadmissinno = reader["admissinno"].ToString(),
                                childHomeaddress = reader["Homeaddress"].ToString(),
                                childHouse = reader["House"].ToString(),
                                childHosteler = reader["Hosteler"].ToString(),
                                childReligion = reader["Religion"].ToString(),
                                childMedium = reader["Medium"].ToString(),
                                childAdmissionYear = reader["AdmissionYear"].ToString(),
                                childLeavingYear = reader["LeavingYear"].ToString(),
                                BloodGroup = reader["Bloodgroup"].ToString(),
                                MedicalConditions = reader["medicalcondition"].ToString(),
                                SpecialNeeds = reader["specialneeds"].ToString(),
                                clubname = reader["clubname"].ToString(),
                                Other = reader["clubother"].ToString(),
                                Contact1 = reader["contact1"].ToString(),
                                Relationship1 = reader["realationship1"].ToString(),
                                MobileNo1 = reader["mobile1"].ToString(),
                                Contact2 = reader["contact2"].ToString(),
                                Relationship2 = reader["realationship2"].ToString(),
                                MobileNo2 = reader["mobile2"].ToString(),
                                //  aluminifather = reader["aluminifather"].ToString(),
                                //    aluminimother = reader["aluminimother"].ToString(),
                                Parentmothername = reader["parentmothername"].ToString(),
                                Parentmothecontectno = reader["parentmothecontectno"].ToString(),
                                Parentmotheremail = reader["parentmotheremail"].ToString(),
                                Parentfathername = reader["parentfathername"].ToString(),
                                Parentfathercontectno = reader["parentfathercontectno"].ToString(),
                                Parentfatheremail = reader["parentfatheremail"].ToString(),
                                GuardianName = reader["GuardianName"].ToString(),
                                GuardianContactNo = reader["GuardianContactNo"].ToString(),
                                GuardianEmail = reader["GuardianEmail"].ToString(),
                                IsMotherPastStudent = reader["IsMotherPastStudent"].ToString(),
                                IsFatherPastStudent = reader["IsFatherPastStudent"].ToString(),
                                Scholarship = reader["Scholarship"].ToString(),
                                Discipline = reader["Discipline"].ToString(),

                                SportName = reader["sportname"].ToString(),

                                IsOtherParentSMSUser = reader["IsOtherParentSMSUser"] != DBNull.Value ? (bool?)reader["Issmsuser"] : false,
                                OtherParentFirstName = reader["OtherParentFirstName"].ToString(),
                                OtherParentLastName = reader["OtherParentLastName"].ToString(),
                                OtherParentEmailAddress = reader["OtherParentEmailAddress"].ToString(),
                                OtherParentPhoneNumber = reader["OtherParentPhoneNumber"].ToString(),
                                Childsgender = reader["childsgender"].ToString(),
                                //  Parentsgender = reader["parentsgender"].ToString(),
                                //   Parentsrelation = reader["parentsrelation"].ToString(),

                                Schoolid = reader["SchoolId"].ToString(),
                                childemail = reader["childemail"].ToString(),

                            }));

                        }
                    }
                }
                return res.AsQueryable().OrderByDescending(x => x.ChildFirstName);
            }
        }

        //Sanduni Delete HiEdu Student
        public async Task<object> DeleteHiEduStudentByChildId(int childid)
        {
            try
            {
              
                if (childid != null)
                {
                    var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(ApplicationConstants.DeleteHiEduStudent, connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@childid", SqlDbType.Int));
                        command.Parameters["@childid"].Value = childid;


                        command.ExecuteNonQuery();
                    }
                    db.SaveChanges();

                }
                else
                {
                    return ("Student Id is Invalid");
                }
                return ("Student Record is Deleted Successfully");
            }

            catch (Exception)
            {
                throw;
            }
        }
        private async Task<IQueryable<GetParentsListModel>> GetStudentDetailsByParentIdAPIListSP(int? parentid)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetParentsListModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetStudentDetailsByParentIdAPIListSP, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@parentid", SqlDbType.Int));
                command.Parameters["@parentid"].Value = parentid;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new GetParentsListModel()
                            {
                                id = (int)reader["id"],
                                ChildId = reader["childid"] != DBNull.Value ? (int?)reader["childid"] : null,
                                ChildSectionId = reader["SectionId"] != DBNull.Value ? (int?)reader["SectionId"] : null,
                                PhoneNumber = reader["phonenumber"].ToString(),
                                ParentsName = reader["parentsname"].ToString(),
                                Issmsuser = reader["issmsuser"] != DBNull.Value ? (bool?)reader["issmsuser"] : false,
                                EmailAddress = reader["ChildEmail"].ToString(),
                                ParentsGender = reader["parentsgender"].ToString(),
                                ChildFirstName = reader["childFirstName"].ToString(),
                                ChildLastName = reader["childLastName"].ToString(),
                                ChildGender = reader["childsgender"].ToString(),
                                ChildGenderId = reader["genderid"] != DBNull.Value ? (int?)reader["genderid"] : null,
                                ParentsRelation = reader["parentsrelation"].ToString(),
                                Schoolid = reader["schoolid"] != DBNull.Value ? (int?)reader["schoolid"] : null,
                                School = reader["schoolname"].ToString(),
                                StandardId = reader["StandardId"] != DBNull.Value ? (int?)reader["StandardId"] : null,
                                Standard = reader["StandardName"].ToString(),
                                SectionId = reader["ChildSchoolMappingId"] != DBNull.Value ? (int?)reader["sectionid"] : null,
                                Section = reader["SectionName"].ToString(),
                               // Parentcreateddate = reader["parentcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                               // Childcreateddate = reader["childcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                                BloodGroup = reader["BloodGroup"].ToString(),
                                MedicalConditions = reader["MedicalConditions"].ToString(),
                                SpecialNeeds = reader["SpecialNeeds"].ToString(),
                                ClubName = reader["ClubName"].ToString(),
                                RegisterationNumber = reader["registerationnumber"].ToString(),
                                HomeAddress = reader["HomeAddress"].ToString(),
                                House = reader["House"].ToString(),
                                Hosteler = reader["Hosteler"].ToString(),
                                Religion = reader["Religion"].ToString(),
                                Medium = reader["Medium"].ToString(),
                                AdmissionYear = reader["AdmissionYear"].ToString(),
                                MobileNo1 = reader["MobileNo1"].ToString(),
                                MobileNo2 = reader["MobileNo2"].ToString(),
                                Contact1 = reader["Contact1"].ToString(),
                                Contact2 = reader["Contact2"].ToString(),
                                Prefectship = reader["Prefectship"].ToString(),
                                Scholarship = reader["Scholarship"].ToString(),
                                Discipline = reader["Discipline"].ToString(),
                                SportName = reader["SportName"].ToString(),
                                GuardianEmail = reader["GuardianEmail"].ToString(),
                                Relationship1 = reader["Relationship1"].ToString(),
                                Relationship2 = reader["Relationship2"].ToString(),
                                MotherEmail = reader["MotherEmail"].ToString(),
                                MotherContactNo = reader["MotherContactNo"].ToString(),
                                FatheerContactNo = reader["FatheerContactNo"].ToString(),
                                FatherEmail = reader["FatherEmail"].ToString(),
                                GuardianName = reader["GuardianName"].ToString(),
                                LeavingYear = reader["LeavingYear"].ToString(),
                                GuardianContactNo = reader["GuardianContactNo"].ToString(),
                                Other = reader["Other"].ToString(),
                                IsMotherPastStudent = reader["IsMotherPastStudent"].ToString(),
                                IsFatherPastStudent = reader["IsFatherPastStudent"].ToString(),
                                // Picture = reader["Picture"].ToString(),
                                IsOtherParentSMSUser = reader["IsOtherParentSMSUser"] != DBNull.Value ? (bool?)reader["Issmsuser"] : false,
                                OtherParentFirstName = reader["OtherParentFirstName"].ToString(),
                                OtherParentLastName = reader["OtherParentLastName"].ToString(),
                                OtherParentEmailAddress = reader["OtherParentEmailAddress"].ToString(),
                                OtherParentPhoneNumber = reader["OtherParentPhoneNumber"].ToString(),
                                DateOfBirth = reader["dob"] != DBNull.Value ? (DateTime?)reader["dob"] : null,
                                schoollogo = reader["schoollogo"].ToString(),
                                ParentPortalEnabled = reader["ParentPortalEnabled"] != DBNull.Value ? (bool?)reader["ParentPortalEnabled"] : false,

                            }));

                        }
                    }
                }
                return res.AsQueryable().OrderByDescending(x => x.ChildFirstName);
            }
        }

        private async Task<IQueryable<GetParentsListModel>> GetsblingsListSp(int parentId, int childId, int Schoolid)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetParentsListModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetsblingsListAPIListSP, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@parentId", SqlDbType.Int));
                command.Parameters["@parentId"].Value = parentId;
                command.Parameters.Add(new SqlParameter("@childId", SqlDbType.Int));
                command.Parameters["@childId"].Value = childId;
                command.Parameters.Add(new SqlParameter("@Schoolid", SqlDbType.Int));
                command.Parameters["@Schoolid"].Value = Schoolid;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new GetParentsListModel()
                            {
                                ChildId = (int)reader["childid"],
                                ChildGenderId = reader["genderid"] != DBNull.Value ? (int?)reader["genderid"] : null,
                                ChildFirstName = reader["firstname"].ToString(),
                                ChildLastName = reader["lastname"].ToString(),
                                School = reader["schoolname"].ToString(),
                                Standard = reader["standardname"].ToString(),
                                Section = reader["sectionname"].ToString(),
                            }));

                        }
                    }
                }
                return res.AsQueryable().OrderByDescending(x => x.ChildFirstName);
            }
        }



        private async Task<IQueryable<GetParentsListModel>> GetParentsListNewSP(int? schoolid, string standard, string section, string childlastname, string searchstring, int? standardid, int? sectionid)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetParentsListModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetParentsNewListSP, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                command.Parameters["@schoolid"].Value = schoolid;
                command.Parameters.Add(new SqlParameter("@standard", SqlDbType.NVarChar));
                command.Parameters["@standard"].Value = standard;
                command.Parameters.Add(new SqlParameter("@section", SqlDbType.NVarChar));
                command.Parameters["@section"].Value = section;
                command.Parameters.Add(new SqlParameter("@childlastname", SqlDbType.NVarChar));
                command.Parameters["@childlastname"].Value = childlastname;
                command.Parameters.Add(new SqlParameter("@searchstring", SqlDbType.NVarChar));
                command.Parameters["@searchstring"].Value = searchstring;
                command.Parameters.Add(new SqlParameter("@standardid", SqlDbType.Int));
                command.Parameters["@standardid"].Value = standardid;
                command.Parameters.Add(new SqlParameter("@sectionid", SqlDbType.Int));
                command.Parameters["@sectionid"].Value = sectionid;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new GetParentsListModel()
                            {
                                id = (int)reader["id"],
                                ChildId = reader["childid"] != DBNull.Value ? (int?)reader["childid"] : null,
                                ChildSectionId = reader["childsectionid"] != DBNull.Value ? (int?)reader["childsectionid"] : null,
                                PhoneNumber = reader["phonenumber"].ToString(),
                                ParentsName = reader["parentsname"].ToString(),
                                UserName = reader["phonenumber"].ToString(),
                                Issmsuser = reader["Issmsuser"] != DBNull.Value ? (bool?)reader["Issmsuser"] : false,
                                EmailAddress = reader["emailid"].ToString(),
                                ParentsGender = reader["parentgender"].ToString(),
                                ChildFirstName = reader["firstname"].ToString(),
                                ChildLastName = reader["lastname"].ToString(),
                                ChildGender = reader["childgender"].ToString(),
                                ChildGenderId = reader["childgenderid"] != DBNull.Value ? (int?)reader["childgenderid"] : null,
                                ParentsRelation = reader["relationtype"].ToString(),
                                Schoolid = reader["schoolid"] != DBNull.Value ? (int?)reader["schoolid"] : null,
                                School = reader["schoolname"].ToString(),
                                StandardId = reader["standardid"] != DBNull.Value ? (int?)reader["standardid"] : null,
                                Standard = reader["standardname"].ToString(),
                                SectionId = reader["sectionid"] != DBNull.Value ? (int?)reader["sectionid"] : null,
                                Section = reader["sectionname"].ToString(),
                               // Parentcreateddate = reader["parentcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                               // Childcreateddate = reader["childcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                            }));

                        }
                    }
                }
                return res.AsQueryable().OrderByDescending(x => x.ChildFirstName);
            }
        }


        //Dinidu New  one
        private async Task<IQueryable<GetStudentParentsListModel>> GetStudentDetailsParentByChildIdAPIListSP(int? parentid, int? childid)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetStudentParentsListModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetHieduStudentDetailsParentByChildIdAPIListSP, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@parentid", SqlDbType.Int));
                command.Parameters["@parentid"].Value = parentid;
                command.Parameters.Add(new SqlParameter("@childid", SqlDbType.Int));
                command.Parameters["@childid"].Value = childid;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new GetStudentParentsListModel()
                            {
                                //     id = (int)reader["id"],
                                ChildId = reader["childid"] != DBNull.Value ? (int?)reader["childid"] : null,
                                //    MotherName = reader["mother"].ToString(),
                                //     FatherName = reader["father"].ToString(),
                                //     MotherEmail = reader["motheremail"].ToString(),
                                //     FatherEmail = reader["FatherEmail"].ToString(),
                                //     MotherContactNo = reader["mothrephone"].ToString(),
                                //     FatheerContactNo = reader["fatherphone"].ToString(),
                                ChildFirstName = reader["childfname"].ToString(),
                                ChildMiddletName = reader["childmname"].ToString(),
                                ChildLastName = reader["childmname"].ToString(),
                                DateOfBirth = reader["dob"] != DBNull.Value ? (DateTime?)reader["dob"] : null,
                                childphone = reader["phonenumber"].ToString(),
                                nic = reader["nic"].ToString(),
                                childbatch = reader["batch"].ToString(),
                                childcourse = reader["course"].ToString(),
                                childadmissinno = reader["admissinno"].ToString(),
                                childHomeaddress = reader["Homeaddress"].ToString(),
                                childHouse = reader["House"].ToString(),
                                childHosteler = reader["Hosteler"].ToString(),
                                childReligion = reader["Religion"].ToString(),
                                childMedium = reader["Medium "].ToString(),
                                childAdmissionYear = reader["AdmissionYear "].ToString(),
                                childLeavingYear = reader["LeavingYear "].ToString(),
                                BloodGroup = reader["Bloodgroup"].ToString(),
                                MedicalConditions = reader["medicalcondition"].ToString(),
                                SpecialNeeds = reader["specialneeds"].ToString(),
                                clubname = reader["clubname"].ToString(),
                                Other = reader["clubother"].ToString(),
                                Contact1 = reader["contact1"].ToString(),
                                Relationship1 = reader["realationship1"].ToString(),
                                MobileNo1 = reader["mobile1"].ToString(),
                                Contact2 = reader["contact2"].ToString(),
                                Relationship2 = reader["realationship2"].ToString(),
                                MobileNo2 = reader["mobile2"].ToString(),
                                // aluminifather = reader["aluminifather"].ToString(),
                                // aluminimother = reader["aluminimother"].ToString(),
                                Parentmothername = reader["parentmothername"].ToString(),
                                Parentmothecontectno = reader["parentmothecontectno"].ToString(),
                                Parentmotheremail = reader["parentmotheremail"].ToString(),
                                Parentfathername = reader["parentfathername"].ToString(),
                                Parentfathercontectno = reader["parentfathercontectno"].ToString(),
                                Parentfatheremail = reader["parentfatheremail"].ToString(),
                                GuardianName = reader["GuardianName"].ToString(),
                                GuardianContactNo = reader["GuardianContactNo"].ToString(),
                                GuardianEmail = reader["GuardianEmail"].ToString(),
                                IsMotherPastStudent = reader["IsMotherPastStudent"].ToString(),
                                IsFatherPastStudent = reader["IsFatherPastStudent"].ToString(),
                                Scholarship = reader["Scholarship"].ToString(),
                                Discipline = reader["Discipline"].ToString(),

                                SportName = reader["portname"].ToString(),

                                IsOtherParentSMSUser = reader["IsOtherParentSMSUser"] != DBNull.Value ? (bool?)reader["Issmsuser"] : false,
                                OtherParentFirstName = reader["OtherParentFirstName"].ToString(),
                                OtherParentLastName = reader["OtherParentLastName"].ToString(),
                                OtherParentEmailAddress = reader["OtherParentEmailAddress"].ToString(),
                                OtherParentPhoneNumber = reader["OtherParentPhoneNumber"].ToString(),

                                /* id = (int)reader["id"],
                                 ChildId = reader["childid"] != DBNull.Value ? (int?)reader["childid"] : null,
                                // ChildSectionId = reader["SectionId"] != DBNull.Value ? (int?)reader["SectionId"] : null,
                                 PhoneNumber = reader["phonenumber"].ToString(),
                                 ParentsName = reader["parentsname"].ToString(),
                                 Issmsuser = reader["issmsuser"] != DBNull.Value ? (bool?)reader["issmsuser"] : false,
                                 EmailAddress = reader["ChildEmail"].ToString(),
                                 ParentsGender = reader["parentsgender"].ToString(),
                                 ChildFirstName = reader["childFirstName"].ToString(),
                                 ChildLastName = reader["childLastName"].ToString(),
                                 ChildGender = reader["childsgender"].ToString(),
                                 ChildGenderId = reader["genderid"] != DBNull.Value ? (int?)reader["genderid"] : null,
                                 ParentsRelation = reader["parentsrelation"].ToString(),
                               //  Schoolid = reader["schoolid"] != DBNull.Value ? (int?)reader["schoolid"] : null,
                               //  School = reader["schoolname"].ToString(),
                               //  StandardId = reader["StandardId"] != DBNull.Value ? (int?)reader["StandardId"] : null,
                               //  Standard = reader["StandardName"].ToString(),
                               //  SectionId = reader["ChildSchoolMappingId"] != DBNull.Value ? (int?)reader["sectionid"] : null,
                               //  Section = reader["SectionName"].ToString(),
                                 Parentcreateddate = reader["parentcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                                 Childcreateddate = reader["childcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                                 BloodGroup = reader["BloodGroup"].ToString(),
                                 MedicalConditions = reader["MedicalConditions"].ToString(),
                                 SpecialNeeds = reader["SpecialNeeds"].ToString(),
                                 ClubName = reader["ClubName"].ToString(),
                                 RegisterationNumber = reader["registerationnumber"].ToString(),//acdamic id
                                 HomeAddress = reader["HomeAddress"].ToString(),
                                 House = reader["House"].ToString(),
                                 Hosteler = reader["Hosteler"].ToString(),
                                 Religion = reader["Religion"].ToString(),
                                 Medium = reader["Medium"].ToString(),
                                 AdmissionYear = reader["AdmissionYear"].ToString(),
                                 MobileNo1 = reader["MobileNo1"].ToString(),
                                 MobileNo2 = reader["MobileNo2"].ToString(),
                                 Contact1 = reader["Contact1"].ToString(),
                                 Contact2 = reader["Contact2"].ToString(),
                                 Prefectship = reader["Prefectship"].ToString(),
                                 Scholarship = reader["Scholarship"].ToString(),
                                 Discipline = reader["Discipline"].ToString(),
                                 SportName = reader["SportName"].ToString(),
                                 GuardianEmail = reader["GuardianEmail"].ToString(),
                                 Relationship1 = reader["Relationship1"].ToString(),
                                 Relationship2 = reader["Relationship2"].ToString(),
                                 MotherEmail = reader["MotherEmail"].ToString(),
                                 MotherContactNo = reader["MotherContactNo"].ToString(),
                                 FatheerContactNo = reader["FatheerContactNo"].ToString(),
                                 FatherEmail = reader["FatherEmail"].ToString(),
                                 GuardianName = reader["GuardianName"].ToString(),
                                 LeavingYear = reader["LeavingYear"].ToString(),
                                 GuardianContactNo = reader["GuardianContactNo"].ToString(),
                                 Other = reader["Other"].ToString(),
                                 IsMotherPastStudent = reader["IsMotherPastStudent"].ToString(),
                                 IsFatherPastStudent = reader["IsFatherPastStudent"].ToString(),
                                 // Picture = reader["Picture"].ToString(),
                                 IsOtherParentSMSUser = reader["IsOtherParentSMSUser"] != DBNull.Value ? (bool?)reader["Issmsuser"] : false,
                                 OtherParentFirstName = reader["OtherParentFirstName"].ToString(),
                                 OtherParentLastName = reader["OtherParentLastName"].ToString(),
                                 OtherParentEmailAddress = reader["OtherParentEmailAddress"].ToString(),
                                 OtherParentPhoneNumber = reader["OtherParentPhoneNumber"].ToString(),
                                 DateOfBirth = reader["dob"] != DBNull.Value ? (DateTime?)reader["dob"] : null,*/
                                //  schoollogo = reader["schoollogo"].ToString(),
                                //  ParentPortalEnabled = reader["ParentPortalEnabled"] != DBNull.Value ? (bool?)reader["ParentPortalEnabled"] : false,

                            }));

                        }
                    }
                }
                return res.AsQueryable().OrderByDescending(x => x.ChildFirstName);
            }
        }

        private List<GetUserStdSec> UserStdSecs(int? userid)
        {
            try
            {
                //admin check
                var check = db.MSchooluserroles.Where(x => x.Schooluserid.Equals(userid)).FirstOrDefault();
                if (check != null && check.Standardsectionmappingid == null)
                {

                    var result = (from ur in db.MSchooluserroles
                                  join si in db.MSchooluserinfos on ur.Schooluserid equals si.Id
                                  join br in db.MBranches on si.Branchid equals br.Id
                                  where ur.Schooluserid == userid
                                  select new
                                  {
                                      id = ur.Id,
                                      schooluserid = ur.Schooluserid,
                                      schoolid = br.Schoolid
                                  }).ToList();
                    List<GetUserStdSec> Guslist = new List<GetUserStdSec>();
                    foreach (var item in result)
                    {
                        GetUserStdSec gus = new GetUserStdSec();
                        gus.Id = item.id;
                        gus.schooluserid = item.schooluserid;
                        gus.schoolid = item.schoolid;
                        gus.sectionid = null;
                        gus.standardid = null;
                        Guslist.Add(gus);
                    }
                    return Guslist;
                }
                else
                {
                    var result = (from ur in db.MSchooluserroles
                                  join sm in db.MStandardsectionmappings on ur.Standardsectionmappingid equals sm.Id
                                  join br in db.MBranches on sm.Branchid equals br.Id
                                  where ur.Schooluserid == userid
                                  select new
                                  {
                                      id = ur.Id,
                                      schooluserid = ur.Schooluserid,
                                      standandardsectionmappingid = sm.Id,
                                      parentid = sm.Parentid,
                                      schoolid = br.Schoolid
                                  }).ToList();
                    List<GetUserStdSec> Guslist = new List<GetUserStdSec>();
                    foreach (var item in result)
                    {
                        GetUserStdSec gus = new GetUserStdSec();
                        gus.Id = item.id;
                        gus.schooluserid = item.schooluserid;
                        gus.schoolid = item.schoolid;
                        if (item.parentid == null)
                        {
                            gus.sectionid = null;
                            gus.standardid = item.standandardsectionmappingid;
                        }
                        else
                        {
                            gus.sectionid = item.standandardsectionmappingid;
                            gus.standardid = item.parentid;
                        }
                        Guslist.Add(gus);
                    }
                    return Guslist;
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public async Task<object> GetParentsListForAPI(Guid token, int nuofRows, int pageNumber, string standard = "", string section = "", string childlastname = "", string searchstring = "")
        {
            try
            {
                var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();
                if (userid != null)
                {
                    var details = UserStdSecs(userid);
                    if (details.Count > 0)
                    {
                        foreach (var item in details)
                        {
                            var objresult = await this.GetParentsListSP(item.schoolid, standard, section, childlastname, searchstring, item.standardid, item.sectionid);
                            if (objresult != null)
                            {
                                int count = objresult.Count();
                                int CurrentPage = pageNumber;
                                int PageSize = nuofRows;
                                int TotalCount = count;
                                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                                var items = objresult.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                                var obj = new
                                {
                                    TotalPages = TotalPages,
                                    items = items
                                };
                                return obj;
                            }
                            else
                            {
                                return (new
                                {
                                    Message = "No data found",
                                    StatusCode = HttpStatusCode.NotFound

                                });
                            }
                        }
                    }
                    else
                    {
                        return (new
                        {
                            Message = "User not found",
                            StatusCode = HttpStatusCode.NotFound

                        });
                    }
                }
                else
                {
                    return (new
                    {
                        Message = "User not found",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }
        public async Task<object> GetsblingsListAPI(int parentId, int childId, int Schoolid)
        {
            try
            {
                // var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();

                var objresult = await this.GetsblingsListSp(parentId, childId, Schoolid);
                if (objresult != null)
                {
                    int count = objresult.Count();
                    //int CurrentPage = 10;
                    //int PageSize = 2;
                    //int TotalCount = count;
                    //int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                    var items = objresult.ToList();
                    // var previousPage = CurrentPage > 1 ? "Yes" : "No";
                    // var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                    var obj = new
                    {
                        // TotalPages = TotalPages,
                        items = items
                    };
                    return obj;
                }
                else
                {
                    return (new
                    {
                        Message = "No data found",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }

            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }
        public async Task<object> GetParentsListNewForAPI(Guid token, int nuofRows, int pageNumber, string standard = "", string section = "", string childlastname = "", string searchstring = "")
        {
            try
            {
                var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();
                if (userid != null)
                {
                    var details = UserStdSecs(userid);
                    if (details.Count > 0)
                    {
                        foreach (var item in details)
                        {
                            var objresult = await this.GetParentsListNewSP(item.schoolid, standard, section, childlastname, searchstring, item.standardid, item.sectionid);
                            if (objresult != null)
                            {
                                int count = objresult.Count();
                                int CurrentPage = pageNumber;
                                int PageSize = nuofRows;
                                int TotalCount = count;
                                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                                var items = objresult.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                                var obj = new
                                {
                                    TotalPages = TotalPages,
                                    items = items
                                };
                                return obj;
                            }
                            else
                            {
                                return (new
                                {
                                    Message = "No data found",
                                    StatusCode = HttpStatusCode.NotFound

                                });
                            }
                        }
                    }
                    else
                    {
                        return (new
                        {
                            Message = "User not found",
                            StatusCode = HttpStatusCode.NotFound

                        });
                    }
                }
                else
                {
                    return (new
                    {
                        Message = "User not found",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }

        //Dinidu Modify by
        public async Task<object> GetHiEduStudentDetailsByChildIdForAPI(Guid token, int schoolid, int childid, int nuofRows = 10, int pageNumber = 1)
        {
            try
            {
                //    var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();
                var userid = 1;
                if (userid != null)
                {
                    var details = UserStdSecs(userid);
                    if (details.Count > 0)
                    {
                        foreach (var item in details)
                        {
                            var objresult = await this.GetHiEduStudentDetailsByChildIdListSP(schoolid, childid);
                            if (objresult != null)
                            {
                                int count = objresult.Count();
                                int CurrentPage = pageNumber;
                                int PageSize = nuofRows;
                                int TotalCount = count;
                                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                                var items = objresult.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                                var obj = new
                                {
                                    TotalPages = TotalPages,
                                    items = items
                                };
                                return obj;
                            }
                            else
                            {
                                return (new
                                {
                                    Message = "No data found",
                                    StatusCode = HttpStatusCode.NotFound

                                });
                            }
                        }
                    }
                    else
                    {
                        return (new
                        {
                            Message = "User not found",
                            StatusCode = HttpStatusCode.NotFound

                        });
                    }
                }
                else
                {
                    return (new
                    {
                        Message = "User not found",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }

        public async Task<object> GetStudentDetailsByParentIdAPI(int parentid, int nuofRows = 10, int pageNumber = 1)
        {
            try
            {
                // var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();
                if (parentid != null)
                {

                    var objresult = await this.GetStudentDetailsByParentIdAPIListSP(parentid);
                    if (objresult != null)
                    {
                        int count = objresult.Count();
                        int CurrentPage = pageNumber;
                        int PageSize = nuofRows;
                        int TotalCount = count;
                        int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                        var items = objresult.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                        var previousPage = CurrentPage > 1 ? "Yes" : "No";
                        var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                        var obj = new
                        {
                            TotalPages = TotalPages,
                            items = items
                        };
                        return obj;
                    }
                    else
                    {
                        return (new
                        {
                            Message = "No data found",
                            StatusCode = HttpStatusCode.NotFound

                        });
                    }
                }
                else
                {
                    return (new
                    {
                        Message = "User not found",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                {
                    return (new
                    {
                        Message = "User not found",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }
        public async Task<object> GetStudentDetailsParentByChildIdAPI(int parentid, int childid, int nuofRows = 10, int pageNumber = 1)
        {
            try
            {
                //   var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();
                if (parentid != null)
                {

                    var objresult = await this.GetStudentDetailsParentByChildIdAPIListSP(parentid, childid);
                    if (objresult != null)
                    {
                        int count = objresult.Count();
                        int CurrentPage = pageNumber;
                        int PageSize = nuofRows;
                        int TotalCount = count;
                        int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                        var items = objresult.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                        var previousPage = CurrentPage > 1 ? "Yes" : "No";
                        var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                        var obj = new
                        {
                            TotalPages = TotalPages,
                            items = items
                        };
                        return obj;
                    }
                    else
                    {
                        return (new
                        {
                            Message = "No data found",
                            StatusCode = HttpStatusCode.NotFound

                        });
                    }
                }
                else
                {
                    return (new
                    {
                        Message = "User not found",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                {
                    return (new
                    {
                        Message = "User not found",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }

        public async Task<object> UpdateHieduStudentInfoAPI(Guid token, GetStudentParentsListModel model)
        {
            try
            {
                //  var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();
                int userid = 1;
                // if (userid is not null) // checks if records are updated

                if (userid != null)
                {


                    var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(ApplicationConstants.UpdateHieduStudentInfoAPI, connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                        command.Parameters["@schoolid"].Value = model.Schoolid;

                        command.Parameters.Add(new SqlParameter("@parentid", SqlDbType.Int));
                        command.Parameters["@parentid"].Value = model.id;

                        command.Parameters.Add(new SqlParameter("@childid", SqlDbType.Int));
                        command.Parameters["@childid"].Value = model.ChildId;

                        command.Parameters.Add(new SqlParameter("@phonenumber", SqlDbType.NVarChar));
                        command.Parameters["@phonenumber"].Value = model.childphone;

                        command.Parameters.Add(new SqlParameter("@SportType ", SqlDbType.NVarChar));
                        command.Parameters["@SportType "].Value = model.SportName;

                        command.Parameters.Add(new SqlParameter("@Blood_Group", SqlDbType.NVarChar));
                        command.Parameters["@Blood_Group"].Value = model.BloodGroup;

                        command.Parameters.Add(new SqlParameter("@Medical_Condition", SqlDbType.NVarChar));
                        command.Parameters["@Medical_Condition"].Value = model.MedicalConditions;

                        command.Parameters.Add(new SqlParameter("@Special_Needs", SqlDbType.NVarChar));
                        command.Parameters["@Special_Needs"].Value = model.SpecialNeeds;

                        command.Parameters.Add(new SqlParameter("@ClubName", SqlDbType.NVarChar));
                        command.Parameters["@ClubName"].Value = model.clubname;

                        command.Parameters.Add(new SqlParameter("@AdminssionNumber", SqlDbType.NVarChar));
                        command.Parameters["@AdminssionNumber"].Value = model.childadmissinno;

                        command.Parameters.Add(new SqlParameter("@HomeAddress", SqlDbType.NVarChar));
                        command.Parameters["@HomeAddress"].Value = model.childHomeaddress;

                        command.Parameters.Add(new SqlParameter("@House", SqlDbType.NVarChar));
                        command.Parameters["@House"].Value = model.childHouse;

                        command.Parameters.Add(new SqlParameter("@Hosteler", SqlDbType.NVarChar));
                        command.Parameters["@Hosteler"].Value = model.childHosteler;

                        command.Parameters.Add(new SqlParameter("@Religion", SqlDbType.NVarChar));
                        command.Parameters["@Religion"].Value = model.childReligion;

                        command.Parameters.Add(new SqlParameter("@Medium", SqlDbType.NVarChar));
                        command.Parameters["@Medium"].Value = model.childMedium;

                        command.Parameters.Add(new SqlParameter("@AdmissionYear", SqlDbType.NVarChar));
                        command.Parameters["@AdmissionYear"].Value = model.childAdmissionYear;

                        command.Parameters.Add(new SqlParameter("@LeavingYear", SqlDbType.NVarChar));
                        command.Parameters["@LeavingYear"].Value = model.childLeavingYear;

                        command.Parameters.Add(new SqlParameter("@Contact1", SqlDbType.NVarChar));
                        command.Parameters["@Contact1"].Value = model.Contact1;

                        command.Parameters.Add(new SqlParameter("@Relationship1", SqlDbType.NVarChar));
                        command.Parameters["@Relationship1"].Value = model.Relationship1;

                        command.Parameters.Add(new SqlParameter("@MobileNo1", SqlDbType.NVarChar));
                        command.Parameters["@MobileNo1"].Value = model.MobileNo1;

                        command.Parameters.Add(new SqlParameter("@Contact2", SqlDbType.NVarChar));
                        command.Parameters["@Contact2"].Value = model.Contact2;

                        command.Parameters.Add(new SqlParameter("@Relationship2", SqlDbType.NVarChar));
                        command.Parameters["@Relationship2"].Value = model.Relationship2;

                        command.Parameters.Add(new SqlParameter("@MobileNo2", SqlDbType.NVarChar));
                        command.Parameters["@MobileNo2"].Value = model.MobileNo2;

                        /*command.Parameters.Add(new SqlParameter("@MotherContactNo", SqlDbType.NVarChar));
                        command.Parameters["@MotherContactNo"].Value = model.MotherContactNo;

                        command.Parameters.Add(new SqlParameter("@MotherEmail", SqlDbType.NVarChar));
                        command.Parameters["@MotherEmail"].Value = model.MotherEmail;

                        command.Parameters.Add(new SqlParameter("@FatheerContactNo", SqlDbType.NVarChar));
                        command.Parameters["@FatheerContactNo"].Value = model.FatheerContactNo;

                        command.Parameters.Add(new SqlParameter("@FatherEmail", SqlDbType.NVarChar));
                        command.Parameters["@FatherEmail"].Value = model.FatherEmail;*/

                        command.Parameters.Add(new SqlParameter("@GuardianContactNo", SqlDbType.NVarChar));
                        command.Parameters["@GuardianContactNo"].Value = model.GuardianContactNo;

                        command.Parameters.Add(new SqlParameter("@GuardianEmail", SqlDbType.NVarChar));
                        command.Parameters["@GuardianEmail"].Value = model.GuardianEmail;
                        /*
                                                command.Parameters.Add(new SqlParameter("@Prefectship", SqlDbType.NVarChar));
                                                command.Parameters["@Prefectship"].Value = model.Prefectship;*/

                        command.Parameters.Add(new SqlParameter("@scholarship", SqlDbType.NVarChar));
                        command.Parameters["@scholarship"].Value = model.Scholarship;

                        command.Parameters.Add(new SqlParameter("@Discipline", SqlDbType.NVarChar));
                        command.Parameters["@Discipline"].Value = model.Discipline;

                        command.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.NVarChar));
                        command.Parameters["@DateOfBirth"].Value = model.DateOfBirth;

                        command.Parameters.Add(new SqlParameter("@nic", SqlDbType.NVarChar));
                        command.Parameters["@nic"].Value = model.nic;

                        command.Parameters.Add(new SqlParameter("@GuardianName", SqlDbType.NVarChar));
                        command.Parameters["@GuardianName"].Value = model.GuardianName;

                        command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar));
                        command.Parameters["@email"].Value = model.childemail;

                        command.Parameters.Add(new SqlParameter("@IsMotherPastStudent", SqlDbType.NVarChar));
                        command.Parameters["@IsMotherPastStudent"].Value = model.IsMotherPastStudent;

                        command.Parameters.Add(new SqlParameter("@IsFatherPastStudent", SqlDbType.NVarChar));
                        command.Parameters["@IsFatherPastStudent"].Value = model.IsFatherPastStudent;


                        command.Parameters.Add(new SqlParameter("@Other", SqlDbType.NVarChar));
                        command.Parameters["@Other"].Value = model.Other;



                        command.ExecuteNonQuery();
                    }
                    db.SaveChanges();

                }
                else
                {

                }
                return ("Updated Successfully");
            }

            catch (Exception)
            {
                throw;
            }
        }


        public async Task<object> UpdateParentInfoForAPI(Guid token, ParentsUpdateModel model)
        {
            try
            {
                var userid = db.TTokens.Where(x => x.Id.Equals(token)).Select(a => a.Referenceid);
                if (userid.Count() > 0)
                {
                    var child = await this.GetEntityIDForUpdate(model.childid);
                    var cmps = await this.db.MChildschoolmappings.Where(x => x.Childid.Equals(child.Id)).FirstOrDefaultAsync();
                    var pcm = await this.db.MParentchildmappings.Where(x => x.Childid.Equals(child.Id)).FirstOrDefaultAsync();
                    var appusr = await this.mAppuserinfoService.GetEntityIDForUpdate((int)pcm.Appuserid);
                    if (child != null && cmps != null && pcm != null && appusr != null)
                    {

                        var buid = await this.db.MStandardsectionmappings.Where(x => x.Id.Equals(model.sectionid)).Select(x => x.Businessunittypeid).FirstOrDefaultAsync();
                        if (buid == 2)
                        {
                            if (model.sectionid > 0)
                            {
                                cmps.Standardsectionmappingid = model.sectionid;
                                cmps.Modifieddate = DateTime.Now;
                            }
                            await this.mChildschoolmappingService.UpdateEntity(cmps);
                        }
                        else
                        {
                            return (new
                            {
                                Message = "Enter a valid SectionID",
                                StatusCode = HttpStatusCode.BadRequest

                            });
                        }



                        if (!string.IsNullOrEmpty(model.childfirstname))
                            child.Firstname = model.childfirstname;
                        if (!string.IsNullOrEmpty(model.childlastname))
                            child.Lastname = model.childlastname;
                        if (model.ChildGenderId.HasValue)
                            child.Genderid = model.ChildGenderId;
                        child.Modifieddate = DateTime.Now;
                        await this.UpdateEntity(child);

                        //if (model.standardsectionmappingid > 0)
                        //    cmps.Standardsectionmappingid = model.standardsectionmappingid;

                        if (!string.IsNullOrEmpty(model.PhoneNumber))
                            appusr.Phonenumber = model.PhoneNumber;
                        if (!string.IsNullOrEmpty(model.EmailAddress))
                            appusr.Emailid = model.EmailAddress;
                        if (model.Issmsuser.HasValue)
                            appusr.Issmsuser = model.Issmsuser;
                        appusr.Modifieddate = DateTime.Now;
                        await this.mAppuserinfoService.UpdateEntity(appusr);

                        return ("Updated Successfully");
                    }
                    return (new
                    {
                        Message = "Entity not found",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                return (new
                {
                    Message = "User not found",
                    StatusCode = HttpStatusCode.NotFound

                });

            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

    }
}
