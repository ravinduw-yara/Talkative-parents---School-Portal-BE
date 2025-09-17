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
    public class SchoolUsersController : ControllerBase
    {
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext db3 = new TpContext();
        private readonly TpContext db4 = new TpContext();
        private readonly IMGroupService mGroupService;
        private readonly IMStandardgroupmappingService mStandardgroupmapping;
        private readonly IMUsermodulemappingService mUsermodulemappingService;
        private readonly IMSchooluserinfoService mSchooluserinfoService;
        private readonly IMSchooluserroleService mSchooluserroleService;
        private readonly IMModuleService mModuleService;
        private readonly IMUsermodulemappingService mUsermodulemapping;


        public SchoolUsersController(
            IMGroupService _mGroupService, 
            IMStandardgroupmappingService _mStandardgroupmapping, 
            IMUsermodulemappingService _mUsermodulemappingService,
            IMSchooluserinfoService _mSchooluserinfoService,
            IMSchooluserroleService _mSchooluserroleService,
            IMModuleService _mModuleService,
            IMUsermodulemappingService _mUsermodulemapping
            )
        {
            mGroupService = _mGroupService;
            mStandardgroupmapping = _mStandardgroupmapping;
            mUsermodulemappingService = _mUsermodulemappingService;
            mSchooluserinfoService = _mSchooluserinfoService;
            mSchooluserroleService = _mSchooluserroleService;
            mModuleService = _mModuleService;
            mUsermodulemapping = _mUsermodulemapping;
        }

        [Route("GetGCLStatus")]
        [HttpGet]
        public IActionResult GetGCLStatus(int schoolid)//DA Mapping
        {
            if (schoolid == 0 || schoolid.Equals(null))
            {
                return BadRequest("Schoolid is requried");
            }

            var school = db.MSchools.Where(w => w.Id == schoolid).FirstOrDefault();
            //   var flag = db.MFeatures.Where(x => x.Schoolid == schoolid).FirstOrDefault();
            var flag = db.MSchoolDamappings.Where(x => x.SchoolId == schoolid).FirstOrDefault();

            if (school == null)
            {
                return BadRequest("School not found");
            }
            else if (flag.Daflag == true)
            {
                return Ok(new { Value = "Enabled" });
            }
            else
            {
                return Ok(new { Value = "Disabled" });
            }
        }


        #region Group
        [Route("sectionalgrade")]
        [HttpPost]
        public async Task<IActionResult> PostSectionalGrade([FromBody] MGroupModel model) // Post/Update Group, update
        {
            try
            {
                var res = await this.mGroupService.PostSectionalGradeForAPI(model);
                if (res == null)
                {
                    return BadRequest("Records not found");
                }
                //return Ok(res);
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


        [Route("sectionalgrade/{schoolid}")]
        [HttpGet]
        public IActionResult GetSectionalGrade(int schoolid, int PageSize = 10, int pageNumber = 1, string searchString = "")
        {
            try
            {
                var res = mGroupService.GetSectionalGradeForAPI(schoolid, PageSize, pageNumber, searchString);
                if (res == null)
                {
                    return BadRequest("Records not found");
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



        [Route("sectionalgrade/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteSectionalGrade(int id) //Deletion
        {
            try
            {
                //var temp = await this.mStandardgroupmapping.GetEntityIDForDelete(id);
                var temp = db.MStandardgroupmappings.Where(x => x.Groupid.Equals(id));
                if (temp.Count().Equals(0))
                {
                    return NotFound(new
                    {
                        Data = "MStandardgroupmapping doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                //await this.mStandardgroupmapping.DeleteEntity(temp);

                db.MStandardgroupmappings.RemoveRange(temp);
                await this.db.SaveChangesAsync();

                var temp2 = await this.mGroupService.GetEntityIDForUpdate(id);
                if (temp2 == null)
                {
                    return NotFound(new
                    {
                        Data = "MGroup doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                await this.mGroupService.DeleteEntity(temp2);
                //return Ok("Deleted succesfully");
                return new JsonResult(new
                {
                    Value = "Deleted Successfully",
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



        #region Users
        [Route("SchoolUsers")]
        [HttpPost]
        public async Task<IActionResult> PostSchoolUser([FromBody]SchoolUserModel model)
        {
            try
            {
                var res = await this.mSchooluserinfoService.PostSchoolUserForAPI(model);
                if (res == null)
                {
                    return null;
                }
                return Ok(new { id = res });
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
        [Route("SchoolUsers")]
        public async Task<IActionResult> GetSchoolUserOld(Guid token, string searchString = "", int PageSize = 10, int pageNumber = 1)
        {
            try
            {
                var res = await this.mSchooluserinfoService.GetSchoolUserOldWithSP(token, searchString, PageSize, pageNumber);
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


        [Route("GetSchoolUserByid")]
        [HttpGet]
        public async Task<IActionResult> GetSchoolUserByid(int id, int schoolid)
        {
            try
            {
                var res = await this.mSchooluserinfoService.GetSchoolUserByidForAPI(id, schoolid);
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

        [Route("DeleteSchoolUser")]
        [HttpPost]
        public async Task<IActionResult> DeleteSchoolUser(int Id, Guid TokenId)
        {
            try
            {

                var schooluserNB = await this.db.TNoticeboardmessages.Where(x => x.Schooluserid.Equals(Id)).FirstOrDefaultAsync();
                if (schooluserNB == null)
                {
                Task<bool> result = mSchooluserinfoService.DeleteSchoolUserAsync(Id);
                var tok = await this.db.TTokens.Where(x => x.Id.Equals(TokenId) && x.Statusid == 1).FirstOrDefaultAsync();
               
                
                    if (tok != null)
                    {
                        var check = await this.db2.MSchooluserroles.Where(a => a.Schooluserid.Equals(Id)).Select(b => b.Categoryid).FirstOrDefaultAsync();
                        if (check == null)
                        {
                            /// var temp = await this.mSchooluserinfoService.GetEntityIDForDelete(Id);
                            //if (temp != null && temp.Statusid == 1)
                            //{
                            //    temp.Statusid = 2;
                            //    temp.Enddate = DateTime.Now;
                            //    temp.Modifieddate = DateTime.Now;
                            //    await this.mSchooluserinfoService.UpdateEntity(temp);

                            //    var temp2 = db3.MUsermodulemappings.Where(x => x.Schooluserid.Equals(Id));
                            //    foreach(var item in temp2)
                            //    {
                            //        item.Statusid = 2;
                            //        db4.MUsermodulemappings.Update(item);
                            //        await db4.SaveChangesAsync();
                            //    }

                            return Ok(new { Message = "User Deleted Successfully" });
                        }
                        else
                        {
                            return BadRequest(new { Message = "User is not Deleted Successfully" });
                        }
                    }
                }
                
                    return BadRequest(new { Message = "User is already mapped to a category or have Notice Board Messages" });
               // }
               // return BadRequest(new { Message = "User is either deactivated or not found" });
            }
            catch (Exception ex)
            {

                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }

        [Route("GetMenuPermissions")]
        [HttpGet]
        public async Task<IActionResult> GetMenuPermissions(Guid Token)
        {
            var validate = await this.db.TTokens.Where(x => x.Id.Equals(Token)).FirstOrDefaultAsync();
            if (validate != null)
            {
                var res = (from a in db.MModules
                           where a.Parentid == null
                           select new
                           {
                               Id = a.Id,
                               Name = a.Name
                           }).ToList();
                return Ok(res);
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }

        #endregion

    }
}
