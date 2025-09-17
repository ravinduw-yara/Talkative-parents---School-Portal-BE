using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NWebsec.Core.Common.Web;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkativeParentAPI.Controllers
{
    //[Route("api/")]
    [ApiController]
    public class StorageController : Controller
    {
        private readonly ICloudBlobManager cloudBlobManager;
        private readonly TpContext db = new TpContext();

        public StorageController(ICloudBlobManager _cloudBlobManager)
        {
            cloudBlobManager = _cloudBlobManager;
        }


        [Route("api/Storage/UploadFile/School")]
        [HttpPost]
        public IActionResult UploadFile(FilesStatus2.FileUploadDetails2 fileDetails)
        {
            try
            {
                string filename = cloudBlobManager.StoreFileInAzureStorage(fileDetails).Result; // Not recommended due to blocking.//Octomer 11 2024
               // string filename = cloudBlobManager.StoreFileInAzureStorage(fileDetails); //Octomer 11 2024
                // Check if the file upload was successful and the file exists
                //if (string.IsNullOrEmpty(filename) || !cloudBlobManager.DoesFileExist(filename))
                //{
                //    return BadRequest("File upload failed or file does not exist. Please try again.");
               // }

                if (fileDetails.isProfile)
                {
                    var user = db.MAppuserinfos.Where(w => w.Id.ToString() == fileDetails.profileId).FirstOrDefault();
                    db.SaveChanges();
                }
                if (fileDetails.isSchool)
                {
                    var school = db.MSchools.Where(w => w.Id.ToString() == fileDetails.profileId).FirstOrDefault();
                    school.Logo = filename;
                    db.SaveChanges();
                }
                return Ok(new { value = filename });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        //[Route("api/Storage/UploadFile/School")]
        //[HttpPost]
        //public IActionResult UploadFile(FilesStatus2.FileUploadDetails3 fileDetails)
        //{
        //    try
        //    {
        //        string filename = cloudBlobManager.StoreFileInAzureStorage(fileDetails);
        //        if (fileDetails.isProfile)
        //        {
        //            var user = db.MAppuserinfos.Where(w => w.Id.ToString() == fileDetails.profileId.ToString()).FirstOrDefault();
        //            db.SaveChanges();
        //        }
        //        if (fileDetails.isSchool)
        //        {
        //            var school = db.MSchools.Where(w => w.Id.ToString() == fileDetails.profileId.ToString()).FirstOrDefault();
        //            school.Logo = filename;
        //            db.SaveChanges();
        //        }
        //        return Ok(new { value = filename });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }
        //}

        //[Route("api/Storage/UploadFile/School")]
        //[HttpPost]
        //public IActionResult UploadFile(FilesStatus2.FileUploadDetails2 fileDetails)
        //{
        //    try
        //    {
        //        string filename = cloudBlobManager.StoreFileInAzureStorage(fileDetails);
        //        if (fileDetails.isProfile)
        //        {
        //            var user = db.MAppuserinfos.Where(w => w.Id == fileDetails.profileId).FirstOrDefault();
        //            //user.IsProfilePicUploaded = true;
        //            db.SaveChanges();
        //        }
        //        if (fileDetails.isSchool)
        //        {
        //            var school = db.MSchools.Where(w => w.Id == fileDetails.profileId).FirstOrDefault();
        //            school.Logo = filename;
        //            db.SaveChanges();
        //        }
        //        //return Ok(filename);
        //        return Ok(new { value = filename });
        //    }
        //    catch (Exception ex)
        //    {
        //        //Shared.telemetryClient.TrackException(ex);
        //        return BadRequest(ex.ToString());
        //    }
        //}

    }
}
