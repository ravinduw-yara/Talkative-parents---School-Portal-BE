using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository.DBContext;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using static Services.DataAnalysisServices;
using static Services.MStandardsectionmappingService;
using static System.Collections.Specialized.BitVector32;
namespace Services
{
    public interface IDataAnalysisServices
    {
        Task<double?> GetOverallStudentPercentage(int childid, int testid, int semyearid);
        Task<object> GetAveragePerformances(int sectionid, int testid, int childid, int academicYear);
        List<GetSubjectsSectionModel> GetSubSecInfo(int sectionid, int subjectid, int semesterid);
        double? calculatePercentage(double count, double size);
        Task<double?> GetSubjectSemesterPercentage(int semesterid, int childid, int subjectid);
        Task<double?> GetOverallSubjectPercentage(int childid, int testid, int subjectid);
        Task<string> GetSemesterPositionVsSection(int semesterid, int subsecid, double percentage);
        Task<string> GetSemesterPositionVsClass(int semesterid, int standardid, int subjectid, double percentage);
        Task<object> GetStudentsRc(int childid, int testid, int schoolid, int academicYearId);
        Task<object> UpdateStudentsRc(postStudentRCModel model);
        Task<object> GetSubjectRankClassStudentCharts(int childid, int SubjectId, int schoolid);
        Task<object> GetSubSubjectRankByClass(int childid, int SubjectId, int SubSubjectId, int LevelID, int schoolid, int TestId);
        Task<object> GetStudentsChartOverallPrecentageRank(int childid, int schoolid);
        Task<object> GetOverallTwoSubjectGradePrecentage(int SubjectId1, int SubjectId2, int GradeId, int TestId);

        Task<object> GetStudentMainSubjectSubSubjectMarks(int childid, int SubjectId, int Sectionid, int schoolid, int TestId);
        Task<object> GeOverallSubjectMarksOverallRankByGrade(int SubjectId1, int GradeId, int TestId, int? AcademicYearId);
        Task<object> GetOvverallGradeSubjectMarksinGradingMethod(int SubjectId1, int GradeId, int AcademicYear, int SchoolId, int SemesterId, int ExamId);
        Task<object> GetHiEduStudentsRC(int courseid, int batchid, int childid, int schoolid, int semeseterid, int examid); 
        Task<object> HiEduUpdateStudentsRc(postHiEduStudentRCModel model);
        Task<List<SubjectMarks>> HiEduGetAllSubjectMarksForStudent(int childId, int examId);
        Task<List<SubjectAverageScore>> GetHiEduAverageSubjectMarks(int courseId, int batchId, int semesterId, int examId);
        Task<List<HiEduMarksGroup>> GetHieduModuleStudentPerformanceScoreChart(int batchId, int semesterId, int examId, int subjectId);
        Task<List<SubSubjectMarks>> GetHiEduSubSubjectMarksForStudent(int childId, int subjectId);
        Task<bool> UpdateAcadamicYear(int id, string yearName);
        Task<bool> UpdateLevel(int id, string levels); 
        Task<bool> UpdateSemester(int id, string name);
        Task<bool> UpdateGrade(int id, string name);
        Task<bool> UpdateSubSubject(int id, string subSubject, int precentage, int subMaxMarks);
        Task<(List<ScoreData>, List<GradePercentage>)> GetPercentageScoreBySubjectChart(int levelId, int standardId, int semesterId, int testId, int subjectId);
        List<GradePercentage> CalculateGradePercentage(List<ScoreData> scores);
        Task<List<StudentCountData>> GetNumberofstudentforsubjectChart(int levelId, int standardId, int subjectId);
        Task<(List<ScoreData>, List<GradePercentage>)> GetTeacharsSubjectPercentageScoreByYearChart(int levelId, int testId, int subjectId, int teacherId);
        Task<object> AddSubjectTestMapping(SubjectTestMappingAddModel model); 
        Task<List<QuestionPaperData>> GetGeneratedQuestions(int academicYearFromId, int academicYearToId, int gradeId, int subjectId);
        Task<List<GetFreezeEnableModel>> GetFreezeEnable(int academicYearId, int levelId, int semesterId, int examId); 
        Task<object> AddFreezeEnable(AddFreezeEnableModel model);
        public Task<StudentPerformanceModel> GetStudentPerformanceSemester(int sectionId, int testId, int childId, int academicYearId);
        Task<object> GetStudentSemesterSummary(int childid, string testName, int schoolid, int academicYearId);
        Task<List<Prompt>> GetQIGeneratedQuestions(List<Prompt> prompts, int schoolid, int Lang, int quetioncount, int subjectId);
        Task<List<Prompt>> QIEditDiagram(string special_query, string avgimage, string quetioncontent, int subjectId, int Lang, List<QueList> quelist);
        Task<List<Prompt>> CovertTotLang(string quetioncontent, int subjectId,int schoolid, int Lang);
        Task<List<Prompt>> GetQIGeneratedSyllabusPrompt(string prompt, int schoolid, int academicYearId, int gradeId, int subjectId, int Lang);
        Task<ClassMarkSummaryResponseDto> GetClassMarksSummaryAsync(int academicYearId,int sectionId,int testId);

        Task<ClassListSummaryResponseDto> GetClassListSummaryAsync(int academicYearId, int standardId, int? sectionId);

    }
    public class DataAnalysisServices : IDataAnalysisServices
    {

        private readonly TpContext db = new TpContext();
        private readonly TpContext db1 = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext db3 = new TpContext();
        private readonly TpContext db4 = new TpContext();
        private readonly TpContext db5 = new TpContext();

        private readonly IConfiguration configuration;
        private readonly string _connectionString;

        public DataAnalysisServices(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TpConnectionString");
            this.configuration = configuration;
        }


        #region QI
       

        public async Task<ClassListSummaryResponseDto> GetClassListSummaryAsync(
    int academicYearId,
    int standardId,
    int? sectionId = null)
        {
            var resp = new ClassListSummaryResponseDto();
            var list = new List<ClassListSummaryDto>();

            using var con = new SqlConnection(configuration.GetConnectionString(ApplicationConstants.TPConnectionString));
            await con.OpenAsync();

            using var cmd = new SqlCommand(ApplicationConstants.GetClassListSummary, con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@AcademicYearId", academicYearId);
            cmd.Parameters.AddWithValue("@StandardId", standardId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId.HasValue ? sectionId.Value : (object)DBNull.Value);

            using var reader = await cmd.ExecuteReaderAsync();
            var first = true;
            while (await reader.ReadAsync())
            {
                if (first)
                {
                    resp.AcademicYearName = reader["AcademicYearName"] as string ?? "";
                    resp.GradeName = reader["GradeName"] as string ?? "";
                    first = false;
                }

                list.Add(new ClassListSummaryDto
                {
                    SectionName = reader["SectionName"]?.ToString() ?? "",
                    AdmissionNo = reader["AdmissionNo"]?.ToString() ?? "",
                    FirstName = reader["FirstName"]?.ToString() ?? "",
                    LastName = reader["LastName"]?.ToString() ?? ""
                });
            }

            resp.Data = list;
            return resp;
        }
        

        public async Task<ClassMarkSummaryResponseDto> GetClassMarksSummaryAsync(
            int academicYearId, int sectionId, int testId)
        {
            var resp = new ClassMarkSummaryResponseDto();
            var list = new List<ClassMarkSummaryDto>();

            using var con = new SqlConnection(configuration.GetConnectionString(ApplicationConstants.TPConnectionString));
            await con.OpenAsync();

            using var cmd = new SqlCommand(ApplicationConstants.GetClassMarksSummary, con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@AcademicYearId", academicYearId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@TestId", testId);

            using var reader = await cmd.ExecuteReaderAsync();
            var cols = reader.GetSchemaTable()!.Rows
                .Cast<DataRow>()
                .Select(r => (string)r["ColumnName"]!)
                .ToList();

            var fixedCols = new[]
            {
    "AcademicYearName","GradeSectionName","TermName",
    "Position","ChildId","AdmissionNo","FullName",
    "TotalMarks","AverageMarks","OverallComments"
};
            var subjectCols = cols.Except(fixedCols).ToList();

            bool first = true;
            while (await reader.ReadAsync())
            {
                if (first)
                {
                    resp.AcademicYearName = reader["AcademicYearName"] as string ?? "";
                    resp.GradeSectionName = reader["GradeSectionName"] as string ?? "";
                    resp.TermName = reader["TermName"] as string ?? "";
                    first = false;
                }

                var dto = new ClassMarkSummaryDto
                {
                    Position = Convert.ToInt32(reader["Position"]),
                    AdmissionNo = reader["AdmissionNo"]!.ToString()!,
                    FullName = reader["FullName"]!.ToString()!,
                    TotalMarks = Convert.ToDecimal(reader["TotalMarks"]),
                    AverageMarks = Convert.ToDecimal(reader["AverageMarks"]),
                    OverallComments = reader["OverallComments"] as string ?? ""
                };

                foreach (var c in subjectCols)
                    dto.SubjectMarks[c] = reader[c] is DBNull ? null : Convert.ToDecimal(reader[c]);

                list.Add(dto);
            }

            resp.Data = list;
            return resp;
        }
        public async Task<List<QuestionPaperData>> GetGeneratedQuestions(int academicYearFromId, int academicYearToId, int gradeId, int subjectId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

            var questionPapers = new List<QuestionPaperData>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetGeneratedQuestions, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@AcademicYearFromId", academicYearFromId);
                    command.Parameters.AddWithValue("@AcademicYearToId", academicYearToId);
                    command.Parameters.AddWithValue("@GradeId", gradeId);
                    command.Parameters.AddWithValue("@SubjectId", subjectId);

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {

                        var questionPaperData = new QuestionPaperData
                        {
                            Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? default : Convert.ToInt32(reader["Id"]),
                            PaperName = reader.IsDBNull(reader.GetOrdinal("PaperName")) ? string.Empty : reader["PaperName"].ToString(),
                            Content = reader.IsDBNull(reader.GetOrdinal("Content")) ? string.Empty : reader["Content"].ToString(),
                            SubjectId = reader.IsDBNull(reader.GetOrdinal("SubjectId")) ? default : Convert.ToInt32(reader["SubjectId"]),
                            SubjectName = reader.IsDBNull(reader.GetOrdinal("SubjectName")) ? string.Empty : reader["SubjectName"].ToString(),
                            GradeId = reader.IsDBNull(reader.GetOrdinal("GradeId")) ? default : Convert.ToInt32(reader["GradeId"]),
                            GradeName = reader.IsDBNull(reader.GetOrdinal("GradeName")) ? string.Empty : reader["GradeName"].ToString(),
                            AcademicYearName = reader.IsDBNull(reader.GetOrdinal("AcademicYearName")) ? string.Empty : reader["AcademicYearName"].ToString(),
                            SemesterId = reader.IsDBNull(reader.GetOrdinal("SemesterId")) ? default : Convert.ToInt32(reader["SemesterId"]),
                            SemesterName = reader.IsDBNull(reader.GetOrdinal("SemesterName")) ? string.Empty : reader["SemesterName"].ToString(),
                            Prompttype1ID = reader.IsDBNull(reader.GetOrdinal("Prompttype1ID")) ? default : Convert.ToInt32(reader["Prompttype1ID"]),
                            Prompttype1Name = reader.IsDBNull(reader.GetOrdinal("Prompttype1Name")) ? string.Empty : reader["Prompttype1Name"].ToString(),
                            Prompttype1Format = reader.IsDBNull(reader.GetOrdinal("Prompttype1Format")) ? string.Empty : reader["Prompttype1Format"].ToString(),
                            Prompttype2ID = reader.IsDBNull(reader.GetOrdinal("Prompttype2ID")) ? default : Convert.ToInt32(reader["Prompttype2ID"]),
                            Prompttype2Name = reader.IsDBNull(reader.GetOrdinal("Prompttype2Name")) ? string.Empty : reader["Prompttype2Name"].ToString(),
                            Prompttype2Format = reader.IsDBNull(reader.GetOrdinal("Prompttype2Format")) ? string.Empty : reader["Prompttype2Format"].ToString(),
                            Prompttype3ID = reader.IsDBNull(reader.GetOrdinal("Prompttype3ID")) ? default : Convert.ToInt32(reader["Prompttype3ID"]),
                            Prompttype3Name = reader.IsDBNull(reader.GetOrdinal("Prompttype3Name")) ? string.Empty : reader["Prompttype3Name"].ToString(),
                            Prompttype3Format = reader.IsDBNull(reader.GetOrdinal("Prompttype3Format")) ? string.Empty : reader["Prompttype3Format"].ToString(),
                            Prompttype4ID = reader.IsDBNull(reader.GetOrdinal("Prompttype4ID")) ? default : Convert.ToInt32(reader["Prompttype4ID"]),
                            Prompttype4Name = reader.IsDBNull(reader.GetOrdinal("Prompttype4Name")) ? string.Empty : reader["Prompttype4Name"].ToString(),
                            Prompttype4Format = reader.IsDBNull(reader.GetOrdinal("Prompttype4Format")) ? string.Empty : reader["Prompttype4Format"].ToString(),
                            Prompttype5ID = reader.IsDBNull(reader.GetOrdinal("Prompttype5ID")) ? default : Convert.ToInt32(reader["Prompttype5ID"]),
                            Prompttype5Name = reader.IsDBNull(reader.GetOrdinal("Prompttype5Name")) ? string.Empty : reader["Prompttype5Name"].ToString(),
                            Prompttype5Format = reader.IsDBNull(reader.GetOrdinal("Prompttype5Format")) ? string.Empty : reader["Prompttype5Format"].ToString(),
                        };

                        questionPapers.Add(questionPaperData);

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return questionPapers;

        }
     public async Task<List<Prompt>> GetQIGeneratedQuestions(List<Prompt> prompts, int schoolid, int Lang, int quetioncount, int subjectId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

            var PromptContent = new List<Prompt>();

            try
            {
                foreach (var prompt in prompts)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        SqlCommand command = new SqlCommand(ApplicationConstants.QIGetPromptsBySchool, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@PromptTypeName", prompt.PromptTypeName);
                        command.Parameters.AddWithValue("@schoolid", schoolid);
                        command.Parameters.AddWithValue("@Lang", Lang);
                        command.Parameters.AddWithValue("@Quetioncount", quetioncount);
                        command.Parameters.AddWithValue("@SubjectId", subjectId);
                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {

                            var questionPaperData = new Prompt
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? default : Convert.ToInt32(reader["Id"]),
                                PromptTypeName = reader.IsDBNull(reader.GetOrdinal("Prompt_Type")) ? string.Empty : reader["Prompt_Type"].ToString(),
                                PromptContent = reader.IsDBNull(reader.GetOrdinal("Prompt")) ? string.Empty : reader["Prompt"].ToString(),
                            };

                            PromptContent.Add(questionPaperData);

                        }

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return PromptContent;
        }
     public async Task<List<Prompt>> GetQIGeneratedSyllabusPrompt(string prompt, int schoolid, int academicYearId, int gradeId, int subjectId, int Lang)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

            var PromptContent = new List<Prompt>();

            try
            {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        SqlCommand command = new SqlCommand(ApplicationConstants.QIGetSyllabusPromptsBySchool, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@schoolid", schoolid);
                        command.Parameters.AddWithValue("@Lang", Lang);
                        command.Parameters.AddWithValue("@SubjectId", subjectId);
                        command.Parameters.AddWithValue("@gradeId", gradeId);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {

                            var questionPaperData = new Prompt
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? default : Convert.ToInt32(reader["Id"]),
                                PromptTypeName = reader.IsDBNull(reader.GetOrdinal("Prompt_Type")) ? string.Empty : reader["Prompt_Type"].ToString(),
                                PromptContent = reader.IsDBNull(reader.GetOrdinal("Prompt")) ? string.Empty : reader["Prompt"].ToString(),
                            };

                            PromptContent.Add(questionPaperData);

                        }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return PromptContent;
        }

        public async Task<List<Prompt>> QIEditDiagram(string special_query, string avgimage, string quetioncontent, int subjectId, int Lang, List<QueList> quelist)
        {

           
            return null;
        }
        public async Task<List<Prompt>> CovertTotLang(string quetioncontent, int schoolid,int subjectId, int Lang)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

            var PromptContent = new List<Prompt>();

            try
            {
               
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        SqlCommand command = new SqlCommand(ApplicationConstants.QIGetLangConvertPromptsBySchool, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@schoolid", schoolid);
                        command.Parameters.AddWithValue("@Lang", Lang);
                        command.Parameters.AddWithValue("@SubjectId", subjectId);
                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {

                            var questionPaperData = new Prompt
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? default : Convert.ToInt32(reader["Id"]),
                                PromptTypeName = reader.IsDBNull(reader.GetOrdinal("Prompt_Type")) ? string.Empty : reader["Prompt_Type"].ToString(),
                                PromptContent = reader.IsDBNull(reader.GetOrdinal("Prompt")) ? string.Empty : reader["Prompt"].ToString(),
                            };

                            PromptContent.Add(questionPaperData);

                        }

                    
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return PromptContent;
        }
        #endregion

        #region GetPercentageScoreBySubjectChart
        public async Task<object> AddSubjectTestMapping(SubjectTestMappingAddModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    foreach (int subjectId in model.SubjectIds)
                    {

                        SqlCommand command = new SqlCommand(ApplicationConstants.PostSubjecttestmapping, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@AcademicYearId", SqlDbType.Int));
                        command.Parameters["@AcademicYearId"].Value = model.AcademicYearId;

                        command.Parameters.Add(new SqlParameter("@SectionId", SqlDbType.Int));
                        command.Parameters["@SectionId"].Value = model.SectionId;

                        command.Parameters.Add(new SqlParameter("@SemesterId", SqlDbType.Int));
                        command.Parameters["@SemesterId"].Value = model.SemesterId;

                        command.Parameters.Add(new SqlParameter("@SubjectId", SqlDbType.Int));
                        command.Parameters["@SubjectId"].Value = subjectId;

                        command.Parameters.Add(new SqlParameter("@TestId", SqlDbType.Int)); 
                        command.Parameters["@TestId"].Value = model.TestId;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return ("Subject Test Mapping Added Successfully");
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
        //public async Task<object> AddSubjectTestMapping(SubjectTestMappingAddModel model)
        //{
        //    try
        //    {
        //        var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {

        //            connection.Open();
        //            SqlCommand command = new SqlCommand(ApplicationConstants.PostSubjecttestmapping, connection);
        //            command.CommandType = CommandType.StoredProcedure;

        //            command.Parameters.Add(new SqlParameter("@AcademicYearId", SqlDbType.Int));
        //            command.Parameters["@AcademicYearId"].Value = model.AcademicYearId;

        //            command.Parameters.Add(new SqlParameter("@SectionId", SqlDbType.Int));
        //            command.Parameters["@SectionId"].Value = model.SectionId;

        //            command.Parameters.Add(new SqlParameter("@SemesterId", SqlDbType.Int));
        //            command.Parameters["@SemesterId"].Value = model.SemesterId;

        //            command.Parameters.Add(new SqlParameter("@SubjectId", SqlDbType.Int));
        //            command.Parameters["@SubjectId"].Value = model.SubjectId;

        //            command.Parameters.Add(new SqlParameter("@TestId", SqlDbType.Int));
        //            command.Parameters["@TestId"].Value = model.TestId;


        //            await command.ExecuteNonQueryAsync();
        //        }

        //        return ("Subject Test Mapping Added Successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return (new
        //        {
        //            Data = ex.Message,
        //            StatusCode = HttpStatusCode.InternalServerError,
        //        });
        //    }
        //}

        public async Task<(List<ScoreData>, List<GradePercentage>)> GetPercentageScoreBySubjectChart(int levelId, int standardId, int semesterId, int testId, int subjectId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

            var scores = new List<ScoreData>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetPercentageScoreBySubjectChart, connection);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@LevelId", levelId);
                    command.Parameters.AddWithValue("@StandardId", standardId);
                    command.Parameters.AddWithValue("@SemesterId", semesterId);
                    command.Parameters.AddWithValue("@TestId", testId);
                    command.Parameters.AddWithValue("@SubjectId", subjectId);

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var marks = Convert.ToInt32(reader["Marks"]);
                        var grade = "Fail";

                        if (marks >= 75 && marks <= 100)
                        {
                            grade = "A";
                        }
                        else if (marks >= 65 && marks < 75)
                        {
                            grade = "B";
                        }
                        else if (marks >= 55 && marks < 65)
                        {
                            grade = "C";
                        }

                        var score = new ScoreData
                        {
                            Year = reader["Year"].ToString(),
                            SemesterName = reader["SemesterName"].ToString(),
                            Marks = marks,
                            Grade = grade
                        };

                        scores.Add(score);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            var gradePercentages = CalculateGradePercentage(scores);

            return (scores, gradePercentages);
        }




        public class GradePercentage
        {
            public string Grade { get; set; }
            public double Percentage { get; set; }
            public string Year { get; set; }
        }


        public List<GradePercentage> CalculateGradePercentage(List<ScoreData> scores)
        {
            var gradePercentages = new List<GradePercentage>();

            foreach (var year in scores.Select(x => x.Year).Distinct())
            {
                int totalScoresForYear = scores.Count(x => x.Year == year);

                foreach (var grade in scores.Where(x => x.Year == year).Select(x => x.Grade).Distinct())
                {
                    double gradeCount = scores.Count(x => x.Grade == grade && x.Year == year);
                    double gradePercentage = (gradeCount / totalScoresForYear) * 100;

                    gradePercentages.Add(new GradePercentage { Grade = grade, Percentage = gradePercentage, Year = year });
                }
            }

            return gradePercentages;
        }


        public class ScoreData
        {
            public string Year { get; set; }
            public string SemesterName { get; set; }
            public int Marks { get; set; }
            public string Grade { get; set; }
        }




        #endregion
        #region Report Card
        public async Task<(List<ScoreData>, List<GradePercentage>)> GetTeacharsSubjectPercentageScoreByYearChart(int levelId, int testId, int subjectId, int teacherId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

            var scores = new List<ScoreData>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetTeacharsSubjectPercentageScoreByYearChart, connection);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@LevelId", levelId);

                    command.Parameters.AddWithValue("@TestId", testId);
                    command.Parameters.AddWithValue("@SubjectId", subjectId);
                    command.Parameters.AddWithValue("@TeacherId", teacherId);

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var marks = Convert.ToInt32(reader["Marks"]);
                        var grade = "Fail";

                        if (marks >= 75 && marks <= 100)
                        {
                            grade = "A";
                        }
                        else if (marks >= 65 && marks < 75)
                        {
                            grade = "B";
                        }
                        else if (marks >= 55 && marks < 65)
                        {
                            grade = "C";
                        }

                        var score = new ScoreData
                        {
                            Year = reader["Year"].ToString(),
                            SemesterName = reader["SemesterName"].ToString(),
                            Marks = marks,
                            Grade = grade
                        };

                        scores.Add(score);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            var gradePercentages = CalculateGradePercentage(scores);

            return (scores, gradePercentages);
        }
        public async Task<bool> UpdateAcadamicYear(int id, string yearName)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.UpdateYear, connection);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    command.Parameters["@Id"].Value = id;
                    command.Parameters.Add(new SqlParameter("@YearName", SqlDbType.NVarChar));
                    command.Parameters["@YearName"].Value = yearName;

                    var affectedRows = await command.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<bool> UpdateLevel(int id, string levels)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.UpdateLevels, connection);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    command.Parameters["@Id"].Value = id;
                    command.Parameters.Add(new SqlParameter("@Levels", SqlDbType.NVarChar));
                    command.Parameters["@Levels"].Value = levels;

                    var affectedRows = await command.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<bool> UpdateSemester(int id, string name)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.UpdateSemester, connection);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    command.Parameters["@Id"].Value = id;
                    command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar));
                    command.Parameters["@Name"].Value = name;

