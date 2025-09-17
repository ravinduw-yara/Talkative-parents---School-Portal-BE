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
using static CommonUtility.RequestModels.SoundingboardmessagePublicAndPrivate;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SoundingBoardController : ControllerBase
    {

        private readonly ITSoundingboardmessageService tSoundingboardmessageService;
        private readonly INotificationService nNotificationService;
        private readonly TpContext db = new TpContext();

        public SoundingBoardController(
            ITSoundingboardmessageService _tSoundingboardmessageService,
            INotificationService _nNotificationService
            )
        {
            tSoundingboardmessageService = _tSoundingboardmessageService;
            nNotificationService = _nNotificationService;
        }



        #region Api for GetSBPrivateMessagesV2  
        //Used in beta
        [HttpGet]
        [Route("GetSBPrivateMessagesV2/school")]
       
        public IActionResult GetSBPrivateMessagesV2(int schoolUser, int schoolId, int? StandardId = null, int? SectionId = null, DateTime? DateFrom = null, DateTime? DateTo = null, string searchString = "", int PageSize = 10, int pageNumber = 1, int Type = 0)
        {
            try
            {
                var temp = tSoundingboardmessageService.GetSBPrivateMessages(schoolUser, schoolId, StandardId, SectionId, DateFrom, DateTo, searchString, PageSize, pageNumber);
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

        #region Api send notification for SBcomments
        [HttpPost]
        [Route("SendSBMessagePushNotification/school")]
        public async Task<IActionResult> SendSBMessagePushNotification(SendSBMessageNotificationModel sBModel, int messageid, string mtype)
        {
            try
            {
                var temp = await this.tSoundingboardmessageService.SendSBMessagePushNotificationForAPI(sBModel, messageid, mtype);
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



        [Route("UpdateSBMessageDidReadandReplied/school")] 
        [HttpPost]
        public async Task<IActionResult> UpdateSBMessageDidReadandReplied(int messageid, Model mode)
        {
            var data1 = this.db.TSoundingboardmessages.Where(w => w.Id.Equals(messageid)).FirstOrDefault();

            if (data1.Isparentreplied == true)
            {
                data1.Didread = true;
                data1.Isstaffreplied = false;
                data1.Isparentreplied = true;
                db.TSoundingboardmessages.Update(data1);
                await db.SaveChangesAsync();
                return Ok(new { Value = "Updated Successfully" });
            }
            else
            {
                data1.Isstaffreplied = true;
                data1.Isparentreplied = false;
                db.TSoundingboardmessages.Update(data1);
                await db.SaveChangesAsync();
                return Ok(new { Value = "No Need To Update" });
            }
        }

        [Route("GetSBMessagesByIdV2/school")]
        [HttpGet]
        public async Task<IActionResult> GetSBMessagesByIdV2(int messageid, int userid)
        {
            try
            {
                var res = await this.tSoundingboardmessageService.GetSBMessagesByIdV2ForAPI(messageid, userid);
                if (res == null)
                {
                    return NotFound(new
                    {
                        Data = "Soundingboard Message doesn't exists with this ID.",
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

    }
}


