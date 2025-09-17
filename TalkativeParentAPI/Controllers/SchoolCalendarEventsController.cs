using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;


namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SchoolCalendarEventsController : ControllerBase
    {

        private readonly INotificationService notificationService;
        private readonly ITCalendereventdetailService tCalendereventdetailService;
        private readonly ITClassCalendereventsService tClassCalendereventsService;
        private readonly IMSchoolService mSchoolService;
        private readonly TpContext db = new TpContext();

        public SchoolCalendarEventsController(

            INotificationService _notificationService,
            ITCalendereventdetailService _tCalendereventdetailService,
            ITClassCalendereventsService _tClassCalendereventsService,
            IMSchoolService _mSchoolService
            )
        {
            //iTCalendereventdetailService = _iTCalendereventdetailService;
            notificationService = _notificationService;
            tCalendereventdetailService = _tCalendereventdetailService;
            tClassCalendereventsService = _tClassCalendereventsService;
            mSchoolService = _mSchoolService;
        }

        #region Api for School_PushCalendarNotifications        //Used in beta
        [Route("School_PushCalendarNotifications/School")]
        [HttpPost]

        public IActionResult School_PushCalendarNotifications(PushCalNotiModel model)
        {
            try
            {
                var temp = mSchoolService.GetEntityByID((int)model.Schoolid);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "School doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }else
                {
                    bool noti = tCalendereventdetailService.PushCalendarNotification(model);
                    if (noti)
                    {
                        return Ok("GETTALKTIVE has sent you a calendar update");
                    }
                    else
                    {
                        return Ok("Push notification not sent");
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }


       
        }
        #endregion
        //public List<Devices> GetDeviceid(string Channelname = "", string Parentid = "")
        //{
        //    List<Devices> Device = new List<Devices>();
        //    var dbcontext = new TPDBContext();
        //    ObjectResult<Talkative.API.School.Models.sp_AppUserDevice_Result> objRes = dbcontext.sp_AppUserDevice(Channelname, Parentid);
        //    if (objRes != null)
        //    {
        //        try
        //        {
        //            foreach (Talkative.API.School.Models.sp_AppUserDevice_Result item in objRes)
        //            {
        //                Device.Add(new Devices
        //                {
        //                    Deviceid = item.DeviceId,
        //                    DeviceType = item.DeviceType
        //                });
        //            }
        //        }
        //        catch (ArgumentOutOfRangeException ex)
        //        {
        //            string a = ex.Message;
        //        }
        //    }
        //    return Device;
        //}



        [Route("School_PostCalendarEvents/School")]
        [HttpPost]
        public async Task<IActionResult> School_PostCalendarEvents([FromBody] PostCalendereventdetailsModel model)
        {
            try
            {
                var temp = await this.tCalendereventdetailService.GetEntityIDForUpdate(model.Id);
                var temp2 =  db.TClassCalendereventss.FirstOrDefault(x => x.calendereventid == model.Id);
                if (temp != null)
                {
                    if (!string.IsNullOrEmpty(model.EventTitle))
                        temp.Name = model.EventTitle;
                    if (!string.IsNullOrEmpty(model.EventDescription))
                        temp.Description = model.EventDescription;
                    if (!string.IsNullOrEmpty(model.Venue))
                        temp.Venue = model.Venue;
                    if (model.StartDate.HasValue)
                        temp.Startdate = model.StartDate;
                    if (model.EndDate.HasValue)
                        temp.Enddate = model.EndDate;

                    int? standardIdToUpdate = model.StandardId.FirstOrDefault();
                    int? sectionIdToUpdate = model.SectionId.FirstOrDefault();

                    var scmu = db.MStandardsectionmappings.FirstOrDefault(x => x.Id == sectionIdToUpdate);
                    if (scmu == null)
                    {

                        temp.Standardsectionmappingid = standardIdToUpdate;
                    }
                    else if (scmu.Parentid == standardIdToUpdate)
                    {

                        temp.Standardsectionmappingid = sectionIdToUpdate;
                    }

                    if (model.SchoolId.HasValue)
                        temp.Schoolid = model.SchoolId;
                    if (!string.IsNullOrEmpty(model.Attachment))
                        temp.Attachment = model.Attachment;
                    if (model.Modifiedby.HasValue)
                        temp.Modifiedby = model.Modifiedby;
                    temp.Modifieddate = DateTime.Now;
                    await this.tCalendereventdetailService.UpdateEntity(temp);
                    if (temp2 != null)
                    {
                        foreach (var item in model.SectionId)
                        {
                            temp2.Id = temp2.Id;
                            temp2.sectionId = item;
                            temp2.calendereventid = item;
                            temp2.schoolid = model.SchoolId;
                            await this.tClassCalendereventsService.UpdateEntity(temp2);
                        }
                    }
                    
                    
                }
                else
                {
                    if (model.StartDate < model.EndDate)
                    {
                        List<Object> createdEventIds = new List<Object>();

                        TCalendereventdetail cald = new TCalendereventdetail
                        {
                            Name = model.EventTitle,
                            Description = model.EventDescription,
                            Venue = model.Venue,
                            Startdate = model.StartDate.Value,
                            Enddate = model.EndDate.Value,
                            Schoolid = model.SchoolId,
                            Attachment = model.Attachment,
                            Statusid = 1,
                            Createddate = DateTime.Now,
                            Createdby = model.Createdby,
                            Modifiedby = model.Createdby,
                            Modifieddate = DateTime.Now,
                            //Standardsectionmappingid = standardsectionmappingid
                        };
                        var id = await this.tCalendereventdetailService.AddEntity(cald);

                        foreach (var standardId in model.StandardId)
                        {
                            foreach (var sectionId in model.SectionId)
                            {

                               // var scm = db.MStandardsectionmappings.FirstOrDefault(x => x.Id == sectionId);
                               // int? standardsectionmappingid = null;

                                //if (scm == null)
                                //{

                                //    //standardsectionmappingid = standardId;
                                //}
                                //else if (scm.Parentid == standardId)
                                //{

                                //   // standardsectionmappingid = sectionId;
                                //}
                                //else
                                //{

                                //    continue;
                                //}

                               
                               

                                TClassCalenderevents cald2 = new TClassCalenderevents
                                {
                                    schoolid = model.SchoolId,
                                    calendereventid = id,
                                    sectionId = sectionId
                                };

                                var id2 = await this.tClassCalendereventsService.AddEntity(cald2);
                                createdEventIds.Add(id);
                            }
                           
                        }
                        if (createdEventIds.Count > 0)
                        {
                            return Ok(createdEventIds);
                        }
                        else
                        {
                            return BadRequest("No events were created.");
                        }
                    }
                    else
                    {
                        return BadRequest("End date has to be after start date.");
                    }
                }
                return Ok(model.Id);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("School_GetCalendarEventsBySchoolId/School")]
        [HttpGet]

        public async Task<IActionResult> School_GetCalendarEventsBySchoolId(int Schoolid, int secid, int stdid)
        {
            try
            {
                var temp = await this.tCalendereventdetailService.GetEntityBySchoolIDv2(Schoolid, secid, stdid);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "TCalenderevent doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(temp);

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }

        }

        [Route("School_GetCalendarEventsById/School")]
        [HttpGet]
        public async Task<IActionResult> School_GetCalendarEventsById(int Eventid, int secid, int stdid)
        {
            try
            {
                var temp = await this.tCalendereventdetailService.GetEntityByEventID(Eventid, secid, stdid);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "TCalenderevent doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(temp);

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }

        }


        [Route("School_DeleteCalendarEvents/School")]
        [HttpDelete]
        public async Task<IActionResult> School_DeleteCalendarEvents(int eventid)
        {
            try
            {
                var temp = await this.tCalendereventdetailService.GetEntityIDForUpdate(eventid);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "TCalenderevent doesn't exists with this ID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                await this.tCalendereventdetailService.DeleteEntity(temp);
                return Ok(new { Value = "TCalenderevent successfully deleted." });

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                });
            }
        }


        [Route("School_GetCalendarEvents/School")]
        [HttpGet]
        public IActionResult School_GetCalendarEvents(int Schoolid, DateTime StartDate, DateTime EndDate, string Standardid = "", string Sectionid = "", string searchString = "", int PageSize = 10, int pageNumber = 1)
        {
            try
            {
                var res = tCalendereventdetailService.GetCalendarEventsForAPI(Schoolid, StartDate, EndDate, Standardid, Sectionid, searchString, PageSize, pageNumber);
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
    }
}
