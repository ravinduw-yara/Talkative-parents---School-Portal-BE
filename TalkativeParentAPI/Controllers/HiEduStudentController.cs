using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Repository.DBContext;
using Services;
using CommonUtility.RequestModels;
using System.Net;
using System.Linq;
using System.Collections.Generic;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class HiEduStudentController : ControllerBase
    {
        readonly TpContext db = new TpContext();
        private readonly TpContext db1 = new TpContext();
        private readonly IMHiEduCourseService mHiEduCourseService;
        private readonly IMHiEduChildInfoStudentService mHieduChildInfoStudentServic;
        private readonly IMHiEduCoursesemestersExamsService mHieduCoursesemesterExamsService;

        public HiEduStudentController(
            IMHiEduCourseService _mHiEduCourseService,
              IMHiEduChildInfoStudentService _mHieduChildInfoStudentServic,
                IMHiEduCoursesemestersExamsService _mHieduCoursesemesterExamService
            )
        {
            mHiEduCourseService = _mHiEduCourseService;
            mHieduChildInfoStudentServic = _mHieduChildInfoStudentServic;
           mHieduCoursesemesterExamsService = _mHieduCoursesemesterExamService;
        }

      
        [Route("GetHieduStudentDetailsByChildId")]
        [HttpGet]
        public async Task<IActionResult> GetHieduStudentDetailsByChildId(Guid token, int schoolid, int childid, int nuofRows = 10, int pageNumber = 1)
        {
            try
            {
                var res = await this.mHieduChildInfoStudentServic.GetHiEduStudentDetailsByChildIdForAPI(token, schoolid, childid, nuofRows, pageNumber);
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


        //[Route("GetHieduStudentDetailsByChildIds")]
        //[HttpGet]
        //public async Task<IActionResult> GetHieduStudentDetailsByChildIds(int parentid, int childid, int nuofRows = 10, int pageNumber = 1)
        //{
        //    try
        //    {
        //        var res = await this.mHieduChildInfoStudentServic.GetStudentDetailsParentByChildIdAPI(parentid, childid, nuofRows, pageNumber);
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



        [Route("UpdateHieduStudentInfo")]
        [HttpPost]
        public async Task<IActionResult> UpdateHieduStudentInfo(Guid token, GetStudentParentsListModel model)
        {
            try
            {
                var res = await this.mHieduChildInfoStudentServic.UpdateHieduStudentInfoAPI(token, model);
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

        [Route("DeleteHiEduStudentByChildId")]
        [HttpPost]
        public async Task<IActionResult> DeleteHiEduStudentByChildId(int childid)
        {
            try
            {
                var res = await this.mHieduChildInfoStudentServic.DeleteHiEduStudentByChildId(childid);
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


        [Route("PostCoursesemesterExam")]
        [HttpPost]
        public async Task<IActionResult> PostCoursesemesterExam([FromBody] MHiEduCourseSemesterExamModel model)
        {
            try
            {
                /*var SemesterId = db.MHieduSemesters.Where(a => a.Id == model.SemesterId).Select(b => b.Id).FirstOrDefault();
                var CourseId = db.MHieducourses.Where(a => a.Id == model.CourseId).Select(b => b.Id).FirstOrDefault();
                
                var temp = await this.mHieduCoursesemesterExamService.GetEntityByName(model.Exam);
                if (temp.Count() > 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Msg = "Course Semesetr Exam already exists."
                    });
                }*/
                var id = await this.mHieduCoursesemesterExamsService.AddEntity(new MHiEduCourseSemesterExam
                {
                    Exam = model.Exam,
                    SemesterCourseMappingId = model.SemesterCourseMappingId



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


    }
}
