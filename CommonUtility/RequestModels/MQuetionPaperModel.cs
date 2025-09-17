using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{

    public class MGetQuetionPaperModel
    {
        public int Id { get; set; }
        public string PaperName { get; set; }
        public int AcademicYearId { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int SemesterId { get; set; }
        public int ExamId { get; set; }
        public DateTime? UploadedDate { get; set; }
        public string UploadedBy { get; set; }
        public string Content { get; set; }
        //public string PDFContent { get; set; }
        public int QuestionPaperTypeId { get; set; }


    }
    public class MQuetionPaperModel
    {

        public string PaperName { get; set; }
        public int AcademicYearId { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int SemesterId { get; set; }
        public int ExamId { get; set; }
        public DateTime? UploadedDate { get; set; }
        public string UploadedBy { get; set; }
        public string Content { get; set; }
        public string PDFContent { get; set; }

        public int QuestionPaperTypeId { get; set; }

    }
    public class MQuetionPaperUpdateModel : MQuetionPaperModel
    {
        public int Id { get; set; }
    }
    public class MGetOneQuetionPaperModel
    {

        public string PDFContent { get; set; }

    }
    public class QuestionPaperUploadModel
    {
        public IFormFile PdfFile { get; set; }
        public int AcademicYearId { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int SemesterId { get; set; }
        public int ExamId { get; set; }
        public string UploadedBy { get; set; }
        public string Content { get; set; }

        public int QuestionPaperTypeId { get; set; }
    }
    public class QuestionPaperBulkDeleteModel
    {
        public List<QuestionPaper> Papers { get; set; }
    }

    public class QuestionPaper
    {
        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int SemesterId { get; set; }
        public int ExamId { get; set; }

        public int QuestionPaperTypeId { get; set; }
    }
}
