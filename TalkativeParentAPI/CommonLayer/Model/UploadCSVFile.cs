using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace UploaderSheet_StudentMark.CommonLayer.Model
{

    //Student Mark Bulk Uploder Jaliya 
    public class UploadCSVFileRequest
    {
        public int AcademicYearId { get; set; }
        public int LevelId { get; set; }
        public int StandardId { get; set; }
        public int SectionId { get; set; }
        public int SemesterId { get; set; }
        public int SubjectId { get; set; }
        public int ExameId { get; set; }
        public IFormFile File { get; set; }
    }



    //Student Register Bulk Uploder Jaliya 07/09/2023
    public class StudentRegistrationUploadCSVFileRequest
    {
        public int AcademicYearId { get; set; }
        public int SchoolId { get; set; }

        public IFormFile File { get; set; }
        //public int AcademicYearId { get; set; }
        //public int LevelId { get; set; }
        //public int StandardId { get; set; }
        //public int SectionId { get; set; }
        //public IFormFile File { get; set; }

    }
    //24/7/2024
    public class UploadCSVFileResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<StudentUploadStatus> UploadStatuses { get; set; } = new List<StudentUploadStatus>();
    }
}
