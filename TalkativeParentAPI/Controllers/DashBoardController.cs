using Microsoft.AspNetCore.Mvc;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        readonly TpContext dbContext = new TpContext();

        private readonly IMSchoolService mSchoolService;

        public DashBoardController(IMSchoolService _mSchoolService)
        {
            mSchoolService = _mSchoolService;

        }


        #region CountDashboardData // Used in Beta
        [Route("CountDashboardData")]
        [HttpGet]
        public async Task<IActionResult> CountDashboardData(int SchoolId)
        {
            //you need to validate user if his belongs to school or not
            //also you have to verify the token as well
            //But in both old & new in reuest only SchoolId is passed
            try
            {
                var count = this.mSchoolService.CountDashboardData(SchoolId);

                var stdC = (from std in dbContext.MStandardsectionmappings
                            join b in dbContext.MBranches
                            on std.Branchid equals b.Id
                            where b.Schoolid == SchoolId && std.Businessunittypeid == 1
                            select new
                            {
                                Name = std.Name
                            }).ToList();

                var secC = (from std in dbContext.MStandardsectionmappings
                            join b in dbContext.MBranches
                            on std.Branchid equals b.Id
                            where b.Schoolid == SchoolId && std.Businessunittypeid == 2
                            select new
                            {
                                Name = std.Name
                            }).ToList();

                return new JsonResult(new
                {
                    TotalStudents = count.Result.ElementAt(0).Studentcount,
                    TotalParents = count.Result.ElementAt(0).Parentcount,
                    TotalClass = stdC.Count,
                    TotalSection = secC.Count
                    //TotalClass = stdC.Count(),
                    //TotalSection = secC.Count()
                });

                //var Data = new
                //{
                //    TotalStudents = count.Result.ElementAt(0).Studentcount,
                //    TotalParents = count.Result.ElementAt(0).Parentcount,
                //    TotalClass = stdC.Count,
                //    TotalSection = secC.Count
                //    //TotalClass = stdC.Count(),
                //    //TotalSection = secC.Count()
                //};

                //return Ok(Data);

                //return Ok(new
                //{
                //    TotalStudents = count.Result.ElementAt(0),
                //    TotalParents = count.Result.ElementAt(1),
                //    TotalClass = stdC.Count(),
                //    TotalSection = secC.Count(),
                //    StatusCode = HttpStatusCode.OK,
                //    Message = "DashBordCount successfully fetched."
                //});


                //    //ChildInfos and Branches NR
                //    var stC = (from ci in dbContext.MChildinfos
                //               join cm in dbContext.MChildschoolmappings
                //               on ci.Id equals cm.Childid
                //               join ssm in dbContext.MStandardsectionmappings
                //               on cm.Standardsectionmappingid equals ssm.Id
                //               join br in dbContext.MBranches
                //               on ssm.Branchid equals br.Id
                //               join sch in dbContext.MSchools
                //               on br.Schoolid equals sch.Id
                //               where sch.Id == SchoolId
                //               select new
                //               {
                //                   Name = ci.Firstname
                //               }).ToList();

                //    var ptC = (from aui in dbContext.MAppuserinfos //MAppuserinfos NR
                //               join pcm in dbContext.MParentchildmappings
                //               on aui.Id equals pcm.Appuserid
                //               join ci in dbContext.MChildinfos
                //               on pcm.Childid equals ci.Id
                //               join csm in dbContext.MChildschoolmappings
                //               on ci.Id equals csm.Childid
                //               join ssm in dbContext.MStandardsectionmappings
                //               on csm.Standardsectionmappingid equals ssm.Id
                //               join br in dbContext.MBranches
                //               on ssm.Branchid equals br.Id
                //               join sch in dbContext.MSchools
                //               on br.Schoolid equals sch.Id
                //               where sch.Id == SchoolId
                //               select new
                //               {
                //                   Name = aui.Firstname
                //               }).ToList();


                //    var stdC = (from std in dbContext.MStandardsectionmappings
                //                join b in dbContext.MBranches
                //                on std.Branchid equals b.Id
                //                where b.Schoolid == SchoolId && std.Businessunittypeid == 1
                //                select new
                //                {
                //                    Name = std.Name
                //                }).ToList();

                //    var secC = (from std in dbContext.MStandardsectionmappings
                //                join b in dbContext.MBranches
                //                on std.Branchid equals b.Id
                //                where b.Schoolid == SchoolId && std.Businessunittypeid == 2
                //                select new
                //                {
                //                    Name = std.Name
                //                }).ToList();

                //var data = new
                //{
                //    TotalStudents = stC.Count(),
                //    TotalParents = ptC.Count(),
                //    TotalClass = stdC.Count(),
                //    TotalSection = secC.Count()
                //};

                //return Ok(data);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        #endregion




    }
}
