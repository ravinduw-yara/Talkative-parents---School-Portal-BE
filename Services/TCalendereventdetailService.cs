using CommonUtility.RequestModels;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.DBContext;
using SendGrid.Helpers.Mail;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static CommonUtility.RequestModels.SoundingboardmessagePublicAndPrivate;

namespace Services
{
    public interface ITCalendereventdetailService : ICommonService
    {
        Task<int> AddEntity(TCalendereventdetail entity);
        Task<TCalendereventdetail> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(TCalendereventdetail entity);
        //Task<List<object>> GetEntityBySchoolID(int Schoolid, int secid, int stdid);

        Task<CalendereventdetailsModel> GetEntityByEventID(int Eventid, int secid, int stdid);

        Task<int> DeleteEntity(TCalendereventdetail entity);

        Task<Object> School_PostCalendarEvents(CalendereventdetailsModel model);

        object GetCalendarEventsForAPI(int Schoolid, DateTime StartDate, DateTime EndDate, string Standardid = "", string Sectionid = "", string searchString = "", int PageSize = 0, int pageNumber = 0);
        bool PushCalendarNotification(PushCalNotiModel model);

        Task<List<CalendereventdetailModel>> GetEntityBySchoolIDv2(int Schoolid, int secid, int stdid);
    }
    public class TCalendereventdetailService : ITCalendereventdetailService
    {
        private readonly IRepository<TCalendereventdetail> repository;
        private readonly IRepository<TClassCalenderevents> repository2;
        private DbSet<TCalendereventdetail> localDBSet;
        private DbSet<TClassCalenderevents> localDBSet2;
        private readonly IMStandardsectionmappingService mStandardsectionmappingService;
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext tpContext;
        private INotificationService notificationService;

        public TCalendereventdetailService(IRepository<TCalendereventdetail> repository, IMStandardsectionmappingService mStandardsectionmappingService, TpContext tpContext, INotificationService notificationService)
        {
            this.repository = repository;
            this.mStandardsectionmappingService = mStandardsectionmappingService;
            this.tpContext = tpContext;
            this.notificationService = notificationService;
        }

