
using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nancy.Extensions;
using net.openstack.Core.Domain;
//using Newtonsoft.Json.Linq;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Services.MChildinfoService;

namespace Services
{
    public interface IMChildinfoService : ICommonService
    {
        Task<int> AddEntity(MChildinfo entity);
        Task<MChildinfo> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MChildinfo entity);
        Task<object> GetParentsListForAPI(Guid token,int nuofRows, int pageNumber, string standard = "", string section = "", string childlastname = "", string searchstring = ""); 

        Task<object> GetParentsListStudentVIewSP(Guid token, int academicyearid, int activestatus, int nuofRows, int pageNumber, string standard = "", string section = "", string childlastname = "", string searchstring = "");
        Task<object> UpdateParentInfoForAPI(Guid token, ParentsUpdateModel model);
        Task<object> UpdateStudentInfoAPI(Guid token, GetParentsListModel model);
        Task<object> GetParentsListNewForAPI(Guid token, int nuofRows, int pageNumber, string standard = "", string section = "", string childlastname = "", string searchstring = "");
        Task<ApiResponse> GetStudentDetailsByChildIdForAPI(Guid token, int schoolid, int childid, int academicYearId, int nuofRows = 10, int pageNumber = 1);
        Task<object> GetStudentDetailsByParentIdAPI(int parentid, int nuofRows, int pageNumber);
        Task<object> GetsblingsListAPI(int parentId, int childId, int Schoolid); 
        Task<object> GetStudentDetailsParentByChildIdAPI(int parentid, int childid, int nuofRows, int pageNumber);
        Task<object> HiEduAddStudentDetails(HiEduStudentAddModel model);
        Task<List<StudentDetailsModel>> GetChildInfoBySchoolCourseBatchSP(int schoolId, int? courseId = null, int? batchId = null);
        Task<object> SchoolAddStudentDetails(SchoolStudentAddModel model);
        Task<object> SchoolAddTeacherDetails(TeacherAddModel model); 
        Task<List<TeacherDetailsViewModel>> GetTeacherDetailsListAsync(int academicYearId, int sectionId);
        Task<List<TeacherDropdownDetailsViewModel>> GetDropdownTeacherDetails(int branchId);
        Task<SingleTeacherDetailsViewModel> GetSingleTeacherDetails(int teacherId);
        Task<object> UpdateDrcEnableForChildren(ChildUpdateModel model);
        Task<object> DeleteStudentDetails(int childId);

