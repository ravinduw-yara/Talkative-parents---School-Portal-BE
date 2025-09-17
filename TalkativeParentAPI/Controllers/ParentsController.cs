using CommonUtility.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Services.MSchooluserroleService;
using System.Configuration;
using System.Data.Entity;
using System.Text.Json;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ParentsController : ControllerBase
    {
        readonly TpContext db = new TpContext();
        private readonly IMSchooluserinfoService mSchooluserinfoService;
        private readonly IMSchooluserroleService mSchooluserrole;
        private readonly IMChildinfoService mChildinfoService;
        private readonly IMChildschoolmappingService mChildschoolmappingService;
        private readonly IMParentchildmappingService mParentchildmappingService;
        private readonly IMAppuserinfoService mAppuserinfoService;
        private readonly IMStandardsectionmappingService standardsectionmappingService;
        private readonly GoogleDriveService _googleDriveService;
        private readonly IMQuetionPaperService _quetionPaperService;
        private readonly IConfiguration _configuration;
        private readonly MUploadPdfGoogleDriveService _mUploadPdfGoogleDriveService;
        private readonly MUploadPdfSyllabusGoogleDriveService _mUploadPdfSyllabusGoogleDriveService;
        private readonly IMSyllabusService mSyllabusService;
        private readonly IDataAnalysisServices dataAnalysisServices;
        public ParentsController(
            IMSchooluserinfoService _mSchooluserinfoService,
            IMChildinfoService _mChildinfoService,
            IMSchooluserroleService _mSchooluserrole,
            IMChildschoolmappingService _mChildschoolmappingService,
            IMParentchildmappingService _mParentchildmappingService,
            IMAppuserinfoService _mAppuserinfoService,
            IMStandardsectionmappingService _standardsectionmappingService,
            GoogleDriveService googleDriveService,
            IMQuetionPaperService quetionPaperService,
            IConfiguration configuration,
            MUploadPdfGoogleDriveService mUploadPdfGoogleDriveService,
            MUploadPdfSyllabusGoogleDriveService mUploadPdfSyllabusGoogleDriveService,
            IMSyllabusService _mSyllabusService,
            IDataAnalysisServices _dataAnalysisServices
            )
        {
            mSchooluserinfoService = _mSchooluserinfoService;
            mChildinfoService = _mChildinfoService;
            mSchooluserrole = _mSchooluserrole;
            mChildschoolmappingService = _mChildschoolmappingService;
            mParentchildmappingService = _mParentchildmappingService;
            mAppuserinfoService = _mAppuserinfoService;
            standardsectionmappingService = _standardsectionmappingService;
            _googleDriveService = googleDriveService;
            _quetionPaperService = quetionPaperService;
            _configuration = configuration;
            _mUploadPdfGoogleDriveService = mUploadPdfGoogleDriveService;
            dataAnalysisServices = _dataAnalysisServices;
            _mUploadPdfSyllabusGoogleDriveService = mUploadPdfSyllabusGoogleDriveService;
            mSyllabusService = _mSyllabusService;
        }

        //[Route("AddAssignChildSport")]
        //[HttpPost]
        //public async Task<IActionResult> AddAssignChildSport([FromBody] AssignChildSportModel model)
        //{
        //    try
        //    {
        //        var result = await this.mChildinfoService.AddAssignChildSportAsync(model);
        //        return Ok(new
        //        {
        //            Message = result,
        //            StatusCode = HttpStatusCode.OK
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new
        //        {
        //            Data = ex.Message,
        //            StatusCode = HttpStatusCode.InternalServerError
        //        });
        //    }
        //}

        //[Route("UpdateAssignChildSport")]
        //[HttpPut]
        //public async Task<IActionResult> UpdateAssignChildSport([FromBody] UpdateAssignChildSportModel model)
        //{
        //    try
        //    {
        //        var result = await this.mChildinfoService.UpdateAssignChildSportAsync(model);
        //        return Ok(new
        //        {
        //            Message = result,
        //            StatusCode = HttpStatusCode.OK
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new
        //        {
        //            Data = ex.Message,
        //            StatusCode = HttpStatusCode.InternalServerError
        //        });
        //    }
        //}

        //[Route("DeleteAssignChildSport")]
        //[HttpDelete]
        //public async Task<IActionResult> DeleteAssignChildSport(int id)
        //{
        //    try
        //    {
        //        var result = await this.mChildinfoService.DeleteAssignChildSportAsync(id);
        //        return Ok(new
        //        {
        //            Message = result,
        //            StatusCode = HttpStatusCode.OK
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new
        //        {
        //            Data = ex.Message,
        //            StatusCode = HttpStatusCode.InternalServerError
        //        });
        //    }
        //}

        //[Route("GetAssignChildSportById")]
        //[HttpGet]
        //public async Task<IActionResult> GetAssignChildSportById(int id)
        //{
        //    try
        //    {
        //        var result = await this.mChildinfoService.GetAssignChildSportByIdAsync(id);
        //        return Ok(new
        //        {
        //            Data = result,
        //            StatusCode = HttpStatusCode.OK
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new
        //        {
        //            Data = ex.Message,
        //            StatusCode = HttpStatusCode.InternalServerError
        //        });
        //    }
        //}

        //[Route("GetAssignedChildSports")]
        //[HttpGet]
        //public async Task<IActionResult> GetAssignedChildSports([FromQuery] int schoolId, [FromQuery] int? sportId, [FromQuery] int? sportGroupId, [FromQuery] int? levelId, [FromQuery] string? searchName)
        //{
        //    try
        //    {
        //        var result = await this.mChildinfoService.GetAssignedChildSportsAsync(schoolId, sportId, sportGroupId, levelId, searchName);
        //        return Ok(new
        //        {
        //            Data = result,
        //            StatusCode = HttpStatusCode.OK
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new
        //        {
        //            Data = ex.Message,
        //            StatusCode = HttpStatusCode.InternalServerError
        //        });
        //    }
        //}

        [Route("DeleteParentDetails")]
        [HttpDelete]
        public async Task<IActionResult> DeleteParentDetails(int schoolId, int childId, int parentId)
        {
            try
            {
                var res = await this.mChildinfoService.DeleteParentDetails(schoolId, childId, parentId);
                if (res == null)
                {
                    return BadRequest("Failed to delete the records");
                }
                return new JsonResult(new
                {
                    Value = res,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [HttpGet]
        [Route("GetPromptByPromptId")]
        public async Task<IActionResult> GetPromptByPromptId(int promptId)
        {
            try
            {
                var prompt = await _quetionPaperService.GetPromptByPromptId(promptId);
                if (prompt == null)
                {
                    return NotFound("Record not found.");
                }

                return Ok(new { Value = prompt, StatusCode = HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in GetPromptByPromptId: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Data = ex.Message });
            }
        }
        [HttpPost]
        [Route("UpdatePromptForSchool")]
        public async Task<IActionResult> UpdatePromptForSchool([FromBody] UpdatePromptModel model)
        {
            try
            {
                var updatePromptModel = new UpdatePromptModel
                {
                    SchoolId = model.SchoolId,
                    PromptTypeId1 = model.PromptTypeId1,
                    PromptType1 = model.PromptType1,
                    PromptTypeFormat1 = model.PromptTypeFormat1,
                    PromptTypeId2 = model.PromptTypeId2,
                    PromptType2 = model.PromptType2,
                    PromptTypeFormat2 = model.PromptTypeFormat2,
                    PromptTypeId3 = model.PromptTypeId3,
                    PromptType3 = model.PromptType3,
                    PromptTypeFormat3 = model.PromptTypeFormat3,
                    PromptTypeId4 = model.PromptTypeId4,
                    PromptType4 = model.PromptType4,
                    PromptTypeFormat4 = model.PromptTypeFormat4,
                    PromptTypeId5 = model.PromptTypeId5,
                    PromptType5 = model.PromptType5,
                    PromptTypeFormat5 = model.PromptTypeFormat5
                };

                var result = await _quetionPaperService.UpdatePromptsForSchool(updatePromptModel);

                if (result == null)
                {
                    return BadRequest("Error while updating prompts for school.");
                }

                return Ok(new
                {
                    Value = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpGet]
        [Route("GetPromptsBySchoolId")]
        public async Task<IActionResult> GetPromptsBySchoolId(int schoolId)
        {
            try
            {
                var prompts = await _quetionPaperService.GetPromptsBySchoolId(schoolId);
                if (prompts == null || !prompts.Any())
                {
                    return NotFound("Records not found.");
                }

                return Ok(new { Value = prompts, StatusCode = HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in GetPromptsBySchoolId: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Data = ex.Message });
            }
        }

        [HttpPost]
        [Route("AddPromptToSchool")]
        public async Task<IActionResult> AddPromptToSchool([FromBody] AddPromptModel model)
        {
            try
            {
                var addPromptModel = new AddPromptModel
                {
                    SchoolId = model.SchoolId,
                    PromptType1 = model.PromptType1,
                    PromptTypeFormat1 = model.PromptTypeFormat1,
                    PromptType2 = model.PromptType2,
                    PromptTypeFormat2 = model.PromptTypeFormat2,
                    PromptType3 = model.PromptType3,
                    PromptTypeFormat3 = model.PromptTypeFormat3,
                    PromptType4 = model.PromptType4,
                    PromptTypeFormat4 = model.PromptTypeFormat4,
                    PromptType5 = model.PromptType5,
                    PromptTypeFormat5 = model.PromptTypeFormat5
                };

                var result = await _quetionPaperService.AddPromptToSchool(addPromptModel);

                if (result == null)
                {
                    return BadRequest("Error while adding prompts to school.");
                }

                return Ok(new
                {
                    Value = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        [Route("GetParentsList")]
           [HttpGet]
          public async Task<IActionResult> GetParentsList(Guid token, int nuofRows = 10, int pageNumber = 1, string standard = "", string section = "", string childlastname = "", string searchstring = "")
           {
               try
               {
                   var res = await this.mChildinfoService.GetParentsListForAPI(token, nuofRows, pageNumber, standard, section, childlastname, searchstring);
                   if (res == null)
                   {
                       return null;
                   }
                   return Ok(res);
               }
               catch (Exception ex)
               {

                   return new JsonResult(new
                   {
                       Data = ex.Message,
                   });
               }
           }
        [Route("GetParentsListStudentView")]
        [HttpGet]
        public async Task<IActionResult> GetParentsListStudentView(Guid token, int academicyearid, int activestatus = 1, int nuofRows = 10, int pageNumber = 1, string standard = "", string section = "", string childlastname = "", string searchstring = "")
        {
            try
            {
                var res = await this.mChildinfoService.GetParentsListStudentVIewSP(token, academicyearid, activestatus, nuofRows, pageNumber, standard, section, childlastname, searchstring);
                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {

                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [HttpPost]
        [Route("UpdateBulkStudentInactiveStatus")]
        public async Task<IActionResult> UpdateBulkStudentInactiveStatus(ChildUpdateInactiveModel model)
        {
            try
            {
                var result = await mChildinfoService.UpdateBulkStudentInactiveStatus(model);
                if (result == null)
                {
                    return BadRequest("Unable to update records");
                }
                return new JsonResult(new
                {
                    Value = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }
        [Route("DeleteQuestionPaper")]
        [HttpDelete]
        public async Task<IActionResult> DeleteQuestionPaper(int paperId)
        {
            try
            {
                var res = await this._quetionPaperService.DeleteQuestionPaper(paperId);
                if (res == null)
                {
                    return BadRequest("Failed to delete the records");
                }
                return new JsonResult(new
                {
                    Value = res,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [HttpGet]
        [Route("GetGeneratedSyllabus")]
        public async Task<IActionResult> GetGeneratedSyllabus(int academicYearFromId, int academicYearToId, int gradeId, int subjectId)
        {
            try
            {
                var result = await mSyllabusService.GetGeneratedSyllabus(academicYearFromId, academicYearToId, gradeId, subjectId);

                if (result == null || !result.Any())
                {
                    return NotFound("No Syllabus found for the given criteria.");
                }

                return Ok(new
                {
                    Values = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    Data = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("GetSyllabus")]
        public async Task<IActionResult> GetSyllabus(int academicYearId, int gradeId, int subjectId, int semesterId, int examId)
        {
            try
            {
                var syllabus = await mSyllabusService.GetSyllabus(academicYearId, gradeId, subjectId, semesterId, examId);
                if (syllabus == null || !syllabus.Any())
                {
                    return NotFound("Records not found.");
                }

                return Ok(new { Value = syllabus, StatusCode = HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in GetSyllabus: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Data = ex.Message });
            }
        }

        [HttpPost]
        [Route("AddSyllabus")]
        public async Task<IActionResult> AddSyllabus([FromForm] SyllabusUploadModel uploadModel)
        {
            try
            {
                if (uploadModel.PdfFile == null || uploadModel.PdfFile.Length == 0 || !uploadModel.PdfFile.FileName.ToLower().EndsWith(".pdf"))
                {
                    return BadRequest(new { Message = "Invalid PDF file provided." });
                }

                var fileId = _mUploadPdfSyllabusGoogleDriveService.UploadFile(uploadModel.PdfFile,
                                                               uploadModel.AcademicYearId,
                                                               uploadModel.GradeId,
                                                               uploadModel.SubjectId,
                                                               uploadModel.SemesterId,
                                                               uploadModel.ExamId);
                if (string.IsNullOrEmpty(fileId))
                {
                    return BadRequest(new { Message = "Failed to upload PDF to Google Drive." });
                }

                // Convert PDF to Base64
                string base64Content;
                using (var memoryStream = new MemoryStream())
                {
                    await uploadModel.PdfFile.CopyToAsync(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();
                    base64Content = Convert.ToBase64String(fileBytes);
                }

                string paperName = Path.GetFileNameWithoutExtension(uploadModel.PdfFile.FileName);

                var model = new MSyllabusModel
                {
                    PaperName = paperName,
                    AcademicYearId = uploadModel.AcademicYearId,
                    GradeId = uploadModel.GradeId,
                    SubjectId = uploadModel.SubjectId,
                    SemesterId = uploadModel.SemesterId,
                    ExamId = uploadModel.ExamId,
                    UploadedDate = DateTime.UtcNow,
                    UploadedBy = uploadModel.UploadedBy,
                    Content = uploadModel.Content,
                    PDFContent = base64Content
                };

                // Save the new question paper information to the database
                var result = await this.mSyllabusService.AddSyllabus(model);

                if (result == null)
                {
                    return BadRequest("Error while adding Syllabus.");
                }

                return Ok(new
                {
                    Message = "Syllabus added successfully.",
                    Value = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpGet]
        [Route("GetOneSyllabus")]
        public async Task<IActionResult> GetOneSyllabus(int Id)
        {
            try
            {
                var syllabus = await mSyllabusService.GetOneSyllabus(Id);
                if (syllabus == null || !syllabus.Any())
                {
                    return NotFound("Records not found.");
                }

                return Ok(new { Value = syllabus, StatusCode = HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in GetSyllabus: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Data = ex.Message });
            }
        }
        [Route("BulkDeleteSchoolStudentDetails")]
        [HttpPost]
        public async Task<IActionResult> BulkDeleteSchoolStudentDetails(ChildBulkDeleteModel model)
        {
            try
            {
                var res = await this.mChildinfoService.BulkDeleteStudentDetails(model);
                if (res == null)
                {
                    return BadRequest("Failed to delete the records");
                }
                return new JsonResult(new
                {
                    Value = res,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }

        [HttpPost]
        [Route("UpdateDrcEnable")]
        public async Task<IActionResult> UpdateDrcEnable(ChildUpdateModel model)
        {
            try
            {
                var result = await mChildinfoService.UpdateDrcEnableForChildren(model);
                if (result == null)
                {
                    return BadRequest("Unable to update records");
                }
                return new JsonResult(new
                {
                    Value = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }
        //[Route("GetParentsListNew")]
        //[HttpGet]
        //public async Task<IActionResult> GetParentsListNew(Guid token, int nuofRows = 10, int pageNumber = 1, string standard = "", string section = "", string childlastname = "", string searchstring = "")
        //{
        //    try
        //    {
        //        var res = await this.mChildinfoService.GetParentsListNewForAPI(token, nuofRows, pageNumber, standard, section, childlastname, searchstring);
        //        if (res == null)
        //        {
        //            return null;
        //        }
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {

        //        return new JsonResult(new
        //        {
        //            Data = ex.Message,
        //        });
        //    }
        //}

        //Jaliya
        [Route("PostSchoolStudentDetails")]
        [HttpPost]
        public async Task<IActionResult> PostSchoolStudentDetails([FromBody] SchoolStudentAddModel model)
        {
            try
            {
                var res = await this.mChildinfoService.SchoolAddStudentDetails(model);
                if (res == null)
                {
                    return BadRequest("Records not found");
                }
                return new JsonResult(new
                {
                    Value = res,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [Route("BulkDeleteQuestionPaper")]
        [HttpPut]
        public async Task<IActionResult> BulkDeleteQuestionPaper(QuestionPaperBulkDeleteModel model)
        {
            try
            {
                var res = await this._quetionPaperService.BulkDeleteQuestionPaper(model);
                if (res == null)
                {
                    return BadRequest("Failed to Delete the records");
                }
                return new JsonResult(new
                {
                    Value = res,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [Route("DeleteSchoolStudentDetails")]
        [HttpDelete]
        public async Task<IActionResult> DeleteSchoolStudentDetails(int childId)
        {
            try
            {
                var res = await this.mChildinfoService.DeleteStudentDetails(childId);
                if (res == null)
                {
                    return BadRequest("Failed to delete the records");
                }
                return new JsonResult(new
                {
                    Value = res,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [Route("PromoteStudentGradeSection")]
        [HttpPost]
        public async Task<IActionResult> PromoteStudentGradeSection(Guid Token, [FromBody] PromoteStudentModel model)
        {
            try
            {
                var promotionResult = await this.mChildinfoService.PromoteStudentGradeSectionAPI(Token,model);

                if (promotionResult is string && (string)promotionResult == "Promoted Successfully")
                {
                    return Ok(new
                    {
                        Message = "Students promoted successfully.",
                        StatusCode = HttpStatusCode.OK
                    });
                }
                else if (promotionResult is string)
                {
                    return BadRequest(new
                    {
                        Message = promotionResult,
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Message = "Invalid operation.",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }
        [Route("BulkDeleteGenerateQuetionPapers")]
        [HttpPut]
        public async Task<IActionResult> BulkDeleteGenerateQuetionPapers(GenerateQuetionPapersBulkDeleteModel model)
        {
            try
            {
                var res = await this._quetionPaperService.BulkDeleteGenerateQuetionPapers(model);
                if (res == null)
                {
                    return BadRequest("Failed to Delete the records");
                }
                return new JsonResult(new
                {
                    Value = res,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [Route("BulkDeleteSyllabus")]
        [HttpPut]
        public async Task<IActionResult> BulkDeleteSyllabus(SyllabusBulkDeleteModel model)
        {
            try
            {
                var res = await this.mSyllabusService.BulkDeleteSyllabus(model);
                if (res == null)
                {
                    return BadRequest("Failed to Delete the records");
                }
                return new JsonResult(new
                {
                    Value = res,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [HttpGet]
        [Route("GetOneQuestionPapers")]
        public async Task<IActionResult> GetOneQuestionPapers(int Id)
        {
            try
            {
                var questionPapers = await _quetionPaperService.GetOneQuestionPapers(Id);
                if (questionPapers == null || !questionPapers.Any())
                {
                    return NotFound("Records not found.");
                }

                return Ok(new { Value = questionPapers, StatusCode = HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in GetQuestionPapers: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Data = ex.Message });
            }
        }
        [HttpPost]
        [Route("AddGenerateQuestionPaper")]
        public async Task<IActionResult> AddGenerateQuestionPaper([FromBody] AddGenerateQuetionPaperModel model)
        {
            try
            {


                var addPaperModel = new AddGenerateQuetionPaperModel
                {
                    PaperName = model.PaperName,
                    AcademicYearFromId = model.AcademicYearFromId,
                    AcademicYearToId = model.AcademicYearToId,
                    GradeId = model.GradeId,
                    SubjectId = model.SubjectId,
                    SemesterId = model.SemesterId,
                    ExamId = model.ExamId,
                    UploadedDate = DateTime.UtcNow,
                    UploadedBy = model.UploadedBy,
                    Content = model.Content,
                    PDFContent = model.PDFContent

                };

                var result = await _quetionPaperService.AddGenerateQuetionPaper(addPaperModel);

                if (result == null)
                {
                    return BadRequest("Error while adding question paper.");
                }

                return Ok(new
                {

                    Value = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }
        [HttpGet]
        [Route("GetGenerateQuestionPapers")]
        public async Task<IActionResult> GetGenerateQuestionPapers(int academicYearfromId, int academicYeartoId, int gradeId, int subjectId)
        {
            try
            {
                var questionPapers = await _quetionPaperService.GetGenerateQuestionPapers(academicYearfromId, academicYeartoId, gradeId, subjectId);
                if (questionPapers == null || !questionPapers.Any())
                {
                    return NotFound("Records not found.");
                }

                return Ok(new { Value = questionPapers, StatusCode = HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in GetQuestionPapers: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Data = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetQuestionPapers")]
        public async Task<IActionResult> GetQuestionPapers(int academicYearId, int gradeId, int subjectId, int semesterId, int examId, int questionpapertypeId)
        {
            try
            {
                var questionPapers = await _quetionPaperService.GetQuestionPapers(academicYearId, gradeId, subjectId, semesterId, examId, questionpapertypeId);
                if (questionPapers == null || !questionPapers.Any())
                {
                    return NotFound("Records not found.");
                }

                return Ok(new { Value = questionPapers, StatusCode = HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in GetQuestionPapers: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Data = ex.Message });
            }
        }



        [HttpPost]
        [Route("AddQuestionPaper")]
        public async Task<IActionResult> AddQuestionPaper([FromForm] QuestionPaperUploadModel uploadModel)
        {
            try
            {
                if (uploadModel.PdfFile == null || uploadModel.PdfFile.Length == 0 || !uploadModel.PdfFile.FileName.ToLower().EndsWith(".pdf"))
                {
                    return BadRequest(new { Message = "Invalid PDF file provided." });
                }

                var fileId = _mUploadPdfGoogleDriveService.UploadFile(uploadModel.PdfFile,
                                                               uploadModel.AcademicYearId,
                                                               uploadModel.GradeId,
                                                               uploadModel.SubjectId,
                                                               uploadModel.SemesterId,
                                                               uploadModel.ExamId,
                                                               uploadModel.QuestionPaperTypeId);

                if (string.IsNullOrEmpty(fileId))
                {
                    return BadRequest(new { Message = "Failed to upload PDF to Google Drive." });
                }


                string base64Content;
                using (var memoryStream = new MemoryStream())
                {
                    await uploadModel.PdfFile.CopyToAsync(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();
                    base64Content = Convert.ToBase64String(fileBytes);
                }

                string paperName = Path.GetFileNameWithoutExtension(uploadModel.PdfFile.FileName);

                var model = new MQuetionPaperModel
                {
                    PaperName = paperName,
                    AcademicYearId = uploadModel.AcademicYearId,
                    GradeId = uploadModel.GradeId,
                    SubjectId = uploadModel.SubjectId,
                    SemesterId = uploadModel.SemesterId,
                    ExamId = uploadModel.ExamId,
                    UploadedDate = DateTime.UtcNow,
                    UploadedBy = uploadModel.UploadedBy,
                    Content = uploadModel.Content,
                    PDFContent = base64Content,
                    QuestionPaperTypeId = uploadModel.QuestionPaperTypeId
                };


                var result = await this._quetionPaperService.AddQuetionPaper(model);

                if (result == null)
                {
                    return BadRequest("Error while adding question paper.");
                }

                return Ok(new
                {
                    Message = "Question Paper added successfully.",
                    Value = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }


        [HttpGet]
        [Route("GetGeneratedQuestions")]
        public async Task<IActionResult> GetGeneratedQuestions(int academicYearFromId, int academicYearToId, int gradeId, int subjectId)
        {
            try
            {
                var result = await dataAnalysisServices.GetGeneratedQuestions(academicYearFromId, academicYearToId, gradeId, subjectId);
                
                //var jsonResult = JsonSerializer.Serialize(result, new JsonSerializerOptions
                //{
                //    WriteIndented = true, // Optional: Formats JSON with indents for readability
                //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Optional: Convert property names to camelCase
                //});

                if (result == null || !result.Any())
                {
                    return NotFound("No question papers found for the given criteria.");
                }

                return Ok(new
                {
                    Values = result,
                    //Values = jsonResult,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    Data = ex.Message
                });
            }
        }
        [Route("GetStudentDetailsByChildId")]
        [HttpGet]
        public async Task<IActionResult> GetStudentDetailsByChildId(Guid token, int schoolid, int childid, int academicYearId, int nuofRows = 10, int pageNumber = 1)
        {
            try
            {
                var res = await this.mChildinfoService.GetStudentDetailsByChildIdForAPI(token, schoolid, childid, academicYearId, nuofRows, pageNumber);
                if (res?.Items == null || !res.Items.Any())
                {
                    return NotFound();
                }

                var studentDetails = res.Items.First();

                string base64Image = null;
                if (!string.IsNullOrWhiteSpace(studentDetails.StudentImageLink))
                {
                    var imageStream = _googleDriveService.GetFile(studentDetails.StudentImageLink);
                    if (imageStream != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            imageStream.CopyTo(memoryStream);
                            byte[] imageBytes = memoryStream.ToArray();

                            base64Image = Convert.ToBase64String(imageBytes);
                        }
                    }
                }

                var responseObject = new
                {
                    StudentDetails = studentDetails,
                    ImageBase64 = base64Image
                };

                return Ok(responseObject);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Data = ex.Message,
                });
            }
        }
        [Route("GetsblingsListSp")]
        [HttpGet]
        public async Task<IActionResult> GetsblingsListSp(int parentId, int childId, int Schoolid)
        {
            try
            {
                var res = await this.mChildinfoService.GetsblingsListAPI(parentId, childId, Schoolid);
                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {

                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [Route("GetChildDRCByChildIdYear")]
        [HttpGet]
        public async Task<IActionResult> GetChildDRCByChildIdYear(int childid, int academicyear, int nuofRows = 10, int pageNumber = 1)
        {
            try
            {
                var res = await this.mChildinfoService.GetChildDRCByChildIdYear(childid, academicyear, nuofRows, pageNumber);
                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {

                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }

        [Route("GetStudentDetailsByParentId")]
        [HttpGet]
        public async Task<IActionResult> GetStudentDetailsByParentId(int parentid, int nuofRows = 10, int pageNumber = 1)
        {
            try
            {
                var res = await this.mChildinfoService.GetStudentDetailsByParentIdAPI(parentid, nuofRows,pageNumber);
                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {

                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        
        [Route("GetStudentDetailsParentByChildId")]
        [HttpGet]
        public async Task<IActionResult> GetStudentDetailsParentByChildId(int parentid, int childid, int nuofRows = 10, int pageNumber = 1)
        {
            try
            {
                var res = await this.mChildinfoService.GetStudentDetailsParentByChildIdAPI(parentid, childid, nuofRows, pageNumber);
                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {

                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [Route("UpdateStudentInfo")]
        [HttpPost]
        public async Task<IActionResult> UpdateStudentInfo(Guid token, [FromBody] GetParentsListModel model)
        {
            try
            {
                if (model.StudentImageFile != null && model.StudentImageFile.Length > 0)
                {
                    var fileId = _googleDriveService.UploadFile(model.StudentImageFile);
                    model.StudentImageLink = fileId;
                }

                var res = await this.mChildinfoService.UpdateStudentInfoAPI(token, model);

                if (res == null)
                {
                    return NotFound(new { Message = "Student information update failed." });
                }

                return Ok(new { Value = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Data = ex.Message
                });
            }
        }
        //[Route("UpdateStudentInfo")]
        //[HttpPost]
        //public async Task<IActionResult> UpdateStudentInfo(Guid token, [FromForm] GetParentsListModel model)
        //{
        //    try
        //    {
        //        if (model.StudentImageFile != null && model.StudentImageFile.Length > 0)
        //        {
        //            var fileId = _googleDriveService.UploadFile(model.StudentImageFile);
        //            model.StudentImageLink = fileId;
        //        }

        //        var res = await this.mChildinfoService.UpdateStudentInfoAPI(token, model);

        //        if (res == null)
        //        {
        //            return NotFound(new { Message = "Student information update failed." });
        //        }

        //        return Ok(new { Value = res });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            Data = ex.Message,
        //        });
        //    }
        //}
        [Route("PostParentForExistingChild")]
        [HttpPost]
        public async Task<IActionResult> PostParentForExistingChild([FromBody] ParentAddModel model)
        {
            try
            {
                var res = await this.mChildinfoService.AddParentForExistingChild(model);
                if (res == null)
                {
                    return BadRequest("Records not found");
                }
                return new JsonResult(new
                {
                    Value = res,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [Route("UpdateParentChild")]
        [HttpPost]
        public async Task<IActionResult> UpdateParentInfo(Guid token, ParentsUpdateModel model)
        {
            try
            {
                var res = await this.mChildinfoService.UpdateParentInfoForAPI(token, model);
                if (res == null)
                {
                    return null;
                }
                return Ok(new { Value = res });
            }
            catch (Exception ex)
            {

                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }

        }

    }
}
