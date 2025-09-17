using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using TalkativeParentAPI_APP.Membership;
using System.Reflection;
using static Services.MStandardsectionmappingService;

namespace TalkativeParentAPI_APP.Controllers
{
    [CustomAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class DrcController : ControllerBase
    {
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext db3 = new TpContext();
        private readonly IMChildschoolmappingService mChildschoolmappingService;
        private readonly IDataAnalysisServices dataAnalysisServices;
        private static HttpActionContext act = new HttpActionContext();

        public DrcController(IMChildschoolmappingService _mChildschoolmappingService, IDataAnalysisServices _dataAnalysisServices)
        {
            mChildschoolmappingService = _mChildschoolmappingService;
            dataAnalysisServices = _dataAnalysisServices;
        }

        [HttpGet]
        public string ExtractToken()
        {
            const string HeaderKeyName = "authToken";
            Request.Headers.TryGetValue(HeaderKeyName, out Microsoft.Extensions.Primitives.StringValues headerValue);
            return (headerValue);
        }

        [Route("GetGCLStatusApp")]
        [HttpGet]
        public IActionResult GetGCLStatusApp(int childid)
        {
            var childschool = db.MChildschoolmappings.Where(w => w.Childid == childid).FirstOrDefault();
            var branchdetails = db.MStandardsectionmappings.Where(w => w.Id == childschool.Standardsectionmappingid).FirstOrDefault();
            int schoolid = (int)db.MBranches.Where(w => w.Id == branchdetails.Branchid).Select(w => w.Schoolid).FirstOrDefault();

            if (schoolid == 0 || schoolid.Equals(null))
            {
                return BadRequest("Schoolid is requried");
            }

            var school = db.MSchools.Where(w => w.Id == schoolid).FirstOrDefault();
            var flag = db.MSchoolDamappings.Where(x => x.Daflag == true && x.SchoolId == schoolid).FirstOrDefault();
            if (school == null || flag == null)
            {
                return Ok(new { Value = "Disabled", Msg = "Digital Report Card is not enabled for your school" });

                //  return BadRequest("School not found");
            }
            else if (flag.Daflag == true)
            {
                return Ok(new { Value = "Enabled" });
            }
            else
            {
                return Ok(new { Value = "Disabled", Msg = "Digital Report Card is not enabled for your school" });
            }
        }

        [Route("GetChildReportDRCStatus")]
        [HttpGet]
        public async Task<IActionResult> GetChildReportDRCStatus(int childid, int academicyearid, int Semesterid, int Examid, int Term)
        {
            try
            {
                if (childid != 0)
                {

                    var Standardsectionmappingid = await db.MChildschoolmappings.Where(ssm => ssm.Childid == childid).Select(ssm => ssm.Standardsectionmappingid).FirstOrDefaultAsync();
                    var BranchId = await db.MStandardsectionmappings.Where(ssm => ssm.Id == Standardsectionmappingid).Select(ssm => ssm.Branchid).FirstOrDefaultAsync();
                    var Schoolid = await db.MBranches.Where(ssm => ssm.Id == BranchId).Select(ssm => ssm.Schoolid).FirstOrDefaultAsync();

                    if (Schoolid == 0 || academicyearid == 0 || Semesterid == 0 || Examid == 0 || Term == 0)
                    {
                        return BadRequest("Schoolid is requried");
                    }

                    var school = await db.MSchoolDamappings.Where(w => w.SchoolId == Schoolid).FirstOrDefaultAsync();
                    if (school != null && school.Daflag == true)
                    {
                        var sectionid = await db.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == academicyearid).Select(w => w.Standardsectionmappingid).FirstOrDefaultAsync();
                        var gradeid = await db.MStandardsectionmappings.Where(z => z.Id == sectionid).Select(z => z.Parentid).FirstOrDefaultAsync();
                        var existsrecord = await db.MStandardyearmappings.Where(y => y.GradeId == gradeid && y.SemesterId == Semesterid && y.ExamId == Examid && y.AcademicYearId == academicyearid && y.FreezeEnable == 1).Select(y => y.Id).FirstOrDefaultAsync();
                        if (existsrecord == 0)
                        {
                            return Ok(new { Value = "Disabled", Msg = "Digital Report Card is not available" });
                           // return BadRequest("This Grade marks is not Finalized"); //Sanduni 28/10/2024
                            //return BadRequest("Disabled");
                        }
                        MChildschoolmapping parentpermission = new MChildschoolmapping();

                        if (Term == 1)
                        {
                            parentpermission = db.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == academicyearid && w.Standardsectionmappingid == sectionid && w.DRCEnable1 == 1).FirstOrDefault();

                        }
                        else if (Term == 2)
                        {
                            parentpermission = db.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == academicyearid && w.Standardsectionmappingid == sectionid && w.DRCEnable2 == 1).FirstOrDefault();

                        }
                        else if (Term == 3)
                        {
                            parentpermission = db.MChildschoolmappings.Where(w => w.Childid == childid && w.AcademicYearId == academicyearid && w.Standardsectionmappingid == sectionid && w.DRCEnable3 == 1).FirstOrDefault();

                        }
                        if (parentpermission == null)
                        {
                            return Ok(new { Value = "Disabled", Msg = "Digital Report Card is not available" });
                        }
                        return Ok(new { Value = "Enabled" });
                    }
                    return Ok(new { Value = "Disabled", Msg = "Digital Report Card is not enabled for your school" });
                }
              return Ok(new { Value = "Childid is Required" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ExtractTokenWithId")]
        public async Task<IActionResult> ExtractTokenWithId(int childid)
        {
            try
            {
                string token = ExtractToken();

                if (childid <= 0)
                {
                    return BadRequest("Invalid childid.");
                }

                var childMapping = await db.MChildschoolmappings
                    .Where(c => c.Childid == childid)
                    .Select(c => new
                    {
                        DrcEnable1 = c.DRCEnable1,
                        DrcEnable2 = c.DRCEnable2,
                        DrcEnable3 = c.DRCEnable3
                    })
                    .FirstOrDefaultAsync();

                if (childMapping == null)
                {
                    return NotFound("Childid not found.");
                }

                return Ok(new
                {
                    Token = token,
                    DrcEnable1 = childMapping.DrcEnable1 == 1,
                    DrcEnable2 = childMapping.DrcEnable2 == 1,
                    DrcEnable3 = childMapping.DrcEnable3 == 1
                });
            }
            catch (Exception ex)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }



        [Route("dataAYearDropdown")]
        [HttpGet]
        public async Task<IActionResult> dataAYearDropdown(int childid)
        {
            try
            {

                var token = ExtractToken();
                var appUserId = await db.TTokens
                                        .Where(x => x.Id.ToString() == token)
                                        .Select(a => a.Referenceid)
                                        .FirstOrDefaultAsync();


                var standardSectionMappingId = await db.MChildschoolmappings
                    .Where(csm => csm.Childid == childid)
                    .Select(csm => csm.Standardsectionmappingid)
                    .FirstOrDefaultAsync();

                if (standardSectionMappingId == 0)
                {
                    return NotFound("Standard section mapping not found for the given child ID.");
                }


                var branchId = await db.MStandardsectionmappings
                    .Where(ssm => ssm.Id == standardSectionMappingId)
                    .Select(ssm => ssm.Branchid)
                    .FirstOrDefaultAsync();

                if (branchId == 0)
                {
                    return NotFound("Branch not found for the given standard section mapping ID.");
                }


                var schoolId = await db.MBranches
                    .Where(b => b.Id == branchId)
                    .Select(b => b.Schoolid)
                    .FirstOrDefaultAsync();

                if (schoolId == 0)
                {
                    return NotFound("School not found for the given branch ID.");
                }


                var academicYears = await db.MAcademicyeardetails
                    .Where(ay => ay.SchoolId == schoolId)
                    .OrderByDescending(ay => ay.Createddate)
                    .ToListAsync();

                if (!academicYears.Any())
                {
                    return Ok(new { result = "ok", yearList = new List<dataAYear>() });
                }


                var yearData = academicYears.Select(ay => new dataAYear
                {
                    YearId = ay.Id,
                    AcademicYear = ay.YearName,
                    Currentyear = ay.Currentyear
                }).ToList();

                return Ok(new { result = "ok", yearList = yearData });
            }
            catch (Exception ex)
            {

                return BadRequest(new { result = "error", message = ex.Message });
            }
        }



        //[Route("GetStudentSectionDropdown")]
        //[HttpGet]
        //public async Task<IActionResult> GetStudentSectionDropdown(int academicYearId, int childId)
        //{
        //    try
        //    {
        //        var token = ExtractToken();
        //        var appUserId = db.TTokens.Where(x => x.Id.ToString() == token).Select(a => a.Referenceid).FirstOrDefault();
        //        List<StudentSection> studentSections = new List<StudentSection>();

        //        if (academicYearId != 0 && childId != 0)
        //        {
        //            var data = await Task.FromResult((from csm in db.MChildschoolmappings
        //                                              join ssm in db.MStandardsectionmappings on csm.Standardsectionmappingid equals ssm.Id
        //                                              join ssm2 in db.MStandardsectionmappings on ssm.Parentid equals ssm2.Id
        //                                              where csm.AcademicYearId == academicYearId
        //                                                    && csm.Childid == childId
        //                                                    && csm.Statusid == 1
        //                                              select new
        //                                              {
        //                                                  StandardId = ssm2.Id,
        //                                                  StandardName = ssm2.Name,
        //                                                  SectionId = ssm.Id,
        //                                                  SectionName = ssm.Name
        //                                              }).Distinct());

        //            foreach (var item in data)
        //            {
        //                studentSections.Add(new StudentSection
        //                {
        //                    StandardId = item.StandardId,
        //                    StandardName = item.StandardName,
        //                    SectionId = item.SectionId,
        //                    SectionName = item.SectionName
        //                });
        //            }

        //            return Ok(new { result = "ok", studentSectionsList = studentSections });
        //        }

        //        return Ok(new { result = "ok", studentSectionsList = studentSections });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}


        [Route("dataASemesterDropdown")]
        [HttpGet]
        public async Task<IActionResult> dataASemesterDropdown(int childid, int academicYearId)
        {
            try
            {

                var token = ExtractToken();
                var appUserId = await db.TTokens
                                        .Where(x => x.Id.ToString() == token)
                                        .Select(a => a.Referenceid)
                                        .FirstOrDefaultAsync();


                var standardSectionMappingId = await db.MChildschoolmappings
                    .Where(csm => csm.Childid == childid)
                    .Select(csm => csm.Standardsectionmappingid)
                    .FirstOrDefaultAsync();

                if (standardSectionMappingId == 0)
                {
                    return NotFound("Standard section mapping not found for the given child ID.");
                }


                var branchId = await db.MStandardsectionmappings
                    .Where(ssm => ssm.Id == standardSectionMappingId)
                    .Select(ssm => ssm.Branchid)
                    .FirstOrDefaultAsync();

                if (branchId == 0)
                {
                    return NotFound("Branch not found for the given standard section mapping ID.");
                }


                var schoolId = await db.MBranches
                    .Where(b => b.Id == branchId)
                    .Select(b => b.Schoolid)
                    .FirstOrDefaultAsync();

                if (schoolId == 0)
                {
                    return NotFound("School not found for the given branch ID.");
                }


                var semesters = await db.MSemesteryearmappings
                    .Join(db.MSemestertestsmappings,
                          sym => sym.SemesterId,
                          stm => stm.Id,
                          (sym, stm) => new { sym, stm })
                    .Where(x => x.sym.AcademicYearId == academicYearId)
                    .Select(x => new
                    {
                        x.stm.Id,
                        x.stm.Name
                    })
                    .Distinct()
                    .ToListAsync();

                if (semesters == null)
                {
                    return NotFound("Semeseters are not found for the selected year.");
                }
                if (!semesters.Any())
                {
                    return Ok(new { result = "ok", semestersList = new List<dataASemester>() });
                }

                var semesterList = semesters.Select(s => new dataASemester
                {
                    SemesterId = s.Id,
                    SemesterName = s.Name
                }).ToList();

                return Ok(new { result = "ok", semestersList = semesterList });
            }
            catch (Exception ex)
            {

                return BadRequest(new { result = "error", message = ex.Message });
            }
        }


        [Route("dataATestDropdown")]
        [HttpGet]
        public async Task<IActionResult> dataATestDropdown(int semesterid)
        {
            try
            {
                var token = ExtractToken();
                var appUserId = db.TTokens.Where(x => x.Id.ToString() == token).Select(a => a.Referenceid).FirstOrDefault();
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



        [Route("GetStudentsRC")]
        [HttpGet]
        public async Task<IActionResult> GetStudentsRC(int childid, int testid, int academicYearId)
        {
            try
            {
                var token = ExtractToken();
                var appUserId = await db.TTokens
                                        .Where(x => x.Id.ToString() == token)
                                        .Select(a => a.Referenceid)
                                        .FirstOrDefaultAsync();


                var standardSectionMappingId = await db.MChildschoolmappings
                    .Where(csm => csm.Childid == childid)
                    .Select(csm => csm.Standardsectionmappingid)
                    .FirstOrDefaultAsync();

                if (standardSectionMappingId == 0)
                {
                    return NotFound("Standard section mapping not found for the given child ID.");
                }


                var branchId = await db.MStandardsectionmappings
                    .Where(ssm => ssm.Id == standardSectionMappingId)
                    .Select(ssm => ssm.Branchid)
                    .FirstOrDefaultAsync();

                if (branchId == 0)
                {
                    return NotFound("Branch not found for the given standard section mapping ID.");
                }


                var schoolIdNullable = await db.MBranches
                    .Where(b => b.Id == branchId)
                    .Select(b => b.Schoolid)
                    .FirstOrDefaultAsync();

                if (!schoolIdNullable.HasValue)
                {
                    return NotFound("School not found for the given branch ID.");
                }


                var schoolId = schoolIdNullable.Value;


                var result = await dataAnalysisServices.GetStudentsRc(childid, testid, schoolId, academicYearId);

                if (result == null)
                {
                    return NotFound("No data found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { result = "error", message = ex.Message });
            }
        }





        [HttpGet]
        [Route("GetAveragePerformances")]
        public async Task<IActionResult> GetAveragePerformances(int testid, int childid, int academicYearId)
        {
            try
            {
                var token = ExtractToken();
                var appUserId = await db.TTokens
                                        .Where(x => x.Id.ToString() == token)
                                        .Select(a => a.Referenceid)
                                        .FirstOrDefaultAsync();


                var sectionIdNullable = await db.MChildschoolmappings
                    .Where(csm => csm.Childid == childid)
                    .Select(csm => csm.Standardsectionmappingid)
                    .FirstOrDefaultAsync();


                if (!sectionIdNullable.HasValue)
                {
                    return NotFound("Section ID not found for the given child ID.");
                }

                int sectionId = sectionIdNullable.Value;


                var result = await dataAnalysisServices.GetAveragePerformances(sectionId, testid, childid, academicYearId);

                if (result == null)
                {
                    return NotFound("No data found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { result = "error", message = ex.Message });
            }
        }


    }
}
