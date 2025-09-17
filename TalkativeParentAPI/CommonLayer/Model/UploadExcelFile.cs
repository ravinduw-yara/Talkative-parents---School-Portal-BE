using Microsoft.AspNetCore.Http;
//using Microsoft.Kiota.Abstractions;
using System;
using System.Collections.Generic;

namespace UploaderSheet_StudentMark.CommonLayer.Model
{
    ////start upload teacher comment

    public class StudentTeacherCommentUploadXMLFileRequest
    {

        public int AcademicYearId { get; set; }
        public int LevelId { get; set; }
        public int SemesterId { get; set; }
        public int ExameId { get; set; }
        public IFormFile File { get; set; }

    }
    public class StudentTeacherCommentParameter
    {
        public string RegistrationId { get; set; }
        public int AcademicYearId { get; set; }
        public int LevelId { get; set; }
        public string OverallComments { get; set; }
        public string OverallCommentstwo { get; set; }
        public string OverallCommenthree { get; set; }
        public string StandardName { get; set; }
        public string SectionName { get; set; }

        public int SemesterId { get; set; }
        public int ExameId { get; set; }

    }
//end upload teacher comment
    public class NewUploadXMLFileRequest
    {
        public int AcademicYearId { get; set; }
        public int LevelId { get; set; }

        public int SemesterId { get; set; }

        public int ExameId { get; set; }
        public IFormFile File { get; set; }

    }

    public class NewExcelBulkUploadParameter
    {
        public string RegistrationId { get; set; }

        public string Grade { get; set; }

        public string Section { get; set; }

        public int? SubjectId { get; set; }

        public string SubjectName { get; set; }

        public float? Thoery { get; set; }
        public float? Practical { get; set; }
        public float? SubSubjectMarks3 { get; set; }
        public float? SubSubjectMarks4 { get; set; }
        public float? SubSubjectMarks5 { get; set; }
        public string MainSubjectComment { get; set; }


    }
    //Student Mark Bulk Uploder Jaliya 
    public class UploadXMLFileRequest
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

    public class UploadXMLFileResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<StudentUploadStatus> UploadStatuses { get; set; } = new List<StudentUploadStatus>();
    }

    //24/7/2024
    public class StudentUploadStatus
    {
        public string Code { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
   
    //Student Mark Bulk Uploder Jaliya 
    public class ExcelBulkUploadParameter
    {
        public string RegistrationId { get; set; }
        public float Thoery { get; set; }
        public float Practical { get; set; }
        public float SubSubjectMarks3 { get; set; }
        public float SubSubjectMarks4 { get; set; }
        public float SubSubjectMarks5 { get; set; }
        public string MainSubjectComment { get; set; }
    }

    //Student Register Bulk Uploder Jaliya 07/09/2023
    public class StudentRegistrationUploadXMLFileRequest
    {//21/3/2025
        public int AcademicYearId { get; set; }
        public int SchoolId { get; set; }

        public IFormFile File { get; set; }
        //public int AcademicYearId { get; set; }
        //public int LevelId { get; set; }
        //public int StandardId { get; set; }
        //public int SectionId { get; set; }
        //public IFormFile File { get; set; }

    }

    //Student Register Bulk Uploder Jaliya 07/09/2023
    public class StudentRegistrationParameter
    {
        public string Code { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string StudentMiddleName { get; set; }

        public string StandardName { get; set; }
        public string SectionName { get; set; }
        public DateTime? StudentDob { get; set; }
        public int? StudentGenderId { get; set; }
        public string StudentNic { get; set; }
        public string StudentEmail { get; set; }
        public int? StudentIsHieduuser { get; set; }
        public int? StudentHieduActive { get; set; }

        public string MotherFirstName { get; set; }
        public string MotherLastName { get; set; }
        public string MotherMiddleName { get; set; }
        public string MotherPhoneNumber { get; set; }
        public int? MotherGenderId { get; set; }
        public string MotherEmail { get; set; }
        public int? MotherIssmsuser { get; set; }
        public int? MotherIshigheduser { get; set; }

        public string FatherFirstName { get; set; }
        public string FatherLastName { get; set; }
        public string FatherMiddleName { get; set; }
        public string FatherPhoneNumber { get; set; }
        public int? FatherGenderId { get; set; }
        public string FatherEmail { get; set; }
        public int? FatherIssmsuser { get; set; }
        public int? FatherIshigheduser { get; set; }
        //public string Code { get; set; }
        //public string StudentFirstName { get; set; }
        //public string StudentLastName { get; set; }
        //public string StudentMiddleName { get; set; }
        //public DateTime? StudentDob { get; set; }
        //public int? StudentGenderId { get; set; }
        //public string StudentNic { get; set; }
        //public string StudentEmail { get; set; }
        //public int? StudentIsHieduuser { get; set; }
        //public int? StudentHieduActive { get; set; }

        //public string MotherFirstName { get; set; }
        //public string MotherLastName { get; set; }
        //public string MotherMiddleName { get; set; }
        //public string MotherPhoneNumber { get; set; }
        //public int? MotherGenderId { get; set; }
        //public string MotherEmail { get; set; }
        //public int? MotherIssmsuser { get; set; }
        //public int? MotherIshigheduser { get; set; }

        //public string FatherFirstName { get; set; }
        //public string FatherLastName { get; set; }
        //public string FatherMiddleName { get; set; }
        //public string FatherPhoneNumber { get; set; }
        //public int? FatherGenderId { get; set; }
        //public string FatherEmail { get; set; }
        //public int? FatherIssmsuser { get; set; }
        //public int? FatherIshigheduser { get; set; }


    }



}

