using ClosedXML.Excel;
using CommonUtility;
using CommonUtility.RequestModels;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using static CommonUtility.RequestModels.MAcademicyeardetailModel;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class DataAnalyticsController : ControllerBase
    {
        private readonly IDataAnalysisServices dataAnalysisServices;
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext db3 = new TpContext();

        public DataAnalyticsController(IDataAnalysisServices _dataAnalysisServices)
        {
            dataAnalysisServices = _dataAnalysisServices;
        }

        #region Support APIs

        ////Dinidu
        //[Route("SubSubjectSubMaxMarks")]
        //[HttpPost]
        //public async Task<IActionResult> SubSubjectSubMaxMarks([FromBody] postSubSubjectSubmaxmarksModel model)
        //{
        //    try
        //    {

        //        var res = await this.dataAnalysisServices.setsubsubjectsubmaxmarksAPI(model);
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
        //            StatusCode = HttpStatusCode.InternalServerError,
        //        });
        //    }

        //}

      
        //Jaliya
        [Route("PostSubjectTestMapping")]
        [HttpPost]
        public async Task<IActionResult> PostSubjectTestMapping([FromBody] SubjectTestMappingAddModel model)
        {
            try
            {
                var res = await this.dataAnalysisServices.AddSubjectTestMapping(model);
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

        #endregion
        [Route("DownloadClassListSummary")]
        [HttpPost]
        public async Task<IActionResult> DownloadClassListSummary(
[FromQuery] int academicYearId,
[FromQuery] int standardId,
[FromQuery] int? sectionId = null)
        {
            try
            {
                var summary = await dataAnalysisServices.GetClassListSummaryAsync(academicYearId, standardId, sectionId);
                var list = summary.Data;
                var yearName = summary.AcademicYearName;
                var gradeName = summary.GradeName;

                var sheetName = sectionId.HasValue
                    ? $"Class List {gradeName} {list.First().SectionName} {yearName}"
                    : $"Class List {gradeName} {yearName}";

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(sheetName);

                var headers = new[] { "No.", "Grade", "Section", "AdmissionNo", "FirstName", "LastName" };
                for (var c = 0; c < headers.Length; c++)
                    ws.Cell(1, c + 1).Value = headers[c];

                for (var i = 0; i < list.Count; i++)
                {
                    var dto = list[i];
                    var r = i + 2;
                    var col = 1;
                    ws.Cell(r, col++).Value = i + 1;
                    ws.Cell(r, col++).Value = gradeName;
                    ws.Cell(r, col++).Value = dto.SectionName;
                    ws.Cell(r, col++).Value = dto.AdmissionNo;
                    ws.Cell(r, col++).Value = dto.FirstName;
                    ws.Cell(r, col++).Value = dto.LastName;
                }

                using var ms = new MemoryStream();
                wb.SaveAs(ms);
                ms.Position = 0;

                var fileName = sectionId.HasValue
                    ? $"Class List {gradeName} {list.First().SectionName} {yearName}.xlsx"
                    : $"Class List {gradeName} {yearName}.xlsx";

                return File(
                    ms.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: DownloadClassListSummary failed – {ex.Message}");
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    new { Data = ex.Message }
                );
            }
        }


        [Route("DownloadClassMarksSummary")]
        [HttpPost]
        public async Task<IActionResult> DownloadClassMarksSummary(
     [FromQuery] int academicYearId,
     [FromQuery] int sectionId,
     [FromQuery] int testId)
        {
            try
            {
                var summary = await dataAnalysisServices.GetClassMarksSummaryAsync(academicYearId, sectionId, testId);
                var list = summary.Data;
                var yearName = summary.AcademicYearName;
                var gradeSection = summary.GradeSectionName;
                var termName = summary.TermName;

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Summary");

                var headers = new List<string> {
            "No.",
            "AdmissionNo",
            "FullName"
        };
                if (list.Any())
                    headers.AddRange(list[0].SubjectMarks.Keys);
                headers.AddRange(new[] { "TotalMarks", "AverageMarks", "Position", "Comment" });

                for (var c = 0; c < headers.Count; c++)
                    ws.Cell(1, c + 1).Value = headers[c];

                for (var i = 0; i < list.Count; i++)
                {
                    var dto = list[i];
                    var r = i + 2;
                    var col = 1;

                    ws.Cell(r, col++).Value = i + 1;
                    ws.Cell(r, col++).Value = dto.AdmissionNo;
                    ws.Cell(r, col++).Value = dto.FullName;

                    foreach (var subj in dto.SubjectMarks.Keys)
                        ws.Cell(r, col++).Value = dto.SubjectMarks[subj];

                    ws.Cell(r, col++).Value = dto.TotalMarks;
                    ws.Cell(r, col++).Value = dto.AverageMarks;
                    ws.Cell(r, col++).Value = dto.Position;
                    ws.Cell(r, col++).Value = dto.OverallComments;
                }

                using var ms = new MemoryStream();
                wb.SaveAs(ms);
                ms.Position = 0;

                var fileName = $"DRC Class Summary {gradeSection} {termName} {yearName}.xlsx";
                return File(
                    ms.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: failed to generate report – {ex.Message}");
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    new { Data = ex.Message }
                );
            }
        }

        [Route("UpdateLevel")]
        [HttpPost]
        public async Task<IActionResult> UpdateLevel([FromBody] UpdateLevel model)
        {
            try
            {
                bool isUpdated = await this.dataAnalysisServices.UpdateLevel(model.Id, model.Level);
                if (!isUpdated)
                {
                    return BadRequest("Update operation failed");
                }
                return Ok(new
                {
                    Message = "Update Successful",
                    StatusCode = HttpStatusCode.OK,
                });
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
        [Route("UpdateSemester")]
        [HttpPost]
        public async Task<IActionResult> UpdateSemester([FromBody] UpdateSemester model)
        {
            try
            {
                bool isUpdated = await this.dataAnalysisServices.UpdateSemester(model.Id, model.Name);
                if (!isUpdated)
                {
                    return BadRequest("Update operation failed");
                }
                return Ok(new
                {
                    Message = "Update Successful",
                    StatusCode = HttpStatusCode.OK,
                });
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
        [Route("UpdateGrade")]
        [HttpPost]
        public async Task<IActionResult> UpdateGrade([FromBody] UpdateGrade model)
        {
            try
            {
                bool isUpdated = await this.dataAnalysisServices.UpdateGrade(model.Id, model.Name);
                if (!isUpdated)
                {
                    return BadRequest("Update operation failed");
                }
                return Ok(new
                {
                    Message = "Update successful",
                    StatusCode = HttpStatusCode.OK,
                });
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
        [Route("UpdateSubSubjects")]
        [HttpPost]
        public async Task<IActionResult> UpdateSubSubjects([FromBody] UpdateSubSubjects model)
        {
            try
            {
                bool isUpdated = await this.dataAnalysisServices.UpdateSubSubject(model.Id, model.SubSubjectName, model.Percentage, model.SubMaxMarks);
                if (!isUpdated)
                {
                    return BadRequest("Update operation failed");
                }
                return Ok(new
                {
                    Message = "Update successful",
                    StatusCode = HttpStatusCode.OK,
                });
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
        [Route("GetDAStatus")]
        [HttpGet]
        public async Task<IActionResult> GetDAStatus(int schoolid)
        {
            try
            {
                if (schoolid == null)
                {
                    return BadRequest("Schoolid is requried");
                }

                var school = await db.MSchoolDamappings.Where(w => w.SchoolId == schoolid).FirstOrDefaultAsync();
                if (school != null && school.Daflag == true)
                {
                    return Ok(new { Value = "Enabled" });
                }
                return Ok(new { Value = "Disabled" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetSchoolDetailsForRC")]
        [HttpGet]
        public async Task<IActionResult> GetSchoolDetailsForRC(int schoolid, int Childid, [FromQuery] int[] childtestIds)
        {
            try
            {
                if (schoolid == null)
                {
                    return BadRequest("Schoolid is requried");
                }

                var school = await db.MSchools.Where(w => w.Id == schoolid).FirstOrDefaultAsync();
                var data = await db.MBranches.Where(x => x.Schoolid == schoolid).FirstOrDefaultAsync();
                if (school != null)
                {
                    SchoolDetailsForRCModel model = new SchoolDetailsForRCModel();
                    model.SchoolName = school.Name;
                    model.Address = data.Address;
                    model.Pincode = data.Pincode;
                    model.PrimaryPhonenumber = school.Primaryphonenumber;
                    model.SecondaryPhonenumber = school.Secondaryphonenumber;
                    model.WebsiteLink = school.Websitelink;
                    model.Email = school.Emailid;
                    model.Logo = school.Logo;
                    model.City = db.MLocations.Where(y => y.Locationtypeid == y.Locationtype.Id && y.Id == data.Locaionid && y.Locationtypeid == 5 && data.Schoolid == schoolid).Select(t => t.Name).FirstOrDefault();

                    var childinfo = await db.MChildschoolmappings.Where(x => x.Childid == Childid && x.Standardsectionmapping.Branch.Schoolid == schoolid).FirstOrDefaultAsync();
                    if (childinfo == null)
                    {
                        return BadRequest("Child not found");
                    }
                    else
                    {
                        var childData = await db2.MChildinfos.Where(x => x.Id == childinfo.Childid).FirstOrDefaultAsync();
                        model.ChildName = childData.Firstname + " " + childData.Lastname;
                        model.RegNo = childinfo.Registerationnumber;
                    }

                    foreach (int ctm in childtestIds)
                    {
                        usersList ul = new usersList();
                        var teachid = await db.MChildschoolmappings.Where(x => x.Id == ctm).Select(w => w.Modifiedby).FirstOrDefaultAsync();
                        if (teachid != null)
                        {
                            var teachinfo = await db.MAppuserinfos.Where(x => x.Id == teachid).FirstOrDefaultAsync();
                            ul.TeacherName = teachinfo.Firstname + " " + teachinfo.Lastname;
                            model.Teachers.Add(ul);
                        }
                    }


                    return Ok(model);
                }
                return BadRequest("School not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("AddFreezeDRCEnable")]
        [HttpPost]
        public async Task<IActionResult> AddFreezeDRCEnable(AddFreezeEnableModel model)
        {
            try
            {
                var result = await this.dataAnalysisServices.AddFreezeEnable(model);
                if (result == null)
                {
                    return BadRequest("Unable to add records");
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
      
        [HttpGet]
        [Route("GetFreezeDRCEnable")]
        public async Task<IActionResult> GetFreezeDRCEnable(int academicYearId, int levelId, int semesterId, int examId)
        {
            try
            {
                var result = await dataAnalysisServices.GetFreezeEnable(academicYearId, levelId, semesterId, examId);

                if (result == null || !result.Any())
                {
                    return NotFound("No FreezeEnable was found for the given criteria.");
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
       

        #region Digital Report Card
        //Jaliya Teacher Analytics

        [Route("GetTeacharsSubjectPercentageScoreByYearChart")]
        [HttpGet]
        public async Task<IActionResult> GetTeacharsSubjectPercentageScoreByYearChart(int levelId, int testId, int subjectId, int teacherId)
        {
            try
            {
                var (scores, gradePercentages) = await this.dataAnalysisServices.GetTeacharsSubjectPercentageScoreByYearChart(levelId, testId, subjectId, teacherId);
                if (scores == null || scores.Count == 0)
                {
                    return BadRequest("Records not found");
                }

                return new JsonResult(new
                {
                    Scores = scores,
                    GradePercentages = gradePercentages,
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
        //Jaliya Academic Trends
        [Route("GetNumberofstudentforsubjectChart")]
        [HttpGet]
        public async Task<IActionResult> GetNumberofstudentforsubjectChart(int levelId, int standardId, int subjectId)
        {
            try
            {
                var res = await this.dataAnalysisServices.GetNumberofstudentforsubjectChart(levelId, standardId, subjectId);
                if (res == null || res.Count == 0)
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
        //Jaliya Grade Analytics

        [Route("GetPercentageScoreBySubjectChart")]
        [HttpGet]
        public async Task<IActionResult> GetPercentageScoreBySubjectChart(int levelId, int standardId, int semesterId, int testId, int subjectId)
        {
            try
            {
                var (scores, gradePercentages) = await this.dataAnalysisServices.GetPercentageScoreBySubjectChart(levelId, standardId, semesterId, testId, subjectId);
                if (scores == null || scores.Count == 0)
                {
                    return BadRequest("Records not found");
                }

                return new JsonResult(new
                {
                    Scores = scores,
                    GradePercentages = gradePercentages,
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
        [Route("dataAStudentsDropdowns")]
        [HttpGet]
        public async Task<IActionResult> dataAStudentsDropdowns(int standardid, int sectionid, int schoolid, int academicYear)
        {
            try
            {
                List<dataAStudent> studentList = new List<dataAStudent>();
                if (sectionid != 0 && schoolid != 0)
                {
                    var data = db.MStandardsectionmappings.Where(y => y.Parentid == standardid && y.Id == sectionid && y.Branch.Schoolid == schoolid).Select(b => b.Id).FirstOrDefault();
                    var data1 = db.MChildschoolmappings.Where(x => x.Standardsectionmappingid == data && x.AcademicYearId == academicYear).OrderBy(a => a.Child.Lastname);
                    var siblingStatus = 0;
                    foreach (var item in data1)
                    {
                        if (item != null)
                        {
                            var studentData = await db2.MChildinfos.Where(x => x.Id == item.Childid).FirstOrDefaultAsync();
                            
                            var parentsData = await db2.MParentchildmappings.Where(x => x.Childid == item.Childid).FirstOrDefaultAsync();
                            if(parentsData !=  null)
                            {
                                var siblingdata = await db2.MParentchildmappings.Where(x => x.Childid != item.Childid && x.Appuserid == parentsData.Appuserid).FirstOrDefaultAsync();
                                siblingStatus = siblingdata != null ? 1 : 0;

                            }

                            studentList.Add(new dataAStudent
                            {
                                StudentId = studentData.Id,
                                RegistrationNumber = item.Registerationnumber,
                                StudentName = studentData.Lastname + " " + studentData.Firstname,
                                Promoted = item.Promoted,
                                Statusid = item.Statusid, // 27/2/2024 Sanduni  23/7/2024
                                Siblngstatus = siblingStatus
                            });
                        }
                    }
                    return Ok(new { result = "ok", studentsList = studentList, No_of_Students = studentList.Count });
                }
                return Ok(new { result = "ok", studentsList = studentList, No_of_Students = studentList.Count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAveragePerformances")]
        public async Task<IActionResult> GetAveragePerformances(int sectionid, int testid, int childid, int academicYear)
        {
            try
            {
                var res = await this.dataAnalysisServices.GetAveragePerformances(sectionid, testid, childid, academicYear);
                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetStudentsRC")]
        [HttpGet]
        public async Task<IActionResult> GetStudentsRC(int childid, int testid, int schoolid, int academicYearId)
        {
            try
            {
                var res = await this.dataAnalysisServices.GetStudentsRc(childid, testid, schoolid, academicYearId);
                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetHiEduStudentsRC")]
        [HttpGet]
        public async Task<IActionResult> GetHiEduStudentsRC(int courseid, int batchid, int childid, int schoolid,int semeseterid, int examid)
        {
            try
            {
                var res = await this.dataAnalysisServices.GetHiEduStudentsRC(courseid, batchid, childid, schoolid, semeseterid, examid);
                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

        [Route("UpdateStudentRC")]
        [HttpPost]
        public async Task<IActionResult> UpdateStudentRC([FromBody] postStudentRCModel model)
        {
            try
            {
                var res = await this.dataAnalysisServices.UpdateStudentsRc(model);
                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("HiEduUpdateStudentsRc")]
        [HttpPost]
        public async Task<IActionResult> HiEduUpdateStudentsRc([FromBody] postHiEduStudentRCModel model)
        {
            try
            {
                var res = await this.dataAnalysisServices.HiEduUpdateStudentsRc(model);
                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("dataASemesterDropdown")]
        [HttpGet]
        public async Task<IActionResult> dataASemesterDropdown(int schoolid, int academicYearId)
        {
            try
            {
                List<dataASemester> semesterList = new List<dataASemester>();
                if (schoolid != 0)
                {
                    var data = await Task.FromResult((from sym in db.MSemesteryearmappings
                                                      join stm in db.MSemestertestsmappings on sym.SemesterId equals stm.Id
                                                      where stm.Branch.Schoolid == schoolid && sym.AcademicYearId == academicYearId
                                                      select new { Id = stm.Id, Name = stm.Name }).Distinct());

                    foreach (var item in data)
                    {
                        semesterList.Add(new dataASemester
                        {
                            SemesterId = item.Id,
                            SemesterName = item.Name
                        });
                    }
                    return Ok(new { result = "ok", semestersList = semesterList });
                }
                return Ok(new { result = "ok", semestersList = semesterList });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("dataATestDropdown")]
        [HttpGet]
        public async Task<IActionResult> dataATestDropdown(int semesterid)
        {
            try
            {
                List<dataATest> testList = new List<dataATest>();
                if (semesterid != 0)
                {
                    var data = db.MSemestertestsmappings.Where(x => x.SemesterId == semesterid).Select(a => a.Id);

                    foreach (var item in data)
                    {
                        var testData = await db2.MSemestertestsmappings.Where(x => x.Id == item).FirstOrDefaultAsync();
                        testList.Add(new dataATest
                        {
                            TestId = testData.Id,
                            TestName = testData.Name
                        });
                    }
                    return Ok(new { result = "ok", testsList = testList });
                }
                return Ok(new { result = "ok", testsList = testList });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("dataAYearDropdown")]
        [HttpGet]
        public async Task<IActionResult> dataAYearDropdown(int schoolid)
        {
            try
            {
                List<dataAYear> yearData = new List<dataAYear>();
                var res = await Task.FromResult(db.MAcademicyeardetails.Where(x => x.SchoolId == schoolid).Distinct().OrderByDescending(b => b.Createddate).ToList());
                if (res != null)
                {
                    foreach (var item in res)
                    {
                        dataAYear das = new dataAYear();
                        das.YearId = item.Id;
                        das.AcademicYear = item.YearName;
                        das.Currentyear = item.Currentyear;
                        yearData.Add(das);
                    }
                    return Ok(new { result = "ok", yearList = yearData });
                }
                return Ok(new { result = "ok", yearList = yearData });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //9/03/2023 - Sanduni - Sub Subject - Student Chart APIS
        [HttpGet]
        [Route("GetSubSubjectRankByClass")]
        public async Task<IActionResult> GetSubSubjectRankByClass(int childid, int SubjectId, int SubSubjectId, int LevelID, int schoolid, int TestId)
        {
            try
            {
                var res = await this.dataAnalysisServices.GetSubSubjectRankByClass(childid, SubjectId, SubSubjectId, LevelID, schoolid, TestId);

                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        //28/03/2023 - Sanduni - Sub Subject marks - Student Chart APIS
        [HttpGet]
        [Route("GetStudentMainSubjectSubSubjectMarks")]
        public async Task<IActionResult> GetStudentMainSubjectSubSubjectMarks(int childid, int SubjectId, int Sectionid, int schoolid, int TestId)
        {
            try
            {
                var res = await this.dataAnalysisServices.GetStudentMainSubjectSubSubjectMarks(childid, SubjectId, Sectionid, schoolid, TestId);

                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("GetOverallTwoSubjectGradePrecentage")]
        public async Task<IActionResult> GetOverallTwoSubjectGradePrecentage(int SubjectId1, int SubjectId2, int GradeId, int TestId)
        {
            try
            {
                var res = await this.dataAnalysisServices.GetOverallTwoSubjectGradePrecentage(SubjectId1, SubjectId2, GradeId, TestId);

                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //Grade Analytics - Get Last 3 Years Grade Subject Performance
        [HttpGet]
        [Route("GetOvverallGradeSubjectMarksinGradingMethod")]
        public async Task<IActionResult> GetOvverallGradeSubjectMarksinGradingMethod(int SubjectId1, int GradeId, int AcademicYear, int SchoolId, int SemesterId, int ExamId)
        {
                try
                {
                var res = await this.dataAnalysisServices.GetOvverallGradeSubjectMarksinGradingMethod(SubjectId1, GradeId, AcademicYear, SchoolId, SemesterId, ExamId);
         
                        if (res == null)
                        {
                            return null;
                        }
                        return Ok(res);
                    }
            catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
        



        [HttpGet]
        [Route("GeOverallSubjectMarksOverallRankByGrade")]
        public async Task<IActionResult> GeOverallSubjectMarksOverallRankByGrade(int SubjectId1, int GradeId, int TestId, int? AcademicYearId)
        {
            try
            {
                var res = await this.dataAnalysisServices.GeOverallSubjectMarksOverallRankByGrade(SubjectId1, GradeId, TestId, AcademicYearId);

                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        //15/11/2022 - Student Chart APIS
        [HttpGet]
        [Route("GetSubjectRankClassStudentCharts")]
        public async Task<IActionResult> GetSubjectRankClassStudentCharts(int childid, int SubjectId, int schoolid)
        {
            try
            {
                var res = await this.dataAnalysisServices.GetSubjectRankClassStudentCharts(childid, SubjectId, schoolid);

                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        //15/11/2022 - Student Chart APIS
        [HttpGet]
        [Route("GetStudentsChartOverallPrecentageRank")]
        public async Task<IActionResult> GetStudentsChartOverallPrecentageRank(int childid, int schoolid)
        {
            try
            {
                var res = await this.dataAnalysisServices.GetStudentsChartOverallPrecentageRank(childid, schoolid);

                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        //Jaliya - Overall Semester Results
        [HttpGet]
        [Route("GetAveragePerformanceForStudentSemester")]
        public async Task<IActionResult> GetAveragePerformanceForStudentSemester(int sectionId, int testId, int childId, int academicYearId)
        {
            try
            {
                var result = await dataAnalysisServices.GetStudentPerformanceSemester(sectionId, testId, childId, academicYearId);

                if (result == null)
                {
                    return NotFound("No Average Performance For Student Semester was found for the given criteria.");
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

        #endregion

        #region HiEdu Digital Report Card
        [Route("GetHiEduModuleStudentPerformanceScoreChart")]
        [HttpGet]
        public async Task<IActionResult> GetHiEduModuleStudentPerformanceScoreChart(int batchId, int semesterId, int examId, int subjectId)
        {
            try
            {
                var res = await this.dataAnalysisServices.GetHieduModuleStudentPerformanceScoreChart(batchId, semesterId, examId, subjectId);
                if (res == null)
                {
                    return NotFound();
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

        [Route("HiEduGetAllSubjectMarksForStudent")]
        [HttpGet]
        public async Task<IActionResult> HiEduGetAllSubjectMarksForStudent(int childId, int examId)
        {
            try
            {
                var res = await dataAnalysisServices.HiEduGetAllSubjectMarksForStudent(childId, examId);
                if (res == null)
                {
                    return NotFound();
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

        [Route("GetHiEduAverageSubjectMarks")]
        [HttpGet]
        public async Task<IActionResult> GetHiEduAverageSubjectMarks(int courseId, int batchId, int semesterId, int examId)
        {
            try
            {
                var res = await dataAnalysisServices.GetHiEduAverageSubjectMarks(courseId, batchId, semesterId, examId);
                if (res == null)
                {
                    return NotFound();
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

        [Route("GetHiEduSubSubjectMarksForStudent")]
        [HttpGet]
        public async Task<IActionResult> GetHiEduSubSubjectMarksForStudent(int childId, int subjectId)
        {
            try
            {
                var res = await dataAnalysisServices.GetHiEduSubSubjectMarksForStudent(childId, subjectId);
                if (res == null)
                {
                    return NotFound();
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
        #endregion

        #region Summary

        [Route("dataASubjectsDropdown")]
        [HttpGet]
        public async Task<IActionResult> dataASubjectsDropdown(int sectionid, int schoolid)
        {
            try
            {
                List<dataASubjects> subjectList = new List<dataASubjects>();
                if (sectionid != 0 && schoolid != 0)
                {
                    var data1 = db.MSubjectsectionmappings.Where(x => x.SectionId.Equals(sectionid) && x.Subject.Branch.Schoolid.Equals(schoolid)).Select(a => a.SubjectId);

                    foreach (var item in data1)
                    {
                        var subjectData = await db2.MSubjects.Where(x => x.Id == item).FirstOrDefaultAsync();
                        subjectList.Add(new dataASubjects
                        {
                            SubjectId = subjectData.Id,
                            SubjectName = subjectData.Name
                        });
                    }
                    return Ok(new { result = "ok", subjectsList = subjectList });
                }
                return Ok(new { result = "ok", subjectsList = subjectList });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetSubjectsSectionInfo")]
        [HttpGet]
        public IActionResult GetSubjectsSectionInfo(int sectionid, int subjectid, int semesterid, [FromQuery] int[] iarr)
        {
            try
            {
                int count = 0, prev = 0;
                var arr = iarr.Distinct().ToArray();
                Array.Sort(arr);
                Array.Reverse(arr);
                var res = this.dataAnalysisServices.GetSubSecInfo(sectionid, subjectid, semesterid);
                var marks = res.OrderByDescending(x => x.Percentage).Select(w => w.Percentage).ToArray();
                int size = marks.Count();
                int arrSize = arr.Length;
                List<summaryConstraintModel> summaryModel = new List<summaryConstraintModel>();

                #region  count-conditions
                for (int i = 0; i < arrSize; i++)
                {
                    count = 0;

                    for (int j = 0; j < size; j++)
                    {
                        if (prev == 0)
                        {
                            if (marks[j] >= arr[i])
                            {
                                count += 1;
                            }
                        }
                        else
                        {
                            if (marks[j] >= arr[i] && marks[j] < prev)
                            {
                                count += 1;

                            }
                        }

                    }
                    prev = arr[i];
                    summaryConstraintModel sumMod = new summaryConstraintModel();
                    sumMod.constraint = " >= " + arr[i];
                    sumMod.percentage = (double)this.dataAnalysisServices.calculatePercentage(count, size);
                    summaryModel.Add(sumMod);


                    if (i == arrSize - 1)
                    {

                        count = 0;
                        for (int j = 0; j < size; j++)
                        {
                            if (marks[j] < arr[i])
                            {
                                count += 1;
                            }

                        }
                        summaryConstraintModel sumMods = new summaryConstraintModel();
                        sumMods.constraint = " < " + arr[i];
                        sumMods.percentage = (double)this.dataAnalysisServices.calculatePercentage(count, size);
                        summaryModel.Add(sumMods);
                    }

                }
                #endregion


                return Ok(new { result = "ok", data = summaryModel });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }
        [Route("GetStudentSemesterSummary")]
        [HttpGet]
        public async Task<IActionResult> GetStudentSemesterSummary(int childid, string testName, int schoolid, int academicYearId)
        {
            try
            {
                var res = await this.dataAnalysisServices.GetStudentSemesterSummary(childid, testName, schoolid, academicYearId);
                if (res == null)
                {
                    return null;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Teachers Tab

        [Route("GetTeacherDetails")]
        [HttpGet]
        public async Task<IActionResult> GetTeacherDetails(int SchoolId)
        {
            try
            {
                List<TeacherDetailsModel> teacherdetailsList = new List<TeacherDetailsModel>();
                if (SchoolId != 0)
                {
                    var data1 = (from sur in db.MSchooluserroles
                                 join cat in db.MCategories on sur.Categoryid equals cat.Id
                                 join r in db.MRoles on cat.Roleid equals r.Id
                                 where r.Schoolid == SchoolId && r.Rank == 4
                                 select new { sur.Schooluserid });

                    foreach (var item in data1)
                    {
                        var teachersData = await db2.MSchooluserinfos.Where(x => x.Id == item.Schooluserid).FirstOrDefaultAsync();
                        teacherdetailsList.Add(new TeacherDetailsModel
                        {
                            teacherid = teachersData.Id,
                            teacherName = teachersData.Firstname + " " + teachersData.Lastname
                        });
                    }
                    return Ok(new { result = "ok", teacherdetailList = teacherdetailsList });
                }
                return Ok(new { result = "ok", teacherdetailList = teacherdetailsList });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetTeacherStandardDropdown")]
        [HttpGet]
        public async Task<IActionResult> GetTeacherStandardDropdown(int TeacherId)
        {
            try
            {
                List<TeacherStdModel> teacherStdList = new List<TeacherStdModel>();
                if (TeacherId != 0)
                {
                    var data = (from ssm in db.MStandardsectionmappings
                                join sur in db.MSchooluserroles on ssm.Id equals sur.Standardsectionmappingid
                                where sur.Schooluserid == TeacherId
                                select new { ssm.Parentid }).Distinct();

                    foreach (var item in data)
                    {
                        if (item != null)
                        {
                            var teacherstdData = await db2.MStandardsectionmappings.Where(x => x.Id == item.Parentid).FirstOrDefaultAsync();
                            teacherStdList.Add(new TeacherStdModel
                            {
                                teacherid = TeacherId,
                                stdid = teacherstdData.Id,
                                stdName = teacherstdData.Name
                            });
                        }
                    }
                    return Ok(new { result = "ok", teachersStdList = teacherStdList });
                }
                return Ok(new { result = "ok", teachersStdList = teacherStdList });
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetTeacherSectionDropdown")]
        [HttpGet]
        public async Task<IActionResult> GetTeacherSectionDropdown(int TeacherId, int stdid)
        {
            try
            {
                List<TeacherSectionModel> teacherSecList = new List<TeacherSectionModel>();
                if (TeacherId != 0)
                {
                    var data1 = db.MStandardsectionmappings.Where(e => e.Parentid == stdid).Select(t => t.Id);
                    foreach (var items in data1)
                    {
                        var data2 = db2.MSchooluserroles.Where(q => q.Schooluserid == TeacherId && q.Standardsectionmappingid == items).Select(g => g.Standardsectionmappingid);

                        foreach (var item in data2)
                        {
                            var teachersecData = await db3.MStandardsectionmappings.Where(x => x.Id == item).FirstOrDefaultAsync();
                            teacherSecList.Add(new TeacherSectionModel
                            {
                                sectionid = teachersecData.Id,
                                sectionName = teachersecData.Name
                            });
                        }
                    }
                    return Ok(new { result = "ok", teachersSecList = teacherSecList });
                }
                return Ok(new { result = "ok", teachersSecList = teacherSecList });
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetTeacherSubjectDropdown")]
        [HttpGet]
        public async Task<IActionResult> GetTeacherSubjectDropdown(int SectionId, int TeacherId, int SchoolId)
        {
            try
            {
                List<object> full = new List<object>();
                List<dataASubjects> teachersubjectList = new List<dataASubjects>();
                List<dataASemester> semesterList = new List<dataASemester>();

                if (SectionId != 0 && SchoolId != 0 && TeacherId != 0)
                {
                    var data2 = db.MTeachersubjectmappings.Where(x => x.TeacherId == TeacherId && x.SubjectSection.SectionId == SectionId).Select(a => a.SubjectSection.SubjectId);
                    var res = db.MSemestertestsmappings.Where(x => x.Branch.Schoolid == SchoolId && x.SemesterId == null).Select(b => b.Id);

                    foreach (var item in data2)
                    {
                        var subjectData = await db3.MSubjects.Where(x => x.Id == item).FirstOrDefaultAsync();
                        teachersubjectList.Add(new dataASubjects
                        {
                            SubjectId = subjectData.Id,
                            SubjectName = subjectData.Name
                        });
                    }
                    full.Add(teachersubjectList);

                    foreach (var item in res)
                    {
                        var semesterData = await db3.MSemestertestsmappings.Where(x => x.Id == item).FirstOrDefaultAsync();
                        semesterList.Add(new dataASemester
                        {
                            SemesterId = semesterData.Id,
                            SemesterName = semesterData.Name
                        });
                    }
                    full.Add(semesterList);

                    return Ok(new { result = "ok", teachersubjectsList = full });
                }
                return Ok(new { result = "ok", teachersubjectsList = full });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetSubjectsSemestersInfo")]
        [HttpGet]
        public async Task<IActionResult> GetSubjectsSemestersInfo(int sectionid, int subjectid, int Agrade, int fail, [FromQuery] int[] semesters)
        {
            try
            {
                if (Agrade <= fail || Agrade == 0 || fail == 0)
                {
                    return BadRequest("Invalid Constraints");
                }
                else
                {

                    List<TeachersConstraintModel> TeacherSemModel = new List<TeachersConstraintModel>();

                    foreach (var item in semesters)
                    {
                        int countA = 0, countF = 0;
                        double? sum = 0;
                        var res = this.dataAnalysisServices.GetSubSecInfo(sectionid, subjectid, item);
                        var marks = res.OrderByDescending(x => x.Percentage).Select(w => w.Percentage).ToArray();
                        int size = marks.Count();
                        for (int i = 0; i < size; i++)
                        {
                            if (marks[i] != null)
                            {
                                sum += marks[i];
                            }
                            if (marks[i] >= Agrade)
                            {
                                countA += 1;
                            }
                            else if (marks[i] < fail)
                            {
                                countF += 1;
                            }
                        }
                        TeachersConstraintModel sumMod = new TeachersConstraintModel();
                        sumMod.Semester = await db.MSemestertestsmappings.Where(x => x.Id == item).Select(x => x.Name).FirstOrDefaultAsync();
                        sumMod.Aconstraint = " >= " + Agrade;
                        sumMod.Apercentage = dataAnalysisServices.calculatePercentage(countA, size);
                        sumMod.Fconstraint = " < " + fail;
                        sumMod.Fpercentage = dataAnalysisServices.calculatePercentage(countF, size);
                        if (size > 0)
                        {
                            var average = sum / size;
                            sumMod.average = Math.Round((double)average, 2);
                        }
                        else
                        {
                            sumMod.average = 0;
                        }
                        TeacherSemModel.Add(sumMod);

                    }
                    return Ok(TeacherSemModel);
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }

        }

        #endregion

        #region Students

        [Route("SubjectsWithSemestersDropdown")]
        [HttpGet]
        public async Task<IActionResult> SubjectsWithSemestersDropdown(int schoolid, int sectionid)
        {
            try
            {
                List<object> full = new List<object>();

                List<dataASubjects> dad = new List<dataASubjects>();
                List<dataASemester> dasl = new List<dataASemester>();
                if (schoolid != 0 && sectionid != 0)
                {
                    var data1 = db.MSubjectsectionmappings.Where(x => x.SectionId.Equals(sectionid) && x.Subject.Branch.Schoolid.Equals(schoolid)).Select(a => a.SubjectId);

                    foreach (var item in data1)
                    {
                        var subjectData = await db2.MSubjects.Where(x => x.Id == item).FirstOrDefaultAsync();
                        dad.Add(new dataASubjects
                        {
                            SubjectId = subjectData.Id,
                            SubjectName = subjectData.Name
                        });
                    }
                    full.Add(dad);

                    var data2 = db.MSemestertestsmappings.Where(x => x.Branch.Schoolid == schoolid && x.SemesterId == null).Select(a => a.Id);

                    foreach (var item in data2)
                    {
                        var semesterData = await db2.MSemestertestsmappings.Where(x => x.Id == item).FirstOrDefaultAsync();
                        dasl.Add(new dataASemester
                        {
                            SemesterId = semesterData.Id,
                            SemesterName = semesterData.Name
                        });
                    }
                    full.Add(dasl);
                }
                return Ok(new { result = "ok", subsemList = full });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetStudentsStandings")]
        [HttpGet]
        public async Task<IActionResult> GetStudentsStandings(int childid, int subjectid, int standardid, int sectionid, [FromQuery] int[] semesters, int flag = 0)
        {
            var subsecid = await db.MSubjectsectionmappings.Where(x => x.SubjectId == subjectid && x.SectionId == sectionid).Select(w => w.Id).FirstOrDefaultAsync();
            if (subsecid == null)
            {
                return BadRequest("Subject or section arent mapped");
            }

            List<studentsTabModel> stdm = new List<studentsTabModel>();

            if (flag == 0) // vs. Grade
            {
                foreach (var semid in semesters)
                {
                    studentsTabModel model = new studentsTabModel();
                    model.semName = await db.MSemestertestsmappings.Where(x => x.Id == semid).Select(w => w.Name).FirstOrDefaultAsync();
                    model.percentage = await dataAnalysisServices.GetSubjectSemesterPercentage(semid, childid, subjectid);
                    model.position = await dataAnalysisServices.GetSemesterPositionVsSection(semid, subsecid, (double)model.percentage);
                    stdm.Add(model);

                }
            }
            else if (flag == 1) // vs. Class
            {
                foreach (var semid in semesters)
                {
                    studentsTabModel model = new studentsTabModel();
                    model.semName = await db.MSemestertestsmappings.Where(x => x.Id == semid).Select(w => w.Name).FirstOrDefaultAsync();
                    model.percentage = await dataAnalysisServices.GetSubjectSemesterPercentage(semid, childid, subjectid);
                    model.position = await dataAnalysisServices.GetSemesterPositionVsClass(semid, standardid, subjectid, (double)model.percentage);
                    stdm.Add(model);

                }
            }

            return Ok(stdm);
        }

        #endregion
    }
}
