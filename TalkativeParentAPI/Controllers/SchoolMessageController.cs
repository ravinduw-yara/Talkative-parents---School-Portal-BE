using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static Services.TNoticeboardmappingService;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SchoolMessageController : ControllerBase
    {
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext db3 = new TpContext();
        private readonly TpContext tpContext;

        private readonly INotificationService notificationService;
        private readonly IAppuserdeviceService appUserDeviceService;
        private readonly IEmailService emailService;
        private readonly ITNoticeboardmessageService tNoticeboardmessageService;
        private readonly ITNoticeboardmappingService tNoticeboardmappingService;
        private readonly IMSchooluserinfoService schooluserinfoService;

        public SchoolMessageController(

            INotificationService _notificationService,
            IAppuserdeviceService _appUserDeviceService,
            IEmailService _emailService,
            ITNoticeboardmessageService _tNoticeboardmessageService,
            ITNoticeboardmappingService _tNoticeboardmappingService,
            TpContext _tpContext,
            IMSchooluserinfoService _schooluserinfoService

            )
        {

            notificationService = _notificationService;
            appUserDeviceService = _appUserDeviceService;
            emailService = _emailService;
            tNoticeboardmessageService = _tNoticeboardmessageService;
            tNoticeboardmappingService = _tNoticeboardmappingService;
            tpContext = _tpContext;
            schooluserinfoService = _schooluserinfoService;
        }


        [HttpGet]
        [Route("GetParents")]
        public async Task<IActionResult> GetParents(Guid token, string standard = "", string section = "")
        {
            try
            {
                var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(a => a.Referenceid).FirstOrDefaultAsync();
                if (userid != null)
                {
                    int? schoolid = await this.db2.MSchooluserinfos.Where(x => x.Id.Equals(userid)).Select(a => a.Branch.Schoolid).FirstOrDefaultAsync();

                    List<GetParents> parents = new List<GetParents>();
                    IQueryable<GetParentsSP> objresult = await this.tNoticeboardmappingService.GetParentsNB((int)schoolid, standard, section);
                    {
                        if (objresult != null)
                        {
                            foreach (var item in objresult.ToList())
                            {
                                GetParents gp = new GetParents();
                                gp.Id = (int)item.Parentid;
                                gp.Parent = item.ParentName;

                                if (item.RelationId == 1)
                                {
                                    gp.ChildRelation = "Father of " + item.ChildName;
                                }
                                else if (item.RelationId == 2)
                                {
                                    gp.ChildRelation = "Mother of " + item.ChildName;
                                }
                                else
                                {
                                    gp.ChildRelation = "Guardian of " + item.ChildName;
                                }

                                gp.ChildSchoolMappingId = (int)item.ChildSchoolMappingId;
                                gp.ChildId = (int)item.ChildId;
                                gp.firstName = item.ChildName;
                                gp.StandardId = (int)item.StandardId;
                                gp.Standard = item.StandardName;
                                gp.Section = item.SectionName;
                                gp.SectionId = (int)item.SectionId;
                                gp.ChildEmail = item.ChildEmail;
                                gp.RegistrationNumber = item.RegistrationNumber.TrimEnd();
                                gp.StatusId = item.StatusId; // 27/2/2024 Sanduni

                                var p2 = await this.db3.MParentchildmappings.Where(x => x.Childid.Equals(item.ChildId) && x.Relationtypeid != item.RelationId).Include(a => a.Appuser).FirstOrDefaultAsync();

                                if(p2 != null)
                                {
                                    gp.SecondParent = (int)p2.Appuserid;
                                    gp.ParentName2 = p2.Appuser.Firstname + " " + p2.Appuser.Lastname;
                                }
                                else
                                {
                                    gp.SecondParent = 0;
                                    gp.ParentName2 = null;
                                }

                                parents.Add(gp);

                            }

                            return Ok(parents);
                        }
                        else
                        {
                            return BadRequest("No Data are found");
                        }
                    }
                }
                return BadRequest("Access not granted");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }




        #region Api for GetSchoolMessagesBySchoolIdV2
        [HttpGet]
        [Route("GetSchoolMessagesBySchoolIdV2")]
        public async Task<IActionResult> GetSchoolMessagesBySchoolIdV2(int userId, int schoolId, int? academicyearid, int? standardId = null, int? sectionId = null, int? childId = null, string searchString = "", int PageSize = 10, int pageNumber = 1,int isSelectAll = 0)
        {
            try
            {
                var res = await tNoticeboardmappingService.GetSchoolMessagesBySchoolIdForAPI(userId, schoolId, standardId, sectionId, childId, searchString, PageSize, pageNumber, isSelectAll);
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
        #endregion

        #region Api for DeleteNBMessages

        [HttpDelete]
        [Route("DeleteNBMessagesV2/school")]
        public async Task<IActionResult> DeleteNBMessagesV2(int messageId)
        {
            try
            {
                var nbm = db.TNoticeboardmappings.Where(x => x.Noticeboardmsgid.Equals(messageId));
                if (nbm.Count() > 0)
                {
                    db2.TNoticeboardmappings.RemoveRange(nbm);
                    await this.db2.SaveChangesAsync();
                }

                var res = await this.tNoticeboardmessageService.GetEntityIDForUpdate(messageId);
                if(res != null)
                {
                    await this.tNoticeboardmessageService.DeleteEntity(res);
                    return Ok(new { Value = "NoticeBoard message Deleted Successfully" });
                }
                return BadRequest(new { Value = "NoticeBoard message does not exist" });
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return BadRequest(a);
            }

        }

        #endregion
        //July 2 2025 Jaliya
        [HttpGet]
        [Route("GetBulkNBMessages")]
        public async Task<IActionResult> GetBulkNBMessages(
    int schoolUserId,
    DateTime startNBDate,
    DateTime endNBDate)
        {
            try
            {
                var res = await tNoticeboardmappingService.GetBulkNBMessages(schoolUserId, startNBDate, endNBDate);

                if (res == null || !res.Any())
                    return BadRequest("Records not found");

                return Ok(res);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Data = ex.Message });
            }
        }

        [HttpDelete("DeleteBulkNBMessages")]
        public async Task<IActionResult> DeleteBulkNBMessages(
    [FromQuery] int schoolUserId,
    [FromQuery] DateTime startNBDate,
    [FromQuery] DateTime endNBDate)
        {
            try
            {
                await tNoticeboardmappingService.DeleteBulkNBMessages(schoolUserId, startNBDate, endNBDate);
                return Ok(new { Message = "Messages deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region PostNoticeBoardMsg
        [HttpPost]
        [Route("PostNoticeBoardMsg")]  //beta
        public async Task<IActionResult> PostNoticeBoardMsg(NBMModel nbmModel)
        {
            int NoticeBoardID;
            CommonUtility.Common.EnableAll();
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return BadRequest(this.ModelState);
                }
                else
                {

                    NoticeBoardID = await this.tNoticeboardmessageService.NBConditionCheck(nbmModel);
                   
                   
                    if (NoticeBoardID > 0)
                    {
                        return Ok(new { msg = "Notice Board Message sent Successfully", NoticeBoardID });
                    }
                    else
                    {
                        //throw new ApplicationException("Exception Occured");
                        return BadRequest("The School does not exists");
                    }
                } 
            }
            catch (Exception ex)
            {
                CommonUtility.WriteLogFile.LogErrorMessage(ex);
                return BadRequest(ex.Message);
            }

            
        }
        
        [Route("GetSchoolUserDetails")]
        [HttpGet]
        public async Task<IActionResult> GetSchoolUserDetails(int schoolId, int userId)
        {
            try
            {
                var res = await this.schooluserinfoService.GetSchoolUserDetails(schoolId, userId);

                if (res == null || res.Count == 0)
                {
                    return NotFound(new { Message = "No user details found for the specified school." });
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("GetSchoolMessageById")]
        public async Task<IActionResult> GetSchoolMessageById(int messageId)
        {
            try
            {
                var res = await tNoticeboardmappingService.GetSchoolMessageById(messageId);
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
        #endregion


    }
}
