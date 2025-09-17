//using CommonUtility;
//using CommonUtility.RequestModels;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using net.openstack.Core.Domain;
//using net.openstack.Providers.Rackspace.Objects.LoadBalancers;
//using Repository;
//using Repository.DBContext;
//using Services.CommonService;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity.Core.Objects;
//using System.Data.Entity;
//using System.Data.Entity.Core.Objects.DataClasses;
//using System.Linq;
//using System.Net;
//using System.Runtime.Serialization;
//using System.Threading.Tasks;
//using static Services.MStandardsectionmappingService;
//using static System.Collections.Specialized.BitVector32;
//using Google.Apis.Drive.v3.Data;
//using System.Drawing.Printing;


//namespace Services
//{
//    public interface ITNoticeboardmappingService : ICommonService
//    {
//        Task<int> AddEntity(TNoticeboardmapping entity);
//        Task<TNoticeboardmapping> GetEntityIDForUpdate(int entityID);
//        Task<int> UpdateEntity(TNoticeboardmapping entity);
//        Task<IQueryable<GetParentsSP>> GetParentsNB(int schoolid, string standard, string section);
//        IQueryable<TNoticeboardmessage> GetSp_GetNoticeBoardMsgBySchoolIdAsync(int schoolid, int userid, int? pagesize, int? pagenumber);
//        IQueryable<TNoticeboardmessage> GetNoticeboardmessagesBySchoolID(int userid, int schoolid, string searchstring);
//        Task<object> GetSchoolMessagesBySchoolIdForAPI(int userId, int schoolId, int? standardId = null, int? sectionId = null, int? childId = null, string searchString = "", int PageSize = 10, int pageNumber = 1);


//    }
//    public class TNoticeboardmappingService : ITNoticeboardmappingService
//    {
//        private readonly IRepository<TNoticeboardmapping> repository;
//        private DbSet<TNoticeboardmapping> localDBSet;

//        private TpContext db = new TpContext();
//        private TpContext db1 = new TpContext();
//        private TpContext db2 = new TpContext();

//        private readonly TpContext tpContext;
//        private readonly IConfiguration configuration;

//        public TNoticeboardmappingService(IRepository<TNoticeboardmapping> repository, TpContext tpContext, IConfiguration configuration)
//        {
//            this.repository = repository;
//            this.tpContext = tpContext;
//            this.configuration = configuration;
//        }

//        public async Task<int> AddEntity(TNoticeboardmapping entity)
//        {
//            var temp = await this.repository.Insert(entity);
//            if (temp)
//            {
//                return entity.Id;
//            }
//            return 0;
//        }

//        private async Task AllEntityValue() => localDBSet = (DbSet<TNoticeboardmapping>)await this.repository.GetAll();

//        private static Object Mapper(TNoticeboardmapping x) => new
//        {
//            x.Id,
//            x.Noticeboardmsgid,
//            x.Childid,
//            x.Appuserid,
//            Status = new
//            {
//                Status = x.Status != null ? x.Status.Name : string.Empty,
//                x.Statusid
//            }
//        };

//        private async Task<IQueryable<TNoticeboardmapping>> GetAllEntitiesPvt()
//        {
//            await AllEntityValue();
//            return this.localDBSet
//            .Include(x => x.CreatedbyNavigation)
//            .Include(x => x.ModifiedbyNavigation)
//            .Include(x => x.Status);
//        }

//        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

//        public async Task<TNoticeboardmapping> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

//        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

//        public async Task<int> UpdateEntity(TNoticeboardmapping entity)
//        {
//            var temp = await this.repository.Update(entity);
//            if (temp)
//            {
//                return entity.Id;
//            }
//            return 0;
//        }

//        public async Task<IQueryable<GetParentsSP>> GetParentsNB(int schoolid, string standard, string section)
//        {
//            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                var res = new List<GetParentsSP>();
//                connection.Open();
//                SqlCommand command = new SqlCommand(ApplicationConstants.GetParentSP, connection);
//                command.CommandType = CommandType.StoredProcedure;
//                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
//                command.Parameters["@schoolid"].Value = schoolid;
//                command.Parameters.Add(new SqlParameter("@standard", SqlDbType.NVarChar));
//                command.Parameters["@standard"].Value = standard;
//                command.Parameters.Add(new SqlParameter("@section", SqlDbType.NVarChar));
//                command.Parameters["@section"].Value = section;
//                using (SqlDataReader reader = command.ExecuteReader())
//                {
//                    if (reader.HasRows)
//                    {
//                        while (reader.Read())
//                        {
//                            await Task.Run(() => res.Add(new GetParentsSP()
//                            {
//                                Parentid = reader["id"] != DBNull.Value ? (int?)reader["id"] : null,
//                                ParentName = reader["ParentName"].ToString(),
//                                RelationId = reader["RelationId"] != DBNull.Value ? (int?)reader["RelationId"] : null,
//                                ChildId = reader["ChildId"] != DBNull.Value ? (int?)reader["ChildId"] : null,
//                                ChildSchoolMappingId = reader["ChildSchoolMappingId"] != DBNull.Value ? (int?)reader["ChildSchoolMappingId"] : null,
//                                ChildName = reader["ChildName"].ToString(),
//                                ChildEmail = reader["ChildEmail"].ToString(),
//                                SectionId = reader["SectionId"] != DBNull.Value ? (int?)reader["SectionId"] : null,
//                                SectionName = reader["SectionName"].ToString(),
//                                StandardId = reader["StandardId"] != DBNull.Value ? (int?)reader["StandardId"] : null,
//                                StandardName = reader["StandardName"].ToString(),
//                                RegistrationNumber = reader["RegistrationNumber"].ToString(),
//                                StatusId = reader["StatusId"] != DBNull.Value ? (int?)reader["StatusId"] : null, // 27/2/2024 Sanduni
//                            }));

//                        }
//                    }
//                }
//                return res.AsQueryable();
//            }
//        }


//        public IQueryable<TNoticeboardmessage> GetNoticeboardmessagesBySchoolID(int userid, int schoolid, string searchstring)
//        {
//            try
//            {
//                var roleid = tpContext.MRoles.Where(x => x.Schoolid.Equals(schoolid) && x.Statusid == 1 && x.Rank.Equals(1)).FirstOrDefault();//sanduni 15/402024
//                var categoryIds = tpContext.MCategories.Where(x => x.Roleid.Equals(roleid.Id) && x.Statusid == 1).Select(x => x.Id).ToList();
//                var status = tpContext.MSchooluserroles.Any(x => categoryIds.Contains((int)x.Categoryid) && x.Statusid == 1 && x.Schooluserid.Equals(userid));

//                string dateString = "2024-01-01 00:00:00";
//                string format = "yyyy-MM-dd HH:mm:ss";

//                DateTime createddatenb = DateTime.ParseExact(dateString, format, System.Globalization.CultureInfo.InvariantCulture);
                