        public async Task<int> AddEntity(TCalendereventdetail entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
      

        private async Task AllEntityValue() => localDBSet = (DbSet<TCalendereventdetail>)await this.repository.GetAll();

        private static Object Mapper(TCalendereventdetail x) => new
        {
            x.Id,
            x.Name,
            x.Description,
            x.Venue,
            x.Attachment,
            x.Startdate,
            x.Enddate,
            School = new
            {
                School = x.School != null ? x.School.Name : string.Empty,
                x.Schoolid
            },
            //Standardsectionmapping = new
            //{
            //    Standardsectionmapping = x.Standardsectionmapping != null ? x.Standardsectionmapping.Name : string.Empty,
            //    x.Standardsectionmappingid
            //},
            //Standardsectionmapping = (new CalenderDetailM()
            //{
            //    Section = x.Schoolid,
            //    Standard = x.Standardsectionmapping != null ? x.Standardsectionmapping.Name : string.Empty,
            //}),
            //Standardsectionmapping = new
            //{
            //    Standardsectionmapping = mStandardsectionmappingService.GetEntityBySchoolID2(x.Schoolid),
            //    x.Standardsectionmappingid
            //},
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };

        #region delete later if v2 works
        //private Object Mapper2(TCalendereventdetail x, IQueryable<MStandardsectionmapping> mStandardsectionmappings, int secid, int stdid) => new
        //{
        //    Id = x.Id,
        //    EventTitle = x.Name,
        //    EventDescription = x.Description,
        //    Venue = x.Venue,
        //    Attachment = x.Attachment,
        //    StartDate = x.Startdate,
        //    EndDate = x.Enddate,
        //    CreatedDate = x.Createddate,
        //    ModifiedDate = x.Modifieddate,
        //    SchoolId = x.Schoolid,
        //    StandardId = x.Standardsectionmapping != null ? mStandardsectionmappings.Where(a => a.Id == stdid).Select(a => a.Id).FirstOrDefault() : 0,
        //    SectionId = x.Standardsectionmapping != null ?  mStandardsectionmappings.Where(a => a.Id == secid).Select(a => a.Id).FirstOrDefault() : 0,

        //    //Can be used later
        //    //SchoolName = x.School != null ? x.School.Name : string.Empty,
        //    //Status = x.Status != null ? x.Status.Name : string.Empty,
        //    //Statusid = x.Statusid

        //};

        //private Object Mapper3(TCalendereventdetail x, IQueryable<MStandardsectionmapping> mStandardsectionmappings) => new
        //{
        //    Id = x.Id,
        //    EventTitle = x.Name,
        //    EventDescription = x.Description,
        //    Venue = x.Venue,
        //    Attachment = x.Attachment,
        //    StartDate = x.Startdate,
        //    EndDate = x.Enddate,
        //    CreatedDate = x.Createddate,
        //    ModifiedDate = x.Modifieddate,
        //    SchoolId = x.Schoolid,
        //    StandardId = x.Standardsectionmapping != null ? x.Standardsectionmapping.Id : 0,
        //    SectionId = x.Standardsectionmapping != null ? mStandardsectionmappings.Where(a => a.Parentid == x.Standardsectionmapping.Id).Select(a => a.Id).FirstOrDefault() : 0,

        //    //Can be used later
        //    //SchoolName = x.School != null ? x.School.Name : string.Empty,
        //    //Status = x.Status != null ? x.Status.Name : string.Empty,
        //    //Statusid = x.Statusid

        //};

        //public async Task<List<object>> GetEntityBySchoolID(int Schoolid, int secid, int stdid)
        //{

        //    try
        //    {
        //        List<TCalendereventdetail> allEntites = new List<TCalendereventdetail>();
        //        List<object> response = new List<object>();
        //        var mappingList = await mStandardsectionmappingService.GetAllEntitiesPvt();
        //        if (stdid == 0)
        //        {
        //            allEntites = (await this.GetAllEntitiesPvt()).Where(x => x.Schoolid.Equals(Schoolid)).ToList();
        //            response = allEntites.Select(x => Mapper3(x, mappingList)).ToList();
        //        }
        //        else
        //        {
        //            allEntites = (await this.GetAllEntitiesPvt()).Where(x => x.Schoolid.Equals(Schoolid) && x.Standardsectionmapping.Id.Equals(stdid)).ToList();
        //            response = allEntites.Select(x => Mapper2(x, mappingList, secid, stdid)).ToList();
        //        }
        //        if (allEntites == null || response.Count == 0)
        //        {
        //            return null;
        //        }
        //        return response;
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}

        #endregion


        public async Task<List<CalendereventdetailModel>> GetEntityBySchoolIDv2(int Schoolid, int secid, int stdid)
        {
            List<CalendereventdetailModel> allEntites = new List<CalendereventdetailModel>();
            if (secid == 0 && stdid == 0)
            {
                var res = db.TCalendereventdetails.Where(x => x.Schoolid == Schoolid);
                foreach (var item in res)
                {
                    CalendereventdetailModel calmod = new CalendereventdetailModel();
                    calmod.Id = item.Id;
                    calmod.EventTitle = item.Name;
                    calmod.EventDescription = item.Description;
                    calmod.Venue = item.Venue;
                    calmod.SchoolId = item.Schoolid;
                    calmod.StartDate = item.Startdate;
                    calmod.EndDate = item.Enddate;
                    calmod.CreatedDate = item.Createddate;
                    calmod.ModifiedDate = item.Modifieddate;

                    if (item.Standardsectionmappingid != null)
                    {
                        var ssm = await db2.MStandardsectionmappings.Where(x => x.Id == item.Standardsectionmappingid).FirstOrDefaultAsync();

                        if (ssm.Parentid == null)
                        {
                            calmod.StandardId = item.Standardsectionmappingid;
                            calmod.SectionId = 0;
                        }
                        else
                        {
                            calmod.StandardId = ssm.Parentid;
                            calmod.SectionId = item.Standardsectionmappingid;
                        }
                    }
                    else
                    {
                        calmod.StandardId = 0;
                        calmod.SectionId = 0;
                    }
                    allEntites.Add(calmod);
                }
            }
            else if(stdid != 0 && secid == 0)
            {
                var res = db.TCalendereventdetails.Where(x => x.Schoolid == Schoolid && x.Standardsectionmappingid == stdid);
                foreach (var item in res)
                {
                    CalendereventdetailModel calmod = new CalendereventdetailModel();
                    calmod.Id = item.Id;
                    calmod.EventTitle = item.Name;
                    calmod.EventDescription = item.Description;
                    calmod.Venue = item.Venue;
                    calmod.SchoolId = item.Schoolid;
                    calmod.StartDate = item.Startdate;
                    calmod.EndDate = item.Enddate;
                    calmod.CreatedDate = item.Createddate;
                    calmod.ModifiedDate = item.Modifieddate;
                    calmod.StandardId = item.Standardsectionmappingid;
                    calmod.SectionId = 0;
                    allEntites.Add(calmod);
                }
            }
            else
            {
                var res = db.TCalendereventdetails.Where(x => x.Schoolid == Schoolid && x.Standardsectionmappingid == secid);
                foreach (var item in res)
                {
                    CalendereventdetailModel calmod = new CalendereventdetailModel();
                    calmod.Id = item.Id;
                    calmod.EventTitle = item.Name;
                    calmod.EventDescription = item.Description;
                    calmod.Venue = item.Venue;
                    calmod.SchoolId = item.Schoolid;
                    calmod.StartDate = item.Startdate;
                    calmod.EndDate = item.Enddate;
                    calmod.CreatedDate = item.Createddate;
                    calmod.ModifiedDate = item.Modifieddate;
                    calmod.StandardId = stdid;
                    calmod.SectionId = secid;
                    allEntites.Add(calmod);
                }
            }
            return allEntites;
        }

