using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IMSchoolService : ICommonService
    {
        Task<int> AddEntity(MSchool entity);
        Task<MSchool> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MSchool entity);

        //Task<List<MSchool>> GetAllEntitiesBySp();

        Task<IQueryable<MSchool>> GetAllEntitiesBySp();

        Task<IQueryable<MSchool>> GetAllSchoolByCode(string code);

        Task<IQueryable<Count>> CountDashboardData(int code);

        Task<List<SchoolModel>> GetSchoolById(int schoolid);
        Task<List<SchoolIpgurlModel>> GetSchoolIpgurlById(int schoolid);
    }
    public class MSchoolService : IMSchoolService
    {
        private readonly IRepository<MSchool> repository;
        private DbSet<MSchool> localDBSet;
        private readonly string _connectionString;
        private readonly IConfiguration configuration;
        private static TpContext db = new TpContext();

        public MSchoolService(IRepository<MSchool> repository, IConfiguration configuration)
        {
            this.repository = repository;
            _connectionString = configuration.GetConnectionString("TpConnectionString");
            this.configuration = configuration;
        }
        public async Task<int> AddEntity(MSchool entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MSchool>)await this.repository.GetAll();

        private static Object Mapper(MSchool x) => new
        {
            x.Id,
            x.Code,
            x.Description,
            x.Name,
            x.Websitelink,
            x.Emailid,
            x.Primaryphonenumber,
            x.Secondaryphonenumber,
            x.Staffcount,
            x.Logo,
            x.Allowcategory,
            x.Issbsms,
            x.Statusid
        };

        public async Task<List<SchoolIpgurlModel>> GetSchoolIpgurlById(int schoolid)
        {
            var res = await db.MSchools.Where(x => x.Id == schoolid).FirstOrDefaultAsync();

            if (res != null)
            {
                List<SchoolIpgurlModel> scml = new List<SchoolIpgurlModel>();
                SchoolIpgurlModel sm = new SchoolIpgurlModel();
                sm.Ipgurl = res.Ipgurl;
                scml.Add(sm);
                return scml;
            }
            else
            {
                return null;
            }
        }
        private async Task<IQueryable<MSchool>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet.Include(x => x.CreatedbyNavigation).Include(x => x.ModifiedbyNavigation).Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<MSchool> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(MSchool entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<List<SchoolModel>> GetSchoolById(int schoolid)
        {
            var res = await db.MSchools.Where(x => x.Id == schoolid).FirstOrDefaultAsync();

            if (res != null)
            {
                List<SchoolModel> scml = new List<SchoolModel>();
                SchoolModel sm = new SchoolModel();
                sm.Id = res.Id;
                sm.Name = res.Name;
                sm.Description = res.Description;
                sm.Logo = res.Logo;
                sm.Websitelink = res.Websitelink;
                sm.PrimaryPhoneNumber = res.Primaryphonenumber;
                sm.SecondaryPhoneNumber = res.Secondaryphonenumber;
                sm.Email = res.Emailid;
                sm.StaffCount = res.Staffcount;
                sm.AllowPublicSoundingBoard = false;
                sm.AllowParentToParentChat = false;
                sm.AllowSectionChat = false;
                scml.Add(sm);
                return scml;
            }
            else
            {
                return null;
            }

        }

        //Testing SP implementation. NOT IMPORTANT for now
        public async Task<IQueryable<MSchool>> GetAllEntitiesBySp()
        {
            var res = new List<MSchool>();
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetAllSchool, connection);
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new MSchool()
                            {
                                //Id = (int)reader["Id"],
                                Code = reader["code"].ToString(),
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                Websitelink = reader["websitelink"].ToString(),
                                Emailid = reader["emailid"].ToString(),
                                Primaryphonenumber = reader["primaryphonenumber"].ToString(),
                                Secondaryphonenumber = reader["secondaryphonenumber"].ToString(),
                                Staffcount = (int?)reader["staffcount"],
                                Logo = reader["logo"].ToString(),
                                Allowcategory = reader["allowcategory"] != DBNull.Value ? (bool?)reader["allowcategory"] : false,
                                Issbsms = reader["issbsms"] != DBNull.Value ? (bool?)reader["issbsms"] : false,
                                Createddate = (DateTime?)reader["createddate"],
                                Modifieddate = (DateTime?)reader["modifieddate"],
                                Createdby = reader["createdby"] != DBNull.Value ? (int?)reader["createdby"] : 0,
                                Modifiedby = reader["modifiedby"] != DBNull.Value ? (int?)reader["modifiedby"] : 0,
                                Statusid = reader["statusid"] != DBNull.Value ? (int?)reader["statusid"] : 0
                            }));
                        }
                    }
                }
                return res.AsQueryable();
            }
        }

        public async Task<IQueryable<MSchool>> GetAllSchoolByCode(string code)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<MSchool>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetAllSchoolByCode, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Code", SqlDbType.NVarChar));
                command.Parameters["@Code"].Value = code;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new MSchool()
                            {
                                Code = reader["code"].ToString(),
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                Websitelink = reader["websitelink"].ToString(),
                                Emailid = reader["emailid"].ToString(),
                                Primaryphonenumber = reader["primaryphonenumber"].ToString(),
                                Secondaryphonenumber = reader["secondaryphonenumber"].ToString(),
                                Staffcount = (int?)reader["staffcount"],
                                Logo = reader["logo"].ToString(),
                                Allowcategory = reader["allowcategory"] != DBNull.Value ? (bool?)reader["allowcategory"] : false,
                                Issbsms = reader["issbsms"] != DBNull.Value ? (bool?)reader["issbsms"] : false,
                                Createddate = (DateTime?)reader["createddate"],
                                Modifieddate = (DateTime?)reader["modifieddate"],
                                Createdby = reader["createdby"] != DBNull.Value ? (int?)reader["createdby"] : 0,
                                Modifiedby = reader["modifiedby"] != DBNull.Value ? (int?)reader["modifiedby"] : 0,
                                Statusid = reader["statusid"] != DBNull.Value ? (int?)reader["statusid"] : 0
                            }));

                        }
                    }
                }
                return res.AsQueryable();
            }
        }


        //Added by Priyanka - for Dashbord Count 
        public async Task<IQueryable<Count>> CountDashboardData(int SchoolId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<Count>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.CountDashboardData, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.NVarChar));
                command.Parameters["@schoolid"].Value = SchoolId;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new Count()
                            {
                                Studentcount = reader["Studentcount"].ToString(),
                                Parentcount = reader["Parentcount"].ToString()
                            }));

                        }
                    }
                }
                return res.AsQueryable();
            }

        }
    }
        public class SchoolIpgurlModel
        {
            public string Ipgurl { get; set; }
        }
        public class SchoolModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Logo { get; set; }
            public string Websitelink { get; set; }
            public string Email { get; set; }
            public string PrimaryPhoneNumber { get; set; }
            public string SecondaryPhoneNumber { get; set; }
            public int? StaffCount { get; set; }
            public bool AllowPublicSoundingBoard { get; set; }
            public bool AllowParentToParentChat { get; set; }
            public bool AllowSectionChat { get; set; }

        }

        public class Count
        {
            public string Studentcount { get; set; }
            public string Parentcount { get; set; }
        }
    }