//                if (status != false) 
//                {
//                   var nbmsg = db2.TNoticeboardmessages.Where(x => x.Branch.Schoolid.Equals(schoolid) && x.Statusid == 1  && x.Createddate > createddatenb);
//                    if (nbmsg.Count() > 0)
//                    {
//                        return nbmsg.AsQueryable();
//                    }
//                    else
//                    {
//                        return Enumerable.Empty<TNoticeboardmessage>().AsQueryable();
//                    }
//                }
//                else
//                {
//                    var nbmsg = db2.TNoticeboardmessages.Where(x => x.Schooluserid.Equals(userid) && x.Branch.Schoolid.Equals(schoolid) && x.Statusid == 1 && ((x.Message.Contains(searchstring)) || (x.Sms.Contains(searchstring)) || (x.Subject.Contains(searchstring)))).Include(x => x.Standardsectionmapping).Include(x => x.Branch);
//                    if (nbmsg.Count() > 0)
//                    {
//                        return nbmsg.AsQueryable();
//                    }
//                    else
//                    {
//                        return Enumerable.Empty<TNoticeboardmessage>().AsQueryable();
//                    }
//                }

//                //var nbmsg = tpContext.TNoticeboardmessages.Where(x => x.Schooluserid.Equals(userid) && x.Branch.Schoolid.Equals(schoolid) && x.Statusid == 1);

//                //var nbmsg = tpContext.TNoticeboardmessages.Where(x => x.Schooluserid.Equals(userid) && x.Branch.Schoolid.Equals(schoolid) && x.Statusid == 1 && ((x.Message.Contains(searchstring)) || (x.Sms.Contains(searchstring)) || (x.Subject.Contains(searchstring)))).Include(x => x.Standardsectionmapping).Include(x => x.Branch);
                
//            }
//            catch (Exception ex)
//            {

//                throw ex;
//            }

//        }


//        //public async Task<object> GetSchoolMessagesBySchoolIdForAPIOld(int userId, int schoolId, string searchString = "", int PageSize = 10, int pageNumber = 1)
//        //{
//        //    try
//        //    {
//        //        List<GetSchoolMessage> schoolmessage = new List<GetSchoolMessage>();
//        //        //ObjectParameter totalCount = new ObjectParameter("totalCount", typeof(int));
//        //        IQueryable<TNoticeboardmessage> objresult = GetNoticeboardmessagesBySchoolID(userId, schoolId, searchString);

//        //        if (objresult != null)
//        //        {

//        //            foreach (var item in objresult.ToList())
//        //            {
//        //                var schooluser = await this.db.MSchooluserinfos.Where(w => w.Id == item.Schooluserid).FirstOrDefaultAsync();

//        //                if (item.Standardsectionmappingid != null && item.IsParticularParent == true)
//        //                {
//        //                    var _count = 0;
//        //                    var ParentList = db.TNoticeboardmappings.Where(w => w.Noticeboardmsgid == item.Id).ToList();
//        //                    var schMsg = new GetSchoolMessage();
//        //                    schMsg.Id = item.Id;
//        //                    schMsg.Attachments = item.Attachments;
//        //                    schMsg.schoolusername = schooluser.Username; //need to check

//        //                    if (item.Standardsectionmapping.Parentid == null)
//        //                    {
//        //                        schMsg.StandardId = item.Standardsectionmappingid;
//        //                        schMsg.SectionId = null;
//        //                        schMsg.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
//        //                        schMsg.sectionname = null;
//        //                    }
//        //                    else
//        //                    {
//        //                        schMsg.StandardId = item.Standardsectionmapping.Parentid;
//        //                        schMsg.SectionId = item.Standardsectionmappingid;
//        //                        schMsg.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmapping.Parentid).FirstOrDefault().Name;
//        //                        schMsg.sectionname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
//        //                    }

//        //                    schMsg.Datetimesent = item.Createddate;
//        //                    schMsg.IsEntireSchool = item.IsEntireSchool;
//        //                    schMsg.IsParticularClass = item.IsParticularClass;
//        //                    schMsg.IsParticularSection = item.IsParticularSection;
//        //                    schMsg.Message = item.Message;
//        //                    schMsg.SchoolUserId = (int)item.Schooluserid;
//        //                    schMsg.Subject = item.Subject;
//        //                    schMsg.IsParticularParent = item.IsParticularParent;
//        //                    schMsg.SMS = item.Sms;
//        //                    schMsg.IsPriority = item.Ispriority;
//        //                    schMsg.OneSignal = item.OneSignal;


//        //                    schMsg.Parents = new List<ParticularParent>();
//        //                    foreach (var inner in ParentList)
//        //                    {
//        //                        var innerParent = await this.db.MAppuserinfos.Where(w => w.Id == inner.Appuserid).FirstOrDefaultAsync();
//        //                        var relationId = db.MParentchildmappings.Where(w => w.Appuserid == inner.Appuserid).FirstOrDefault().Relationtypeid;
//        //                        var perPar = new ParticularParent();
//        //                        if (innerParent != null && relationId != null)
//        //                        {

//        //                            perPar.ParentName = innerParent.Firstname + " " + innerParent.Lastname;
//        //                            perPar.Relation = relationId;

//        //                        }
//        //                        perPar.ParentId = inner.Appuserid;
//        //                        perPar.ChildId = inner.Childid;
//        //                        schMsg.Parents.Add(perPar);

//        //                    }
//        //                    _count = ParentList.Count();
//        //                    schMsg.ParCount = _count;
//        //                    schoolmessage.Add(schMsg);


//        //                }
//        //                else if (item.Standardsectionmappingid != null && item.IsParticularParent == false)
//        //                {
//        //                    var _count = 0;
//        //                    var allPar = new GetSchoolMessage();
//        //                    allPar.Id = item.Id;
//        //                    allPar.Attachments = item.Attachments;
//        //                    allPar.schoolusername = schooluser.Username; //need to check


//        //                    if (item.Standardsectionmapping.Parentid == null)
//        //                    {
//        //                        allPar.StandardId = item.Standardsectionmappingid;
//        //                        allPar.SectionId = null;
//        //                        allPar.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
//        //                        allPar.sectionname = null;
//        //                    }
//        //                    else
//        //                    {
//        //                        allPar.StandardId = item.Standardsectionmapping.Parentid;
//        //                        allPar.SectionId = item.Standardsectionmappingid;
//        //                        allPar.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmapping.Parentid).FirstOrDefault().Name;
//        //                        allPar.sectionname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
//        //                    }

//        //                    //allPar.StandardId = item.StandardId;
//        //                    //allPar.standardname = db.Standards.Where(w => w.Id == item.StandardId).FirstOrDefault().Name;
//        //                    //allPar.sectionname = db.Sections.Where(w => w.Id == item.SectionId).FirstOrDefault().Name;
//        //                    //allPar.SectionId = item.SectionId;


//        //                    allPar.Datetimesent = item.Createddate;
//        //                    allPar.IsEntireSchool = item.IsEntireSchool;
//        //                    allPar.IsParticularClass = item.IsParticularClass;
//        //                    allPar.IsParticularSection = item.IsParticularSection;
//        //                    allPar.Message = item.Message;
//        //                    allPar.SchoolUserId = (int)item.Schooluserid;

