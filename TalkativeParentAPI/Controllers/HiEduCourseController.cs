using CommonUtility.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Data;
using CommonUtility;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using static Services.MHiEduSubjectExamMappingService;
using static Services.MHiEduBatchService;
//using static Services.MHiEduSubjectExamMappingService;
//using static Services.MSubSubjectService;
using static Services.MHiEduSubSubjectService;
using static Services.MHiEduSemesterService;
using static Services.MHiEduCoursesemestersExamsService;
namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class HiEduCourseController : ControllerBase
    {
        //Hardcoded for now. Needs to be discussed on the DB side and assigned a value.
        readonly short DeleteStatusID = 3;
        //JKR 2023/03/30
        private readonly IMHiEduBatchService mHiEduBatchService;
        readonly TpContext db = new TpContext();
       private readonly IMHiEduCourseService mHiEduCourseService;
       private readonly IMHiEduSubjectExamMappingService mHiEduSubjectExamMappingService;
        private readonly IMChildinfoService mChildinfoService;
         private readonly IMHiEduSubSubjectService mHiEduSubSubjectService;
        private readonly IMHiEduSemesterService mHiEduSemesterService;
        private readonly IMHiEduCoursesemestersExamsService mHiEduCoursesemestersExamsService;



        public HiEduCourseController(
            IMHiEduCourseService _mHiEduCourseService,
           IMHiEduBatchService _mHiEduBatchService,
          IMHiEduSubjectExamMappingService _mHiEduSubjectExamMappingService,
            IMChildinfoService _mChildinfoService,
        IMHiEduSubSubjectService _mHiEduSubSubjectService,
       IMHiEduSemesterService _mHiEduSemesterService,
       IMHiEduCoursesemestersExamsService _mHiEduCoursesemestersExamsService
        )

        {
          mHiEduCourseService = _mHiEduCourseService;
          mHiEduBatchService = _mHiEduBatchService;
          mHiEduSubjectExamMappingService = _mHiEduSubjectExamMappingService;
          mChildinfoService = _mChildinfoService;
           mHiEduSubSubjectService = _mHiEduSubSubjectService;
            mHiEduSemesterService = _mHiEduSemesterService;
            mHiEduCoursesemestersExamsService = _mHiEduCoursesemestersExamsService;

        }



        #region SubSubject

        //07/04/2023--- jaliya-----------------
        [Route("GetHiEduSubSubjects")]
        [HttpGet]
        public async Task<IActionResult> GetHiEduSubSubjects(int courseId, int semesterId, int subjectId)
        {
            try
            {
                var res = await this.mHiEduSubSubjectService.GetSubSubjectNames(courseId, semesterId, subjectId);
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

        ////07/04/2023--- jaliya-----------------
        [Route("PostHiEduSubSubjects")]
        [HttpPost]
        public async Task<IActionResult> PostHiEduSubSubjects([FromBody] HieduSubSubjectModel model)
        {
            try
            {              
                var ids = await this.mHiEduSubSubjectService.AddEntity(new MHiEduSubSubject
                {

                    SubSubjectName = model.SubSubjectName,
                    SubjectId = model.SubjectId


                });
                return Ok(new
                {
                    ids,
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Sub Module is created successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    InnerException = ex.InnerException?.Message, // Add this line to get the inner exception message
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        #endregion



        #region Student 
        //Jaliya Hiedu Student
        [Route("PostHiEduStudentDetails")]
        [HttpPost]
        public async Task<IActionResult> PostHieduStudentDetails([FromBody] HiEduStudentAddModel model)
        {
            try
            {
                var res = await this.mChildinfoService.HiEduAddStudentDetails(model);
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
        //jaliya hiedu student
        [Route("GetHiEduChildInfoBySchoolCourseBatch")]
        [HttpGet]
        public async Task<IActionResult> GetHiEduChildInfoBySchoolCourseBatch(int schoolId, int? courseId = null, int? batchId = null)
        {
            try
            {
                var res = await this.mChildinfoService.GetChildInfoBySchoolCourseBatchSP(schoolId, courseId, batchId);
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

        #region  Semester
        //Dinidu 
        [Route("GetHiEduSemester")]
        [HttpGet]
        public async Task<IActionResult> GetHiEduSemester(int? courseid)
        {
            try
            {
                if (courseid == null)
                {
                    return BadRequest("CourseID is requried");
                }

                var semester = db.HiEdu_SemesterCourseMappings.Where(w => w.CourseId == courseid).ToList();

                if (semester != null)
                {
                    List<MHiEduSemester> semesterlist = new List<MHiEduSemester>();
                    foreach (var item in semester)
                    {
                        semesterlist.Add(new MHiEduSemester
                        {
                            Id = item.Id,
                            SemesterName = item.SemesterName,
                            CourseId = item.CourseId
                        });

                    }
                    return Ok(semesterlist);
                }
                return BadRequest("HigEdu Semester are not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    //Dinidu 
    [Route("PostHiEduSemeter")]
    [HttpPost]
    public async Task<IActionResult> PostHiEduSemeter([FromBody] MHiEduSemesterModel model)
    {
        try
            {
                HiEdu_SemesterCourseMapping ctm = new HiEdu_SemesterCourseMapping();
                ctm.SemesterName = model.SemesterName;
                ctm.CourseId = model.CourseId;
                db.HiEdu_SemesterCourseMappings.Add(ctm);
                var dbup1 = db.SaveChanges();

              //  var courseid = db.MHiEduCourses.Where(a => a.Id == model.CourseId).Select(b => b.Id).FirstOrDefault();
            //var ids = await this.mHiEduSemesterService.AddEntity(new MHiEduSemester
            //{
            //    SemesterName = model.SemesterName,
            //    CourseId = courseid

            //});
            return Ok(new
            {
                StatusCode = HttpStatusCode.OK,
                Msg = "Semester is Created successfully"
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

    #region Semester Module

    [Route("GetSemesterModule")]
        [HttpGet]
        public async Task<IActionResult> GetSemesterModule(int courseId, int semesterId, int examId)
        {
            try
            {
                var res = await this.mHiEduSubjectExamMappingService.GetSemesterModuleByCourseAndSemesterIdAsync(courseId, semesterId, examId);
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



        [Route("PostSemesterModules")]
        [HttpPost]
        public async Task<IActionResult> PostSemesterModules([FromBody] HieduSubjectExamMappingModel model)
        {
            try
            {
                var CourseSemesterExamid = db.HiEdu_SemesterCourseMappings.Where(a => a.Id == model.CourseSemesterExamId).Select(a => a.Id).FirstOrDefault();
             
                var ids = await this.mHiEduSubjectExamMappingService.AddEntity(new HiEdu_SubjectExamMapping
                {

                    Subject = model.Subject,
                    CourseSemesterExamId = CourseSemesterExamid


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
                    InnerException = ex.InnerException?.Message, // Add this line to get the inner exception message
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }
        #endregion

        #region Batchs

        [Route("GetHiEduBatchs")]
        [HttpGet]
        public async Task<IActionResult> GetHiEduBatchs(int courseId, int? createdYear = null, int? createdMonth = null, int? createdDate = null)
        {
            try
            {
                var res = await this.mHiEduBatchService.GetHiEduBatchs(courseId, createdYear, createdMonth, createdDate);
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


        [Route("PostHieduBatchs")]
        [HttpPost]
        public async Task<IActionResult> PostHieduBatchs([FromBody] HiEduBatchModel model)
        {
            try
            {
                var couid = db.MHiEduCourses.Where(a => a.Id == model.CourseId).Select(b => b.Id).FirstOrDefault();
                var temp = await this.mHiEduBatchService.GetEntityByName(model.Batch);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "MHiedubatch already exists."
                    });
                }

                DateTime batchCreatedDate = model.Batch_Created_Date;
                var ids = await this.mHiEduBatchService.AddEntity(new MHiEduBatch
                {
                    Batch = model.Batch,
                    CourseId = couid,
                    Created_Year = batchCreatedDate.Year,
                    Created_Month = batchCreatedDate.Month,
                    Created_Date = batchCreatedDate.Day,
                    Batch_Created_Date = batchCreatedDate,
                    StudentCount = model.StudentCount,
                    Status = model.Status
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
                    InnerException = ex.InnerException?.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }


        #endregion

        #region Courses
        //Quanta Get Course List - Sanduni
        [Route("GetCourseList")]
        [HttpGet]
        public async Task<IActionResult> GetCourseList(int SchoolId)
        {
            try
            {
                var res = await this.mHiEduCourseService.GetCourseListForAPI(SchoolId);
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

        //Quanta Create Course  -  Sanduni
        [Route("HiEduCreateCourse")]
        [HttpPost]
        public async Task<IActionResult> HiEduCreateCourse(MHiEduCourses course)
        {
            try
            {
                var res = await this.mHiEduCourseService.HiEduCreateCourse(course);
                if (res == null)
                {
                    return null;
                }
                return Ok(new
                {
                    StatusCode = HttpStatusCode.OK,
                    Msg = "Course is Created successfully"
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


        //Dinidu Exam
        [Route("GetCourseSemesterexam")]
        [HttpGet]
        public async Task<IActionResult> GetCourseSemesterexam(int courseId, int semesterId)
        {
            try
            {
                var res = await this.mHiEduCoursesemestersExamsService.GetCourseSemesterexamAPI(courseId, semesterId);
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
    }
}