        Task<object> PromoteStudentGradeSectionAPI(Guid Token, PromoteStudentModel model);
        Task<object> UpdateBulkStudentInactiveStatus(ChildUpdateInactiveModel model);
        Task<object> BulkDeleteStudentDetails(ChildBulkDeleteModel model);
        Task<object> AddParentForExistingChild(ParentAddModel model);
        Task<object> DeleteParentDetails(int schoolId, int childId, int parentId);
        Task<object> GetChildDRCByChildIdYear(int? childid, int? academicyear, int nuofRows, int pageNumber);
        Task<object> GetTeacherDetailsSubjects(int academicYearId, int schoolUserId);
    }
    public class ApiResponse
    {
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public int? TotalPages { get; set; }
        public List<GetParentsListModel> Items { get; set; }
        public string Data { get; set; }
    }
    public class MChildinfoService : IMChildinfoService
    {
        private readonly IRepository<MChildinfo> repository;
        private DbSet<MChildinfo> localDBSet;
        private readonly IConfiguration configuration;
        readonly TpContext db = new TpContext();
        private readonly IMChildschoolmappingService mChildschoolmappingService;
        private readonly IMParentchildmappingService mParentchildmappingService;
        private readonly IMAppuserinfoService mAppuserinfoService;

        public MChildinfoService(
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
            Gender=x.Genderid,                     
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
        public async Task<object> GetTeacherDetailsSubjects(int academicYearId, int schoolUserId)
        {
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

                var res = new List<TeacherDetailsViewModel>();

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        SqlCommand command = new SqlCommand(ApplicationConstants.GetTeacherDetailsSubjects, connection);

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
                        command.Parameters.AddWithValue("@schoolUserId", schoolUserId);


                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            var teacherDetails = new TeacherDetailsViewModel
                            {
                                TeacherName = reader["TeacherName"].ToString(),
                                SubjectId = (int)reader["SubjectId"],
                                SubjectName = reader["SubjectName"].ToString(),
                                GradeId = (int)reader["GradeId"],
                                SectionId = (int)reader["SectionId"]
                            };

                            res.Add(teacherDetails);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }

                return res;
            }
            }
        public async Task<object> DeleteParentDetails(int schoolId, int childId, int parentId)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.Parentdelete, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                    command.Parameters["@schoolid"].Value = schoolId;

                    command.Parameters.Add(new SqlParameter("@childid", SqlDbType.Int));
                    command.Parameters["@childid"].Value = childId;

                    command.Parameters.Add(new SqlParameter("@parentId", SqlDbType.Int));
                    command.Parameters["@parentId"].Value = parentId;

                    await command.ExecuteNonQueryAsync();
                }

                return "Parent details deleted successfully";
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
        public async Task<object> BulkDeleteStudentDetails(ChildBulkDeleteModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (int childId in model.ChildIds)
                    {
                        SqlCommand command = new SqlCommand(ApplicationConstants.DeleteSchoolStudentDetails, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@ChildId", SqlDbType.Int));
                        command.Parameters["@ChildId"].Value = childId;

                        await command.ExecuteNonQueryAsync();

                    }
                }

                return "Students Details Deleted Successfully";
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
        public async Task<object> UpdateBulkStudentInactiveStatus(ChildUpdateInactiveModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (int childId in model.ChildIds)
                    {
                        SqlCommand command = new SqlCommand(ApplicationConstants.UpdateStudentInactiveStatusForChildren, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@AcademicYearId", SqlDbType.Int));
                        command.Parameters["@AcademicYearId"].Value = model.AcademicYearId;

                        command.Parameters.Add(new SqlParameter("@StandardId", SqlDbType.Int));
                        command.Parameters["@StandardId"].Value = model.StandardId;

                        command.Parameters.Add(new SqlParameter("@SectionId", SqlDbType.Int));
                        command.Parameters["@SectionId"].Value = model.SectionId;

                        command.Parameters.Add(new SqlParameter("@ChildId", SqlDbType.Int));
                        command.Parameters["@ChildId"].Value = childId;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return "Students status updated successfully";
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
        //Jaliya  Student Add to batch
        public async Task<object> HiEduAddStudentDetails(HiEduStudentAddModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.AddHiEduStudentDetails, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@code", SqlDbType.NVarChar));
                    command.Parameters["@code"].Value = model.Code;

                    command.Parameters.Add(new SqlParameter("@firstname", SqlDbType.NVarChar));
                    command.Parameters["@firstname"].Value = model.Firstname;

                    command.Parameters.Add(new SqlParameter("@lastname", SqlDbType.NVarChar));
                    command.Parameters["@lastname"].Value = model.Lastname;

                    command.Parameters.Add(new SqlParameter("@middlename", SqlDbType.NVarChar));
                    command.Parameters["@middlename"].Value = model.Middlename;

                    command.Parameters.Add(new SqlParameter("@dob", SqlDbType.Date));
                    command.Parameters["@dob"].Value = model.Dob;

                    command.Parameters.Add(new SqlParameter("@phonenumber", SqlDbType.NVarChar));
                    command.Parameters["@phonenumber"].Value = model.Phonenumber;

                    command.Parameters.Add(new SqlParameter("@genderid", SqlDbType.Int));
                    command.Parameters["@genderid"].Value = model.Genderid;

                    command.Parameters.Add(new SqlParameter("@createddate", SqlDbType.Date));
                    command.Parameters["@createddate"].Value = model.Createddate;

                    command.Parameters.Add(new SqlParameter("@modifieddate", SqlDbType.Date));
                    command.Parameters["@modifieddate"].Value = model.Modifieddate;

                    command.Parameters.Add(new SqlParameter("@createdby", SqlDbType.Int));
                    command.Parameters["@createdby"].Value = model.Createdby;

                    command.Parameters.Add(new SqlParameter("@modifiedby", SqlDbType.Int));
                    command.Parameters["@modifiedby"].Value = model.Modifiedby;

                    command.Parameters.Add(new SqlParameter("@statusid", SqlDbType.Int));
                    command.Parameters["@statusid"].Value = model.Statusid;

                    command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar));
                    command.Parameters["@email"].Value = model.Email;

                    command.Parameters.Add(new SqlParameter("@nic", SqlDbType.NVarChar));
                    command.Parameters["@nic"].Value = model.Nic;

                    command.Parameters.Add(new SqlParameter("@BatchCourseMappingId", SqlDbType.Int));
                    command.Parameters["@BatchCourseMappingId"].Value = model.BatchCourseMappingId;

                    await command.ExecuteNonQueryAsync();
                }

                return ("Student Details Added Successfully");
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
        //Jaliya  Student Get
        public async Task<List<StudentDetailsModel>> GetChildInfoBySchoolCourseBatchSP(int schoolId, int? courseId = null, int? batchId = null)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<StudentDetailsModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetChildInfoBySchoolCourseBatch, connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@SchoolId", schoolId);
                if (courseId.HasValue)
                    command.Parameters.AddWithValue("@CourseId", courseId.Value);
                else
                    command.Parameters.AddWithValue("@CourseId", DBNull.Value);

                if (batchId.HasValue)
                    command.Parameters.AddWithValue("@BatchId", batchId.Value);
                else
                    command.Parameters.AddWithValue("@BatchId", DBNull.Value);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new StudentDetailsModel
                            {
                                ChildId = (int)reader["ChildId"],
                                ChildFirstName = reader["ChildFirstName"].ToString(),
                                ChildLastName = reader["ChildLastName"].ToString(),
                                CourseName = reader["CourseName"].ToString(),
                                BatchName = reader["BatchName"].ToString()
                            }));
                        }
                    }
                }
                return res;
            }
        }

        public async Task<object> PromoteStudentGradeSectionAPI(Guid Token, PromoteStudentModel model)
        {
            try
            {
                var res = new List<PromoteStudentsList>();
                var userid = db.TTokens.Where(x => x.Id.Equals(Token)).Select(a => a.Referenceid).FirstOrDefault();
                var buid = await this.db.MStandardsectionmappings.Where(x => x.Id.Equals(model.SectionId)).Select(x => x.Businessunittypeid).FirstOrDefaultAsync();
                var Standardsectionmappingid = 0;
                if (buid == 2)
                {
                    if (model.SectionId > 0)
                    {
                        Standardsectionmappingid = model.SectionId;
                        //var Modifieddate = DateTime.Now;
                    }
                }
                else
                {
                    return (new
                    {
                        Message = "Enter a valid SectionID",
                        StatusCode = HttpStatusCode.BadRequest

                    });
                }
                if (userid != 0 && userid != null)
                {
                    var Schoolid = db.MBranches.Where(x => x.Id.Equals(model.BranchId)).FirstOrDefault().Schoolid;
                    //var Standardsectionmappingid = model.Standardsectionmappingid;
                    var SectionId = model.SectionId;
                    var StandardId = model.StandardId;
                    var AccedemicYearID = model.AccedemicYearID;
                    // var temp = await mSchoolService.GetEntityByID((int)Schoolid);

                    if (model == null)
                    {
                        return 0;
                    }
                    else
                    {
                        foreach (var childitem in model.students)
                        {
                            PromoteStudentsList childrenlist = new PromoteStudentsList();
                            childrenlist.ChildId = childitem.ChildId;
                            childrenlist.AdmissionId = childitem.AdmissionId;
                            var admissionid = childitem.AdmissionId;

                            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                SqlCommand command = new SqlCommand(ApplicationConstants.PromoteStudentSp, connection);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                                command.Parameters["@schoolid"].Value = model.SchoolId;

                                command.Parameters.Add(new SqlParameter("@standardId", SqlDbType.Int));
                                command.Parameters["@standardId"].Value = model.StandardId;

                                command.Parameters.Add(new SqlParameter("@standardsectionmappingid", SqlDbType.Int));
                                command.Parameters["@standardsectionmappingid"].Value = Standardsectionmappingid;

                                command.Parameters.Add(new SqlParameter("@sectionid", SqlDbType.Int));
                                command.Parameters["@sectionid"].Value = model.SectionId;

                                command.Parameters.Add(new SqlParameter("@childid", SqlDbType.Int));
                                command.Parameters["@childid"].Value = childitem.ChildId;

                                command.Parameters.Add(new SqlParameter("@registerationnumber", SqlDbType.NVarChar));
                                command.Parameters["@registerationnumber"].Value = admissionid;

                                command.Parameters.Add(new SqlParameter("@AcademicYearId", SqlDbType.Int));
                                command.Parameters["@AcademicYearId"].Value = model.AccedemicYearID;

                                command.Parameters.Add(new SqlParameter("@modifiedby", SqlDbType.Int));
                                command.Parameters["@modifiedby"].Value = userid;

                                command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int));
                                command.Parameters["@CreatedBy"].Value = userid;

                                //command.Parameters.Add(new SqlParameter("@error", SqlDbType.NVarChar));
                                //command.Parameters["@error"].Value = null;

                                //command.Parameters.Add(new SqlParameter("@message", SqlDbType.NVarChar));
                                //command.Parameters["@message"].Value = null;

                                command.ExecuteNonQuery();
                            }
                            db.SaveChanges();

                        }

                        return ("Promoted Successfully");
                    }

                }
                return ("Error in Promoting"); ;
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

        private async Task<IQueryable<GetParentsSP>> GetParentsListSP(int? schoolid, string standard, string section, string childlastname, string searchstring, int? standardid, int? sectionid)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetParentsSP>();
                connection.Open(); 
                SqlCommand command = new SqlCommand(ApplicationConstants.GetParentSP, connection);
                //SqlCommand command = new SqlCommand(ApplicationConstants.GetParentsListSP, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                command.Parameters["@schoolid"].Value = schoolid;
                command.Parameters.Add(new SqlParameter("@standard", SqlDbType.NVarChar));
                command.Parameters["@standard"].Value = standard;
                command.Parameters.Add(new SqlParameter("@section", SqlDbType.NVarChar));
                command.Parameters["@section"].Value = section;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new GetParentsSP()
                            {
                                Parentid = reader["id"] != DBNull.Value ? (int?)reader["id"] : null,
                                ParentName = reader["ParentName"].ToString(),
                                RelationId = reader["RelationId"] != DBNull.Value ? (int?)reader["RelationId"] : null,
                                ChildId = reader["ChildId"] != DBNull.Value ? (int?)reader["ChildId"] : null,
                                ChildSchoolMappingId = reader["ChildSchoolMappingId"] != DBNull.Value ? (int?)reader["ChildSchoolMappingId"] : null,
                                ChildName = reader["childname"].ToString(),
                                ChildEmail = reader["ChildEmail"].ToString(),
                                SectionId = reader["SectionId"] != DBNull.Value ? (int?)reader["SectionId"] : null,
                                SectionName = reader["SectionName"].ToString(),
                                StandardId = reader["StandardId"] != DBNull.Value ? (int?)reader["StandardId"] : null,
                                StandardName = reader["StandardName"].ToString(),
                                RegistrationNumber = reader["RegistrationNumber"].ToString(),
                                //id = (int)reader["id"],
                                //ChildId = reader["childid"] != DBNull.Value ? (int?)reader["childid"] : null,
                                //ChildSectionId = reader["childsectionid"] != DBNull.Value ? (int?)reader["childsectionid"] : null,
                                //PhoneNumber = reader["phonenumber"].ToString(),
                                //ParentsName = reader["parentsname"].ToString(),
                                //UserName = reader["phonenumber"].ToString(),
                                //Issmsuser = reader["Issmsuser"] != DBNull.Value ? (bool?)reader["Issmsuser"] : false,
                                //EmailAddress = reader["emailid"].ToString(),
                                //ParentsGender = reader["parentgender"].ToString(),
                                //ChildFirstName = reader["firstname"].ToString(),
                                //ChildLastName = reader["lastname"].ToString(),
                                //ChildGender = reader["childgender"].ToString(),
                                //ChildGenderId = reader["childgenderid"] != DBNull.Value ? (int?)reader["childgenderid"] : null,
                                //ParentsRelation = reader["relationtype"].ToString(),
                                //Schoolid = reader["schoolid"] != DBNull.Value ? (int?)reader["schoolid"] : null,
                                //School = reader["schoolname"].ToString(),
                                //StandardId = reader["standardid"] != DBNull.Value ? (int?)reader["standardid"] : null,
                                //Standard = reader["standardname"].ToString(),
                                //SectionId = reader["sectionid"] != DBNull.Value ? (int?)reader["sectionid"] : null,
                                //Section = reader["sectionname"].ToString(),
                                //Parentcreateddate = reader["parentcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                                //Childcreateddate = reader["childcreateddate"] != DBNull.Value ? (DateTime?)reader["childcreateddate"] : null,
                            }));

                        }
                    }
                }
                //return (IQueryable<GetParentsSP>)res.AsQueryable().OrderByDescending(x => x.ChildId).DistinctBy(x => x.ChildId);
                //return res.AsQueryable().OrderByDescending(x => x.ChildId);
                var uniqueRecords = res.DistinctBy(x => x.ChildId)
                   .ToList();
                return uniqueRecords.AsQueryable();
                // return (IQueryable<GetParentsSP>)res.Select(x => x.ChildId).Distinct();
            }
        }
        private async Task<IQueryable<GetParentsSP>> GetParentsListStudentVIewSP(int? schoolid, int academicyearid, int activestatus, string standard, string section, string childlastname, string searchstring, int? standardid, int? sectionid)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetParentsSP>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetParentsStudentView, connection);
                //SqlCommand command = new SqlCommand(ApplicationConstants.GetParentsListSP, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                command.Parameters["@schoolid"].Value = schoolid;
                command.Parameters.Add(new SqlParameter("@academicyearid", SqlDbType.Int));
                command.Parameters["@academicyearid"].Value = academicyearid;
                command.Parameters.Add(new SqlParameter("@activestatus", SqlDbType.Int));
                command.Parameters["@activestatus"].Value = activestatus;
                command.Parameters.Add(new SqlParameter("@standard", SqlDbType.NVarChar));
                command.Parameters["@standard"].Value = standard;
                command.Parameters.Add(new SqlParameter("@section", SqlDbType.NVarChar));
                command.Parameters["@section"].Value = section;
                command.Parameters.Add(new SqlParameter("@searchstring", SqlDbType.NVarChar));
                command.Parameters["@searchstring"].Value = searchstring;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new GetParentsSP()
                            {
                                Parentid = reader["id"] != DBNull.Value ? (int?)reader["id"] : null,
                                ParentName = reader["ParentName"].ToString(),
                                RelationId = reader["RelationId"] != DBNull.Value ? (int?)reader["RelationId"] : null,
                                ChildId = reader["ChildId"] != DBNull.Value ? (int?)reader["ChildId"] : null,
                                ChildSchoolMappingId = reader["ChildSchoolMappingId"] != DBNull.Value ? (int?)reader["ChildSchoolMappingId"] : null,
                                //ChildName = reader["childname"].ToString(),
                                ChildFirstName = reader["firstname"].ToString(),
                                ChildLastName = reader["lastname"].ToString(),
                                ChildEmail = reader["ChildEmail"].ToString(),
                                SectionId = reader["SectionId"] != DBNull.Value ? (int?)reader["SectionId"] : null,
                                SectionName = reader["SectionName"].ToString(),
                                StandardId = reader["StandardId"] != DBNull.Value ? (int?)reader["StandardId"] : null,
                                StandardName = reader["StandardName"].ToString(),
                                RegistrationNumber = reader["RegistrationNumber"].ToString(),
                                DRCEnable1 = reader["DRCEnable1"] != DBNull.Value ? (int)reader["DRCEnable1"] : null,
                                DRCEnable2 = reader["DRCEnable2"] != DBNull.Value ? (int)reader["DRCEnable2"] : null,
                                DRCEnable3 = reader["DRCEnable3"] != DBNull.Value ? (int)reader["DRCEnable3"] : null,
                                //id = (int)reader["id"],
                                //ChildId = reader["childid"] != DBNull.Value ? (int?)reader["childid"] : null,
                                //ChildSectionId = reader["childsectionid"] != DBNull.Value ? (int?)reader["childsectionid"] : null,
                                //PhoneNumber = reader["phonenumber"].ToString(),
                                //ParentsName = reader["parentsname"].ToString(),
                                //UserName = reader["phonenumber"].ToString(),
                                //Issmsuser = reader["Issmsuser"] != DBNull.Value ? (bool?)reader["Issmsuser"] : false,
                                //EmailAddress = reader["emailid"].ToString(),
                                //ParentsGender = reader["parentgender"].ToString(),
                                //ChildFirstName = reader["firstname"].ToString(),
                                //ChildLastName = reader["lastname"].ToString(),
                                //ChildGender = reader["childgender"].ToString(),
                                //ChildGenderId = reader["childgenderid"] != DBNull.Value ? (int?)reader["childgenderid"] : null,
                                //ParentsRelation = reader["relationtype"].ToString(),
                                //Schoolid = reader["schoolid"] != DBNull.Value ? (int?)reader["schoolid"] : null,
                                //School = reader["schoolname"].ToString(),
                                //StandardId = reader["standardid"] != DBNull.Value ? (int?)reader["standardid"] : null,
                                //Standard = reader["standardname"].ToString(),
                                //SectionId = reader["sectionid"] != DBNull.Value ? (int?)reader["sectionid"] : null,
                                //Section = reader["sectionname"].ToString(),
                                //Parentcreateddate = reader["parentcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                                //Childcreateddate = reader["childcreateddate"] != DBNull.Value ? (DateTime?)reader["childcreateddate"] : null,
                            }));

                        }
                    }
                }
                //return (IQueryable<GetParentsSP>)res.AsQueryable().OrderByDescending(x => x.ChildId).DistinctBy(x => x.ChildId);
                // return res.AsQueryable().OrderByDescending(x => x.ChildId
                var uniqueSortedRecords = res
                  .GroupBy(x => x.ChildId)
                  .Select(g => g.First())
                  .OrderBy(x => x.ChildLastName)
                  .ToList();
                return uniqueSortedRecords.AsQueryable();
                // var uniqueRecords = res.DistinctBy(x => x.ChildId)
                //    .ToList();
                //return uniqueRecords.AsQueryable();
                // return (IQueryable<GetParentsSP>)res.AsQueryable().OrderByDescending(x => x.ChildId).DistinctBy(x => x.ChildId);
                //return (IQueryable<GetParentsSP>)res.AsQueryable().OrderByDescending(x => x.ChildId).GroupBy(y => y.ChildId);
                // return (IQueryable<GetParentsSP>)res.Select(x => x.ChildId).Distinct();
            }
        }
        public async Task<object> UpdateDrcEnableForChildren(ChildUpdateModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var item in model.parentdrcenable)
                    {

                        SqlCommand command = new SqlCommand(ApplicationConstants.UpdateDrcEnableForChildren, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@AcademicYearId", SqlDbType.Int));
                        command.Parameters["@AcademicYearId"].Value = model.AcademicYearId;

                        command.Parameters.Add(new SqlParameter("@childId", SqlDbType.Int));
                        command.Parameters["@childId"].Value = item.ChildId;

                        command.Parameters.Add(new SqlParameter("@drcenable1", SqlDbType.Int));
                        command.Parameters["@drcenable1"].Value = item.drcenable1;

                        command.Parameters.Add(new SqlParameter("@drcenable2", SqlDbType.Int));
                        command.Parameters["@drcenable2"].Value = item.drcenable2;

                        command.Parameters.Add(new SqlParameter("@drcenable3", SqlDbType.Int));
                        command.Parameters["@drcenable3"].Value = item.drcenable3;


                        await command.ExecuteNonQueryAsync();
                    }
                }

                return "Children DrcEnable updated successfully";
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
        private async Task<IQueryable<GetParentsListModel>> GetStudentDetailsByChildIdListSP(int? schoolid, int? childid,int? AcademicYearId, int nuofRows = 10, int pageNumber = 1)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetParentsListModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetStudentDetailsByChildIdListSP, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                command.Parameters["@schoolid"].Value = schoolid;
                command.Parameters.Add(new SqlParameter("@childid", SqlDbType.Int));
                command.Parameters["@childid"].Value = childid;
                command.Parameters.Add(new SqlParameter("@AcademicYearId", SqlDbType.Int));
                command.Parameters["@AcademicYearId"].Value = AcademicYearId;
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
                                //Parentcreateddate = reader["parentcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                                //Childcreateddate = reader["childcreateddate"] != DBNull.Value ? (DateTime?)reader["childcreateddate"] : null,
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
                                //MotherName = reader["MotherName"].ToString(),
                                MotherFirstName = reader["MotherFirstName"].ToString(),
                                MotherLastName = reader["MotherLastName"].ToString(),
                                MotherContactNo = reader["MotherContactNo"].ToString(),
                                FatheerContactNo = reader["FatheerContactNo"].ToString(),
                                //FatherName = reader["FatherName"].ToString(),
                                FatherFirstName = reader["FatherFirstName"].ToString(),
                                FatherLastName = reader["FatherLastName"].ToString(),
                                FatherEmail = reader["FatherEmail"].ToString(),
                                GuardianName = reader["GuardianName"].ToString(),
                                LeavingYear = reader["LeavingYear"].ToString(),
                                GuardianContactNo = reader["GuardianContactNo"].ToString(),
                                Other = reader["Other"].ToString(),
                                IsMotherPastStudent = reader["IsMotherPastStudent"].ToString(),
                                IsFatherPastStudent = reader["IsFatherPastStudent"].ToString(),
                                // Picture = reader["Picture"].ToString(),
                                OtherparentId = reader["OtherparentId"].ToString(),
                                IsOtherParentSMSUser = reader["IsOtherParentSMSUser"] != DBNull.Value ? (bool?)reader["Issmsuser"] : false,
                                OtherParentFirstName = reader["OtherParentFirstName"].ToString(),
                                OtherParentLastName = reader["OtherParentLastName"].ToString(),
                                OtherParentEmailAddress = reader["OtherParentEmailAddress"].ToString(),
                                OtherParentPhoneNumber = reader["OtherParentPhoneNumber"].ToString(),
                                DateOfBirth = reader["dob"] != DBNull.Value ? (DateTime?)reader["dob"] : null,
                                schoollogo = reader["schoollogo"].ToString(),
                                ParentPortalEnabled = reader["ParentPortalEnabled"] != DBNull.Value ? (bool?)reader["ParentPortalEnabled"] : false,
                               // AcademicYearName = academiyearname,
                                AcademicYearId = AcademicYearId,
                                Inactivestudent = reader["statusid"] != DBNull.Value ? (int?)reader["statusid"] : null,
                                StudentImageLink = reader["StudentImageLink"].ToString(),
                                AcademicYearName = reader["AcademicYearName"].ToString()

                            }));

                        }
                    }
                }
                return res.AsQueryable().OrderByDescending(x => x.ChildFirstName);
            }
        }
        public async Task<object> GetChildDRCByChildIdYear(int? childid, int? academicyear, int nuofRows, int pageNumber)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetParentsListModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetChildDRCByChildIdYearAPIListSP, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@childid", SqlDbType.Int));
                command.Parameters["@childid"].Value = childid;
                command.Parameters.Add(new SqlParameter("@academicyear", SqlDbType.Int));
                command.Parameters["@academicyear"].Value = academicyear;
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
                                //Parentcreateddate = reader["parentcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                                //Childcreateddate = reader["childcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
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
                               // Childcreateddate = reader["childcreateddate"] != DBNull.Value ? (DateTime?)reader["childcreateddate"] : null,
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
                                DRCEnable1 = reader["DRCEnable1"] != DBNull.Value ? (int)reader["DRCEnable1"] : null,
                                DRCEnable2 = reader["DRCEnable2"] != DBNull.Value ? (int)reader["DRCEnable2"] : null,
                                DRCEnable3 = reader["DRCEnable3"] != DBNull.Value ? (int)reader["DRCEnable3"] : null,
                                AcademicYearId = reader["Academicyearid"] != DBNull.Value ? (int)reader["Academicyearid"] : null
                            }));

                        }
                    }
                }
                //var distinctChildren = res.GroupBy(x => x.ChildId).Select(g => g.FirstOrDefault());// Select the first record from each group (grouped by ChildId)
                                                                                                   // .OrderByDescending(x => x.ChildFirstName); // Order by ChildFirstName
               // var distinctChildren = res.GroupBy(x => x.ChildId).Select(g => g.ToList()); 
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

        // School Student Register
        public async Task<object> SchoolAddStudentDetails(SchoolStudentAddModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.AddSchoolAddStudentDetails, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@code", SqlDbType.NVarChar));
                    command.Parameters["@code"].Value = string.IsNullOrEmpty(model.Code) ? (object)DBNull.Value : model.Code;


                    command.Parameters.Add(new SqlParameter("@firstname", SqlDbType.NVarChar));
                    command.Parameters["@firstname"].Value = model.Firstname;

                    command.Parameters.Add(new SqlParameter("@lastname", SqlDbType.NVarChar));
                    command.Parameters["@lastname"].Value = model.Lastname;

                    command.Parameters.Add(new SqlParameter("@middlename", SqlDbType.NVarChar));
                    command.Parameters["@middlename"].Value = string.IsNullOrEmpty(model.Middlename) ? (object)DBNull.Value : model.Middlename;

                    command.Parameters.Add(new SqlParameter("@dob", SqlDbType.Date));
                    command.Parameters["@dob"].Value = model.Dob ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@phonenumber", SqlDbType.NVarChar));
                    command.Parameters["@phonenumber"].Value = string.IsNullOrEmpty(model.Phonenumber) ? (object)DBNull.Value : model.Phonenumber;

                    command.Parameters.Add(new SqlParameter("@genderid", SqlDbType.Int));
                    command.Parameters["@genderid"].Value = model.Genderid;

                    command.Parameters.Add(new SqlParameter("@createddate", SqlDbType.Date));
                    command.Parameters["@createddate"].Value = model.Createddate;

                    command.Parameters.Add(new SqlParameter("@modifieddate", SqlDbType.Date));
                    command.Parameters["@modifieddate"].Value = model.Modifieddate;

                    command.Parameters.Add(new SqlParameter("@createdby", SqlDbType.Int));
                    command.Parameters["@createdby"].Value = model.Createdby;

                    command.Parameters.Add(new SqlParameter("@modifiedby", SqlDbType.Int));
                    command.Parameters["@modifiedby"].Value = model.Modifiedby;

                    command.Parameters.Add(new SqlParameter("@statusid", SqlDbType.Int));
                    command.Parameters["@statusid"].Value = model.Statusid;

                    command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar));
                    command.Parameters["@email"].Value = string.IsNullOrEmpty(model.Email) ? (object)DBNull.Value : model.Email;

                    command.Parameters.Add(new SqlParameter("@nic", SqlDbType.NVarChar));
                    command.Parameters["@nic"].Value = string.IsNullOrEmpty(model.Nic) ? (object)DBNull.Value : model.Nic;

                    command.Parameters.Add(new SqlParameter("@standardsectionmappingId", SqlDbType.Int));
                    command.Parameters["@standardsectionmappingId"].Value = model.StandardsectionmappingId;

                    command.Parameters.Add(new SqlParameter("@AcademicYearId", SqlDbType.Int));
                    command.Parameters["@AcademicYearId"].Value = model.AcademicYearId;

                    command.Parameters.Add(new SqlParameter("@registerationnumber", SqlDbType.NVarChar));
                    command.Parameters["@registerationnumber"].Value = string.IsNullOrEmpty(model.registerationnumber) ? (object)DBNull.Value : model.registerationnumber;

                    // Add the parent details

                    command.Parameters.Add(new SqlParameter("@ParentFirstname", SqlDbType.NVarChar));
                    command.Parameters["@ParentFirstname"].Value = string.IsNullOrEmpty(model.ParentFirstname) ? (object)DBNull.Value : model.ParentFirstname;

                    command.Parameters.Add(new SqlParameter("@ParentLastname", SqlDbType.NVarChar));
                    command.Parameters["@ParentLastname"].Value = string.IsNullOrEmpty(model.ParentLastname) ? (object)DBNull.Value : model.ParentLastname;

                    command.Parameters.Add(new SqlParameter("@ParentMiddlename", SqlDbType.NVarChar));
                    command.Parameters["@ParentMiddlename"].Value = string.IsNullOrEmpty(model.ParentMiddlename) ? (object)DBNull.Value : model.ParentMiddlename;

                    command.Parameters.Add(new SqlParameter("@ParentDob", SqlDbType.Date));
                    command.Parameters["@ParentDob"].Value = model.ParentDob ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@ParentPhonenumber", SqlDbType.NVarChar));
                    command.Parameters["@ParentPhonenumber"].Value = string.IsNullOrEmpty(model.ParentPhonenumber) ? (object)DBNull.Value : model.ParentPhonenumber;

                    command.Parameters.Add(new SqlParameter("@ParentGenderid", SqlDbType.Int));
                    command.Parameters["@ParentGenderid"].Value = model.ParentGenderid.HasValue ? (object)model.ParentGenderid : DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@ParentEmailid", SqlDbType.NVarChar));
                    command.Parameters["@ParentEmailid"].Value = string.IsNullOrEmpty(model.ParentEmailid) ? (object)DBNull.Value : model.ParentEmailid;

                    command.Parameters.Add(new SqlParameter("@ParentPassword", SqlDbType.NVarChar));
                    command.Parameters["@ParentPassword"].Value = string.IsNullOrEmpty(model.ParentPassword) ? (object)DBNull.Value : model.ParentPassword;

                    command.Parameters.Add(new SqlParameter("@ParentIssmsuser", SqlDbType.Bit));
                    command.Parameters["@ParentIssmsuser"].Value = model.ParentIssmsuser;

                    command.Parameters.Add(new SqlParameter("@ParentIshigheduser", SqlDbType.Bit));
                    command.Parameters["@ParentIshigheduser"].Value = model.ParentIshigheduser;

                    // Second Parent Details
                    command.Parameters.Add(new SqlParameter("@Parent2Firstname", SqlDbType.NVarChar));
                    command.Parameters["@Parent2Firstname"].Value = string.IsNullOrEmpty(model.Parent2Firstname) ? (object)DBNull.Value : model.Parent2Firstname;

                    command.Parameters.Add(new SqlParameter("@Parent2Lastname", SqlDbType.NVarChar));
                    command.Parameters["@Parent2Lastname"].Value = string.IsNullOrEmpty(model.Parent2Lastname) ? (object)DBNull.Value : model.Parent2Lastname;

                    command.Parameters.Add(new SqlParameter("@Parent2Middlename", SqlDbType.NVarChar));
                    command.Parameters["@Parent2Middlename"].Value = string.IsNullOrEmpty(model.Parent2Middlename) ? (object)DBNull.Value : model.Parent2Middlename;

                    command.Parameters.Add(new SqlParameter("@Parent2Dob", SqlDbType.Date));
                    command.Parameters["@Parent2Dob"].Value = model.Parent2Dob ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@Parent2Phonenumber", SqlDbType.NVarChar));
                    command.Parameters["@Parent2Phonenumber"].Value = string.IsNullOrEmpty(model.Parent2Phonenumber) ? (object)DBNull.Value : model.Parent2Phonenumber;

                    command.Parameters.Add(new SqlParameter("@Parent2Genderid", SqlDbType.Int));
                    command.Parameters["@Parent2Genderid"].Value = model.Parent2Genderid.HasValue ? (object)model.Parent2Genderid : DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@Parent2Emailid", SqlDbType.NVarChar));
                    command.Parameters["@Parent2Emailid"].Value = string.IsNullOrEmpty(model.Parent2Emailid) ? (object)DBNull.Value : model.Parent2Emailid;

                    command.Parameters.Add(new SqlParameter("@Parent2Password", SqlDbType.NVarChar));
                    command.Parameters["@Parent2Password"].Value = string.IsNullOrEmpty(model.Parent2Password) ? (object)DBNull.Value : model.Parent2Password;

                    command.Parameters.Add(new SqlParameter("@Parent2Issmsuser", SqlDbType.Bit));
                    command.Parameters["@Parent2Issmsuser"].Value = model.Parent2Issmsuser;

                    command.Parameters.Add(new SqlParameter("@Parent2Ishigheduser", SqlDbType.Bit));
                    command.Parameters["@Parent2Ishigheduser"].Value = model.Parent2Ishigheduser;

                    await command.ExecuteNonQueryAsync();
                }

                return ("Student Details Added Successfully");
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
        public async Task<object> DeleteStudentDetails(int childId)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.DeleteSchoolStudentDetails, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@ChildId", SqlDbType.Int));
                    command.Parameters["@ChildId"].Value = childId;

                    await command.ExecuteNonQueryAsync();
                }

                return "Student Details Deleted Successfully";
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
        //Teacher Add
        public async Task<SingleTeacherDetailsViewModel> GetSingleTeacherDetails(int teacherId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

            SingleTeacherDetailsViewModel teacherDetails = new SingleTeacherDetailsViewModel();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetSingleTeacherViewDetails, connection);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TeacherId", teacherId);

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        teacherDetails.FirstName = reader["firstname"].ToString();
                        teacherDetails.LastName = reader["lastname"].ToString();
                        teacherDetails.MiddleName = reader["middlename"].ToString();
                        teacherDetails.EmailId = reader["emailid"].ToString();
                        teacherDetails.PhoneNumber = reader["phonenumber"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return teacherDetails;
        }
        public async Task<List<TeacherDropdownDetailsViewModel>> GetDropdownTeacherDetails(int branchId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

            var res = new List<TeacherDropdownDetailsViewModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetDropdownTeacherViewDetails, connection);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BranchId", branchId);

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var teacherDropdownDetails = new TeacherDropdownDetailsViewModel
                        {
                            TeacherId = reader["TeacherId"].ToString(),
                            TeacherName = reader["TeacherName"].ToString()

                        };

                        res.Add(teacherDropdownDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return res;
        }
        public async Task<List<TeacherDetailsViewModel>> GetTeacherDetailsListAsync(int academicYearId, int sectionId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

            var res = new List<TeacherDetailsViewModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetTeacherViewDetails, connection);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
                    command.Parameters.AddWithValue("@SectionId", sectionId);

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var teacherDetails = new TeacherDetailsViewModel
                        {
                            TeacherName = reader["TeacherName"].ToString(),
                            SubjectName = reader["SubjectName"].ToString(),
                        };

                        res.Add(teacherDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return res;
        }
        public async Task<object> SchoolAddTeacherDetails(TeacherAddModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.AddSchoolTeacherDetails, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@salutation", SqlDbType.NVarChar));
                    command.Parameters["@salutation"].Value = model.Salutation;

                    command.Parameters.Add(new SqlParameter("@firstname", SqlDbType.NVarChar));
                    command.Parameters["@firstname"].Value = model.Firstname;

                    command.Parameters.Add(new SqlParameter("@lastname", SqlDbType.NVarChar));
                    command.Parameters["@lastname"].Value = model.Lastname;

                    command.Parameters.Add(new SqlParameter("@middlename", SqlDbType.NVarChar));
                    command.Parameters["@middlename"].Value = model.Middlename;

                    command.Parameters.Add(new SqlParameter("@code", SqlDbType.NVarChar));
                    command.Parameters["@code"].Value = model.Code;

                    command.Parameters.Add(new SqlParameter("@emailid", SqlDbType.NVarChar));
                    command.Parameters["@emailid"].Value = model.Email;

                    command.Parameters.Add(new SqlParameter("@genderid", SqlDbType.Int));
                    command.Parameters["@genderid"].Value = model.Genderid;

                    command.Parameters.Add(new SqlParameter("@username", SqlDbType.NVarChar));
                    command.Parameters["@username"].Value = model.Username;

                    command.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar));
                    command.Parameters["@password"].Value = model.Password;

                    command.Parameters.Add(new SqlParameter("@phonenumber", SqlDbType.NVarChar));
                    command.Parameters["@phonenumber"].Value = model.Phonenumber;

                    command.Parameters.Add(new SqlParameter("@enddate", SqlDbType.Date));
                    command.Parameters["@enddate"].Value = model.Createddate;

                    command.Parameters.Add(new SqlParameter("@createddate", SqlDbType.Date));
                    command.Parameters["@createddate"].Value = model.Createddate;

                    command.Parameters.Add(new SqlParameter("@modifieddate", SqlDbType.Date));
                    command.Parameters["@modifieddate"].Value = model.Modifieddate;




                    await command.ExecuteNonQueryAsync();
                }

                return ("Teacher Details Added Successfully");
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
                                //Parentcreateddate = reader["parentcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                                //Childcreateddate = reader["childcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                            }));

                        }
                    }
                }
                return (IQueryable<GetParentsListModel>)res.AsQueryable().OrderByDescending(x => x.ChildFirstName).DistinctBy(x => x.ChildId);
            }
        }
        
        private async Task<IQueryable<GetParentsListModel>> GetStudentDetailsParentByChildIdAPIListSP(int? parentid, int? childid)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetParentsListModel>();
                connection.Open(); 
                SqlCommand command = new SqlCommand(ApplicationConstants.GetStudentDetailsParentByChildIdAPIListSP, connection);
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
                                //Parentcreateddate = reader["parentcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
                                //Childcreateddate = reader["childcreateddate"] != DBNull.Value ? (DateTime?)reader["parentcreateddate"] : null,
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
        public async Task<object> GetParentsListStudentVIewSP(Guid token, int academicyearid, int activestatus, int nuofRows, int pageNumber, string standard = "", string section = "", string childlastname = "", string searchstring = "")
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
                            var objresult = await this.GetParentsListStudentVIewSP(item.schoolid, academicyearid, activestatus,standard, section, childlastname, searchstring, item.standardid, item.sectionid);
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
        public async Task<ApiResponse> GetStudentDetailsByChildIdForAPI(Guid token, int schoolid, int childid, int AcademicYearId, int nuofRows = 10, int pageNumber = 1)
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
                            var objresult = await this.GetStudentDetailsByChildIdListSP(schoolid, childid, AcademicYearId);
                            if (objresult != null && objresult.Any())
                            {
                                int count = objresult.Count();
                                int CurrentPage = pageNumber;
                                int PageSize = nuofRows;
                                int TotalCount = count;
                                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                                var items = objresult.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                                return new ApiResponse
                                {
                                    TotalPages = TotalPages,
                                    Items = items
                                };
                            }
                            else
                            {
                                return new ApiResponse
                                {
                                    Message = "No data found",
                                    StatusCode = HttpStatusCode.NotFound
                                };
                            }
                        }
                    }
                    else
                    {
                        return new ApiResponse
                        {
                            Message = "User not found",
                            StatusCode = HttpStatusCode.NotFound
                        };
                    }
                }
                else
                {
                    return new ApiResponse
                    {
                        Message = "User not found",
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                return new ApiResponse { Message = "Ok", StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };
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
        public async Task<object> GetStudentDetailsParentByChildIdAPI(int parentid, int childid,int nuofRows = 10, int pageNumber = 1)
        {
            try
            {
                // var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();
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
        public async Task<object> UpdateStudentInfoAPI(Guid token, GetParentsListModel model)
        {
            try
            {
                var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();
                if (userid is not null) // checks if records are updated
                {


                    var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(ApplicationConstants.UpdateStudentInfoAPI, connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                        command.Parameters["@schoolid"].Value = model.Schoolid;

                        command.Parameters.Add(new SqlParameter("@parentid", SqlDbType.Int));
                        command.Parameters["@parentid"].Value = model.id;

                        command.Parameters.Add(new SqlParameter("@childid", SqlDbType.Int));
                        command.Parameters["@childid"].Value = model.ChildId;

                        command.Parameters.Add(new SqlParameter("@StandardId", SqlDbType.Int));
                        command.Parameters["@StandardId"].Value = model.StandardId;

                        command.Parameters.Add(new SqlParameter("@SectionId", SqlDbType.Int));
                        command.Parameters["@SectionId"].Value = model.SectionId;

                        command.Parameters.Add(new SqlParameter("@Inactivestudent", SqlDbType.Int));
                        command.Parameters["@Inactivestudent"].Value = model.Inactivestudent;

                        command.Parameters.Add(new SqlParameter("@phonenumber", SqlDbType.NVarChar));
                        command.Parameters["@phonenumber"].Value = model.PhoneNumber;

                        command.Parameters.Add(new SqlParameter("@StudentImageLink", SqlDbType.NVarChar));
                        command.Parameters["@StudentImageLink"].Value = model.StudentImageLink;


                        command.Parameters.Add(new SqlParameter("@SportType", SqlDbType.NVarChar));
                        command.Parameters["@SportType"].Value = model.SportName;

                        command.Parameters.Add(new SqlParameter("@Blood_Group", SqlDbType.NVarChar));
                        command.Parameters["@Blood_Group"].Value = model.BloodGroup;

                        command.Parameters.Add(new SqlParameter("@Medical_Condition", SqlDbType.NVarChar));
                        command.Parameters["@Medical_Condition"].Value = model.MedicalConditions;

                        command.Parameters.Add(new SqlParameter("@Special_Needs", SqlDbType.NVarChar));
                        command.Parameters["@Special_Needs"].Value = model.SpecialNeeds;

                        command.Parameters.Add(new SqlParameter("@ClubName", SqlDbType.NVarChar));
                        command.Parameters["@ClubName"].Value = model.ClubName;

                        command.Parameters.Add(new SqlParameter("@RegisterationNumber", SqlDbType.NVarChar));
                        command.Parameters["@RegisterationNumber"].Value = model.RegisterationNumber;

                        command.Parameters.Add(new SqlParameter("@HomeAddress", SqlDbType.NVarChar));
                        command.Parameters["@HomeAddress"].Value = model.HomeAddress;

                        command.Parameters.Add(new SqlParameter("@House", SqlDbType.NVarChar));
                        command.Parameters["@House"].Value = model.House;

                        command.Parameters.Add(new SqlParameter("@Hosteler", SqlDbType.NVarChar));
                        command.Parameters["@Hosteler"].Value = model.Hosteler;

                        command.Parameters.Add(new SqlParameter("@Religion", SqlDbType.NVarChar));
                        command.Parameters["@Religion"].Value = model.Religion;

                        command.Parameters.Add(new SqlParameter("@Medium", SqlDbType.NVarChar));
                        command.Parameters["@Medium"].Value = model.Medium;

                        command.Parameters.Add(new SqlParameter("@AcademicYear", SqlDbType.NVarChar));
                        command.Parameters["@AcademicYear"].Value = model.AcademicYearId;

                        command.Parameters.Add(new SqlParameter("@AdmissionYear", SqlDbType.NVarChar));
                        command.Parameters["@AdmissionYear"].Value = model.AdmissionYear;

                        command.Parameters.Add(new SqlParameter("@LeavingYear", SqlDbType.NVarChar));
                        command.Parameters["@LeavingYear"].Value = model.LeavingYear;

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

                        command.Parameters.Add(new SqlParameter("@MotherContactNo", SqlDbType.NVarChar));
                        command.Parameters["@MotherContactNo"].Value = model.MotherContactNo;

                        command.Parameters.Add(new SqlParameter("@MotherEmail", SqlDbType.NVarChar));
                        command.Parameters["@MotherEmail"].Value = model.MotherEmail;

                        command.Parameters.Add(new SqlParameter("@FatheerContactNo", SqlDbType.NVarChar));
                        command.Parameters["@FatheerContactNo"].Value = model.FatheerContactNo;

                        command.Parameters.Add(new SqlParameter("@FatherEmail", SqlDbType.NVarChar));
                        command.Parameters["@FatherEmail"].Value = model.FatherEmail;

                        command.Parameters.Add(new SqlParameter("@GuardianContactNo", SqlDbType.NVarChar));
                        command.Parameters["@GuardianContactNo"].Value = model.GuardianContactNo;

                        command.Parameters.Add(new SqlParameter("@GuardianEmail", SqlDbType.NVarChar));
                        command.Parameters["@GuardianEmail"].Value = model.GuardianEmail;

                        command.Parameters.Add(new SqlParameter("@Prefectship", SqlDbType.NVarChar));
                        command.Parameters["@Prefectship"].Value = model.Prefectship;

                        command.Parameters.Add(new SqlParameter("@scholarship", SqlDbType.NVarChar));
                        command.Parameters["@scholarship"].Value = model.Scholarship;

                        command.Parameters.Add(new SqlParameter("@Discipline", SqlDbType.NVarChar));
                        command.Parameters["@Discipline"].Value = model.Discipline;

                        command.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime));
                        command.Parameters["@DateOfBirth"].Value = model.DateOfBirth;

                        command.Parameters.Add(new SqlParameter("@GuardianName", SqlDbType.NVarChar));
                        command.Parameters["@GuardianName"].Value = model.GuardianName;

                        command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar));
                        command.Parameters["@email"].Value = model.EmailAddress;

                        command.Parameters.Add(new SqlParameter("@IsMotherPastStudent", SqlDbType.NVarChar));
                        command.Parameters["@IsMotherPastStudent"].Value = model.IsMotherPastStudent;

                        command.Parameters.Add(new SqlParameter("@IsFatherPastStudent", SqlDbType.NVarChar));
                        command.Parameters["@IsFatherPastStudent"].Value = model.IsFatherPastStudent;


                        command.Parameters.Add(new SqlParameter("@standardsectionmappingid", SqlDbType.Int));
                        command.Parameters["@standardsectionmappingid"].Value = model.ChildSectionId;


                        command.Parameters.Add(new SqlParameter("@Other", SqlDbType.NVarChar));
                        command.Parameters["@Other"].Value = model.Other;

                        command.Parameters.Add(new SqlParameter("@Childgender", SqlDbType.Int));
                        command.Parameters["@Childgender"].Value = model.ChildGenderId;

                        command.Parameters.Add(new SqlParameter("@IsOtherParentSMSUser", SqlDbType.Int));
                        command.Parameters["@IsOtherParentSMSUser"].Value = model.IsOtherParentSMSUser;

                        command.Parameters.Add(new SqlParameter("@IsSMSUser", SqlDbType.Int));
                        command.Parameters["@IsSMSUser"].Value = model.Issmsuser;

                        command.Parameters.Add(new SqlParameter("@FatherFirstName", SqlDbType.NVarChar));
                        command.Parameters["@FatherFirstName"].Value = model.FatherFirstName ?? (object)DBNull.Value;

                        command.Parameters.Add(new SqlParameter("@FatherLastName", SqlDbType.NVarChar));
                        command.Parameters["@FatherLastName"].Value = model.FatherLastName ?? (object)DBNull.Value;

                        command.Parameters.Add(new SqlParameter("@MotherFirstName", SqlDbType.NVarChar));
                        command.Parameters["@MotherFirstName"].Value = model.MotherFirstName ?? (object)DBNull.Value;

                        command.Parameters.Add(new SqlParameter("@MotherLastName", SqlDbType.NVarChar));
                        command.Parameters["@MotherLastName"].Value = model.MotherLastName ?? (object)DBNull.Value;



                        command.Parameters.Add(new SqlParameter("@ChildFirstName", SqlDbType.NVarChar));
                        command.Parameters["@ChildFirstName"].Value = model.ChildFirstName;

                        command.Parameters.Add(new SqlParameter("@ChildLastName", SqlDbType.NVarChar));
                        command.Parameters["@ChildLastName"].Value = model.ChildLastName;

                        command.Parameters.Add(new SqlParameter("@error", SqlDbType.NVarChar, 400) { Direction = ParameterDirection.Output });

                        command.ExecuteNonQuery();

                        var errorValue = command.Parameters["@error"].Value;
                        if (errorValue != DBNull.Value && !string.IsNullOrEmpty(errorValue.ToString()))
                        {
                            return ("Error: " + errorValue.ToString());
                        }
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
        //public async Task<object> UpdateStudentInfoAPI(Guid token, GetParentsListModel model)
        //{
        //    try
        //    {
        //        var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();
        //        if (userid is not null) // checks if records are updated
        //        {


        //            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
        //            using (SqlConnection connection = new SqlConnection(connectionString))
        //            {
        //                connection.Open();
        //                SqlCommand command = new SqlCommand(ApplicationConstants.UpdateStudentInfoAPI, connection);
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
        //                command.Parameters["@schoolid"].Value = model.Schoolid;

        //                command.Parameters.Add(new SqlParameter("@parentid", SqlDbType.Int));
        //                command.Parameters["@parentid"].Value = model.id;

        //                command.Parameters.Add(new SqlParameter("@childid", SqlDbType.Int));
        //                command.Parameters["@childid"].Value = model.ChildId;

        //                command.Parameters.Add(new SqlParameter("@StandardId", SqlDbType.Int));
        //                command.Parameters["@StandardId"].Value = model.StandardId;

        //                command.Parameters.Add(new SqlParameter("@SectionId", SqlDbType.Int));
        //                command.Parameters["@SectionId"].Value = model.SectionId;

        //                command.Parameters.Add(new SqlParameter("@Inactivestudent", SqlDbType.Int));
        //                command.Parameters["@Inactivestudent"].Value = model.Inactivestudent;

        //                command.Parameters.Add(new SqlParameter("@phonenumber", SqlDbType.NVarChar));
        //                command.Parameters["@phonenumber"].Value = model.PhoneNumber;

        //                command.Parameters.Add(new SqlParameter("@StudentImageLink", SqlDbType.NVarChar));
        //                command.Parameters["@StudentImageLink"].Value = model.StudentImageLink;


        //                command.Parameters.Add(new SqlParameter("@SportType", SqlDbType.NVarChar));
        //                command.Parameters["@SportType"].Value = model.SportName;

        //                command.Parameters.Add(new SqlParameter("@Blood_Group", SqlDbType.NVarChar));
        //                command.Parameters["@Blood_Group"].Value = model.BloodGroup;

        //                command.Parameters.Add(new SqlParameter("@Medical_Condition", SqlDbType.NVarChar));
        //                command.Parameters["@Medical_Condition"].Value = model.MedicalConditions;

        //                command.Parameters.Add(new SqlParameter("@Special_Needs", SqlDbType.NVarChar));
        //                command.Parameters["@Special_Needs"].Value = model.SpecialNeeds;

        //                command.Parameters.Add(new SqlParameter("@ClubName", SqlDbType.NVarChar));
        //                command.Parameters["@ClubName"].Value = model.ClubName;

        //                command.Parameters.Add(new SqlParameter("@RegisterationNumber", SqlDbType.NVarChar));
        //                command.Parameters["@RegisterationNumber"].Value = model.RegisterationNumber;

        //                command.Parameters.Add(new SqlParameter("@HomeAddress", SqlDbType.NVarChar));
        //                command.Parameters["@HomeAddress"].Value = model.HomeAddress;

        //                command.Parameters.Add(new SqlParameter("@House", SqlDbType.NVarChar));
        //                command.Parameters["@House"].Value = model.House;

        //                command.Parameters.Add(new SqlParameter("@Hosteler", SqlDbType.NVarChar));
        //                command.Parameters["@Hosteler"].Value = model.Hosteler;

        //                command.Parameters.Add(new SqlParameter("@Religion", SqlDbType.NVarChar));
        //                command.Parameters["@Religion"].Value = model.Religion;

        //                command.Parameters.Add(new SqlParameter("@Medium", SqlDbType.NVarChar));
        //                command.Parameters["@Medium"].Value = model.Medium;

        //                command.Parameters.Add(new SqlParameter("@AcademicYear", SqlDbType.NVarChar));
        //                command.Parameters["@AcademicYear"].Value = model.AcademicYearId;

        //                command.Parameters.Add(new SqlParameter("@AdmissionYear", SqlDbType.NVarChar));
        //                command.Parameters["@AdmissionYear"].Value = model.AdmissionYear;

        //                command.Parameters.Add(new SqlParameter("@LeavingYear", SqlDbType.NVarChar));
        //                command.Parameters["@LeavingYear"].Value = model.LeavingYear;

        //                command.Parameters.Add(new SqlParameter("@Contact1", SqlDbType.NVarChar));
        //                command.Parameters["@Contact1"].Value = model.Contact1;

        //                command.Parameters.Add(new SqlParameter("@Relationship1", SqlDbType.NVarChar));
        //                command.Parameters["@Relationship1"].Value = model.Relationship1;

        //                command.Parameters.Add(new SqlParameter("@MobileNo1", SqlDbType.NVarChar));
        //                command.Parameters["@MobileNo1"].Value = model.MobileNo1;

        //                command.Parameters.Add(new SqlParameter("@Contact2", SqlDbType.NVarChar));
        //                command.Parameters["@Contact2"].Value = model.Contact2;

        //                command.Parameters.Add(new SqlParameter("@Relationship2", SqlDbType.NVarChar));
        //                command.Parameters["@Relationship2"].Value = model.Relationship2;

        //                command.Parameters.Add(new SqlParameter("@MobileNo2", SqlDbType.NVarChar));
        //                command.Parameters["@MobileNo2"].Value = model.MobileNo2;

        //                command.Parameters.Add(new SqlParameter("@MotherContactNo", SqlDbType.NVarChar));
        //                command.Parameters["@MotherContactNo"].Value = model.MotherContactNo;

        //                command.Parameters.Add(new SqlParameter("@MotherEmail", SqlDbType.NVarChar));
        //                command.Parameters["@MotherEmail"].Value = model.MotherEmail;

        //                command.Parameters.Add(new SqlParameter("@FatheerContactNo", SqlDbType.NVarChar));
        //                command.Parameters["@FatheerContactNo"].Value = model.FatheerContactNo;

        //                command.Parameters.Add(new SqlParameter("@FatherEmail", SqlDbType.NVarChar));
        //                command.Parameters["@FatherEmail"].Value = model.FatherEmail;

        //                command.Parameters.Add(new SqlParameter("@GuardianContactNo", SqlDbType.NVarChar));
        //                command.Parameters["@GuardianContactNo"].Value = model.GuardianContactNo;

        //                command.Parameters.Add(new SqlParameter("@GuardianEmail", SqlDbType.NVarChar));
        //                command.Parameters["@GuardianEmail"].Value = model.GuardianEmail;

        //                command.Parameters.Add(new SqlParameter("@Prefectship", SqlDbType.NVarChar));
        //                command.Parameters["@Prefectship"].Value = model.Prefectship;

        //                command.Parameters.Add(new SqlParameter("@scholarship", SqlDbType.NVarChar));
        //                command.Parameters["@scholarship"].Value = model.Scholarship;

        //                command.Parameters.Add(new SqlParameter("@Discipline", SqlDbType.NVarChar));
        //                command.Parameters["@Discipline"].Value = model.Discipline;

        //                command.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime));
        //                command.Parameters["@DateOfBirth"].Value = model.DateOfBirth;

        //                command.Parameters.Add(new SqlParameter("@GuardianName", SqlDbType.NVarChar));
        //                command.Parameters["@GuardianName"].Value = model.GuardianName;

        //                command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar));
        //                command.Parameters["@email"].Value = model.EmailAddress;

        //                command.Parameters.Add(new SqlParameter("@IsMotherPastStudent", SqlDbType.NVarChar));
        //                command.Parameters["@IsMotherPastStudent"].Value = model.IsMotherPastStudent;

        //                command.Parameters.Add(new SqlParameter("@IsFatherPastStudent", SqlDbType.NVarChar));
        //                command.Parameters["@IsFatherPastStudent"].Value = model.IsFatherPastStudent;


        //                command.Parameters.Add(new SqlParameter("@standardsectionmappingid", SqlDbType.Int));
        //                command.Parameters["@standardsectionmappingid"].Value = model.ChildSectionId;


        //                command.Parameters.Add(new SqlParameter("@Other", SqlDbType.NVarChar));
        //                command.Parameters["@Other"].Value = model.Other;

        //                command.Parameters.Add(new SqlParameter("@Childgender", SqlDbType.Int));
        //                command.Parameters["@Childgender"].Value = model.ChildGenderId;

        //                command.Parameters.Add(new SqlParameter("@IsOtherParentSMSUser", SqlDbType.Int));
        //                command.Parameters["@IsOtherParentSMSUser"].Value = model.IsOtherParentSMSUser;

        //                command.Parameters.Add(new SqlParameter("@IsSMSUser", SqlDbType.Int));
        //                command.Parameters["@IsSMSUser"].Value = model.Issmsuser;

        //                command.Parameters.Add(new SqlParameter("@ChildFirstName", SqlDbType.NVarChar));
        //                command.Parameters["@ChildFirstName"].Value = model.ChildFirstName;

        //                command.Parameters.Add(new SqlParameter("@ChildLastName", SqlDbType.NVarChar));
        //                command.Parameters["@ChildLastName"].Value = model.ChildLastName;

        //                command.Parameters.Add(new SqlParameter("@error", SqlDbType.NVarChar, 400) { Direction = ParameterDirection.Output });

        //                command.ExecuteNonQuery();

        //                var errorValue = command.Parameters["@error"].Value;
        //                if (errorValue != DBNull.Value && !string.IsNullOrEmpty(errorValue.ToString()))
        //                {
        //                    return ("Error: " + errorValue.ToString());
        //                }
        //            }
        //            db.SaveChanges();
        //        }
        //        else
        //        {

        //        }
        //        return ("Updated Successfully");
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //Add New Parent Record to Existing child - 6/6/2024

        public async Task<object> AddParentForExistingChild(ParentAddModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.AddParentForExistingChild, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@ChildId", SqlDbType.Int));
                    command.Parameters["@ChildId"].Value = model.ChildId;

                    command.Parameters.Add(new SqlParameter("@ParentFirstName", SqlDbType.NVarChar));
                    command.Parameters["@ParentFirstName"].Value = string.IsNullOrEmpty(model.ParentFirstName) ? (object)DBNull.Value : model.ParentFirstName;

                    command.Parameters.Add(new SqlParameter("@ParentLastName", SqlDbType.NVarChar));
                    command.Parameters["@ParentLastName"].Value = string.IsNullOrEmpty(model.ParentLastName) ? (object)DBNull.Value : model.ParentLastName;

                    command.Parameters.Add(new SqlParameter("@RelationType", SqlDbType.NVarChar));
                    command.Parameters["@RelationType"].Value = string.IsNullOrEmpty(model.RelationType) ? (object)DBNull.Value : model.RelationType;

                    command.Parameters.Add(new SqlParameter("@ParentMobileNumber", SqlDbType.NVarChar));
                    command.Parameters["@ParentMobileNumber"].Value = string.IsNullOrEmpty(model.ParentMobileNumber) ? (object)DBNull.Value : model.ParentMobileNumber;

                    command.Parameters.Add(new SqlParameter("@ParentEmail", SqlDbType.NVarChar));
                    command.Parameters["@ParentEmail"].Value = string.IsNullOrEmpty(model.ParentEmail) ? (object)DBNull.Value : model.ParentEmail;

                    command.Parameters.Add(new SqlParameter("@IsSmsUser", SqlDbType.Bit));
                    command.Parameters["@IsSmsUser"].Value = model.IsSmsUser;

                    command.Parameters.Add(new SqlParameter("@IsHighEduUser", SqlDbType.Bit));
                    command.Parameters["@IsHighEduUser"].Value = model.IsHighEduUser;

                    await command.ExecuteNonQueryAsync();
                }

                return new { Message = "Parent added successfully" };
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
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