                    var affectedRows = await command.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<bool> UpdateGrade(int id, string name)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.UpdateGrade, connection);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    command.Parameters["@Id"].Value = id;
                    command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar));
                    command.Parameters["@Name"].Value = name;

                    var affectedRows = await command.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<bool> UpdateSubSubject(int id, string subSubject, int precentage, int subMaxMarks)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.UpdateSubSubject, connection);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    command.Parameters["@Id"].Value = id;
                    command.Parameters.Add(new SqlParameter("@SubSubject", SqlDbType.NVarChar));
                    command.Parameters["@SubSubject"].Value = subSubject;
                    command.Parameters.Add(new SqlParameter("@Precentage", SqlDbType.Int));
                    command.Parameters["@Precentage"].Value = precentage;
                    command.Parameters.Add(new SqlParameter("@SubMaxMarks", SqlDbType.Int));
                    command.Parameters["@SubMaxMarks"].Value = subMaxMarks;

                    var affectedRows = await command.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //Jaliya Semester Sunmamry 30 OCT 2024
        public async Task<object> GetStudentSemesterSummary(int childid, string testName, int schoolid, int academicYearId)
        {
            try
            {
                var studentRCList = new List<studentRCModelSummery>();


                var testMappings = await db.MSemestertestsmappings
                    .Where(x => x.Name == testName)
                    .Select(x => new { x.Id, x.SemesterId, x.Name })
                    .ToListAsync();

                if (testMappings == null || testMappings.Count == 0)
                {
                    return "Test not found for the given test name.";
                }

                foreach (var testMapping in testMappings)
                {
                    var testId = testMapping.Id;
                    var semesterId = testMapping.SemesterId;
                    var testNameMapped = testMapping.Name;

                    // semester name
                    var semesterName = await db.MSemestertestsmappings
                        .Where(x => x.Id == semesterId)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync();

                    var stdRC = await (from ctm in db.MChildtestmappings
                                       join stm in db.MSubjecttestmappings on ctm.SubjectTestMappingId equals stm.Id
                                       join sym in db.MSemesteryearmappings on stm.SemesterYearMappingId equals sym.Id
                                       join ssm in db.MSubjectsectionmappings on stm.SubjectSectionMappingId equals ssm.Id
                                       join sub in db.MSubjects on ssm.SubjectId equals sub.Id
                                       where ctm.ChildId == childid && stm.TestId == testId && sym.AcademicYearId == academicYearId
                                       orderby sym.SemesterId, sub.DRCSubjectOrder
                                       select new
                                       {
                                           ChildTestId = ctm.Id,
                                           SubjectID = sub.Id,
                                           Subject = sub.Name,
                                           ctm.Marks,
                                           stm.MaxMarks,
                                           ctm.Percentage,
                                           ctm.Comments,
                                           ctm.IsAbsent,
                                           sym.SemesterId,
                                           sym.AcademicYearId
                                       }).ToListAsync();

                    if (stdRC.Count == 0)
                    {
                        continue;
                    }

                    var stdRCM = new studentRCModelSummery
                    {
                        SemesterName = semesterName,
                        TestName = testNameMapped
                    };

                    foreach (var item in stdRC)
                    {
                        var src = new studentRCInnerModelSummery
                        {
                            ChildTestMappingId = item.ChildTestId,
                            Subject = item.Subject,
                            MaxMarks = item.MaxMarks,
                            Comments = item.Comments,
                            Position = (item.Marks == null || item.IsAbsent == true) ? "N/A" : GetStudentPosition(item.ChildTestId, item.Marks)
                        };

                        var subsubjects = db1.MSubSubjects.Where(x => x.SubjectId == item.SubjectID).ToList();
                        int subCount = subsubjects.Count;

                        src.SubSubjectsIds = new int?[subCount];
                        src.SubSubjects = new string[subCount];
                        src.SubSubjectsMarks = new int?[subCount];
                        src.SubSubjectWeight = new int?[subCount];
                        src.SubSubjectMaxMarks = new int?[subCount];
                        src.SubSubjectAbsent = new int?[subCount];

                        for (int i = 0; i < subCount; i++)
                        {
                            var sub = subsubjects[i];
                            src.SubSubjectsIds[i] = sub.Id;
                            src.SubSubjects[i] = sub.SubSubject;
                            src.SubSubjectsMarks[i] = await db3.MSubsubjectmarkss
                                .Where(x => x.SubSubjectId == sub.Id && x.ChildTestMappingId == item.ChildTestId)
                                .Select(w => w.Marks).FirstOrDefaultAsync();
                            src.SubSubjectWeight[i] = sub.Precentage;
                            src.SubSubjectMaxMarks[i] = sub.SubMaxMarks;
                            src.SubSubjectAbsent[i] = await db3.MSubsubjectmarkss
                                .Where(x => x.SubSubjectId == sub.Id && x.ChildTestMappingId == item.ChildTestId)
                                .Select(w => w.Absent).FirstOrDefaultAsync();
                        }

                        stdRCM.studentRCInnerModelsSummery.Add(src);
                    }

                    stdRCM.pdfmodel = db2.MSchools.Where(w => w.Id == schoolid).Select(w => w.pdfmodel).FirstOrDefault() ?? 1;
                    stdRCM.drcmodel = db2.MSchools.Where(w => w.Id == schoolid).Select(w => w.drcmodel).FirstOrDefault() ?? 1;

                    var sectionid = await db2.MChildschoolmappings
                        .Where(w => w.Childid == childid && w.AcademicYearId == academicYearId)
                        .Select(w => w.Standardsectionmappingid).FirstOrDefaultAsync();

                    var gradeid = await db3.MStandardsectionmappings
                        .Where(w => w.Id == sectionid)
                        .Select(w => w.Parentid).FirstOrDefaultAsync();

                    var leveid = db2.MStandardsectionmappings
                        .Where(w => w.Id == sectionid)
                        .Select(w => w.LevelID).FirstOrDefault();

                    stdRCM.isfrezeenabled = db2.MStandardyearmappings
                        .Where(w => w.AcademicYearId == academicYearId && w.LevelId == leveid && w.ExamId == testId && w.GradeId == gradeid)
                        .Select(w => w.FreezeEnable)
                        .FirstOrDefault();

                    var overall = await db2.MOverallchildtests
                        .Where(x => x.ChildId.Equals(childid) && x.TestId.Equals(testId) && x.AcademicYearId == academicYearId && x.SectionId == sectionid)
                        .FirstOrDefaultAsync();

                    var yearsemesterid = db2.MSemesteryearmappings
                        .Where(w => w.AcademicYearId == academicYearId && w.SemesterId == semesterId)
                        .Select(w => w.Id)
                        .FirstOrDefault();

                    var overallStudentPercentage = await this.GetOverallStudentPercentage(childid, testId, yearsemesterid);

                    if (overall != null)
                    {
                        stdRCM.OverallPosition = await this.GetOverallStudentPosition(testId, overall.OverallPercentage, (int)sectionid, academicYearId);
                        stdRCM.OverallPercentage = overallStudentPercentage;
                        stdRCM.OverallComments = overall.OverallComments;
                        stdRCM.OverallCommentsSectionalHead = overall.OverallCommentstwo;
                        stdRCM.OverallCommentsHeadmaster = overall.OverallCommenthree;
                    }
                    else
                    {
                        continue;
                    }

                    studentRCList.Add(stdRCM);
                }

                return studentRCList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return "An error occurred while fetching the student semester summary.";
            }
        }
        //Jaliya Academic Trends
        public async Task<List<StudentCountData>> GetNumberofstudentforsubjectChart(int levelId, int standardId, int subjectId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

            var studentsCount = new List<StudentCountData>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetNumberofstudentforsubjectChart, connection);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@LevelId", levelId);
                    command.Parameters.AddWithValue("@StandardId", standardId);
                    command.Parameters.AddWithValue("@SubjectId", subjectId);

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var studentCountData = new StudentCountData
                        {
                            Year = reader["Year"].ToString(),
                            SubjectName = reader["SubjectName"].ToString(),
                            NumberOfStudents = Convert.ToInt32(reader["NumberofStudent"]),
                        };

                        studentsCount.Add(studentCountData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return studentsCount;
        }
        public async Task<double?> GetOverallStudentPercentage(int childid, int testid, int semyearid)
        {
            try
            {
                var res = await Task.FromResult((from ctm in db.MChildtestmappings
                                                 join stm in db.MSubjecttestmappings on new { a = ctm.SubjectTestMappingId, a1 = semyearid } equals new { a = stm.Id, a1 = (int)stm.SemesterYearMappingId }
                                                 where stm.TestId == testid && ctm.ChildId == childid && ctm.Marks != null
                                                 select new { marks = ctm.Marks, maxmark = stm.MaxMarks }).ToArray());

                if (res.Count() > 0)
                {
                    var obmarks = res.Select(x => x.marks);
                    var maxmarks = res.Select(y => y.maxmark);

                    double resSum = ((double)obmarks.Sum() / (double)maxmarks.Sum()) * 100;

                    return Math.Round(resSum, 2);

                }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<object> GetAveragePerformances(int sectionid, int testid, int childid, int academicYear)
        {
            try
            {
                AveragePerformancesModel avgModel = new AveragePerformancesModel();
                var restemp = 0.0;
                var top = db.MOverallchildtests.Where(x => x.SectionId == sectionid && x.TestId == testid && x.AcademicYearId == academicYear).Max(w => w.OverallPercentage);
                var childidlist = db.MOverallchildtests.Where(x => x.SectionId == sectionid && x.TestId == testid && x.AcademicYearId == academicYear).Select(w => w.ChildId).ToArray();
                var semesterid = db2.MSemestertestsmappings.Where(w => w.Id == testid).Select(w => w.SemesterId).FirstOrDefault();
                var yearsemesterid = db2.MSemesteryearmappings.Where(w => w.AcademicYearId == academicYear && w.SemesterId == semesterid).Select(w => w.Id).FirstOrDefault();
                
                for (int i = 0; i < childidlist.Length; i++)

                {
                    var temp = await this.GetOverallStudentPercentage(childidlist[i], testid, yearsemesterid);
                    restemp = (double)(restemp + temp);

                }
                var res = db.MOverallchildtests.Where(x => x.SectionId == sectionid && x.TestId == testid && x.AcademicYearId == academicYear).Select(w => w.OverallPercentage).ToArray();
                var res1 = await Task.FromResult((from ctm in db.MChildtestmappings
                                                  join stm in db.MSubjecttestmappings on ctm.SubjectTestMappingId equals stm.Id
                                                  join sym in db.MSemesteryearmappings on stm.SemesterYearMappingId equals sym.Id
                                                  where stm.TestId == testid && ctm.ChildId == childid && sym.AcademicYearId == academicYear
                                                  select new { marks = ctm.Marks, maxmark = stm.MaxMarks }).ToArray());

                var count = res.Count();

                if (top != null && count > 0 && res1.Count() > 0)
                {
                    var obmarks = res1.Sum(x => x.marks);
                    var maxmarks = res1.Sum(y => y.maxmark);
                    double classavg = (double)restemp / count;
                   // double classavg = (((double)obmarks / (double)maxmarks) * 100);
                   

                    avgModel.obtainedMarks = obmarks;
                    avgModel.maxMarks = maxmarks;
                    avgModel.ClassAverage = Math.Round(classavg, 1);
                    avgModel.TopStudent = top;
                }
                return avgModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public double? calculatePercentage(double count, double size)
        {
            if (count != 0 && size != 0)
            {
                double per = count / size * 100;
                return Math.Round(per, 2);
            }
            return 0.0;
        }

        public List<GetSubjectsSectionModel> GetSubSecInfo(int sectionid, int subjectid, int semesterid)
        {
            try
            {
                List<GetSubjectsSectionModel> gssml = new List<GetSubjectsSectionModel>();

                var ssm = db.MSubjectsectionmappings.Where(x => x.SubjectId.Equals(subjectid) && x.SectionId.Equals(sectionid)).FirstOrDefault();
                if (ssm != null)
                {
                    var stm = db.MSubjecttestmappings.Where(w => w.SubjectSectionMappingId.Equals(ssm.Id));
                    var subject = db.MSubjects.Where(s => s.Id.Equals(subjectid)).Select(w => w.Name).FirstOrDefault();
                    foreach (var stms in stm)
                    {
                        var tests = db1.MSemestertestsmappings.Where(x => x.SemesterId == semesterid);
                        foreach (var testid in tests)
                        {
                            var ctm = db2.MChildtestmappings.Where(c => c.SubjectTestMappingId == stms.Id && c.SubjectTestMapping.TestId == testid.Id);
                            if (ctm != null)
                            {
                                foreach (var item in ctm)
                                {
                                    GetSubjectsSectionModel gssm = new GetSubjectsSectionModel();
                                    gssm.ChildId = item.ChildId;
                                    gssm.Percentage = item.Percentage;
                                    gssm.Subject = subject;
                                    gssml.Add(gssm);
                                }
                            }
                        }
                    }

                }
                return gssml;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<double?> GetSubjectSemesterPercentage(int semesterid, int childid, int subjectid)
        {
            List<double?> allsemper = new List<double?>();
            int i = 0;
            var testids = db.MSemestertestsmappings.Where(x => x.SemesterId == semesterid).Select(x => x.Id);
            foreach (int test in testids)
            {
                var res = await this.GetOverallSubjectPercentage(childid, test, subjectid);
                if (res > 0)
                {
                    allsemper.Add(res);
                }
            }

            var count = allsemper.Count();
            if (count > 0)
            {
                var finalres = allsemper.Sum() / count;
                return finalres;
            }
            else
            { return 0; }

        }

        public async Task<double?> GetOverallSubjectPercentage(int childid, int testid, int subjectid)
        {
            try
            {
                var res = await Task.FromResult((from ctm in db1.MChildtestmappings
                                                 join stm in db1.MSubjecttestmappings on ctm.SubjectTestMappingId equals stm.Id
                                                 join ssm in db1.MSubjectsectionmappings on stm.SubjectSectionMappingId equals ssm.Id
                                                 where stm.TestId == testid && ctm.ChildId == childid && ssm.SubjectId == subjectid
                                                 select ctm.Percentage).ToArray());

                var count = res.Count();
                if (count > 0)
                {
                    double resSum = (double)res.Sum() / count;
                    return Math.Round(resSum, 2);
                }
                return 0;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> GetSemesterPositionVsSection(int semesterid, int subsecid, double percentage)
        {
            try
            {
                double?[] res = await Task.FromResult(db.MSubjectsemesterpercentages.Where(x => x.SemesterId == semesterid && x.SubjectSectionMappingId == subsecid).OrderByDescending(o => o.SubjectSemesterPercentage)
                    .Select(a => a.SubjectSemesterPercentage).ToArray());
                if (res != null)
                {
                    int totalStudents = res.Count();
                    var position = Array.FindIndex(res, x => x.Equals(percentage)) + 1;
                    if (position > 0)
                    {
                        return (position + "/" + totalStudents);
                    }
                    return ("N/A");
                }
                return ("N/A");
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<string> GetSemesterPositionVsClass(int semesterid, int standardid, int subjectid, double percentage)
        {
            try
            {
                double?[] res = await Task.FromResult((from ssp in db.MSubjectsemesterpercentages
                                                       join ssm in db.MSubjectsectionmappings on ssp.SubjectSectionMappingId equals ssm.Id
                                                       where ssp.SemesterId == semesterid && ssm.SubjectId == subjectid && ssp.StandardId == standardid
                                                       orderby ssp.SubjectSemesterPercentage descending
                                                       select ssp.SubjectSemesterPercentage).ToArray());
                if (res != null)
                {
                    int totalStudents = res.Count();
                    var position = Array.FindIndex(res, x => x.Equals(percentage)) + 1;
                    if (position > 0)
                    {
                        return (position + "/" + totalStudents);
                    }
                    return ("N/A");
                }
                return ("N/A");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> GetStudentsRc(int childid, int testid, int schoolid, int academicYearId)
        {
            try
            {
                List<studentRCModel> studentRCList = new List<studentRCModel>();
               
                //join subsubj in db.MSubSubjects on sub.Id equals subsubj.SubjectId
                //&& sub.SubSubjectionId == subsubj.Id
                var stdRC = await Task.FromResult((from ctm in db.MChildtestmappings
                                                   join sttm in db.MSemestertestsmappings on testid equals sttm.Id
                                                   join sym in db.MSemesteryearmappings on academicYearId equals sym.AcademicYearId
                                                   join stm in db.MSubjecttestmappings on sym.Id equals stm.SemesterYearMappingId
                                                   join ssm in db.MSubjectsectionmappings on stm.SubjectSectionMappingId equals ssm.Id
                                                   join sub in db.MSubjects on ssm.SubjectId equals sub.Id
                                                   where ctm.ChildId == childid && ctm.SubjectTestMappingId == stm.Id && sttm.Id == stm.TestId
                                                   orderby sub.DRCSubjectOrder
                                                   select new { ChildTestId = ctm.Id, SubjectID = sub.Id, childtestmappingid = ctm.Id, Subject = sub.Name, ctm.Marks, stm.MaxMarks, ctm.Percentage, ctm.Comments, ctm.IsAbsent }).ToList());
                studentRCModel stdRCM = new studentRCModel();
                
                int x = 0;
                if (stdRC.Count() > 0)
                {
                    foreach (var item in stdRC)
                    {
                        studentRCInnerModel src = new studentRCInnerModel();
                        src.ChildTestMappingId = item.ChildTestId;
                        src.Subject = item.Subject;
                        src.MaxMarks = item.MaxMarks;
                        //src.Percentage = item.Percentage;
                        src.Comments = item.Comments;
                        //src.IsAbsent = item.IsAbsent;
                        src.SubjectTestMappingId = db1.MChildtestmappings.Where(x => x.Id.Equals(item.childtestmappingid)).Select(w => w.SubjectTestMappingId).FirstOrDefault();
                        if (item.Marks == null || item.IsAbsent == true)
                        {
                            src.Position = "N/A";
                        }
                        else
                        {
                            src.Position = GetStudentPosition(src.SubjectTestMappingId, item.Marks);
                        }
                        var subsubjects2 = db1.MSubSubjects.Where(x => x.SubjectId == item.SubjectID).ToList();
                        var arraysize = subsubjects2.Count();
                        var subabsentcount = 0;
                        int i = 0;
                        src.SubSubjectsIds = new int?[arraysize];
                        src.SubSubjects = new string[arraysize];
                        src.SubSubjectsMarks = new int?[arraysize];
                        src.SubSubjectWeight = new int?[arraysize];
                        //src.SubSubjectComment = new string[5];
                        src.SubSubjectMaxMarks = new int?[arraysize];
                        src.SubSubjectAbsent = new int?[arraysize];
                        var secid = await db3.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == academicYearId).Select(w => w.Standardsectionmappingid).FirstOrDefaultAsync();
                        var gradeid = await db3.MStandardsectionmappings.Where(w => w.Id == secid).Select(w => w.Parentid).FirstOrDefaultAsync();
                      
                        //freezeenabled
                        var leveid = db2.MStandardsectionmappings.Where(w => w.Id == secid).Select(w => w.LevelID).FirstOrDefault();
                        stdRCM.isfrezeenabled = db2.MStandardyearmappings.Where(w => w.AcademicYearId == academicYearId && w.LevelId == leveid && w.ExamId == testid && w.GradeId == gradeid).Select(w => w.FreezeEnable).FirstOrDefault();

                        src.GradeStudentsSubjectMarksAverage = GetStudentSubjectGradeAverage(item.SubjectID, gradeid, testid);
                        x++;
                        foreach (var sub3 in subsubjects2)
                        {
                            src.SubSubjectsIds[i] = await db3.MSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.Id).FirstOrDefaultAsync();
                            src.SubSubjects[i] = await db3.MSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.SubSubject).FirstOrDefaultAsync();
                            src.SubSubjectsMarks[i] = await db3.MSubsubjectmarkss.Where(x => x.SubSubjectId.Equals(sub3.Id) && x.ChildTestMappingId == item.childtestmappingid).Select(w => w.Marks).FirstOrDefaultAsync();
                            src.SubSubjectWeight[i] = await db3.MSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.Precentage).FirstOrDefaultAsync();
                            src.SubSubjectMaxMarks[i] = await db3.MSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.SubMaxMarks).FirstOrDefaultAsync();
                            src.SubSubjectAbsent[i] = await db3.MSubsubjectmarkss.Where(x => x.SubSubjectId.Equals(sub3.Id) && x.ChildTestMappingId == item.childtestmappingid).Select(w => w.Absent).FirstOrDefaultAsync();
                            if (src.SubSubjectAbsent[i] == 1)
                            {
                                subabsentcount = subabsentcount + 1;
                            }
                            i++;

                        }
                        if(subabsentcount == arraysize)
                        {
                            MChildtestmapping ctm = new MChildtestmapping();
                            ctm.IsAbsent = true;
                            src.IsAbsent = true;
                            src.Position = "N/A";
                            var existingEntity = db.MChildtestmappings.FirstOrDefault(e => e.Id == src.ChildTestMappingId);
                            //var temp = db.MChildtestmappings.Find(src.ChildTestMappingId);
                            if (existingEntity != null)
                            {
                                existingEntity.IsAbsent = ctm.IsAbsent;
                                //db.MChildtestmappings.Update(ctm);
                                var dbup10 = db.SaveChanges();
                            }
                        }
                        stdRCM.studentRCInnerModels.Add(src);
                    }
                    var drcmodel = db2.MSchools.Where(w => w.Id == schoolid).Select(w => w.drcmodel).FirstOrDefault();
                    var pdfmodel = db2.MSchools.Where(w => w.Id == schoolid).Select(w => w.pdfmodel).FirstOrDefault();
                    if (pdfmodel != null && drcmodel != null)
                    {
                        stdRCM.pdfmodel = (int)pdfmodel;
                        stdRCM.drcmodel = (int)drcmodel;
                    }
                    else
                    {
                        stdRCM.pdfmodel = 1;
                        stdRCM.drcmodel = 1;
                    }
                        
                    var sectionid = db2.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == academicYearId).Select(w => w.Standardsectionmappingid).FirstOrDefault();
                    var overall = await db2.MOverallchildtests.Where(x => x.ChildId.Equals(childid) && x.TestId.Equals(testid) && x.AcademicYearId == academicYearId && x.SectionId == sectionid).FirstOrDefaultAsync();
                    var semesterid = db2.MSemestertestsmappings.Where(w => w.Id == testid).Select(w => w.SemesterId).FirstOrDefault();
                    var yearsemesterid = db2.MSemesteryearmappings.Where(w => w.AcademicYearId == academicYearId && w.SemesterId == semesterid).Select(w => w.Id).FirstOrDefault(); 
                    var OverallStudentPercentage = await this.GetOverallStudentPercentage(childid, testid, yearsemesterid);
               
                    if (overall != null)
                    {
                        stdRCM.OverallPosition = await this.GetOverallStudentPosition(testid, overall.OverallPercentage, (int)sectionid, academicYearId);
                        stdRCM.OverallPercentage = OverallStudentPercentage;
                       // stdRCM.OverallPercentage = overall.OverallPercentage;
                        stdRCM.OverallComments = overall.OverallComments;
                        stdRCM.OverallCommentsSectionalHead = overall.OverallCommentstwo;
                        stdRCM.OverallCommentsHeadmaster = overall.OverallCommenthree;

                    }
                    else
                    {
                        return ("Overall Data Not Mapped");
                    }
                    studentRCList.Add(stdRCM);
                    return (studentRCList);
                }
                else
                {
                    var secid = await db3.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == academicYearId).Select(w => w.Standardsectionmappingid).FirstOrDefaultAsync();
                    var branchid = db3.MBranches.Where(x => x.Schoolid == schoolid).Select(w => w.Id).FirstOrDefault();
                    var subjects = db3.MSubjects.Where(x => x.BranchId == branchid);

                    foreach (var sub in subjects)
                    {
                        //var semid = db.MSemestertestsmappings.Where(x => x.Id == testid).Select(a => a.SemesterId).FirstOrDefault();
                        //var semyearid = db.MSemesteryearmappings.Where(y => y.SemesterId == semid && y.AcademicYearId == academicYearId).Select(b => b.Id).FirstOrDefault();
                        var subtestid = await db4.MSubjecttestmappings.Where(x => x.SubjectSectionMapping.SubjectId == sub.Id && x.SubjectSectionMapping.SectionId == secid && x.TestId == testid).FirstOrDefaultAsync();
                        if (subtestid != null)
                        {
                            studentRCInnerModel src = new studentRCInnerModel();
                            src.Subject = sub.Name;
                            src.SubjectTestMappingId = subtestid.Id;
                            //src.MaxMarks = subtestid.MaxMarks;
                            int i = 0;

                            //var subsubjects = db3.MSubSubjects.Where(x => x.SubjectId == sub.Id);


                            var subsubjects2 = db4.MSubSubjects.Where(x => x.SubjectId == sub.Id).ToList();
                            var arraysize = subsubjects2.Count();
                            src.SubSubjectsIds = new int?[arraysize];
                            src.SubSubjects = new string[arraysize];
                            src.SubSubjectsMarks = new int?[arraysize];
                            src.SubSubjectWeight = new int?[arraysize];
                            //src.SubSubjectComment = new string[5];
                            src.SubSubjectMaxMarks = new int?[arraysize];
                            src.SubSubjectAbsent = new int?[arraysize];
                            var gradeid = await db4.MStandardsectionmappings.Where(w => w.Id == secid).Select(w => w.Parentid).FirstOrDefaultAsync();
                            src.GradeStudentsSubjectMarksAverage = GetStudentSubjectGradeAverage(sub.Id, gradeid, testid);
                            x++;
                            foreach (var sub3 in subsubjects2)
                            {
                                src.SubSubjectsIds[i] = await db4.MSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.Id).FirstOrDefaultAsync();
                                src.SubSubjects[i] = await db4.MSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.SubSubject).FirstOrDefaultAsync();
                                src.SubSubjectsMarks[i] = 0;
                                src.SubSubjectWeight[i] = await db4.MSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.Precentage).FirstOrDefaultAsync();
                                //src.SubSubjectComment[i] = null;
                                src.SubSubjectMaxMarks[i] = await db4.MSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.SubMaxMarks).FirstOrDefaultAsync();
                                src.SubSubjectAbsent[i] = 0;

                                i++;

                            }
                            stdRCM.studentRCInnerModels.Add(src);
                        }

                    }
                    studentRCList.Add(stdRCM);
                    return (studentRCList);
                }
                return ("Student test mapping not exist");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Jaliya SubSubject Marks
        public async Task<List<SubSubjectMarks>> GetHiEduSubSubjectMarksForStudent(int childId, int subjectId)
        {
            try
            {
                var subSubjectMarks = await Task.Run(() => (from ssm in db.HiEdu_SubSubjectMarkss
                                                            join cem in db.MHiEdu_CourseExamMarkss on ssm.CourseExamMarksId equals cem.Id
                                                            join sem in db.MHieduSubjectExamMappings on cem.SubjectExamMappingId equals sem.Id
                                                            join sub in db.MHiEduSubSubjects on ssm.SubSubjectId equals sub.Id
                                                            where cem.ChildId == childId && sem.Id == subjectId
                                                            select new SubSubjectMarks
                                                            {
                                                                SubSubjectName = sub.SubSubjectName,
                                                                Marks = ssm.SubSubjectMarks
                                                            }).ToList());

                return subSubjectMarks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Jalya Student Chart Grading
        public async Task<List<HiEduMarksGroup>> GetHieduModuleStudentPerformanceScoreChart(int batchId, int semesterId, int examId, int subjectId)
        {
            try
            {
                var ranges = new[] { "0-10", "10-20", "20-30", "30-40", "40-50", "50-60", "60-70", "70-80", "80-90", "90-100" };

                var result = new List<HiEduMarksGroup>();

                foreach (var range in ranges)
                {
                    var lowerBound = int.Parse(range.Split('-')[0]);
                    var upperBound = int.Parse(range.Split('-')[1]);

                    var studentsInRange = await (from cem in db.MHiEdu_CourseExamMarkss
                                                 join sem in db.MHieduSubjectExamMappings on cem.SubjectExamMappingId equals sem.Id
                                                 join cse in db.MHiEduCourseSemesterExams on sem.CourseSemesterExamId equals cse.Id
                                                 join scm in db.HiEdu_SemesterCourseMappings on cse.SemesterCourseMappingId equals scm.Id
                                                 join b in db.MHiEdubatchs on scm.CourseId equals b.CourseId
                                                 where b.Id == batchId && sem.Id == subjectId && cse.Id == examId && scm.Id == semesterId
                                                       && cem.Marks >= lowerBound && (cem.Marks < upperBound || (upperBound == 100 && cem.Marks <= upperBound))
                                                 select cem).CountAsync();

                    result.Add(new HiEduMarksGroup { Range = range, StudentCount = studentsInRange });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Sanduni Get Hi Edu Student RC
        public async Task<object> GetHiEduStudentsRC(int courseid, int batchid, int childid, int schoolid, int semeseterid, int examid)
        {

            //            select m_HiEdu_SubjectExamMapping.Subject,m_HiEdu_SubSubject.SubSubjectName,
            //m_HiEdu_SubSubjectMarks.SubSubjectMarks
            //from m_HiEdu_CourseExamMarks,
            //m_HiEdu_CourseSemesterExam,
            //m_HiEdu_SemesterCourseMapping,m_HiEdu_SubjectExamMapping,[dbo].[m_HiEdu_SubSubjectMarks]
            //,[dbo].[m_HiEdu_SubSubject]
            //where m_HiEdu_CourseExamMarks.ChildId = 6455
            //and m_HiEdu_CourseSemesterExam.Id = 1  and m_HiEdu_SemesterCourseMapping.CourseId = 1
            //and m_HiEdu_SemesterCourseMapping.Id = 1
            //and m_HiEdu_SemesterCourseMapping.Id = m_HiEdu_CourseSemesterExam.SemesterCourseMappingId
            //and m_HiEdu_CourseSemesterExam.Id = m_HiEdu_SubjectExamMapping.CourseSemesterExamId
            //and m_HiEdu_CourseExamMarks.SubjectExamMappingId = m_HiEdu_SubjectExamMapping.Id
            //and m_HiEdu_SubSubject.Id = m_HiEdu_SubSubjectMarks.SubSubjectId
            //and m_HiEdu_SubSubjectMarks.CourseExamMarksId = m_HiEdu_CourseExamMarks.Id

            try
            {
                List<HiEduStudentRCModel> studentRCList = new List<HiEduStudentRCModel>();
                var stdRC = await Task.FromResult((from cem in db.MHiEdu_CourseExamMarkss
                                                   join cse in db.MHiEduCourseSemesterExams on cem.ChildId equals childid
                                                   join scm in db.HiEdu_SemesterCourseMappings on cse.SemesterCourseMappingId equals scm.Id
                                                   join sem in db.MHieduSubjectExamMappings on cse.Id equals sem.CourseSemesterExamId
                                                   where
                                                   cse.Id == examid && scm.CourseId == courseid && scm.Id == semeseterid
                                                   && cem.SubjectExamMappingId == sem.Id
                                                   select new { ChildCourseExamMarksId = cem.Id, SubjectID = sem.Id, Subject = sem.Subject, cem.Marks }).ToList());

                HiEduStudentRCModel stdRCM = new HiEduStudentRCModel();

                int x = 0;
                if (stdRC.Count() > 0)
                {
                    foreach (var item in stdRC)
                    {
                        HiEduStudentRCInnerModel src = new HiEduStudentRCInnerModel();
                        src.Subject = item.Subject;
                        src.Marks = item.Marks;
                        src.SubjectId = item.SubjectID;
                        src.ChildCourseExamMarksId = item.ChildCourseExamMarksId;
                        //src.IsAbsent = item.IsAbsent;
                        src.SubjectExamMappingId = db1.MHiEdu_CourseExamMarkss.Where(x => x.Id.Equals(item.ChildCourseExamMarksId)).Select(w => w.SubjectExamMappingId).FirstOrDefault();
                        if (item.Marks == null)
                        {
                            src.Position = "N/A";
                        }
                        else
                        {
                            src.Position = GetHiEduStudentPosition(src.SubjectExamMappingId, item.Marks);
                        }
                        var subsubjects2 = db1.MHiEduSubSubjects.Where(x => x.SubjectId == item.SubjectID).ToList();
                        var arraysize = subsubjects2.Count();
                        int i = 0;
                        src.SubSubjectsIds = new int?[arraysize];
                        src.SubSubjects = new string[arraysize];
                        src.SubSubjectsMarks = new int?[arraysize];
                        src.SubSubjectWeight = new int?[arraysize];
                        src.SubSubjectComment = new string[5];
                        src.SubSubjectMaxMarks = new int?[arraysize];
                        src.SubSubjectAbsent = new int?[arraysize];
                        // var secid = await db3.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == academicYearId).Select(w => w.Standardsectionmappingid).FirstOrDefaultAsync();
                        // var gradeid = await db3.MStandardsectionmappings.Where(w => w.Id == secid).Select(w => w.Parentid).FirstOrDefaultAsync();
                        //  src.GradeStudentsSubjectMarksAverage = GetStudentSubjectGradeAverage(item.SubjectID, gradeid, testid);
                        x++;
                        foreach (var sub3 in subsubjects2)
                        {
                            src.SubSubjectsIds[i] = await db3.MHiEduSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.Id).FirstOrDefaultAsync();
                            src.SubSubjects[i] = await db3.MHiEduSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.SubSubjectName).FirstOrDefaultAsync();
                            src.SubSubjectsMarks[i] = await db3.HiEdu_SubSubjectMarkss.Where(x => x.SubSubjectId.Equals(sub3.Id) && x.CourseExamMarksId == item.ChildCourseExamMarksId).Select(w => w.SubSubjectMarks).FirstOrDefaultAsync();
                            src.SubSubjectWeight[i] = await db3.MHiEduSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.Weight).FirstOrDefaultAsync();
                            src.SubSubjectMaxMarks[i] = await db3.MHiEduSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.Maxmarks).FirstOrDefaultAsync();
                            src.SubSubjectAbsent[i] = await db3.HiEdu_SubSubjectMarkss.Where(x => x.SubSubjectId.Equals(sub3.Id) && x.CourseExamMarksId == item.ChildCourseExamMarksId).Select(w => w.IsAbsent).FirstOrDefaultAsync();

                            src.SubSubjectComment[i] = await db3.HiEdu_SubSubjectMarkss.Where(x => x.SubSubjectId.Equals(sub3.Id) && x.CourseExamMarksId == item.ChildCourseExamMarksId).Select(w => w.Comment).FirstOrDefaultAsync();
                            i++;

                        }
                        stdRCM.HiEduStudentRCInnerModels.Add(src);
                    }
                    // var sectionid = db2.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == academicYearId).Select(w => w.Standardsectionmappingid).FirstOrDefault();
                    int batchcoursemappingid = db4.MHiEdubatchs.Where(x => x.CourseId == courseid && x.Id == batchid).OrderByDescending(w => w.Id).Select(w => w.Id).FirstOrDefault();
                    var overall = db2.HiEdu_OverallChildTestss.Where(x => x.ChildId.Equals(childid) && x.BatchCourseMappingId.Equals(batchcoursemappingid)).FirstOrDefault();
                    if (overall != null)
                    {
                        stdRCM.OverallPosition = await this.HiEduGetOverallBatchStudentPosition(examid, batchid, courseid, childid);
                        stdRCM.OverallPercentage = overall.OverallSemesterExamPrecentage;
                        stdRCM.OverallComments = overall.OverallComment;
                    }
                    else
                    {
                        return ("Overall Data Not Mapped");
                    }

                    studentRCList.Add(stdRCM);
                    return (studentRCList);
                }
                else
                {
                    int batchcoursemappingid = db4.MHiEdubatchs.Where(x => x.CourseId == courseid && x.Id == batchid).OrderByDescending(w => w.Id).Select(w => w.Id).FirstOrDefault();

                    var modules = db4.MHieduSubjectExamMappings.Where(x => x.CourseSemesterExamId == examid).OrderByDescending(w => w.Id);

                    //  var subjects = db3.MSubjects.Where(x => x.Branch.Schoolid == schoolid);

                    foreach (var sub in modules)
                    {
                        var subjectid = await db5.MHieduSubjectExamMappings.Where(x => x.Id == sub.Id).FirstOrDefaultAsync();
                        if (subjectid != null)
                        {
                            HiEduStudentRCInnerModel src = new HiEduStudentRCInnerModel();
                            src.Subject = sub.Subject;
                            src.SubjectId = sub.Id;
                            //src.MaxMarks = subtestid.MaxMarks;
                            int i = 0;

                            //var subsubjects = db3.MSubSubjects.Where(x => x.SubjectId == sub.Id);


                            var subsubjects2 = db5.MHiEduSubSubjects.Where(x => x.SubjectId == sub.Id).ToList();
                            var arraysize = subsubjects2.Count();
                            src.SubSubjectsIds = new int?[arraysize];
                            src.SubSubjects = new string[arraysize];
                            src.SubSubjectsMarks = new int?[arraysize];
                            src.SubSubjectWeight = new int?[arraysize];
                            src.SubSubjectMaxMarks = new int?[arraysize];
                            src.SubSubjectComment = new string[arraysize];
                            src.SubSubjectAbsent = new int?[arraysize];
                            // var gradeid = await db3.MStandardsectionmappings.Where(w => w.Id == secid).Select(w => w.Parentid).FirstOrDefaultAsync();
                            // src.GradeStudentsSubjectMarksAverage = GetStudentSubjectGradeAverage(sub.Id, gradeid, testid);
                            x++;
                            foreach (var sub3 in subsubjects2)
                            {
                                src.SubSubjectsIds[i] = await db5.MHiEduSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.Id).FirstOrDefaultAsync();
                                src.SubSubjects[i] = await db5.MHiEduSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.SubSubjectName).FirstOrDefaultAsync();
                                src.SubSubjectsMarks[i] = 0;
                                src.SubSubjectWeight[i] = await db5.MHiEduSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.Weight).FirstOrDefaultAsync();
                                src.SubSubjectComment[i] = null;
                                src.SubSubjectMaxMarks[i] = await db5.MHiEduSubSubjects.Where(x => x.Id.Equals(sub3.Id)).Select(w => w.Maxmarks).FirstOrDefaultAsync();
                                src.SubSubjectAbsent[i] = 0;

                                i++;

                            }
                            stdRCM.HiEduStudentRCInnerModels.Add(src);
                        }

                    }
                    studentRCList.Add(stdRCM);
                    return (studentRCList);
                }
                return ("Student test mapping not exist");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Sanduni Update Hi Edu Student RC
        public async Task<object> HiEduUpdateStudentsRc(postHiEduStudentRCModel model)
        {
            try
            {
                var batchcoursemappingid = db4.MHiEdubatchs.Where(x => x.CourseId == model.courseid && x.Id == model.batchid).OrderByDescending(w => w.Id).Select(w => w.Id).FirstOrDefault();

                if (model.studentsHiEduRCModels.Count() == 0 && !string.IsNullOrEmpty(model.OverallComments))
                {
                    var temp2 = await db.HiEdu_OverallChildTestss.Where(s => s.ChildId == model.childid && s.BatchCourseMappingId == batchcoursemappingid).FirstOrDefaultAsync();
                    if (temp2 != null)
                    {
                        if (!string.IsNullOrEmpty(model.OverallComments))
                            temp2.OverallComment = model.OverallComments;
                        db.SaveChanges();
                        return (new { Value = "Updated Succesfully" });
                    }
                    return (new { Value = "Overall details not found" });

                }
                else
                {
                    foreach (var item in model.studentsHiEduRCModels)
                    {
                       // var batchcoursemappingid = db4.MHiEdu_CourseExamMarkss.Where(x => x.SubjectExamMappingId == model.courseid && x. == model.batchid).OrderByDescending(w => w.Id).Select(w => w.Id).FirstOrDefault();

                        var temp = db.MHiEdu_CourseExamMarkss.Find(item.ChildExamSubjectMarksId);
                        if (temp == null)
                        {
                            MHiEdu_CourseExamMarks ctm = new MHiEdu_CourseExamMarks();
                            //ctm.Id = Guid.NewGuid();
                            ctm.ChildId = (int)model.childid;
                            ctm.Marks = item.Marks;
                            ctm.SubjectExamMappingId = item.SubjectID;
                            //ctm.Percentage = item.Percentage;
                           // ctm.Createddate = DateTime.Today;
                           // ctm.Modifieddate = DateTime.Today;
                           // ctm.Modifiedby = model.userid;
                           // ctm.Statusid = 1;
                            //ctm.IsAbsent = item.IsAbsent;
                            db.MHiEdu_CourseExamMarkss.Add(ctm);
                            var dbup1 = db.SaveChanges();
                            var arraysize = item.SubSubjectsMarks.Count();
                            
                            for (int i = 0; i < arraysize; i++)
                            {
                                HiEdu_SubSubjectMarks ssm = new HiEdu_SubSubjectMarks();
                                var subsubjectmarks = item.SubSubjectsMarks[i];
                                var subsubjectIds = item.SubSubjectsIds[i];
                                var Comments = item.SubSubjectComment[i];
                                var SubAbsent = item.SubAbsent[i];
                                ssm.SubSubjectId = subsubjectIds;
                                ssm.SubSubjectMarks = subsubjectmarks;
                                ssm.Comment = Comments.ToString();
                                ssm.IsAbsent = SubAbsent;
                                ssm.CourseExamMarksId = db.MHiEdu_CourseExamMarkss.Where(x => x.ChildId == model.childid && x.SubjectExamMappingId == item.SubjectID).Select(a => a.Id).FirstOrDefault();
                                db.HiEdu_SubSubjectMarkss.Add(ssm);
                                db.SaveChanges();
                            }

                           
                                var temp2 = await db.HiEdu_OverallChildTestss.Where(s => s.ChildId == model.childid && s.BatchCourseMappingId == batchcoursemappingid).FirstOrDefaultAsync();
                                if (temp2 != null)
                            {
                                if (!string.IsNullOrEmpty(model.OverallComments))
                                    temp2.OverallComment = model.OverallComments;
                                db.SaveChanges();
                             
                            }
                            else //if record doesn't exist we add it for the first time
                            {
                                HiEdu_OverallChildTests oct = new HiEdu_OverallChildTests();
                                oct.ChildId = (int)model.childid;
                                oct.BatchCourseMappingId = (int)model.batchid;
                                oct.OverallBatchPostition = 0;
                                oct.OverallComment = "";
                                oct.OverallSemesterExamPrecentage = 0;
                                db.HiEdu_OverallChildTestss.Add(oct);
                                db.SaveChanges();
                            }

                          
                           // item.SubjectSemesterPercentage = await this.GetSubjectSemesterPercentage((int)semtestid, (int)model.childid, subid);

                            //var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                            //using (SqlConnection connection = new SqlConnection(connectionString))
                            //{
                            //    connection.Open();
                            //    SqlCommand command = new SqlCommand(ApplicationConstants.UpdateReportCard, connection);
                            //    command.CommandType = CommandType.StoredProcedure;
                            //    command.Parameters.Add(new SqlParameter("@ChildId", SqlDbType.Int));
                            //    command.Parameters["@ChildId"].Value = model.childid;
                            //    command.Parameters.Add(new SqlParameter("@SubjectSectionMappingId", SqlDbType.Int));
                            //    command.Parameters["@SubjectSectionMappingId"].Value = subsecid;
                            //    command.Parameters.Add(new SqlParameter("@StandardId", SqlDbType.Int));
                            //    command.Parameters["@StandardId"].Value = stdid;
                            //    command.Parameters.Add(new SqlParameter("@SemesterId", SqlDbType.Int));
                            //    command.Parameters["@SemesterId"].Value = semtestid;
                            //    command.Parameters.Add(new SqlParameter("@SubjectSemesterPercentage", SqlDbType.Float));
                            //    command.Parameters["@SubjectSemesterPercentage"].Value = item.SubjectSemesterPercentage;
                            //    command.Parameters.Add(new SqlParameter("@Createdby", SqlDbType.Int));
                            //    command.Parameters["@Createdby"].Value = model.userid;
                            //    command.Parameters.Add(new SqlParameter("@Modifiedby", SqlDbType.Int));
                            //    command.Parameters["@Modifiedby"].Value = model.userid;
                            //    command.ExecuteNonQuery();
                            //    //}
                            //    db.SaveChanges();

                            //}
                        }
                        else
                        {
                            var connectionString3 = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                            using (SqlConnection connection3 = new SqlConnection(connectionString3))
                            {
                                connection3.Open();
                                SqlCommand command3 = new SqlCommand(ApplicationConstants.UpdateHiEduSubjectMarksSp, connection3);
                                command3.CommandType = CommandType.StoredProcedure;
                                command3.Parameters.Add(new SqlParameter("@ChildExamSubjectMarksId", SqlDbType.Int));
                                command3.Parameters["@ChildExamSubjectMarksId"].Value = temp.Id;
                                command3.Parameters.Add(new SqlParameter("@Marks", SqlDbType.Int));
                                command3.Parameters["@Marks"].Value = item.Marks;
                                command3.Parameters.Add(new SqlParameter("@Childid", SqlDbType.Int));
                                command3.Parameters["@Childid"].Value = model.childid;
                                command3.Parameters.Add(new SqlParameter("@SubjectID", SqlDbType.NVarChar));
                                command3.Parameters["@SubjectID"].Value = item.SubjectID;
                                command3.ExecuteNonQuery();
                                db.SaveChanges();
                            }
                            //  var temp3 = db.MSubjecttestmappings.Where(w => w.Id == temp.SubjectTestMappingId).FirstOrDefault();
                            //  var temp4 = db.MChildtestmappings.Where(x => x.SubjectTestMappingId == temp.SubjectTestMappingId).Select(a => a.Marks).ToList();


                            //if (item.Marks.has)
                            //{

                            //    temp.Marks = item.Marks;

                            //}
                            //else
                            //{
                            //    temp.Marks = item.Marks;
                            //}
                            //if (item.Percentage.HasValue)
                            //{
                            //    if (item.Percentage <= 100)
                            //    {
                            //        temp.per = item.Percentage;
                            //    }
                            //    else
                            //    {
                            //        return (new { Value = "Percentage has to be less than or equal 100" });
                            //    }
                            //}
                            //else
                            //{
                            //    temp.Percentage = item.Percentage;
                            //}
                            //if (!string.IsNullOrEmpty(item.Comments))
                            //    temp.Comments = item.Comments;

                            //temp.Modifiedby = model.userid;

                            //var dbup = db.SaveChanges();
                            //update existing sub subject marks
                            var arraysize = item.SubSubjectsMarks.Count();
                            for (int i = 0; i < arraysize; i++)
                            {
                                HiEdu_SubSubjectMarks ssm = new HiEdu_SubSubjectMarks();
                                var subsubjectmarks = item.SubSubjectsMarks[i];
                                var subsubjectIds = item.SubSubjectsIds[i];
                                var Comments = item.SubSubjectComment[i];
                                var SubAbsent = item.SubAbsent[i];
                                ssm.SubSubjectId = subsubjectIds;
                                ssm.SubSubjectMarks = subsubjectmarks;
                                ssm.IsAbsent = SubAbsent;
                                ssm.Comment = Comments.ToString();
                                ssm.CourseExamMarksId = item.ChildExamSubjectMarksId;

                                var connectionString2 = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                                using (SqlConnection connection = new SqlConnection(connectionString2))
                                {
                                    connection.Open();
                                    SqlCommand command = new SqlCommand(ApplicationConstants.UpdateHiEduSubSubjectMarksSp, connection);
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.Add(new SqlParameter("@CourseExamMarksId", SqlDbType.Int));
                                    command.Parameters["@CourseExamMarksId"].Value = ssm.CourseExamMarksId;
                                    command.Parameters.Add(new SqlParameter("@SubSubjectId", SqlDbType.Int));
                                    command.Parameters["@SubSubjectId"].Value = ssm.SubSubjectId;
                                    command.Parameters.Add(new SqlParameter("@Marks", SqlDbType.Int));
                                    command.Parameters["@Marks"].Value = ssm.SubSubjectMarks;
                                    command.Parameters.Add(new SqlParameter("@IsAbsent", SqlDbType.Int));
                                    command.Parameters["@IsAbsent"].Value = ssm.IsAbsent;
                                    command.Parameters.Add(new SqlParameter("@Comment", SqlDbType.NVarChar));
                                    command.Parameters["@Comment"].Value = ssm.Comment;
                                    command.ExecuteNonQuery();
                                    db.SaveChanges();
                                }

                            }


                            //if (dbup > 0) // checks if records are updated
                            //{
                            var temp3 = await db.HiEdu_OverallChildTestss.Where(s => s.ChildId == model.childid && s.BatchCourseMappingId == batchcoursemappingid).FirstOrDefaultAsync();
                            // for updating overallchildtest table
                           
                           
                            if (temp3 != null)
                            {
                                if (!string.IsNullOrEmpty(model.OverallComments))
                                temp3.OverallComment = model.OverallComments;
                               // temp3.OverallSemesterExamPrecentage = await this.GetHiEduStudentPrecentage(temp2.ChildId, temp2.TestId, semyearid);
                                db.SaveChanges();
                            }
                            else //if record doesn't exist we add it for the first time
                            {
                                HiEdu_OverallChildTests oct = new HiEdu_OverallChildTests();
                                //oct.Id = Guid.NewGuid();
                                oct.ChildId = (int)model.childid;
                                oct.BatchCourseMappingId = batchcoursemappingid;
                                //oct.TestId = (int)model.testid;
                                db.HiEdu_OverallChildTestss.Add(oct);
                                db.SaveChanges();
                            }

                            //code for updating/ inserting into SubjectSemesterPercentage
                            var semtestid = db.MSemestertestsmappings.Where(y => y.Id == model.testid).Select(x => x.SemesterId).FirstOrDefault();
                            //var secid = db.MChildschoolmappings.Where(x => x.Childid == model.childid).Select(w => w.Standardsectionmappingid).FirstOrDefault();
                            //var stdid = db.MStandardsectionmappings.Where(p => p.Id == secid).Select(w => w.Parentid).FirstOrDefault();
                            //var subid = db.MSubjectsectionmappings.Where(e => e.SectionId == secid).Select(a => a.SubjectId).FirstOrDefault();
                            //item.SubjectSemesterPercentage = await this.GetSubjectSemesterPercentage((int)semtestid, (int)model.childid, subid);

                            //var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                            //using (SqlConnection connection = new SqlConnection(connectionString))
                            //{
                            //    connection.Open();
                            //    SqlCommand command = new SqlCommand(ApplicationConstants.UpdateReportCard, connection);
                            //    command.CommandType = CommandType.StoredProcedure;
                            //    command.Parameters.Add(new SqlParameter("@ChildId", SqlDbType.Int));
                            //    command.Parameters["@ChildId"].Value = model.childid;
                            //    command.Parameters.Add(new SqlParameter("@SubjectSectionMappingId", SqlDbType.Int));
                            //    command.Parameters["@SubjectSectionMappingId"].Value = temp3.SubjectSectionMappingId;
                            //    command.Parameters.Add(new SqlParameter("@StandardId", SqlDbType.Int));
                            //    command.Parameters["@StandardId"].Value = stdid;
                            //    command.Parameters.Add(new SqlParameter("@SemesterId", SqlDbType.Int));
                            //    command.Parameters["@SemesterId"].Value = semtestid;
                            //    command.Parameters.Add(new SqlParameter("@SubjectSemesterPercentage", SqlDbType.Float));
                            //    command.Parameters["@SubjectSemesterPercentage"].Value = item.SubjectSemesterPercentage;
                            //    command.Parameters.Add(new SqlParameter("@Createdby", SqlDbType.Int));
                            //    command.Parameters["@Createdby"].Value = model.userid;
                            //    command.Parameters.Add(new SqlParameter("@Modifiedby", SqlDbType.Int));
                            //    command.Parameters["@Modifiedby"].Value = model.userid;
                            //    command.ExecuteNonQuery();
                            //    //}
                            //    db.SaveChanges();
                            //}
                        }

                    }
                    return (new { Value = "Updated Successfully" });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //HiEdu Student Batch Position
        public async Task<string> HiEduGetOverallBatchStudentPosition(int examid, int batchid, int courseid, int childid)
        {
            try
            {

                var batchcoursemappingid = db4.MHiEdubatchs.Where(x => x.CourseId == courseid && x.Id == batchid).OrderByDescending(w => w.Id).Select(w => w.Id).FirstOrDefault();
                var childoverallprecentage = await Task.FromResult(db4.HiEdu_OverallChildTestss.Where(x => x.BatchCourseMappingId == batchcoursemappingid && x.ChildId == childid).OrderByDescending(w => w.ChildId).Select(w => w.OverallSemesterExamPrecentage).ToArray());
                var res = await Task.FromResult(db4.HiEdu_OverallChildTestss.Where(x => x.BatchCourseMappingId == batchcoursemappingid).OrderByDescending(w => w.Id).Select(w => w.OverallSemesterExamPrecentage).ToArray());

                if (res != null)
                {
                    int totalMarks = res.Count();
                    //Array.Sort(res);
                    //Array.Reverse(res);
                    var position = Array.FindIndex(res, x => x.Equals(childoverallprecentage)) + 1;
                    if (position > 0)
                    {
                        return (position + "/" + totalMarks);
                    }
                    return ("N/A");
                }
                return ("N/A");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //HiEdu Student Subject Marks

        public async Task<List<SubjectMarks>> HiEduGetAllSubjectMarksForStudent(int childId, int examId)
        {
            try
            {
                var result = await Task.Run(() => (from cem in db.MHiEdu_CourseExamMarkss
                                                   join sem in db.MHieduSubjectExamMappings on cem.SubjectExamMappingId equals sem.Id
                                                   join cse in db.MHiEduCourseSemesterExams on sem.CourseSemesterExamId equals cse.Id
                                                   where cem.ChildId == childId && cse.Id == examId
                                                   select new SubjectMarks
                                                   {
                                                       SubjectName = sem.Subject,
                                                       Marks = cem.Marks
                                                   }).ToList());

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    
        //HiEdu Student Average
        public async Task<List<SubjectAverageScore>> GetHiEduAverageSubjectMarks(int courseId, int batchId, int semesterId, int examId)
        {
            try
            {
                var subjectMarks = await Task.Run(() =>
                    (from cem in db.MHiEdu_CourseExamMarkss
                     join sem in db.MHieduSubjectExamMappings on cem.SubjectExamMappingId equals sem.Id
                     join cse in db.MHiEduCourseSemesterExams on sem.CourseSemesterExamId equals cse.Id
                     join scm in db.HiEdu_SemesterCourseMappings on cse.SemesterCourseMappingId equals scm.Id
                     join hb in db.MHiEdubatchs on scm.CourseId equals hb.CourseId
                     where hb.CourseId == courseId && hb.Id == batchId && scm.Id == semesterId && cse.Id == examId
                     select new { sem.Subject, Marks = cem.Marks })
                    .GroupBy(sm => sm.Subject)
                    .Select(group => new SubjectAverageScore { Subject = group.Key, AverageScore = group.Average(sm => sm.Marks) })
                    .ToListAsync());

                return subjectMarks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> GetOverallStudentPosition(int testid, double? marks, int sectionid, int? academicYearId)
        {
            try
            {
                //var section = db.ChildTestMappings.
                var res = await Task.FromResult(db4.MOverallchildtests.Where(x => x.TestId == testid && x.SectionId == sectionid && x.AcademicYearId == academicYearId).OrderByDescending(w => w.OverallPercentage).Select(w => w.OverallPercentage).ToArray());
                if (res != null)
                {
                    int totalMarks = res.Count();
                    //Array.Sort(res);
                    //Array.Reverse(res);
                    var position = Array.FindIndex(res, x => x.Equals(marks)) + 1;
                    if (position > 0)
                    {
                        return (position + "/" + totalMarks);
                    }
                    return ("N/A");
                }
                return ("N/A");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> UpdateStudentsRc(postStudentRCModel model)
        {
            try
            {
                if (model.studentsRCModels.Count() == 0 && !string.IsNullOrEmpty(model.OverallComments))
                {
                    var temp2 = await db.MOverallchildtests.Where(s => s.ChildId == model.childid && s.TestId == model.testid && s.AcademicYearId == model.AcademicYearId).FirstOrDefaultAsync();
                    if (temp2 != null)
                    {
                        if (!string.IsNullOrEmpty(model.OverallComments))
                            temp2.OverallComments = model.OverallComments;
                        temp2.OverallCommentstwo = model.OverallCommentsSectionalHead;
                        temp2.OverallCommenthree = model.OverallCommentsHeadmaster;

                        db.SaveChanges();
                        return (new { Value = "Updated Succesfully" });
                    }
                    return (new { Value = "Overall details not found" });

                }
                else
                {
                    foreach (var item in model.studentsRCModels)
                    {
                        var temp = db.MChildtestmappings.Find(item.ChildTestMappingId);
                        if (temp == null)
                        {
                            MChildtestmapping ctm = new MChildtestmapping();
                            //ctm.Id = Guid.NewGuid();
                            ctm.ChildId = (int)model.childid;
                            ctm.SubjectTestMappingId = (int)item.SubjectTestMappingId;
                            ctm.Marks = item.Marks;
                            ctm.Comments = item.Comments;
                            //ctm.Percentage = item.Percentage;
                            ctm.Createddate = DateTime.Today;
                            ctm.Modifieddate = DateTime.Today;
                            ctm.Modifiedby = model.userid;
                            ctm.Statusid = 1;
                            //ctm.IsAbsent = item.IsAbsent;
                            db.MChildtestmappings.Add(ctm);
                            var dbup1 = db.SaveChanges();
                            var arraysize = item.SubSubjectsMarks.Count();
                            for (int i = 0; i < arraysize; i++)
                            {
                                MSubsubjectmarks ssm = new MSubsubjectmarks();
                                var subsubjectmarks = item.SubSubjectsMarks[i];
                                var subsubjectIds = item.SubSubjectsIds[i];
                                ssm.SubSubjectId = subsubjectIds;
                                ssm.Marks = subsubjectmarks;
                                ssm.SubPrecentage = subsubjectmarks;
                                ssm.Absent = item.SubAbsent[i];
                                ssm.ChildTestMappingId = db.MChildtestmappings.Where(x => x.SubjectTestMappingId == item.SubjectTestMappingId && x.ChildId == (int)model.childid).Select(a => a.Id).FirstOrDefault();
                                db.MSubsubjectmarkss.Add(ssm);
                                db.SaveChanges();
                                //var subjectsection = await db.MSubjectsectionmappings.Where(x => x.Id == maxmar.SubjectSectionMappingId).FirstOrDefaultAsync();
                                //var ssm2 = db.MSubsubjectmarkss.Where(x => x.ChildTestMappingId == item.ChildTestMappingId && x.SubSubjectId == subsubjectid).FirstOrDefaultAsync();
                            }
                            var maxmar = await db.MSubjecttestmappings.Where(x => x.Id == item.SubjectTestMappingId).FirstOrDefaultAsync();

                            if (maxmar != null)
                            {
                                //maxmar.MaxMarks = item.MaxMarks;
                                var semid = db.MSemestertestsmappings.Where(y => y.Id == model.testid).Select(b => b.SemesterId).FirstOrDefault();
                                maxmar.SemesterYearMappingId = db.MSemesteryearmappings.Where(x => x.SemesterId == semid && x.AcademicYearId == model.AcademicYearId).Select(a => a.Id).FirstOrDefault();
                            }
                            else
                            {
                                return (new { Value = "SubjectTestMapping doesn't exist" });
                            }

                            //var dbup = db.SaveChanges();

                            //if (dbup > 0) // checks if records are updated
                            //{
                            var temp2 = await db.MOverallchildtests.Where(s => s.ChildId == model.childid && s.TestId == model.testid && s.AcademicYearId == model.AcademicYearId).FirstOrDefaultAsync(); // for updating overallchildtest table
                            var semesterid = db.MSemestertestsmappings.Where(y => y.Id == model.testid).Select(a => a.SemesterId).FirstOrDefault();
                            var semyearid = db.MSemesteryearmappings.Where(a => a.SemesterId == semesterid && a.AcademicYearId == model.AcademicYearId).Select(a => a.Id).FirstOrDefault();
                            if (temp2 != null)
                            {
                                if (!string.IsNullOrEmpty(model.OverallComments))
                                    temp2.OverallComments = model.OverallComments;
                                temp2.OverallCommentstwo = model.OverallCommentsSectionalHead;
                                temp2.OverallCommenthree = model.OverallCommentsHeadmaster;
                                temp2.OverallPercentage = await this.GetOverallStudentPercentage(temp2.ChildId, temp2.TestId, semyearid);
                                temp2.OverallPosition = await GetOverallStudentPosition(temp2.TestId, temp2.OverallPercentage, temp2.SectionId, temp2.AcademicYearId).ConfigureAwait(false);
                                db.SaveChanges();
                                //temp2.OverallPosition = GetOverallStudentPosition(temp2.TestId, temp2.OverallMarks, sectionid);
                                //db.SaveChanges();
                            }
                            else //if record doesn't exist we add it for the first time
                            {
                                MOverallchildtest oct = new MOverallchildtest();
                                //oct.Id = Guid.NewGuid();
                                oct.ChildId = (int)model.childid;
                                oct.TestId = (int)model.testid;
                                oct.SectionId = (int)db.MChildschoolmappings.Where(x => x.Childid == model.childid && x.AcademicYearId == model.AcademicYearId).Select(w => w.Standardsectionmappingid).FirstOrDefault();
                                oct.AcademicYearId = model.AcademicYearId;
                                db.MOverallchildtests.Add(oct);
                                db.SaveChanges();
                            }

                            var semtestid = db.MSemestertestsmappings.Where(y => y.Id == model.testid).Select(x => x.SemesterId).FirstOrDefault();
                            var secid = db.MChildschoolmappings.Where(x => x.Childid == model.childid).Select(w => w.Standardsectionmappingid).FirstOrDefault();
                            var stdid = db.MStandardsectionmappings.Where(p => p.Id == secid).Select(w => w.Parentid).FirstOrDefault();
                            var subid = db.MSubjectsectionmappings.Where(e => e.SectionId == secid).Select(a => a.SubjectId).FirstOrDefault();
                            var subsecid = db.MSubjecttestmappings.Where(w => w.Id == item.SubjectTestMappingId).Select(g => g.SubjectSectionMappingId).FirstOrDefault();

                            item.SubjectSemesterPercentage = await this.GetSubjectSemesterPercentage((int)semtestid, (int)model.childid, subid);

                            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                SqlCommand command = new SqlCommand(ApplicationConstants.UpdateReportCard, connection);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.Add(new SqlParameter("@ChildId", SqlDbType.Int));
                                command.Parameters["@ChildId"].Value = model.childid;
                                command.Parameters.Add(new SqlParameter("@SubjectSectionMappingId", SqlDbType.Int));
                                command.Parameters["@SubjectSectionMappingId"].Value = subsecid;
                                command.Parameters.Add(new SqlParameter("@StandardId", SqlDbType.Int));
                                command.Parameters["@StandardId"].Value = stdid;
                                command.Parameters.Add(new SqlParameter("@SemesterId", SqlDbType.Int));
                                command.Parameters["@SemesterId"].Value = semtestid;
                                command.Parameters.Add(new SqlParameter("@SubjectSemesterPercentage", SqlDbType.Float));
                                command.Parameters["@SubjectSemesterPercentage"].Value = item.SubjectSemesterPercentage;
                                command.Parameters.Add(new SqlParameter("@Createdby", SqlDbType.Int));
                                command.Parameters["@Createdby"].Value = model.userid;
                                command.Parameters.Add(new SqlParameter("@Modifiedby", SqlDbType.Int));
                                command.Parameters["@Modifiedby"].Value = model.userid;
                                command.ExecuteNonQuery();
                                //}
                                db.SaveChanges();

                            }
                        }
                        else
                        {
                            var temp3 = db.MSubjecttestmappings.Where(w => w.Id == temp.SubjectTestMappingId).FirstOrDefault();
                            var temp4 = db.MChildtestmappings.Where(x => x.SubjectTestMappingId == temp.SubjectTestMappingId).Select(a => a.Marks).ToList();
                            //if (item.MaxMarks.HasValue)
                            //{
                            //    if (temp4 != null)
                            //    {
                            //        foreach (var item1 in temp4)
                            //        {
                            //            if (item1 > item.MaxMarks)
                            //            {
                            //                return (new { Value = "Max marks cannot be greater than marks obtained" });
                            //            }
                            //            else
                            //            {
                            //                if (item.MaxMarks <= temp3.MaxMarks || item.MaxMarks <= item.MaxMarks)
                            //                {
                            //                    temp3.MaxMarks = item.MaxMarks;
                            //                }
                            //                else
                            //                {
                            //                    return (new { Value = "Max marks has to be less than or equal 100" });
                            //                }
                            //            }
                            //        }
                            //    }
                            //}

                            if (item.Marks.HasValue)
                            {
                                //if (item.Marks <= temp3.MaxMarks || item.Marks <= item.MaxMarks)
                                //{
                                temp.Marks = item.Marks;
                                //}
                                //else
                                //{
                                //    return (new { Value = "Marks has to be less than or equal to max marks" });
                                //}
                            }
                            else
                            {
                                temp.Marks = item.Marks;
                            }
                            if (item.Percentage.HasValue)
                            {
                                if (item.Percentage <= 100)
                                {
                                    temp.Percentage = item.Percentage;
                                }
                                else
                                {
                                    return (new { Value = "Percentage has to be less than or equal 100" });
                                }
                            }
                            else
                            {
                                temp.Percentage = item.Percentage;
                            }
                            if (!string.IsNullOrEmpty(item.Comments))
                                temp.Comments = item.Comments;

                            temp.Modifiedby = model.userid;
                            //temp.IsAbsent = item.IsAbsent;
                            var dbup = db.SaveChanges();
                            //update existing sub subject marks
                            var arraysize = item.SubSubjectsMarks.Count();
                            for (int i = 0; i < arraysize; i++)
                            {
                                MSubsubjectmarks ssm = new MSubsubjectmarks();
                                var subsubjectmarks = item.SubSubjectsMarks[i];
                                var subsubjectIds = item.SubSubjectsIds[i];
                                // var subcomment = item.SubSubjectComment[i];
                                ssm.SubSubjectId = subsubjectIds;
                                ssm.Marks = subsubjectmarks;
                                ssm.SubPrecentage = subsubjectmarks;
                                ssm.Absent = item.SubAbsent[i];
                                ssm.ChildTestMappingId = item.ChildTestMappingId;

                                var connectionString2 = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                                using (SqlConnection connection = new SqlConnection(connectionString2))
                                {
                                    connection.Open();
                                    SqlCommand command = new SqlCommand(ApplicationConstants.UpdateSubSubjectMarksSp, connection);
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.Add(new SqlParameter("@ChildTestMappingId", SqlDbType.Int));
                                    command.Parameters["@ChildTestMappingId"].Value = ssm.ChildTestMappingId;
                                    command.Parameters.Add(new SqlParameter("@SubSubjectId", SqlDbType.Int));
                                    command.Parameters["@SubSubjectId"].Value = ssm.SubSubjectId;
                                    command.Parameters.Add(new SqlParameter("@Marks", SqlDbType.Int));
                                    command.Parameters["@Marks"].Value = ssm.Marks;
                                    command.Parameters.Add(new SqlParameter("@Absent", SqlDbType.Int));
                                    command.Parameters["@Absent"].Value = ssm.Absent;
                                    command.Parameters.Add(new SqlParameter("@SubPrecentage", SqlDbType.Float));
                                    command.Parameters["@SubPrecentage"].Value = ssm.SubPrecentage;
                                    //command.Parameters.Add(new SqlParameter("@subcomment", SqlDbType.NVarChar));
                                    // command.Parameters["@subcomment"].Value = subcomment;
                                    command.ExecuteNonQuery();
                                    //}
                                    db.SaveChanges();
                                }

                                //db.MSubsubjectmarkss.Update(ssm);
                                //var subjectsection = await db.MSubjectsectionmappings.Where(x => x.Id == maxmar.SubjectSectionMappingId).FirstOrDefaultAsync();
                                //var ssm2 = db.MSubsubjectmarkss.Where(x => x.ChildTestMappingId == item.ChildTestMappingId && x.SubSubjectId == subsubjectid).FirstOrDefaultAsync();
                            }


                            //if (dbup > 0) // checks if records are updated
                            //{
                            var temp2 = db.MOverallchildtests.Where(s => s.ChildId == model.childid && s.TestId == model.testid && s.AcademicYearId == model.AcademicYearId).FirstOrDefault(); // for updating overallchildtest table
                            var semid = db.MSemestertestsmappings.Where(y => y.Id == model.testid).Select(a => a.SemesterId).FirstOrDefault();
                            var semyearid = db.MSemesteryearmappings.Where(a => a.SemesterId == semid && a.AcademicYearId == model.AcademicYearId).Select(a => a.Id).FirstOrDefault();
                            if (temp2 != null)
                            {
                                //if (!string.IsNullOrEmpty(model.OverallComments))
                                    temp2.OverallComments = model.OverallComments;
                                temp2.OverallCommentstwo = model.OverallCommentsSectionalHead;
                                temp2.OverallCommenthree = model.OverallCommentsHeadmaster;
                                temp2.OverallPercentage = await this.GetOverallStudentPercentage(temp2.ChildId, temp2.TestId, semyearid);
                                temp2.OverallPosition = await GetOverallStudentPosition(temp2.TestId, temp2.OverallPercentage, temp2.SectionId, temp2.AcademicYearId).ConfigureAwait(false);

                                db.SaveChanges();
                            }
                            else //if record doesn't exist we add it for the first time
                            {

                                MOverallchildtest oct = new MOverallchildtest();
                                //oct.Id = Guid.NewGuid();
                                oct.ChildId = (int)model.childid;
                                oct.TestId = (int)model.testid;
                                oct.SectionId = (int)db.MChildschoolmappings.Where(x => x.Childid == model.childid && x.AcademicYearId == model.AcademicYearId).Select(w => w.Standardsectionmappingid).FirstOrDefault();
                                oct.AcademicYearId = model.AcademicYearId;
                                temp2.OverallPercentage = await this.GetOverallStudentPercentage(temp2.ChildId, temp2.TestId, semyearid);
                                temp2.OverallPosition = await GetOverallStudentPosition(temp2.TestId, temp2.OverallPercentage, temp2.SectionId, temp2.AcademicYearId).ConfigureAwait(false);

                                db.MOverallchildtests.Add(oct);
                                db.SaveChanges();
                            }

                            //code for updating/ inserting into SubjectSemesterPercentage
                            var semtestid = db.MSemestertestsmappings.Where(y => y.Id == model.testid).Select(x => x.SemesterId).FirstOrDefault();
                            var secid = db.MChildschoolmappings.Where(x => x.Childid == model.childid).Select(w => w.Standardsectionmappingid).FirstOrDefault();
                            var stdid = db.MStandardsectionmappings.Where(p => p.Id == secid).Select(w => w.Parentid).FirstOrDefault();
                            var subid = db.MSubjectsectionmappings.Where(e => e.SectionId == secid).Select(a => a.SubjectId).FirstOrDefault();
                            item.SubjectSemesterPercentage = await this.GetSubjectSemesterPercentage((int)semtestid, (int)model.childid, subid);

                            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                SqlCommand command = new SqlCommand(ApplicationConstants.UpdateReportCard, connection);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.Add(new SqlParameter("@ChildId", SqlDbType.Int));
                                command.Parameters["@ChildId"].Value = model.childid;
                                command.Parameters.Add(new SqlParameter("@SubjectSectionMappingId", SqlDbType.Int));
                                command.Parameters["@SubjectSectionMappingId"].Value = temp3.SubjectSectionMappingId;
                                command.Parameters.Add(new SqlParameter("@StandardId", SqlDbType.Int));
                                command.Parameters["@StandardId"].Value = stdid;
                                command.Parameters.Add(new SqlParameter("@SemesterId", SqlDbType.Int));
                                command.Parameters["@SemesterId"].Value = semtestid;
                                command.Parameters.Add(new SqlParameter("@SubjectSemesterPercentage", SqlDbType.Float));
                                command.Parameters["@SubjectSemesterPercentage"].Value = item.SubjectSemesterPercentage;
                                command.Parameters.Add(new SqlParameter("@Createdby", SqlDbType.Int));
                                command.Parameters["@Createdby"].Value = model.userid;
                                command.Parameters.Add(new SqlParameter("@Modifiedby", SqlDbType.Int));
                                command.Parameters["@Modifiedby"].Value = model.userid;
                                command.ExecuteNonQuery();
                                //}
                                db.SaveChanges();
                            }
                        }

                    }
                    return (new { Value = "Updated Successfully" });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        // 09/03/2023 -Sanduni SubSubject Student Chart APIS
        public async Task<object> GetSubSubjectRankByClass(int childid, int SubjectId, int SubSubjectId, int LevelID, int schoolid, int TestId)
        {
            try
            {
                var stdRC = (from ctm in db5.MOverallchildtests
                             where ctm.ChildId == childid
                             select new { ChildTestId = ctm.TestId, ExamOverallPercentage = ctm.OverallPercentage, ChildAcademicYearId = ctm.AcademicYearId }).ToList();

                int i = 0;
                var arraysize = stdRC.Count();
                studentRChartCModel stdRCM = new studentRChartCModel();
                stdRCM.OverallTotalMarksArray = new double?[arraysize];
                stdRCM.YearSemesterExamArray = new string[arraysize];
                stdRCM.OverallPercentageArray = new int?[arraysize];
                stdRCM.OverallRankArray = new int?[arraysize];
                stdRCM.OverallSubjectRankArray = new int?[arraysize];
                stdRCM.OverallSubjectMarksArray = new double?[arraysize];
                stdRCM.SubjectClassExamSubjectRankPrecentageArray = new double?[arraysize];
                stdRCM.SubjectRankArrayWithTotalStudents = new string[arraysize];


                //var List1 = (from ctm in db5.MChildtestmappings
                //             join sym in db5.MSemesteryearmappings on ctm.ChildId equals childid
                //             join sttm in db5.MSemestertestsmappings on sym.SemesterId equals sttm.SemesterId
                //             join stm in db5.MSubjecttestmappings on ctm.SubjectTestMappingId equals stm.Id
                //             join stm1 in db5.MSubjecttestmappings on sttm.Id equals stm1.TestId
                //             join stm2 in db5.MSubjecttestmappings on sym.Id equals stm2.SemesterYearMappingId
                //             join ssm in db5.MSubjectsectionmappings on stm.SubjectSectionMappingId equals ssm.Id
                //             join sub in db5.MSubjects on ssm.SubjectId equals sub.Id

                //             where ctm.ChildId == childid && sub.Id == SubjectId && ssm.SubjectId == SubjectId
                //             select new { ChildTestId = sttm.Id, SubjectId = sub.Id, SubjectTestMappingId = ctm.SubjectTestMappingId, Subject = sub.Name, ctm.Marks, stm.MaxMarks, ctm.Percentage, ctm.Comments, ctm.IsAbsent, sym.AcademicYearId }).ToList();


                var res = new List<ChildSubSubjectMarksFor9Semesters>();
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetSubjectMarksByChildVSClass_1Sp, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                    command.Parameters["@schoolid"].Value = schoolid;
                    command.Parameters.Add(new SqlParameter("@childid", SqlDbType.Int));
                    command.Parameters["@childid"].Value = childid;
                    command.Parameters.Add(new SqlParameter("@SubjectId", SqlDbType.Int));
                    command.Parameters["@SubjectId"].Value = SubjectId;
                    command.Parameters.Add(new SqlParameter("@LevelID", SqlDbType.Int));
                    command.Parameters["@LevelID"].Value = LevelID;
                    command.Parameters.Add(new SqlParameter("@TestId", SqlDbType.Int));
                    command.Parameters["@TestId"].Value = TestId;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                await Task.Run(() => res.Add(new ChildSubSubjectMarksFor9Semesters()
                                {
                                    Marks = reader["Marks"] != DBNull.Value ? (int?)reader["Marks"] : null,
                                    ChildTestId = reader["childtestmappingid"] != DBNull.Value ? (int?)reader["childtestmappingid"] : null,
                                    Subject = reader["subname"].ToString(),
                                    //Comment = reader["Comment"].ToString(),
                                    //SubMaxMarks = reader["SubMaxMarks"] != DBNull.Value ? (int?)reader["SubMaxMarks"] : null,
                                    //SubSubject = reader["subsubname"].ToString(),
                                    AcademicYearId = reader["AcademicYearId"] != DBNull.Value ? (int?)reader["AcademicYearId"] : null,
                                    //MaxMarks = reader["MaxMarks"] != DBNull.Value ? (int?)reader["MaxMarks"] : null,
                                    SubjectTestMappingId = reader["SubjectTestMappingId"] != DBNull.Value ? (int?)reader["SubjectTestMappingId"] : null,
                                    ExamId = reader["ExamId"] != DBNull.Value ? (int?)reader["ExamId"] : null,
                                }));

                            }
                        }
                    }
                }

                if (stdRC.Count() > 0)
                {
                    foreach (var item in res)
                    {
                        // var SubjectTestMappingId = db.ChildTestMappings.Where(x => x.SubjectTestMappingId.Equals(item.ChildTestId)).Select(w => w.SubjectTestMappingId).FirstOrDefault();
                        if (item.Marks == null)
                        {
                            stdRCM.OverallSubjectRankArray[i] = 0;
                            stdRCM.OverallSubjectMarksArray[i] = 0;
                        }
                        else
                        {
                            double tempval = (double)item.Marks;
                            //var testmarks = tempval * 100;
                            stdRCM.OverallSubjectMarksArray[i] = tempval;
                            stdRCM.ChildID = childid;
                            //stdRCM.SubSubjectRankArrayWithStudentCount[i] = await this.GetSubSubjectStudentPosition(item.SubjectTestMappingId, item.Marks, item.SubSubjectId);
                            //stdRCM.SubSubjectRankArrayByChild[i] = GetStudentSubSubjectPositionOnly(item.SubjectTestMappingId, item.Marks, item.SubSubjectId);
                            //stdRCM.SubSubjectExamTotalStudentSubjectMarksAverageByClass[i] = GetSubSubjectExamTotalStudentSubjectMarksClass(item.SubjectTestMappingId, item.Marks, (double)item.SubMaxMarks, item.SubSubjectId);
                            stdRCM.OverallSubjectRankArray[i] = GetStudentSubjectPositionOnly(item.SubjectTestMappingId, item.Marks);
                            stdRCM.SubjectRankArrayWithTotalStudents[i] = GetStudentPosition(item.SubjectTestMappingId, item.Marks);

                            //var childtestmappingid = db.MChildtestmappings.Where(x => x.ChildId.Equals(childid) && x.SubjectTestMappingId.Equals(item.SubjectTestMappingId)).Select(w => w.Id).FirstOrDefault();
                            //var subsubjects = db.MSubSubjects.Where(x => x.SubjectId.Equals(SubjectId)).ToList();
                            //var res3 = new List<studentRCSubSubjectInnerModel>();

                            //foreach (var item2 in subsubjects)
                            //{
                            //    await Task.Run(() => res3.Add(new studentRCSubSubjectInnerModel()
                            //    {
                            //        StudentSubSubjectsMark = db.MSubsubjectmarkss.Where(x => x.ChildTestMappingId.Equals(childtestmappingid) && x.SubSubjectId.Equals(item2.Id)).Select(w => w.Marks).FirstOrDefault(),

                            //        StudentSubSubjectsId = item2.Id,

                            //        StudentSubSubject = item2.SubSubject,
                            //    }));

                            //}

                            //foreach (var item3 in res3)
                            //{
                            //    studentRCSubSubjectInnerModel subsubjectmarksobjects = new studentRCSubSubjectInnerModel();
                            //    subsubjectmarksobjects.StudentSubSubjectsId = item3.StudentSubSubjectsId;
                            //    subsubjectmarksobjects.StudentSubSubject = item3.StudentSubSubject;
                            //    subsubjectmarksobjects.StudentSubSubjectsMark = item3.StudentSubSubjectsMark;
                            //    stdRCM.studentRCSubSubjectInnerModels.Add(subsubjectmarksobjects);

                            //}


                            stdRCM.SubjectClassExamSubjectRankPrecentageArray[i] = GetExamTotalStudentSubjectMarksClass(item.SubjectTestMappingId, item.Marks);

                            var Exam = db.MSemestertestsmappings.Where(x => x.Id.Equals(item.ExamId)).Select(w => w.Name).FirstOrDefault();
                            var SemesterIDTemp = db.MSemestertestsmappings.Where(x => x.Id.Equals(item.ExamId)).Select(w => w.SemesterId).FirstOrDefault();
                            var YearName = db5.MAcademicyeardetails.Where(x => x.Id.Equals(item.AcademicYearId)).Select(w => w.YearName).FirstOrDefault();
                            var SectionId = db5.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == item.AcademicYearId).Select(w => w.Standardsectionmappingid).FirstOrDefault();
                            var sectioname = db5.MStandardsectionmappings.Where(p => p.Id == SectionId).Select(w => w.Name).FirstOrDefault();
                            var Standardid = db5.MStandardsectionmappings.Where(p => p.Id == SectionId).Select(w => w.Parentid).FirstOrDefault();
                            var stdname = db5.MStandardsectionmappings.Where(p => p.Id == Standardid).Select(w => w.Name).FirstOrDefault();
                            var BranchId = db5.MBranches.Where(p => p.Schoolid == schoolid).Select(w => w.Id).FirstOrDefault();

                            var res2 = (from sym in db5.MSemesteryearmappings
                                        join stm in db5.MSemestertestsmappings on sym.SemesterId equals stm.Id
                                        where stm.BranchId == BranchId && sym.AcademicYearId == item.AcademicYearId && stm.Id == SemesterIDTemp
                                        select new { Id = stm.Id, Name = stm.Name }).Distinct().FirstOrDefault();
                            var result = Regex.Match(Exam, @"^([\w\-]+)");

                            stdRCM.YearSemesterExamArray[i] = YearName + " " + res2.Name + " " + result + " " + "Gr" + stdname;
                        }
                        i++;
                    }
                    return (stdRCM);
                }
                else
                {

                    return (stdRCM);
                }
                return ("Student test mapping not exist");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<object> GetStudentMainSubjectSubSubjectMarks(int childid, int SubjectId, int Sectionid, int schoolid, int TestId)
        {
            try
            {

                studentRChartCModel stdRCM = new studentRChartCModel();
                var subjectsectionmappingid = db.MSubjectsectionmappings.Where(x => x.SectionId.Equals(Sectionid) && x.SubjectId.Equals(SubjectId)).Select(w => w.Id).FirstOrDefault();
                var subjecttestmappingsid = db.MSubjecttestmappings.Where(x => x.TestId.Equals(TestId) && x.SubjectSectionMappingId.Equals(subjectsectionmappingid)).Select(w => w.Id).FirstOrDefault();

                var childtestmappingid = db.MChildtestmappings.Where(x => x.ChildId.Equals(childid) && x.SubjectTestMappingId.Equals(subjecttestmappingsid)).Select(w => w.Id).FirstOrDefault();
                var subsubjects = db.MSubSubjects.Where(x => x.SubjectId.Equals(SubjectId)).ToList();
                var res3 = new List<studentRCSubSubjectInnerModel>();

                foreach (var item2 in subsubjects)
                {
                    await Task.Run(() => res3.Add(new studentRCSubSubjectInnerModel()
                    {
                        StudentSubSubjectsMark = db.MSubsubjectmarkss.Where(x => x.ChildTestMappingId.Equals(childtestmappingid) && x.SubSubjectId.Equals(item2.Id)).Select(w => w.Marks).FirstOrDefault(),

                        StudentSubSubjectsId = item2.Id,

                        StudentSubSubject = item2.SubSubject,
                    }));

                }

                foreach (var item3 in res3)
                {
                    studentRCSubSubjectInnerModel subsubjectmarksobjects = new studentRCSubSubjectInnerModel();
                    subsubjectmarksobjects.StudentSubSubjectsId = item3.StudentSubSubjectsId;
                    subsubjectmarksobjects.StudentSubSubject = item3.StudentSubSubject;
                    subsubjectmarksobjects.StudentSubSubjectsMark = item3.StudentSubSubjectsMark;
                    stdRCM.studentRCSubSubjectInnerModels.Add(subsubjectmarksobjects);

                }
                return (stdRCM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Onesubject Grade Analytics - OneSubject and Rank Grade 
        public async Task<object> GeOverallSubjectMarksOverallRankByGrade(int SubjectId1, int GradeId, int TestId, int? AcademicYearId)
        {
            try
            {

                studentRChartCModel stdRCM = new studentRChartCModel();
                if (SubjectId1 != 0 && GradeId != 0)
                {
                    var res1 = new List<SubjectGradePrecentage>();
                    var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {

                        connection.Open();
                        SqlCommand command = new SqlCommand(ApplicationConstants.GetsubjectGradeprecentage, connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@SubjectId", SqlDbType.Int));
                        command.Parameters["@SubjectId"].Value = SubjectId1;
                        command.Parameters.Add(new SqlParameter("@GradeId", SqlDbType.Int));
                        command.Parameters["@GradeId"].Value = GradeId;
                        command.Parameters.Add(new SqlParameter("@TestId", SqlDbType.Int));
                        command.Parameters["@TestId"].Value = TestId;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    await Task.Run(() => res1.Add(new SubjectGradePrecentage()
                                    {
                                        Marks = reader["Marks"] != DBNull.Value ? (int?)reader["Marks"] : null,
                                        ChildId = reader["ChildId"] != DBNull.Value ? (int?)reader["ChildId"] : null,
                                        SubjectTestMappingId = reader["SubjectTestMappingId"] != DBNull.Value ? (int?)reader["SubjectTestMappingId"] : null
                                    }));

                                }
                            }
                        }
                    }
                    //var res2 = new List<SubjectGradePrecentage>();

                    //var connectionString2 = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                    //using (SqlConnection connection2 = new SqlConnection(connectionString2))
                    //{

                    //    connection2.Open();
                    //    SqlCommand command2 = new SqlCommand(ApplicationConstants.GetsubjectGradeprecentage, connection2);
                    //    command2.CommandType = CommandType.StoredProcedure;
                    //    command2.Parameters.Add(new SqlParameter("@SubjectId", SqlDbType.Int));
                    //    command2.Parameters["@SubjectId"].Value = SubjectId2;
                    //    command2.Parameters.Add(new SqlParameter("@GradeId", SqlDbType.Int));
                    //    command2.Parameters["@GradeId"].Value = GradeId;
                    //    command2.Parameters.Add(new SqlParameter("@TestId", SqlDbType.Int));
                    //    command2.Parameters["@TestId"].Value = TestId;
                    //    using (SqlDataReader reader2 = command2.ExecuteReader())
                    //    {
                    //        if (reader2.HasRows)
                    //        {
                    //            while (reader2.Read())
                    //            {
                    //                await Task.Run(() => res2.Add(new SubjectGradePrecentage()
                    //                {
                    //                    Marks = reader2["Marks"] != DBNull.Value ? (int?)reader2["Marks"] : null,
                    //                    SubjectTestMappingId = reader2["SubjectTestMappingId"] != DBNull.Value ? (int?)reader2["SubjectTestMappingId"] : null
                    //                }));

                    //            }
                    //        }
                    //    }
                    //}

                    var arraysize1 = res1.Count();
                    // var arraysize2 = res2.Count();
                    var arraysize = res1.Count();
                    //if (arraysize1 > arraysize2)
                    //{
                    //    arraysize = arraysize1;
                    //}
                    //else
                    //{
                    //    arraysize = arraysize2;
                    //}

                    stdRCM.Subject1GradeMarksPrecentageArray = new double?[arraysize];
                    stdRCM.OverallRankArray = new int?[arraysize];


                    if (res1.Count() > 0)
                    {
                        int i = 0;
                        foreach (var item in res1)
                        {


                            if (item.Marks == null)
                            {
                                stdRCM.Subject1GradeMarksPrecentageArray[i] = 0;
                                int? studentsectionid = db.MOverallchildtests.Where(x => x.ChildId.Equals(item.ChildId) && x.TestId.Equals(TestId) && x.AcademicYearId.Equals(AcademicYearId)).Select(x => x.SectionId).FirstOrDefault();
                                var OverallPercentage = db.MOverallchildtests.Where(x => x.ChildId.Equals(item.ChildId) && x.TestId.Equals(TestId) && x.AcademicYearId.Equals(AcademicYearId) && x.SectionId.Equals(studentsectionid)).Select(x => x.OverallPercentage).FirstOrDefault();

                                stdRCM.OverallRankArray[i] = GetOverallStudentPositionOnly(TestId, OverallPercentage, studentsectionid, AcademicYearId);
                            }
                            else
                            {

                                var maxmarks = db.MSubjecttestmappings.Where(x => x.Id.Equals(item.SubjectTestMappingId)).Select(w => w.MaxMarks).FirstOrDefault();

                                stdRCM.Subject1GradeMarksPrecentageArray[i] = getsubjectGradeprecentage(maxmarks, item.Marks);
                                int? studentsectionid = db.MOverallchildtests.Where(x => x.ChildId.Equals(item.ChildId) && x.TestId.Equals(TestId) && x.AcademicYearId.Equals(AcademicYearId)).Select(x => x.SectionId).FirstOrDefault();
                                var OverallPercentage = db.MOverallchildtests.Where(x => x.ChildId.Equals(item.ChildId) && x.TestId.Equals(TestId) && x.AcademicYearId.Equals(AcademicYearId) && x.SectionId.Equals(studentsectionid)).Select(x => x.OverallPercentage).FirstOrDefault();

                                stdRCM.OverallRankArray[i] = GetOverallStudentPositionOnly(TestId, OverallPercentage, studentsectionid, AcademicYearId);
                            }
                            i++;
                        }
                    }


                    //stdRCM.Subject2GradeMarksPrecentageArray = new double?[arraysize];
                    //if (res2.Count() > 0)
                    //{
                    //    int i = 0;
                    //    foreach (var item in res2)
                    //    {


                    //        if (item.Marks == null)
                    //        {
                    //            stdRCM.Subject2GradeMarksPrecentageArray[i] = 0;
                    //        }
                    //        else
                    //        {

                    //            var maxmarks = db.MSubjecttestmappings.Where(x => x.Id.Equals(item.SubjectTestMappingId)).Select(w => w.MaxMarks).FirstOrDefault();

                    //            stdRCM.Subject2GradeMarksPrecentageArray[i] = getsubjectGradeprecentage(maxmarks, item.Marks);

                    //        }
                    //        i++;
                    //    }

                    //}
                    return (stdRCM);
                }
                else
                {

                    return (stdRCM);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // 26/03/2023 -SanduniGetOverallTwoSubjectGradePrecentage Academic Overall Chart APIS
        public async Task<object> GetOverallTwoSubjectGradePrecentage(int SubjectId1, int SubjectId2, int GradeId, int TestId)
        {
            try
            {

                studentRChartCModel stdRCM = new studentRChartCModel();
                if (SubjectId1 != 0 && SubjectId2 != 0)
                {
                    var res1 = new List<SubjectGradePrecentage>();
                    var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {

                        connection.Open();
                        SqlCommand command = new SqlCommand(ApplicationConstants.GetsubjectGradeprecentage, connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@SubjectId", SqlDbType.Int));
                        command.Parameters["@SubjectId"].Value = SubjectId1;
                        command.Parameters.Add(new SqlParameter("@GradeId", SqlDbType.Int));
                        command.Parameters["@GradeId"].Value = GradeId;
                        command.Parameters.Add(new SqlParameter("@TestId", SqlDbType.Int));
                        command.Parameters["@TestId"].Value = TestId;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    await Task.Run(() => res1.Add(new SubjectGradePrecentage()
                                    {
                                        Marks = reader["Marks"] != DBNull.Value ? (int?)reader["Marks"] : null,
                                        SubjectTestMappingId = reader["SubjectTestMappingId"] != DBNull.Value ? (int?)reader["SubjectTestMappingId"] : null
                                    }));

                                }
                            }
                        }
                    }
                    var res2 = new List<SubjectGradePrecentage>();

                    var connectionString2 = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                    using (SqlConnection connection2 = new SqlConnection(connectionString2))
                    {

                        connection2.Open();
                        SqlCommand command2 = new SqlCommand(ApplicationConstants.GetsubjectGradeprecentage, connection2);
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.Add(new SqlParameter("@SubjectId", SqlDbType.Int));
                        command2.Parameters["@SubjectId"].Value = SubjectId2;
                        command2.Parameters.Add(new SqlParameter("@GradeId", SqlDbType.Int));
                        command2.Parameters["@GradeId"].Value = GradeId;
                        command2.Parameters.Add(new SqlParameter("@TestId", SqlDbType.Int));
                        command2.Parameters["@TestId"].Value = TestId;
                        using (SqlDataReader reader2 = command2.ExecuteReader())
                        {
                            if (reader2.HasRows)
                            {
                                while (reader2.Read())
                                {
                                    await Task.Run(() => res2.Add(new SubjectGradePrecentage()
                                    {
                                        Marks = reader2["Marks"] != DBNull.Value ? (int?)reader2["Marks"] : null,
                                        SubjectTestMappingId = reader2["SubjectTestMappingId"] != DBNull.Value ? (int?)reader2["SubjectTestMappingId"] : null
                                    }));

                                }
                            }
                        }
                    }

                    var arraysize1 = res1.Count();
                    var arraysize2 = res2.Count();
                    var arraysize = 0;
                    if (arraysize1 > arraysize2)
                    {
                        arraysize = arraysize1;
                    }
                    else
                    {
                        arraysize = arraysize2;
                    }

                    stdRCM.Subject1GradeMarksPrecentageArray = new double?[arraysize];


                    if (res1.Count() > 0)
                    {
                        int i = 0;
                        foreach (var item in res1)
                        {


                            if (item.Marks == null)
                            {
                                stdRCM.Subject1GradeMarksPrecentageArray[i] = 0;
                            }
                            else
                            {

                                var maxmarks = db.MSubjecttestmappings.Where(x => x.Id.Equals(item.SubjectTestMappingId)).Select(w => w.MaxMarks).FirstOrDefault();

                                stdRCM.Subject1GradeMarksPrecentageArray[i] = getsubjectGradeprecentage(maxmarks, item.Marks);

                            }
                            i++;
                        }
                    }


                    stdRCM.Subject2GradeMarksPrecentageArray = new double?[arraysize];
                    if (res2.Count() > 0)
                    {
                        int i = 0;
                        foreach (var item in res2)
                        {


                            if (item.Marks == null)
                            {
                                stdRCM.Subject2GradeMarksPrecentageArray[i] = 0;
                            }
                            else
                            {

                                var maxmarks = db.MSubjecttestmappings.Where(x => x.Id.Equals(item.SubjectTestMappingId)).Select(w => w.MaxMarks).FirstOrDefault();

                                stdRCM.Subject2GradeMarksPrecentageArray[i] = getsubjectGradeprecentage(maxmarks, item.Marks);

                            }
                            i++;
                        }

                    }
                    return (stdRCM);
                }
                else
                {

                    return (stdRCM);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //   26/03/2023 -GetOvverallGradeSubjectMarksinGradingMethod for last 6 semesters Grade Analytics Chart APIS
        public async Task<object> GetOvverallGradeSubjectMarksinGradingMethod(int SubjectId1, int GradeId, int AcademicYear, int SchoolId, int SemesterId, int ExamId)
        {
            try
            {

                //var AcademicYears = db.MAcademicyeardetails.Where(x => x.SchoolId == SchoolId).OrderBy(w => w.Id).Take(3).Select(w => w.Id).ToList();
                //foreach (var item1 in AcademicYears)
                //{
                //    semesteryearmappingarray[i] = db.MSemesteryearmappings.Where(x => x.AcademicYearId == item1 && x.SemesterId <= SemesterId).Select(w => w.Id).FirstOrDefault();
                //        i++;
                //}
                //var semestermarks = await Task.FromResult((from ctm in db.MChildtestmappings
                //                                   join stm in db.MSubjecttestmappings on new { a = ctm.SubjectTestMappingId} equals new { a = stm.Id }
                //                                   join ssm in db.MSubjectsectionmappings on stm.SubjectSectionMappingId equals ssm.Id
                //                                   join ssecm in db.MStandardsectionmappings on ssm.SectionId equals ssecm.Id
                //                                   where stm.SemesterYearMappingId == semesteryearmappingarray[x] && stm.TestId == ExamId
                //                                   && ssecm.Parentid == GradeId && ssm.SubjectId == SubjectId1
                //                                   select new { ctm.Marks }).ToList());

                GradeSubjectMarks grademarks = new GradeSubjectMarks();


                var semestermarks = await Task.FromResult((from ayd in db.MAcademicyeardetails
                                                           join sym in db.MSemesteryearmappings on ayd.SchoolId equals SchoolId
                                                           where sym.SemesterId == SemesterId && sym.AcademicYearId == ayd.Id
                                                           select new { sym.Id, ayd.YearName, sym.SemesterId }).ToList());
                int arraysize = semestermarks.Count();
                int i = 0;

                grademarks.YearSemesterExamArray = new string[arraysize];

                //grademarks.grademarkslist = new grademarkslistinnermodel();

                foreach (var item in semestermarks)
                {
                    var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(ApplicationConstants.GetPastYearsGradeSubjectMarks, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        int Agrade = 0;
                        int Bgrade = 0;
                        int Cgrade = 0;
                        int Dgrade = 0;
                        var semestername = db.MSemestertestsmappings.Where(x => x.Id == item.SemesterId).Select(w => w.Name).FirstOrDefault();
                        var ExamName = db.MSemestertestsmappings.Where(x => x.SemesterId == item.SemesterId).Select(w => w.Name).FirstOrDefault();
                        var result = Regex.Match(ExamName, @"^([\w\-]+)");
                        var GradeMarksListobj = new grademarkslistinnermodel();
                        List<GradeMarksModel> grademarksmodel = new List<GradeMarksModel>();
                        grademarks.YearSemesterExamArray[i] = item.YearName + semestername + result;
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("@SubjectId", SqlDbType.Int));
                        command.Parameters["@SubjectId"].Value = SubjectId1;
                        command.Parameters.Add(new SqlParameter("@GradeId", SqlDbType.Int));
                        command.Parameters["@GradeId"].Value = GradeId;
                        command.Parameters.Add(new SqlParameter("@semesteryearmappingid", SqlDbType.Int));
                        command.Parameters["@semesteryearmappingid"].Value = item.Id;
                        command.Parameters.Add(new SqlParameter("@TestId", SqlDbType.Int));
                        command.Parameters["@TestId"].Value = ExamId;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    await Task.Run(() => grademarksmodel.Add(new GradeMarksModel()
                                    {
                                        Marks = reader["Marks"] != DBNull.Value ? (int?)reader["Marks"] : null
                                    }));


                                }

                            }

                        }
                        if (command.Connection.State == ConnectionState.Closed)
                            connection.Open();
                        if (grademarksmodel != null)
                        {
                            foreach (var itemx in grademarksmodel)
                            {
                                if (itemx.Marks >= 75)
                                {
                                    Agrade = Agrade + 1;
                                }
                                else if (itemx.Marks >= 65)
                                {
                                    Bgrade = Bgrade + 1;
                                }
                                else if (itemx.Marks >= 35)
                                {
                                    Cgrade = Cgrade + 1;
                                }
                                else
                                {
                                    Dgrade = Dgrade + 1;
                                }
                            }
                            int totalcount = grademarksmodel.Count();
                            if (totalcount == 0)
                            {
                                GradeMarksListobj.AGradePrecentage = 0;

                                GradeMarksListobj.BGradePrecentage = 0;

                                GradeMarksListobj.CGradePrecentage = 0;

                                GradeMarksListobj.DGradePrecentage = 0;
                            }
                            else
                            {
                                GradeMarksListobj.AGradePrecentage = ((double)Agrade / (double)totalcount) * 100;

                                GradeMarksListobj.BGradePrecentage = ((double)Bgrade / (double)totalcount) * 100;

                                GradeMarksListobj.CGradePrecentage = ((double)Cgrade / (double)totalcount) * 100;

                                GradeMarksListobj.DGradePrecentage = ((double)Dgrade / (double)totalcount) * 100;
                            }

                        }
                        grademarks.grademarkslist.Add(GradeMarksListobj);
                    }

                    i++;
                }


                return (grademarks);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // 15/11/2022 - Student Chart APIS
        public async Task<object> GetSubjectRankClassStudentCharts(int childid, int SubjectId, int schoolid)
        {
            try
            {
                var stdRC = (from ctm in db5.MOverallchildtests
                             where ctm.ChildId == childid
                             select new { ChildTestId = ctm.TestId, ExamOverallPercentage = ctm.OverallPercentage, ChildAcademicYearId = ctm.AcademicYearId }).ToList();
                var branchid = db.MBranches.Where(x => x.Schoolid.Equals(schoolid)).Select(w => w.Id).FirstOrDefault();

                int i = 0;
                var arraysize = stdRC.Count();
                studentRChartCModel stdRCM = new studentRChartCModel();
                stdRCM.OverallTotalMarksArray = new double?[arraysize];
                stdRCM.YearSemesterExamArray = new string[arraysize];
                stdRCM.OverallPercentageArray = new int?[arraysize];
                stdRCM.OverallRankArray = new int?[arraysize];
                stdRCM.OverallSubjectRankArray = new int?[arraysize];
                stdRCM.OverallSubjectMarksArray = new double?[arraysize];
                stdRCM.SubjectClassExamSubjectRankPrecentageArray = new double?[arraysize];
                stdRCM.SubjectRankArrayWithTotalStudents = new string[arraysize];
                var List1 = (from ctm in db5.MChildtestmappings
                             join sym in db5.MSemesteryearmappings on ctm.ChildId equals childid
                             join sttm in db5.MSemestertestsmappings on sym.SemesterId equals sttm.SemesterId
                             join stm in db5.MSubjecttestmappings on ctm.SubjectTestMappingId equals stm.Id
                             join stm1 in db5.MSubjecttestmappings on sttm.Id equals stm1.TestId
                             join stm2 in db5.MSubjecttestmappings on sym.Id equals stm2.SemesterYearMappingId
                             join ssm in db5.MSubjectsectionmappings on stm.SubjectSectionMappingId equals ssm.Id
                             join sub in db5.MSubjects on ssm.SubjectId equals sub.Id
                             join  br in db5.MBranches on sttm.BranchId equals br.Id
                             join ci in db5.MChildinfos on ctm.ChildId equals ci.Id
                             join oct in db5.MOverallchildtests on ci.Id equals oct.ChildId
                             join stsm in db5.MStandardsectionmappings on oct.SectionId equals stsm.Id
                             join csm in db5.MChildschoolmappings on oct.ChildId equals csm.Childid
                             where ctm.ChildId == childid && sub.Id == SubjectId && br.Id == branchid && oct.TestId == stm1.TestId && csm.Standardsectionmappingid == oct.SectionId && stm1.Id == ctm.SubjectTestMappingId && stm2.Id == ctm.SubjectTestMappingId
                             select new { ChildTestId = sttm.Id, SubjectId = sub.Id, SubjectTestMappingId = ctm.SubjectTestMappingId, Subject = sub.Name, ctm.Marks, stm.MaxMarks, ctm.Percentage, ctm.Comments, ctm.IsAbsent, sym.AcademicYearId }).ToList();
                //20223/10/15
                if (stdRC.Count() > 0)
                {
                    foreach (var item in List1)
                    {
                        // var SubjectTestMappingId = db.ChildTestMappings.Where(x => x.SubjectTestMappingId.Equals(item.ChildTestId)).Select(w => w.SubjectTestMappingId).FirstOrDefault();
                        if (item.Marks == null)
                        {
                            stdRCM.OverallSubjectRankArray[i] = 0;
                            stdRCM.OverallSubjectMarksArray[i] = 0;
                        }
                        else
                        {
                            double tempval = (double)item.Marks / (double)item.MaxMarks;
                            var testmarks = tempval * 100;
                            stdRCM.OverallSubjectMarksArray[i] = testmarks;
                            stdRCM.OverallSubjectRankArray[i] = GetStudentSubjectPositionOnly(item.SubjectTestMappingId, item.Marks);
                            stdRCM.SubjectRankArrayWithTotalStudents[i] = GetStudentPosition(item.SubjectTestMappingId, item.Marks);
                            stdRCM.SubjectClassExamSubjectRankPrecentageArray[i] = GetExamTotalStudentSubjectMarksClass(item.SubjectTestMappingId, item.Marks);
                            var Exam = db.MSemestertestsmappings.Where(x => x.Id.Equals(item.ChildTestId)).Select(w => w.Name).FirstOrDefault();
                            var SemesterIDTemp = db.MSemestertestsmappings.Where(x => x.Id.Equals(item.ChildTestId)).Select(w => w.SemesterId).FirstOrDefault();
                            var YearName = db5.MAcademicyeardetails.Where(x => x.Id.Equals(item.AcademicYearId)).Select(w => w.YearName).FirstOrDefault();
                            var SectionId = db5.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == item.AcademicYearId).Select(w => w.Standardsectionmappingid).FirstOrDefault();
                            var sectioname = db5.MStandardsectionmappings.Where(p => p.Id == SectionId).Select(w => w.Name).FirstOrDefault();
                            var Standardid = db5.MStandardsectionmappings.Where(p => p.Id == SectionId).Select(w => w.Parentid).FirstOrDefault();
                            var stdname = db5.MStandardsectionmappings.Where(p => p.Id == Standardid).Select(w => w.Name).FirstOrDefault();
                            var BranchId = db5.MBranches.Where(p => p.Schoolid == schoolid).Select(w => w.Id).FirstOrDefault();
                            var res = (from sym in db5.MSemesteryearmappings
                                       join stm in db5.MSemestertestsmappings on sym.SemesterId equals stm.Id
                                       where stm.BranchId == BranchId && sym.AcademicYearId == item.AcademicYearId && stm.Id == SemesterIDTemp
                                       select new { Id = stm.Id, Name = stm.Name }).Distinct().FirstOrDefault();
                            var result = Regex.Match(Exam, @"^([\w\-]+)");
                            stdRCM.YearSemesterExamArray[i] = YearName + " " + res.Name + " " + result + " " + "Gr" + stdname;
                        }
                        i++;
                    }
                    return (stdRCM);
                }
                else
                {

                    return (stdRCM);
                }
                return ("Student test mapping not exist");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // 15/11/2022 - Student Chart APIS
        public async Task<object> GetStudentsChartOverallPrecentageRank(int childid, int schoolid)
        {
            try
            {

                var stdRC = (from ctm in db.MOverallchildtests.OrderByDescending(m => m.AcademicYearId)
                             where ctm.ChildId == childid
                             select new { ChildTestId = ctm.TestId, ExamOverallPercentage = ctm.OverallPercentage, ChildAcademicYearId = ctm.AcademicYearId }).ToList();

                studentRChartCModel stdRCM = new studentRChartCModel();
                stdRCM.ChildID = childid;
                int? allranktotal = 0;
                if (stdRC.Count() > 0)
                {
                    int i = 0;
                    var arraysize = stdRC.Count();
                    stdRCM.OverallPercentageArray = new int?[arraysize];
                    stdRCM.YearSemesterExamArray = new string[arraysize];
                    stdRCM.OverallRankArray = new int?[arraysize];
                    stdRCM.OverallTotalMarksArray = new double?[arraysize];
                    stdRCM.OverallRankArrayWithTotalStudents = new string[arraysize];
                    stdRCM.ClassOverallPercentageArray = new int?[arraysize];
                    var childinformation = (from child1 in db.MChildinfos
                                            where child1.Id == childid
                                            select new { Childfirstname = child1.Firstname, Childlastname = child1.Lastname }).FirstOrDefault();
                    stdRCM.ChildName = childinformation.Childfirstname + " " + childinformation.Childlastname;

                    List<dataAStudent> studentData = new List<dataAStudent>();
                    var count = stdRC.Count();
                    int? totalclassprecentage = 0;


                    foreach (var item in stdRC)
                    {

                        var ChildSchoolInfovar = (from child2 in db.MChildschoolmappings
                                                  where child2.Childid == childid && child2.AcademicYearId == item.ChildAcademicYearId
                                                  select new { Sectionid = child2.Standardsectionmappingid }).FirstOrDefault();
                        var Sectionid = ChildSchoolInfovar.Sectionid;
                        var res1 = (from ctm in db.MChildtestmappings
                                    join stm in db.MSubjecttestmappings on ctm.SubjectTestMappingId equals stm.Id
                                    join sym in db.MSemesteryearmappings on stm.SemesterYearMappingId equals sym.Id
                                    join ssm in db.MSubjectsectionmappings on stm.SubjectSectionMappingId equals ssm.Id
                                    where stm.TestId == item.ChildTestId && ctm.ChildId == childid && ssm.SectionId == Sectionid && sym.AcademicYearId == item.ChildAcademicYearId
                                    select new { marks = ctm.Marks }).ToArray();
                        var stdRC2 = (from ctm in db.MOverallchildtests
                                      where ctm.TestId == item.ChildTestId && ctm.AcademicYearId == item.ChildAcademicYearId && ctm.SectionId == Sectionid
                                      select new { ChildTestId = ctm.TestId, ExamOverallPercentage = ctm.OverallPercentage, ChildAcademicYearId = ctm.AcademicYearId }).ToList();
                        if (stdRC2 != null && count > 0 && stdRC2.Count() > 0)
                        {
                            foreach (var item1 in stdRC2)
                            {
                                totalclassprecentage = totalclassprecentage + Convert.ToInt32(item1.ExamOverallPercentage);

                            }
                            stdRCM.ClassOverallPercentageArray[i] = totalclassprecentage / stdRC2.Count();
                            totalclassprecentage = 0;
                        }
                        //var res2 = (from ctm in db.ChildTestMappings
                        //            join stm in db.SubjectTestMappings on ctm.SubjectTestMappingId equals stm.Id
                        //            join sym in db.SemesterYearMappings on stm.SemesterYearMappingId equals sym.Id
                        //            where stm.TestId == item.ChildTestId && sym.AcademicYearId == item.ChildAcademicYearId
                        //            select new { marks = ctm.Marks }).ToArray();
                        //int res2count = res2.Count();
                        int res1count = res1.Count();
                        // stdRCM.ClassOverallTotalMarksArray = new double?[res2count];

                        if (stdRC != null && count > 0 && res1.Count() > 0)
                        {
                            var obmarks = res1.Sum(x => x.marks);
                            stdRCM.OverallTotalMarksArray[i] = obmarks;
                        }
                        //if (stdRC != null && count > 0 && res2.Count() > 0)
                        //{
                        //    var obmarks = res2.Sum(x => x.marks);
                        //    stdRCM.ClassOverallTotalMarksArray[i] = obmarks;
                        //}
                        var Exam = db.MSemestertestsmappings.Where(x => x.Id.Equals(item.ChildTestId)).Select(w => w.Name).FirstOrDefault();
                        var SemesterIDTemp = db.MSemestertestsmappings.Where(x => x.Id.Equals(item.ChildTestId)).Select(w => w.SemesterId).FirstOrDefault();
                        var YearName = db.MAcademicyeardetails.Where(x => x.Id.Equals(item.ChildAcademicYearId)).Select(w => w.YearName).FirstOrDefault();
                        var SectionId = db.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == item.ChildAcademicYearId).Select(w => w.Standardsectionmappingid).FirstOrDefault();
                        var sectioname = db.MStandardsectionmappings.Where(p => p.Id == SectionId).Select(w => w.Name).FirstOrDefault();
                        var Standardid = db.MStandardsectionmappings.Where(p => p.Id == SectionId).Select(w => w.Parentid).FirstOrDefault();
                        var stdname = db.MStandardsectionmappings.Where(p => p.Id == Standardid).Select(w => w.Name).FirstOrDefault();
                        var BranchId = db.MBranches.Where(p => p.Id == schoolid).Select(w => w.Id).FirstOrDefault();

                        //var SectionId = db.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == item.ChildAcademicYearId).Select(w => w.SectionId).FirstOrDefault();
                        //var stdid = db.Sections.Where(p => p.Id == SectionId).Select(w => w.StandardId).FirstOrDefault();
                        //var sectioname = db.Sections.Where(p => p.Id == SectionId).Select(w => w.Name).FirstOrDefault();
                        //var stdname = db.Standards.Where(p => p.Id == stdid).Select(w => w.Name).FirstOrDefault();
                        stdRCM.OverallPercentageArray[i] = Convert.ToInt32(item.ExamOverallPercentage);
                        stdRCM.OverallRankArray[i] = GetOverallStudentPositionOnly(item.ChildTestId, item.ExamOverallPercentage, SectionId, item.ChildAcademicYearId);
                        stdRCM.OverallRankArrayWithTotalStudents[i] = await this.GetOverallStudentPosition(item.ChildTestId, item.ExamOverallPercentage, (int)SectionId, item.ChildAcademicYearId);
                        stdRCM.OverallRankArrayGrade[i] = GetOverallStudentPositionOverallGradeRankOnly(item.ChildTestId, item.ExamOverallPercentage, item.ChildAcademicYearId);
                        stdRCM.OverallRankArrayGradeWithCount[i] = GetOverallStudentPositionOverallGrade(item.ChildTestId, item.ExamOverallPercentage, item.ChildAcademicYearId);
                        allranktotal = allranktotal + stdRCM.OverallRankArray[i];
                        var res = (from sym in db.MSemesteryearmappings
                                   join stm in db.MSemestertestsmappings on sym.SemesterId equals stm.Id
                                   where stm.BranchId == BranchId && sym.AcademicYearId == item.ChildAcademicYearId && stm.Id == SemesterIDTemp
                                   select new { Id = stm.Id, Name = stm.Name }).Distinct().FirstOrDefault();
                        var result = Regex.Match(Exam, @"^([\w\-]+)");
                        stdRCM.YearSemesterExamArray[i] = YearName + " " + res.Name + " " + result + " " + "Gr" + stdname;
                        i++;
                    }
                    stdRCM.OverallRankAverage = allranktotal / arraysize;

                    return stdRCM;
                }
                else
                {

                    return stdRCM;
                }
                return "Student test mapping not exist";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // Sub Functions Area

        //  28/3/2023 - Student Chart APIS
        public double GetStudentSubjectGradeAverage(int? subjectid, int? gradeid, int? testid)
        {
            try
            {
                int?[] GradeSubjectSumSectionMarksArray = new int?[0];
                int?[] GradeSubjectSumSectionMarksArrayCount = new int?[0];
                var sectionids = db.MStandardsectionmappings.Where(x => x.Parentid == gradeid).OrderByDescending(w => w.Id).Select(w => w.Id).ToArray();
                GradeSubjectSumSectionMarksArray = new int?[sectionids.Count()];
                GradeSubjectSumSectionMarksArrayCount = new int?[sectionids.Count()];
                int i = 0;
                foreach (var item in sectionids)
                {

                    int? markssum = 0;
                    var subjectsectionid = db.MSubjectsectionmappings.Where(x => x.SectionId == item && x.SubjectId == subjectid).OrderByDescending(w => w.Id).Select(w => w.Id).FirstOrDefault();

                    var MSubjecttestmappingid = db.MSubjecttestmappings.Where(x => x.SubjectSectionMappingId == subjectsectionid && x.TestId == testid).OrderByDescending(w => w.Id).Select(w => w.Id).FirstOrDefault();
                    var MChildsSubjectMarks = db.MChildtestmappings.Where(x => x.SubjectTestMappingId == MSubjecttestmappingid).OrderByDescending(w => w.Id).Select(w => w.Marks).ToArray();


                    foreach (var item2 in MChildsSubjectMarks)
                    {
                        markssum = item2 + markssum;
                    }
                    GradeSubjectSumSectionMarksArray[i] = markssum;
                    GradeSubjectSumSectionMarksArrayCount[i] = MChildsSubjectMarks.Count();
                    i++;

                }
                int? totalmarks = 0;
                int? totalstudnets = 0;
                int countarray = GradeSubjectSumSectionMarksArray.Count();
                for (int x = 0; x < countarray; x++)
                {
                    totalmarks = totalmarks + GradeSubjectSumSectionMarksArray[x];
                    totalstudnets = totalstudnets + GradeSubjectSumSectionMarksArrayCount[x];
                }
                if(totalstudnets <= 0 )
                {
                    totalstudnets = 1;
                }
                double AverageSubjectMarksByGrade = (double)totalmarks / (double)totalstudnets;
                if (AverageSubjectMarksByGrade != 0 && AverageSubjectMarksByGrade != double.NaN)
                {

                    return (AverageSubjectMarksByGrade);
                }
                else
                {
                    return (0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //  15/11/2022 - Student Chart APIS
        public double? getsubjectGradeprecentage(int? maxmarks, int? marks)
        {
            try
            {
                if (marks == 0 || marks < 0 || marks > maxmarks)
                {
                    return (0);
                }
                double? tempval = marks / (double)maxmarks;
                var testmarks = tempval * 100;
                return testmarks;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        // 15/11/2022 - Student Chart APIS
        public int GetStudentSubjectPositionOnly(int? subjecttestid, int? score)
        {
            try
            {
                if (score == null)
                {
                    return (0);
                }
                var res = db.MChildtestmappings.Where(x => x.SubjectTestMappingId.Equals(subjecttestid)).OrderByDescending(w => w.Id).Select(w => w.Marks).ToArray();
                // var res2 = db.MSubsubjectmarkss.Where(x => x.ChildTestMappingId.Equals(subjecttestid) && x.IsAbsent.Equals(false)).OrderByDescending(w => w.Marks).Select(w => w.Marks).ToArray();

                if (res != null)
                {
                    int totalStudents = res.Count();
                    var position = Array.FindIndex(res, x => x.Equals(score)) + 1;
                    if (position > 0)
                    {
                        return position;
                    }
                    return 0;
                }
                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //  15/11/2022 - Student Chart APIS
        public double? GetExamTotalStudentSubjectMarksClass(int? subjecttestid, int? score)
        {
            try
            {
                if (score == null)
                {
                    return (0);
                }
                var res = (from ctm in db.MChildtestmappings.OrderByDescending(m => m.Marks)
                           where ctm.SubjectTestMappingId == subjecttestid && ctm.IsAbsent.Equals(false)
                           select new { Marks = ctm.Marks }).ToList();
                int totalStudents = res.Count();

                // var res = db.ChildTestMappings.Where(x => x.SubjectTestMappingId.Equals(subjecttestid) && x.IsAbsent.Equals(false)).OrderByDescending(w => w.Marks).Select(w => w.Marks).ToList();
                if (res != null)
                {
                    int? totalStudentsSubjectMarks = 0;
                    foreach (var item in res)
                    {
                        double tempval = (double)item.Marks; // / (double)MaxMarks;
                        //var testmarks = tempval * 100;
                        totalStudentsSubjectMarks = totalStudentsSubjectMarks + (int)tempval;
                    }
                    //var position = Array.FindIndex(res, x => x.Equals(score)) + 1;
                    double? AveragetotalStudents = totalStudentsSubjectMarks / totalStudents;
                    if (AveragetotalStudents > 0)
                    {
                        return AveragetotalStudents;
                    }
                }
                return (0);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //  15/11/2022 - Student Chart APIS
        public int GetOverallStudentPositionOnly(int testid, double? marks, int? sectionid, int? academicYearId)
        {
            try
            {
                //var section = db.ChildTestMappings.
                var res = db.MOverallchildtests.Where(x => x.TestId == testid && x.SectionId == sectionid && x.AcademicYearId == academicYearId).OrderByDescending(w => w.OverallPercentage).Select(w => w.OverallPercentage).ToArray();
                if (res != null)
                {
                    int totalMarks = res.Count();
                    //Array.Sort(res);
                    //Array.Reverse(res);
                    var position = Array.FindIndex(res, x => x.Equals(marks)) + 1;
                    if (position > 0)
                    {
                        return position;
                    }
                    return 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //  15/11/2022 - Student Chart APIS
        public int GetOverallStudentPositionOverallGradeRankOnly(int testid, double? marks, int? academicYearId)
        {
            try
            {
                //var section = db.ChildTestMappings.
                var res = db.MOverallchildtests.Where(x => x.TestId == testid && x.AcademicYearId == academicYearId).OrderByDescending(w => w.OverallPercentage).Select(w => w.OverallPercentage).ToArray();
                if (res != null)
                {
                    int StudentsinGrade = res.Count();
                    //Array.Sort(res);
                    //Array.Reverse(res);
                    var position = Array.FindIndex(res, x => x.Equals(marks)) + 1;
                    if (position > 0)
                    {
                        return (position);
                    }
                    return (0);
                }
                return (0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // 15/11/2022 - Student Chart APIS
        public string GetOverallStudentPositionOverallGrade(int testid, double? marks, int? academicYearId)
        {
            try
            {
                //var section = db.ChildTestMappings.
                var res = db.MOverallchildtests.Where(x => x.TestId == testid && x.AcademicYearId == academicYearId).OrderByDescending(w => w.OverallPercentage).Select(w => w.OverallPercentage).ToArray();
                if (res != null)
                {
                    int StudentsinGrade = res.Count();
                    //Array.Sort(res);
                    //Array.Reverse(res);
                    var position = Array.FindIndex(res, x => x.Equals(marks)) + 1;
                    if (position > 0)
                    {
                        return (position + "/" + StudentsinGrade);
                    }
                    return ("N/A");
                }
                return ("N/A");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Sanduni Hiedu GetChildModulePosition
        public string GetHiEduStudentPosition(int? subjectexammappingid, decimal score)
        {
            try
            {
                // var res = await Task.FromResult(db4.MChildtestmappings.Where(x => x.SubjectTestMappingId.Equals(subjecttestid) && x.Percentage != null && x.IsAbsent == false).OrderByDescending(w => w.Marks).Select(w => w.Marks).ToArray());

                var res = db4.MHiEdu_CourseExamMarkss.Where(x => x.SubjectExamMappingId.Equals(subjectexammappingid)).OrderByDescending(w => w.Marks).Select(w => w.Marks).ToArray();
                if (res != null)
                {
                    int totalStudents = res.Count();
                    var position = Array.FindIndex(res, x => x.Equals(score)) + 1;
                    if (position > 0)
                    {
                        return (position + "/" + totalStudents);
                    }
                    return ("N/A");
                }
                return ("N/A");
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string GetStudentPosition(int? subjecttestid, int? score)
        {
            try
            {
                // var res = await Task.FromResult(db4.MChildtestmappings.Where(x => x.SubjectTestMappingId.Equals(subjecttestid) && x.Percentage != null && x.IsAbsent == false).OrderByDescending(w => w.Marks).Select(w => w.Marks).ToArray());

                var res = db4.MChildtestmappings.Where(x => x.SubjectTestMappingId.Equals(subjecttestid) && x.IsAbsent.Equals(false)).OrderByDescending(w => w.Marks).Select(w => w.Marks).ToArray();
                if (res != null && res.Length > 0)
                {
                    int totalStudents = res.Length;
                    int position = Array.FindIndex(res, x => x.Equals(score));
                    // int totalStudents = res.Count();
                    //var position = Array.FindIndex(res, x => x.Equals(score)) + 1;
                    if (position >= 0)
                    {
                        return (position + 1) + "/" + totalStudents;
                        //  return (position + "/" + totalStudents);
                    }
                }
                return ("N/A");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //sanduni subsubjectstudentposition 14/3/2023
        public async Task<string> GetSubSubjectStudentPosition(int? subjecttestid, int? score, int? subsubjectid)
        {
            try
            {
                if (score == null)
                {
                    return (null);
                }
                var myStringList = new int[10];
                int i = 0;
                var Id = db.MChildtestmappings.Where(x => x.SubjectTestMappingId.Equals(subjecttestid) && x.IsAbsent.Equals(false)).OrderByDescending(w => w.Id).Select(w => w.Id).ToList();
                foreach (var item in Id)
                {
                    myStringList[i] = db.MSubsubjectmarkss.Where(x => x.ChildTestMappingId.Equals(item) && x.SubSubjectId.Equals(subsubjectid)).OrderByDescending(w => w.Marks).Select(w => w.Marks).FirstOrDefault();
                    // var testData = await db2.MSemestertestsmappings.Where(x => x.Id == item).FirstOrDefaultAsync();
                    i++;
                }
                if (myStringList != null)
                {
                    int totalStudents = myStringList.Count();
                    var position = Array.FindIndex(myStringList, x => x.Equals(score)) + 1;
                    if (position == 0)
                    {

                    }
                    if (position > 0)
                    {
                        return (position + "/" + totalStudents);
                    }
                    return ("N/A");
                }
                return ("N/A");
            }
            catch (Exception)
            {

                throw;
            }
        }
        //sanduni - SubSubjectPositionOnly  14/3/2023
        public int GetStudentSubSubjectPositionOnly(int? subjecttestid, int? score, int? subsubjectid)
        {
            try
            {
                if (score == null)
                {
                    return (0);
                }
                var myStringList = new int[] { };
                int i = 0;
                var Id = db.MChildtestmappings.Where(x => x.SubjectTestMappingId.Equals(subjecttestid) && x.IsAbsent.Equals(false)).OrderByDescending(w => w.Id).Select(w => w.Id).ToList();
                foreach (var item in Id)
                {
                    myStringList[i] = db.MSubsubjectmarkss.Where(x => x.ChildTestMappingId.Equals(item) && x.SubSubjectId.Equals(subsubjectid)).OrderByDescending(w => w.Marks).Select(w => w.Marks).FirstOrDefault();
                    // var testData = await db2.MSemestertestsmappings.Where(x => x.Id == item).FirstOrDefaultAsync();
                    i++;
                }

                if (myStringList != null)
                {
                    int totalStudents = myStringList.Count();
                    var position = Array.FindIndex(myStringList, x => x.Equals(score)) + 1;
                    if (position > 0)
                    {
                        return position;
                    }
                    return 0;
                }
                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //sanduni - GetSubSubjectExamTotalStudentSubjectMarksClass  15/3/2023
        public double? GetSubSubjectExamTotalStudentSubjectMarksClass(int? subjecttestid, int? score, double MaxMarks, int? subsubjectid)
        {
            try
            {
                if (score == null)
                {
                    return (0);
                }
                var myStringList = new int[] { };
                int i = 0;
                var Id = db.MChildtestmappings.Where(x => x.SubjectTestMappingId.Equals(subjecttestid) && x.IsAbsent.Equals(false)).OrderByDescending(w => w.Id).Select(w => w.Id).ToList();
                foreach (var item in Id)
                {
                    myStringList[i] = db.MSubsubjectmarkss.Where(x => x.ChildTestMappingId.Equals(item) && x.SubSubjectId.Equals(subsubjectid)).OrderByDescending(w => w.Marks).Select(w => w.Marks).FirstOrDefault();
                    // var testData = await db2.MSemestertestsmappings.Where(x => x.Id == item).FirstOrDefaultAsync();
                    i++;
                }
                int totalStudents = myStringList.Count();

                // var res = db.ChildTestMappings.Where(x => x.SubjectTestMappingId.Equals(subjecttestid) && x.IsAbsent.Equals(false)).OrderByDescending(w => w.Marks).Select(w => w.Marks).ToList();
                if (myStringList != null)
                {
                    int? totalStudentsSubjectMarks = 0;
                    foreach (var item in myStringList)
                    {
                        double tempval = (double)item / (double)MaxMarks;
                        var testmarks = tempval * 100;
                        totalStudentsSubjectMarks = totalStudentsSubjectMarks + (int)testmarks;
                    }
                    //var position = Array.FindIndex(res, x => x.Equals(score)) + 1;
                    double? AveragetotalStudents = totalStudentsSubjectMarks / totalStudents;
                    if (AveragetotalStudents > 0)
                    {
                        return AveragetotalStudents;
                    }
                }
                return (0);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //DRC Freeze
        public class GetFreezeEnableModel
        {
            public int GradeId { get; set; }
            public string GradeName { get; set; }
            public int FreezeEnable { get; set; }

        }

        public async Task<List<GetFreezeEnableModel>> GetFreezeEnable(int academicYearId, int levelId, int semesterId, int examId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            List<GetFreezeEnableModel> freezeEnables = new List<GetFreezeEnableModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetFreezeEnable, connection);
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
                    command.Parameters.AddWithValue("@LevelId", levelId);
                    command.Parameters.AddWithValue("@semesterId", semesterId);
                    command.Parameters.AddWithValue("@examId", examId);





                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var freezeEnable = new GetFreezeEnableModel
                            {

                                FreezeEnable = (int)reader["FreezeEnable"],
                                GradeId = (int)reader["GradeId"],
                                GradeName = reader["GradeName"].ToString(),


                            };

                            freezeEnables.Add(freezeEnable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in GetFreezeEnable: {ex.Message}", ex);
            }

            return freezeEnables;
        }
        public async Task<object> AddFreezeEnable(AddFreezeEnableModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var freezeModel in model.GetFreezeEnableModels)
                    {
                        SqlCommand command = new SqlCommand(ApplicationConstants.AddFreezeEnable, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        //command.Parameters.Add(new SqlParameter("@AcademicYearId", SqlDbType.Int));
                        //command.Parameters["@AcademicYearId"].Value = model.AcademicYearId;

                        command.Parameters.AddWithValue("@AcademicYearId", model.AcademicYearId);
                        command.Parameters.AddWithValue("@LevelId", model.LevelId);
                        command.Parameters.AddWithValue("@GradeId", freezeModel.GradeId);
                        command.Parameters.AddWithValue("@FreezeEnable", freezeModel.FreezeEnable);
                        command.Parameters.AddWithValue("@SemesterId", freezeModel.SemesterId);
                        command.Parameters.AddWithValue("@ExamId", freezeModel.ExamId);


                        await command.ExecuteNonQueryAsync();
                    }
                }

                return "Freeze Enable Added Successfully";
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

        //Jaliya Overall student performance July 15

        public async Task<StudentPerformanceModel> GetStudentPerformanceSemester(int sectionId, int testId, int childId, int academicYearId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            StudentPerformanceModel studentPerformance = new StudentPerformanceModel();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(ApplicationConstants.GetAveragePerformanceForStudent, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@sectionId", sectionId);
                    if (testId != 0)
                        command.Parameters.AddWithValue("@testId", testId);
                    else
                        command.Parameters.AddWithValue("@testId", DBNull.Value);
                    command.Parameters.AddWithValue("@childId", childId);
                    command.Parameters.AddWithValue("@academicYear", academicYearId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            studentPerformance.TotalMarks = reader["TotalMarks"] is DBNull ? 0 : Convert.ToSingle(reader["TotalMarks"]);
                            studentPerformance.MaxMarks = reader["MaxMarks"] is DBNull ? 0 : Convert.ToSingle(reader["MaxMarks"]);
                            studentPerformance.ClassAverage = reader["ClassAverage"] is DBNull ? 0 : Convert.ToSingle(reader["ClassAverage"]);
                            studentPerformance.TopStudent = reader["TopStudent"] is DBNull ? 0 : Convert.ToSingle(reader["TopStudent"]);
                            studentPerformance.SemesterName = reader["SemesterName"] is DBNull ? string.Empty : reader["SemesterName"].ToString();
                            studentPerformance.OverallPercentage = reader["OverallPercentage"] is DBNull ? 0 : Convert.ToSingle(reader["OverallPercentage"]);
                            studentPerformance.OverallPosition = reader["OverallPosition"] is DBNull ? string.Empty : reader["OverallPosition"].ToString();
                            studentPerformance.OverallComments = reader["OverallComments"] is DBNull ? string.Empty : reader["OverallComments"].ToString();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetStudentPerformance: {ex.Message}", ex);
            }

            return studentPerformance;
        }
        #endregion
    }
}
