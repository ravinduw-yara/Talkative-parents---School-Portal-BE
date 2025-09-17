using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Services.MStandardsectionmappingService;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using CommonUtility;
using System.Data;
using System.Net;

namespace Services
{
    public interface IMStandardsectionmappingService : ICommonService
    {
        Task<int> AddEntity(MStandardsectionmapping entity);
        Task<MStandardsectionmapping> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MStandardsectionmapping entity);
        Task<object> GetEntityByBranchID(int entityID);
        Task<object> GetEntityBySchoolID(int entityID);
        Task<List<StandardModel>> GetEntityBySchoolID2(int? entityID); 
        Task<List<StandardModel>> GetEntityByLevelSchoolID2(int? entityID, int? levelid);
        Task<IQueryable<MStandardsectionmapping>> GetAllEntitiesPvt();
        Task<List<StandardModel>> GetStandardSectionMappingByBranchIdAsync(int branchId);
        Task<object> AddSchool(SchoolRequest model);
        Task<List<StandardModel>> GetSubjectTeacherEntityByLevel(int SchoolId, int SubjectTeacherId);
        Task<string> UpdateGradeAndSectionName(UpdateGradeSectionNameModel model);
        Task<string> AddGradeAndSection(PostGradeSectionModel model);



    }


    public class MStandardsectionmappingService : IMStandardsectionmappingService
    {
        private readonly IRepository<MStandardsectionmapping> repository;
        private DbSet<MStandardsectionmapping> localDBSet;
        private readonly TpContext db = new TpContext();
        private readonly IConfiguration configuration;
        private readonly string _connectionString;

        public MStandardsectionmappingService(
          IRepository<MStandardsectionmapping> repository,
            IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;
        }


        public async Task<int> AddEntity(MStandardsectionmapping entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MStandardsectionmapping>)await this.repository.GetAll();

        private static Object Mapper(MStandardsectionmapping x) => new 
        {
            x.Id,
            x.Name,
            x.Description,
            x.Businessunittype,
            x.Parentid,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            },
            Branch = new
            {
                Branch = x.Branch != null ? x.Branch.Name : string.Empty,
                x.Branchid
            },
        };

        public async Task<IQueryable<MStandardsectionmapping>> GetAllEntitiesPvt()
        {
            try {
                await AllEntityValue();
                return this.localDBSet
                .Include(x => x.CreatedbyNavigation)
                .Include(x => x.ModifiedbyNavigation)
                .Include(x => x.Branch)
                .Include(x => x.Status);
            }
            catch(Exception Ex)
            {
                throw Ex;
            }

        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MStandardsectionmapping> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<object> GetEntityByBranchID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Branchid.Equals(entityID)).Select(x => Mapper(x));

        public async Task<object> GetEntityBySchoolID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Branch.Schoolid.Equals(entityID)).Select(x => Mapper(x));

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));
      
        public async Task<int> UpdateEntity(MStandardsectionmapping entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        //sanduni Feb 28 - to get grade and sections available for the subject teacher

        public async Task<List<StandardModel>> GetSubjectTeacherEntityByLevel(int SchoolId, int SubjectTeacherId)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand command = new SqlCommand(ApplicationConstants.GetSubjectTeacherStandardSectionMappingBySectionSubject, connection);
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SchoolId", SchoolId);
                        command.Parameters.AddWithValue("@SubjectTeacherId", SubjectTeacherId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var standardMappings = new Dictionary<int, StandardModel>();

                            while (await reader.ReadAsync())
                            {
                                var standardId = reader.GetInt32(reader.GetOrdinal("StandardId"));
                                var standardsName = reader.GetString(reader.GetOrdinal("StandardsName"));
                                var sectionId = reader.GetInt32(reader.GetOrdinal("SectionId"));
                                var sectionName = reader.GetString(reader.GetOrdinal("SectionName"));

                                if (!standardMappings.TryGetValue(standardId, out var standardModel))
                                {
                                    standardModel = new StandardModel
                                    {
                                        standardId = standardId,
                                        standardsName = standardsName
                                    };
                                    standardMappings.Add(standardId, standardModel);
                                }

                                standardModel.Sections.Add(new SectionModel
                                {
                                    SectionId = sectionId,
                                    sectionName = sectionName
                                });
                            }

                            return standardMappings.Values.ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log exception as appropriate
                throw;
            }
        }
        public async Task<List<StandardModel>> GetStandardSectionMappingByBranchIdAsync(int branchId)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand command = new SqlCommand(ApplicationConstants.GetStandardSectionMappingByBranchId, connection);
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BranchId", branchId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var standardMappings = new Dictionary<int, StandardModel>();

                            while (await reader.ReadAsync())
                            {
                                var standardId = reader.GetInt32(reader.GetOrdinal("StandardId"));
                                var standardsName = reader.GetString(reader.GetOrdinal("StandardsName"));
                                var sectionId = reader.GetInt32(reader.GetOrdinal("SectionId"));
                                var sectionName = reader.GetString(reader.GetOrdinal("SectionName"));

                                if (!standardMappings.TryGetValue(standardId, out var standardModel))
                                {
                                    standardModel = new StandardModel
                                    {
                                        standardId = standardId,
                                        standardsName = standardsName
                                    };
                                    standardMappings.Add(standardId, standardModel);
                                }

                                standardModel.Sections.Add(new SectionModel
                                {
                                    SectionId = sectionId,
                                    sectionName = sectionName
                                });
                            }

                            return standardMappings.Values.ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log exception as appropriate
                throw;
            }
        }

        //sanduni LevelGradeSectionLoad
        public async Task<List<StandardModel>> GetEntityByLevelSchoolID2(int? entityID,int? levelid)
        {
            IQueryable<MStandardsectionmapping> entities = await GetAllEntitiesPvt();

            List<MStandardsectionmapping> classList = entities.Where(a => a.Branch.Schoolid == entityID && a.LevelID == levelid).OrderBy(a => a.Id).ToList();
           

            if (classList.Count != 0)
            {

                var standardLists = classList.FindAll(a => a.Businessunittypeid == 1);
                var sectionLists = classList.FindAll(a => a.Businessunittypeid == 2);

                List<StandardModel> standards = new List<StandardModel>();

                standardLists.ForEach(a =>
                {

                    StandardModel standard = new StandardModel();
                    standard.standardId = a.Id;
                    standard.standardsName = a.Name;


                    var std_sec_mapping = sectionLists.FindAll(x => x.Parentid == a.Id);

                    std_sec_mapping.ForEach(q =>
                    {
                        SectionModel section = new SectionModel
                        {
                            SectionId = q.Id,
                            sectionName = q.Name
                        };
                        standard.Sections.Add(section);
                    });
                    standards.Add(standard);
                });
                return standards;
            }
            return null;
        }


        public async Task<List<StandardModel>> GetEntityBySchoolID2(int? entityID)
        {
            IQueryable<MStandardsectionmapping> entities = await GetAllEntitiesPvt();

            List<MStandardsectionmapping> classList = entities.Where(a => a.Branch.Schoolid == entityID).ToList();

            if (classList.Count != 0)
            {

                var standardLists = classList.FindAll(a => a.Businessunittypeid == 1);
                var sectionLists = classList.FindAll(a => a.Businessunittypeid == 2);

                List<StandardModel> standards = new List<StandardModel>();

                standardLists.ForEach(a =>
                {

                    StandardModel standard = new StandardModel();
                    standard.standardId = a.Id;
                    standard.standardsName = a.Name;


                    var std_sec_mapping = sectionLists.FindAll(x => x.Parentid == a.Id);

                    std_sec_mapping.ForEach(q =>
                    {
                        SectionModel section = new SectionModel
                        {
                            SectionId = q.Id,
                            sectionName = q.Name
                        };
                        standard.Sections.Add(section);
                    });
                    standards.Add(standard);
                });
                return standards;
            }
            return null;
        }
        public class SchoolRequest
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string WebsiteLink { get; set; }
            public string EmailId { get; set; }
            public string PrimaryPhoneNumber { get; set; }
            public string SecondaryPhoneNumber { get; set; }
            public int? StaffCount { get; set; }
            public string Logo { get; set; }
            public bool? AllowCategory { get; set; }
            public bool? IsSBSMS { get; set; }
            public int CreatedBy { get; set; }
            public int StatusId { get; set; }
            public bool? ParentPortalEnabled { get; set; }
            public int? PdfModel { get; set; }
            public int? DrcModel { get; set; }
            public string IpgUrl { get; set; }
            public int? IsMigrated { get; set; }
        }

        public async Task<object> AddSchool(SchoolRequest model)
        {

            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand command = new SqlCommand(ApplicationConstants.AddSchool, connection);
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        // Add parameters
                        command.Parameters.Add(new SqlParameter("@code", SqlDbType.NVarChar, 32) { Value = model.Code ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 128) { Value = model.Name ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 265) { Value = model.Description ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@websitelink", SqlDbType.NVarChar, 256) { Value = model.WebsiteLink ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@emailid", SqlDbType.NVarChar, 128) { Value = model.EmailId ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@primaryphonenumber", SqlDbType.NVarChar, 256) { Value = model.PrimaryPhoneNumber ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@secondaryphonenumber", SqlDbType.NVarChar, 256) { Value = model.SecondaryPhoneNumber ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@staffcount", SqlDbType.Int) { Value = model.StaffCount ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@logo", SqlDbType.NVarChar, 128) { Value = model.Logo ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@allowcategory", SqlDbType.Bit) { Value = model.AllowCategory ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@issbsms", SqlDbType.Bit) { Value = model.IsSBSMS ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@createdby", SqlDbType.Int) { Value = model.CreatedBy });
                        command.Parameters.Add(new SqlParameter("@statusid", SqlDbType.Int) { Value = model.StatusId });
                        command.Parameters.Add(new SqlParameter("@ParentPortalEnabled", SqlDbType.Bit) { Value = model.ParentPortalEnabled ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@pdfmodel", SqlDbType.Int) { Value = model.PdfModel ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@drcmodel", SqlDbType.Int) { Value = model.DrcModel ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@Ipgurl", SqlDbType.NVarChar, 256) { Value = model.IpgUrl ?? (object)DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@ismigrated", SqlDbType.Int) { Value = model.IsMigrated ?? (object)DBNull.Value });


                        await command.ExecuteNonQueryAsync();
                    }
                }

                return new
                {
                    Message = "School Added Successfully",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Error = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }


        public class UpdateGradeSectionNameModel
        {
            public int GradeAndSectionId { get; set; }
            public string GradeAndSectionName { get; set; }
        }


        public async Task<string> UpdateGradeAndSectionName(UpdateGradeSectionNameModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var cmd = new SqlCommand(ApplicationConstants.UpdateGradeAndSectionName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@GradeAndSectionId", model.GradeAndSectionId);
                cmd.Parameters.AddWithValue("@GradeAndSectionName", model.GradeAndSectionName);

                var statusParam = new SqlParameter("@Status", SqlDbType.VarChar, 400)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(statusParam);

                await cmd.ExecuteNonQueryAsync();
                return statusParam.Value?.ToString() ?? "No status returned";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return "Error occurred while updating grade and section name";
            }
        }
        public async Task<string> AddGradeAndSection(PostGradeSectionModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(ApplicationConstants.AddGradeAndSectionSp, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@schoolid", model.SchoolId);
                        command.Parameters.AddWithValue("@levelid", model.LevelId);
                        command.Parameters.AddWithValue("@standardid", model.StandardId);
                        command.Parameters.AddWithValue("@sectionname", model.SectionName);

                        var statusParam = new SqlParameter("@status", SqlDbType.VarChar, 400)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(statusParam);

                        await command.ExecuteNonQueryAsync();

                        return statusParam.Value?.ToString() ?? "No status returned";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return "Error occurred while adding grade and section";
            }
        }
        public class PostGradeSectionModel
        {
            public int SchoolId { get; set; }
            public int LevelId { get; set; }
            public int StandardId { get; set; }
            public string SectionName { get; set; }
        }

        public class StandardModel
        {
            public int standardId { get; set; }
            public string standardsName { get; set; }
            public List<SectionModel> Sections { get; set; } = new List<SectionModel>();
        }

        public class SectionModel
        {
            public int SectionId { get; set; }
            public string sectionName { get; set; }
        }
        public class StandardSectionMapping
        {
            public int StandardId { get; set; }
            public string StandardsName { get; set; }
            public int SectionId { get; set; }
            public string SectionName { get; set; }
        }

    }
}
