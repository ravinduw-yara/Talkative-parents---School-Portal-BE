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
using static CommonUtility.RequestModels.SoundingboardmessagePublicAndPrivate;

namespace TalkativeParentAPI_APP.Controllers
{
    [CustomAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageSBController : ControllerBase
    {
        private static TpContext db = new TpContext();
        private readonly ITSoundingboardmessageService tSoundingboardmessageService;
        private readonly ITNoticeboardmessageService tNoticeboardmessageService;
        private readonly INotificationService notificationService;

        public MessageSBController(ITSoundingboardmessageService tSoundingboardmessageService, INotificationService notificationService, ITNoticeboardmessageService tNoticeboardmessageService)
        {
            this.tSoundingboardmessageService = tSoundingboardmessageService;
            this.notificationService = notificationService;
            this.tNoticeboardmessageService = tNoticeboardmessageService;
        }

        [HttpGet]
        [Route("SBUnreadMessageCount")]
        public IActionResult SBUnreadMessageCount(int ChildId)
        {
            var count = db.TSoundingboardmessages.Where(w => w.Childinfoid == ChildId && w.Didread == false && w.Isparentreplied == false && w.Isstaffreplied == true).Count();

            SBMessageStatus val = new SBMessageStatus();
            val.Status = 1;
            val.SbCount = count;
            return Ok(val);

        }

        [HttpGet]
        [Route("GetCategoryListV2/App")]
        public IActionResult GetCategory(int schoolid)
        {
            List<GetCategory_APP> category = new List<GetCategory_APP>();
            try
            {
                var objresult = db.MCategories.Where(x => x.Role.Schoolid.Equals(schoolid));
                if (objresult != null)
                {
                    foreach (var item in objresult)
                    {
                        category.Add(new GetCategory_APP
                        {
                            Id = item.Id,
                            Name = item.Name,
                        });
                    }
                    return Ok(category);
                }
                return BadRequest("Categories not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/PostSBPrivateAndPublicMessageByMessageIdV2")]
        [HttpPost]
        public async Task<IActionResult> PostSBPrivateAndPublicMessageByMessageIdV2(PostCommentModel pModel) 
        {              
            try
            {
                int appUserId = AuthExtensions.GetAppUserId();
                //int appUserId = 1; //only for testing
                var temp = await this.tSoundingboardmessageService.PostSBPrivateAndPublicMessageByMessageIdV2ForAPI(pModel, appUserId);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "Soundingboard Messages not found.",
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
                });
            }

        }

        [Route("api/GetSBPrivateAndPublicMessageV2")]
        [HttpGet]
        public async Task<IActionResult> GetSBPrivateAndPublicMessageV2(string Type, int schoolid, int parentid, int Childid, int PageSize = 20, int pageNumber = 1)
        {
            try
            {
                int appUserId = AuthExtensions.GetAppUserId();
                //int appUserId = 1; //only for testing
                var temp = await this.tSoundingboardmessageService.GetSBPrivateAndPublicMessageV2ForAPI(Type, schoolid, appUserId, parentid, Childid, PageSize, pageNumber);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "Soundingboard Messagesnot found.",
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
                });
            }

        }

        [HttpPost]
        [Route("api/PostSBMessageV2/App")]
        public async Task<IActionResult> PostSBMessageV2(SoundingboardmessagePublicAndPrivate message)
        {
            try
            {
                int appUserId = AuthExtensions.GetAppUserId();
                //int appUserId = 4452; //Testing for Tiasha
                //int appUserId = 1; //only for testing
                var temp = await this.tSoundingboardmessageService.PostSBMessageV2ForAPI(message, appUserId);
               if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "Could not post Soundingboard Messages",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                if (temp == "Success")
                {
                    SBMessageStatus val = new SBMessageStatus();
                    val.Status = 1;
                    return Ok(val);
                }
                else
                {
                    return BadRequest(temp);
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
        #region Api send notification for SBcomments Teacher App to Parents
        // Api send notification for SBcomments Teacher App to Parents
        [HttpPost]
        [Route("SendTeacherAppSBMessagePushNotification/school")]
        public async Task<IActionResult> SendTeacherAppSBMessagePushNotification(SendSBMessageNotificationModel sBModel, int messageid, string mtype)
        {
            try
            {
                var temp = await this.tSoundingboardmessageService.SendTeacherAppSBMessagePushNotification(sBModel, messageid, mtype);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "Soundingboard Messagesnot found.",
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
                });
            }
        }
        #endregion
        [HttpGet]
        [Route("api/GetSBPrivateAndPublicMessageByMessageIdV2")]
        public async Task<IActionResult> GetSBPrivateAndPublicMessageByMessageIdV2(int messageid)
        {
            try
            {
                var res = await db.TSoundingboardmessages.Where(x => x.Id == messageid).Include(x => x.Standardsectionmapping.Branch).FirstOrDefaultAsync();

                if (res != null)
                {
                    if (res.Isparentreplied == true || res.Isparentreplied == false)
                    {
                        res.Didread = true;
                        db.TSoundingboardmessages.Update(res);
                        await db.SaveChangesAsync();
                    }
                    //if (res.Isparentreplied == false || res.Isparentreplied == true)
                    //{
                       //res.Didread = true;
                       // await db.SaveChangesAsync();
                   // }

                    var count = db.TSoundingboardmessages.Where(w => w.Appuserinfoid == res.Appuserinfoid && w.Childinfoid == res.Childinfoid && w.Didread == false && w.Isstaffreplied == true && w.Isparentreplied == false).Count();

                    GetSBMessageByIDModel sbm = new GetSBMessageByIDModel();
                    sbm.Id = res.Id;
                    sbm.ParentId = res.Appuserinfoid;
                    sbm.ChildId = res.Childinfoid;
                    sbm.CategoryKey = (int)res.Categoryid;
                    sbm.CategoryName = await db.MCategories.Where(w => w.Id == res.Categoryid).Select(x => x.Name).FirstOrDefaultAsync();
                    sbm.ParentName = await db.MAppuserinfos.Where(w => w.Id == res.Appuserinfoid).Select(x => x.Firstname).FirstOrDefaultAsync();
                    sbm.ChildName = await db.MChildinfos.Where(w => w.Id == res.Childinfoid).Select(x => x.Firstname).FirstOrDefaultAsync();
                    sbm.Relation = db.MParentchildmappings.Where(x => x.Appuserid == res.Appuserinfoid && x.Childid == res.Childinfoid).Include(w => w.Relationtype).Select(w => w.Relationtype.Type).FirstOrDefault();
                    sbm.StandardId = res.Standardsectionmapping.Parentid;
                    sbm.SectionId = res.Standardsectionmappingid;
                    sbm.Subject = res.Subject;
                    sbm.Description = res.Description;
                    sbm.CommentsCount = res.Commentscount;
                    sbm.Attachment = res.Attachments;
                    sbm.IsStaffReplied = res.Isstaffreplied;
                    sbm.IsParentReplied = res.Isparentreplied;
                    sbm.Schoolid = res.Standardsectionmapping.Branch.Schoolid;

                    DateTime currentTime = (DateTime)res.Createddate;
                    DateTime x30MinsLater = currentTime.AddHours(5);
                    x30MinsLater = x30MinsLater.AddMinutes(30);
                    sbm.CreatedDate = x30MinsLater;

                    DateTime currentTime2 = (DateTime)res.Modifieddate;
                    DateTime x30MinsLater2 = currentTime2.AddHours(5);
                    x30MinsLater2 = x30MinsLater2.AddMinutes(30);
                    sbm.CreatedDate = x30MinsLater2;

                    sbm.DidRead = true;
                    sbm.IsActive = res.Statusid;
                    sbm.SBcount = count;

                    ////Not Present in DB
                    //sbm.LikesCount = res.LikesCount;
                    //sbm.Type = data.Type.ToString();
                    //sbm.AttachmentCount = res.AttachmentCount;



                    return Ok(sbm);
                }
                return BadRequest("SB message not found");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }

    public class GetSBMessageByIDModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int? ChildId { get; set; }
        public int CategoryKey { get; set; }
        public string CategoryName { get; set; }
        public string ParentName { get; set; }
        public string ChildName { get; set; }
        public string Relation { get; set; }
        public int? StandardId { get; set; }
        public int? SectionId { get; set; }
        public string StandardName { get; set; }
        public string SectionName { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int? CommentsCount { get; set; }
        public int? LikesCount { get; set; }
        public string Attachment { get; set; }
        public int? AttachmentCount { get; set; }
        public bool? IsStaffReplied { get; set; }
        public bool? IsParentReplied { get; set; }
        public int? Schoolid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? DidRead { get; set; }
        public int? IsActive { get; set; }
        public string Type { get; set; }
        public int? SBcount { get; set; }
        public Guid? schooluserid { get; set; }


        // public string CreateDatelong { get; set; }
        // public string UpdateDatelong { get; set; }
        //public Guid CreatedById { get; set; }
        //public string CreatedByName { get; set; }

    }
}