//        //                    allPar.Subject = item.Subject;
//        //                    allPar.IsParticularParent = item.IsParticularParent;
//        //                    allPar.SMS = item.Sms;
//        //                    allPar.IsPriority = item.Ispriority;
//        //                    allPar.OneSignal = item.OneSignal;
//        //                    //var _classes = db.Standards.Where(w => w.Id == item.StandardId).FirstOrDefault().Id;
//        //                    //var _sections = db.Sections.Where(w => w.Id == item.SectionId).FirstOrDefault().Id;
//        //                    var childmap = db.MChildschoolmappings.Where(w => w.Standardsectionmappingid.Equals(item.Standardsectionmappingid)).ToList();
//        //                    foreach (var child in childmap)
//        //                    {
//        //                        var _chinfo = db.MChildinfos.Where(w => w.Id.Equals(child.Childid)).FirstOrDefault();
//        //                        var _parid = db2.MParentchildmappings.Where(w => w.Childid.Equals(_chinfo.Id)).FirstOrDefault().Appuserid;
//        //                        var _parent = db.MAppuserinfos.Where(w => w.Id.Equals(_parid)).Count();
//        //                        _count = _count + _parent;
//        //                    }
//        //                    allPar.ParCount = _count;
//        //                    schoolmessage.Add(allPar);
//        //                }
//        //                else if (item.Standardsectionmappingid == null)
//        //                {
//        //                    var _count = 0;
//        //                    var entSchool = new GetSchoolMessage();
//        //                    entSchool.Id = item.Id;
//        //                    entSchool.Attachments = item.Attachments;

//        //                    //entSchool.StandardId = item.StandardId;
//        //                    //entSchool.SectionId = item.SectionId;
//        //                    entSchool.StandardId = item.Standardsectionmappingid;
//        //                    entSchool.SectionId = item.Standardsectionmappingid;

//        //                    entSchool.schoolusername = schooluser.Username; //need to check

//        //                    entSchool.Datetimesent = item.Createddate;
//        //                    entSchool.IsEntireSchool = item.IsEntireSchool;
//        //                    entSchool.IsParticularClass = item.IsParticularClass;
//        //                    entSchool.IsParticularSection = item.IsParticularSection;
//        //                    entSchool.Message = item.Message;
//        //                    entSchool.SchoolUserId = (int)item.Schooluserid;
//        //                    entSchool.Subject = item.Subject;
//        //                    entSchool.IsParticularParent = item.IsParticularParent;
//        //                    entSchool.SMS = item.Sms;
//        //                    entSchool.IsPriority = item.Ispriority;
//        //                    entSchool.OneSignal = item.OneSignal;
//        //                    var childmap = db.MChildschoolmappings.Where(w => w.Standardsectionmapping.Branch.Schoolid == schoolId).ToList();
//        //                    foreach (var child in childmap)
//        //                    {
//        //                        var map = await this.db.MParentchildmappings.Where(w => w.Childid == child.Childid).FirstOrDefaultAsync();
//        //                        if (map != null)
//        //                        {
//        //                            var parmap = map.Appuserid;
//        //                            var parents = db.MAppuserinfos.Where(w => w.Id == parmap).Distinct().Count();
//        //                            _count = _count + parents;
//        //                        }

//        //                    }
//        //                    entSchool.ParCount = _count;
//        //                    schoolmessage.Add(entSchool);
//        //                }
//        //                else
//        //                {
//        //                    GetSchoolMessage def = new GetSchoolMessage();
//        //                    def.Id = item.Id;
//        //                    def.Attachments = item.Attachments;

//        //                    if (item.Standardsectionmapping.Parentid == null)
//        //                    {
//        //                        def.StandardId = item.Standardsectionmappingid;
//        //                        def.SectionId = null;
//        //                    }
//        //                    else
//        //                    {
//        //                        def.StandardId = item.Standardsectionmapping.Parentid;
//        //                        def.SectionId = item.Standardsectionmappingid;
//        //                    }
//        //                    def.schoolusername = schooluser.Username; //need to check
//        //                    def.Datetimesent = item.Createddate;

//        //                    def.IsEntireSchool = item.IsEntireSchool;
//        //                    def.IsParticularClass = item.IsParticularClass;
//        //                    def.IsParticularSection = item.IsParticularSection;
//        //                    def.Message = item.Message;
//        //                    def.SchoolUserId = (int)item.Schooluserid;
//        //                    def.Subject = item.Subject;
//        //                    def.IsParticularParent = item.IsParticularParent;
//        //                    def.SMS = item.Sms;
//        //                    def.IsPriority = item.Ispriority;
//        //                    def.OneSignal = item.OneSignal;
//        //                    schoolmessage.Add(def);

//        //                }

//        //            }
//        //        }

//        //        int count = schoolmessage.Count;
//        //        int CurrentPage = pageNumber;
//        //        int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
//        //        var items = schoolmessage.OrderByDescending(x => x.Datetimesent).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
//        //        var previousPage = CurrentPage > 1 ? "Yes" : "No";
//        //        var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

//        //        var obj = new
//        //        {
//        //            TotalPages = TotalPages,
//        //            items = items
//        //        };
//        //        return (obj);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return ex.ToString();
//        //    }
//        //}
//        public async Task<IQueryable<TNoticeboardmessage>> GetSp_GetNoticeBoardMsgBySchoolIdAsync(int schoolid, int userid, int? pagesize,int? pagenumber)
//        {
//            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                var res = new List<TNoticeboardmessage>();
//                connection.Open();
//                SqlCommand command = new SqlCommand(ApplicationConstants.GetNoticeBoardMsgBySchoolId, connection);
//                command.CommandType = CommandType.StoredProcedure;
//                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
//                command.Parameters["@schoolid"].Value = schoolid;
//                command.Parameters.Add(new SqlParameter("@userid", SqlDbType.NVarChar));
//                command.Parameters["@userid"].Value = userid;
//                command.Parameters.Add(new SqlParameter("@pagesize", SqlDbType.NVarChar));
//                command.Parameters["@pagesize"].Value = pagesize;
//                command.Parameters.Add(new SqlParameter("@pagenumber", SqlDbType.NVarChar));
//                command.Parameters["@pagenumber"].Value = pagenumber;
//                using (SqlDataReader reader = command.ExecuteReader())
//                {
//                    if (reader.HasRows)
//                    {
//                        while (reader.Read())
//                        {
//                            await Task.Run(() => res.Add(new TNoticeboardmessage()
//                            {
//                                Id = (int)reader["Id"],
//                                Attachments = reader["attachments"].ToString(),
//                                Standardsectionmappingid = reader["standardsectionmappingid"] != DBNull.Value ? (int?)reader["standardsectionmappingid"] : null,
//                                IsEntireSchool = reader["IsEntireSchool"] != DBNull.Value ? (bool?)reader["IsEntireSchool"] : null,
//                                IsParticularSection = reader["IsParticularSection"] != DBNull.Value ? (bool?)reader["IsParticularSection"] : null,
//                                IsParticularClass = reader["IsParticularClass"] != DBNull.Value ? (bool?)reader["IsParticularClass"] : null,
//                                IsParticularParent = reader["IsParticularParent"] != DBNull.Value ? (bool?)reader["IsParticularParent"] : null,
//                                Message = reader["Message"].ToString(),
//                                Subject = reader["Subject"].ToString(),
//                                OneSignal = reader["OneSignal"].ToString(),
//                                Sms = reader["SMS"].ToString(),
//                                Ispriority = reader["IsPriority"] != DBNull.Value ? (bool?)reader["IsPriority"] : null,
//                                Schooluserid = reader["SchoolUserId"] != DBNull.Value ? (int?)reader["SchoolUserId"] : null, // 27/2/2024 Sanduni
//                            }));

