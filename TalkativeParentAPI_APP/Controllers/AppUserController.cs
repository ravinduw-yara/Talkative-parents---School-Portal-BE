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
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc.Filters;


namespace TalkativeParentAPI_APP.Controllers
{
    [CustomAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private static TpContext db = new TpContext();
        private static HttpActionContext act = new HttpActionContext();
        private static IAppuserdeviceService appuserdeviceService;
        private IEmailService emailService;
        private static IMSchoolService mSchoolService;
        private readonly ITNoticeboardmessageService tNoticeboardmessageService;

        public AppUserController(IAppuserdeviceService _appuserdeviceService, IMSchoolService _mSchoolService, ITNoticeboardmessageService _tNoticeboardmessageService, IEmailService _emailService)
        {
            appuserdeviceService = _appuserdeviceService;
            mSchoolService = _mSchoolService;
            emailService = _emailService;
            tNoticeboardmessageService = _tNoticeboardmessageService;
        }
        [HttpGet]
        public string ExtractToken()
        {
            const string HeaderKeyName = "authToken";
            Request.Headers.TryGetValue(HeaderKeyName, out Microsoft.Extensions.Primitives.StringValues headerValue);
            return (headerValue);
        }
        [Route("GetSchoolIpgUrlDetails")]
        [HttpGet]
        public async Task<IActionResult> GetSchoolIpgUrlDetails(int schoolId)
        {
            try
            {
                //var appUserId = AuthExtensions.GetAppUserId();
                var appUserId = 1;
                //var token = ExtractToken(); //2/8/2024
                //var appUserId = db.TTokens.Where(x => x.Id.ToString() == token).Select(a => a.Referenceid).FirstOrDefault();
                var appUser = db.MAppuserinfos.Where(w => w.Id == appUserId).FirstOrDefault();
                if (null != appUser)
                {
                    var temp = await mSchoolService.GetSchoolIpgurlById(schoolId);
               
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MSchool doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                    return Ok(temp);
                }
                else
                {
                    return NotFound(new
                    {
                        Data = "App User doesn't exists with this Token.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                

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

        [Route("GetEduloanStatus")]
        [HttpGet]
        public async Task<IActionResult> GetEduloanStatus(int schoolId)
        {
            try
            {
                //var appUserId = AuthExtensions.GetAppUserId();
                //var appUserId = 1;
                //var token = ExtractToken(); //2/8/2024
                //var appUserId = db.TTokens.Where(x => x.Id.ToString() == token).Select(a => a.Referenceid).FirstOrDefault();
                var school = db.MSchools.Where(w => w.Id == schoolId && w.eduloanstatus == 1).FirstOrDefault();
                if (null != school)
                {
                   if (school == null)
                    {
                        return NotFound(new
                        {
                            Data = "MSchool doesn't exists with this ID.",
                            StatusCode = HttpStatusCode.NotFound
                        });
                    }
                    return Ok(school.eduloanstatus);
                }
                else
                {
                    return NotFound(new
                    {
                        Data = "App User doesn't exists with this Token.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }


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

        [Route("SendFeedbackV2")]
        [HttpPost]
        public async Task<IActionResult> SendFeedbackV2(FeedbackModel feedbackModel)
        {
            try
            {
                // int appUserId = 4872; //
                // AuthorizationFilterContext filterContext = null;
                //IMembershipService tempmember = new TalkativeApiMembershipProvider();
                //CustomAuthorization tempauth = new CustomAuthorization();
                // tempauth.OnAuthorization();
                //if (filterContext == null)
                //{
                //    Microsoft.Extensions.Primitives.StringValues authTokens;
                //    filterContext.HttpContext.Request.Headers.TryGetValue("authToken", out authTokens);
                //    var token = authTokens.FirstOrDefault();
                //    appUserId = tempmember.GetAppUserId(token);
                //}
                var token = ExtractToken(); //23/7/2024
                var appUserId = db.TTokens.Where(x => x.Id.ToString() == token).Select(a => a.Referenceid).FirstOrDefault();
                //int appUserId = AuthExtensions.GetAppUserId();
                //int appUserId = act.Request.GetAppUserId();
                var appUser = await db.MAppuserinfos.Where(w => w.Id == appUserId).FirstOrDefaultAsync();

                List<string> schoolNames = new List<string>();
                var schoolname = await Task.FromResult((from s in db.MSchools
                                                       join b in db.MBranches on s.Id equals b.Schoolid
                                                       join ssm in db.MStandardsectionmappings on b.Id equals ssm.Branchid
                                                       join csm in db.MChildschoolmappings on ssm.Id equals csm.Standardsectionmappingid
                                                       join pcm in db.MParentchildmappings on csm.Childid equals pcm.Childid
                                                       join a in db.MAppuserinfos on pcm.Appuserid equals a.Id
                                                       where a.Id == appUserId
                                                       select s.Name).Distinct().ToList());

                foreach(var item in schoolname)
                {
                    schoolNames.Add(item);
                }

                string schoolresult = string.Join(", ", schoolNames);

                if (null != appUser)
                {
                    string content = "The user " + appUser.Firstname + " " + appUser.Lastname + " of " + schoolresult + " with phone number " + appUser.Phonenumber + " and email " + appUser.Emailid + " has sent the following feedback:</p><blockquote><br>" +
                        feedbackModel.Feedback + "<br></blockquote>";// <p> Ref: " + appUser.Id + "</p>";

                    //bool responce = emailService.FeedbackSendgridEmail("User Feedback", content, appUser.Emailid);
                    // EmailService.SendMailToTakativeSupport(appUser.Emailid, "User Feedback", content);
                    EmailService.SendMailToTakativeSupportAsync(appUser.Emailid, "User Feedback", content);
                    //AppStatus val = new AppStatus();
                    // val.Status = 1;
                    return Ok(1);
                   // return Ok(1);
                }
                else
                {
                    return BadRequest("User with that token does not exist");
                }
            }
            catch (Exception ex)
            {
                Shared.telemetryClient.TrackException(ex);
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("SetOfferOptIn")]
        public IActionResult SetOffersOptIn(bool isYes)
        {
            try
            {
               // new fix sanduni 13 / 9 / 2024
                using (var _db = new TpContext())
                { 
                    var token = ExtractToken(); //23/7/2024
                var appUserId = _db.TTokens.Where(x => x.Id.ToString() == token).Select(a => a.Referenceid).FirstOrDefault();
                    //var appUserId = AuthExtensions.GetAppUserId();
                    _db.MAppuserinfos.Where(w => w.Id == appUserId).FirstOrDefault().Isofferoptedin = isYes;
                    _db.SaveChanges();
                return Ok();
            }
            }
            catch (Exception ex) //GetFullDetail
            {
                Shared.telemetryClient.TrackException(ex);
                return BadRequest(ex.ToString());
            }
        }

        

        [Route("GetProfile")]
        [HttpGet]
        public IActionResult GetProfile()
        {
            try
            {
                var token = ExtractToken();
                // new fix sanduni 13 / 9 / 2024
                using (var _db = new TpContext())
                {

                    var appUserId = _db.TTokens.Where(x => x.Id.ToString() == token).Select(a => a.Referenceid).FirstOrDefault();

                    //  var appUserId = AuthExtensions.GetAppUserId(); /1/6/2024 sanduni
                    //var appUserId = 1;
                    var appUser = _db.MAppuserinfos.Where(w => w.Id == appUserId).FirstOrDefault();
                    if (null != appUser)
                    {
                        var user = _db.MAppuserinfos.Where(w => w.Id == appUser.Id)
                            .Select(t => new
                            {
                                Id = t.Id,
                                FirstName = t.Firstname,
                                MiddleName = t.Middlename,
                                LastName = t.Lastname,
                                Gender = t.Gender.Type,
                                EmailAddress = t.Emailid,
                                //t.LastLogin, 
                                //t.IsProfilePicUploaded,
                                //t.Salutation,  
                                IsOffersOptedIn = t.Isofferoptedin
                            })
                            .FirstOrDefault();

                        return Ok(user);
                    }
                    else
                    {
                        return BadRequest("User with that token do not exist");
                    }
                }
            }
            catch (Exception ex)
            {
                Shared.telemetryClient.TrackException(ex);
                return BadRequest(ex.ToString());
            }
        }

        [Route("PostAppUserDevicev2")]
        [HttpPost]
         public async Task<IActionResult> PostAppUserDevicev2([FromBody] AppUserDevice model)
        {
            try
            {
                var temp = await db.Appuserdevices.FirstOrDefaultAsync(w => w.Appuserid == model.Appuserid);
                if (temp == null)
                {
                    var id = await appuserdeviceService.AddEntity(new Appuserdevice
                    {
                        Groupid = model.Groupid,
                        Appuserid = model.Appuserid,
                        Deviceid = model.Deviceid,
                        Devicetype = model.Statusid,
                        Version = model.Version,
                        Createdby = model.Createdby,
                        Createddate = DateTime.Now,
                        Modifiedby = model.Createdby,
                        Modifieddate = DateTime.Now
                    });
                    return Ok(new
                    {
                        id,
                        StatusCode = HttpStatusCode.OK,
                        Msg = "Inserted successfully"
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.Deviceid))
                        temp.Deviceid = model.Deviceid;
                    if (!string.IsNullOrEmpty(model.Version))
                        temp.Version = model.Version;
                    if (model.Devicetype.HasValue)
                        temp.Devicetype = model.Devicetype;
                    if (model.Groupid.HasValue)
                        temp.Groupid = model.Groupid;
                    if (model.Statusid.HasValue)
                        temp.Statusid = model.Statusid;
                    temp.Modifieddate = DateTime.Now;
                    await db.SaveChangesAsync();
                    return Ok(new
                    {
                        StatusCode = HttpStatusCode.OK,
                        Msg = "Updated successfully"
                    });
                }
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
       


        [Route("GetSchoolDetails")]
        [HttpGet]
        public async Task<IActionResult> GetSchoolDetails(int schoolId)
        {
            try
            {
                var temp = await mSchoolService.GetSchoolById(schoolId);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MSchool doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(temp);

            }
            catch (Exception ex) //fulldetails
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }

        [Route("GetNotificationDetails")]
        [HttpGet]
        public async Task<IActionResult> GetNotificationDetails(int id, int messageType)//messageType doesn't seem to matter
        {
            try
            {
                // new fix sanduni 13 / 9 / 2024
                using (var _db = new TpContext())
                {

                    if (messageType == 1 || messageType == 2)
                    {
                        var detail = await _db.TNoticeboardmessages.Where(x => x.Id == id).Include(w => w.Branch.School).FirstOrDefaultAsync();
                        if (detail != null)
                        {
                            NotificationDetailModel nbd = new NotificationDetailModel();

                            nbd.Id = detail.Id;
                            nbd.Attachments = detail.Attachments;
                            DateTime currentTime = (DateTime)detail.Createddate;
                            DateTime x30MinsLater = currentTime.AddHours(5);
                            x30MinsLater = x30MinsLater.AddMinutes(30);
                            nbd.DateTime = x30MinsLater;
                            //nbd.DateTime = (DateTime)detail.Createddate;
                            nbd.Icon = "school.png"; //hardcoded in the old code too
                            nbd.Message = detail.Message;
                            nbd.Subject = detail.Subject;
                            nbd.Name = detail.Branch.School.Name;
                            return Ok(nbd);
                        }
                        return BadRequest("Message not found");
                    }
                    return BadRequest("Enter a valid messageType");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Route("api/GetNoticeBoardMsg")]
        [HttpGet]
        public IActionResult GetNoticeBoardMsg(int SchoolId, int ChildId, int PageSize = 10, int pageNumber = 1)
        {
            try
            {
                var res = tNoticeboardmessageService.GetNoticeBoardMsgForAPI(SchoolId, ChildId, PageSize, pageNumber);
                if (res == null)
                {
                    return NotFound(new
                    {
                        Data = "Noticeboard messages not found",
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
