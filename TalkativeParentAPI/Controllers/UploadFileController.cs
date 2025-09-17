using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using UploaderSheet_StudentMark.CommonLayer.Model;
using UploaderSheet_StudentMark.DataAccessLayer;
using OfficeOpenXml;
namespace UploaderSheet_StudentMark.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        public readonly IUploadFileDL _uploadFileDL;
        public UploadFileController(IUploadFileDL uploadFileDL)
        {
            _uploadFileDL = uploadFileDL;
        }

        #region UploaderSheet 

        [Route("StudentTeacherCommentUploadFile")]
        [HttpPost]
        public async Task<IActionResult> StudentTeacherCommentUploadFile([FromForm] StudentTeacherCommentUploadXMLFileRequest request)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();

            try
            {
                response = await _uploadFileDL.StudentTeacherCommentUploadFile(request, request.File.OpenReadStream());
                Console.WriteLine($"Debug: API Response -> IsSuccess: {response.IsSuccess}, Message: {response.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Debug: Exception: {ex.Message}");
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [Route("UploadExcelFile")]
        [HttpPost]
        public async Task<IActionResult> UploadExcelFile([FromForm] UploadXMLFileRequest request)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();

            try
            {
                response = await _uploadFileDL.UploadXMLFile(request, request.File.OpenReadStream());
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [Route("UploadCSVFile")]
        [HttpPost]
        public async Task<IActionResult> UploadCSVFile([FromForm] UploadCSVFileRequest request)
        {
            UploadCSVFileResponse response = new UploadCSVFileResponse();

            try
            {
                response = await _uploadFileDL.UploadCSVFile(request, request.File.OpenReadStream());
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        #endregion

        #region StudentRegisterUploaderSheet 
        //30/3/2025 subjectmarksnewupload
        [Route("SubjectMarkUploadFile")]
        [HttpPost]
        public async Task<IActionResult> SubjectMarkUploadFile([FromForm] NewUploadXMLFileRequest request)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();

            try
            {
                response = await _uploadFileDL.SubjectUploadFile(request, request.File.OpenReadStream());
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }


        [Route("SubjectwithSubsubjectUploadFile")]
        [HttpPost]
        public async Task<IActionResult> SubjectwithSubsubjectUploadFile([FromForm] NewUploadXMLFileRequest request)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();

            try
            {
                response = await _uploadFileDL.SubjectwithSubsubjectUploadFile(request, request.File.OpenReadStream());
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }
        //ebdsubjectupload
        [Route("StudentRegisterUploadXMLFile")]
        [HttpPost]
        public async Task<IActionResult> StudentRegisterUploadXMLFile([FromForm] StudentRegistrationUploadXMLFileRequest request)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();

            try
            {
                response = await _uploadFileDL.StudentRegisterUploadXMLFile(request, request.File.OpenReadStream());
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [Route("StudentRegisterUploadCSVFile")]
        [HttpPost]
        public async Task<IActionResult> StudentRegisterUploadCSVFile([FromForm] StudentRegistrationUploadCSVFileRequest request)
        {
            UploadCSVFileResponse response = new UploadCSVFileResponse();

            try
            {
                response = await _uploadFileDL.StudentRegisterUploadCSVFile(request, request.File.OpenReadStream());
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

    

        //Jaliya 24/7/2023

      [Route("DownloadUploadStatus")]
      [HttpPost]
      public IActionResult DownloadUploadStatus([FromBody] UploadXMLFileResponse response)
       {
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Upload Status");


            worksheet.Cells[1, 1].Value = "Code";
            worksheet.Cells[1, 2].Value = "StudentFirstName";
            worksheet.Cells[1, 3].Value = "StudentLastName";
            worksheet.Cells[1, 4].Value = "Status";
            worksheet.Cells[1, 5].Value = "Message";

            for (int i = 0; i < response.UploadStatuses.Count; i++)
            {
                var status = response.UploadStatuses[i];
                worksheet.Cells[i + 2, 1].Value = status.Code;
                worksheet.Cells[i + 2, 2].Value = status.StudentFirstName;
                worksheet.Cells[i + 2, 3].Value = status.StudentLastName;
                worksheet.Cells[i + 2, 4].Value = status.IsSuccess ? "Success" : "Failed";
                worksheet.Cells[i + 2, 5].Value = status.Message;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            string excelName = $"UploadStatus-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }



    #endregion


}
}