//                        }
//                    }
//                }
//                return res.AsQueryable();
//            }
//        }
//        public async Task<object> GetSchoolMessagesBySchoolIdForAPI(int userId, int schoolId, int? standardId = null, int? sectionId = null, int? childId = null, string searchString = "", int PageSize = 10, int pageNumber = 1)
//        {
//            try
//            {
//                List<GetSchoolMessage> schoolmessage = new List<GetSchoolMessage>();
//                //ObjectParameter totalCount = new ObjectParameter("totalCount", typeof(int));
//                IQueryable<TNoticeboardmessage> objresult = GetNoticeboardmessagesBySchoolID(userId, schoolId, searchString);

//                if (standardId.HasValue)
//                    objresult = objresult.Where(x => x.Standardsectionmapping.Parentid == standardId);

//                if (sectionId.HasValue)
//                    objresult = objresult.Where(x => x.Standardsectionmappingid == sectionId);

//                if (childId.HasValue)
//                    objresult = objresult.Where(x => x.TNoticeboardmappings.Any(p => p.Childid == childId.Value));


//                int count2 = objresult.Count();

//                if (objresult != null)
//                {

//                    foreach (var item in objresult)
//                    {
//                        var Branchid = await this.db.MStandardsectionmappings.Where(c => c.Id == item.Standardsectionmappingid).Select(t => t.Branchid).FirstOrDefaultAsync();
//                        var Schoolid = await this.db.MBranches.Where(c => c.Id == Branchid).Select(t => t.Schoolid).FirstOrDefaultAsync();
//                        var academicyearid = await this.db.MAcademicyeardetails.Where(c => c.SchoolId == Schoolid && c.Currentyear == 1).Select(t => t.Id).FirstOrDefaultAsync();
//                        var schooluser = await this.db.MSchooluserinfos.Where(w => w.Id == item.Schooluserid).FirstOrDefaultAsync();

//                        if (item.Standardsectionmappingid != null && item.IsParticularParent == true)
//                        {
//                            var _count = 0;
//                            //var ParentList = db.TNoticeboardmappings.Where(w => w.Noticeboardmsgid == item.Id).ToList(); //sanduni

//                            // Assuming `item.Id` is the Noticeboard message ID
//                            var ParentList = db.TNoticeboardmappings
//                                .Where(w => w.Noticeboardmsgid == item.Id)
//                                .Select(w => new
//                                {
//                                    // Select only necessary fields
//                                    w.Id,
//                                    w.Appuserid,
//                                    w.Childid// Add other fields as needed
//                                })
//                                .ToList();




//                            var schMsg = new GetSchoolMessage();
//                            schMsg.Id = item.Id;
//                            schMsg.Attachments = item.Attachments;
//                            schMsg.schoolusername = null; //need to check

//                            if (item.Standardsectionmapping == null)
//                            {
//                                //schMsg.StandardId = item.Standardsectionmappingid;
//                                schMsg.StandardId = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Parentid; //21/4/2024 Sanduni
//                                                                                                                                                             //schMsg.SectionId = null;
//                                schMsg.SectionId = item.Standardsectionmappingid; //21/4/2024 Sanduni
//                                schMsg.standardname = db.MStandardsectionmappings.Where(w => w.Id == schMsg.StandardId).FirstOrDefault().Name;  //21/4/2024 Sanduni
//                                                                                                                                                //schMsg.sectionname = null;
//                                schMsg.sectionname = db.MStandardsectionmappings.Where(w => w.Id == schMsg.SectionId).FirstOrDefault().Name; ; //21/4/2024 Sanduni
//                            }
//                            else
//                            {
//                                schMsg.StandardId = item.Standardsectionmapping.Parentid;
//                                schMsg.SectionId = item.Standardsectionmappingid;
//                                schMsg.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmapping.Parentid).FirstOrDefault().Name;
//                                schMsg.sectionname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
//                            }

//                            schMsg.Datetimesent = item.Createddate;
//                            schMsg.IsEntireSchool = item.IsEntireSchool;
//                            schMsg.IsParticularClass = item.IsParticularClass;
//                            schMsg.IsParticularSection = item.IsParticularSection;
//                            schMsg.Message = item.Message;
//                            schMsg.SchoolUserId = (int)item.Schooluserid;
//                            schMsg.Subject = item.Subject;
//                            schMsg.IsParticularParent = item.IsParticularParent;
//                            schMsg.SMS = item.Sms;
//                            schMsg.IsPriority = item.Ispriority;
//                            schMsg.OneSignal = item.OneSignal;


//                            schMsg.Parents = new List<ParticularParent>();



//                            foreach (var inner in ParentList)
//                            {
//                                //var childdetails = db.MChildschoolmappings.Where(w => w.Childid.Equals(inner.Childid) && w.AcademicYearId.Equals(academicyearid)).ToList()
//                                var childDetailsCount = db.MChildschoolmappings.Count(w => w.Childid.Equals(inner.Childid) && w.AcademicYearId.Equals(academicyearid));


//                                if (childDetailsCount > 0)
//                                {
//                                    if (inner.Appuserid == null)
//                                    {
//                                        var innerParent = "";
//                                        var innerChild = "";
//                                        var relationId = "";
//                                        var perPar = new ParticularParent();
//                                        if (innerParent != null && relationId != null)
//                                        {

//                                            perPar.ParentName = "";
//                                            perPar.Relation = 1;

//                                        }
//                                        perPar.ParentId = null;
//                                        perPar.ChildId = null;
//                                        perPar.ChildName = null;
//                                        schMsg.Parents.Add(perPar);
//                                    }
//                                    else
//                                    {

//                                        var innerParent = await this.db1.MAppuserinfos.FirstOrDefaultAsync(w => w.Id == inner.Appuserid);
//                                        var innerChild = await this.db1.MChildinfos.FirstOrDefaultAsync(w => w.Id == inner.Childid);
//                                        var relationId = db1.MParentchildmappings.FirstOrDefault(w => w.Appuserid == inner.Appuserid)?.Relationtypeid;
//                                        var perPar = new ParticularParent();
//                                        if (innerParent != null && relationId != null)
//                                        {

//                                            perPar.ParentName = innerParent.Firstname + " " + innerParent.Lastname;
//                                            perPar.Relation = relationId;

