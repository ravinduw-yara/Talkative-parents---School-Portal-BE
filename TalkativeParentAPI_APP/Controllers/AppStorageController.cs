using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using Repository.DBContext;
using Services;
using System;
using System.IO;
using System.Linq;
using TalkativeParentAPI_APP.Membership;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TalkativeParentAPI_APP.Controllers
{
    [CustomAuthorization]
    [Route("api/Storage")]
    [ApiController]
    public class AppStorageController : ControllerBase
    {

        private readonly ICloudBlobManager cloudBlobManager;
        private readonly TpContext db = new TpContext();

        public AppStorageController(ICloudBlobManager _cloudBlobManager)
        {
            cloudBlobManager = _cloudBlobManager;
        }

       

        [Route("UploadFilev2")]
        [HttpPost]
        public IActionResult UploadFilev2(FilesStatus.FileUploadDetails fileDetails)
        {
            try
            {
                var filename = cloudBlobManager.StoreFileInAzureStorageV2(fileDetails);

                // Check if the file upload was successful and the file exists
                //if (string.IsNullOrEmpty(filename) || !cloudBlobManager.DoesFileExist(filename))
                //{
                //    return BadRequest("File upload failed or file does not exist. Please try again.");
                //}

                if (fileDetails.isProfile)
                {
                    var user = db.MAppuserinfos.FirstOrDefault(w => w.Id == fileDetails.profileId);
                    if (user != null)
                    {
                        // user.IsProfilePicUploaded = true;
                        db.SaveChanges();
                    }
                }

                if (fileDetails.isSchool)
                {
                    var school = db.MSchools.FirstOrDefault(w => w.Id == fileDetails.profileId);
                    if (school != null)
                    {
                        db.SaveChanges();
                    }
                }

                return Ok(new { filename });
            }
            catch (Exception ex)
            {
                Shared.telemetryClient.TrackException(ex);
                return BadRequest("An error occurred during the file upload. Please try again later.");
            }
        }





        //[Route("UploadFilev2")]
        ////[AllowReferrer]
        //[HttpPost]
        //public IActionResult UploadFilev2(FilesStatus.FileUploadDetails fileDetails)
        //{
        //    try
        //    {
        //        string filename = cloudBlobManager.StoreFileInAzureStorageV2(fileDetails);
        //        if (fileDetails.isProfile)
        //        {
        //            var user = db.MAppuserinfos.Where(w => w.Id == fileDetails.profileId).FirstOrDefault();
        //            //user.IsProfilePicUploaded = true;
        //            db.SaveChanges();
        //        }
        //        if (fileDetails.isSchool)
        //        {
        //            var school = db.MSchools.Where(w => w.Id == fileDetails.profileId).FirstOrDefault();
        //            db.SaveChanges();
        //        }
        //        var obj = new
        //        {
        //            filename = filename,
        //        };
        //        return Ok(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.telemetryClient.TrackException(ex);
        //        return BadRequest(ex.ToString());
        //    }
        //}


        //[AttributeUsage(AttributeTargets.Method)]
        //public class AllowReferrerAttribute : System.Web.Http.Filters.ActionFilterAttribute
        //{
        //    public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        //    {
        //        var ctx = (System.Web.HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"];
        //        var referrer = ctx.Request.UrlReferrer;

        //        if (referrer != null)
        //        {
        //            string refhost = referrer.Host;
        //            string thishost = ctx.Request.Url.Host;
        //            if (refhost != thishost)
        //                ctx.Response.AddHeader("Access-Control-Allow-Origin", string.Format("{0}://{1}", referrer.Scheme, referrer.Authority));
        //        }
        //        base.OnActionExecuting(actionContext);
        //    }
        //}
    }
}
