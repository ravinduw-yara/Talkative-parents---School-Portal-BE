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
using System.Net.Sockets;

namespace TalkativeParentAPI_APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();


        [AllowAnonymous]
        [Route("Prelogin2")]
        [HttpPost]
        public async Task<IActionResult> Prelogin2(UserModelPreLogin userModel) //model has changed slightly from before to concur with the new tables. Added code, middlename and Dob
        {
            try
            {
                //var tempPassword = "";
                //var tempnumber = "+94123456789";
                var tempPassword = "123456";
                var tempnumber = "+94777774444";
                if (userModel.UserName == null || userModel.UserName == "")
                    return BadRequest();

                userModel.UserName = SanitizeNumber(userModel.UserName);


                var existingUser = await db.MAppuserinfos.Where(w => w.Phonenumber == userModel.UserName).FirstOrDefaultAsync(); //phone no is username
                
                //if (null != existingUser && existingUser.Statusid == 2)
                if (existingUser == null)
                {
                    return Unauthorized();
                }
                else if (userModel.UserName.Length <= 8) // +91123 => 0 to 99999
                {
                    tempPassword = "123456";
                } 
                else if ((!userModel.UserName.StartsWith("+91") && !userModel.UserName.StartsWith("+346") && !userModel.UserName.StartsWith("+94") && !userModel.UserName.StartsWith("+34")) || userModel.UserName == tempnumber) // +91123 => 0 to 99999
                {
                    tempPassword = "123456";
                }

                else
                {
                    try
                    {
                        var childid = await db.MParentchildmappings.Where(x => x.Appuserid == existingUser.Id).Select(m => m.Childid).FirstOrDefaultAsync();
                        var schoolid = await db.MChildschoolmappings.Where(x => x.Childid == childid).Select(x => x.Standardsectionmapping.Branch.Schoolid).FirstOrDefaultAsync();
                        //var ismigrated = await db.MSchools.Where(x => x.Id == schoolid).Select(m => m.ismigrated).FirstOrDefaultAsync();

                        ////alternate
                        //var schoolid = await (from c in db.MParentchildmappings
                        //                      join cm in db.MChildschoolmappings on c.Childid equals cm.Childid
                        //                      join ssm in db.MStandardsectionmappings on cm.Standardsectionmappingid equals ssm.Id
                        //                      join b in db.MBranches on ssm.Branchid equals b.Id
                        //                      where c.Appuserid == existingUser.Id
                        //                      select b.Schoolid).FirstOrDefaultAsync();


                        string mask = await db2.MFeatures.Where(a => a.Schoolid == schoolid).Select(m => m.Mask).FirstOrDefaultAsync();

                        var random = new Random();
                        var number = random.Next(100000, 999999);
                        //number = 666666; //comment out in production
                        tempPassword = number.ToString();
                        var smsMessage = string.Format("Your Talkative Code is {0}", number);
                        await MSMSService.SendSingleSMS(userModel.UserName, smsMessage, mask);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new
                        {
                            Message = ex.Message,              // The error message of the exception
                            StackTrace = ex.StackTrace,        // The stack trace of the exception
                            InnerException = ex.InnerException?.Message,  // Inner exception message (if any)
                            Source = ex.Source,                // The source of the exception
                            ExceptionType = ex.GetType().Name  // The type of the exception
                        });
                        // return BadRequest("FAILURE - OTP not sent to mobile");
                    }
                }
               
                if (existingUser != null)
                {
                    try
                    {
                        existingUser.Password = tempPassword;
                        await db.SaveChangesAsync();
                        PreloginModel val1 = new PreloginModel();
                        val1.Status = true;
                        return Ok(val1); // Existing User
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new
                        {
                            Message = ex.Message,              // The error message of the exception
                            StackTrace = ex.StackTrace,        // The stack trace of the exception
                            InnerException = ex.InnerException?.Message,  // Inner exception message (if any)
                            Source = ex.Source,                // The source of the exception
                            ExceptionType = ex.GetType().Name  // The type of the exception
                        });
                        //return BadRequest(ex);
                    }

                }
                else
                {
                    try
                    {
                        //MAppuserinfo newuser = new MAppuserinfo();// you should never use this part.
                        //newuser.Password = tempPassword;
                        //newuser.Code = userModel.Code;
                        //newuser.Phonenumber = userModel.UserName;
                        //newuser.Firstname = userModel.FirstName;
                        //newuser.Lastname = userModel.LastName;
                        //newuser.Middlename = userModel.MiddleName;
                        //newuser.Emailid = userModel.EmailAddress;
                        //newuser.Genderid = userModel.Gender;
                        //newuser.Createddate = DateTime.Now;
                        //newuser.Modifieddate = DateTime.Now;
                        //newuser.Dob = userModel.Dob;
                        //newuser.Statusid = 1;

                        //db.MAppuserinfos.Add(newuser);
                        //await db.SaveChangesAsync();
                        //PreloginModel val1 = new PreloginModel();
                        //val1.Status = true;
                        //return Ok(val1);
                       return BadRequest("FAILURE - Contact Administrator"); 
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("failure - otp not sent to mobile");
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,              // The error message of the exception
                    StackTrace = ex.StackTrace,        // The stack trace of the exception
                    InnerException = ex.InnerException?.Message,  // Inner exception message (if any)
                    Source = ex.Source,                // The source of the exception
                    ExceptionType = ex.GetType().Name  // The type of the exception
                });
                // Shared.telemetryClient.TrackException(ex);
                // return BadRequest(ex.ToString());
            }
        }
        
        [AllowAnonymous]
        [Route("Validate2")]
        [HttpPost]
        public async Task<IActionResult> Validate2(UserModelValidate userModel)//simlified the old model, only username and password are enough
        {
            try
            {
                if (userModel.UserName == null) return BadRequest();
                userModel.UserName = SanitizeNumber(userModel.UserName);
                var existingUser = db.MAppuserinfos.Where(w => w.Phonenumber == userModel.UserName && w.Password == userModel.Password).FirstOrDefault();

                if (null != existingUser)
                {
                    var existToken = await db.TTokens.Where(w => w.Referenceid == existingUser.Id && w.Usertype == "App User").FirstOrDefaultAsync();
                    if (existToken == null)
                    {
                       // var appuserdevice = await db.Appuserdevices.Where(w => w.Appuserid == existingUser.Id).FirstOrDefaultAsync();
                        TToken token = new TToken();
                        token.Id = Guid.NewGuid();
                        token.Referenceid = existingUser.Id;
                        token.Statusid = 1;
                        token.Ttl = DateTime.Now.AddDays(100);
                        token.Usertype = "App User";
                        token.Ipaddress = GetIPAddress();
                        ValidateModel val = new ValidateModel();
                        val.Status = 1;
                        val.TokenId = token.Id;
                        db.TTokens.Add(token);
                        await db.SaveChangesAsync();
                        return Ok(val);
                    }
                    else if (existToken != null && existToken.Ttl < DateTime.Now)
                    {
                        // Update the existing token
                        existToken.Ttl = DateTime.Now.AddDays(100);
                        existToken.Statusid = 1;
                        existToken.Ipaddress = GetIPAddress();

                        ValidateModel val = new ValidateModel
                        {
                            Status = 1,
                            TokenId = existToken.Id
                        };

                        db.TTokens.Update(existToken);  // Update the existing token in the database
                        await db.SaveChangesAsync();     // Save changes to the database

                        //TToken token = new TToken(); //Sanduni 9/24/2024
                        //token.Id = Guid.NewGuid();
                        //token.Referenceid = existingUser.Id;
                        //token.Statusid = 1;
                        //token.Ttl = DateTime.Now.AddDays(100);
                        //token.Usertype = "App User";
                        //token.Ipaddress = GetIPAddress();
                        //ValidateModel val = new ValidateModel();
                        //val.Status = 1;
                        //val.TokenId = token.Id;
                        //db.TTokens.Add(token);
                        //await db.SaveChangesAsync();
                        return Ok(val);
                    }
                    else
                    {
                        ValidateModel val = new ValidateModel();
                        val.Status = 1;
                        val.TokenId = existToken.Id;
                        return Ok(val);
                    }
                }
                else
                {
                    ValidateModel1 val = new ValidateModel1();
                    val.Status = 0;
                    val.TokenId = "";
                    return Ok(val);
                }
            }
            catch (Exception ex)
            {
                Shared.telemetryClient.TrackException(ex);
                return BadRequest(ex.ToString());
            }
        }


        private string SanitizeNumber(string p)
        {
            if (p.StartsWith("+"))
                p = p.Substring(1);
            return "+" + p.Replace("+", "").Replace(" ", "");
        }


        private string GetIPAddress()
        {
            string localIP = string.Empty;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }


    }
}