//                                        }
//                                        perPar.ParentId = inner.Appuserid;
//                                        perPar.ChildId = inner.Childid;
//                                        perPar.ChildName = innerChild.Firstname + " " + innerChild.Lastname;
//                                        schMsg.Parents.Add(perPar);
//                                    }
//                                }
//                            }
//                            _count = ParentList.Count();
//                            schMsg.ParCount = _count;
//                            schoolmessage.Add(schMsg);


//                        }
//                        else if (item.Standardsectionmappingid != null && item.IsParticularParent == false)
//                        {
//                            var _count = 0;
//                            var allPar = new GetSchoolMessage();
//                            allPar.Id = item.Id;
//                            allPar.Attachments = item.Attachments;
//                            allPar.schoolusername = null; //need to check


//                            if (item.Standardsectionmapping == null)
//                            {
//                                // allPar.StandardId = item.Standardsectionmappingid;
//                                allPar.StandardId = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Parentid; //21/4/2024 Sanduni
//                                                                                                                                                             // allPar.SectionId = null;
//                                allPar.SectionId = item.Standardsectionmappingid; //21/4/2024 Sanduni
//                                allPar.standardname = db.MStandardsectionmappings.Where(w => w.Id == allPar.StandardId).FirstOrDefault().Name;  //21/4/2024 Sanduni
//                                                                                                                                                // allPar.standardname = db.MStandardsectionmappings.FirstOrDefault(w => w.Id == item.Standardsectionmappingid)?.Name;
//                                                                                                                                                // allPar.sectionname = null;
//                                allPar.sectionname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name; //21/4/2024 Sanduni
//                            }
//                            else
//                            {
//                                allPar.StandardId = item.Standardsectionmapping.Parentid;
//                                allPar.SectionId = item.Standardsectionmappingid;
//                                allPar.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmapping.Parentid).FirstOrDefault().Name;
//                                allPar.sectionname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
//                            }

//                            //allPar.StandardId = item.StandardId;
//                            //allPar.standardname = db.Standards.Where(w => w.Id == item.StandardId).FirstOrDefault().Name;
//                            //allPar.sectionname = db.Sections.Where(w => w.Id == item.SectionId).FirstOrDefault().Name;
//                            //allPar.SectionId = item.SectionId;


//                            allPar.Datetimesent = item.Createddate;
//                            allPar.IsEntireSchool = item.IsEntireSchool;
//                            allPar.IsParticularClass = item.IsParticularClass;
//                            allPar.IsParticularSection = item.IsParticularSection;
//                            allPar.Message = item.Message;
//                            allPar.SchoolUserId = (int)item.Schooluserid;

//                            allPar.Subject = item.Subject;
//                            allPar.IsParticularParent = item.IsParticularParent;
//                            allPar.SMS = item.Sms;
//                            allPar.IsPriority = item.Ispriority;
//                            allPar.OneSignal = item.OneSignal;
//                            //var _classes = db.Standards.Where(w => w.Id == item.StandardId).FirstOrDefault().Id;
//                            //var _sections = db.Sections.Where(w => w.Id == item.SectionId).FirstOrDefault().Id;


//                            //var childmap = db.MChildschoolmappings.Where(w => w.Standardsectionmappingid.Equals(item.Standardsectionmappingid) && w.AcademicYearId.Equals(academicyearid)).ToList();
//                            //foreach (var child in childmap)
//                            //{
//                            //    var _chinfo = db1.MChildinfos.FirstOrDefault(w => w.Id.Equals(child.Childid));
//                            //    var _parid = db1.MParentchildmappings.FirstOrDefault(w => w.Childid.Equals(_chinfo.Id))?.Appuserid;
//                            //    var _parent = db1.MAppuserinfos.Where(w => w.Id.Equals(_parid)).Count();
//                            //    _count = _count + _parent;
//                            //}
//                            // Fetch all child mappings and related child information in a single query
//                            var childMappings = db.MChildschoolmappings.Where(w => w.Standardsectionmappingid.Equals(item.Standardsectionmappingid) && w.AcademicYearId.Equals(academicyearid)).Select(m => new
//                            {
//                                ChildId = m.Childid,
//                                ParentUserId = db.MParentchildmappings
//            .Where(pcm => pcm.Childid.Equals(m.Childid))
//            .Select(pcm => pcm.Appuserid)
//            .FirstOrDefault()
//                            })
//    .ToList();

//                            // Fetch count of distinct parent users in a single query using in-memory operations
//                            var parentUserIds = childMappings.Select(cm => cm.ParentUserId).ToList();
//                            var parentCount = db.MAppuserinfos
//                                .Where(w => parentUserIds.Contains(w.Id))
//                                .Count();


//                            _count += parentCount;

//                            allPar.ParCount = _count;
//                            schoolmessage.Add(allPar);
//                        }
//                        else if (item.Standardsectionmappingid == null)
//                        {
//                            var _count = 0;
//                            var entSchool = new GetSchoolMessage();
//                            entSchool.Id = item.Id;
//                            entSchool.Attachments = item.Attachments;

//                            //entSchool.StandardId = item.StandardId;
//                            //entSchool.SectionId = item.SectionId;

//                            entSchool.StandardId = item.Standardsectionmappingid;
//                            // entSchool.StandardId = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Parentid; //sanduni
//                            entSchool.SectionId = item.Standardsectionmappingid;

//                            entSchool.schoolusername = null; //need to check

//                            entSchool.Datetimesent = item.Createddate;
//                            entSchool.IsEntireSchool = item.IsEntireSchool;
//                            entSchool.IsParticularClass = item.IsParticularClass;
//                            entSchool.IsParticularSection = item.IsParticularSection;
//                            entSchool.Message = item.Message;
//                            entSchool.SchoolUserId = (int)item.Schooluserid;
//                            entSchool.Subject = item.Subject;
//                            entSchool.IsParticularParent = item.IsParticularParent;
//                            entSchool.SMS = item.Sms;
//                            entSchool.IsPriority = item.Ispriority;
//                            entSchool.OneSignal = item.OneSignal;

//                            //var childmap = db.MChildschoolmappings.Where(w => w.Standardsectionmapping.Branch.Schoolid == schoolId && w.AcademicYearId.Equals(academicyearid)).ToList();
//                            //foreach (var child in childmap)
//                            //{
//                            //    var map = await this.db1.MParentchildmappings.Where(w => w.Childid == child.Childid).FirstOrDefaultAsync();
//                            //    if (map != null)
//                            //    {
//                            //        var parmap = map.Appuserid;
//                            //        var parents = db1.MAppuserinfos.Where(w => w.Id == parmap).Distinct().Count();
//                            //        _count = _count + parents;
//                            //    }

//                            //}
//                            var childMappings = await db.MChildschoolmappings
//    .Include(w => w.Standardsectionmapping.Branch) // Include related entities to avoid lazy loading
//    .Where(w => w.Standardsectionmapping.Branch.Schoolid == schoolId && w.AcademicYearId.Equals(academicyearid))
//    .ToListAsync();

//                            foreach (var child in childMappings)
//                            {
//                                var parentMapping = await this.db1.MParentchildmappings
//                                    .Where(w => w.Childid == child.Childid)
//                                    .Select(w => w.Appuserid)
//                                    .FirstOrDefaultAsync();