        private async Task<IQueryable<TCalendereventdetail>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status)
            .Include(x => x.Standardsectionmapping)
            .Include(x => x.School);
        }


        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<TCalendereventdetail> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) =>
            (await this.GetAllEntitiesPvt())
            .Where(x => x.Id.Equals(entityID))
            .Select(x => Mapper(x))
            .SingleOrDefault();

        public async Task<Object> School_PostCalendarEvents(CalendereventdetailsModel model)
        {
            try
            {
                var temp = await this.GetEntityIDForUpdate(model.Id);
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

                    var scmu = db.MStandardsectionmappings.Where(x => x.Id.Equals(model.SectionId)).FirstOrDefault();
                    if (scmu is null)
                    {
                        temp.Standardsectionmappingid = model.StandardId;
                    }
                    else if (scmu.Parentid.Equals(model.StandardId))
                    {
                        temp.Standardsectionmappingid = model.SectionId;
                    }

                    if (model.SchoolId.HasValue)
                        temp.Schoolid = model.SchoolId;
                    if (!string.IsNullOrEmpty(model.Attachment))
                        temp.Attachment = model.Attachment;
                    if (model.Modifiedby.HasValue)
                        temp.Modifiedby = model.Modifiedby;
                    temp.Modifieddate = DateTime.Now;
                    await this.UpdateEntity(temp);
                }
                else
                {
                    if (model.StartDate < model.EndDate)
                    {
                        TCalendereventdetail cald = new TCalendereventdetail();

                        cald.Name = model.EventTitle;
                        cald.Description = model.EventDescription;
                        cald.Venue = model.Venue;
                        cald.Startdate = model.StartDate;
                        cald.Enddate = model.EndDate;
                        cald.Schoolid = model.SchoolId;

                        //checking for standard or section since we have a combined field
                        var scm = db.MStandardsectionmappings.Where(x => x.Id.Equals(model.SectionId)).FirstOrDefault();
                        if (scm is null)
                        {
                            cald.Standardsectionmappingid = model.StandardId;
                        }
                        else if (scm.Parentid.Equals(model.StandardId))
                        {
                            cald.Standardsectionmappingid = model.SectionId;
                        }

                        cald.Attachment = model.Attachment;
                        cald.Statusid = 1;
                        cald.Createddate = DateTime.Now;
                        cald.Createdby = model.Createdby;
                        cald.Modifiedby = model.Createdby;
                        cald.Modifieddate = DateTime.Now;

                        var id = await this.AddEntity(cald);
                        return id;
                    }
                    else
                    {
                        return (new
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Msg = "End date has to be after start date."
                        });
                    }
                }
                return model.Id;

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


        public async Task<CalendereventdetailsModel> GetEntityByEventID(int Eventid, int secid, int stdid)
        {

            try
            {

                var res = await this.db.TCalendereventdetails.Where(x => x.Id.Equals(Eventid)).FirstOrDefaultAsync();

                CalendereventdetailsModel cald = new CalendereventdetailsModel();

                cald.EventTitle = res.Name;
                cald.EventDescription = res.Description;
                cald.Venue = res.Venue;
                cald.StartDate = res.Startdate;
                cald.EndDate = res.Enddate;
                cald.SchoolId = res.Schoolid;

                //checking for standard or section since we have a combined field
                var scm = db.MStandardsectionmappings.Where(x => x.Id.Equals(res.Standardsectionmappingid)).FirstOrDefault();

                if(scm is null)
                {
                    cald.StandardId = null;
                    cald.SectionId = null;
                }
                else if(scm.Parentid != null)
                {
                    cald.StandardId = scm.Parentid;
                    cald.SectionId = scm.Id;
                }
                else
                {
                    cald.StandardId = res.Standardsectionmappingid;
                    cald.SectionId = null;
                }

                cald.Attachment = res.Attachment;
                //cald.Statusid = res.Statusid;
                cald.CreatedDate = DateTime.UtcNow;
                cald.Createdby = res.Createdby;
                cald.Modifiedby = res.Createdby;
                cald.ModifiedDate = DateTime.UtcNow;

                var sectionsleist = db2.TClassCalendereventss.Where(x => x.calendereventid == Eventid).Select(y => y.sectionId).ToList();
                foreach (var item2 in sectionsleist)
                {
                    CalenderSectionIdList sectionIdItem = new CalenderSectionIdList
                    {
                        sectionid = item2
                    };

                    cald.sectionidlist.Add(sectionIdItem);

                }
                return cald;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

        }

        #region for School_GetCalendarEvents

        private IQueryable<TCalendereventdetail> GetCalendarEvents(int Schoolid, DateTime StartDate, DateTime EndDate, string Standardid, string Sectionid , string searchString)
        {
            try
            {
                if (Schoolid != 0)
                {
                    var res = db.TCalendereventdetails.Where(x => (x.Schoolid == Schoolid) && ((x.Name.Contains(searchString) || x.Description.Contains(searchString) || x.Venue.Contains(searchString))) && (((x.Startdate >= StartDate) && (x.Startdate <= EndDate)) || ((x.Enddate >= StartDate) && x.Enddate <= EndDate)));
  
                    return res.OrderByDescending(x => x.Createddate);
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public object GetCalendarEventsForAPI(int Schoolid, DateTime StartDate, DateTime EndDate, string Standardid = "", string Sectionid = "", string searchString = "", int PageSize = 0, int pageNumber = 0)
        {
            // new Date String added For Calendar results
            DateTime sDate = Convert.ToDateTime(StartDate);
            sDate = sDate.AddDays(-1);
            DateTime eDate = Convert.ToDateTime(EndDate);
            eDate = eDate.AddMilliseconds(-1);

            var selectedStandards = (null != Standardid) ? Standardid.Split(',') : null;
            var selectedSections = (null != Sectionid) ? Sectionid.Split(',') : null;
            List<GetCalendarEvents> getcalenderevents = new List<GetCalendarEvents>();
            if (Standardid == "" && Sectionid == "")
            {
                //searchString = null;
                Standardid = null;
                Sectionid = null;
            }
            try
            {
                foreach (var std in selectedStandards)
                {
                    foreach (var sec in selectedSections)
                    {
                        if (std == "" && sec == "")
                        {
                            var objRes = GetCalendarEvents(Schoolid, sDate, eDate, Standardid, Sectionid, searchString);
                            if (objRes != null)
                            {
                                foreach (var item in objRes)
                                {
                                    if ((StartDate == EndDate) && (StartDate == null) && (EndDate == null))
                                    {
                                        return ("Invalid Date");
                                    }

                                    if ((item.Startdate != null) && (item.Enddate != null) && (item.Enddate >= item.Startdate))
                                    {
                                        GetCalendarEvents calendarEvent = new GetCalendarEvents
                                        {
                                            Id = item.Id,
                                            EventTitle = item.Name,
                                            EventDescription = item.Description,
                                            StartDate = item.Startdate,
                                            EndDate = item.Enddate,
                                            SchoolId = item.Schoolid,
                                            Venue = item.Venue,
                                            Attachment = item.Attachment,
                                            CreatedDate = item.Createddate,
                                            ModifiedDate = item.Modifieddate
                                        };
                                        //getcalenderevents.Add(new GetCalendarEvents
                                        //{
                                        //    Id = item.Id,
                                        //    EventTitle = item.Name,
                                        //    EventDescription = item.Description,
                                        //    StartDate = item.Startdate,
                                        //    EndDate = item.Enddate,
                                        //    //StandardId = (int)item.Standardsectionmapping.Id,
                                        //    //SectionId = (int)item.Standardsectionmappingid,
                                        //    SchoolId = item.Schoolid,
                                        //    Venue = item.Venue,
                                        //    Attachment = item.Attachment,
                                        //    CreatedDate = item.Createddate,
                                        //    ModifiedDate = item.Modifieddate
                                        //});
                                        var sectionsleist = db2.TClassCalendereventss.Where(x => x.calendereventid == item.Id).Select(y => y.sectionId).ToList();
                                        foreach (var item2 in sectionsleist)
                                        {
                                            CalenderSectionIdList sectionIdItem = new CalenderSectionIdList
                                            {
                                                sectionid = item2
                                            };

                                            calendarEvent.sectionidlist.Add(sectionIdItem);

                                        }
                                        getcalenderevents.Add(calendarEvent);
                                    }
                                }
                                int count1 = getcalenderevents.Count();
                                // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
                                int CurrentPage1 = pageNumber;
                                // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
                                // int PageSize = nuofRows;
                                // Display TotalCount to Records to User  
                                int TotalCount1 = count1;
                                // Calculating Totalpage by Dividing (No of Records / Pagesize)  
                                int TotalPages1 = (int)Math.Ceiling(count1 / (double)PageSize);
                                // Returns List of Customer after applying Paging   
                                var items1 = getcalenderevents.Skip((CurrentPage1 - 1) * PageSize).Take(PageSize).ToList();
                                // if CurrentPage is greater than 1 means it has previousPage  
                                var previousPage1 = CurrentPage1 > 1 ? "Yes" : "No";
                                // if TotalPages is greater than CurrentPage means it has nextPage  
                                var nextPage1 = CurrentPage1 < TotalPages1 ? "Yes" : "No";

                                var obj1 = new
                                {
                                    TotalPages = TotalPages1,
                                    items = items1
                                };
                                return obj1;
                            }
                        }
                        else
                        {
                            var objRes1 = GetCalendarEvents(Schoolid, sDate, eDate, Standardid, Sectionid, searchString);
                            if (objRes1 != null)
                            {
                                foreach (var item in objRes1)
                                {
                                    if ((StartDate == EndDate) && (StartDate == null) && (EndDate == null))
                                    {
                                        return ("Invalid Date");
                                    }
                                    if ((item.Startdate != null) && (item.Enddate != null) && (item.Enddate >= item.Startdate))
                                    {
                                        getcalenderevents.Add(new GetCalendarEvents
                                        {
                                            Id = item.Id,
                                            EventTitle = item.Name,
                                            EventDescription = item.Description,
                                            StartDate = item.Startdate,
                                            EndDate = item.Enddate,
                                            //StandardId = item.Standardsectionmapping.Id,
                                            //SectionId = item.Sectionid,
                                            SchoolId = item.Schoolid,
                                            Venue = item.Venue,
                                            Attachment = item.Attachment,
                                            CreatedDate = item.Createddate,
                                            ModifiedDate = item.Modifieddate
                                        });
                                    }
                                }

                            }
                        }
                    }
                    int count = getcalenderevents.Count();
                    int CurrentPage = pageNumber;
                    // int PageSize = nuofRows;
                    int TotalCount = count;
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                    var items = getcalenderevents.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                    var previousPage = CurrentPage > 1 ? "Yes" : "No";
                    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                    var obj = new
                    {
                        TotalPages = TotalPages,
                        items = items
                    };
                    return obj;
                }
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
            return ("Success");
        }

        #endregion


        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(TCalendereventdetail entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(TCalendereventdetail entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public bool PushCalendarNotification(PushCalNotiModel model)
        {
            CommonUtility.Config.ResetAll();
            DbSet<MChildschoolmapping> oChildSchool = null;
            DbSet<MParentchildmapping> oParentChildMappings = null;
            var output = "";

            var data1 = tpContext.MSchools.Where(x => x.Id == model.Schoolid).FirstOrDefault();
            model.EventDescription = model.EventDescription.Length >= 160 ? model.EventDescription.Substring(0, 160) : model.EventDescription;

            if ((model.StandardId == null || model.StandardId.Length == 0) && (model.SectionId == null || model.SectionId.Length == 0))
            {
                if (oChildSchool == null)
                    oChildSchool = tpContext.MChildschoolmappings;

                var childSchool = (from csm in tpContext.MChildschoolmappings
                                   join ssm in tpContext.MStandardsectionmappings
                                   on csm.Standardsectionmappingid equals ssm.Id
                                   join br in tpContext.MBranches
                                   on ssm.Branchid equals br.Id
                                   join sch in tpContext.MSchools
                                   on br.Schoolid equals sch.Id
                                   join ay in db.MAcademicyeardetails
                                   on sch.Id equals ay.SchoolId
                                   where sch.Id == model.Schoolid && ay.Currentyear == 1 && ay.SchoolId == model.Schoolid && csm.Statusid == 1 && csm.AcademicYearId == ay.Id
                                   select new
                                   {
                                       Childid = csm.Childid,
                                       Schoolid = sch.Id,
                                       SectionId = ssm.Id,
                                       StdId = ssm.Parentid,
                                   }).ToList();

                foreach (var child in childSchool)
                {
                    if (oParentChildMappings == null)
                        oParentChildMappings = tpContext.MParentchildmappings;

                    var parentChild = oParentChildMappings.Where(w => w.Childid == child.Childid).FirstOrDefault();

                    if (parentChild != null)
                    {
                        var childgender = tpContext.MChildinfos.Where(w => w.Id == parentChild.Childid).FirstOrDefault().Genderid;
                        var setofferoptn = tpContext.MAppuserinfos.Where(w => w.Id == parentChild.Appuserid).FirstOrDefault();

                        if (setofferoptn != null)
                        {
                            if (setofferoptn.Isofferoptedin == true)
                            {
                                var deviceid = tpContext.Appuserdevices.Where(x => x.Appuserid == parentChild.Appuserid).ToList();
                                string mask = db2.MFeatures.Where(a => a.Schoolid == model.Schoolid).Select(a => a.Mask).FirstOrDefault();
                                if (deviceid.Count > 0)
                                {
                                    foreach (var devices in deviceid)
                                    {
                                        //output = notificationService.PushCalNoti(model.EventTitle, "2", CommonUtility.Config.osAppId, CommonUtility.Config.osAuth, string.Join("\", \"", devices.Deviceid), (int)devices.Devicetype, child.Childid, child.Schoolid, child.SectionId, child.StdId, childgender, model.EventTitle, model.Id, CommonUtility.Config.mask);

                                        output = notificationService.PushCalNoti(model.EventTitle, "2", CommonUtility.Config.osAppId, CommonUtility.Config.osAuth, string.Join("\", \"", devices.Deviceid), (int)devices.Devicetype, child.Childid, child.Schoolid, child.SectionId, child.StdId, childgender, model.EventTitle, model.Id, mask);
                                    }

                                }
                            }
                        }
                    }
                }
            }

            else if (model.SectionId == null || model.SectionId.Length == 0)
            {

                foreach (var standardId in model.StandardId)
                {
                    if (oChildSchool == null)
                        oChildSchool = tpContext.MChildschoolmappings;


                    var stdsecmps = tpContext.MStandardsectionmappings.Where(x => x.Parentid == standardId).ToList();

                    foreach (var stdsecmp in stdsecmps)
                    {

                        var childrenMappings = oChildSchool.Where(w => w.Standardsectionmappingid == stdsecmp.Id).ToList();

                        if (oParentChildMappings == null)
                            oParentChildMappings = tpContext.MParentchildmappings;

                        foreach (var childMapping in childrenMappings)
                        {
                            var parentChildMapping = oParentChildMappings.FirstOrDefault(w => w.Childid == childMapping.Childid);
                            if (parentChildMapping != null)
                            {
                                var childGender = tpContext.MChildinfos.FirstOrDefault(w => w.Id == parentChildMapping.Childid)?.Genderid;
                                var setOfferOptIn = tpContext.MAppuserinfos.FirstOrDefault(w => w.Id == parentChildMapping.Appuserid)?.Isofferoptedin ?? false;

                                if (setOfferOptIn)
                                {
                                    var devices = tpContext.Appuserdevices.Where(x => x.Appuserid == parentChildMapping.Appuserid).ToList();
                                    string mask = db2.MFeatures.Where(a => a.Schoolid == model.Schoolid).Select(a => a.Mask).FirstOrDefault();
                                    foreach (var device in devices)
                                    {
                                        output = notificationService.PushCalNoti(model.EventTitle, "2", CommonUtility.Config.osAppId, CommonUtility.Config.osAuth,
                                            string.Join("\", \"", device.Deviceid), (int)device.Devicetype, parentChildMapping.Childid,
                                            (int)model.Schoolid, stdsecmp.Id, stdsecmp.Parentid.Value, childGender, model.EventTitle, model.Id, mask);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var standardId in model.StandardId)
                {
                    foreach (var sectionId in model.SectionId)
                    {

                        var childSchoolMappings = from csm in tpContext.MChildschoolmappings
                                                  join ssm in tpContext.MStandardsectionmappings on csm.Standardsectionmappingid equals ssm.Id
                                                  join br in tpContext.MBranches on ssm.Branchid equals br.Id
                                                  join sch in tpContext.MSchools on br.Schoolid equals sch.Id
                                                  join ay in tpContext.MAcademicyeardetails on sch.Id equals ay.SchoolId
                                                  where sch.Id == model.Schoolid && (standardId == 0 || ssm.Parentid == standardId) && (sectionId == 0 || ssm.Id == sectionId)
                                                  && ay.Id == csm.AcademicYearId && csm.Statusid == 1 && ay.Currentyear == 1
                                                  select new
                                                  {
                                                      csm.Childid,
                                                      Schoolid = sch.Id,
                                                      SectionId = ssm.Id,
                                                      StdId = ssm.Parentid,
                                                  };

                        foreach (var child in childSchoolMappings.ToList())
                        {
                            var parentChild = tpContext.MParentchildmappings.Where(w => w.Childid == child.Childid).ToList();
                            //var parentChild = tpContext.MParentchildmappings.FirstOrDefault(w => w.Childid == child.Childid);
                            if (parentChild != null)
                            {
                                var appUserIds = parentChild.Select(pc => pc.Appuserid).ToList();
                                var childgender = tpContext.MChildinfos.FirstOrDefault(w => w.Id == child.Childid)?.Genderid;
                                var setofferoptn = tpContext.MAppuserinfos.FirstOrDefault(w => appUserIds.Contains(w.Id))?.Isofferoptedin;

                                // var childgender = tpContext.MChildinfos.FirstOrDefault(w => w.Id == parentChild.Childid)?.Genderid;
                                //var setofferoptn = tpContext.MAppuserinfos.FirstOrDefault(w => w.Id == parentChild.Appuserid)?.Isofferoptedin;

                                if (setofferoptn == true)
                                {
                                    var deviceids = tpContext.Appuserdevices.Where(w => appUserIds.Contains(w.Appuserid)).ToList();
                                    //  var deviceids = tpContext.Appuserdevices.Where(x => x.Appuserid == parentChild.Appuserid).ToList();
                                    string mask = db2.MFeatures.Where(a => a.Schoolid == model.Schoolid).Select(a => a.Mask).FirstOrDefault();
                                    foreach (var device in deviceids)
                                    {
                                        output = notificationService.PushCalNoti(model.EventTitle, "2", CommonUtility.Config.osAppId, CommonUtility.Config.osAuth, string.Join("\", \"", device.Deviceid), (int)device.Devicetype, child.Childid, child.Schoolid, child.SectionId, child.StdId, childgender, model.EventTitle, model.Id, mask);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (output == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

