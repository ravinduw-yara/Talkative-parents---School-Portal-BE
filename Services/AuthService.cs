using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;

namespace Services
{
    public interface IAuthService
    {
        Task<object> CheckParentPhonenumberAPI(string phonenumber); 
        Task<object> CheckParentTokenAPI(string phonenumber, string enteredtoken);
        Task<object> CheckParentEmailExistsAPI(string email); 
        Task<object> SetParentEmailPasswordAPI(string email, string password, string sendtoken);
        Task<object> CheckParentEmailLoginAPI(string email, string password);
        Task<object> CheckForgetPasswordAPI(string email);
        Task<object> SetForgetPasswordAPI(string email, string password, string sendtoken);

        Task<object> Authenticate(string emailId, string password);
        string Sha256Encryption(string rawpassword);
        Task<Object> UserPermissions(int userid);
        Task<Object> TeacherAuthenticate(string username, string password);
        Task<Object> TeacherUserPermissions(int userid);
        Task<Object> ValidateSubjectTeacher(string username, string password);
    }
    public class AuthService : IAuthService
    {
        private readonly TpContext tpContext;
        //private DbSet<GetAuthReport> localDBSet;
        private readonly IConfiguration configuration;
        //readonly TpContext db4 = new TpContext();
        TpContext db = new TpContext();
        TpContext db2 = new TpContext();
        TpContext db3 = new TpContext();
        private readonly ITTokenService tTokenService;
        private readonly IMSchooluserinfoService mSchooluserinfoService;
        