//                                if (parentMapping != null)
//                                {
//                                    var parentCount = await db1.MAppuserinfos
//                                        .Where(w => w.Id == parentMapping)
//                                        .Select(w => w.Id)
//                                        .Distinct()
//                                        .CountAsync();

//                                    _count += parentCount;
//                                }
//                            }

//                            entSchool.ParCount = _count;
//                            schoolmessage.Add(entSchool);
//                        }
//                        else
//                        {
//                            GetSchoolMessage def = new GetSchoolMessage();
//                            def.Id = item.Id;
//                            def.Attachments = item.Attachments;

//                            if (item.Standardsectionmapping == null)
//                            {
//                                def.StandardId = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Parentid; //sanduni
//                                                                                                                                                          // def.StandardId = item.Standardsectionmappingid;
//                                                                                                                                                          //def.SectionId = null;
//                                def.SectionId = item.Standardsectionmappingid;
//                            }
//                            else
//                            {
//                                def.StandardId = item.Standardsectionmapping.Parentid;
//                                def.SectionId = item.Standardsectionmappingid;
//                            }
//                            def.schoolusername = null; //need to check
//                            def.Datetimesent = item.Createddate;

//                            def.IsEntireSchool = item.IsEntireSchool;
//                            def.IsParticularClass = item.IsParticularClass;
//                            def.IsParticularSection = item.IsParticularSection;
//                            def.Message = item.Message;
//                            def.SchoolUserId = (int)item.Schooluserid;
//                            def.Subject = item.Subject;
//                            def.IsParticularParent = item.IsParticularParent;
//                            def.SMS = item.Sms;
//                            def.IsPriority = item.Ispriority;
//                            def.OneSignal = item.OneSignal;
//                            schoolmessage.Add(def);

//                        }



//                    }
//                }

//                int count = schoolmessage.Count;
//                int CurrentPage = pageNumber;
//                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
//                var items = schoolmessage.OrderByDescending(x => x.Datetimesent).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
//                var previousPage = CurrentPage > 1 ? "Yes" : "No";
//                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

//                var obj = new
//                {
//                    TotalPages = TotalPages,
//                    items = items
//                };
//                return (obj);
//            }
//            catch (Exception ex)
//            {
//                return ex.ToString();
//            }
//        }







//        public Task<IQueryable<object>> GetEntityByName(string EntityName)
//        {
//            throw new NotImplementedException();
//        }

//    }
//    //    public async Task<object> GetSchoolMessagesBySchoolIdForAPI(int userId, int schoolId, int? standardId = null, int? sectionId = null, int? childId = null, string searchString = "", int PageSize = 10, int pageNumber = 1)
//    //    {
//    //        try
//    //        {
//    //            List<GetSchoolMessage> schoolmessage = new List<GetSchoolMessage>();
//    //            //ObjectParameter totalCount = new ObjectParameter("totalCount", typeof(int));
//    //            IQueryable<TNoticeboardmessage> objresult = GetNoticeboardmessagesBySchoolID(userId, schoolId, searchString);

//    //            if (standardId.HasValue)
//    //                objresult = objresult.Where(x => x.Standardsectionmapping.Parentid == standardId);

//    //            if (sectionId.HasValue)
//    //                objresult = objresult.Where(x => x.Standardsectionmappingid == sectionId);

//    //            if (childId.HasValue)
//    //                objresult = objresult.Where(x => x.TNoticeboardmappings.Any(p => p.Childid == childId.Value));


//    //            int count2 = objresult.Count();

//    //            if (objresult != null)
//    //            {

//    //                foreach (var item in objresult)
//    //                {
//    //                    var Branchid = await this.db.MStandardsectionmappings.Where(c => c.Id == item.Standardsectionmappingid).Select(t => t.Branchid).FirstOrDefaultAsync();
//    //                    var Schoolid = await this.db.MBranches.Where(c => c.Id == Branchid).Select(t => t.Schoolid).FirstOrDefaultAsync();
//    //                    var academicyearid = await this.db.MAcademicyeardetails.Where(c => c.SchoolId == Schoolid && c.Currentyear == 1).Select(t => t.Id).FirstOrDefaultAsync();
//    //                    var schooluser = await this.db.MSchooluserinfos.Where(w => w.Id == item.Schooluserid).FirstOrDefaultAsync();

//    //                    if (item.Standardsectionmappingid != null && item.IsParticularParent == true)
//    //                    {
//    //                        var _count = 0;
//    //                        //var ParentList = db.TNoticeboardmappings.Where(w => w.Noticeboardmsgid == item.Id).ToList(); //sanduni

//    //                        // Assuming `item.Id` is the Noticeboard message ID
//    //                        var ParentList = db.TNoticeboardmappings
//    //                            .Where(w => w.Noticeboardmsgid == item.Id)
//    //                            .Select(w => new
//    //                            {
//    //                                // Select only necessary fields
//    //                                w.Id,
//    //                                w.Appuserid ,
//    //                                w.Childid// Add other fields as needed
//    //                            })
//    //                            .ToList();




//    //                        var schMsg = new GetSchoolMessage();
//    //                        schMsg.Id = item.Id;
//    //                        schMsg.Attachments = item.Attachments;
//    //                        schMsg.schoolusername = null; //need to check

//    //                        if (item.Standardsectionmapping == null)
//    //                        {
//    //                            //schMsg.StandardId = item.Standardsectionmappingid;
//    //                            schMsg.StandardId = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Parentid; //21/4/2024 Sanduni
//    //                            //schMsg.SectionId = null;
//    //                            schMsg.SectionId = item.Standardsectionmappingid; //21/4/2024 Sanduni
//    //                            schMsg.standardname = db.MStandardsectionmappings.Where(w => w.Id == schMsg.StandardId).FirstOrDefault().Name;  //21/4/2024 Sanduni
//    //                            //schMsg.sectionname = null;
//    //                            schMsg.sectionname = db.MStandardsectionmappings.Where(w => w.Id == schMsg.SectionId).FirstOrDefault().Name; ; //21/4/2024 Sanduni
//    //                        }
//    //                        else
//    //                        {
//    //                            schMsg.StandardId = item.Standardsectionmapping.Parentid;
//    //                            schMsg.SectionId = item.Standardsectionmappingid;
//    //                            schMsg.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmapping.Parentid).FirstOrDefault().Name;
//    //                            schMsg.sectionname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
//    //                        }

//    //                        schMsg.Datetimesent = item.Createddate;
//    //                        schMsg.IsEntireSchool = item.IsEntireSchool;
//    //                        schMsg.IsParticularClass = item.IsParticularClass;
//    //                        schMsg.IsParticularSection = item.IsParticularSection;
//    //                        schMsg.Message = item.Message;
//    //                        schMsg.SchoolUserId = (int)item.Schooluserid;
//    //                        schMsg.Subject = item.Subject;
//    //                        schMsg.IsParticularParent = item.IsParticularParent;
//    //                        schMsg.SMS = item.Sms;
//    //                        schMsg.IsPriority = item.Ispriority;
//    //                        schMsg.OneSignal = item.OneSignal;


//    //                        schMsg.Parents = new List<ParticularParent>();



