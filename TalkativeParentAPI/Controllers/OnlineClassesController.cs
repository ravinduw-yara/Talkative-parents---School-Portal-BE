using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class OnlineClassesController : ControllerBase
    {
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext db3 = new TpContext();
        private readonly TpContext db4 = new TpContext();

        private readonly TpContext tpContext;

        private readonly ITGoogleclassService tGoogleclassService;
        private readonly ITGclvedioclassService tGclvedioclassService;
        private readonly IMSchooluserroleService mSchooluserroleService;


        public OnlineClassesController(ITGoogleclassService tGoogleclassService, ITGclvedioclassService tGclvedioclassService, IMSchooluserroleService mSchooluserroleService, TpContext tpContext, ITGoogleclassService tGoogleclassService1)
        {
            this.tGoogleclassService = tGoogleclassService;
            this.tGclvedioclassService = tGclvedioclassService;
            this.mSchooluserroleService = mSchooluserroleService;
            this.tpContext = tpContext;
        }

        [HttpPost]
        [Route("addVideoClass")]
        public async Task<IActionResult> addVideoClass(GCLVideoClass data)
        {
            if (data.Title.Trim().Length == 0 || data.MeetingLink.Trim().Length == 0)
            {
                return BadRequest(new
                {
                    result = "error",
                    message = "Invalid GCL Data"
                });
            }
            else
            {
                if (data.schoolId == 0 || data.schoolId.Equals(null)
                    || data.standardId.Equals(null) || data.standardId == 0
                    || data.sectionId.Equals(null) || data.sectionId == 0
                    || data.createdBy.Equals(null) || data.createdBy == 0)
                {
                    return BadRequest(new
                    {
                        result = "error",
                        message = "Invalid GCL Data"
                    });
                }
                else
                {
                    try
                    {
                        TGclvedioclass tvc = new TGclvedioclass();
                        tvc.Name = data.Title;
                        tvc.Description = data.Description;
                        tvc.Meetinglink = data.MeetingLink;
                        tvc.Startdate = data.StartDateTime;
                        tvc.Enddate = data.EndDateTime;
                        tvc.Createddate = DateTime.Now;
                        tvc.Modifieddate = DateTime.Now;
                        tvc.Statusid = 1;
                        tvc.Createdby = data.createdBy;
                        tvc.Modifiedby = data.createdBy;
                        tvc.Standardsectionmappingid = data.sectionId;

                        await this.tGclvedioclassService.AddEntity(tvc);

                        return Ok(new
                        {
                            result = "Ok",
                            message = "Video class added successfully"
                        });
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }
                }

            }
        }

        [HttpGet]
        [Route("getVideoClass")]
        public async Task<IActionResult> getVideoClass(int schoolId, int appuserid, Guid authToken)
        {
            try
            {

                int appUserId1 = (int)db.TTokens.FirstOrDefault(w => w.Id == authToken && w.Statusid == 1).Referenceid;
                if (appUserId1 > 0 && appUserId1 == appuserid)
                {
                    IQueryable<TGclvedioclass> dbData = await this.tGclvedioclassService.getVideoClasses(schoolId, appuserid);

                    List<GCLVideoClass> res = new List<GCLVideoClass>();
                    foreach (var item in dbData)
                    {
                        GCLVideoClass gvc = new GCLVideoClass();
                        gvc.Title = item.Name;
                        gvc.Description = item.Description;
                        gvc.MeetingLink = item.Meetinglink;
                        gvc.StartDateTime = item.Startdate;
                        gvc.EndDateTime = item.Enddate;

                        var ssm = db.MStandardsectionmappings.Where(x => x.Id.Equals(item.Standardsectionmappingid)).FirstOrDefault();

                        if (ssm.Parentid == null)
                        {
                            gvc.standardId = item.Standardsectionmappingid;
                            gvc.sectionId = null;
                        }
                        else
                        {
                            gvc.standardId = ssm.Parentid;
                            gvc.sectionId = item.Standardsectionmappingid;
                        }
                        //gvc.createdBy = item.createdBy
                        res.Add(gvc);
                    }
                    return Ok(new { result = "ok", data = res });
                }
                else
                {
                    return BadRequest(new { result = "error", message = "Invalid User" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { result = "error", message = "Invalid credentials" });
            }
        }

        [HttpGet]
        [Route("GetGCLClassStatus")]
        public async Task<IActionResult> GetGCLClassStatus(int SchoolID, int StdID, int SecId)
        {
            try
            {
                var data = await this.db.TGclvedioclasses.FirstOrDefaultAsync(w => w.Standardsectionmappingid == SecId);
                if (data == null)
                {
                    string str = "Not approved";
                    return BadRequest(str);
                }
                else
                {
                    return Ok(new { msg = "Already approved", SecId });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("addGCL")]
        public async Task<IActionResult> addGCL(GCLTemplate data)
        {
            if (data.gcourseid.Trim().Length == 0 || data.gcoursename.Trim().Length == 0 || data.gownerId.Trim().Length == 0 || data.teacherfolderid.Trim().Length == 0)
            {
                return BadRequest(new { result = "error", message = "Invalid GCL Data" });
            }
            else
            {
                if (data.schoolId == 0 || data.schoolId.Equals(null)
                    || data.standardId.Equals(null) || data.standardId == 0
                    || data.sectionId.Equals(null) || data.sectionId == 0
                    || data.createdBy.Equals(null) || data.createdBy == 0)
                {
                    return BadRequest(new
                    {
                        result = "error",
                        message = "Invalid GCL Data"
                    });
                }
                else
                {
                    try
                    {
                        TGoogleclass tgc = new TGoogleclass();
                        tgc.Gcourseid = data.gcourseid;
                        tgc.Name = data.gcoursename;
                        tgc.Gownerid = data.gownerId;
                        tgc.Teacherfolderid = data.teacherfolderid;
                        //data.standardId; 
                        tgc.Standardsectionmappingid = data.sectionId;
                        tgc.Createdby = data.createdBy;
                        tgc.Modifiedby = data.createdBy;
                        tgc.Createddate = DateTime.Now;
                        tgc.Modifieddate = DateTime.Now;
                        tgc.Statusid = 1;

                        await this.tGoogleclassService.AddEntity(tgc);

                        return Ok(new { result = "ok", massage = "class Added Successfully" });
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }
                }

            }
        }

        [HttpGet]
        [Route("getGCL")]
        public async Task<IActionResult> getGCL(int schoolId, int appuserid, Guid authToken)
        {
            try
            {

                int appUserId1 = (int)db.TTokens.FirstOrDefault(w => w.Id == authToken && w.Statusid == 1).Referenceid;
                if (appUserId1 > 0 && appUserId1 == appuserid)
                {
                    IQueryable<TGoogleclass> dbData = await this.tGoogleclassService.getGcl(schoolId, appuserid);

                    List<GCLTemplate> res = new List<GCLTemplate>();
                    foreach (var item in dbData)
                    {
                        GCLTemplate gvc = new GCLTemplate();
                        gvc.gcoursename = item.Name;
                        gvc.gcourseid = item.Gcourseid;
                        gvc.gownerId = item.Gownerid;
                        gvc.teacherfolderid = item.Teacherfolderid;
                        gvc.createdBy = (int)item.Createdby;
                        gvc.schoolId = schoolId;

                        var ssm = db.MStandardsectionmappings.Where(x => x.Id.Equals(item.Standardsectionmappingid)).FirstOrDefault();

                        if (ssm.Parentid == null)
                        {
                            gvc.standardId = item.Standardsectionmappingid;
                            gvc.sectionId = null;
                        }
                        else
                        {
                            gvc.standardId = ssm.Parentid;
                            gvc.sectionId = item.Standardsectionmappingid;
                        }
                        res.Add(gvc);
                    }
                    return Ok(new { result = "ok", data = res });
                }
                else
                {
                    return BadRequest(new { result = "error", message = "Invalid User" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { result = "error", message = "Invalid credentials" });
            }
        }

        [HttpPost]
        [Route("deleteGCL")]
        public async Task<IActionResult> deleteGCL(int appuserid, Guid authToken, int SchoolId, int StandardId, int SectionId, int GCLId)
        {
            try
            {

                int appUserId1 = (int)db.TTokens.FirstOrDefault(w => w.Id == authToken && w.Statusid == 1).Referenceid;
                if (appUserId1 > 0 && appUserId1 == appuserid)
                {
                    var del_rec = await this.tGoogleclassService.GetEntityIDForUpdate(GCLId);
                    if (del_rec != null)
                    {
                        await this.tGoogleclassService.DeleteEntity(del_rec);
                        return Ok(new { result = "OK", message = "Deleted Successfully" });
                    }
                    else
                    {
                        return BadRequest(new { result = "error", message = "No record found" });
                    }
                }
                else
                {
                    return BadRequest(new { result = "error", message = "User don't have previleges to delete" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { result = "error", message = "Invalid credentials" });
            }
        }

        [HttpPost]
        [Route("addteacherclass")]
        public async Task<IActionResult> PostTeacherClass(TeacherClass classDetails)
        {
            try
            {
                if (await mSchooluserroleService.ValidateTeacher(classDetails.schoolId, classDetails.teacherId, classDetails.sectionId))
                {
                    //var ssm = await db.MStandardsectionmappings.Where(x => x.Id == classDetails.sectionId)


                    var tech = await db.TGclteacherclasses.Where(x => x.Branch.Schoolid == classDetails.schoolId && x.Standardsectionmappingid == classDetails.sectionId).FirstOrDefaultAsync();
                    if (tech == null)
                    {
                        TGclteacherclass tgc = new TGclteacherclass();
                        tgc.Branchid = classDetails.schoolId; // not sure for now
                        tgc.TeacherId = classDetails.teacherId;
                        tgc.Createdby = classDetails.createdBy;
                        tgc.Createdby = classDetails.createdBy;
                        tgc.Modifiedby = classDetails.createdBy;
                        tgc.IsApproved = true;
                        tgc.Standardsectionmappingid = classDetails.sectionId;
                        tgc.Createddate = DateTime.Now;
                        tgc.Modifieddate = DateTime.Now;
                        tgc.Statusid = 1;

                        db.TGclteacherclasses.Add(tgc);
                        await db.SaveChangesAsync();
                        return Ok(new
                        {
                            result = "Ok",
                            message = "Class approved successfully."
                        });
                    }
                    return BadRequest(new
                    {
                        result = "Error",
                        message = "Class already approved."
                    });
                }
                return BadRequest(new
                {
                    result = "Error",
                    message = "Teacher not validated."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { result = "error", message = ex.Message });
            }
        }

        [HttpPost]
        [Route("updateGCLcoursename")]
        public async Task<IActionResult> updateGCLcoursename(int appuserid, Guid authToken, int SchoolId, int StandardId, int SectionId, int GCLId, string gcoursename)
        {
            try
            {
                if (SchoolId == 0 || appuserid == 0 || StandardId == 0 || SectionId == 0 || authToken.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    return BadRequest(new
                    {
                        result = "Error",
                        message = "No previleges to access the data"
                    });
                }
                else
                {
                    int appuserid1 = (int)await db4.TTokens.Where(w => w.Id.Equals(authToken) && w.Statusid == 1).Select(w => w.Referenceid).FirstOrDefaultAsync();
                    if (appuserid1 != 0 && appuserid == appuserid1)
                    {
                        if (gcoursename.Trim().Length == 0)
                        {
                            return BadRequest(new { result = "error", message = "GCourse name should not be empty" });
                        }
                        else
                        {
                            var temp = await this.tGoogleclassService.GetEntityIDForUpdate(GCLId);
                            if (temp == null)
                            {
                                return NotFound(new
                                {
                                    result = "error.",
                                    message = "No record found"
                                });
                            }

                            temp.Standardsectionmappingid = SectionId;
                            temp.Name = gcoursename;
                            temp.Modifieddate = DateTime.Now;
                            await this.tGoogleclassService.UpdateEntity(temp);

                            return Ok(new { result = "OK", message = "Updated Successfully" });
                        }
                    }
                    return BadRequest(new { result = "error", message = "User don't have previleges to update" });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new { result = "error", message = ex.Message });
            }
        }

        [HttpGet]
        [Route("getAllTeacherClassandGCL")]
        public async Task<IActionResult> getAllTeacherClassandGCL(int schoolId, int appuserid, Guid authToken)
        {
            try
            {
                if (schoolId == 0 || appuserid == 0 || authToken.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    return BadRequest(new
                    {
                        result = "Error",
                        message = "NO previleges to access the data"
                    });
                }
                else
                {
                    int appuserid1 = (int)await db4.TTokens.Where(w => w.Id.Equals(authToken) && w.Statusid == 1).Select(w => w.Referenceid).FirstOrDefaultAsync();
                    if (appuserid1 != 0 && appuserid == appuserid1)
                    {
                        List<TeacherClassandGCLRes> res = new List<TeacherClassandGCLRes>();

                        var tempDetails = (from T in tpContext.TGclteacherclasses.Where(w => w.Branch.Schoolid == schoolId && w.IsApproved == true)
                                           from ssm in tpContext.MStandardsectionmappings.Where(w => w.Branchid == T.Branchid && w.Id == T.Standardsectionmappingid)
                                           select new { T.Id, T.Branch.Schoolid, T.Standardsectionmappingid, ssm.Parentid, ssm.Name, T.TeacherId, T.IsApproved });

                        //IQueryable<techclassModel> newdet = await this.tGoogleclassService.getTeachClassSP(schoolId); //Alternative SP

                        foreach (var TCD in tempDetails)
                        {
                            var GCLDetails = db.TGoogleclasses.Where(x => x.Standardsectionmappingid == TCD.Standardsectionmappingid && x.Statusid == 1);

                            TeacherClassandGCLRes xRow = new TeacherClassandGCLRes();

                            xRow.id = TCD.Id;


                            if (TCD.Parentid == null)
                            {
                                xRow.standardId = (int)TCD.Standardsectionmappingid;
                                xRow.stdName = TCD.Name;
                                xRow.sectionId = 0;
                                xRow.sectionName = null;
                            }
                            else
                            {
                                xRow.standardId = (int)TCD.Parentid;
                                xRow.stdName = await db2.MStandardsectionmappings.Where(n => n.Id == TCD.Parentid).Select(m => m.Name).FirstOrDefaultAsync();
                                xRow.sectionId = (int)TCD.Standardsectionmappingid;
                                xRow.sectionName = TCD.Name;
                            }

                            xRow.isApproved = (bool)TCD.IsApproved;
                            xRow.teacherid = TCD.TeacherId;
                            xRow.GCL = new List<GCLTemplateNew>();

                            foreach (var GCD in GCLDetails)
                            {
                                GCLTemplateNew GCLRow = new GCLTemplateNew();
                                GCLRow.ID = GCD.Id;
                                GCLRow.gcourseid = GCD.Gcourseid;
                                GCLRow.gcoursename = GCD.Name;
                                GCLRow.gownerId = GCD.Gownerid;
                                GCLRow.teacherfolderid = GCD.Teacherfolderid;
                                xRow.GCL.Add(GCLRow);
                            }
                            res.Add(xRow);
                        }
                        return Ok(new { result = "OK", data = res });

                    }
                    else
                    {
                        return BadRequest(new { result = "error", message = "User don't have previleges to access the data" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { result = "error", message = ex.Message });
            }
        }


        [HttpGet]
        [Route("getAllGCLByClasses")]
        public async Task<IActionResult> getAllGCLByClasses(int schoolId, int appuserid, Guid authToken, int standardid, int sectionid)
        {
            try
            {

                if (schoolId == 0 || appuserid == 0 || standardid == 0 || sectionid == 0 || authToken.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    return BadRequest(new
                    {
                        result = "Error",
                        message = "No previleges to access the data"
                    });
                }
                else
                {
                    int appuserid1 = (int)await db4.TTokens.Where(w => w.Id.Equals(authToken) && w.Statusid == 1).Select(w => w.Referenceid).FirstOrDefaultAsync();
                    if (appuserid1 != 0 && appuserid == appuserid1)
                    {
                        var res = db.TGoogleclasses.Where(w => w.Standardsectionmappingid == sectionid);
                        return Ok(new
                        {
                            result = "OK",
                            data = res
                        });
                    }
                    return BadRequest(new
                    {
                        result = "error",
                        message = "User doesn't have previleges to access the data"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { result = "error", message = ex.Message });
            }
        }


        [HttpGet]
        [Route("getApprovalStudentsListByClass")]
        public async Task<IActionResult> getApprovalStudentsListByClass(int SchoolId, int StandardId, int SectionId)
        {
            try
            {
                List<TeacherEmail> Teacherlist = new List<TeacherEmail>();
                List<ChildEmail> Childlist = new List<ChildEmail>();
                if (SchoolId != 0 && StandardId != 0 && SectionId != 0)
                {

                    var Teacherdata = db.MSchooluserroles.Where(x => x.Category.Role.Rank == 4 && x.Standardsectionmappingid == SectionId).Select(w => w.Schooluserid);
                    // List<object> datalist =new List<object>()

                    foreach (var item in Teacherdata)
                    {
                        if (item != null)
                        {
                            var teachdata = await db2.MSchooluserinfos.Where(x => x.Id == item).FirstOrDefaultAsync();
                            Teacherlist.Add(new TeacherEmail
                            {
                                Id = teachdata.Id,
                                FirstName = teachdata.Firstname,
                                MiddleName = teachdata.Middlename,
                                LastName = teachdata.Lastname,
                                EmailAddress = teachdata.Emailid
                            });
                        }

                    }
                    var Childdata = db.MChildschoolmappings.Where(x => x.Standardsectionmappingid == SectionId).Select(a => a.Childid);
                    foreach (var item in Childdata)
                    {
                        if (item != null)
                        {
                            var childinfo = await db2.MChildinfos.Where(x => x.Id == item).FirstOrDefaultAsync();
                            Childlist.Add(new ChildEmail
                            {
                                Id = childinfo.Id,
                                FirstName = childinfo.Firstname,
                                LastName = childinfo.Lastname,
                                email = childinfo.Email
                            });
                        }
                    }
                    return Ok(new { result = "ok", teacherFolder = Teacherlist, studentsList = Childlist });
                }
                return Ok(new { result = "ok", teacherFolder = Teacherlist, studentsList = Childlist });
            }
            catch (Exception e) { return BadRequest(e.Message); }
        }


        [HttpGet]
        [Route("GetGCLClassSumamry")]
        public async Task<IActionResult> GetGCLClassSumamry(int SchoolId, int appuserid, Guid authToken)
        {
            try
            {

                if (SchoolId == 0 || appuserid == 0 || authToken.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    return BadRequest(new
                    {
                        result = "Error",
                        message = "No previleges to access the data"
                    });
                }
                else
                {
                    int appuserid1 = (int)await db4.TTokens.Where(w => w.Id.Equals(authToken) && w.Statusid == 1).Select(w => w.Referenceid).FirstOrDefaultAsync();
                    if (appuserid1 != 0 && appuserid == appuserid1)
                    {
                        List<TC_GCL_Res> res = new List<TC_GCL_Res>();
                        List<TC_GCL_Res> resT = await tGoogleclassService.GetGCLClassSumamryForAPI(SchoolId, appuserid);
                        if (resT != null)
                        {
                            foreach (var itemT in resT)
                            {
                                TC_GCL_Res xRow = new TC_GCL_Res();
                                xRow.standardId = itemT.standardId;
                                xRow.stdName = itemT.stdName;
                                xRow.sectionId = itemT.sectionId;
                                xRow.sectionName = itemT.sectionName;
                                xRow.isApproved = itemT.isApproved;
                                xRow.GCL = new List<GCLTemplateNew>();

                                var resG = db.TGoogleclasses.Where(w => w.Standardsectionmappingid == xRow.sectionId);
                                if (resG != null)
                                {
                                    foreach (var itemG in resG)
                                    {
                                        GCLTemplateNew GCLRow = new GCLTemplateNew();
                                        GCLRow.ID = itemG.Id;
                                        GCLRow.gcourseid = itemG.Gcourseid;
                                        GCLRow.gcoursename = itemG.Name;
                                        GCLRow.gownerId = itemG.Gownerid;
                                        GCLRow.teacherfolderid = itemG.Teacherfolderid;
                                        xRow.GCL.Add(GCLRow);
                                    }
                                }
                                res.Add(xRow);
                            }
                        }
                        return Ok(new { result = "OK", data = res });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            result = "error",
                            message = "User doesn't have previleges to access the data"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { result = "error", message = ex.Message });
            }
        }
    }
}
