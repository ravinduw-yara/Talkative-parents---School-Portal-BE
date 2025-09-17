
using Repository.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
//using System.Web.Http.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Azure.Core;

namespace TalkativeParentAPI_APP.Membership
{


    #region authorizeapi
    //public static class HttpRequestMessageExtension
    //{
    //    private static int appUserId { set; get; }
    //    public static void SetAppUserId(this HttpRequestMessage request, int newAppUserId)
    //    {
    //        appUserId = newAppUserId;
    //    }
    //    public static int GetAppUserId(this HttpRequestMessage request)
    //    {
    //        return appUserId;
    //    }
    //    public static int InvalidateToken(this HttpRequestMessage request)
    //    {
    //        return appUserId;
    //    }
    //}

    //public class AuthorizeApi : AuthorizeAttribute
    //{

    //    private readonly IMembershipService _membershipService = new TalkativeApiMembershipProvider();

    //    public string AppUser { get; private set; }

    //    public override void OnAuthorization(HttpActionContext actionContext)
    //    {
    //        IEnumerable<string> authTokens;
    //        actionContext.Request.Headers.TryGetValues("authToken", out authTokens);

    //        if (authTokens != null)
    //        {
    //            string authToken = authTokens.FirstOrDefault();
    //            if (authToken != null)
    //            {
    //                if (_membershipService.ValidateToken(authToken))
    //                {
    //                    //HttpContext.Current.Response.AddHeader("authToken", authToken);
    //                    //HttpContext.Current.Response.AddHeader("AuthStatus", "Authorized");
    //                    actionContext.Request.SetAppUserId(_membershipService.GetAppUserId(authToken));
    //                    return;
    //                }
    //                //HttpContext.Current.Response.AddHeader("authToken", authToken);
    //                //HttpContext.Current.Response.AddHeader("AuthStatus", "NotAuthorized");
    //                //actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
    //                actionContext.Response.ReasonPhrase = "Not Authorised";
    //                return;
    //            }
    //        }
    //        //actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.ExpectationFailed);
    //        actionContext.Response.ReasonPhrase = "Please Provide authToken";
    //        base.OnAuthorization(actionContext);
    //    }

    //}
    #endregion

    #region CustomAuthorization
    public static class AuthExtensions 
    {
        private static int appUserId { set; get; }
        public static void SetAppUserId(int newAppUserId)
        {
            appUserId = newAppUserId;
        }
        public static int GetAppUserId()
        {
            return appUserId;
        }
        public static int InvalidateToken()
        {
            return appUserId;
        }
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class CustomAuthorization : Attribute, IAuthorizationFilter
    {
        private readonly IMembershipService _membershipService = new TalkativeApiMembershipProvider();
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext != null)
            {
                Microsoft.Extensions.Primitives.StringValues authTokens;
                filterContext.HttpContext.Request.Headers.TryGetValue("authToken", out authTokens);

                var _token = authTokens.FirstOrDefault();

                if (_token != null)
                {
                    string authToken = _token;
                    if (authToken != null)
                    {

                        if (_membershipService.ValidateToken(authToken))
                        {
                            filterContext.HttpContext.Response.Headers.Add("authToken", authToken);
                            filterContext.HttpContext.Response.Headers.Add("AuthStatus", "Authorized");
                            AuthExtensions.SetAppUserId(_membershipService.GetAppUserId(authToken));
                            return;
                        }
                        filterContext.HttpContext.Response.Headers.Add("authToken", authToken);
                        filterContext.HttpContext.Response.Headers.Add("AuthStatus", "NotAuthorized");
                        filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Not Authorized";
                        filterContext.Result = new JsonResult("Not Authorized")
                        {
                            Value = new
                            {
                                Message = "Not Authorised",
                                StatusCode = HttpStatusCode.BadRequest
                            },
                        };
                    }
                    filterContext.HttpContext.Response.Headers.Add("authToken", authToken);
                    filterContext.HttpContext.Response.Headers.Add("AuthStatus", "NotAuthorized");
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Not Authorized";
                    filterContext.Result = new JsonResult("Not Authorized")
                    {
                        Value = new
                        {
                            Message = "Not Authorised",
                            StatusCode = HttpStatusCode.BadRequest
                        },
                    };
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Please Provide authToken";
                    filterContext.Result = new JsonResult("Please Provide an authToken")
                    {
                        Value = new
                        {
                            Message = "Please Provide authToken",
                            StatusCode = HttpStatusCode.NotFound
                        },
                    };
                }
            }
        }
    }
    #endregion


    public interface IMembershipService
    {
        TToken ValidateUser(string userName, string password);
        bool ValidateToken(string authtoken);
        bool InvalidateToken(string authtoken);
        int GetAppUserId(string authtoken);
    }

    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string token { get; set; }
    }
    public class TalkativeApiMembershipProvider : IMembershipService, IPostConfigureOptions<BasicAuthenticationOptions>
    {
        private readonly TpContext _db = new TpContext();
        public TToken ValidateUser(string userName, string password)
        {
            //new fix sanduni 13/9/2024
            using (var db = new TpContext())
            {
                MAppuserinfo appUser = db.MAppuserinfos.FirstOrDefault(u => u.Phonenumber == userName && u.Password == password && u.Statusid == 1);
                if (appUser != null)
                {
                    TToken token = new TToken { Referenceid = appUser.Id, Ttl = DateTime.UtcNow.AddDays(100) };
                    db.TTokens.Add(token);
                    db.SaveChanges();

                    return token;
                }
                return null;
            }
        }

       
        public int GetAppUserId(string authToken)
        {
            Guid authTokenGuid = Guid.Parse(authToken);
            //new fix sanduni 13/9/2024
            using (var db = new TpContext())
            {
                var token = db.TTokens.Where(w => w.Id.ToString() == authToken).FirstOrDefault();
                if (token == null)
                {
                    return 0;
                }
                else
                {
                    var appUserId = db.MAppuserinfos.Where(w => w.Id == token.Referenceid).FirstOrDefault().Id;
                    return appUserId;
                }
            }
               
        }

        public bool ValidateToken(string authtoken)
        {   //new fix sanduni 13/9/2024
            using (var db = new TpContext())
            {
                var token =
            db.TTokens.Where(t => t.Id.ToString() == authtoken && t.Statusid == 1); //&& t.AppUser.IsDisabled == false
                if (token == null)
                    return false;
                else
                    return true;
            }
        }


        public bool InvalidateToken(string authtoken)
        {
            try
            {
                // new fix sanduni 13 / 9 / 2024
              using (var db = new TpContext()) 
                { 
                    Guid authTokenGuid = Guid.Parse(authtoken);
                var token = db.TTokens.Where(t => t.Id == authTokenGuid).FirstOrDefault();
                token.Statusid = 1;
                    //token.DeactivatedOn = DateTime.UtcNow;
                    db.SaveChanges();
                return true;
            }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public void PostConfigure(string name, BasicAuthenticationOptions options)
        {
            if (string.IsNullOrEmpty(options.token))
            {
                throw new InvalidOperationException("token must be provided in options");
            }
        }
    }

}
