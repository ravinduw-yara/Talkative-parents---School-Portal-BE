using CommonUtility.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SchoolsController : ControllerBase
    {
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext db3 = new TpContext();
        private readonly TpContext db4 = new TpContext();
        private readonly IMBranchService mBranchService;


        public SchoolsController(
             IMBranchService _mBranchService
            )
        {
           
           mBranchService = _mBranchService;
        }

        [Route("GetSchool")]
        [HttpGet]
        public async Task<IActionResult> GetSchool()//GetSchool
        {
            

            var school = db.MSchools.Where(w => w.Statusid == 1);

            if (school == null)
            {
                return BadRequest("School not found");
            }
            else
            {
                return new JsonResult(new
                {
                    Value = school,
                    StatusCode = HttpStatusCode.OK
                });
            }

        }
        [Route("getMSchoolByID")]
        [HttpGet]

        public async Task<IActionResult> getMSchoolByID(int MSchoolID)
        {
            try
            {
                var school = db.MSchools.Where(w => w.Statusid == 1 &&  w.Id == MSchoolID).FirstOrDefault();
                if (school == null)
                {
                    return NotFound(new
                    {
                        Data = "School doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new
                {
                    Data = school,
                    StatusCode = HttpStatusCode.OK,
                    Message = "School successfully fetched."
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