//    //                        foreach (var inner in ParentList)
//    //                        {
//    //                            //var childdetails = db.MChildschoolmappings.Where(w => w.Childid.Equals(inner.Childid) && w.AcademicYearId.Equals(academicyearid)).ToList()
//    //                            var childDetailsCount = db.MChildschoolmappings.Count(w => w.Childid.Equals(inner.Childid) && w.AcademicYearId.Equals(academicyearid));


//    //                            if (childDetailsCount > 0)
//    //                            {
//    //                                if (inner.Appuserid == null)
//    //                                {
//    //                                    var innerParent = "";
//    //                                    var innerChild = "";
//    //                                    var relationId = "";
//    //                                    var perPar = new ParticularParent();
//    //                                    if (innerParent != null && relationId != null)
//    //                                    {

//    //                                        perPar.ParentName = "";
//    //                                        perPar.Relation = 1;

//    //                                    }
//    //                                    perPar.ParentId = null;
//    //                                    perPar.ChildId = null;
//    //                                    perPar.ChildName = null;
//    //                                    schMsg.Parents.Add(perPar);
//    //                                }
//    //                                else
//    //                                {

//    //                                    var innerParent = await this.db1.MAppuserinfos.FirstOrDefaultAsync(w => w.Id == inner.Appuserid);
//    //                                    var innerChild = await this.db1.MChildinfos.FirstOrDefaultAsync(w => w.Id == inner.Childid);
//    //                                    var relationId = db1.MParentchildmappings.FirstOrDefault(w => w.Appuserid == inner.Appuserid)?.Relationtypeid;
//    //                                    var perPar = new ParticularParent();
//    //                                    if (innerParent != null && relationId != null)
//    //                                    {

//    //                                        perPar.ParentName = innerParent.Firstname + " " + innerParent.Lastname;
//    //                                        perPar.Relation = relationId;

//    //                                    }
//    //                                    perPar.ParentId = inner.Appuserid;
//    //                                    perPar.ChildId = inner.Childid;
//    //                                    perPar.ChildName = innerChild.Firstname + " " + innerChild.Lastname;
//    //                                    schMsg.Parents.Add(perPar);
//    //                                }
//    //                            }
//    //                        }
//    //                        _count = ParentList.Count();
//    //                        schMsg.ParCount = _count;
//    //                        schoolmessage.Add(schMsg);


//    //                    }
//    //                    else if (item.Standardsectionmappingid != null && item.IsParticularParent == false)
//    //                    {
//    //                        var _count = 0;
//    //                        var allPar = new GetSchoolMessage();
//    //                        allPar.Id = item.Id;
//    //                        allPar.Attachments = item.Attachments;
//    //                        allPar.schoolusername = null; //need to check


//    //                        if (item.Standardsectionmapping == null)
//    //                        {
//    //                            // allPar.StandardId = item.Standardsectionmappingid;
//    //                            allPar.StandardId = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Parentid; //21/4/2024 Sanduni
//    //                           // allPar.SectionId = null;
//    //                            allPar.SectionId = item.Standardsectionmappingid; //21/4/2024 Sanduni
//    //                            allPar.standardname = db.MStandardsectionmappings.Where(w => w.Id == allPar.StandardId).FirstOrDefault().Name;  //21/4/2024 Sanduni
//    //                           // allPar.standardname = db.MStandardsectionmappings.FirstOrDefault(w => w.Id == item.Standardsectionmappingid)?.Name;
//    //                           // allPar.sectionname = null;
//    //                            allPar.sectionname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name; //21/4/2024 Sanduni
//    //                        }
//    //                        else
//    //                        {
//    //                            allPar.StandardId = item.Standardsectionmapping.Parentid;
//    //                            allPar.SectionId = item.Standardsectionmappingid;
//    //                            allPar.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmapping.Parentid).FirstOrDefault().Name;
//    //                            allPar.sectionname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
//    //                        }

//    //                        //allPar.StandardId = item.StandardId;
//    //                        //allPar.standardname = db.Standards.Where(w => w.Id == item.StandardId).FirstOrDefault().Name;
//    //                        //allPar.sectionname = db.Sections.Where(w => w.Id == item.SectionId).FirstOrDefault().Name;
//    //                        //allPar.SectionId = item.SectionId;


//    //                        allPar.Datetimesent = item.Createddate;
//    //                        allPar.IsEntireSchool = item.IsEntireSchool;
//    //                        allPar.IsParticularClass = item.IsParticularClass;
//    //                        allPar.IsParticularSection = item.IsParticularSection;
//    //                        allPar.Message = item.Message;
//    //                        allPar.SchoolUserId = (int)item.Schooluserid;

//    //                        allPar.Subject = item.Subject;
//    //                        allPar.IsParticularParent = item.IsParticularParent;
//    //                        allPar.SMS = item.Sms;
//    //                        allPar.IsPriority = item.Ispriority;
//    //                        allPar.OneSignal = item.OneSignal;
//    //                        //var _classes = db.Standards.Where(w => w.Id == item.StandardId).FirstOrDefault().Id;
//    //                        //var _sections = db.Sections.Where(w => w.Id == item.SectionId).FirstOrDefault().Id;


//    //                        //var childmap = db.MChildschoolmappings.Where(w => w.Standardsectionmappingid.Equals(item.Standardsectionmappingid) && w.AcademicYearId.Equals(academicyearid)).ToList();
//    //                        //foreach (var child in childmap)
//    //                        //{
//    //                        //    var _chinfo = db1.MChildinfos.FirstOrDefault(w => w.Id.Equals(child.Childid));
//    //                        //    var _parid = db1.MParentchildmappings.FirstOrDefault(w => w.Childid.Equals(_chinfo.Id))?.Appuserid;
//    //                        //    var _parent = db1.MAppuserinfos.Where(w => w.Id.Equals(_parid)).Count();
//    //                        //    _count = _count + _parent;
//    //                        //}
//    //                        // Fetch all child mappings and related child information in a single query
//    //                        var childMappings = db.MChildschoolmappings.Where(w => w.Standardsectionmappingid.Equals(item.Standardsectionmappingid) && w.AcademicYearId.Equals(academicyearid)).Select(m => new
//    //{
//    //    ChildId = m.Childid,
//    //    ParentUserId = db.MParentchildmappings
//    //        .Where(pcm => pcm.Childid.Equals(m.Childid))
//    //        .Select(pcm => pcm.Appuserid)
//    //        .FirstOrDefault()
//    //})
//    //.ToList();

//    //                        // Fetch count of distinct parent users in a single query using in-memory operations
//    //                        var parentUserIds = childMappings.Select(cm => cm.ParentUserId).ToList();
//    //                        var parentCount = db.MAppuserinfos
//    //                            .Where(w => parentUserIds.Contains(w.Id))
//    //                            .Count();


//    //                        _count += parentCount;

//    //                        allPar.ParCount = _count;
//    //                        schoolmessage.Add(allPar);
//    //                    }
//    //                    else if (item.Standardsectionmappingid == null)
//    //                    {
//    //                        var _count = 0;
//    //                        var entSchool = new GetSchoolMessage();
//    //                        entSchool.Id = item.Id;
//    //                        entSchool.Attachments = item.Attachments;

