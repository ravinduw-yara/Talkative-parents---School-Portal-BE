using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Rest.Serialization;
using Nancy.Routing.Trie;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static CommonUtility.RequestModels.SoundingboardmessagePublicAndPrivate;
using static System.Collections.Specialized.BitVector32;

namespace Services
{
    public interface ITSoundingboardmessageService : ICommonService
    {
        Task<int> AddEntity(TSoundingboardmessage entity);
        Task<int> UpdateEntity(TSoundingboardmessage entity);
        //Task<Object> GetSBPrivateMessages(int schoolUser, int schoolid, int Type);

        Task<object> GetSBMessagesByIdV2ForAPI(int messageid, int userid);

        Task<TSoundingboardmessage> GetEntityIDForUpdate(int entityID);

        object GetSBPrivateMessages(int schoolUser, int schoolId, int? StandardId = null, int? SectionId = null, DateTime? DateFrom = null, DateTime? DateTo = null, string searchString = "", int PageSize = 10, int pageNumber = 1, int Type = 0);

        Task<object> SendSBMessagePushNotificationForAPI(SendSBMessageNotificationModel sBModel, int messageid, string mtype);
        Task<object> PostSBPrivateAndPublicMessageByMessageIdV2ForAPI(PostCommentModel pModel, int appUserId);
        Task<object> GetSBPrivateAndPublicMessageV2ForAPI(string Type, int schoolid, int appuserid, int parentid, int Childid, int PageSize, int pageNumber);
        Task<object> PostSBMessageV2ForAPI(SoundingboardmessagePublicAndPrivate message, int appuserid);
        Task<object> SendTeacherAppSBMessagePushNotification(SendSBMessageNotificationModel sBModel, int messageid, string mtype);


    }
    public class TSoundingboardmessageService : ITSoundingboardmessageService
    {
        private readonly IRepository<TSoundingboardmessage> repository;
        private DbSet<TSoundingboardmessage> localDBSet;
        private readonly TpContext db = new TpContext();
        private readonly INotificationService nNotificationService;
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext db3 = new TpContext();

        private readonly TpContext tpContext;

        public TSoundingboardmessageService(IRepository<TSoundingboardmessage> repository, TpContext tpContext, INotificationService nNotificationService)
        {
            this.repository = repository;
            this.tpContext = tpContext;
            this.nNotificationService = nNotificationService;
        }

        public async Task<int> AddEntity(TSoundingboardmessage entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<TSoundingboardmessage>)await this.repository.GetAll();

        private static Object Mapper(TSoundingboardmessage x) => new
        {
            x.Id,
            x.Subject,
            x.Description,
            x.Isparentreplied,
            x.Isstaffreplied,
            x.Attachments,
            x.Commentscount,
            x.Standardsectionmappingid,
            x.Appuserinfoid,
            x.Childinfoid,
            x.Categoryid,
            x.Groupid,
            x.Didread,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };

