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

namespace TalkativeParentAPI_APP.Controllers
{
    [CustomAuthorization]
    [Route("api/Child")]
    [ApiController]
    public class ChildrenController : ControllerBase
    {
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext db3 = new TpContext();
        private readonly IMChildschoolmappingService mChildschoolmappingService;
        private static HttpActionContext act = new HttpActionContext();

        public ChildrenController(IMChildschoolmappingService _mChildschoolmappingService)
        {
            mChildschoolmappingService = _mChildschoolmappingService;
        }
        [HttpGet]
        public string ExtractToken()
        {
            const string HeaderKeyName = "authToken";
            Request.Headers.TryGetValue(HeaderKeyName, out Microsoft.Extensions.Primitives.StringValues headerValue);
            return (headerValue);
        }
        [Route("GetChildrenV2Mapping")]
        [HttpGet]
        public async Task<IActionResult> GetChildrenV2Mapping() 
        {
            List<ChildModel> childrenList = new List<ChildModel>();
            try
            {
                var token = ExtractToken();
                var appUserId = db.TTokens.Where(x => x.Id.ToString() == token).Select(a => a.Referenceid).FirstOrDefault();
                //int appUserId = AuthExtensions.GetAppUserId();
                // int appUserId = 14478; //for testing
                var res = await mChildschoolmappingService.GetChildrenV2MappingForAPI((int)appUserId);
                if (res == null)
                {
                    return NotFound(new
                    {
                        Data = "Child Data not found.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                Shared.telemetryClient.TrackException(ex);
                return Ok(childrenList.ToList());
            }

        }
    }
}
