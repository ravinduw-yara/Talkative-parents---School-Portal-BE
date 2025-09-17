using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommonUtility.RequestModels
{
    class DataAnalyticsModel
    {
        
    }
    public class ClassListSummaryDto
    {
        public string SectionName { get; set; }
        public string AdmissionNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }


    public class ClassListSummaryResponseDto
    {
        public string AcademicYearName { get; set; }
        public string GradeName { get; set; }
        public string SectionName { get; set; }
        public List<ClassListSummaryDto> Data { get; set; } = new();
    }
    public class ClassMarkSummaryDto
    {
        public int Position { get; set; }
        public string AdmissionNo { get; set; }
        public string FullName { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal AverageMarks { get; set; }
        public string OverallComments { get; set; }
        public Dictionary<string, decimal?> SubjectMarks { get; set; } = new();
    }

    public class ClassMarkSummaryResponseDto
    {
        public string AcademicYearName { get; set; }
        public string GradeSectionName { get; set; }
        public string TermName { get; set; }
        public List<ClassMarkSummaryDto> Data { get; set; } = new();
    }
    public class AddFreezeEnableModel
    {
        public int AcademicYearId { get; set; }
        public int LevelId { get; set; }
        public List<UpdateFreezeEnabledList> GetFreezeEnableModels { get; set; }
        // public int FreezeEnable { get; set; }
        //public int SemesterId { get; set; }
        // public int ExamId { get; set; }
        public AddFreezeEnableModel()
        {
            GetFreezeEnableModels = new List<UpdateFreezeEnabledList>();
        }
    }
    public class UpdateFreezeEnabledList
    {
        public int GradeId { get; set; }
        public int SemesterId { get; set; }
        public int ExamId { get; set; }
        public int FreezeEnable { get; set; }

    }
    //QI
    public class Prompt
    {
        public int Id { get; set; }
        public string PromptTypeName { get; set; }
        public string PromptContent { get; set; }
    }
    public class QueList
    {
        public int Id { get; set; }
        public string ContextQue { get; set; }
        public string ImageCode { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }
        public string Q3 { get; set; }
        public string Q4 { get; set; }
    }
    

    public class QuestionPaperData
    {
        public int Id { get; set; }
        public string PaperName { get; set; }
        public string Content { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public string AcademicYearName { get; set; }
        public int SemesterId { get; set; }
        public string SemesterName { get; set; }

        public int Prompttype1ID { get; set; }
        public string Prompttype1Name { get; set; }

        public string Prompttype1Format { get; set; }

        public int Prompttype2ID { get; set; }
        public string Prompttype2Name { get; set; }

        public string Prompttype2Format { get; set; }

        public int Prompttype3ID { get; set; }
        public string Prompttype3Name { get; set; }

        public string Prompttype3Format { get; set; }

        public int Prompttype4ID { get; set; }
        public string Prompttype4Name { get; set; }

        public string Prompttype4Format { get; set; }

        public int Prompttype5ID { get; set; }
        public string Prompttype5Name { get; set; }

        public string Prompttype5Format { get; set; }


    }
    
    public class GradeSectionMappingAddModel
    {
        public int SchoolId { get; set; }
        public string Grade { get; set; }
        public List<string> SectionNames { get; set; }
        public int Schooluserid { get; set; }

    }
    public class SubjectTestMappingAddModel
    {
        public int AcademicYearId { get; set; }
        public int SectionId { get; set; }
        public int SemesterId { get; set; }
        //public int SubjectId { get; set; }
        public List<int> SubjectIds { get; set; }
        public int TestId { get; set; }

    }
    //Jaliya Academic Year
    public class UpdateAcadamicYearModel
    {
        public int Id { get; set; }
        public string YearName { get; set; }
    }
    public class UpdateSemester
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class UpdateLevel
    {
        public int Id { get; set; }
        public string Level { get; set; }
    }
    public class UpdateGrade
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class UpdateSubSubjects
    {
        public int Id { get; set; }
        public string SubSubjectName { get; set; }
        public int Percentage { get; set; }
        public int SubMaxMarks { get; set; }

    }
    //Jaliya SubSubject Marks

    public class SubSubjectMarks
    {
        public string SubSubjectName { get; set; }
        public decimal Marks { get; set; }
    }
    //Jaliya - Student Grade Chart
    public class HiEduMarksGroup
    {
        public string Range { get; set; }
        public int StudentCount { get; set; }
    }
    //5/3/2023
    public class postGradeLeveModel
    {
        public int Levelid { get; set; }
        public List<SectionIdList> sectionidlist { get; set; }
        public string status { get; set; }

        public postGradeLeveModel()
        {
            sectionidlist = new List<SectionIdList>();
        }

    }

    public class SectionIdList
    {
        public int sectionid { get; set; }
    }
    public class postSubjectmapSectionModel
    {
        public int Schoolid { get; set; }
        public List<SectionIdList> sectionidlist { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BranchId { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int DrcSubjectOrder { get; set; }


        public postSubjectmapSectionModel()
        {
            sectionidlist = new List<SectionIdList>();
        }

    }
    public class ChildSubSubjectMarksFor9Semesters
    {
        public int? ChildTestId { get; set; }
        public double? Percentage { get; set; }
        public int? SubMaxMarks { get; set; }       
        public int? AcademicYearId { get; set; }
        public string Subject { get; set; }
        public string SubSubject { get; set; }
        public int? SubSubjectId { get; set; }
        public int? Marks { get; set; }
        public int? MaxMarks { get; set; }
        public int? SubjectTestMappingId { get; set; }
        public string Comment { get; set; }
        public int? ExamId { get; set; }
        

    }
    //26/3/2023 - SubjectGradePrecetage Two Subject
    public class SubjectGradePrecentage
    {
        public int? ChildId { get; set; }
        public int? Marks { get; set; }
        public int? TestId { get; set; }
        public int? SemesterId { get; set; }
        public int? SubjectId { get; set; }
        public int? GradeId { get; set; }
        public int? SubjectTestMappingId { get; set; }

    }
    public class GradeSubjectMarks
    {

        public int?[] GradeMarks { get; set; }
        public int? TestId { get; set; }
        public int? SemesterId { get; set; }
        public int? SubjectId { get; set; }
        public int? GradeId { get; set; }
        public int? SubjectTestMappingId { get; set; }
        public string[] YearSemesterExamArray { get; set; }
        public ICollection<grademarkslistinnermodel> grademarkslist { get; set; }
        public GradeSubjectMarks()
        {
            grademarkslist = new List<grademarkslistinnermodel>();
            YearSemesterExamArray = new string[] { };
        }
    }
    public class grademarkslistinnermodel
    {
        public double AGradePrecentage { get; set; }
        public double BGradePrecentage { get; set; }
        public double CGradePrecentage { get; set; }
        public double DGradePrecentage { get; set; }
        public grademarkslistinnermodel()
        {
    }
    }
    public class GradeMarksModel
    {
        public int? Marks { get; set; }
    }


    //15/11/2022 - Student Chart APIS
    public class studentRChartCModel
    {
        public string OverallPosition { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? StandardID { get; set; }
        public double? OverallPercentage { get; set; }
        public int ChildID { get; set; }
        public Guid? Year { get; set; }
        public string YearName { get; set; }
        public string Exam { get; set; }
        public Guid? ExamID { get; set; }
        public Guid? SemesterID { get; set; }
        public string SemesterName { get; set; }
        public string YearSemesterExam { get; set; }

        public string[] YearSemesterExamArray { get; set; }

        public int?[] OverallPercentageArray { get; set; }

        public int?[] ClassOverallPercentageArray { get; set; }

        public int?[] OverallRankArray { get; set; }

        public string[] OverallRankArrayWithTotalStudents { get; set; }

        public string[] SubjectRankArrayWithTotalStudents { get; set; }

        public double?[] OverallTotalMarksArray { get; set; }

        public double?[] ClassOverallTotalMarksArray { get; set; }

        public int?[] OverallSubjectRankArray { get; set; }

        public double?[] SubjectClassExamSubjectRankPrecentageArray { get; set; }

        public double?[] OverallSubjectMarksArray { get; set; }

        public double?[] OverallSubjectMarksGradeingArray { get; set; }
        public double? OverallMarksAverage { get; set; }

        public double?[] OverallRankAverageArray { get; set; }
        public int? OverallRankAverage { get; set; }
        public string ChildName { get; set; }

        public int?[] OverallRankArrayGrade { get; set; }

        public string[] OverallRankArrayGradeWithCount { get; set; }

        //subsubject arrays
        public string[] SubSubjectRankArrayWithStudentCount { get; set; }
        public int?[] SubSubjectRankArrayByChild { get; set; }
        public double?[] SubSubjectExamTotalStudentSubjectMarksAverageByClass { get; set; }

        public double?[] Subject1GradeMarksPrecentageArray { get; set; }
        public double?[] Subject2GradeMarksPrecentageArray { get; set; }
        public ICollection<studentRCSubSubjectInnerModel> studentRCSubSubjectInnerModels { get; set; }
        public studentRChartCModel()
        {
            YearSemesterExamArray = new string[] { };
            SubjectRankArrayWithTotalStudents = new string[] { };
            SubjectClassExamSubjectRankPrecentageArray = new double?[] { };
            Subject1GradeMarksPrecentageArray = new double?[] { };
            Subject2GradeMarksPrecentageArray = new double?[] { };
            studentRCSubSubjectInnerModels = new List<studentRCSubSubjectInnerModel>();
        }
    }
    public class studentRCSubSubjectInnerModel
    {
        public int? StudentSubSubjectsId { get; set; }
        public string StudentSubSubject { get; set; }

        public int? StudentSubSubjectsMark { get; set; }

    }
    
        public class HiEduStudentRCModel
    {
        public string OverallComments { get; set; }
        public string OverallPosition { get; set; }
        public decimal OverallPercentage { get; set; }
        [NotMapped]
        public ICollection<HiEduStudentRCInnerModel> HiEduStudentRCInnerModels { get; set; }

        public HiEduStudentRCModel()
        {
            HiEduStudentRCInnerModels = new List<HiEduStudentRCInnerModel>();

        }

    }
    public class HiEduStudentRCInnerModel
    {
        public int? ChildCourseExamMarksId { get; set; }
        public int? SubjectExamMappingId { get; set; }
        public string Subject { get; set; }
        public int? SubjectId { get; set; }
        public decimal Marks { get; set; }
        public int? MaxMarks { get; set; }
        public double? Percentage { get; set; }
        public string Comments { get; set; }
        public string Position { get; set; }
        // public bool? IsAbsent { get; set; }

        public int?[] SubSubjectsIds { get; set; }
        public string[] SubSubjects { get; set; }

        public int?[] SubSubjectsMarks { get; set; }

        public int?[] SubSubjectWeight { get; set; }
        public string[] SubSubjectComment { get; set; }
        public int?[] SubSubjectMaxMarks { get; set; }
        public int?[] SubSubjectAbsent { get; set; }
        public double GradeStudentsSubjectMarksAverage { get; set; }




        public HiEduStudentRCInnerModel()
        {
            SubSubjectsIds = new int?[] { };
            SubSubjects = new string[] { };
            SubSubjectsMarks = new int?[] { };
            //SubSubjectComment = new string[] { };
            SubSubjectMaxMarks = new int?[] { };


        }

    }
    public class studentRCModelSummery
    {
        public string OverallComments { get; set; }
        public string OverallCommentsSectionalHead { get; set; }
        public string OverallCommentsHeadmaster { get; set; }
        public string OverallPosition { get; set; }
        public double? OverallPercentage { get; set; }
        public int pdfmodel { get; set; }
        public int drcmodel { get; set; }
        public int isfrezeenabled { get; set; }
        public string SemesterName { get; set; }
        public string TestName { get; set; }


        [NotMapped]
        public ICollection<studentRCInnerModelSummery> studentRCInnerModelsSummery { get; set; }

        public studentRCModelSummery()
        {
            studentRCInnerModelsSummery = new List<studentRCInnerModelSummery>();

        }

    }
    public class studentRCInnerModelSummery
    {
        public int? ChildTestMappingId { get; set; }
        public string Subject { get; set; }
        public int? SubjectTestMappingId { get; set; }
        public int? Marks { get; set; }
        public int? MaxMarks { get; set; }
        public double? Percentage { get; set; }
        public string Comments { get; set; }
        public string Position { get; set; }
        public bool? IsAbsent { get; set; }

        public int?[] SubSubjectsIds { get; set; }
        public string[] SubSubjects { get; set; }

        public int?[] SubSubjectsMarks { get; set; }

        public int?[] SubSubjectWeight { get; set; }
        //public string[] SubSubjectComment { get; set; }
        public int?[] SubSubjectMaxMarks { get; set; }
        public int?[] SubSubjectAbsent { get; set; }
        public double GradeStudentsSubjectMarksAverage { get; set; }




        public studentRCInnerModelSummery()
        {
            SubSubjectsIds = new int?[] { };
            SubSubjects = new string[] { };
            SubSubjectsMarks = new int?[] { };
            //SubSubjectComment = new string[] { };
            SubSubjectMaxMarks = new int?[] { };


        }

    }


    public class studentRCModel
    {
        public string OverallComments { get; set; }
        public string OverallCommentsSectionalHead { get; set; }
        public string OverallCommentsHeadmaster { get; set; }
        public string OverallPosition { get; set; }
        public double? OverallPercentage { get; set; }
        public int pdfmodel { get; set; }
        public int drcmodel { get; set; }
        public int isfrezeenabled { get; set; }

        [NotMapped]
        public ICollection<studentRCInnerModel> studentRCInnerModels { get; set; }

        public studentRCModel()
        {
            studentRCInnerModels = new List<studentRCInnerModel>();

        }

    }

    public class studentRCInnerModel
    {
        public int? ChildTestMappingId { get; set; }
        public string Subject { get; set; }
        public int? SubjectTestMappingId { get; set; }
        public int? Marks { get; set; }
        public int? MaxMarks { get; set; }
        public double? Percentage { get; set; }
        public string Comments { get; set; }
        public string Position { get; set; }
        public bool? IsAbsent { get; set; }

        public int?[] SubSubjectsIds { get; set; }
        public string[] SubSubjects { get; set; }

        public int?[] SubSubjectsMarks { get; set; }
        
        public int?[] SubSubjectWeight { get; set; }
        //public string[] SubSubjectComment { get; set; }
        public int?[] SubSubjectMaxMarks { get; set; }
        public int?[] SubSubjectAbsent { get; set; }
        public double GradeStudentsSubjectMarksAverage { get; set; }




        public studentRCInnerModel()
        {
            SubSubjectsIds = new int?[] { };
            SubSubjects = new string[] { };
            SubSubjectsMarks = new int?[] { };
            //SubSubjectComment = new string[] { };
            SubSubjectMaxMarks = new int?[] { };


        }

    }




    public class postStudentRCModel
    {
        public string OverallComments { get; set; }
        public string OverallCommentsSectionalHead { get; set; }
        public string OverallCommentsHeadmaster { get; set; }
        public int? testid { get; set; }
        public int? childid { get; set; }
        public int? userid { get; set; }
        public int? AcademicYearId { get; set; }
        public List<studentsRCModel> studentsRCModels { get; set; }
        public postStudentRCModel()
        {
            studentsRCModels = new List<studentsRCModel>();
        }

    }
    //Sanduni Post HiEdu
    public class postHiEduStudentRCModel
    {
        public string OverallComments { get; set; }
        public int? testid { get; set; }
        public int? childid { get; set; }
        public int? userid { get; set; }
        
        public int? courseid { get; set; }
        public int? batchid { get; set; }
        public List<studentsHiEduRCModel> studentsHiEduRCModels { get; set; }
        public postHiEduStudentRCModel()
        {
            studentsHiEduRCModels = new List<studentsHiEduRCModel>();
        }

    }

    public class studentsHiEduRCModel
    {
        public int? ChildExamSubjectMarksId { get; set; }
        public int? SubjectTestMappingId { get; set; }
        public int? SubjectID { get; set; }
        public int? BatchID { get; set; }
        public decimal Marks { get; set; }
        //public int? MaxMarks { get; set; }
        public double? Percentage { get; set; }
        public string Comments { get; set; }
        public double? SubjectSemesterPercentage { get; set; }
        //public bool IsAbsent { get; set; }
        //update api sub subject arrays
        public int[] SubSubjectsIds { get; set; }
        public int[] SubSubjectsMarks { get; set; }
        public string[] SubSubjectComment { get; set; }
        public int[] SubAbsent { get; set; }

    }

    // HiEdu Charts
    public class SubjectMarks
    {
        public string SubjectName { get; set; }
        public decimal? Marks { get; set; }
    }
    public class SubjectAverageScore
    {
        public string Subject { get; set; }
        public decimal AverageScore { get; set; }
     }
     
public class studentsRCModel
    {
        public int? ChildTestMappingId { get; set; }
        public int? SubjectTestMappingId { get; set; }
        public int? Marks { get; set; }
        //public int? MaxMarks { get; set; }
        public double? Percentage { get; set; }
        public string Comments { get; set; }
        public double? SubjectSemesterPercentage { get; set; }
        //public bool IsAbsent { get; set; }
        //update api sub subject arrays
        public int[] SubSubjectsIds { get; set; }
        public int[] SubSubjectsMarks { get; set; }
        //public string[] SubSubjectComment { get; set; }
        public int[] SubAbsent { get; set; }

    }


    //public class dataAnalDropdown
    //{
    //    public List<dataAStudent> Students { get; set; }
    //    public List<dataASemester> Semesters { get; set; }
    //    public List<dataATest> Tests { get; set; }
    //    public List<dataASubjects> Subjects { get; set; }
    //}

    public class dataAStudent
    {
        public int? StudentId { get; set; }
        public string RegistrationNumber { get; set; }
        public string StudentName { get; set; }
        public int? Promoted { get; set; }
        public int? Statusid { get; set; } // 27/2/2024 Sanduni
        public int? Siblngstatus { get; set; }


    }

    public class dataASemester
    {
        public int? SemesterId { get; set; }
        public string SemesterName { get; set; }
    }

    public class dataATest
    {
        public int? TestId { get; set; }
        public string TestName { get; set; }
    }

    public class dataAYear
    {
        public int YearId { get; set; }
        public string AcademicYear { get; set; }
        public int? Currentyear { get; set; }
    }

    public class dataASubjects
    {
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
    }

    public class GetSubjectsSectionModel
    {
        public int ChildId { get; set; }
        public string Subject { get; set; }
        public double? Percentage { get; set; }

    }

    public class summaryConstraintModel
    {
        public string constraint { get; set; }
        public double percentage { get; set; }
    }

    public class TeachersConstraintModel
    {
        public string Semester { get; set; }
        public string Aconstraint { get; set; }
        public double? Apercentage { get; set; }
        public string Fconstraint { get; set; }
        public double? Fpercentage { get; set; }
        public double? average { get; set; }
    }

    public class studentsTabModel
    {
        public string semName { get; set; }
        public double? percentage { get; set; }
        public string position { get; set; }
    }

    public class SchoolDetailsForRCModel
    {
        public string SchoolName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int? Pincode { get; set; }
        public string PrimaryPhonenumber { get; set; }
        public string SecondaryPhonenumber { get; set; }
        public string WebsiteLink { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
        public string ChildName { get; set; }
        public string RegNo { get; set; }
        public List<usersList> Teachers { get; set; }

        public SchoolDetailsForRCModel()
        {
            Teachers = new List<usersList>();
        }
    }
    public class usersList
    {
        public string TeacherName { get; set; }
    }

    public class TeacherDetailsModel
    {
        public int teacherid { get; set; }
        public string teacherName { get; set; }
    }

    public class TeacherStdModel
    {
        public int teacherid { get; set; }
        public int stdid { get; set; }
        public string stdName { get; set; }
    }

    public class TeacherSectionModel
    {
        public int sectionid { get; set; }
        public string sectionName { get; set; }
    }

    public class AveragePerformancesModel
    {
        public double? obtainedMarks { get; set; }
        public double? maxMarks { get; set; }
        public double? ClassAverage { get; set; }
        public double? TopStudent { get; set; }
    }
    public class StudentPerformanceModel
    {
        public double TotalMarks { get; set; }
        public double MaxMarks { get; set; }
        public double ClassAverage { get; set; }
        public double TopStudent { get; set; }
        public string SemesterName { get; set; }
        public float OverallPercentage { get; set; }
        public string OverallPosition { get; set; }
        public string OverallComments { get; set; }
    }
}
