using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class SyllabusData
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
    }
    public class MGetSyllabusModel
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
        public string PDFContent { get; set; }

    }

    public class MGetOneSyllabusModel
    {

        public string PDFContent { get; set; }

    }
    public class MSyllabusModel
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

    }
    public class MSyllabusUpdateModel : MSyllabusModel
    {
        public int Id { get; set; }
    }

    public class SyllabusUploadModel
    {
        public IFormFile PdfFile { get; set; }
        public int AcademicYearId { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int SemesterId { get; set; }
        public int ExamId { get; set; }
        public string UploadedBy { get; set; }
        public string Content { get; set; }
    }
    public class SyllabusBulkDeleteModel
    {
        public List<Syllabus> Syllabuss { get; set; }
    }

    public class Syllabus
    {
        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int SemesterId { get; set; }
        public int ExamId { get; set; }
    }
}
