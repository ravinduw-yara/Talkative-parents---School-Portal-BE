using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public partial class MSyllabus
    {
        public MSyllabus()
        {

        }

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
}

