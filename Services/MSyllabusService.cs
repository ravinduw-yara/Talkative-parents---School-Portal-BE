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
using static Services.MSyllabusService;

namespace Services
{
    public interface IMSyllabusService : ICommonService
    {
        Task<int> AddEntity(MSyllabus entity);
        Task<object> GetEntityBySchoolID6(int EntityID);
        Task<int> DeleteEntity(MSyllabus entity);

        Task<int> UpdateEntity(MSyllabus entity);
        Task<object> AddSyllabus(MSyllabusModel model);
        Task<List<MGetSyllabusModel>> GetSyllabus(int academicYearId, int gradeId, int subjectId, int semesterId, int examId);
        Task<List<MGetOneSyllabusModel>> GetOneSyllabus(int Id);
        Task<List<SyllabusData>> GetGeneratedSyllabus(int academicYearFromId, int academicYearToId, int gradeId, int subjectId);
        Task<object> BulkDeleteSyllabus(SyllabusBulkDeleteModel model);
    }
    public class MSyllabusService : IMSyllabusService
    {
        private readonly IRepository<MSyllabus> repository;
        private readonly IConfiguration configuration;
        private DbSet<MSyllabus> localDBSet;

        public MSyllabusService(
            IRepository<MSyllabus> repository,
             IConfiguration configuration
            )
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        #region base Syllabus
        private async Task AllEntityValue() => localDBSet = (DbSet<MSyllabus>)await this.repository.GetAll();

        private static Object Mapper(MSyllabus x) => new
        {
            x.Id,
            x.PaperName,
            x.AcademicYearId,
            x.GradeId,
            x.SubjectId,
            x.SemesterId,
            x.ExamId
        };

        private async Task<IQueryable<MSyllabus>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.SubjectId);

        }



        public async Task<int> AddEntity(MSyllabus entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MSyllabus> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.PaperName.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<object> GetEntityBySchoolID6(int entityID) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.SubjectId == entityID).Select(x => Mapper(x));



        public async Task<int> UpdateEntity(MSyllabus entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MSyllabus entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<List<SyllabusData>> GetGeneratedSyllabus(int academicYearFromId, int academicYearToId, int gradeId, int subjectId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

            var syllabus = new List<SyllabusData>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetGeneratedSyllabus, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@AcademicYearFromId", academicYearFromId);
                    command.Parameters.AddWithValue("@AcademicYearToId", academicYearToId);
                    command.Parameters.AddWithValue("@GradeId", gradeId);
                    command.Parameters.AddWithValue("@SubjectId", subjectId);

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var syllabusData = new SyllabusData
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            PaperName = reader["PaperName"].ToString(),
                            Content = reader["Content"].ToString(),
                            SubjectId = Convert.ToInt32(reader["SubjectId"]),
                            SubjectName = reader["SubjectName"].ToString(),
                            GradeId = Convert.ToInt32(reader["GradeId"]),
                            GradeName = reader["GradeName"].ToString(),
                            AcademicYearName = reader["AcademicYearName"].ToString(),
                            SemesterId = Convert.ToInt32(reader["SemesterId"]),
                            SemesterName = reader["SemesterName"].ToString(),
                        };

                        syllabus.Add(syllabusData);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return syllabus;
        }
        public async Task<List<MGetOneSyllabusModel>> GetOneSyllabus(int Id)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            List<MGetOneSyllabusModel> questionPapers = new List<MGetOneSyllabusModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetOneSyllabus, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Id", Id);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var questionPaper = new MGetOneSyllabusModel
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


        public async Task<object> AddSyllabus(MSyllabusModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.AddSyllabus, connection);
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

                    await command.ExecuteNonQueryAsync();
                }

                return ("Syllabus Added Successfully");
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


        //Jaliya 2024 03 07

        public async Task<List<MGetSyllabusModel>> GetSyllabus(int academicYearId, int gradeId, int subjectId, int semesterId, int examId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            List<MGetSyllabusModel> syllabuss = new List<MGetSyllabusModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetSyllabus, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
                    command.Parameters.AddWithValue("@GradeId", gradeId);
                    command.Parameters.AddWithValue("@SubjectId", subjectId);
                    command.Parameters.AddWithValue("@SemesterId", semesterId);
                    command.Parameters.AddWithValue("@ExamId", examId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var syllabus = new MGetSyllabusModel
                            {
                                Id = (int)reader["Id"],
                                PaperName = reader["PaperName"].ToString(),
                                AcademicYearId = (int)reader["AcademicYearId"],
                                GradeId = (int)reader["GradeId"],
                                SubjectId = (int)reader["SubjectId"],
                                SemesterId = (int)reader["SemesterId"],
                                ExamId = (int)reader["ExamId"],
                                Content = reader["Content"] == DBNull.Value ? null : reader["Content"].ToString(),
                                PDFContent = reader["PDFContent"] == DBNull.Value ? null : reader["PDFContent"].ToString()

                            };

                            syllabuss.Add(syllabus);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in GetSyllabus: {ex.Message}", ex);
            }

            return syllabuss;
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

        #endregion

    }
}

