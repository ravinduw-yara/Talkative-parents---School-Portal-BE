using CommonUtility.RequestModels;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;
using Repository;
using Repository.DBContext;
using SendGrid.Helpers.Mail;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface ITNoticeboardmessageService : ICommonService
    {
        Task<int> AddEntity(TNoticeboardmessage entity);
        Task<TNoticeboardmessage> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(TNoticeboardmessage entity);
        Task<int> NBConditionCheck(NBMModel nbmModel);
        Task<int> DeleteEntity(TNoticeboardmessage entity);
        object GetNoticeBoardMsgForAPI(int SchoolId, int ChildId, int PageSize = 10, int pageNumber = 1);

        //void SENServices(NBMModel nbmModel);

    }
    public class TNoticeboardmessageService : ITNoticeboardmessageService
    {
        private readonly IRepository<TNoticeboardmessage> repository;

        private DbSet<TNoticeboardmessage> localDBSet;
        private IEmailService emailService;
        private TNoticeboardmappingService tNoticeboardmappingService;
        private INotificationService notificationService;
        private ITEmaillogService emaillogService;
        private IMSchoolService mSchoolService;

        private readonly TpContext db;
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext db3 = new TpContext();
        int NoticeBoardID = 0;

        public TNoticeboardmessageService(IRepository<TNoticeboardmessage> repository, TpContext db, IEmailService emailService, ITNoticeboardmappingService tNoticeboardmappingService, INotificationService notificationService, ITEmaillogService emaillogService, IMSchoolService mSchoolService)
        {
            this.repository = repository;
            this.db = db;
            this.emailService = emailService;
            this.tNoticeboardmappingService = (TNoticeboardmappingService)tNoticeboardmappingService;
            this.notificationService = notificationService;
            this.emaillogService = emaillogService;
            this.mSchoolService = mSchoolService;
        }

        public async Task<int> AddEntity(TNoticeboardmessage entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<TNoticeboardmessage>)await this.repository.GetAll();

        private static Object Mapper(TNoticeboardmessage x) => new
        {
            x.Id,
            x.Subject,
            x.Attachments,
            x.Sms,
            x.Isemail,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };

        private async Task<IQueryable<TNoticeboardmessage>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<TNoticeboardmessage> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<int> UpdateEntity(TNoticeboardmessage entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(TNoticeboardmessage entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }


        public void SENServices(NBMModel nbmModel, List<TNoticeboardmessage> messageid)
        {

                SendSMSEmail(nbmModel);

                SendNotification(nbmModel, messageid);

        }
        public void SendNotification(NBMModel nbmModel, List<TNoticeboardmessage> messageid)
        {
            string strOneSignalResponse = "";
            TpContext db = new TpContext();

            DbSet<MChildschoolmapping> oChildSchool = null;
            DbSet<MParentchildmapping> oParentChildMappings = null;

            var parentsNE = (from aui in db.MAppuserinfos
                           join pcm in db.MParentchildmappings
                           on aui.Id equals pcm.Appuserid
                           join csm in db.MChildschoolmappings
                           on pcm.Childid equals csm.Childid
                           join ssm in db.MStandardsectionmappings
                           on csm.Standardsectionmappingid equals ssm.Id
                           join br in db.MBranches
                           on ssm.Branchid equals br.Id
                           join sch in db.MSchools
                           on br.Schoolid equals sch.Id
                           join ay in db.MAcademicyeardetails
                           on sch.Id equals ay.SchoolId
                           where sch.Id == nbmModel.SchoolId && ay.Currentyear == 1 && ay.SchoolId == nbmModel.SchoolId && csm.Statusid == 1 && csm.AcademicYearId == ay.Id 

                             select new
                           {
                               csm.Childid,
                               SectionId = ssm.Id,
                               StdId = ssm.Parentid,
                               ParentId = pcm.Appuserid,
                               SchoolId = sch.Id,
                               StandardId = ssm.Parentid,
                               ChildGenderId = aui.Genderid //ParentGender
                           }).ToList();

            if ((bool)nbmModel.IsEntireSchool)
            {
                foreach (var child in parentsNE)
                {
                    if (oParentChildMappings == null)
                        oParentChildMappings = db.MParentchildmappings;

                    var parentChild = oParentChildMappings.Where(w => w.Childid == child.Childid && w.Appuserid == child.ParentId).FirstOrDefault();

                    if (parentChild != null)
                    {
                        var childgender = db.MChildinfos.Where(w => w.Id == parentChild.Childid).FirstOrDefault().Genderid;
                        var setofferoptn = db.MAppuserinfos.Where(w => w.Id == parentChild.Appuserid).FirstOrDefault();

                        if (setofferoptn != null)
                        {
                            if (setofferoptn.Isofferoptedin == true)  // Confirm
                            {


                                var deviceid = db.Appuserdevices.Where(x => x.Appuserid == parentChild.Appuserid).ToList();
                                //GetDeviceid(string.Empty, parentChild.Appuserid.ToString());
                                //PushNotification(deviceid, (int)parentChild.Childid, nbmModel, child.SectionId, (int)child.StandardId, (int)childgender);

                                var output = "";

                                string mask = db2.MFeatures.Where(a => a.Schoolid == nbmModel.SchoolId).Select(a => a.Mask).FirstOrDefault();
                                bool isErrorFound = false;

                                if (deviceid.Count > 0)
                                {
                                    foreach (var notidata in deviceid)
                                    {
                                        foreach (var msgid in messageid)
                                        {
                                            if(msgid.Id != 0)
                                            {
                                                output = notificationService.PushNotification(nbmModel.Message, "2", CommonUtility.Config.osAppId, CommonUtility.Config.osAuth, string.Join("\", \"", notidata.Deviceid), notidata.Devicetype, parentChild.Childid, (int)nbmModel.SchoolId, child.SectionId, child.StandardId, childgender, nbmModel.Subject, msgid.Id, mask);

                                            }
                                        }     
                                    }
                                }
                                if (output == "NULL")
                                {
                                    isErrorFound = true;
                                }
                                else
                                {
                                    // Might be storing Push Notifcation for further investigation if any issues.
                                    //strOneSignalResponse = ((strOneSignalResponse == "") ? "" : strOneSignalResponse + ",") + output;
                                    if (strOneSignalResponse == "")
                                    {
                                        strOneSignalResponse = output;
                                    }
                                    else if (strOneSignalResponse != "" && output != "")
                                    {
                                        strOneSignalResponse = strOneSignalResponse + ',' + output;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (nbmModel.IsParticularClass == true && nbmModel.IsParticularSection == true && nbmModel.IsParticularParent == false)
            {
                foreach (var inner in nbmModel.Classes)
                {
                    foreach (var item in inner.sectionModel)
                    {
                        //Get Parents 
                        var innerParentList = (from aui in db.MAppuserinfos
                                               join pcm in db.MParentchildmappings
                                               on aui.Id equals pcm.Appuserid
                                               join csm in db.MChildschoolmappings
                                               on pcm.Childid equals csm.Childid
                                               join ssm in db.MStandardsectionmappings
                                               on csm.Standardsectionmappingid equals ssm.Id
                                               join br in db.MBranches
                                               on ssm.Branchid equals br.Id
                                               join sch in db.MSchools
                                               on br.Schoolid equals sch.Id
                                               join ay in db.MAcademicyeardetails
                                               on sch.Id equals ay.SchoolId
                                               where sch.Id == nbmModel.SchoolId && ssm.Parentid == inner.StdId && ssm.Id == item.SectionId && csm.AcademicYearId == ay.Id && ay.Currentyear == 1 && ay.SchoolId == nbmModel.SchoolId && csm.Statusid ==1
                                               select new
                                               {
                                                   ParentId = pcm.Appuserid,
                                                   ChildId = csm.Childid,
                                                   SchoolId = sch.Id,
                                                   StandardId = ssm.Parentid,
                                                   SectionId = ssm.Id,
                                                   ChildGenderId = aui.Genderid //ParentGender
                                               }).ToList();

                        //parentsList.AddRange(innerParentList);

                        foreach (var parchild in innerParentList)
                        {
                            var childgender = db.MChildinfos.Where(w => w.Id == parchild.ChildId).FirstOrDefault().Genderid;
                            var setofferoptn = db.MAppuserinfos.Where(w => w.Id == parchild.ParentId).FirstOrDefault();

                            if (setofferoptn != null)
                            {
                                //To select parent whose notification is ON
                                if (setofferoptn.Isofferoptedin == true)
                                {
                                    
                                    //List<Devices> deviceidv2 = this.GetDeviceid(string.Empty, parchild.ParentId.ToString());
                                    var deviceid = db.Appuserdevices.Where(x => x.Appuserid == parchild.ParentId).ToList(); 
                                    //PushNotification(deviceid, (int)parchild.ChildId, nbmModel, parchild.SectionId, (int)parchild.StandardId, (int)childgender);

                                    var output = "";

                                    string mask = db2.MFeatures.Where(a => a.Schoolid == nbmModel.SchoolId).Select(a => a.Mask).FirstOrDefault();
                                    bool isErrorFound = false;

                                    if (deviceid.Count > 0)
                                    {
                                        foreach (var notidata in deviceid)
                                        {
                                            foreach (var msgid in messageid)
                                            {
                                                if(msgid.Standardsectionmappingid == parchild.SectionId && msgid.Id != 0)
                                                {
                                                    output = notificationService.PushNotification(nbmModel.Message, "2", CommonUtility.Config.osAppId, CommonUtility.Config.osAuth, string.Join("\", \"", notidata.Deviceid), notidata.Devicetype, parchild.ChildId, (int)nbmModel.SchoolId, item.SectionId, inner.StdId, childgender, nbmModel.Subject, msgid.Id, mask);
                                                }
                                            }
                                        }
                                    }
                                    if (output == "NULL")
                                    {
                                        isErrorFound = true;
                                    }
                                    else
                                    {
                                        // Might be storing Push Notifcation for further investigation if any issues.
                                        if (strOneSignalResponse == "")
                                        {
                                            strOneSignalResponse = output;
                                        }
                                        else if (strOneSignalResponse != "" && output != "")
                                        {
                                            strOneSignalResponse = strOneSignalResponse + ',' + output;
                                        }
                                        //strOneSignalResponse = ((strOneSignalResponse == "") ? "" : strOneSignalResponse + ",") + output;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            else if ((bool)nbmModel.IsParticularParent)
            {
                foreach (var inner in nbmModel.Classes)
                {
                    foreach (var item in inner.sectionModel)
                    {
                        foreach (var parchild in item.Parents)
                        {
                            var childgender = db.MChildinfos.Where(w => w.Id == parchild.ChildId).FirstOrDefault().Genderid;
                            var setofferoptn = db.MAppuserinfos.Where(w => w.Id == parchild.Id).FirstOrDefault();

                            if (setofferoptn != null)
                            {
                                if (setofferoptn.Isofferoptedin == true)
                                {
                                    //List<Devices> deviceidv2 = this.GetDeviceid(string.Empty, parchild.Id.ToString());
                                    // Get all parent-child mappings for the given child ID // new addition 31/5/2024 sanduni
                                    var parentChildMappings = db.MParentchildmappings
                                                                .Where(x => x.Childid == parchild.ChildId)
                                                                .ToList();

                                    // Extract parent IDs from the mappings
                                    var parentIds = parentChildMappings.Select(x => x.Appuserid).ToList();

                                    // Get all devices for these parent IDs
                                    var deviceid = db.Appuserdevices
                                                    .Where(x => parentIds.Contains(x.Appuserid))
                                                    .ToList();

                                   // var deviceid = db.Appuserdevices.Where(x => x.Appuserid == parentsids).ToList(); //old 31/5/2024 sanduni
                                    //GetDeviceid(string.Empty, parentChild.Appuserid.ToString());
                                    //PushNotification(deviceid, parchild.ChildId, nbmModel, item.SectionId, inner.StdId, (int)childgender);

                                    var output = "";

                                    string mask = db2.MFeatures.Where(a => a.Schoolid == nbmModel.SchoolId).Select(a=> a.Mask).FirstOrDefault();
                                    bool isErrorFound = false;

                                    if (deviceid.Count > 0)
                                    {
                                        foreach (var notidata in deviceid)
                                        {
                                            foreach (var msgid in messageid)
                                            {
                                                if (msgid.Standardsectionmappingid == item.SectionId && msgid.Id != 0)
                                                {
                                                    output = notificationService.PushNotification(nbmModel.Message, "2", CommonUtility.Config.osAppId, CommonUtility.Config.osAuth, string.Join("\", \"", notidata.Deviceid), notidata.Devicetype, parchild.ChildId, (int)nbmModel.SchoolId, item.SectionId, inner.StdId, childgender, nbmModel.Subject, msgid.Id, mask);
                                                }
                                            }
                                        }
                                    }
                                    if (output == "NULL")
                                    {
                                        isErrorFound = true;
                                    }
                                    else
                                    {
                                        // Might be storing Push Notifcation for further investigation if any issues.
                                        if (strOneSignalResponse == "")
                                        {
                                            strOneSignalResponse = output;
                                        }
                                        else if (strOneSignalResponse != "" && output != "")
                                        {
                                            strOneSignalResponse = strOneSignalResponse + ',' + output;
                                        }
                                        //strOneSignalResponse = ((strOneSignalResponse == "") ? "" : strOneSignalResponse + ",") + output;
                                    }
                                }
                            }
                        }
                    }

                }
            }
            
            if (strOneSignalResponse != null)
            {
                List<TNoticeboardmessage> upd = new List<TNoticeboardmessage>();
                foreach (var msgid in messageid)
                {
                    var msgidData = db2.TNoticeboardmessages.Where(x => x.Id.Equals(msgid.Id)).FirstOrDefault();
                    upd.Add(new TNoticeboardmessage
                    {
                        Id = msgidData.Id,
                        Schooluserid = msgidData.Schooluserid,
                        Branchid = msgidData.Branchid,
                        Standardsectionmappingid = msgidData.Standardsectionmappingid,
                        Subject = msgidData.Subject,
                        Message = msgidData.Message,
                        Attachments = msgidData.Attachments,
                        Sms = msgidData.Sms,
                        Ispriority = msgidData.Ispriority,
                        Isemail = msgidData.Isemail,
                        IsEntireSchool = msgidData.IsEntireSchool,
                        IsParticularClass = msgidData.IsParticularClass,
                        IsParticularSection = msgidData.IsParticularSection,
                        IsParticularParent = msgidData.IsParticularParent,
                        Createddate = msgidData.Createddate,
                        Modifieddate = msgidData.Modifieddate,
                        Createdby = msgidData.Createdby,
                        Modifiedby = msgidData.Modifiedby,
                        Statusid = msgidData.Statusid,
                        OneSignal = strOneSignalResponse
                    }) ;
                    
                }
                if (upd != null)
                {
                    //upd.OneSignal = strOneSignalResponse;
                    db.TNoticeboardmessages.UpdateRange(upd);
                    db.SaveChanges();
                }
            }
        }

        #region remove if not used
        private void PushNotification(List<Appuserdevice> deviceid, int parentChildid, NBMModel nbmModel, int sectionId, int standardId, int childgender)
        {
            var output = "";
            string strOneSignalResponse = "";

            string mask = db2.MFeatures.Where(a => a.Schoolid == nbmModel.SchoolId).FirstOrDefault().Mask;
            bool isErrorFound = false;

            if (deviceid.Count > 0)
            {
                foreach (var notidata in deviceid)
                {
                    //look into this - rpn
                    //if (notidata.Devicetype == 1)
                    //{
                    //    output = notificationService.PushNotification(nbmModel.Message, "2", CommonUtility.Config.osAppId, CommonUtility.Config.osAuth, string.Join("\", \"", notidata.Deviceid), notidata.Devicetype, parentChildid, (int)nbmModel.SchoolId, sectionId, standardId, childgender, nbmModel.Subject, nbmModel.Id, mask);
                    //}
                    //if (notidata.Devicetype == 2)
                    //{
                    //    output = notificationService.PushNotification(nbmModel.Message, "2", CommonUtility.Config.osAppId, CommonUtility.Config.osAuth, string.Join("\", \"", notidata.Deviceid), notidata.Devicetype, parentChildid, (int)nbmModel.SchoolId, sectionId, standardId, childgender, nbmModel.Subject, nbmModel.Id, mask);
                    //}
                    output = notificationService.PushNotification(nbmModel.Message, "2", CommonUtility.Config.osAppId, CommonUtility.Config.osAuth, string.Join("\", \"", notidata.Deviceid), notidata.Devicetype, parentChildid, (int)nbmModel.SchoolId, sectionId, standardId, childgender, nbmModel.Subject, nbmModel.Id, mask);
                }
            }
            if (output == "NULL")
            {
                isErrorFound = true;
            }
            else
            {
                // Might be storing Push Notifcation for further investigation if any issues.
                strOneSignalResponse = ((strOneSignalResponse == "") ? "" : strOneSignalResponse + ",") + output;
            }
        }
        #endregion


        public void SendSMSEmail(NBMModel nbmModel)
        {
            TpContext db = new TpContext();
            string smsusers = "";
            string emailusers = "";
            List<string> toNumbersArr = new List<string>();
            List<Appuserdevice> deviceid = new List<Appuserdevice>();
            //var parentsSE = (from pcm in db.MParentchildmappings
            //                 join aui in db.MAppuserinfos
            //                 on pcm.Appuserid equals aui.Id
            //                 join csm in db.MChildschoolmappings
            //                 on pcm.Childid equals csm.Childid
            //                 join ssm in db.MStandardsectionmappings
            //                 on csm.Standardsectionmappingid equals ssm.Id
            //                 join br in db.MBranches
            //                 on ssm.Branchid equals br.Id
            //                 join sch in db.MSchools
            //                 on br.Schoolid equals sch.Id
            //                 where sch.Id == nbmModel.SchoolId && aui.Statusid == 1
            //                 select new
            //                 {
            //                     ParentName = aui.Firstname,
            //                     Phonenumber = aui.Phonenumber,
            //                     Emailid = aui.Emailid,
            //                     Issmsuser = aui.Issmsuser,
            //                     Parentid = ssm.Parentid,
            //                     Id = ssm.Id,
            //                 }).ToList();

            var parentsSE = (from aui in db.MAppuserinfos
                             join pcm in db.MParentchildmappings
                             on aui.Id equals pcm.Appuserid
                             join csm in db.MChildschoolmappings
                             on pcm.Childid equals csm.Childid
                             join ssm in db.MStandardsectionmappings
                             on csm.Standardsectionmappingid equals ssm.Id
                             join br in db.MBranches
                             on ssm.Branchid equals br.Id
                             join sch in db.MSchools
                             on br.Schoolid equals sch.Id
                             join ay in db.MAcademicyeardetails
                             on sch.Id equals ay.SchoolId
                             where sch.Id == nbmModel.SchoolId && aui.Statusid == 1 && ay.Currentyear == 1 && ay.SchoolId == nbmModel.SchoolId && csm.Statusid == 1 && csm.AcademicYearId == ay.Id
                             select new
                             {
                                 ParentName = aui.Firstname,
                                 Phonenumber = aui.Phonenumber,
                                 Emailid = aui.Emailid,
                                 Issmsuser = aui.Issmsuser,
                                 Parentid = ssm.Parentid,
                                 Id = ssm.Id,
                             }).ToList();

            if ((bool)nbmModel.IsEntireSchool)
            {
                if ((bool)nbmModel.Ispriority)
                {
                    // If the message is priority we need to send this message to all Parents
                    if (parentsSE != null)
                    {
                        var stringSmsUser = (from p in parentsSE
                                             select p.Phonenumber);

                        smsusers = String.Join(",", stringSmsUser);

                        var stringEmail = (from p in parentsSE select p.Emailid);
                        if (stringEmail != null)
                        {
                            foreach (var sEmail in stringEmail)
                            {
                                if (sEmail != null && sEmail.Trim().Length > 5)
                                {
                                    emailusers = emailusers + sEmail + ",";
                                }
                            }
                        }
                    }
                }
                else
                {
                    // If the message is not priority we need to send SMS only to Parents who registered as SMS users.
                    if (parentsSE != null)
                    {
                        var stringSmsUser = (from p in parentsSE
                                             where p.Issmsuser == true
                                             select p.Phonenumber);

                        smsusers = String.Join(",", stringSmsUser);

                        if (nbmModel.Isemail == true)
                        {
                            var stringEmail = (from p in parentsSE select p.Emailid);
                            if (stringEmail != null)
                            {
                                foreach (var sEmail in stringEmail)
                                {
                                    if (sEmail != null && sEmail.Trim().Length > 5)
                                    {
                                        emailusers = emailusers + sEmail + ",";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (nbmModel.IsParticularClass == true && nbmModel.IsParticularSection == true && nbmModel.IsParticularParent == false)
            {
                foreach (var inner in nbmModel.Classes)
                {
                    foreach (var item in inner.sectionModel)
                    {
                        if (parentsSE != null)
                        {
                            var stringSmsUser = (from p in parentsSE
                                                 where p.Issmsuser == true && p.Parentid == inner.StdId && p.Id == item.SectionId
                                                 select p.Phonenumber);

                            string smsuserssec = String.Join(",", stringSmsUser);

                            smsusers += smsuserssec + ",";

                            //Need to add email here.
                            //Uncommented by Priyanka
                            var stringEmail = (from p in parentsSE
                                               where p.Parentid == inner.StdId && p.Id == item.SectionId
                                               select p.Emailid);

                            if (stringEmail != null)
                            {
                                foreach (var sEmail in stringEmail)
                                {
                                    if (sEmail != null && sEmail.Trim().Length > 5)
                                    {
                                        emailusers = emailusers + sEmail + ",";
                                    }
                                }
                            }
                        }
                    }

                }
            }
            else if ((bool)nbmModel.IsParticularParent)
            {
                foreach (var inner in nbmModel.Classes)
                {
                    foreach (var item in inner.sectionModel)
                    {
                        foreach (var parchild in item.Parents)
                        {
                            var setofferoptn = db.MAppuserinfos.Where(w => w.Id == parchild.Id).FirstOrDefault();

                            if (setofferoptn != null)
                            {
                                //SMS for Non SmartPhone Users
                                if (setofferoptn.Issmsuser == true) //setofferoptn !=null
                                {
                                    toNumbersArr.Add(setofferoptn.Phonenumber);
                                }

                                if (nbmModel.Isemail == true)
                                {
                                    if (setofferoptn.Emailid.Trim().Length > 5)
                                        emailusers = emailusers + setofferoptn.Emailid + ",";
                                }
                            }
                        }
                    }
                }

            }

            pushSMSEmail(toNumbersArr, nbmModel, emailusers, smsusers);
        }


        public void pushSMSEmail(List<string> toNumbersArr, NBMModel nbmModel, string emailusers, string smsusers)
        {

            //string mask = "GETTALKTIVE";
            string mask = db2.MFeatures.Where(a => a.Schoolid == nbmModel.SchoolId).FirstOrDefault().Mask;
            //SMS 
            if (smsusers.Length > 0)
            {
                foreach (var to_number in smsusers.Substring(0, smsusers.Length - 1).Split(',').Distinct())
                {
                    toNumbersArr.Add(to_number);
                }
            }
            if (nbmModel.Sms != null && nbmModel.Sms.Trim().Length > 0)
            {
                MSMSService.SendSMS(toNumbersArr.ToArray(), nbmModel.Sms, mask);
            }

            //Email 
            if (emailusers.Trim().Length > 5 && (nbmModel.Ispriority == true || nbmModel.Isemail == true))
            {
                string template = /*"818d817a-69d9-46a2-a4a2-e379a0b228c1";*/ EMAIL_Message_Template.template;
                string body = template.Replace("MESSAGE_BODY", nbmModel.Message);

                while (emailusers.Trim().EndsWith(","))
                {
                    emailusers = emailusers.Substring(0, emailusers.Length - 1);
                }
                if (CommonUtility.Common.SendEMail == true)
                {
                    //var em = emailService.SendrackspaceEmail(nbmModel.Subject, body, emailusers, nbmModel.filename, nbmModel.base64);
                    var em = emailService.SendgridEmail(nbmModel.Subject, body, emailusers, nbmModel.filename, nbmModel.base64, mask);

                    //Email Logs - Table confirmed for Email
                    _ = emaillogService.AddEntity(new TEmaillog
                    {
                        Fromemailid = "no-reply@talkativeparents.com",
                        Noticeboardmsgid = NoticeBoardID,
                        Emailcount = emailusers.Split(',').Count(),
                        Toemailid = emailusers,
                        Createddate = DateTime.Now //Sent Date
                    });
                }
            }
        }

        public async Task<int> NBConditionCheck(NBMModel nbmModel)
        {
            //string regex = "<p>|</p>";
            //var message = System.Text.RegularExpressions.Regex.Replace(nbmModel.Message, regex, "").Trim();
            //nbmModel.Message = message;

            var Schoolid = db.MBranches.Where(x => x.Id.Equals(nbmModel.BranchId)).FirstOrDefault().Schoolid;
            var temp = await mSchoolService.GetEntityByID((int)Schoolid);

            if (temp == null)
            {
                return 0;
            }
            else
            {
                List<string> toNumbersArr = new List<string>();
                List<Appuserdevice> deviceid = new List<Appuserdevice>();
                List<TNoticeboardmessage> notiBoardId = new List<TNoticeboardmessage>();

                if ((bool)nbmModel.IsEntireSchool)
                {
                    TNoticeboardmessage tnb = new TNoticeboardmessage();
                    tnb.Branchid = nbmModel.BranchId;
                    tnb.Schooluserid = nbmModel.SchoolUserId;
                    tnb.Subject = nbmModel.Subject;
                    tnb.Message = nbmModel.Message;
                    tnb.Ispriority = nbmModel.Ispriority;
                    tnb.Isemail = nbmModel.Isemail;
                    tnb.Sms = nbmModel.Sms;
                    tnb.Attachments = nbmModel.Attachments;
                    tnb.IsEntireSchool = nbmModel.IsEntireSchool;
                    tnb.IsParticularClass = nbmModel.IsParticularClass;
                    tnb.IsParticularParent = nbmModel.IsParticularParent;
                    tnb.IsParticularSection = nbmModel.IsParticularSection;

                    tnb.Standardsectionmappingid = null;

                    tnb.Statusid = 1;
                    tnb.Createddate = DateTime.Now;
                    tnb.Modifieddate = DateTime.Now;

                    NoticeBoardID = await this.AddEntity(tnb);

                    notiBoardId.Add(new TNoticeboardmessage {
                        Id = NoticeBoardID,
                        Standardsectionmappingid = tnb.Standardsectionmappingid
                    });
                }
                //Particular Class , particular Section ,All Parents
                else if (nbmModel.IsParticularClass == true && nbmModel.IsParticularSection == true && nbmModel.IsParticularParent == false)
                {
                    foreach (var inner in nbmModel.Classes)
                    {
                        foreach (var item in inner.sectionModel)
                        {
                            TNoticeboardmessage tnb = new TNoticeboardmessage();
                            tnb.Branchid = nbmModel.BranchId;
                            tnb.Schooluserid = nbmModel.SchoolUserId;
                            tnb.Subject = nbmModel.Subject;
                            tnb.Message = nbmModel.Message;
                            tnb.Ispriority = nbmModel.Ispriority;
                            tnb.Isemail = nbmModel.Isemail;
                            tnb.Sms = nbmModel.Sms;
                            tnb.Attachments = nbmModel.Attachments;
                            tnb.IsEntireSchool = nbmModel.IsEntireSchool;
                            tnb.IsParticularClass = nbmModel.IsParticularClass;
                            tnb.IsParticularParent = nbmModel.IsParticularParent;
                            tnb.IsParticularSection = nbmModel.IsParticularSection;
                            tnb.Statusid = 1;
                            tnb.Createddate = DateTime.Now;
                            tnb.Modifieddate = DateTime.Now;

                            var std_sec = await this.db.MStandardsectionmappings.Where(x => x.Id.Equals(item.SectionId)).FirstOrDefaultAsync();
                            if (std_sec == null)
                            {
                                tnb.Standardsectionmappingid = inner.StdId;
                            }
                            else
                            {
                                tnb.Standardsectionmappingid = item.SectionId;
                            }

                            NoticeBoardID = await this.AddEntity(tnb);

                            notiBoardId.Add(new TNoticeboardmessage
                            {
                                Id = NoticeBoardID,
                                Standardsectionmappingid = tnb.Standardsectionmappingid
                            });

                            //foreach (var parchild in item.Parents)
                            //{
                            //    TNoticeboardmapping oNotiBrdMapping = new TNoticeboardmapping();
                            //    oNotiBrdMapping.Noticeboardmsgid = NoticeBoardID;
                            //    oNotiBrdMapping.Childid = parchild.ChildId;
                            //    oNotiBrdMapping.Appuserid = parchild.Id;
                            //    oNotiBrdMapping.Statusid = 1;
                            //    oNotiBrdMapping.Createddate = DateTime.Now;
                            //    oNotiBrdMapping.Modifieddate = DateTime.Now;
                            //    await this.tNoticeboardmappingService.AddEntity(oNotiBrdMapping);
                            //}
                        }
                    }
                }
                //Particular Parent
                else if ((bool)nbmModel.IsParticularParent)
                {
                    foreach (var inner in nbmModel.Classes)
                    {
                        foreach (var item in inner.sectionModel)
                        {
                            TNoticeboardmessage tnb = new TNoticeboardmessage();
                            tnb.Branchid = nbmModel.BranchId;
                            tnb.Schooluserid = nbmModel.SchoolUserId;
                            tnb.Subject = nbmModel.Subject;
                            tnb.Message = nbmModel.Message;
                            tnb.Ispriority = nbmModel.Ispriority;
                            tnb.Isemail = nbmModel.Isemail;
                            tnb.Sms = nbmModel.Sms;
                            tnb.Attachments = nbmModel.Attachments;
                            tnb.IsEntireSchool = nbmModel.IsEntireSchool;
                            tnb.IsParticularClass = nbmModel.IsParticularClass;
                            tnb.IsParticularParent = nbmModel.IsParticularParent;
                            tnb.IsParticularSection = nbmModel.IsParticularSection;
                            tnb.Statusid = 1;
                            tnb.Createddate = DateTime.Now;
                            tnb.Modifieddate = DateTime.Now;

                            var std_sec = await this.db.MStandardsectionmappings.Where(x => x.Id.Equals(item.SectionId)).FirstOrDefaultAsync();
                            if (std_sec == null)
                            {
                                tnb.Standardsectionmappingid = inner.StdId;
                            }
                            else
                            {
                                tnb.Standardsectionmappingid = item.SectionId;
                            }

                            NoticeBoardID = await this.AddEntity(tnb);

                            notiBoardId.Add(new TNoticeboardmessage
                            {
                                Id = NoticeBoardID,
                                Standardsectionmappingid = tnb.Standardsectionmappingid
                            });

                            foreach (var parchild in item.Parents)
                            {
                                TNoticeboardmapping oNotiBrdMapping = new TNoticeboardmapping();
                                oNotiBrdMapping.Noticeboardmsgid = NoticeBoardID;
                                oNotiBrdMapping.Childid = parchild.ChildId;
                                oNotiBrdMapping.Appuserid = parchild.Id;
                                //oNotiBrdMapping.Otherparentid = db.MParentchildmappings.Where(x => x.Childid == parchild.ChildId && x.Appuserid != parchild.Id).Select(x => x.Appuserid).FirstOrDefault();
                                oNotiBrdMapping.Statusid = 1;
                                oNotiBrdMapping.Createddate = DateTime.Now;
                                oNotiBrdMapping.Modifieddate = DateTime.Now;
                                await this.tNoticeboardmappingService.AddEntity(oNotiBrdMapping);

                            }
                        }
                    }
                }

                //Saving to Database 
                //if (CommonUtility.Common.SaveNotificationToDB)
                //{
                //    db.SaveChanges();
                //}

                #region SMS and Push Notificattion
                //Asynchronous Task
                ThreadStart ts = delegate
                {
                    SENServices(nbmModel, notiBoardId);
                };
                new Thread(ts).Start();
                #endregion


                return NoticeBoardID;
            }

        }


        //Mobie App APIs

        public object GetNoticeBoardMsgForAPI(int SchoolId, int ChildId, int PageSize = 10, int pageNumber = 1)
        {
            try
            {
                List<SchoolMessageModel> msgList = new List<SchoolMessageModel>();
                var schoolname = db2.MSchools.Where(w => w.Id == SchoolId).FirstOrDefault().Name;
                var currentacademicyear = db.MAcademicyeardetails.Where(w => w.SchoolId == SchoolId && w.Currentyear == 1).Select(w => w.Id).FirstOrDefault();
                //Case 1: Entire School
                var entireSchoolMsgs = db.TNoticeboardmessages.Where(x => x.Branch.Schoolid == SchoolId && x.IsEntireSchool == true).Include(x => x.Branch).Include(w => w.Standardsectionmapping).OrderByDescending(d => d.Createddate);
                if (entireSchoolMsgs != null)
                {
                    foreach (var nbm in entireSchoolMsgs)
                    {
                        SchoolMessageModel smm = new SchoolMessageModel();
                        smm.Id = nbm.Id;
                        smm.SchoolUserId = (int)nbm.Schooluserid;
                        smm.SchoolId = (int)nbm.Branch.Schoolid;

                        if (nbm.Standardsectionmappingid != 0 && nbm.Standardsectionmappingid != null)
                        {
                            if (nbm.Standardsectionmapping.Parentid == null)
                            {
                                smm.StandardId = (int)nbm.Standardsectionmappingid;
                                smm.SectionId = 0;
                            }
                            else
                            {
                                smm.StandardId = (int)nbm.Standardsectionmapping.Parentid;
                                smm.SectionId = (int)nbm.Standardsectionmappingid;
                            }
                        }
                        else
                        {
                            smm.StandardId = 0;
                            smm.SectionId = 0;
                        }

                        smm.Subject = nbm.Subject;
                        smm.Message = nbm.Message;
                        smm.IsEntireSchool = nbm.IsEntireSchool;
                        smm.IsParticularClass = nbm.IsParticularClass;
                        smm.IsParticularSection = nbm.IsParticularSection;
                        smm.IsParticularParent = nbm.IsParticularParent;
                        smm.IsPriority = nbm.Ispriority;
                        DateTime currentTime = (DateTime)nbm.Createddate;
                        DateTime x30MinsLater = currentTime.AddHours(5);
                         x30MinsLater = x30MinsLater.AddMinutes(30);
                        smm.DateTimeSent = x30MinsLater;
                        //smm.ShouldDelete = nbm.ShouldDelete;
                        smm.SMS = nbm.Sms;
                        smm.Attachments = nbm.Attachments;
                        smm.SchoolName = schoolname;

                        msgList.Add(smm);
                    }
                }

                //Case 2:  For Particular Class and Section and  All Parents
               
                var secStdList = db.MChildschoolmappings.Where(x => x.Childid == ChildId && x.AcademicYearId == currentacademicyear).Select(w => w.Standardsectionmappingid);
                if (secStdList != null)
                {
                    foreach (var inner in secStdList)
                    {
                        var secStdMsgs = db3.TNoticeboardmessages.Where(x => x.Standardsectionmappingid == inner && x.IsParticularParent == false).Include(x => x.Branch).Include(w => w.Standardsectionmapping).OrderByDescending(d => d.Createddate);
                        if (secStdMsgs != null)
                        {
                            foreach (var nbm in secStdMsgs)
                            {
                                SchoolMessageModel smm = new SchoolMessageModel();
                                smm.Id = nbm.Id;
                                smm.SchoolUserId = (int)nbm.Schooluserid;
                                smm.SchoolId = (int)nbm.Branch.Schoolid;


                                if (nbm.Standardsectionmappingid != 0 && nbm.Standardsectionmappingid != null)
                                {
                                    if (nbm.Standardsectionmapping.Parentid == null)
                                    {
                                        smm.StandardId = (int)nbm.Standardsectionmappingid;
                                        smm.SectionId = 0;
                                    }
                                    else
                                    {
                                        smm.StandardId = (int)nbm.Standardsectionmapping.Parentid;
                                        smm.SectionId = (int)nbm.Standardsectionmappingid;
                                    }
                                }
                                else
                                {
                                    smm.StandardId = 0;
                                    smm.SectionId = 0;
                                }

                                smm.Subject = nbm.Subject;
                                smm.Message = nbm.Message;
                                smm.IsEntireSchool = nbm.IsEntireSchool;
                                smm.IsParticularClass = nbm.IsParticularClass;
                                smm.IsParticularSection = nbm.IsParticularSection;
                                smm.IsParticularParent = nbm.IsParticularParent;
                                smm.IsPriority = nbm.Ispriority;
                                DateTime currentTime = (DateTime)nbm.Createddate;
                                DateTime x30MinsLater = currentTime.AddHours(5);
                                x30MinsLater = x30MinsLater.AddMinutes(30);
                                smm.DateTimeSent = x30MinsLater;
                                //smm.ShouldDelete = nbm.ShouldDelete;
                                smm.SMS = nbm.Sms;
                                smm.Attachments = nbm.Attachments;
                                smm.SchoolName = schoolname;

                                msgList.Add(smm);
                            }
                        }

                    }
                }

                //Case 3:  For Particular Parent
                var parentMsgsList = (from nbm in db.TNoticeboardmessages
                                      join nbp in db.TNoticeboardmappings on nbm.Id equals nbp.Noticeboardmsgid
                                      join csm in db.MChildschoolmappings on nbp.Childid equals csm.Childid
                                      where nbp.Childid == ChildId && nbm.Branch.Schoolid == SchoolId && nbm.IsParticularParent == true && csm.AcademicYearId == currentacademicyear
                                      select nbm).Include(x => x.Branch).Include(x => x.Standardsectionmapping);
                if (parentMsgsList != null)
                {
                    foreach (var item in parentMsgsList)
                    {
                        SchoolMessageModel simp = new SchoolMessageModel();
                        simp.Id = item.Id;
                        simp.SchoolUserId = (int)item.Schooluserid;
                        simp.SchoolId = (int)item.Branch.Schoolid;

                        if (item.Standardsectionmappingid != 0 && item.Standardsectionmappingid != null)
                        {
                            if (item.Standardsectionmapping.Parentid == null)
                            {
                                simp.StandardId = (int)item.Standardsectionmappingid;
                                simp.SectionId = 0;
                            }
                            else
                            {
                                simp.StandardId = (int)item.Standardsectionmapping.Parentid;
                                simp.SectionId = (int)item.Standardsectionmappingid;
                            }
                        }
                        else
                        {
                            simp.StandardId = 0;
                            simp.SectionId = 0;
                        }

                        simp.Subject = item.Subject;
                        simp.Message = item.Message;
                        simp.IsEntireSchool = item.IsEntireSchool;
                        simp.IsParticularClass = item.IsParticularClass;
                        simp.IsParticularSection = item.IsParticularSection;
                        simp.IsParticularParent = item.IsParticularParent;
                        simp.IsPriority = item.Ispriority;
                        DateTime currentTime = (DateTime)item.Createddate;
                        DateTime x30MinsLater = currentTime.AddHours(5);
                        x30MinsLater = x30MinsLater.AddMinutes(30);
                        simp.DateTimeSent = x30MinsLater;
                        //simp.ShouldDelete = item.ShouldDelete;
                        simp.SMS = item.Sms;
                        simp.Attachments = item.Attachments;
                        simp.SchoolName = schoolname;

                        msgList.Add(simp);
                    }
                }


                //msgList.AddRange(parentMsgsList);
                msgList = msgList.Distinct().ToList();
                msgList = msgList.OrderByDescending(x => x.DateTimeSent).ToList();
                msgList = msgList.GroupBy(i => i.Id).Select(g => g.First()).ToList();


                int count = msgList.Count();
                int CurrentPage = pageNumber;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                var items = msgList.Skip((CurrentPage - 1) * PageSize).Take(PageSize).OrderByDescending(w => w.DateTimeSent).ToList();
                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                var obj = new
                {
                    TotalPages = TotalPages,
                    TotalItems = count,
                    items = items
                };
                if (msgList.Count > 0)
                {
                    return (obj);
                }
                else
                {
                    return (new
                    {
                        msg = "No Messages",
                        StatusCode = HttpStatusCode.NoContent
                    });
                    //return null;
                }
            }
            catch (Exception ex)
            {

                return (new
                {
                    msg = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest
                });
                //return null;
            }
        }

    }
}
