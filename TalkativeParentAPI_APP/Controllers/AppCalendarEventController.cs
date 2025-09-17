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
    //[CustomAuthorization]
    [ApiController]
    public class AppCalendarEventController : ControllerBase
    {
        private static TpContext db = new TpContext();
        private static ITCalendereventdetailService tCalendereventdetail;

        public AppCalendarEventController(ITCalendereventdetailService _tCalendereventdetail)
        {
            tCalendereventdetail = _tCalendereventdetail;
        }

        [Route("api/App_ReadCalendarEventsById/App")]
        [HttpGet]
        public async Task<IActionResult> App_ReadCalendarEventsById(int Eventid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                //List<ReadCalendarEvents> readcalendarevents = new List<ReadCalendarEvents>();
                ReadCalendarEvents readcalendarevents = new ReadCalendarEvents();
                var objRes = await db.TCalendereventdetails.Where(x => x.Id.Equals(Eventid)).FirstOrDefaultAsync();
                if (objRes != null)
                {
                    if ((objRes.Startdate != null) && (objRes.Enddate != null))
                    {
                        readcalendarevents.Id = objRes.Id;
                        readcalendarevents.EventTitle = objRes.Name;
                        readcalendarevents.EventDescription = objRes.Description;
                        readcalendarevents.StartDate = DateTime.Parse(objRes.Startdate.ToString()).ToString("yyyy-MM-ddTHH:mm:ss") + ".000Z";
                        readcalendarevents.EndDate = DateTime.Parse(objRes.Enddate.ToString()).ToString("yyyy-MM-ddTHH:mm:ss") + ".000Z";
                        readcalendarevents.Venue = objRes.Venue;
                        readcalendarevents.Attachment = objRes.Attachment;
                    }
                    return Ok(readcalendarevents);
                }
                else
                {
                    return BadRequest("No Events are Found");
                }
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return BadRequest(a);
            }
        }
    }
}