//    //                        //entSchool.StandardId = item.StandardId;
//    //                        //entSchool.SectionId = item.SectionId;

//    //                          entSchool.StandardId = item.Standardsectionmappingid;
//    //                       // entSchool.StandardId = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Parentid; //sanduni
//    //                        entSchool.SectionId = item.Standardsectionmappingid;

//    //                        entSchool.schoolusername = null; //need to check

//    //                        entSchool.Datetimesent = item.Createddate;
//    //                        entSchool.IsEntireSchool = item.IsEntireSchool;
//    //                        entSchool.IsParticularClass = item.IsParticularClass;
//    //                        entSchool.IsParticularSection = item.IsParticularSection;
//    //                        entSchool.Message = item.Message;
//    //                        entSchool.SchoolUserId = (int)item.Schooluserid;
//    //                        entSchool.Subject = item.Subject;
//    //                        entSchool.IsParticularParent = item.IsParticularParent;
//    //                        entSchool.SMS = item.Sms;
//    //                        entSchool.IsPriority = item.Ispriority;
//    //                        entSchool.OneSignal = item.OneSignal;

//    //                        //var childmap = db.MChildschoolmappings.Where(w => w.Standardsectionmapping.Branch.Schoolid == schoolId && w.AcademicYearId.Equals(academicyearid)).ToList();
//    //                        //foreach (var child in childmap)
//    //                        //{
//    //                        //    var map = await this.db1.MParentchildmappings.Where(w => w.Childid == child.Childid).FirstOrDefaultAsync();
//    //                        //    if (map != null)
//    //                        //    {
//    //                        //        var parmap = map.Appuserid;
//    //                        //        var parents = db1.MAppuserinfos.Where(w => w.Id == parmap).Distinct().Count();
//    //                        //        _count = _count + parents;
//    //                        //    }

//    //                        //}
//    //                        var childMappings = await db.MChildschoolmappings
//    //.Include(w => w.Standardsectionmapping.Branch) // Include related entities to avoid lazy loading
//    //.Where(w => w.Standardsectionmapping.Branch.Schoolid == schoolId && w.AcademicYearId.Equals(academicyearid))
//    //.ToListAsync();

//    //                        foreach (var child in childMappings)
//    //                        {
//    //                            var parentMapping = await this.db1.MParentchildmappings
//    //                                .Where(w => w.Childid == child.Childid)
//    //                                .Select(w => w.Appuserid)
//    //                                .FirstOrDefaultAsync();

//    //                            if (parentMapping != null)
//    //                            {
//    //                                var parentCount = await db1.MAppuserinfos
//    //                                    .Where(w => w.Id == parentMapping)
//    //                                    .Select(w => w.Id)
//    //                                    .Distinct()
//    //                                    .CountAsync();

//    //                                _count += parentCount;
//    //                            }
//    //                        }

//    //                        entSchool.ParCount = _count;
//    //                        schoolmessage.Add(entSchool);
//    //                    }
//    //                    else
//    //                    {
//    //                        GetSchoolMessage def = new GetSchoolMessage();
//    //                        def.Id = item.Id;
//    //                        def.Attachments = item.Attachments;

//    //                        if (item.Standardsectionmapping == null)
//    //                        {
//    //                            def.StandardId = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Parentid; //sanduni
//    //                           // def.StandardId = item.Standardsectionmappingid;
//    //                            //def.SectionId = null;
//    //                            def.SectionId = item.Standardsectionmappingid;
//    //                        }
//    //                        else
//    //                        {
//    //                            def.StandardId = item.Standardsectionmapping.Parentid;
//    //                            def.SectionId = item.Standardsectionmappingid;
//    //                        }
//    //                        def.schoolusername = null; //need to check
//    //                        def.Datetimesent = item.Createddate;

//    //                        def.IsEntireSchool = item.IsEntireSchool;
//    //                        def.IsParticularClass = item.IsParticularClass;
//    //                        def.IsParticularSection = item.IsParticularSection;
//    //                        def.Message = item.Message;
//    //                        def.SchoolUserId = (int)item.Schooluserid;
//    //                        def.Subject = item.Subject;
//    //                        def.IsParticularParent = item.IsParticularParent;
//    //                        def.SMS = item.Sms;
//    //                        def.IsPriority = item.Ispriority;
//    //                        def.OneSignal = item.OneSignal;
//    //                        schoolmessage.Add(def);

//    //                    }



//    //                }
//    //            }

//    //            int count = schoolmessage.Count;
//    //            int CurrentPage = pageNumber;
//    //            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
//    //            var items = schoolmessage.OrderByDescending(x => x.Datetimesent).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
//    //            var previousPage = CurrentPage > 1 ? "Yes" : "No";
//    //            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

//    //            var obj = new
//    //            {
//    //                TotalPages = TotalPages,
//    //                items = items
//    //            };
//    //            return (obj);
//    //        }
//    //        catch (Exception ex)
//    //        {
//    //            return ex.ToString();
//    //        }
//    //    }







//    //    public Task<IQueryable<object>> GetEntityByName(string EntityName)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //}
//    //sanduni15march2024
//    public class GetSchooluserids
//    {
//        public int? Parentid { get; set; }
//        public string ParentName { get; set; }
//        public int? RelationId { get; set; }
//        public int? ChildId { get; set; }
//    }
//    public class GetNoticeboardmsgs
//    {
//        public int? Id { get; set; }
//        public string attachments { get; set; }
//        public int? standardsectionmappingid { get; set; }
//        public DateTime? createddate { get; set; }
//        public int? IsEntireSchool { get; set; }
//        public int? IsParticularClass { get; set; }
//        public int? IsParticularSection { get; set; }
//        public int? IsParticularParent { get; set; }
//        public string Subject { get; set; }
//        public string Message { get; set; }
//        public int? SchoolUserId { get; set; }
//        public string OneSignal { get; set; }
//        public string SMS { get; set; }
//        public int? IsPriority { get; set; }
//        public string ParentName { get; set; }
//        public int? RelationId { get; set; }
//        public int? ChildId { get; set; }
//    }
//    public class GetParentsSP
//    {
//        public int? Parentid { get; set; }
//        public string ParentName { get; set; }
//        public int? RelationId { get; set; }
//        public int? ChildId { get; set; }
//        public string ChildEmail { get; set; }
//        public int? ChildSchoolMappingId { get; set; }
//        public string ChildName { get; set; }
//        public string ChildFirstName { get; set; }
//        public string ChildLastName { get; set; }
//        public int? SectionId { get; set; }
//        public string SectionName { get; set; }
//        public int? StandardId { get; set; }
//        public string StandardName { get; set; }
//        public int SecondParent { get; set; }
//        public string RegistrationNumber { get; set; }
//        public int? DRCEnable1 { get; set; }
//        public int? DRCEnable2 { get; set; }
//        public int? DRCEnable3 { get; set; }
//        public int? StatusId { get; set; } // 27/2/2024 Sanduni


//    }



//}