        public AuthService(ITTokenService _tTokenService, TpContext _tpContext, IMSchooluserinfoService _mSchooluserinfoService,
            IConfiguration configuration)
        {
            tpContext = _tpContext;
            tTokenService = _tTokenService;
            mSchooluserinfoService = _mSchooluserinfoService;
            this.configuration = configuration;
        }
        public class Sendgridtest2
        {
            public string Subject { get; set; }
            public string Message { get; set; }
            public string To { get; set; }
            public string Filename { get; set; }
            public string Base64 { get; set; }
            public string mask { get; set; }

        }
        //Subject Teacher Authenticate
        public async Task<Object> ValidateSubjectTeacher(string username, string password)
        {
            try
            {
                var temp = await this.db.MSchooluserinfos.Where(x => x.Username.Equals(username) && x.Password.Equals(Sha256Encryption(password)) && x.Statusid == 1).Include(x => x.Gender).Include(x => x.Branch).Include(x => x.Branch.School).FirstOrDefaultAsync();

                if (temp?.Subjectteacher != 1 && temp?.Isquantaenabled != 1)
                    return "User does not have required permissions or is not authenticated";

                if (temp != null)
                {
                    var tok = await this.tTokenService.AddToken(new TToken
                    {
                        Id = Guid.NewGuid(),
                        Referenceid = temp.Id,
                        Usertype = "School User",
                        Ttl = DateTime.Now.AddYears(2),
                        Ipaddress = GetIPAddress(),
                        Statusid = 1
                    });

                    //if(temp.Branch.School.Id != 0  temp.Branch.School.Id != null)
                    //{
                    //    var maxcount = 
                    //}

                    AuthenticationModel auth = new AuthenticationModel();
                    auth.Id = temp.Id;
                    auth.Token = tok;
                    auth.Salutation = temp.Salutation;
                    auth.Firstname = temp.Firstname;
                    auth.Middlename = temp.Middlename;
                    auth.Lastname = temp.Lastname;
                    auth.Username = temp.Username;
                    auth.OSAppId = Config.osAppId;
                    auth.OSAuth = Config.osAuth;
                    auth.Code = temp.Code;
                    //auth.Gender = temp.Gender.Type;
                    //auth.GenderId = (int)temp.Genderid;
                    auth.Emailid = temp.Emailid;
                    auth.Profilephoto = temp.Profilephoto;
                    auth.Phonenumber = temp.Phonenumber;
                    auth.SchoolId = temp.Branch.School.Id;
                    auth.AllowedCriticalCount = (int)(db2.MFeatures.FirstOrDefault(x => x.Schoolid == temp.Branch.School.Id)?.Maxmsgcount);
                    auth.School = temp.Branch.School.Name;
                    auth.BranchId = temp.Branch.Id;
                    auth.Branch = temp.Branch.Name;
                    auth.Pincode = temp.Branch.Pincode;
                    auth.Logo = temp.Branch.School.Logo;
                    auth.Staffcount = temp.Branch.School.Staffcount;
                    auth.Allowcategory = temp.Branch.School.Allowcategory;
                    auth.Issbsms = temp.Branch.School.Issbsms;
                    auth.Enddate = temp.Enddate;
                    auth.Statusid = temp.Statusid;

                    var adm_check = await db.MSchooluserroles.Where(x => x.Schooluserid.Equals(temp.Id)).Select(a => a.Category.Role.Rank).FirstOrDefaultAsync();

                    if (adm_check == 1)
                    {
                        auth.IsSchoolAdminUser = true;
                        auth.Isschooluser = true;
                    }
                    else
                    {
                        auth.IsSchoolAdminUser = false;
                        auth.Isschooluser = true;
                    }

                    return (auth);
                }

                return "User not found";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Teacher Authenticate
        public async Task<Object> TeacherAuthenticate(string username, string password)
        {
            try
            {
                var temp = await this.db.MSchooluserinfos.Where(x => x.Username.Equals(username) && x.Password.Equals(Sha256Encryption(password)) && x.Statusid == 1).Include(x => x.Gender).Include(x => x.Branch).Include(x => x.Branch.School).FirstOrDefaultAsync();

                if (temp?.Subjectteacher != 1 &&  temp?.Isquantaenabled != 1)
                    return "User does not have required permissions or is not authenticated";

                if (temp != null)
                {
                    var tok = await this.tTokenService.AddToken(new TToken
                    {
                        Id = Guid.NewGuid(),
                        Referenceid = temp.Id,
                        Usertype = "School User",
                        Ttl = DateTime.Now.AddYears(2),
                        Ipaddress = GetIPAddress(),
                        Statusid = 1
                    });

                    //if(temp.Branch.School.Id != 0  temp.Branch.School.Id != null)
                    //{
                    //    var maxcount = 
                    //}

                    AuthenticationModel auth = new AuthenticationModel();
                    auth.Id = temp.Id;
                    auth.Token = tok;
                    auth.Salutation = temp.Salutation;
                    auth.Firstname = temp.Firstname;
                    auth.Middlename = temp.Middlename;
                    auth.Lastname = temp.Lastname;
                    auth.Username = temp.Username;
                    auth.OSAppId = Config.osAppId;
                    auth.OSAuth = Config.osAuth;
                    auth.Code = temp.Code;
                    //auth.Gender = temp.Gender.Type;
                    //auth.GenderId = (int)temp.Genderid;
                    auth.Emailid = temp.Emailid;
                    auth.Profilephoto = temp.Profilephoto;
                    auth.Phonenumber = temp.Phonenumber;
                    auth.SchoolId = temp.Branch.School.Id;
                    auth.AllowedCriticalCount = (int)(db2.MFeatures.FirstOrDefault(x => x.Schoolid == temp.Branch.School.Id)?.Maxmsgcount);
                    auth.School = temp.Branch.School.Name;
                    auth.BranchId = temp.Branch.Id;
                    auth.Branch = temp.Branch.Name;
                    auth.Pincode = temp.Branch.Pincode;
                    auth.Logo = temp.Branch.School.Logo;
                    auth.Staffcount = temp.Branch.School.Staffcount;
                    auth.Allowcategory = temp.Branch.School.Allowcategory;
                    auth.Issbsms = temp.Branch.School.Issbsms;
                    auth.Enddate = temp.Enddate;
                    auth.Statusid = temp.Statusid;

                    var adm_check = await db.MSchooluserroles.Where(x => x.Schooluserid.Equals(temp.Id)).Select(a => a.Category.Role.Rank).FirstOrDefaultAsync();

                    if (adm_check == 1)
                    {
                        auth.IsSchoolAdminUser = true;
                        auth.Isschooluser = true;
                    }
                    else
                    {
                        auth.IsSchoolAdminUser = false;
                        auth.Isschooluser = true;
                    }

                    return (auth);
                }

                return "User not found";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }//TeacherUserPermissions
        public async Task<Object> TeacherUserPermissions(int userid)
        {
            try
            {
                var temp = await this.db.MSchooluserinfos.Where(x => x.Id.Equals(userid)).FirstOrDefaultAsync();
                if (temp == null)
                    return "User not found";

                if (temp.Subjectteacher != 1 || temp.Isquantaenabled != 1)
                    return "User does not have required permissions";

                UserPermissionModel upm = new UserPermissionModel();

                upm.Id = temp.Id;
                upm.Firstname = temp.Firstname;
                upm.Lastname = temp.Lastname;

                var res4 = db.MSchooluserroles.Where(x => x.Schooluserid.Equals(userid)).Include(x => x.Category).Include(x => x.Group).Include(x => x.Category.Role).Include(x => x.Schooluser).Include(x => x.Schooluser.Branch).Include(x => x.Standardsectionmapping);
                foreach (var item in res4)
                {
                    GetUserClass guc = new GetUserClass();
                    guc.Schoolid = item.Schooluser.Branch.Schoolid;
                    guc.schooluserid = (int)item.Schooluserid;

                    if (item.Standardsectionmapping == null)
                    {
                        guc.Sectionid = null;
                        guc.Standardid = null;
                    }
                    else if (item.Standardsectionmapping.Parentid == null)
                    {
                        guc.Sectionid = null;
                        guc.Standardid = item.Standardsectionmappingid;
                    }
                    else
                    {
                        guc.Sectionid = item.Standardsectionmappingid;
                        guc.Standardid = item.Standardsectionmapping.Parentid;
                    }
                    guc.UserSpecializationCategoryId = item.Categoryid;
                    upm.selectedClass.Add(guc);
                }

                var res2 = db2.MUsermodulemappings.Where(x => x.Schooluserid.Equals(temp.Id)).Include(a => a.Module);
                foreach (var item3 in res2)
                {
                    GetModulePermissions getmod = new GetModulePermissions();
                    getmod.id = (int)item3.Moduleid;
                    getmod.name = item3.Module.Name;
                    getmod.type = item3.Module.Type;
                    getmod.icon = item3.Module.Icon;
                    getmod.state = item3.Module.State;
                    getmod.selected = (bool)item3.Module.Selected;

                    var res3 = db3.MModules.Where(x => x.Parentid.Equals(item3.Moduleid));
                    foreach (var item4 in res3)
                    {
                        GetModuleChildren getch = new GetModuleChildren();
                        getch.name = item4.Name;
                        getch.state = item4.State;
                        getmod.children.Add(getch);
                    }

                    upm.selectedPermission.Add(getmod);
                }
                return upm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<object> CheckParentPhonenumberAPI(string phonenumber)
        {
            try
            {
               // var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();
                if (phonenumber != null)
                {
                    //var details = UserStdSecs(userid);
                    //if (details.Count > 0)
                   // {
                    //    foreach (var item in details)
                      //  {
                            var objresult = CheckParentPhonenumberSP(phonenumber);
                            if (objresult != null)
                            {
                        // int count = objresult.Count();
                        //int CurrentPage = pageNumber;
                        //  int PageSize = nuofRows;
                        // int TotalCount = count;
                        // int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                        var items = objresult;
                              //  var previousPage = CurrentPage > 1 ? "Yes" : "No";
                              //  var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                                var obj = new
                                {
                                    //TotalPages = TotalPages,
                                    items = items
                                };
                                return obj;
                            }
                            else
                            {
                                return (new
                                {
                                    Message = "No data found",
                                    StatusCode = HttpStatusCode.NotFound

                                });
                            }
                        
                    }
                    
                else
                {
                    return (new
                    {
                        Message = "Enter Phone Number",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }

        public async Task<object> CheckParentTokenAPI(string phonenumber, string enteredtoken)
        {
            try
            {
                if (phonenumber != null)
                {
                    var objresult = CheckParentTokenSP(phonenumber, enteredtoken);
                    if (objresult != null)
                    {
                        var items = objresult;
                      
                        var obj = new
                        {
                            items = items
                        };
                        return obj;
                    }
                    else
                    {
                        return (new
                        {
                            Message = "No data found",
                            StatusCode = HttpStatusCode.NotFound

                        });
                    }

                }

                else
                {
                    return (new
                    {
                        Message = "Enter Phone Number",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }

        public async Task<object> CheckParentEmailExistsAPI(string email)
        {
            try
            {
                if (email != null)
                {
                    var objresult = CheckParentEmailExistsSP(email);
                    if (objresult != null)
                    {
                       
                        var items = objresult;
                        var obj = new
                        {
                            items = items
                        };
                        return obj;
                    }
                    else
                    {
                        return (new
                        {
                            Message = "No data found",
                            StatusCode = HttpStatusCode.NotFound

                        });
                    }

                }

                else
                {
                    return (new
                    {
                        Message = "Enter Phone Number",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }
        public async Task<object> SetParentEmailPasswordAPI(string email, string password, string sendtoken)
        {
            try
            {
                if (email != null)
                {
                    var objresult = SetParentEmailPasswordSP(email, password, sendtoken);
                    if (objresult != null)
                    {

                        var items = objresult;
                        var obj = new
                        {
                            items = items
                        };
                        return obj;
                    }
                    else
                    {
                        return (new
                        {
                            Message = "No data found",
                            StatusCode = HttpStatusCode.NotFound

                        });
                    }

                }

                else
                {
                    return (new
                    {
                        Message = "Enter Phone Number",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }

        public async Task<object> CheckParentEmailLoginAPI(string email, string password)
        {
            try
            {
                if (email != null)
                {
                    var objresult = CheckParentEmailLoginSP(email, password);
                    if (objresult != null)
                    {
                        var items = objresult;

                        var obj = new
                        {
                            items = items
                        };
                        return obj;
                    }
                    else
                    {
                        return (new
                        {
                            Message = "No data found",
                            StatusCode = HttpStatusCode.NotFound

                        });
                    }

                }

                else
                {
                    return (new
                    {
                        Message = "Enter Phone Number",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }
        public async Task<object> CheckForgetPasswordAPI(string email)
        {
            try
            {
                if (email != null)
                {
                    var objresult = CheckForgetPasswordSP(email);
                    if (objresult != null)
                    {
                        var items = objresult;

                        var obj = new
                        {
                            items = items
                        };
                        return obj;
                    }
                    else
                    {
                        return (new
                        {
                            Message = "No data found",
                            StatusCode = HttpStatusCode.NotFound

                        });
                    }

                }

                else
                {
                    return (new
                    {
                        Message = "Enter Phone Number",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }
        public async Task<object> SetForgetPasswordAPI(string email, string password, string sendtoken)
        {
            try
            {
                if (email != null)
                {
                    var objresult = SetForgetPasswordSP(email, password, sendtoken);
                    if (objresult != null)
                    {

                        var items = objresult;
                        var obj = new
                        {
                            items = items
                        };
                        return obj;
                    }
                    else
                    {
                        return (new
                        {
                            Message = "No data found",
                            StatusCode = HttpStatusCode.NotFound

                        });
                    }

                }

                else
                {
                    return (new
                    {
                        Message = "Enter Phone Number",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }

        private async Task<Object> CheckParentPhonenumberSP(string phonenumber)
        {
            try { 
            AuthenticationModel auth = new AuthenticationModel();
            var appuser = await this.db.MAppuserinfos.Where(x => x.Phonenumber.Equals(phonenumber) && x.Statusid == 1).FirstOrDefaultAsync();
            if (appuser != null)
            {
                string connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    Random AutToken = new Random();
                    int sendtoken = AutToken.Next();
                    string sendtokenstr = sendtoken.ToString();
                        var tempPassword = sendtokenstr.ToString();
                        var smsMessage = string.Format("Your Talkative Parents Portal Code is {0}", sendtokenstr);
                        string mask = "Talkative Parents";
                        await MSMSService.SendSingleSMS(phonenumber, smsMessage, mask);
                        var res = new List<GetAuthReport>();
                    connection.Open();
                    SqlCommand command = new SqlCommand(ApplicationConstants.UpdateOTPAppuser, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@phonenumber", SqlDbType.NVarChar));
                    command.Parameters["@phonenumber"].Value = appuser.Phonenumber;
                    command.Parameters.Add(new SqlParameter("@sendtoken", SqlDbType.NVarChar));
                    command.Parameters["@sendtoken"].Value = sendtokenstr;
                    command.ExecuteNonQuery();
                }
                    auth.parentexists = true;
                    auth.error = "Success";
                    db.SaveChanges();
                    return (auth);
                }
            else
            {
                auth.parentexists = false;
                auth.error = "This mobile number is not registered with Talkative Parents. Kindly contact your School to register";
                    return (auth);
                }
            
            }
            catch (Exception ex)
            {
                throw ex;
            }

}
        
        private async Task<Object> CheckParentEmailExistsSP(string email)
        {
            try
            {
                AuthenticationModel auth = new AuthenticationModel();
                var check1 = await this.db.MAppuserinfos.Where(x => x.Emailid.Equals(email) && x.Statusid == 1).FirstOrDefaultAsync();
                if (check1 != null)
                {
                   // Random AutToken = new Random();
                   // int sendtoken = AutToken.Next();
                    var random = new Random();
                    var number = random.Next(100000, 999999);
                    string sendtokenstr = number.ToString();
                    var check2 = await this.db.MAppuserinfos.Where(x => x.Emailid.Equals(email) && x.Statusid == 1 && x.EmailRegistered == true).FirstOrDefaultAsync();
                    if (check2 != null)
                    {
                       
                        string template = EMAIL_Message_Template.template;
                        Sendgridtest2 sg = new Sendgridtest2();
                        sg.Filename = null;
                        sg.Base64 = null;
                        sg.To = email;
                        sg.Subject = "Talkative Parents Token Verification";
                        EmailService emailService = new EmailService();
                        var msg = "Your Talkative Parent Token :" + sendtokenstr +" </a>";
                        sg.Message = template.Replace("MESSAGE_BODY", msg);
                        var sgu1 = emailService.SendrackspaceEmail(sg.Subject, sg.Message, sg.To, sg.Filename, sg.Base64);

                        string connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                        
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                           
                            var res = new List<GetAuthReport>();
                            connection.Open();
                            SqlCommand command = new SqlCommand(ApplicationConstants.UpdateEmailToken, connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar));
                            command.Parameters["@email"].Value = email;
                            command.Parameters.Add(new SqlParameter("@sendtoken", SqlDbType.NVarChar));
                            command.Parameters["@sendtoken"].Value = sendtokenstr;
                            command.ExecuteNonQuery();
                        }
                        auth.parentexists = true;
                        auth.error = "Success";
                        auth.Emailid = email;
                        db.SaveChanges();
                        return (auth);

                    }
                    else
                    {
                        string connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {

                            var res = new List<GetAuthReport>();
                            connection.Open();
                            SqlCommand command = new SqlCommand(ApplicationConstants.UpdateEmailToken, connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar));
                            command.Parameters["@email"].Value = email;
                            command.Parameters.Add(new SqlParameter("@sendtoken", SqlDbType.NVarChar));
                            command.Parameters["@sendtoken"].Value = sendtokenstr;
                            command.ExecuteNonQuery();
                        }
                        db.SaveChanges();
                        string template = EMAIL_Message_Template.template;
                        Sendgridtest2 sg = new Sendgridtest2();
                        sg.Filename = null;
                        sg.Base64 = null;
                        sg.To = email;
                        sg.Subject = "Email Verification";
                        EmailService emailService = new EmailService();
                        var msg = "<a href='http://yara-dev-portal.azurewebsites.net/#/authentication/parent-portal-login?action=parent_email_register&email=" + email + "&token=" + sendtokenstr + "'> Set Password </a>";
                     

                        sg.Message = template.Replace("MESSAGE_BODY", msg);
                        var sgu1 = emailService.SendrackspaceEmail(sg.Subject, sg.Message, sg.To, sg.Filename, sg.Base64);
                        if (sgu1)
                        {
                            auth.parentexists = false;
                            auth.Emailid = email;
                            auth.error = "This email address is not verified with Talkative Parents. Kindly check your email for verification code";
                            return (auth);
                        }
                        else
                        {
                            auth.parentexists = false;
                            auth.Emailid = email;
                            auth.error = "Error";
                            return (auth);
                        }
                        
                    }
                }
                else
                {
                    auth.parentexists = false;
                    auth.Emailid = email;
                    auth.error = "You are not registered with Talkative Parents. Kindly contact the admin";
                    return (auth);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private async Task<Object> CheckParentTokenSP(string phonenumber, string enteredtoken)
        {
            try
            {
                AuthenticationModel auth = new AuthenticationModel();
                var appuser = await this.db.MAppuserinfos.Where(x => x.Phonenumber.Equals(phonenumber) && x.Statusid == 1 && x.Otp.Equals(enteredtoken)).FirstOrDefaultAsync();
                if (appuser != null)
                {
                    auth.error = "Success";
                    auth.Id = appuser.Id;
                    auth.Phonenumber = phonenumber;
                    return (auth);
                }
                else
                {
                    auth.error = "Invalid Token";
                    auth.Phonenumber = phonenumber;
                    return (auth);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private async Task<Object> SetParentEmailPasswordSP(string email, string password, string sendtoken)
        {
            try
            {
                AuthenticationModel auth = new AuthenticationModel();
                var check1 = await this.db.MAppuserinfos.Where(x => x.Emailid.Equals(email) && x.Statusid == 1).FirstOrDefaultAsync();
                if (check1 != null)
                {
                    Random AutToken = new Random();
                    int sendtoken2 = AutToken.Next();
                    string sendtokenstr = sendtoken2.ToString();
                    var check2 = await this.db.MAppuserinfos.Where(x => x.Emailid.Equals(email) && x.Statusid == 1 && x.Otp == sendtoken).FirstOrDefaultAsync();
                    if (check2 != null)
                    {

                        string connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {

                            var res = new List<GetAuthReport>();
                            connection.Open();
                            SqlCommand command = new SqlCommand(ApplicationConstants.UpdateEmailPassword, connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar));
                            command.Parameters["@email"].Value = email;
                            command.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar));
                            command.Parameters["@password"].Value = password;
                            command.ExecuteNonQuery();
                        }
                        auth.parentexists = true;
                        auth.error = "Successfully Registered. Please login now";
                        auth.Emailid = email;
                        db.SaveChanges();
                        return (auth);

                    }
                    else
                    {
                        auth.parentexists = false;
                        auth.Emailid = email;
                        auth.error = "Token is invalid";
                        return (auth);
                    }
                }
                else
                {
                    auth.parentexists = false;
                    auth.Emailid = email;
                    auth.error = "Email is not registered in TP";
                    return (auth);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task<Object> CheckParentEmailLoginSP(string email, string otp)
        {
            try
            {
                AuthenticationModel auth = new AuthenticationModel();
                var appuser = await this.db.MAppuserinfos.Where(x => x.Emailid.Equals(email) && x.Statusid == 1 && x.Otp.Equals(otp)).FirstOrDefaultAsync();
                if (appuser != null)
                {
                    auth.error = "Success";
                    auth.Id = appuser.Id;
                    auth.Emailid = email;
                    return (auth);
                }
                else
                {
                    auth.error = "Unsuccesful";
                    auth.Emailid = email;
                    return (auth);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        

        private async Task<Object> CheckForgetPasswordSP(string email)
        {
            try
            {
                AuthenticationModel auth = new AuthenticationModel();
                var check1 = await this.db.MAppuserinfos.Where(x => x.Emailid.Equals(email) && x.Statusid == 1).FirstOrDefaultAsync();
                if (check1 != null)
                {
                    Random AutToken = new Random();
                    int sendtoken = AutToken.Next();
                    string sendtokenstr = sendtoken.ToString();
                    var check2 = await this.db.MAppuserinfos.Where(x => x.Emailid.Equals(email) && x.Statusid == 1 && x.EmailRegistered == true).FirstOrDefaultAsync();
                    if (check2 != null)
                    {

                        string connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {

                            var res = new List<GetAuthReport>();
                            connection.Open();
                            SqlCommand command = new SqlCommand(ApplicationConstants.UpdateEmailToken, connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar));
                            command.Parameters["@email"].Value = email;
                            command.Parameters.Add(new SqlParameter("@sendtoken", SqlDbType.NVarChar));
                            command.Parameters["@sendtoken"].Value = sendtokenstr;
                            command.ExecuteNonQuery();
                        }
                        db.SaveChanges();
                        string template = EMAIL_Message_Template.template;
                        Sendgridtest2 sg = new Sendgridtest2();
                        sg.Filename = null;
                        sg.Base64 = null;
                        sg.To = email;
                        sg.Subject = "Reset Password";
                        EmailService emailService = new EmailService();
                        var msg = "<a href='http://yara-dev-portal.azurewebsites.net/#/authentication/parent-portal-login?action=parent_email_reset&email=" + email + "&token=" + sendtokenstr + "'>Reset Your Password Here - Your Token is " + sendtokenstr + "</a>";
                        sg.Message = template.Replace("MESSAGE_BODY", msg);
                        var sgu1 = emailService.SendrackspaceEmail(sg.Subject, sg.Message, sg.To, sg.Filename, sg.Base64);
                        auth.parentexists = true;
                        auth.error = "Success";
                        auth.Emailid = email;
                        return (auth);

                    }
                    else
                    {
                        string connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {

                            var res = new List<GetAuthReport>();
                            connection.Open();
                            SqlCommand command = new SqlCommand(ApplicationConstants.UpdateEmailToken, connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar));
                            command.Parameters["@email"].Value = email;
                            command.Parameters.Add(new SqlParameter("@sendtoken", SqlDbType.NVarChar));
                            command.Parameters["@sendtoken"].Value = sendtokenstr;
                            command.ExecuteNonQuery();
                        }
                        db.SaveChanges();
                        string template = EMAIL_Message_Template.template;
                        Sendgridtest2 sg = new Sendgridtest2();
                        sg.Filename = null;
                        sg.Base64 = null;
                        sg.To = email;
                        sg.Subject = "Set Password";
                        EmailService emailService = new EmailService();
                        var msg = "<a href='http://yara-dev-portal.azurewebsites.net/#/authentication/parent-portal-login?action=parent_email_reset&email=" + email + "&token=" + sendtokenstr + "'>Reset Your Password Here - Your Token is " + sendtokenstr + "</a>";
                        sg.Message = template.Replace("MESSAGE_BODY", msg);
                        var sgu1 = emailService.SendrackspaceEmail(sg.Subject, sg.Message, sg.To, sg.Filename, sg.Base64);
                        if (sgu1)
                        {
                            auth.parentexists = false;
                            auth.Emailid = email;
                            auth.error = "This email address is not verified with Talkative Parents. Kindly check your email for verification code";
                            return (auth);
                        }
                        else
                        {
                            auth.parentexists = false;
                            auth.Emailid = email;
                            auth.error = "Error";
                            return (auth);
                        }

                    }
                }
                else
                {
                    auth.parentexists = false;
                    auth.Emailid = email;
                    auth.error = "You are not registered with Talkative Parents. Kindly contact the admin";
                    return (auth);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        
            private async Task<Object> SetForgetPasswordSP(string email, string password, string sendtoken)
        {
            try
            {
                AuthenticationModel auth = new AuthenticationModel();
                var check1 = await this.db.MAppuserinfos.Where(x => x.Emailid.Equals(email) && x.Statusid == 1).FirstOrDefaultAsync();
                if (check1 != null)
                {
                    var check2 = await this.db.MAppuserinfos.Where(x => x.Emailid.Equals(email) && x.Statusid == 1 && x.Otp == sendtoken).FirstOrDefaultAsync();
                    if (check2 != null)
                    {

                        string connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {

                            var res = new List<GetAuthReport>();
                            connection.Open();
                            SqlCommand command = new SqlCommand(ApplicationConstants.UpdateEmailPassword, connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar));
                            command.Parameters["@email"].Value = email;
                            command.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar));
                            command.Parameters["@password"].Value = password;
                            command.ExecuteNonQuery();
                        }
                        auth.parentexists = true;
                        auth.error = "Successfully Password Reset. Please login now";
                        auth.Emailid = email;
                        db.SaveChanges();
                        return (auth);

                    }
                    else
                    {
                        auth.parentexists = false;
                        auth.Emailid = email;
                        auth.error = "Token is invalid";
                        return (auth);
                    }
                }
                else
                {
                    auth.parentexists = false;
                    auth.Emailid = email;
                    auth.error = "Email is not registered in TP";
                    return (auth);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<Object> Authenticate(string username, string password)
        {
            try
            {
                var temp = await this.db.MSchooluserinfos.Where(x => x.Username.Equals(username) && x.Password.Equals(Sha256Encryption(password)) && x.Statusid == 1).Include(x => x.Gender).Include(x => x.Branch).Include(x => x.Branch.School).FirstOrDefaultAsync();
                if (temp != null)
                {
                    var tok = await this.tTokenService.AddToken(new TToken
                    {
                        Id = Guid.NewGuid(),
                        Referenceid = temp.Id,
                        Usertype = "School User",
                        Ttl = DateTime.Now.AddYears(2),
                        Ipaddress = GetIPAddress(),
                        Statusid = 1
                    });

                    //if(temp.Branch.School.Id != 0 || temp.Branch.School.Id != null)
                    //{
                    //    var maxcount = 
                    //}

                    AuthenticationModel auth = new AuthenticationModel();
                    auth.Id = temp.Id;
                    auth.Token = tok;
                    auth.Salutation = temp.Salutation;
                    auth.Firstname = temp.Firstname;
                    auth.Middlename = temp.Middlename;
                    auth.Lastname = temp.Lastname;
                    auth.Username = temp.Username;
                    auth.OSAppId = Config.osAppId;
                    auth.OSAuth = Config.osAuth;
                    auth.Code = temp.Code;
                    //auth.Gender = temp.Gender.Type;
                    //auth.GenderId = (int)temp.Genderid;
                    auth.Emailid = temp.Emailid;
                    auth.Profilephoto = temp.Profilephoto;
                    auth.Phonenumber = temp.Phonenumber;
                    auth.SchoolId = temp.Branch.School.Id;
                    auth.AllowedCriticalCount = (int)(db2.MFeatures.FirstOrDefault(x => x.Schoolid == temp.Branch.School.Id)?.Maxmsgcount);
                    auth.School = temp.Branch.School.Name;
                    auth.BranchId = temp.Branch.Id;
                    auth.Branch = temp.Branch.Name;
                    auth.Pincode = temp.Branch.Pincode;
                    auth.Logo = temp.Branch.School.Logo;
                    auth.Staffcount = temp.Branch.School.Staffcount;
                    auth.Allowcategory = temp.Branch.School.Allowcategory;
                    auth.Issbsms = temp.Branch.School.Issbsms;
                    auth.Enddate = temp.Enddate;
                    auth.Statusid = temp.Statusid;
                    auth.usedcount = db2.TNoticeboardmessages.Count(x => x.Ispriority.Equals(1)  && x.Branchid == temp.Branch.Id); //25/2/2024 By Sanduni

                    var adm_check = await db.MSchooluserroles.Where(x => x.Schooluserid.Equals(temp.Id)).Select(a => a.Category.Role.Rank).FirstOrDefaultAsync();

                    if(adm_check == 1)
                    {
                        auth.IsSchoolAdminUser = true;
                        auth.Isschooluser = true;
                    }
                    else
                    {
                        auth.IsSchoolAdminUser = false;
                        auth.Isschooluser = true;
                    }

                    return (auth);
                }

                return "User not found";
            }
            catch (Exception ex)
            {
                throw ex;
            } 

        }


        public async Task<Object> UserPermissions(int userid)
        {
            try
            {
                var temp = await this.db.MSchooluserinfos.Where(x => x.Id.Equals(userid)).FirstOrDefaultAsync();
                UserPermissionModel upm = new UserPermissionModel();

                upm.Id = temp.Id;
                upm.Firstname = temp.Firstname;
                upm.Lastname = temp.Lastname;
               
                upm.Isquantaenabled = (int)temp.Isquantaenabled; //sanduni july 2024
                if(upm.Isquantaenabled == null)
                {
                    upm.Isquantaenabled = 0;
                }
                var res4 = db.MSchooluserroles.Where(x => x.Schooluserid.Equals(userid)).Include(x => x.Category).Include(x => x.Group).Include(x => x.Category.Role).Include(x => x.Schooluser).Include(x => x.Schooluser.Branch).Include(x => x.Standardsectionmapping);
                foreach (var item in res4)
                {
                    GetUserClass guc = new GetUserClass();
                    guc.Schoolid = item.Schooluser.Branch.Schoolid;
                    guc.schooluserid = (int)item.Schooluserid;

                    if (item.Standardsectionmapping == null)
                    {
                        guc.Sectionid = null;
                        guc.Standardid = null;
                    }
                    else if (item.Standardsectionmapping.Parentid == null)
                    {
                        guc.Sectionid = null;
                        guc.Standardid = item.Standardsectionmappingid;
                    }
                    else
                    {
                        guc.Sectionid = item.Standardsectionmappingid;
                        guc.Standardid = item.Standardsectionmapping.Parentid;
                    }
                    guc.UserSpecializationCategoryId = item.Categoryid;
                    upm.selectedClass.Add(guc);
                }

                var res2 = db2.MUsermodulemappings.Where(x => x.Schooluserid.Equals(temp.Id)).Include(a => a.Module);
                foreach (var item3 in res2)
                {
                    GetModulePermissions getmod = new GetModulePermissions();
                    getmod.id = (int)item3.Moduleid;
                    getmod.name = item3.Module.Name;
                    getmod.type = item3.Module.Type;
                    getmod.icon = item3.Module.Icon;
                    getmod.state = item3.Module.State;
                    getmod.selected = (bool)item3.Module.Selected;

                    var res3 = db3.MModules.Where(x => x.Parentid.Equals(item3.Moduleid));
                    foreach (var item4 in res3)
                    {
                        GetModuleChildren getch = new GetModuleChildren();
                        getch.name = item4.Name;
                        getch.state = item4.State;
                        getmod.children.Add(getch);
                    }

                    upm.selectedPermission.Add(getmod);
                }
                return upm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public string Sha256Encryption(string rawpassword)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {

                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawpassword));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
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