        private async Task<IQueryable<TSoundingboardmessage>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.Category)
            .Include(x => x.Standardsectionmapping)
            .Include(x => x.Standardsectionmapping.Branch)
            .Include(x => x.Appuserinfo)
            .Include(x => x.Childinfo)
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<TSoundingboardmessage> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<int> UpdateEntity(TSoundingboardmessage entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        #region service for get sb private messages
        private int GetUserRank(int schoolUser)
        {
            try
            {
                if (schoolUser != 0)
                {
                    var res = (from mr in tpContext.MRoles
                               join mc in tpContext.MCategories
                               on mr.Id equals mc.Roleid
                               join sur in tpContext.MSchooluserroles
                               on mc.Id equals sur.Categoryid
                               where sur.Schooluserid == schoolUser
                               select new
                               {
                                   Rank = mr.Rank,
                               }).Min(x => x.Rank);

                    return (int)res;
                }
                return 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private IQueryable<TSoundingboardmessage> GetSBMessages(int schoolUser, int schoolid, string searchString)
        {
            try
            {
                if (schoolUser != 0)
                {
                    IQueryable<TSoundingboardmessage> res = null;
                    IQueryable<TSoundingboardmessage> res1 = null;
                    var UserRank = GetUserRank(schoolUser);

                    var SchooUserRoles = tpContext.MSchooluserroles.Where(x => x.Schooluserid.Equals(schoolUser)).FirstOrDefault();

                    //var rankid = tpContext.MRoles.Where(x => x.Rank == UserRank || x.Rank > UserRank).FirstOrDefault().Id;
                    //var catKey = tpContext.MCategories.Where(x => x.Roleid == rankid).FirstOrDefault().Id;
                    var roleids = tpContext.MRoles.Where(x => x.Rank >= UserRank && x.Schoolid == schoolid).Select(x => x.Id).ToList(); //18/5/2024
                    //var roleid = tpContext.MRoles.Where(x => x.Rank == UserRank && x.Schoolid == schoolid).Select(x => x.Id).FirstOrDefault(); //18/5/2024
                    var catKey = tpContext.MCategories.Where(x => roleids.Contains((int)x.Roleid)).Select(x => x.Id).ToList();

                    // var catKey = tpContext.MCategories.Where(x => x.Roleid == roleid).Select(x => x.Id).FirstOrDefault();//18/5/2024
                    var SchoolUserStd = db.MSchooluserroles.Where(x => x.Schooluserid == schoolUser).Select(x => x.Standardsectionmappingid);


                    if (UserRank == 1)
                    {
                        res = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmapping.Branch.Schoolid == schoolid && ((x.Category.Name.Contains(searchString)) || ((x.Childinfo.Firstname.Contains(searchString)) || (x.Childinfo.Lastname.Contains(searchString))))).OrderBy(x => x.Modifieddate);

                    }
                    else //if (UserRank > 1)
                    {
                        #region remove later
                        //foreach (var item in SchoolUserStd)
                        //{
                        //    var chk = tpContext.MStandardsectionmappings.Where(x => x.Id == item).FirstOrDefault();

                        //    if (chk.Parentid == null)
                        //    {
                        //        var stdid = chk.Parentid;
                        //        if (res == null)
                        //        {
                        //            res = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == stdid && x.Categoryid >= catKey && ((x.Category.Name.Contains(searchString)) || ((x.Childinfo.Firstname.Contains(searchString)) || (x.Childinfo.Lastname.Contains(searchString))))).OrderBy(x => x.Modifieddate);
                        //        }
                        //        else
                        //        {
                        //            var res1 = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == stdid && x.Categoryid >= catKey && ((x.Category.Name.Contains(searchString)) || ((x.Childinfo.Firstname.Contains(searchString)) || (x.Childinfo.Lastname.Contains(searchString))))).OrderBy(x => x.Modifieddate);
                        //            res = res.Concat(res1);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        var secid = chk.Id;
                        //        if (res == null)
                        //        {
                        //            res = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == secid && x.Categoryid >= catKey && ((x.Category.Name.Contains(searchString)) || ((x.Childinfo.Firstname.Contains(searchString)) || (x.Childinfo.Lastname.Contains(searchString))))).OrderBy(x => x.Modifieddate);
                        //        }
                        //        else
                        //        {
                        //            var res1 = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == secid && x.Categoryid >= catKey && ((x.Category.Name.Contains(searchString)) || ((x.Childinfo.Firstname.Contains(searchString)) || (x.Childinfo.Lastname.Contains(searchString))))).OrderBy(x => x.Modifieddate);
                        //            res = res.Concat(res1);
                        //        }
                        //    }
                        //}
                        #endregion

                        foreach (var item in SchoolUserStd)
                        {
                            var chk = tpContext.MStandardsectionmappings.Where(x => x.Id == item).FirstOrDefault();
                            var stdid = chk.Parentid; // 26/7/2024
                            if (chk.Parentid == null)
                            {
                                var sectionids = tpContext.MStandardsectionmappings.Where(x => x.Parentid == item).Select(x=> x.Id).ToList();  // 26/7/2024

                                if (res == null)
                                {
                                    res = tpContext.TSoundingboardmessages
                                                    .Where(x => sectionids.Contains((int)x.Standardsectionmappingid) &&
                                                                catKey.Contains((int)x.Categoryid) &&
                                                                (x.Category.Name.Contains(searchString) ||
                                                                 x.Childinfo.Firstname.Contains(searchString) ||
                                                                 x.Childinfo.Lastname.Contains(searchString)))
                                                    .OrderBy(x => x.Modifieddate);

                                    //  var res2 = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == sectionid && catKey.Contains((int)x.Categoryid) && (x.Category.Name.Contains(searchString) || x.Childinfo.Firstname.Contains(searchString) || x.Childinfo.Lastname.Contains(searchString))).OrderBy(x => x.Modifieddate);
                             
                                                                  //res = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == stdid && x.Category.Roleid <= roleid && ((x.Category.Name.Contains(searchString)) || ((x.Childinfo.Firstname.Contains(searchString)) || (x.Childinfo.Lastname.Contains(searchString))))).OrderBy(x => x.Modifieddate);
                                }
                                else
                                {
                                    var res2 = tpContext.TSoundingboardmessages
                                                  .Where(x => sectionids.Contains((int)x.Standardsectionmappingid) &&
                                                              catKey.Contains((int)x.Categoryid) &&
                                                              (x.Category.Name.Contains(searchString) ||
                                                               x.Childinfo.Firstname.Contains(searchString) ||
                                                               x.Childinfo.Lastname.Contains(searchString)))
                                                  .OrderBy(x => x.Modifieddate);
                                    //var res2 = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == sectionid && catKey.Contains((int)x.Categoryid) && (x.Category.Name.Contains(searchString) || x.Childinfo.Firstname.Contains(searchString) || x.Childinfo.Lastname.Contains(searchString))).OrderBy(x => x.Modifieddate);
                                    res = res.Concat(res2); // 26/7/2024
                                    
                                    //var res1 = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == stdid && catKey.Contains((int)x.Categoryid) && (x.Category.Name.Contains(searchString) || x.Childinfo.Firstname.Contains(searchString) || x.Childinfo.Lastname.Contains(searchString))).OrderBy(x => x.Modifieddate);

                                    //var res1 = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == stdid && x.Category.Roleid <= roleid && ((x.Category.Name.Contains(searchString)) || ((x.Childinfo.Firstname.Contains(searchString)) || (x.Childinfo.Lastname.Contains(searchString))))).OrderBy(x => x.Modifieddate);
                                    //res = res.Concat(res1);
                                }
                            }
                            else
                            {
                                var secid = chk.Id;
                                if (res == null)
                                {
                                    res = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == secid && catKey.Contains((int)x.Categoryid) && (x.Category.Name.Contains(searchString) || x.Childinfo.Firstname.Contains(searchString) || x.Childinfo.Lastname.Contains(searchString))).OrderBy(x => x.Modifieddate);

                                    // res = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == secid && x.Category.Roleid <= roleid && ((x.Category.Name.Contains(searchString)) || ((x.Childinfo.Firstname.Contains(searchString)) || (x.Childinfo.Lastname.Contains(searchString))))).OrderBy(x => x.Modifieddate);
                                }
                                else
                                {
                                    res1 = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == secid && catKey.Contains((int)x.Categoryid) && (x.Category.Name.Contains(searchString) || x.Childinfo.Firstname.Contains(searchString) || x.Childinfo.Lastname.Contains(searchString))).OrderBy(x => x.Modifieddate);

                                    // var res1 = tpContext.TSoundingboardmessages.Where(x => x.Standardsectionmappingid == secid && x.Category.Roleid <= roleid && ((x.Category.Name.Contains(searchString)) || ((x.Childinfo.Firstname.Contains(searchString)) || (x.Childinfo.Lastname.Contains(searchString))))).OrderBy(x => x.Modifieddate);
                                    res = res.Concat(res1);
                                }
                            }
                        }
                    }
                    return res;
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public object GetSBPrivateMessages(int schoolUser, int schoolid, int? Standardid = null, int? Sectionid = null, DateTime? DateFrom = null, DateTime? DateTo = null, string searchString = "", int PageSize = 0, int pageNumber = 0, int Type = 0)
        {
            List<MTSoundingboardmessageMapModel> getSBMessages = new List<MTSoundingboardmessageMapModel>();

            try
            {
                var objRes = GetSBMessages(schoolUser, schoolid, searchString);
                if (objRes != null)
                {
                    foreach (var item in objRes)
                    {
                        TpContext tpContext = new TpContext();

                        int relationId = Convert.ToInt32(tpContext.MParentchildmappings.FirstOrDefault(x => x.Childid == item.Childinfoid && x.Appuserid == item.Appuserinfoid)?.Relationtypeid);

                        MTSoundingboardmessageMapModel mod = new MTSoundingboardmessageMapModel
                        {
                            Id = item.Id,
                            ParentId = item.Appuserinfoid,
                            ChildId = item.Childinfoid,
                            CategoryKey = item.Categoryid,
                            CategoryName = tpContext.MCategories.FirstOrDefault(x => x.Id == item.Categoryid)?.Name,
                            ParentName = tpContext.MAppuserinfos.FirstOrDefault(x => x.Id == item.Appuserinfoid)?.Firstname,
                            ChildName = tpContext.MChildinfos.Where(x => x.Id == item.Childinfoid).Select(x => x.Firstname + " " + x.Lastname).FirstOrDefault(),
                            Relation = tpContext.MRelationtypes.FirstOrDefault(x => x.Id == relationId)?.Type,
                            StandardId = tpContext.MStandardsectionmappings.FirstOrDefault(x => x.Id == item.Standardsectionmappingid)?.Parentid,
                            SectionId = tpContext.MStandardsectionmappings.FirstOrDefault(x => x.Id == item.Standardsectionmappingid)?.Id,
                            Subject = item.Subject,
                            Description = item.Description,
                            CommentsCount = item.Commentscount,
                            Attachment = item.Attachments,
                            IsStaffReplied = item.Isstaffreplied,
                            IsParentReplied = item.Isparentreplied,
                            Schoolid = schoolid,
                            CreatedDate = item.Createddate,
                            UpdatedDate = item.Modifieddate,
                            DidRead = item.Didread,
                            IsActive = item.Statusid,
                            Type = Type,

                        };

                        mod.ClassName = tpContext.MStandardsectionmappings.FirstOrDefault(w => w.Id == mod.StandardId)?.Name;
                        mod.SectionName = tpContext.MStandardsectionmappings.FirstOrDefault(x => x.Id == item.Standardsectionmappingid)?.Name;

                        // Filter by Standard and Section
                        if (Standardid.HasValue && mod.StandardId != Standardid.Value || Sectionid.HasValue && mod.SectionId != Sectionid.Value)
                        {
                            continue;
                        }

                        // Filter by Date Range
                        if ((DateFrom.HasValue && mod.UpdatedDate < DateFrom) || (DateTo.HasValue && mod.UpdatedDate > DateTo))
                        {
                            continue;
                        }

                        // Filter by searchString in ChildName
                        if (!string.IsNullOrEmpty(searchString) && !mod.ChildName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }


                        getSBMessages.Add(mod);
                    }

                    if (getSBMessages.Any())
                    {
                        var orderedMessages = getSBMessages.OrderByDescending(w => w.UpdatedDate).ToList();
                        int totalCount = orderedMessages.Count;
                        int totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
                        int unreadCount = orderedMessages.Count(x => x.DidRead == false && x.IsParentReplied == true);

                        var items = orderedMessages.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();

                        var obj1 = new
                        {
                            unreadcount = unreadCount,
                            TotalPages = totalPages,
                            items
                        };

                        return obj1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return "No data found";
        }


        #endregion

        public async Task<object> GetSBMessagesByIdV2ForAPI(int messageid, int userid)//Modify this Api Reply Notification
        {
            var data = await db.TSoundingboardmessages.Where(x => x.Id == messageid)
            .Include(x => x.Category)
            .Include(x => x.Standardsectionmapping)
            .Include(x => x.Standardsectionmapping.Branch)
            .Include(x => x.Appuserinfo)
            .Include(x => x.Childinfo).SingleOrDefaultAsync();

            if (data != null)
            {
                SoundingboardmessagePublicAndPrivate sbm = new SoundingboardmessagePublicAndPrivate();
                sbm.Id = data.Id;
                sbm.ParentId = data.Appuserinfoid;
                sbm.ChildId = data.Childinfoid;
                sbm.CategoryKey = data.Categoryid;
                sbm.CategoryName = data.Category.Name;
                sbm.ParentName = data.Appuserinfo.Firstname;
                sbm.ParentMiddleName = data.Appuserinfo.Middlename;
                sbm.ParentLastName = data.Appuserinfo.Lastname;
                sbm.ChildName = data.Childinfo.Firstname;
                sbm.ChildMiddleName = data.Childinfo.Middlename;
                sbm.ChildLastName = data.Childinfo.Lastname;


                sbm.Relation = await this.db.MParentchildmappings.Where(x => x.Childid.Equals(data.Childinfoid) && x.Appuserid.Equals(data.Appuserinfoid)).Select(a => a.Relationtype.Type).FirstOrDefaultAsync();

                if (data.Standardsectionmapping.Parentid == null)
                {
                    sbm.StandardId = data.Standardsectionmappingid;
                    sbm.SectionId = null;
                    sbm.StandardName = db.MStandardsectionmappings.Where(w => w.Id == data.Standardsectionmappingid).FirstOrDefault().Name;
                    sbm.SectionName = null;
                }
                else
                {
                    sbm.StandardId = data.Standardsectionmapping.Parentid;
                    sbm.SectionId = data.Standardsectionmappingid;
                    sbm.StandardName = db.MStandardsectionmappings.Where(w => w.Id == data.Standardsectionmapping.Parentid).FirstOrDefault().Name;
                    sbm.SectionName = db.MStandardsectionmappings.Where(w => w.Id == data.Standardsectionmappingid).FirstOrDefault().Name;
                }

                sbm.Schoolid = data.Standardsectionmapping.Branch.Schoolid;

                sbm.Subject = data.Subject;
                sbm.Description = data.Description;
                sbm.CommentsCount = data.Commentscount;
                //sbm.LikesCount = data.;
                sbm.Attachment = data.Attachments;
                //sbm.AttachmentCount = data.
                sbm.IsStaffReplied = data.Isstaffreplied;
                sbm.IsParentReplied = data.Isparentreplied;


                sbm.CreatedDate = data.Createddate;
                sbm.UpdatedDate = data.Modifieddate;
                //sbm.DidRead = data.Didread;
                sbm.DidRead = data.Didread; //2024/4/2
                sbm.IsActive = data.Statusid;
                sbm.Type = 0.ToString(); //it was 0 for everything
                //sbm.SBcount = data.;
                sbm.schooluserid = userid;

                return sbm;
            }
            return (new
            {
                Value = "Soundingboard message not found",
                StatusCode = HttpStatusCode.BadRequest
            });
        }


        #region Services for send SB notification
        public async Task<object> SendSBMessagePushNotificationForAPI(SendSBMessageNotificationModel sBModel, int messageid, string mtype)
        {

            var iserrorfound = false;

            var data = await this.db.MSchools.Select(w => new
            {
                Id = w.Id,
                Name = w.Name
            }).Where(x => x.Id == sBModel.Schoolid).FirstOrDefaultAsync();

            var osAppId = Config.osAppId;
            var osAuth = Config.osAuth;

            var data1 = await this.db.TSoundingboardmessages.Where(w => w.Id == messageid).Include(w => w.Standardsectionmapping).FirstOrDefaultAsync();

            if (data1 != null)
            {
                SBMessagePushNotiModel sbm = new SBMessagePushNotiModel();
                sbm.Id = data1.Id;
                sbm.Schoolid = sBModel.Schoolid;
                sbm.ParentId = data1.Appuserinfoid;
                sbm.ChildId = data1.Childinfoid;
                sbm.Subject = data1.Subject;

                if (data1.Standardsectionmapping.Parentid == null)
                {
                    sbm.StandardId = data1.Standardsectionmappingid;
                    sbm.SectionId = null;
                }
                else
                {
                    sbm.StandardId = data1.Standardsectionmapping.Parentid;
                    sbm.SectionId = data1.Standardsectionmappingid;
                }


                string mask = this.db2.MFeatures.Where(a => a.Schoolid.Equals(sBModel.Schoolid)).FirstOrDefault().Mask;

                List<Devices> Deviceid = GetDeviceid((int)sbm.ParentId);
                foreach (var notidata in Deviceid)
                {
                    var output = "";

                    if (notidata.DeviceType == 1)//(notidata.DeviceType == 1)
                    {
                        output = nNotificationService.PushSBNoti(sBModel.Description, "2", osAppId.ToString(), osAuth.ToString(), string.Join("\", \"", notidata.Deviceid), notidata.DeviceType, sbm.ParentId, sbm.ChildId, sBModel.Schoolid, sbm.SectionId, sbm.StandardId, sbm.Subject, messageid, mask);
                    }
                    else if (notidata.DeviceType == 2) //(notidata.DeviceType == 2)
                    {
                        output = nNotificationService.PushSBNoti(sBModel.Description, "2", osAppId.ToString(), osAuth.ToString(), string.Join("\", \"", notidata.Deviceid), notidata.DeviceType, sbm.ParentId, sbm.ChildId, sBModel.Schoolid, sbm.SectionId, sbm.StandardId, sbm.Subject, messageid, mask);
                    }

                    if (output == "null")
                    {
                        return (new
                        {
                            Message = "Failed to send notification",
                            StatusCode = HttpStatusCode.BadRequest
                        });
                    }
                }
                var ups = await this.UpdateSBPostComments((int)sBModel.Schoolid, messageid, sBModel.IsParentReplied);
                if (ups == null)
                {
                    return (new
                    {
                        Message = "Failed to update soundingboard",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
                else
                {
                    return (new
                    {
                        Message = "Send Successfully",
                        StatusCode = HttpStatusCode.OK
                    });
                }
            }
            else
            {
                return (new
                {
                    Message = "Message not found",
                    StatusCode = HttpStatusCode.NotFound
                });
            }

        }
        // Services for send SB notification SendTeacherAppSBMessagePushNotification 29/4/2025
        public async Task<object> SendTeacherAppSBMessagePushNotification(SendSBMessageNotificationModel sBModel, int messageid, string mtype)
        {
            //6/5/2025

            var msgdetails = await this.db.TSoundingboardmessages.Where(w => w.Id == messageid).Include(w => w.Standardsectionmapping).FirstOrDefaultAsync();

            var SchooUserId = tpContext.MSchooluserroles.Where(x => x.Categoryid == msgdetails.Categoryid || x.Standardsectionmappingid == msgdetails.Standardsectionmappingid).Select(x => x.Schooluserid).FirstOrDefault();

            var SchoolUserPhonenumber = db.MSchooluserinfos.Where(x => x.Id == SchooUserId).Select(x => x.Phonenumber);


            var iserrorfound = false;
            if (msgdetails != null)
            {
                SBMessagePushNotiModel sbm = new SBMessagePushNotiModel();
                sbm.Id = msgdetails.Id;
                sbm.Schoolid = sBModel.Schoolid;
                sbm.ParentId = msgdetails.Appuserinfoid;
                sbm.ChildId = msgdetails.Childinfoid;
                sbm.Subject = msgdetails.Subject;

                if (msgdetails.Standardsectionmapping.Parentid == null)
                {
                    sbm.StandardId = msgdetails.Standardsectionmappingid;
                    sbm.SectionId = null;
                }
                else
                {
                    sbm.StandardId = msgdetails.Standardsectionmapping.Parentid;
                    sbm.SectionId = msgdetails.Standardsectionmappingid;
                }
            

            string mask = this.db2.MFeatures.Where(a => a.Schoolid.Equals(sBModel.Schoolid)).FirstOrDefault().Mask;
            var output = "";
            var osAppId = Config.osAppId;
            var osAuth = Config.osAuth;
            string Deviceid = "7a556b65-1701-44b2-a5a1-b54ad7334604";
            int DeviceType = 2;
            output = nNotificationService.PushTeacherAppSBNoti(sBModel.Description, "2", osAppId.ToString(), osAuth.ToString(), string.Join("\", \"", Deviceid), DeviceType, sbm.ParentId, sbm.ChildId, sBModel.Schoolid, sbm.SectionId, sbm.StandardId, sbm.Subject, messageid, mask);
                if (output == "null")
                {
                    return (new
                    {
                        Message = "Failed to send notification",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
                else
                {
                    return (new
                    {
                        Message = "Message not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
            }
            return (new
            {
                Message = "Sucessfully sent the notification",
                StatusCode = HttpStatusCode.OK
            });


        }
        private List<Devices> GetDeviceid(int Parentid)
        {
            List<Devices> Device = new List<Devices>();
            var objRes = db.Appuserdevices.Where(x => x.Appuserid.Equals(Parentid));
            if (objRes != null)
            {
                try
                {
                    foreach (var item in objRes)
                    {
                        Device.Add(new Devices
                        {
                            Deviceid = item.Deviceid,
                            DeviceType = (int)item.Devicetype
                        });
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    string a = ex.Message;
                }
            }
            return Device;
        }

        private async Task<string> UpdateSBPostComments(int schoolid, int messageid, bool IsParentReplied)
        {
            try
            {
                var parentreplied = await GetEntityIDForUpdate(messageid);

                if (parentreplied != null)
                {
                    if (IsParentReplied == true)
                    {
                        // parentreplied.Isparentreplied = IsParentReplied;
                        parentreplied.Isparentreplied = true; //2024/4/2 
                        parentreplied.Isstaffreplied = false;//2024/4/2 
                        parentreplied.Modifieddate = DateTime.Now;
                        parentreplied.Didread = false;
                        parentreplied.Commentscount += 1;
                        //await this.UpdateEntity(parentreplied);
                        db2.TSoundingboardmessages.Update(parentreplied);
                        await db2.SaveChangesAsync();
                        return ("Ok");
                    }
                    else if (IsParentReplied == false)
                    {
                        //parentreplied.Isparentreplied = IsParentReplied;
                        parentreplied.Isparentreplied = false; //2024/4/2 
                        parentreplied.Isstaffreplied = true;
                        parentreplied.Modifieddate = DateTime.Now;
                        parentreplied.Didread = false;
                        parentreplied.Commentscount += 1;
                        //await this.UpdateEntity(parentreplied);
                        db2.TSoundingboardmessages.Update(parentreplied);
                        await db2.SaveChangesAsync();
                        return ("Ok");

                        //db.TSoundingboardmessages.Update(parentreplied);
                        //db.SaveChanges();
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        #endregion

        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }


        //APP services

        public async Task<object> PostSBPrivateAndPublicMessageByMessageIdV2ForAPI(PostCommentModel pModel, int appUserId)
        {
            try
            {
                string StdName = "";
                string SecName = "";
                bool smsupdate = false;
                var currentDate = DateTime.Now;
                var appUser = await db.MAppuserinfos.Where(w => w.Id == appUserId).FirstOrDefaultAsync();


                var mask = "GETTALKTIVE";

                var data1 = await db.TSoundingboardmessages.Where(w => w.Id == pModel.id).Include(w => w.Childinfo).FirstOrDefaultAsync();
                int SBschoolid = (int)await db.MStandardsectionmappings.Where(x => x.Id == data1.Standardsectionmappingid).Select(w => w.Branch.Schoolid).FirstOrDefaultAsync();

                string update = await UpdateSBPostComments(SBschoolid, pModel.id, pModel.IsParentReplied);
                var IsSBsms = db.MSchools.Where(w => w.Id == SBschoolid).FirstOrDefault();
                if ((bool)IsSBsms.Issbsms)       // Read from DB school table
                {
                    var Schooladmin = db3.MSchooluserroles.Where(x => x.Category.Roleid == 1 && x.Category.Role.Schoolid == SBschoolid).Select(w => w.Schooluserid);

                    //var Schooladmin = db.AppUsers.Where(w => w.SchoolId == appUser.SchoolId && w.IsSchoolAdminUser == true).FirstOrDefault();

                    foreach (var item in Schooladmin)
                    {
                        var phno = await db2.MSchooluserinfos.Where(x => x.Id == item).Select(w => w.Phonenumber).FirstOrDefaultAsync();
                        var ssm = await db.MStandardsectionmappings.Where(x => x.Id == data1.Standardsectionmappingid).FirstOrDefaultAsync();

                        if (ssm.Parentid == null)
                        {
                            StdName = await db2.MStandardsectionmappings.Where(x => x.Id == ssm.Parentid).Select(w => w.Name).FirstOrDefaultAsync();
                            SecName = null;
                        }
                        else
                        {
                            StdName = await db2.MStandardsectionmappings.Where(x => x.Id == ssm.Parentid).Select(w => w.Name).FirstOrDefaultAsync();
                            SecName = ssm.Name;
                        }

                        // var messages = "You have a new Sounding Board message from " + data1.Childinfo.Firstname + " of " + StdName + " " + SecName + ". View message here: https://yara-schoolportal.azurewebsites.net/#/soundingboard";
                        var messages = "You have a new Sounding Board message from " + data1.Childinfo.Firstname + " of " + StdName + " " + SecName + ". View message here: https://talktiveparents.azurewebsites.net/#/school/soundingboard";


                        Task smsu = Task.Run(() => MSMSService.SendSingleSMS(phno, messages, mask));
                        if (!smsu.IsCompleted)
                        {
                            smsupdate = true;
                        }
                    }
                }
                if (update != null && smsupdate == true)
                {
                    return (1);
                }
                else
                {
                    return (new
                    {
                        Message = "Failed to update/send soundingboard message",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
            }
            catch (Exception ex)
            {
                return (new
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

        }

        public async Task<object> GetSBPrivateAndPublicMessageV2ForAPI(string Type, int schoolid, int appuserid, int parentid, int Childid, int PageSize, int pageNumber)
        {
            try
            {
                List<SoundingboardmessagePublicAndPrivate> SBMessage = new List<SoundingboardmessagePublicAndPrivate>();
                string Type1 = Type.ToLower();
                int iType = 0;
                if (Type1 == "private" || Type1 == "0")
                {
                    iType = 0;
                }
                else if (Type1 == "public" || Type1 == "1")
                {
                    iType = 1;
                }

                if (iType == 1 || iType == 0)
                {
                    var appUserId = appuserid;
                    //var appUserId = 1; // only for testing
                    var res = db.TSoundingboardmessages.Where(x => x.Appuserinfoid == parentid && x.Childinfoid == Childid).Include(x => x.Category.Role).Include(x => x.Appuserinfo).Include(x => x.Childinfo);
                    if (res != null)
                    {
                        foreach (var item in res)
                        {
                            if (appUserId == item.Appuserinfoid)
                            {
                                SoundingboardmessagePublicAndPrivate sbMsg = new SoundingboardmessagePublicAndPrivate();
                                sbMsg.Id = item.Id;
                                sbMsg.ParentId = item.Appuserinfoid;
                                sbMsg.ChildId = item.Childinfoid;
                                sbMsg.CategoryKey = item.Categoryid;
                                sbMsg.CategoryName = item.Category.Name;
                                sbMsg.ParentName = item.Appuserinfo.Firstname;
                                sbMsg.ChildName = item.Childinfo.Firstname;

                                sbMsg.Relation = await db2.MParentchildmappings.Where(x => x.Appuserid == item.Appuserinfoid && x.Childid == item.Childinfoid).Select(w => w.Relationtype.Type).FirstOrDefaultAsync();

                                var ssm = await db2.MStandardsectionmappings.Where(x => x.Id == item.Standardsectionmappingid).FirstOrDefaultAsync();

                                if (ssm != null && ssm.Parentid == null)
                                {
                                    sbMsg.StandardId = item.Standardsectionmappingid;
                                    sbMsg.SectionId = 0;
                                }
                                else
                                {
                                    sbMsg.StandardId = ssm.Parentid;
                                    sbMsg.SectionId = item.Standardsectionmappingid;
                                }

                                sbMsg.Subject = item.Subject;
                                sbMsg.Description = item.Description;
                                sbMsg.CommentsCount = item.Commentscount;
                                //sbMsg.LikesCount = item.LikesCount;// not there
                                sbMsg.Attachment = item.Attachments;
                                //sbMsg.AttachmentCount = item.AttachmentCount; //not there
                                sbMsg.IsStaffReplied = item.Isstaffreplied;
                                //sbMsg.IsParentReplied = true;
                                sbMsg.IsParentReplied = item.Isparentreplied; //2/4/2024
                                sbMsg.Schoolid = item.Category.Role.Schoolid;

                                DateTime currentTime = (DateTime)item.Createddate;
                                DateTime x30MinsLater = currentTime.AddHours(5);
                                x30MinsLater = x30MinsLater.AddMinutes(30);
                                sbMsg.CreatedDate = x30MinsLater;

                                DateTime Modifieddate = (DateTime)item.Modifieddate;
                                DateTime x30MinsLatermodify = Modifieddate.AddHours(5);
                                x30MinsLatermodify = x30MinsLatermodify.AddMinutes(30);
                                sbMsg.UpdatedDate = x30MinsLatermodify;

                                // sbMsg.CreatedDate = item.Createddate;
                                //sbMsg.UpdatedDate = item.Modifieddate;
                                sbMsg.DidRead = item.Didread;
                                sbMsg.IsActive = item.Statusid;
                                sbMsg.Type = iType.ToString();
                                //  CreateDatelong = Timestamp.DateToTimestamp(Convert.ToDateTime(item.CreatedDate)),
                                // UpdateDatelong = Timestamp.DateToTimestamp(Convert.ToDateTime(item.CreatedDate)),
                                SBMessage.Add(sbMsg);
                            }
                            else
                            {
                                return (new
                                {
                                    message = "Userid does not match",
                                    StatusCode = HttpStatusCode.NotFound
                                });
                            }
                        }
                    }
                    else
                    {
                        return (new
                        {
                            message = "SB message not found",
                            StatusCode = HttpStatusCode.NotFound
                        });
                    }

                    var orderedSbMessages = SBMessage.OrderByDescending(w => w.UpdatedDate);
                    int count = orderedSbMessages.Count();
                    int CurrentPage = pageNumber;
                    int TotalCount = count;
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                    var items = orderedSbMessages.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                    var previousPage = CurrentPage > 1 ? "Yes" : "No";
                    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                    var obj = new
                    {
                        TotalPages = TotalPages,
                        items = items
                    };

                    if (SBMessage.Count > 0)
                    {
                        return (obj);
                    }
                    else
                    {
                        return (new
                        {
                            message = "SB message not found",
                            StatusCode = HttpStatusCode.NotFound
                        });
                    }
                }
                else
                {
                    return (new
                    {
                        message = "Input a Type",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
            }
            catch (Exception ex)
            {

                return (ex.Message);
            }
        }

        public async Task<object> PostSBMessageV2ForAPI(SoundingboardmessagePublicAndPrivate message, int appuserid)
        {
            int iType = 0;
            string Type1 = message.Type.ToLower();
            if (Type1 == "private" || Type1 == "0")
            {
                iType = 0;
            }
            else if (Type1 == "public" || Type1 == "1")
            {
                iType = 1;
            }
            else
            {
                return (0);
            }

            string StdName = "";
            string SecName = "";
            string output = "";
            bool smsupdate = false;
            // message.Type 
            List<ParentChannel> Pushdata = new List<ParentChannel>();

            var appUserId = appuserid;

            var appUser = await db.MAppuserinfos.Where(w => w.Id == appUserId).FirstOrDefaultAsync();
            var childid = await db.MParentchildmappings.Where(x => x.Appuserid == appUserId).Select(m => m.Childid).FirstOrDefaultAsync();
            var parentschoolid = await db.MChildschoolmappings.Where(x => x.Childid == childid).Select(x => x.Standardsectionmapping.Branch.Schoolid).FirstOrDefaultAsync();

            var mask = "GETTALKTIVE";

            try
            {
                TSoundingboardmessage smessage = new TSoundingboardmessage();
                smessage.Appuserinfoid = message.ParentId;
                smessage.Childinfoid = message.ChildId;
                //smessage.Category.Name = message.CategoryName; // cant add
                var categorykey = db.MCategories.FirstOrDefault(w => w.Role.Schoolid == message.Schoolid && w.Name == message.CategoryName).Id;
                if (categorykey != 0)
                {
                    if (categorykey.CompareTo(message.CategoryKey) != 0)
                    {
                        // Do not match,
                        smessage.Categoryid = categorykey;
                    }
                    else
                    {
                        smessage.Categoryid = message.CategoryKey;
                    }
                }


                #region not present in current db
                //smessage.ParentName = message.ParentName;
                //smessage.ChildName = message.ChildName;
                //smessage.Relation = message.Relation;
                //smessage.StandardId = message.StandardId;
                //smessage.SectionId = message.SectionId;
                //smessage.AttachmentCount = message.AttachmentCount;
                //smessage.LikesCount = message.LikesCount;
                //smessage.Schoolid = message.Schoolid; 
                //smessage.Type = iType;
                #endregion

                smessage.Standardsectionmappingid = message.SectionId;
                smessage.Subject = message.Subject;
                smessage.Description = message.Description;
                smessage.Commentscount = message.CommentsCount;
                smessage.Attachments = message.Attachment;
                //smessage.Isparentreplied = message.IsParentReplied;
                smessage.Isparentreplied = true; //2024/4/2 sanduni
                //smessage.Isstaffreplied = message.IsStaffReplied;
                smessage.Isstaffreplied = false; //2024/4/2 sanduni
                smessage.Createddate = DateTime.Now;
                smessage.Modifieddate = DateTime.Now;
                //smessage.Didread = message.DidRead;
                smessage.Didread = false; //2024/4/2 sanduni
                smessage.Statusid = 1;
                smessage.Appuserinfoid = message.ParentId;
                smessage.Childinfoid = message.ChildId;

                //smessage.Groupid = ; // not present in old model

                var sbid = await AddEntity(smessage);
                SendSBMessageNotificationModel sbm = new SendSBMessageNotificationModel();
                sbm.Schoolid = message.Schoolid;
                sbm.ChildId = message.ChildId;
                sbm.ParentId = message.ParentId;
                sbm.DidRead = (bool)message.DidRead;
                sbm.UpdatedDate = message.UpdatedDate;
                sbm.IsParentReplied = (bool)message.IsParentReplied;
                sbm.Attachment = message.Attachment;
                sbm.CategoryName = message.CategoryName;
                sbm.ChildName = message.ChildName;
                sbm.CommentsCount = (int)message.CommentsCount;
                sbm.Description = message.Description;
                

                #region can be used if public messages are added later
                if (iType == 1)// type not present
                {
                    var osAppId = Config.osAppId;
                    var osAuth = Config.osAuth;
                    message.Description = message.Description.Length >= 160 ? message.Description.Substring(0, 160) : message.Description;

                    DbSet<MChildschoolmapping> oChildSchool = null;
                    DbSet<MParentchildmapping> oParentChildMappings = null;
                    if (oChildSchool == null)
                        oChildSchool = db.MChildschoolmappings;
                    var parents = oChildSchool.Where(w => w.Standardsectionmapping.Branch.Schoolid == message.Schoolid).Include(x => x.Child).ToList();


                    if (parents != null)
                    {

                        foreach (var child in parents)
                        {
                            if (oParentChildMappings == null)
                                oParentChildMappings = db.MParentchildmappings;
                            var _child = child;
                            var Opar = await oParentChildMappings.Where(w => w.Childid == child.Childid).Select(w => w.Appuserid).FirstOrDefaultAsync();

                            var setofferoptn = await db.MAppuserinfos.Where(w => w.Id == Opar).Select(w => w.Isofferoptedin).FirstOrDefaultAsync();
                            if ((bool)setofferoptn)
                            {

                                var device = db.Appuserdevices.Where(x => x.Appuserid == Opar);
                                if (device.Count() > 0)
                                {
                                    var ssm = db.MStandardsectionmappings.Where(x => x.Id == child.Standardsectionmappingid).FirstOrDefault();
                                    foreach (var devices in device)
                                    {
                                        output = nNotificationService.PushNoti1(message.Description, "2", osAppId, osAuth, string.Join("\", \"", devices.Deviceid), devices.Devicetype, child.Childid, message.Schoolid, ssm.Id, ssm.Parentid, child.Child.Genderid, message.Subject, sbid, mask);

                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                else
                {
                    var osAppId = Config.osAppId;
                    var osAuth = Config.osAuth;
                    message.Description = message.Description.Length >= 160 ? message.Description.Substring(0, 160) : message.Description;

                }

                var IsSBsms = await db.MSchools.Where(w => w.Id == parentschoolid).FirstOrDefaultAsync();


                if ((bool)IsSBsms.Issbsms)       // Read from DB school table
                {
                    var Schooladmin = db3.MSchooluserroles.Where(x => x.Category.Role.Rank == 1 && x.Category.Role.Schoolid == message.Schoolid).Select(w => w.Schooluserid);

                    //var Schooladmin = db.AppUsers.Where(w => w.SchoolId == appUser.SchoolId && w.IsSchoolAdminUser == true).FirstOrDefault();

                    foreach (var item in Schooladmin)
                    {
                        var phno = await db2.MSchooluserinfos.Where(x => x.Id == item).Select(w => w.Phonenumber).FirstOrDefaultAsync();

                        var ssm = await db.MStandardsectionmappings.Where(x => x.Id == message.SectionId).FirstOrDefaultAsync();

                        if (ssm.Parentid == null)
                        {
                            StdName = await db2.MStandardsectionmappings.Where(x => x.Id == ssm.Parentid).Select(w => w.Name).FirstOrDefaultAsync();
                            SecName = null;
                        }
                        else
                        {
                            StdName = await db2.MStandardsectionmappings.Where(x => x.Id == ssm.Parentid).Select(w => w.Name).FirstOrDefaultAsync();
                            SecName = ssm.Name;
                        }

                        var messages = "You have a new Sounding Board message from " + message.ChildName + " of " + StdName + " " + SecName + ". View message here: https://talkativeparents.azurewebsites.net/#/authentication/school-portal-login";


                        Task smsu = Task.Run(() => MSMSService.SendSingleSMS(phno, messages, mask));
                        
                        if (!smsu.IsCompleted)
                        {
                            smsupdate = true;
                        }
                    }
                }

                if (output != null && smsupdate == true)
                {
                   // var tempnotification = SendTeacherAppSBMessagePushNotification(sbm, sbid, message.Type);
                    return ("Success");
                }
                else
                {
                    return (new
                    {
                        message = "not posted or sent",
                        HttpStatusCode = HttpStatusCode.BadRequest
                    });
                }
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
