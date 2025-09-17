using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nancy.Extensions;
using Newtonsoft.Json.Linq;
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
using static Services.MQuetionPaperService;


namespace Services
{
    public interface IMQuetionPaperService : ICommonService
    {
        Task<int> AddEntity(MQuetionPaper entity);
        Task<object> GetEntityBySchoolID6(int EntityID);
        Task<int> DeleteEntity(MQuetionPaper entity);

        Task<int> UpdateEntity(MQuetionPaper entity);
        Task<object> AddQuetionPaper(MQuetionPaperModel model);
        Task<List<MGetQuetionPaperModel>> GetQuestionPapers(int academicYearId, int gradeId, int subjectId, int semesterId, int examId, int questionpapertypeId); Task<List<MGetOneQuetionPaperModel>> GetOneQuestionPapers(int Id);
        Task<object> AddGenerateQuetionPaper(AddGenerateQuetionPaperModel model);
        Task<List<MGetGenerateQuetionPaperModel>> GetGenerateQuestionPapers(int academicYearfromId, int academicYeartoId, int gradeId, int subjectId);
        Task<object> DeleteQuestionPaper(int paperId);
        Task<object> BulkDeleteGenerateQuetionPapers(GenerateQuetionPapersBulkDeleteModel model);
        Task<object> BulkDeleteQuestionPaper(QuestionPaperBulkDeleteModel model);
        Task<List<MGetPromptModel>> GetPromptsBySchoolId(int schoolId);
        Task<object> AddPromptToSchool(AddPromptModel model); 
        Task<object> UpdatePromptsForSchool(UpdatePromptModel model);
        Task<MGetPromptModel> GetPromptByPromptId(int promptId);
        Task<string> QIGetQuestionPapers(int academicYearId, int gradeId, int subjectId);

        Task<string> QIGetSyllabusContent(int academicYearId, int gradeId, int subjectId);

    }
    public class MQuetionPaperService : IMQuetionPaperService
    {
        private readonly IRepository<MQuetionPaper> repository;
        private readonly IConfiguration configuration;
        private DbSet<MQuetionPaper> localDBSet;

        public MQuetionPaperService(
            IRepository<MQuetionPaper> repository,
             IConfiguration configuration
            )
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        #region base QuetionPaper
        private async Task AllEntityValue() => localDBSet = (DbSet<MQuetionPaper>)await this.repository.GetAll();

        private static Object Mapper(MQuetionPaper x) => new
        {
            x.Id,
            x.PaperName,
            x.AcademicYearId,
            x.GradeId,
            x.SubjectId,
            x.SemesterId,
            x.ExamId
        };

