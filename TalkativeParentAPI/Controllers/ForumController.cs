using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumController : ControllerBase
    {
        private readonly IFirebaseService firebaseService;

        public ForumController(IFirebaseService _firebaseService)
        {
            firebaseService = _firebaseService;
        }

        [HttpPost]
        [Route("PostForumComments")]
        public IActionResult PostForumComments(ForumComments comments)
        {
            try
            {
                var json = firebaseService.PostForumCommentsMethod(comments);
                if (json != null)
                {
                    return Ok("Replied Successfully");
                }
                else
                {
                    return BadRequest("Reply not sent");
                }
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return BadRequest(a);
            }
        }


        [HttpGet]
        [Route("GetForumComments")]
        public IActionResult GetForumComments(int schoolid, int messageid)
        {
            try
            {
                dynamic jsonvalues = firebaseService.GetForumCommentsMethod(schoolid, messageid);
                List<GetForumComments> datalist = new List<GetForumComments>();
                GetForumCommentsList forumcommentlist = new GetForumCommentsList();
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
                //int count = AddCommentCount(schoolid, messageid); //Function uses SP
                forumcommentlist.Commentcount = 666;
                forumcommentlist.Getforumcomments = datalist;
                return Ok(forumcommentlist);
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return BadRequest(a);
            }
            
        }
    }
}

