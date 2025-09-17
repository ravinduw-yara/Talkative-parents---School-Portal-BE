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
    [Route("api/")]
    [ApiController]
    public class ForumMessageController : ControllerBase
    {
        private readonly IFirebaseService firebaseService;

        public ForumMessageController(IFirebaseService _firebaseService)
        {
            firebaseService = _firebaseService;
        }

        [HttpGet]
        [Route("GetForumComments")]
        public IActionResult GetForumComments(int schoolid, int forumid)
        {
            try
            {
                dynamic jsonvalues = firebaseService.GetForumCommentsMethod(schoolid, forumid);
                List<GetForumComments> datalist = new List<GetForumComments>();
                if (jsonvalues != null)
                {
                    foreach (var jsondata in jsonvalues.Values)
                    {
                        datalist.Add(new GetForumComments
                        {
                            commentedById = jsondata["commentedById"],
                            commentedByName = jsondata["commentedByName"],
                            forumid = jsondata["forumid"],
                            message = jsondata["message"],
                            numberUploaded = jsondata["numberUploaded"],
                            commentedOn = jsondata["commentedOn"],
                            updateddate = jsondata["updateddate"],
                            isActive = jsondata["isActive"],
                            attachment = jsondata["attachment"]
                        });
                    }
                }
                else
                {
                    return BadRequest("No comments found for these message");
                }
                return Ok(datalist);
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return BadRequest(a);
            }

        }

    }
}
