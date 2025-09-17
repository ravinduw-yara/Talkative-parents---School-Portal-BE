using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;
using Repository;
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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly ILogger<AuthController> logger;
        private readonly ITTokenService tTokenService;

        private readonly IUtilityService utilityService;
        private static IAppuserdeviceService appuserdeviceService;
        TpContext db = new TpContext();

        public abstract class HttpRequest
        {
            public abstract IRequestCookieCollection Cookies { get; set; }
        }

        public AuthController(
            IAuthService _authService,
            ILogger<AuthController> _logger,
            IUtilityService _utilityService,
            ITTokenService _tTokenService,
            IAppuserdeviceService _appuserdeviceService

            )
        {
            authService = _authService;
            logger = _logger;
            utilityService = _utilityService;
            tTokenService = _tTokenService;
            appuserdeviceService = _appuserdeviceService;
        }

        //May 6 2025

        [Route("PostSchoolUserDevice")]
        [HttpPost]
        public async Task<IActionResult> PostSchoolUserDevice([FromBody] MSchoolUserDevice model)
        {
            try
            {
                var temp = await db.MSchoolUserDevices.FirstOrDefaultAsync(w => w.SchoolUserid == model.SchoolUserid);
                if (temp == null)
                {
                    var id = await appuserdeviceService.AddEntity(new Appuserdevice
                    {
                        Groupid = model.Groupid,
                        Appuserid = model.SchoolUserid,
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
        [AllowAnonymous]
        [Route("TeacherValidate")]
        [HttpPost]
        public async Task<IActionResult> TeacherValidate(string username, string password)
        {

            try
            {
                object tempUser = await this.authService.TeacherAuthenticate(username, password);
                if (tempUser == null)
                {
                    return NotFound("Login Failed");
                }
                return Ok(tempUser);

            }
            catch (Exception ex)
            {
                string Message = $"LogIn method visited at {DateTime.UtcNow.ToLongTimeString()} ,LoggedIn By:{string.Empty}";
                logger.LogInformation(Message);
                logger.LogError($"{ex.StackTrace}");
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }

        }
        //Teacher
        [Route("ValidateSubjectTeacher")]
        [HttpPost]
        public async Task<IActionResult> ValidateSubjectTeacher(string username, string password)
        {
            try
            {
                var res = await this.authService.ValidateSubjectTeacher(username, password);
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
        [Route("GetTeacherUserPermissions")]
        [HttpGet]
        public async Task<IActionResult> GetUserTeacherPermissions(int userid)
        {

            try
            {
                object userperm = await this.authService.TeacherUserPermissions(userid);
                if (userperm == null)
                {
                    return NotFound("User Not Found");
                }
                return Ok(userperm);
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }

        }

        [Route("CheckParentPhonenumber")]
        [HttpGet]
        public async Task<IActionResult> CheckParentPhonenumber(string phonenumber)
        {
            try
            {
                var res = await this.authService.CheckParentPhonenumberAPI(phonenumber);
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

        [Route("CheckParentToken")]
        [HttpGet]
        public async Task<IActionResult> CheckParentToken(string phonenumber, string enteredtoken)
        {
            try
            {
                var res = await this.authService.CheckParentTokenAPI(phonenumber, enteredtoken);
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
        [Route("CheckParentEmailExists")]
        [HttpGet]
        public async Task<IActionResult> CheckParentEmailExists(string email)
        {
            try
            {
                var res = await this.authService.CheckParentEmailExistsAPI(email);
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

        [Route("SetParentEmailPassword")]
        [HttpGet]
        public async Task<IActionResult> SetParentEmailPassword(string email, string password, string sendtoken)
        {
            try
            {
                var res = await this.authService.SetParentEmailPasswordAPI(email, password, sendtoken);
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
       
        [Route("CheckParentEmailLogin")]
        [HttpGet]
        public async Task<IActionResult> CheckParentEmailLogin(string email, string otp)
        {
            try
            {
                var res = await this.authService.CheckParentEmailLoginAPI(email, otp);
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
        [Route("CheckForgetPassword")]
        [HttpGet]
        public async Task<IActionResult> CheckForgetPassword(string email)
        {
            try
            {
                var res = await this.authService.CheckForgetPasswordAPI(email);
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
        [Route("SetForgetPassword")]
        [HttpGet]
        public async Task<IActionResult> SetForgetPassword(string email, string password, string sendtoken)
        {
            try
            {
                var res = await this.authService.SetForgetPasswordAPI(email, password, sendtoken);
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

        [AllowAnonymous]
        [Route("Validate")]
        [HttpPost]
        public async Task<IActionResult> Validate(string username, string password)
        {

            try
            {
                object tempUser = await this.authService.Authenticate(username, password);
                if (tempUser == null)
                {
                    return NotFound("Login Failed");
                }
                return Ok(tempUser);

            }
            catch (Exception ex)
            {
                string Message = $"LogIn method visited at {DateTime.UtcNow.ToLongTimeString()} ,LoggedIn By:{ string.Empty}";
                logger.LogInformation(Message);
                logger.LogError($"{ex.StackTrace}");
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }

        }
        
        [Route("GetUserPermissions")]
        [HttpGet]
        public async Task<IActionResult> GetUserPermissions(int userid)
        {

            try
            {
                object userperm = await this.authService.UserPermissions(userid);
                if (userperm == null)
                {
                    return NotFound("User Not Found");
                }
                return Ok(userperm);
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }

        }


        [Route("Logout")]
        [HttpDelete]
        public async Task<IActionResult> Logout(Guid token)
        {
            var temp = await this.tTokenService.GetTokenIDForUpdate(token);
            if(temp != null)
            {
                await this.tTokenService.DeleteToken(temp);

                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }


                return Ok(new { Value = "Logged out successfully" });
            }
            return BadRequest(new { Value = "User not logged in" });
        }

        [Route("SanitizeNumber")]
        [HttpPost]
        public IActionResult SanitizeNumber(string p)
        {
            if (p.StartsWith("0")) p = p.Substring(1);
            return Ok("+" + p.Replace("+", "").Replace(" ", ""));
        }

        [Route("Sha256PasswordGenerator")]
        [HttpPost]
        public string Sha256PasswordGenerator(string rawpassword)
        {
            return authService.Sha256Encryption(rawpassword);
        }


        [Route("zero")]
        [HttpPost]
        public void zero()
        {
            try
            {
                int x = 0;
                int res = 10 / x;
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.WarnException("An exception has occured", ex);
            }
        }
        [Route("GetSMSTokenEduloan")]
        [HttpGet]
        public async Task<IActionResult> GetSMSTokenEduloan(String phonenumber)
        {

            try
            {
                var tempPassword = "";
                var tempnumber = "+94123456789";
                var random = new Random();
                var number = random.Next(100000, 999999);
                //number = 666666; //comment out in production
                tempPassword = number.ToString();
                string mask = "EDULOAN";
                var smsMessage = string.Format("Your Eduloan Code is {0}", number);
                await MSMSService.SendSingleSMS(phonenumber, smsMessage, mask);

                
                return Ok(tempPassword);
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }

        }


    }
}
