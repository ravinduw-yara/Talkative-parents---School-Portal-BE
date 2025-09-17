using CommonUtility;
using CommonUtility.RequestModels;
using K4os.Compression.LZ4.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static CommonUtility.RequestModels.MAcademicyeardetailModel;
using static Services.MAcadamicyearService;
using static Services.MStandardsectionmappingService;
using static TalkativeParentAPI.Controllers.DataAnalyticsController;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class DataAnalyticsMappingController : ControllerBase
    {
        //Hardcoded for now. Needs to be discussed on the DB side and assigned a value.
        readonly short DeleteStatusID = 3;
        private readonly IMSubSubjectService mSubSubjectService;
        private readonly IMSubjectsService mSubjectService;
        private readonly IMLevelService mLevelService;
        private readonly IMTeacherSubjectMappingService mTeacherSubjectMappingService;
        private readonly IMSemesterTestsMappingService mSemesterTestsMappingService;
        private readonly IMSubjectTestMappingService mSubjectTestMappingService;
        private readonly IMSubjectSectionMappingService mSubjectSectionMappingService;
        private readonly IMSemesterYearMappingService mSemesterYearMappingService;
        private readonly IMStandardsectionmappingService mStandardsectionmappingService;
        private readonly IMAcadamicyearService mAcadamicyearService;
        private readonly IMChildinfoService mChildinfoService;
        private readonly TpContext db = new TpContext();
        private readonly TpContext db1 = new TpContext(); 
        private readonly TpContext db2 = new TpContext();

        public DataAnalyticsMappingController(
            IMSubjectsService _mSubjectService,
            IMSubSubjectService _mSubSubjectService,
            IMLevelService _mLevelService,
            IMTeacherSubjectMappingService _mTeacherSubjectMappingService,
            IMSemesterTestsMappingService _mSemesterTestsMappingService,
            IMSubjectTestMappingService _mSubjectTestMappingService,
            IMSubjectSectionMappingService _mSubjectSectionMappingService,
            IMSemesterYearMappingService _mSemesterYearMappingService,
            IMStandardsectionmappingService _mStandardsectionmappingService,
            IMChildinfoService _mChildinfoService,
            IMAcadamicyearService _mAcadamicyearService
        )
        {
            mSubSubjectService = _mSubSubjectService;
            mSubjectService = _mSubjectService;
            mLevelService = _mLevelService;
            mTeacherSubjectMappingService = _mTeacherSubjectMappingService;
            mSemesterTestsMappingService = _mSemesterTestsMappingService;
            mSubjectTestMappingService = _mSubjectTestMappingService;
            mSubjectSectionMappingService = _mSubjectSectionMappingService;
            mSemesterYearMappingService = _mSemesterYearMappingService;
            mStandardsectionmappingService = _mStandardsectionmappingService;
            mChildinfoService = _mChildinfoService;
            mAcadamicyearService = _mAcadamicyearService;
        }
        #region AcademicYear
        
       
        [HttpPost]
        [Route("UpdateCurrentYear")]
        public async Task<IActionResult> UpdateCurrentYear(UpdateCurrentYearModel model)
        {
            try
            {
                var result = await mAcadamicyearService.UpdateCurrentYear(model);
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
        [Route("PostAcadamicYear")]
        [HttpPost]
        public async Task<IActionResult> PostAcadamicYear([FromBody] MAcademicyeardetailModel model)
        {
            try
            {
                var schid = db.MSchools.Where(a => a.Id == model.SchoolId).Select(b => b.Id).FirstOrDefault();
               // var temp = await this.mAcadamicyearService.GetEntityBySchoolID();
                var temp = db.MAcademicyeardetails.Where(x => x.SchoolId == model.SchoolId && x.YearName == model.YearName).FirstOrDefault();
                if (temp != null)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MAcadamicYear already exists."
                    });
                }
                var id = await this.mAcadamicyearService.AddEntity(new MAcademicyeardetail
                {
                    YearName = model.YearName,
                    SchoolId = schid,
                    //Createdby = model.Createdby,
                    // Statusid = model.Statusid,
                    // Createddate = DateTime.UtcNow,
                    // Modifiedby = model.Createdby,
                    // Modifieddate = DateTime.UtcNow

                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
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
        [Route("UpdateAcadamicYear")]
        [HttpPost]
        public async Task<IActionResult> UpdateAcadamicYear([FromBody] MAcademicyeardetailUpdateModel model)
        {
            try
            {
                var temp = await this.mAcadamicyearService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return BadRequest(new
                    {
                        Msg = "Academicyear doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.YearName))
                    temp.YearName = model.YearName;

                return Ok(new
                {
                    ID = await this.mAcadamicyearService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "Academicyear updated successfully."
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
        #endregion
        #region Admin 
        //sanduni 9/12/2024
        [Route("PostSchool")]
        [HttpPost]
        public async Task<object> PostSchool(SchoolRequest model)
        {
            try
            {
                // Call a service to handle the database operation
                var result = await mStandardsectionmappingService.AddSchool(model);

                if (result == null)
                {
                    return BadRequest("Failed to add school");
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
                    Error = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }
        //2/July/2025 Jaliya
        [Route("AddGradeAndSection")]
        [HttpPost]
        public async Task<IActionResult> AddGradeAndSection([FromBody] PostGradeSectionModel model)
        {
            try
            {
                var res = await this.mStandardsectionmappingService.AddGradeAndSection(model);
                if (string.IsNullOrWhiteSpace(res))
                {
                    return BadRequest("No response from service");
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message
                });
            }
        }
        //sanduni 19/9/2024
        [Route("PostGradeSectionMapping")]
        [HttpPost]
        public async Task<object> PostGradeSectionMapping(GradeSectionMappingAddModel model)
        {
            try
            {
                var branchid = db.MBranches.Where(a => a.Schoolid == model.SchoolId).Select(b => b.Id).FirstOrDefault();

                var id = await this.mStandardsectionmappingService.AddEntity(new MStandardsectionmapping
                {
                    Name = model.Grade,
                    Description = model.Grade,
                    Parentid = null,
                    Branchid = branchid,
                    Businessunittypeid = 1,
                    Createddate = DateTime.UtcNow,
                    Modifieddate = DateTime.UtcNow,
                    Createdby = model.Schooluserid,
                    Modifiedby = model.Schooluserid,
                    Statusid = 1
                });
                var sectionsubjectmapid = 0;
                if (id != 0)
                {
                    foreach (var item in model.SectionNames)
                    {
                        sectionsubjectmapid = await this.mStandardsectionmappingService.AddEntity(new MStandardsectionmapping
                        {
                            Name = item,
                            Description = item,
                            Parentid = id,
                            Branchid = branchid,
                            Businessunittypeid = 2,
                            Createddate = DateTime.UtcNow,
                            Modifieddate = DateTime.UtcNow,
                            Createdby = model.Schooluserid,
                            Modifiedby = model.Schooluserid,
                            Statusid = 1,
                           
                            
                        });
                    }
                }
                return Ok(new
                {
                    id,
                    sectionsubjectmapid,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
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
        [Route("UpdateGradeAndSectionName")]
        [HttpPost]
        public async Task<IActionResult> UpdateGradeAndSectionName([FromBody] UpdateGradeSectionNameModel model)
        {
            try
            {
                var res = await this.mStandardsectionmappingService.UpdateGradeAndSectionName(model);
                if (string.IsNullOrWhiteSpace(res))
                    return BadRequest("No response from service");
                return Ok(res);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Data = ex.Message });
            }
        }

        #endregion
        #region Course
        //[Route("PostCourses")]
        //[HttpPost]
        //public async Task<IActionResult> PostCourses([FromBody] MHieducoursesModel model)
        //{
        //    try
        //    {

        //        var id = await this.mHiEduCourseService.AddEntity(new MHieducourses
        //        {
        //            // YearName = model.YearName,

        //            Course = model.Course,
        //            SchoolId = model.SchoolId,
        //            DepartmentId = model.DepartmentId,
        //            Duration = model.Duration,


        //        });
        //        return Ok(new
        //        {
        //            id,
        //            StatusCode = HttpStatusCode.OK,
        //            Msg = "Created successfully"
        //        });
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
        #endregion
        #region Level

        [Route("GetLevels")]
        [HttpGet]
        public async Task<IActionResult> GetLevels(int schoolid)
        {
            try
            {
                var Levels = db.MLevels.Where(w => w.schoolid == schoolid).ToList();

                if (Levels != null)
                {
                    List<MLevel> subsubjectlist = new List<MLevel>();
                    foreach (var item in Levels)
                    {
                        subsubjectlist.Add(new MLevel
                        {
                            Id = item.Id,
                            levels = item.levels,
                            schoolid = item.schoolid
                        });

                    }
                    return Ok(subsubjectlist);
                }

                else
                {
                    return NotFound("No valid subject id");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        //sanduni SetGradeLevel
        [Route("SetGradeLevel")]
        [HttpPost]
        public async Task<IActionResult> SetGradeLevel([FromBody] postGradeLeveModel model)
        {
            try
            {
                var res = await this.mSubjectService.SetGradeLevelAPI(model);
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
        //dinidu 
        [Route("PostLevel")]
        [HttpPost]
        public async Task<IActionResult> PostLevel([FromBody] MLevelModel model)
        {
            try
            {
                var sclid = db.MSchools.Where(a => a.Id == model.schoolid).Select(b => b.Id).FirstOrDefault();
                var temp = db.MLevels.Where(x => x.levels == model.levels && x.schoolid == model.schoolid).FirstOrDefault();
                if (temp != null)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MLevel already exists."
                    });
                }
                var ids = await this.mLevelService.AddEntity(new MLevel
                {

                    levels = model.levels,
                    schoolid = sclid


                });
                return Ok(new
                {
                    ids,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = $"Message: {ex.Message} | Inner Exception: {ex.InnerException?.Message}",
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }
        #endregion

        #region SubSubjects
        //Jaliya
        [Route("GetSubSubjects")]
        [HttpGet]
        public async Task<IActionResult> GetSubSubjects(int SubjectID)
        {
            try
            {
                var SubSubjects = db.MSubSubjects.Where(w => w.SubjectId == SubjectID).ToList();

                if (SubjectID != 0)
                {
                    List<MSubSubject> subsubjectlist = new List<MSubSubject>();
                    foreach (var item in SubSubjects)
                    {
                        subsubjectlist.Add(new MSubSubject
                        {
                            Id = item.Id,
                            SubjectId = item.SubjectId,
                            SubSubject = item.SubSubject,
                            Precentage = item.Precentage,
                            SubMaxMarks = item.SubMaxMarks,
                        });

                    }
                    return Ok(subsubjectlist);
                }

                else
                {
                    return NotFound("No valid subject id");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
       
        [Route("PostSubSubjects")]
        [HttpPost]
        public async Task<IActionResult> PostSubSubjects([FromBody] MSubSubjectModel model)
        {
            try
            {
                var Subid = db.MSubjects.Where(a => a.Id == model.SubjectId).Select(b => b.Id).FirstOrDefault();
                var data1 = db.MSubSubjects.Where(y => y.SubjectId == model.SubjectId && y.SubSubject == model.SubSubjectName).FirstOrDefault();
                if (data1 != null)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MSubSubject already exists."
                    });
                }
                var ids = await this.mSubSubjectService.AddEntity(new MSubSubject
                {

                    SubSubject = model.SubSubjectName,
                    Precentage = model.Percentage,
                    SubMaxMarks = model.SubMaxMarks,
                    ExcelSheetOrder = model.ExcelSheetOrder,
                    SubjectId = Subid


                });
                return Ok(new
                {
                    ids,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
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


        //// Dinidu Modify by 2023/03/16 SubSubject Maxmarks
        //private async Task<object> setsubsubjectsubmaxmarksSP(postSubSubjectSubmaxmarksModel model)
        //{
        //    try
        //    {

        //        if (model != null)
        //        {
        //            var subsubjectid = model.SubSubjectId;
        //            var Submaxmarks = model.Submaxmarks;

        //            var connectionstring = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

        //            using (SqlConnection connection = new SqlConnection(connectionstring))
        //            {
        //                connection.Open();
        //                SqlCommand command = new SqlCommand(ApplicationConstants.updateSubSubjecSubMaxMarks, connection);
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.Add(new SqlParameter("@SubSubjectId", SqlDbType.Int));
        //                command.Parameters["@SubSubjectId"].Value = subsubjectid;
        //                command.Parameters.Add(new SqlParameter("@SubMaxMarks", SqlDbType.Int));
        //                command.Parameters["@SubMaxMarks"].Value = Submaxmarks;
        //                command.ExecuteNonQuery();

        //            }
        //            db.SaveChanges();
        //            return ("Successfully added");
        //        }
        //        else
        //        {
        //            return ("UnSuccessfully added");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}
        //// Dinidu Modify by 2023/03/16 SubSubject Maxmarks
        //public async Task<object> setsubsubjectsubmaxmarksAPI(postSubSubjectSubmaxmarksModel model)
        //{
        //    try
        //    {
        //        if (model.SubSubjectId != 0)
        //        {
        //            var objeresult = setsubsubjectsubmaxmarksSP(model);
        //            if (objeresult != null)
        //            {
        //                var items = objeresult;
        //                var obj = new
        //                {
        //                    items = items
        //                };
        //                return obj;
        //            }
        //            else
        //            {
        //                return (new
        //                {
        //                    Message = "No data Found",
        //                    StatusCode = HttpStatusCode.NotFound
        //                });
        //            }
        //        }
        //        else
        //        {
        //            return (new
        //            {
        //                Message = " data Found",
        //                StatusCode = HttpStatusCode.NotFound
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        #endregion

        #region SubSubjects

        //[Route("GetSubSubjects")]
        //[HttpGet]
        //public async Task<IActionResult> GetSubSubjects(int SubjectID)
        //{
        //    try
        //    {
        //        var SubSubjects = db.MSubSubjects.Where(w => w.SubjectId == SubjectID).ToList();

        //        if (SubjectID != 0)
        //        {
        //            List<MSubSubject> subsubjectlist = new List<MSubSubject>();
        //            foreach (var item in SubSubjects)
        //            {
        //                subsubjectlist.Add(new MSubSubject
        //                {
        //                    Id = item.Id,
        //                    SubjectId = item.SubjectId,
        //                    SubSubject = item.SubSubject,
        //                    Precentage = item.Precentage,
        //                    SubMaxMarks = item.SubMaxMarks,
        //                });

        //            }
        //            return Ok(subsubjectlist);
        //        }

        //        else
        //        {
        //            return NotFound("No valid subject id");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        //[Route("GetSubSubjects")]
        //[HttpGet]
        //public async Task<IActionResult> GetSubSubjects(int SubjectID)
        //{
        //    try
        //    {
        //        var SubSubjects = db.MSubSubjects.Where(w => w.SubjectId == SubjectID).ToList();

        //        if (SubjectID != 0)
        //        {
        //            List<MSubSubject> subsubjectlist = new List<MSubSubject>();
        //            foreach (var item in SubSubjects)
        //            {
        //                subsubjectlist.Add(new MSubSubject
        //                {
        //                    Id = item.Id,
        //                    SubjectId = item.SubjectId,
        //                    SubSubject = item.SubSubject,
        //                    Precentage = item.Precentage,
        //                });

        //            }
        //            return Ok(subsubjectlist);
        //        }

        //        else
        //        {
        //            return NotFound("No valid subject id");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        #endregion
        #region Subjects

        [Route("GetSubjects")]
        [HttpGet]
        public async Task<IActionResult> GetSubjects(int schoolid)
        {
            try
            {
                var res = await this.mSubjectService.GetEntityBySchoolID2(schoolid);
                if (res == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "Subjects not found for this school."
                    });
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

        [Route("PostSubject")]
        [HttpPost]
        public async Task<IActionResult> PostSubject([FromBody] postSubjectmapSectionModel model)
        {
            try
            {
                var branchid = db.MBranches.Where(a => a.Schoolid == model.Schoolid).Select(b => b.Id).FirstOrDefault();
                //if (temp.Count() > 0)
                //{
                //    return BadRequest(new
                //    {
                //        StatusCode = HttpStatusCode.BadRequest,
                //        Msg = "MSubject already exists."
                //    });
                //}
                var id = await this.mSubjectService.AddEntity(new MSubject
                {
                    Name = model.Name,
                    Description = model.Description,
                    BranchId = branchid,
                    Createdby = model.Createdby,
                    Statusid = 1,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow,
                    DRCSubjectOrder = model.DrcSubjectOrder,
                });
                var sectionsubjectmapid = 0;
                if (id != 0)
                {
                    foreach (var item in model.sectionidlist)
                    {
                        sectionsubjectmapid = await this.mSubjectSectionMappingService.AddEntity(new MSubjectsectionmapping
                        {
                            SubjectId = id,
                            SectionId = item.sectionid,
                            Createdby = model.Createdby,
                            Statusid = 1,
                            Createddate = DateTime.UtcNow,
                            Modifiedby = model.Createdby,
                            Modifieddate = DateTime.UtcNow,
                        });
                    }
                }
                return Ok(new
                {
                    id,
                    sectionsubjectmapid,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
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

        [Route("UpdateSubject")]
        [HttpPost]
        public async Task<IActionResult> UpdateSubject([FromBody] MSubjectUpdateModel model)
        {
            try
            {
                var temp = await this.mSubjectService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return BadRequest(new
                    {
                        Msg = "MSubject doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Name))
                    temp.Name = model.Name;
                if (!string.IsNullOrEmpty(model.Description))
                    temp.Description = model.Description;
                //if (model.Statusid.HasValue)
                //    temp.Statusid = model.Statusid;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                if (model.Createdby > 0)
                    temp.Createdby = model.Createdby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mSubjectService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MSubject updated successfully."
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

        [Route("DeleteSubject")]
        [HttpPost]
        public async Task<IActionResult> DeleteSubject(int Id)
        {
            try
            {
                var del_rec = await this.mSubjectService.GetEntityIDForUpdate(Id);
                if (del_rec != null)
                {
                    await this.mSubjectService.DeleteEntity(del_rec);
                    return Ok(new { result = "OK", message = "Deleted Successfully" });
                }
                else
                {
                    return BadRequest(new { result = "error", message = "No record found" });
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

        #endregion

        #region Teacher-Subject
        [Route("GetSingleTeacherDetails")]
        [HttpGet]
        public async Task<IActionResult> GetSingleTeacherDetails(int teacherId)
        {
            try
            {
                var res = await this.mChildinfoService.GetSingleTeacherDetails(teacherId);
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
        [Route("GetDropDownTeacherDetails")]
        [HttpGet]
        public async Task<IActionResult> GetDropDownTeacherDetails(int branchId)
        {
            try
            {
                var res = await this.mChildinfoService.GetDropdownTeacherDetails(branchId);
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
        [Route("GetTeacherDetailsForSection")]
        [HttpGet]
        public async Task<IActionResult> GetTeacherDetailsForSection(int academicYearId, int sectionId)
        {
            try
            {
                var res = await this.mChildinfoService.GetTeacherDetailsListAsync(academicYearId, sectionId);
                if (res == null || !res.Any())
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
        [Route("GetTeacherDetailsSubjects")]
        [HttpGet]
        public async Task<IActionResult> GetTeacherDetailsSubjects(int academicYearId, int schoolUserId)
        {
            try
            {
                var res = await this.mChildinfoService.GetTeacherDetailsSubjects(academicYearId, schoolUserId);
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
        [Route("PostSchoolTeacherDetails")]
        [HttpPost]
        public async Task<IActionResult> PostSchoolTeacherDetails(TeacherAddModel model)
        {
            try
            {
                var res = await this.mChildinfoService.SchoolAddTeacherDetails(model);
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
        [Route("PostTeacherSubject")]
        [HttpPost]
        public async Task<IActionResult> PostTeacherSubject(int TeacherId, int SubjectId, int SectionId, int CreatedBy, int ModifiedBy, int AcademicYearId)
        {
            try
            {
                var subsecid = db.MSubjectsectionmappings.Where(x => x.SubjectId == SubjectId && x.SectionId == SectionId).FirstOrDefault();
                var data = db.MTeachersubjectmappings.Where(x => x.SubjectSectionId == subsecid.Id && x.TeacherId == TeacherId).FirstOrDefault();

                if (data != null)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "Teacher is already mapped to this section and subject"
                    });
                }
                var id = await this.mTeacherSubjectMappingService.AddEntity(new MTeachersubjectmapping
                {
                    TeacherId = TeacherId,
                    SubjectSectionId = subsecid.Id,
                    Createdby = CreatedBy,
                    Statusid = 1,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = ModifiedBy,
                    Modifieddate = DateTime.UtcNow,
                    AcademicYearID = AcademicYearId,
                    SubjectId = SubjectId,
                    SectionId = SectionId
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
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

        [Route("DeleteTeacherSubject")]
        [HttpPost]
        public async Task<IActionResult> DeleteTeacherSubject(int Id)
        {
            try
            {
                var del_rec = await this.mTeacherSubjectMappingService.GetEntityIDForUpdate(Id);
                if (del_rec != null)
                {
                    await this.mTeacherSubjectMappingService.DeleteEntity(del_rec);
                    return Ok(new { result = "OK", message = "Deleted Successfully" });
                }
                else
                {
                    return BadRequest(new { result = "error", message = "No record found" });
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

        #endregion

        #region Semester-Tests

        [HttpGet]
        [Route("GetSemesters")]
        public async Task<IActionResult> GetSemesters(int schoolid)
        {
            try
            {
                var res = await Task.FromResult(db.MSemestertestsmappings.Where(w => w.Branch.Schoolid == schoolid && w.SemesterId == null).Select(w => new { w.Id, w.Name }).OrderBy(w => w.Name));
                if (res.Count() > 0)
                {
                    return Ok(res);
                }
                return BadRequest("No semesters found for this school");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [Route("PostSemesters")]
        [HttpPost]
        public async Task<IActionResult> PostSemesters([FromBody] MSemesterTestsMappingModel model)
        {
            try
            {
                //var temp = db.MSemestertestsmappings.Where(b => b.Name == model.Name && b.BranchId == model.Branchid).FirstOrDefault();
                var branchid = db.MBranches.Where(a => a.Schoolid == model.Schoolid).Select(b => b.Id).FirstOrDefault();
                //var temp1 = db1.MSemestertestsmappings.Where(a => a.BranchId == branchid).FirstOrDefault();
                if (model.Name == null && model.Schoolid == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "Enter Correct Semester Details"
                    });
                }
                else
                {
                    var id = await this.mSemesterTestsMappingService.AddEntity(new MSemestertestsmapping
                    {
                        Name = model.Name,
                        Description = model.Description,
                        SemesterId = null,
                        BranchId = branchid,
                        Createdby = model.Createdby,
                        Statusid = 1,
                        Createddate = DateTime.UtcNow,
                        Modifiedby = model.Modifiedby,
                        Modifieddate = DateTime.UtcNow,
                    });
                    if (id > 0)
                    {
                        var data1 = db.MSemesteryearmappings.Where(x => x.SemesterId == id && x.AcademicYearId == model.AcademicYearId).FirstOrDefault();
                        if (data1 == null)
                        {
                            await this.mSemesterYearMappingService.AddEntity(new MSemesteryearmapping
                            {
                                SemesterId = id,
                                AcademicYearId = (int)model.AcademicYearId,
                                Statusid = 1,
                                Createddate = DateTime.UtcNow,
                                Modifiedby = model.Modifiedby,
                                Modifieddate = DateTime.UtcNow,
                                Createdby = model.Createdby
                            });
                        }
                        return Ok(new
                        {
                            id,
                            StatusCode = HttpStatusCode.OK,
                            Msg = "Created successfully"
                        });
                    }
                    else
                    {
                        return BadRequest("Semester could not be added");
                    }
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

        [Route("PostSemesterTests")]
        [HttpPost]
        public async Task<IActionResult> PostSemesterTests([FromBody] MSemesterTestsMappingModel model)
        {
            try
            {
               // var temp2 = await this.mSemesterTestsMappingService.GetEntityBySemesterID((int)model.Semesterid);
                var temp = db.MSemestertestsmappings.Where(b => b.Name == model.Name && b.SemesterId == model.Semesterid).FirstOrDefault();
                var branchid = db.MBranches.Where(a => a.Schoolid == model.Schoolid).Select(b => b.Id).FirstOrDefault();
                //var temp1 = db1.MSemestertestsmappings.Where(a => a.BranchId == branchid).FirstOrDefault();
               
                if (temp != null)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "Test for this semester already exists."
                    });
                }
                var id = await this.mSemesterTestsMappingService.AddEntity(new MSemestertestsmapping
                {
                    Name = model.Name,
                    Description = model.Description,
                    SemesterId = model.Semesterid,
                    BranchId = branchid,
                    Createdby = model.Createdby,
                    Statusid = 1,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Modifiedby,
                    Modifieddate = DateTime.UtcNow,
                });
                return Ok(new
                {
                    id,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Created successfully"
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

        [Route("PostSemesterYear")]
        [HttpPost]
        public async Task<IActionResult> PostSemesterYear([FromBody] MSemesterTestsMappingUpdateModel model)
        {
            try
            {
                var data = db.MSemestertestsmappings.Where(x => x.Id == model.Id).FirstOrDefault();
                if (data == null)
                {
                    return BadRequest("Semester doesnot exist");
                }
                else
                {
                    var data1 = db.MSemesteryearmappings.Where(y => y.SemesterId == model.Id && y.AcademicYearId == model.AcademicYearId).FirstOrDefault();
                    if (data1 == null)
                    {
                        var id = await this.mSemesterYearMappingService.AddEntity(new MSemesteryearmapping
                        {
                            SemesterId = model.Id,
                            AcademicYearId = (int)model.AcademicYearId,
                            Statusid = 1,
                            Createddate = DateTime.UtcNow,
                            Modifiedby = model.Modifiedby,
                            Modifieddate = DateTime.UtcNow,
                            Createdby = model.Createdby
                        });

                        if (id > 0)
                        {
                            return Ok(new { result = "OK", message = "Semester year mapped successfully" });
                        }
                        else
                        {
                            return BadRequest(new { result = "error", message = "Semester is already mapped to current year" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { result = "error", message = "Semester for the current year already exists" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("RemoveSemester")]
        [HttpPost]
        public async Task<IActionResult> RemoveSemester(int semesterid, int schoolid)
        {
            try
            {
                var data = db.MSemestertestsmappings.Where(x => x.Branch.Schoolid == schoolid && x.SemesterId == semesterid).FirstOrDefault();
                var data1 = db.MSemestertestsmappings.Where(y => y.Branch.Schoolid == schoolid && y.Id == semesterid).FirstOrDefault();
                if (data != null)
                {
                    return BadRequest("Semester is alreaddy mapped to a test");
                }
                else
                {
                    var del_rec = await this.mSemesterTestsMappingService.GetEntityIDForUpdate(data1.Id);
                    if (del_rec != null)
                    {
                        await this.mSemesterTestsMappingService.DeleteEntity(del_rec);
                        return Ok(new { result = "OK", message = "Deleted Successfully" });
                    }
                    else
                    {
                        return BadRequest(new { result = "error", message = "No record found" });
                    }
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

        [Route("RemoveSemesterTest")]
        [HttpPost]
        public async Task<IActionResult> RemoveSemesterTest(int Id)
        {
            try
            {
                var del_rec = await this.mSemesterTestsMappingService.GetEntityIDForUpdate(Id);
                var res = db.MSubjecttestmappings.Where(x => x.TestId == Id).FirstOrDefault();
                if (del_rec == null || res != null)
                {
                    return BadRequest(new { result = "error", message = "Test not found or is already mapped" });

                }
                else
                {
                    await this.mSemesterTestsMappingService.DeleteEntity(del_rec);
                    return Ok(new { result = "OK", message = "Deleted Successfully" });
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

        [Route("UpdateSemesters")]
        [HttpPost]
        public async Task<IActionResult> UpdateSemesters([FromBody] MSemesterTestsMappingUpdateModel model)
        {
            try
            {
                var temp = await this.mSemesterTestsMappingService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return BadRequest(new
                    {
                        Msg = "MSemester doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Name))
                    temp.Name = model.Name;
                if (!string.IsNullOrEmpty(model.Description))
                    temp.Description = model.Description;
                //if (model.Branchid.HasValue)
                //    temp.BranchId = (int)model.Branchid;
                //if (model.Statusid.HasValue)
                //    temp.Statusid = model.Statusid;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                if (model.Createdby > 0)
                    temp.Createdby = model.Createdby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mSemesterTestsMappingService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MSemester updated successfully."
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

        [Route("UpdateSemesterTests")]
        [HttpPost]
        public async Task<IActionResult> UpdateSemesterTests([FromBody] MSemesterTestsMappingUpdateModel model)
        {
            try
            {
                var temp = await this.mSemesterTestsMappingService.GetEntityIDForUpdate(model.Id);
                if (temp == null)
                {
                    return BadRequest(new
                    {
                        Msg = "MTest doesn't exist with this Id.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (!string.IsNullOrEmpty(model.Name))
                    temp.Name = model.Name;
                if (!string.IsNullOrEmpty(model.Description))
                    temp.Description = model.Description;
                //if (model.Branchid.HasValue)
                //    temp.BranchId = (int)model.Branchid;
                //if (model.Statusid.HasValue)
                //    temp.Statusid = model.Statusid;
                if (model.Modifiedby.HasValue)
                    temp.Modifiedby = model.Modifiedby;
                if (model.Createdby > 0)
                    temp.Createdby = model.Createdby;
                temp.Modifieddate = DateTime.UtcNow;

                return Ok(new
                {
                    ID = await this.mSemesterTestsMappingService.UpdateEntity(temp),
                    StatusCode = HttpStatusCode.OK,
                    Message = "MTest updated successfully."
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

        [Route("PostTestSectionsMapping")]
        [HttpPost]
        public async Task<IActionResult> PostTestSectionsMapping([FromBody] MSubjectTestMappingModel model)
        {
            try
            {
                var res = await this.mSubjectTestMappingService.PostTestSections(model);
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

        [Route("GetSemesterTestMapping")]
        [HttpGet]
        public async Task<IActionResult> GetSemesterTestMapping(int schoolid,int levelid)
        {
            try
            {
                var res = await this.mSemesterTestsMappingService.GetSemesterTestMappingData(schoolid,levelid);
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

        #region SubjectSectionMapping

        [Route("AddSubjectSectionMapping")]
        [HttpPost]
        public async Task<IActionResult> AddSubjectSectionMapping(MSubjectSectionMappingModel model)
        {
            try
            {
                var res = await this.mSubjectSectionMappingService.AddSubjectSectionMappingData(model);
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

        [Route("ShowSubjectSectionMapping")]
        [HttpGet]
        public async Task<IActionResult> ShowSubjectSectionMapping(int schoolid, int levelId, int standardId)
        {
            try
            {
                var res = await this.mSubjectSectionMappingService.GetSubjectSectionMappingData(schoolid, levelId, standardId);
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

        #region SubjectTestMapping

        [Route("AddSubjectTestMapping")]
        [HttpPost]
        public async Task<IActionResult> AddSubjectTestMapping(SubTestMapModel model)
        {
            try
            {
                var res = await this.mSubjectTestMappingService.AddSubjectTestMappingData(model);
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

        [Route("GetSubjectTestMapping")]
        [HttpGet]
        public async Task<IActionResult> GetSubjectTestMapping(int SchoolId)
        {
            try
            {
                var res = await this.mSubjectTestMappingService.GetSubjectTestMappingData(SchoolId);
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
    }
}

