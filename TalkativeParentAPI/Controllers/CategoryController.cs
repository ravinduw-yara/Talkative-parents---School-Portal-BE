using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    public class CategoryController : ControllerBase
    {
        private readonly IMCategoryService mCategoryService;
        private readonly TpContext db = new TpContext();
        

        public CategoryController(IMCategoryService _mCategoryService)
        {
            mCategoryService = _mCategoryService;
        }

        [Route("UserSpecializationCategory")]
        [HttpPost]
        public async Task<IActionResult> PostUserSpecializationCategory([FromBody] MCategoryModel model) //Post category
        {
            try
            {
                var temp = await this.mCategoryService.GetEntityIDForUpdate(model.Id);
                if (temp != null)
                {
                    if (!string.IsNullOrEmpty(model.Name))
                        temp.Name = model.Name;
                    if (!string.IsNullOrEmpty(model.Description))
                        temp.Description = model.Description;
                    //if (model.Statusid.HasValue)
                    //    temp.Statusid = model.Statusid;
                    temp.Statusid = 1;
                    if (model.Modifiedby.HasValue)
                        temp.Modifiedby = model.Modifiedby;
                    if (model.Createdby.HasValue)
                        temp.Createdby = model.Createdby;
                    if (model.SBAccessRankId.HasValue)
                        temp.Roleid = model.SBAccessRankId;
                    temp.Modifieddate = DateTime.UtcNow;

                    await this.mCategoryService.UpdateEntity(temp);
                    return Ok(new { Value = "Category Updated!" });
                }
                await this.mCategoryService.AddEntity(new MCategory
                {
                    Name = model.Name,
                    Description = model.Description,
                    Roleid = model.SBAccessRankId,
                    Createdby = model.Createdby,
                    Statusid = model.Statusid,
                    Createddate = DateTime.UtcNow,
                    Modifiedby = model.Createdby,
                    Modifieddate = DateTime.UtcNow,
                });
                return Ok(new
                {
                    Value = "Category Inserted!"
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


        [Route("sbaccessroles")]
        [HttpGet]

        public IActionResult GetSbAccessRoles(int schoolId) //getMCategoryBySchoolID(int SchoolID)
        {
            try
            {
                //var temp = await this.mCategoryService.GetEntityBySchoolID(schoolId);
                var temp = db.MRoles.Where(x => x.Schoolid == schoolId).Select(w => new { id = w.Id, name = w.Name, remarks = w.Description, rank = w.Rank, selectiontype = w.Rank });
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "No roles exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(temp);
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

        [HttpGet]
        [Route("UserSpecializationCategory")]
        public IActionResult GetUserSpecializationCategory(int schoolId, bool isList, int PageSize = 10, int pageNumber = 1, string searchString = "")
        {
            try
            {
                var res = mCategoryService.GetUserSpecializationCategoryForAPI(schoolId, isList, PageSize, pageNumber, searchString);
                if (res == null)
                {
                    return NotFound(new
                    {
                        Data = "UserSpecialization Categories not found.",
                        StatusCode = HttpStatusCode.NotFound
                    });
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


        [HttpDelete]
        [Route("sbaccessroles")]
        public async Task<IActionResult> DeleteSbAccessRoles(int sbAccessRoleId) //Deletion
        {
            try
            {
                var temp = await this.mCategoryService.GetEntityIDForUpdate(sbAccessRoleId);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MCategory doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                await this.mCategoryService.DeleteEntity(temp);
                return Ok(new { Value = "Deleted Successfully!" });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }


        #region delete later if not used
        //[HttpGet]
        //[Route("GetRoles")]
        //public IActionResult GetRoles(int schoolid)
        //{
        //    var res = db.MRoles.Where(x => x.Schoolid == schoolid).Select(w => new { Id = w.Id, RoleName = w.Name });
        //    if (res != null)
        //    {
        //        return Ok(res);
        //    }
        //    return BadRequest("Categories not found");
        //}
        #endregion

    }
}