        private async Task<IQueryable<MQuetionPaper>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.SubjectId);

        }



        public async Task<int> AddEntity(MQuetionPaper entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MQuetionPaper> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.PaperName.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<object> GetEntityBySchoolID6(int entityID) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.SubjectId == entityID).Select(x => Mapper(x));



        public async Task<int> UpdateEntity(MQuetionPaper entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MQuetionPaper entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }
        //prompt
        public class MGetPromptModel
        {
            public int Id { get; set; }
            public string Promptypename { get; set; }
            public string Promtypefromat { get; set; }
        }

        public async Task<MGetPromptModel> GetPromptByPromptId(int promptId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            MGetPromptModel prompt = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetPromptByPromptId, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PromptId", promptId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            prompt = new MGetPromptModel
                            {
                                Id = (int)reader["Id"],
                                Promptypename = reader["Promptypename"].ToString(),
                                Promtypefromat = reader["Promtypefromat"] == DBNull.Value ? null : reader["Promtypefromat"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetPromptByPromptId: {ex.Message}", ex);
            }

            return prompt;
        }
        public async Task<object> UpdatePromptsForSchool(UpdatePromptModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.UpdatePromptsForSchool, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@SchoolId", SqlDbType.Int));
                    command.Parameters["@SchoolId"].Value = model.SchoolId;

                    command.Parameters.Add(new SqlParameter("@PromptTypeId1", SqlDbType.Int));
                    command.Parameters["@PromptTypeId1"].Value = model.PromptTypeId1 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptType1", SqlDbType.NVarChar, 255));
                    command.Parameters["@PromptType1"].Value = model.PromptType1 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeFormat1", SqlDbType.NVarChar, -1));
                    command.Parameters["@PromptTypeFormat1"].Value = model.PromptTypeFormat1 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeId2", SqlDbType.Int));
                    command.Parameters["@PromptTypeId2"].Value = model.PromptTypeId2 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptType2", SqlDbType.NVarChar, 255));
                    command.Parameters["@PromptType2"].Value = model.PromptType2 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeFormat2", SqlDbType.NVarChar, -1));
                    command.Parameters["@PromptTypeFormat2"].Value = model.PromptTypeFormat2 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeId3", SqlDbType.Int));
                    command.Parameters["@PromptTypeId3"].Value = model.PromptTypeId3 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptType3", SqlDbType.NVarChar, 255));
                    command.Parameters["@PromptType3"].Value = model.PromptType3 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeFormat3", SqlDbType.NVarChar, -1));
                    command.Parameters["@PromptTypeFormat3"].Value = model.PromptTypeFormat3 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeId4", SqlDbType.Int));
                    command.Parameters["@PromptTypeId4"].Value = model.PromptTypeId4 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptType4", SqlDbType.NVarChar, 255));
                    command.Parameters["@PromptType4"].Value = model.PromptType4 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeFormat4", SqlDbType.NVarChar, -1));
                    command.Parameters["@PromptTypeFormat4"].Value = model.PromptTypeFormat4 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeId5", SqlDbType.Int));
                    command.Parameters["@PromptTypeId5"].Value = model.PromptTypeId5 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptType5", SqlDbType.NVarChar, 255));
                    command.Parameters["@PromptType5"].Value = model.PromptType5 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeFormat5", SqlDbType.NVarChar, -1));
                    command.Parameters["@PromptTypeFormat5"].Value = model.PromptTypeFormat5 ?? (object)DBNull.Value;

                    await command.ExecuteNonQueryAsync();
                }

                return "Prompts updated successfully for the school.";
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

        public async Task<object> AddPromptToSchool(AddPromptModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.AddPromptToSchool, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@SchoolId", SqlDbType.Int));
                    command.Parameters["@SchoolId"].Value = model.SchoolId;

                    command.Parameters.Add(new SqlParameter("@PromptType1", SqlDbType.NVarChar));
                    command.Parameters["@PromptType1"].Value = model.PromptType1 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeFormat1", SqlDbType.NVarChar, -1));
                    command.Parameters["@PromptTypeFormat1"].Value = model.PromptTypeFormat1 ?? (object)DBNull.Value;


                    command.Parameters.Add(new SqlParameter("@PromptType2", SqlDbType.NVarChar));
                    command.Parameters["@PromptType2"].Value = model.PromptType2 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeFormat2", SqlDbType.NVarChar));
                    command.Parameters["@PromptTypeFormat2"].Value = model.PromptTypeFormat2 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptType3", SqlDbType.NVarChar));
                    command.Parameters["@PromptType3"].Value = model.PromptType3 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeFormat3", SqlDbType.NVarChar));
                    command.Parameters["@PromptTypeFormat3"].Value = model.PromptTypeFormat3 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptType4", SqlDbType.NVarChar));
                    command.Parameters["@PromptType4"].Value = model.PromptType4 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeFormat4", SqlDbType.NVarChar));
                    command.Parameters["@PromptTypeFormat4"].Value = model.PromptTypeFormat4 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptType5", SqlDbType.NVarChar));
                    command.Parameters["@PromptType5"].Value = model.PromptType5 ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@PromptTypeFormat5", SqlDbType.NVarChar));
                    command.Parameters["@PromptTypeFormat5"].Value = model.PromptTypeFormat5 ?? (object)DBNull.Value;

                    await command.ExecuteNonQueryAsync();
                }

                return ("Prompts added successfully to the school.");
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
        public async Task<List<MGetPromptModel>> GetPromptsBySchoolId(int schoolId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            List<MGetPromptModel> prompts = new List<MGetPromptModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetPromptsBySchoolId, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@SchoolId", schoolId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var prompt = new MGetPromptModel
                            {
                                Id = (int)reader["Id"],
                                Promptypename = reader["Promptypename"].ToString(),
                                Promtypefromat = reader["Promtypefromat"] == DBNull.Value ? null : reader["Promtypefromat"].ToString()
                            };

                            prompts.Add(prompt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetPromptsBySchoolId: {ex.Message}", ex);
            }

            return prompts;
        }
        public async Task<List<MGetGenerateQuetionPaperModel>> GetGenerateQuestionPapers(int academicYearfromId, int academicYeartoId, int gradeId, int subjectId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            List<MGetGenerateQuetionPaperModel> questionPapers = new List<MGetGenerateQuetionPaperModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetGenerateQuetionPaper, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@AcademicYearFromId", academicYearfromId);
                    command.Parameters.AddWithValue("@AcademicYearToId", academicYeartoId);
                    command.Parameters.AddWithValue("@GradeId", gradeId);
                    command.Parameters.AddWithValue("@SubjectId", subjectId);


                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var questionPaper = new MGetGenerateQuetionPaperModel
                            {
                                Id = (int)reader["Id"],
                                PaperName = reader["PaperName"].ToString(),
                                AcademicYearFromName = reader["AcademicYearFromName"].ToString(),
                                AcademicYearToName = reader["AcademicYearToName"].ToString(),
                                GradeId = (int)reader["GradeId"],
                                GradeName = reader["GradeName"].ToString(),
                                SubjectId = (int)reader["SubjectId"],
                                SubjectName = reader["SubjectName"].ToString(),
                                Content = reader["Content"] == DBNull.Value ? null : reader["Content"].ToString(),
                                PDFContent = reader["PDFContent"] == DBNull.Value ? null : reader["PDFContent"].ToString()

                            };

                            questionPapers.Add(questionPaper);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in GetGenerateQuestionPapers: {ex.Message}", ex);
            }

            return questionPapers;
        }
        public async Task<object> AddGenerateQuetionPaper(AddGenerateQuetionPaperModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.AddGenerateQuetionPaper, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@PaperName", SqlDbType.NVarChar));
                    command.Parameters["@PaperName"].Value = model.PaperName;

                    command.Parameters.Add(new SqlParameter("@AcademicYearFromId", SqlDbType.Int));
                    command.Parameters["@AcademicYearFromId"].Value = model.AcademicYearFromId;

                    command.Parameters.Add(new SqlParameter("@AcademicYearToId", SqlDbType.Int));
                    command.Parameters["@AcademicYearToId"].Value = model.AcademicYearToId;

                    command.Parameters.Add(new SqlParameter("@GradeId", SqlDbType.Int));
                    command.Parameters["@GradeId"].Value = model.GradeId;

                    command.Parameters.Add(new SqlParameter("@SubjectId", SqlDbType.Int));
                    command.Parameters["@SubjectId"].Value = model.SubjectId;

                    command.Parameters.Add(new SqlParameter("@SemesterId", SqlDbType.Int));
                    command.Parameters["@SemesterId"].Value = model.SemesterId != 0 ? (object)model.SemesterId : DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@ExamId", SqlDbType.Int));
                    command.Parameters["@ExamId"].Value = model.ExamId != 0 ? (object)model.ExamId : DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@UploadedDate", SqlDbType.Date));
                    command.Parameters["@UploadedDate"].Value = model.UploadedDate;

                    command.Parameters.Add(new SqlParameter("@UploadedBy", SqlDbType.NVarChar));
                    command.Parameters["@UploadedBy"].Value = (model.UploadedBy == null) ? (object)DBNull.Value : model.UploadedBy;

                    command.Parameters.Add(new SqlParameter("@Content", SqlDbType.NVarChar));
                    command.Parameters["@Content"].Value = (model.Content == null) ? (object)DBNull.Value : model.Content;

                    command.Parameters.Add(new SqlParameter("@PDFContent", SqlDbType.NVarChar));
                    command.Parameters["@PDFContent"].Value = (model.PDFContent == null) ? (object)DBNull.Value : model.PDFContent;

                    await command.ExecuteNonQueryAsync();
                }

                return ("Generate Question Paper Added Successfully");
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
        public async Task<object> DeleteQuestionPaper(int paperId)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.DeleteQuestionPaper, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    command.Parameters["@Id"].Value = paperId;

                    await command.ExecuteNonQueryAsync();
                }

                return "Question Paper Deleted Successfully";
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

        public async Task<object> AddQuetionPaper(MQuetionPaperModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.AddQuetionPaper, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@PaperName", SqlDbType.NVarChar));
                    command.Parameters["@PaperName"].Value = model.PaperName;

                    command.Parameters.Add(new SqlParameter("@AcademicYearId", SqlDbType.Int));
                    command.Parameters["@AcademicYearId"].Value = model.AcademicYearId;

                    command.Parameters.Add(new SqlParameter("@GradeId", SqlDbType.Int));
                    command.Parameters["@GradeId"].Value = model.GradeId;

                    command.Parameters.Add(new SqlParameter("@SubjectId", SqlDbType.Int));
                    command.Parameters["@SubjectId"].Value = model.SubjectId;

                    command.Parameters.Add(new SqlParameter("@SemesterId", SqlDbType.Int));
                    command.Parameters["@SemesterId"].Value = model.SemesterId;

                    command.Parameters.Add(new SqlParameter("@ExamId", SqlDbType.Int));
                    command.Parameters["@ExamId"].Value = model.ExamId;

                    command.Parameters.Add(new SqlParameter("@UploadedDate", SqlDbType.Date));
                    command.Parameters["@UploadedDate"].Value = model.UploadedDate;

                    command.Parameters.Add(new SqlParameter("@UploadedBy", SqlDbType.NVarChar));
                    command.Parameters["@UploadedBy"].Value = (model.UploadedBy == null) ? (object)DBNull.Value : model.UploadedBy; ;

                    command.Parameters.Add(new SqlParameter("@Content", SqlDbType.NVarChar));
                    command.Parameters["@Content"].Value = (model.Content == null) ? (object)DBNull.Value : model.Content;

                    command.Parameters.Add(new SqlParameter("@PDFContent", SqlDbType.NVarChar));
                    command.Parameters["@PDFContent"].Value = (model.PDFContent == null) ? (object)DBNull.Value : model.PDFContent;

                    command.Parameters.Add(new SqlParameter("@QuestionPaperTypeId", SqlDbType.Int));
                    command.Parameters["@QuestionPaperTypeId"].Value = model.QuestionPaperTypeId;

                    await command.ExecuteNonQueryAsync();
                }

                return ("Question Paper Added Successfully");
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
        public async Task<object> BulkDeleteGenerateQuetionPapers(GenerateQuetionPapersBulkDeleteModel model)
        {

            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    foreach (var paper in model.Papers)
                    {
                        SqlCommand command = new SqlCommand(ApplicationConstants.DeleteGenerateQuestionPaper, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id", paper.Id);
                        command.Parameters.AddWithValue("@AcademicYearFromId", paper.AcademicYearFromId);
                        command.Parameters.AddWithValue("@AcademicYearToId", paper.AcademicYearToId);
                        command.Parameters.AddWithValue("@GradeId", paper.GradeId);
                        command.Parameters.AddWithValue("@SubjectId", paper.SubjectId);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return "Generate Question Paper Deleted Successfully";
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

        public async Task<object> BulkDeleteQuestionPaper(QuestionPaperBulkDeleteModel model)
        {

            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    foreach (var paper in model.Papers)
                    {
                        SqlCommand command = new SqlCommand(ApplicationConstants.DeleteQuestionPaper, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id", paper.Id);
                        command.Parameters.AddWithValue("@AcademicYearId", paper.AcademicYearId);
                        command.Parameters.AddWithValue("@GradeId", paper.GradeId);
                        command.Parameters.AddWithValue("@SubjectId", paper.SubjectId);
                        command.Parameters.AddWithValue("@SemesterId", paper.SemesterId);
                        command.Parameters.AddWithValue("@ExamId", paper.ExamId);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return "Question Paper Deleted Successfully";
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

        public async Task<object> BulkDeleteSyllabus(SyllabusBulkDeleteModel model)
        {

            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    foreach (var syllabus in model.Syllabuss)
                    {
                        SqlCommand command = new SqlCommand(ApplicationConstants.DeleteSyllabus, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id", syllabus.Id);
                        command.Parameters.AddWithValue("@AcademicYearId", syllabus.AcademicYearId);
                        command.Parameters.AddWithValue("@GradeId", syllabus.GradeId);
                        command.Parameters.AddWithValue("@SubjectId", syllabus.SubjectId);
                        command.Parameters.AddWithValue("@SemesterId", syllabus.SemesterId);
                        command.Parameters.AddWithValue("@ExamId", syllabus.ExamId);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return "Syllabus Deleted Successfully";
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

      


        public async Task<List<MGetOneQuetionPaperModel>> GetOneQuestionPapers(int Id)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            List<MGetOneQuetionPaperModel> questionPapers = new List<MGetOneQuetionPaperModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetOneQuetionPaper, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Id", Id);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var questionPaper = new MGetOneQuetionPaperModel
                            {
                                PDFContent = reader["PDFContent"] == DBNull.Value ? null : reader["PDFContent"].ToString()

                            };

                            questionPapers.Add(questionPaper);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in GetQuestionPapers: {ex.Message}", ex);
            }

            return questionPapers;
        }

        //Jaliya 2023 11 13

       
        public async Task<List<MGetQuetionPaperModel>> GetQuestionPapers(int academicYearId, int gradeId, int subjectId, int semesterId, int examId, int questionpapertypeId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            List<MGetQuetionPaperModel> questionPapers = new List<MGetQuetionPaperModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetQuetionPaper, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
                    command.Parameters.AddWithValue("@GradeId", gradeId);
                    command.Parameters.AddWithValue("@SubjectId", subjectId);
                    command.Parameters.AddWithValue("@SemesterId", semesterId);
                    command.Parameters.AddWithValue("@ExamId", examId);
                    command.Parameters.AddWithValue("@QuestionPaperTypeId", questionpapertypeId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var questionPaper = new MGetQuetionPaperModel
                            {
                                Id = (int)reader["Id"],
                                PaperName = reader["PaperName"].ToString(),
                                AcademicYearId = (int)reader["AcademicYearId"],
                                GradeId = (int)reader["GradeId"],
                                SubjectId = (int)reader["SubjectId"],
                                SemesterId = (int)reader["SemesterId"],
                                ExamId = (int)reader["ExamId"],
                                Content = reader["Content"] == DBNull.Value ? null : reader["Content"].ToString(),
                                //PDFContent = reader["PDFContent"] == DBNull.Value ? null : reader["PDFContent"].ToString()
                                QuestionPaperTypeId = (int)reader["QuestionPaperTypeId"],

                            };

                            questionPapers.Add(questionPaper);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in GetQuestionPapers: {ex.Message}", ex);
            }

            return questionPapers;
        }

        public async Task<string> QIGetQuestionPapers(int academicYearId, int gradeId, int subjectId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            List<string> questionPapers = new List<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.QIGetQuetionPaper, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
                    command.Parameters.AddWithValue("@GradeId", gradeId);
                    command.Parameters.AddWithValue("@SubjectId", subjectId);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {

                            string questionPaper = reader["Content"]?.ToString();
                            questionPapers.Add(questionPaper);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in GetQuestionPapers: {ex.Message}", ex);
            }

            return string.Join(", ", questionPapers);
        }

        public async Task<string> QIGetSyllabusContent(int academicYearId, int gradeId, int subjectId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            List<string> questionPapers = new List<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.QIGetSyllabusContent, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@GradeId", gradeId);
                    command.Parameters.AddWithValue("@SubjectId", subjectId);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {

                            string questionPaper = reader["Content"]?.ToString();
                            questionPapers.Add(questionPaper);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in GetQuestionPapers: {ex.Message}", ex);
            }

            return string.Join(", ", questionPapers);
        }


        #endregion

    }
}

