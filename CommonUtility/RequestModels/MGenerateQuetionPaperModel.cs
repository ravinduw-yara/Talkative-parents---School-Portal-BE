using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MGenerateQuetionPaperModel
    {
        public int Id { get; set; }
        public string PaperName { get; set; }
        public int AcademicYearFromId { get; set; }
        public int AcademicYearToId { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int SemesterId { get; set; }
        public int ExamId { get; set; }
        public DateTime? UploadedDate { get; set; }
        public string UploadedBy { get; set; }
        public string Content { get; set; }
        public string PDFContent { get; set; }
    }
    public class MGetGenerateQuetionPaperModel
    {
        public int Id { get; set; }
        public string PaperName { get; set; }
        public string AcademicYearFromName { get; set; }
        public string AcademicYearToName { get; set; }
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Content { get; set; }
        public string PDFContent { get; set; }

    }
    public class AddGenerateQuetionPaperModel
    {

        public string PaperName { get; set; }
        public int AcademicYearFromId { get; set; }
        public int AcademicYearToId { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int SemesterId { get; set; }
        public int ExamId { get; set; }
        public DateTime? UploadedDate { get; set; }
        public string UploadedBy { get; set; }
        public string Content { get; set; }
        public string PDFContent { get; set; }

    }
}
