using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Services.TNoticeboardmappingService;
using static System.Collections.Specialized.BitVector32;


namespace Services
{
    public interface ITNoticeboardmappingService : ICommonService
    {
        Task<int> AddEntity(TNoticeboardmapping entity);
        Task<TNoticeboardmapping> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(TNoticeboardmapping entity);
        Task<IQueryable<GetParentsSP>> GetParentsNB(int schoolid, string standard, string section);
        IQueryable<TNoticeboardmessage> GetNoticeboardmessagesBySchoolID(int userid, int schoolid, string searchstring);
        Task<object> GetSchoolMessagesBySchoolIdForAPI(int userId, int schoolId, int? standardId, int? sectionId, int? childId, string searchString, int PageSize, int pageNumber, int isSelectAll);
        Task<IQueryable<TNoticeboardmessage>> GetSp_GetNoticeBoardMsgBySchoolId(int schoolid, int userid, int? standardId, int? sectionId, string searchString, int? pagesize, int? pagenumber, int isSelectAll);
        Task<object> GetSchoolMessageById(int messageId);
        Task<IQueryable<GetBulkNBMessagesSP>> GetBulkNBMessages(int schoolUserId, DateTime startNBDate, DateTime endNBDate);
        Task DeleteBulkNBMessages(int schoolUserId, DateTime startNBDate, DateTime endNBDate);


    }
    public class TNoticeboardmappingService : ITNoticeboardmappingService
    {
        private readonly IRepository<TNoticeboardmapping> repository;
        private DbSet<TNoticeboardmapping> localDBSet;

        private TpContext db = new TpContext();
        private TpContext db2 = new TpContext();

        private readonly TpContext tpContext;
        private readonly IConfiguration configuration;

        public TNoticeboardmappingService(IRepository<TNoticeboardmapping> repository, TpContext tpContext, IConfiguration configuration)
        {
            this.repository = repository;
            this.tpContext = tpContext;
            this.configuration = configuration;
        }

        public async Task<int> AddEntity(TNoticeboardmapping entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<TNoticeboardmapping>)await this.repository.GetAll();

        private static Object Mapper(TNoticeboardmapping x) => new
        {
            x.Id,
            x.Noticeboardmsgid,
            x.Childid,
            x.Appuserid,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };

        private async Task<IQueryable<TNoticeboardmapping>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<TNoticeboardmapping> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<int> UpdateEntity(TNoticeboardmapping entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<GetBulkNBMessagesSP>> GetBulkNBMessages(int schoolUserId, DateTime startNBDate, DateTime endNBDate)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetBulkNBMessagesSP>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetBulkNBMessagesSP, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@SchoolUserId", SqlDbType.Int)).Value = schoolUserId;
                command.Parameters.Add(new SqlParameter("@StartNBDate", SqlDbType.DateTime)).Value = startNBDate;
                command.Parameters.Add(new SqlParameter("@EndNBDate", SqlDbType.DateTime)).Value = endNBDate;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new GetBulkNBMessagesSP
                            {
                                Id = reader["id"] != DBNull.Value ? (int?)reader["id"] : null,
                                SchoolUserId = reader["schooluserid"] != DBNull.Value ? (int?)reader["schooluserid"] : null,
                                Title = reader["subject"]?.ToString(),
                                Message = reader["message"]?.ToString(),
                                CreatedDate = reader["createddate"] != DBNull.Value ? (DateTime?)reader["createddate"] : null
                            }));
                        }
                    }
                }
                return res.AsQueryable();
            }
        }


        public class GetBulkNBMessagesSP
        {
            public int? Id { get; set; }
            public int? SchoolUserId { get; set; }
            public string Title { get; set; }

            public string Message { get; set; }

            public DateTime? CreatedDate { get; set; }

        }

        public async Task DeleteBulkNBMessages(int schoolUserId, DateTime startNBDate, DateTime endNBDate)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            try
            {
                await using var conn = new SqlConnection(connectionString);
                await using var cmd = new SqlCommand(ApplicationConstants.DeleteBulkNBMessagesSP, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@SchoolUserId", SqlDbType.Int).Value = schoolUserId;
                cmd.Parameters.Add("@StartNBDate", SqlDbType.DateTime).Value = startNBDate;
                cmd.Parameters.Add("@EndNBDate", SqlDbType.DateTime).Value = endNBDate;

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: {ex.Message}");
            }
        }


        public class DeleteBulkNBMessagesSP
        {
            public bool? IsAuthorized { get; set; }
            public string Message { get; set; }
            public int? ErrorNumber { get; set; }
            public string ErrorMessage { get; set; }
        }

        public async Task<object> GetSchoolMessageById(int messageId)
        {
            try
            {
                var schoolmessage = new List<GetSchoolMessage>();


                var objresult = await this.db.TNoticeboardmessages
                                            .Where(w => w.Id == messageId)
                                            .ToListAsync();

                if (objresult != null)
                {
                    foreach (var item in objresult.ToList())
                    {

                        var schooluser = await this.db.MSchooluserinfos.FirstOrDefaultAsync(w => w.Id == item.Schooluserid);

                        if (schooluser == null)
                        {

                            schooluser = await this.db.MSchooluserinfos.FirstOrDefaultAsync(w => w.Id == messageId);

                            if (schooluser != null)
                            {
                                item.Schooluserid = schooluser.Id;
                            }
                        }


                        if (item.Standardsectionmappingid != null && (item.IsParticularParent ?? false))
                        {
                            var ParentList = db.TNoticeboardmappings.Where(w => w.Noticeboardmsgid == item.Id).ToList();
                            var schMsg = new GetSchoolMessage
                            {
                                Id = item.Id,
                                Attachments = item.Attachments,
                                schoolusername = schooluser?.Username,
                                Datetimesent = item.Createddate,
                                IsEntireSchool = item.IsEntireSchool,
                                IsParticularClass = item.IsParticularClass,
                                IsParticularSection = item.IsParticularSection,
                                Message = item.Message,
                                SchoolUserId = (int)item.Schooluserid,
                                Subject = item.Subject,
                                IsParticularParent = item.IsParticularParent,
                                SMS = item.Sms,
                                isemail = item.Isemail,
                                IsPriority = item.Ispriority,
                                OneSignal = item.OneSignal,
                                Parents = new List<ParticularParent>()
                            };

                            if (item.Standardsectionmapping != null)
                            {
                                schMsg.StandardId = item.Standardsectionmappingid;
                                schMsg.SectionId = null;
                                schMsg.standardname = db.MStandardsectionmappings
                                                        .Where(w => w.Id == item.Standardsectionmappingid)
                                                        .FirstOrDefault()?.Name;
                                schMsg.sectionname = null;
                            }
                            else
                            {
                                schMsg.StandardId = db.MStandardsectionmappings
                                                      .Where(w => w.Id == item.Standardsectionmappingid)
                                                      .FirstOrDefault()?.Parentid;
                                schMsg.SectionId = item.Standardsectionmappingid;
                                schMsg.standardname = db.MStandardsectionmappings
                                                        .Where(w => w.Id == schMsg.StandardId)
                                                        .FirstOrDefault()?.Name;
                                schMsg.sectionname = db.MStandardsectionmappings
                                                       .Where(w => w.Id == item.Standardsectionmappingid)
                                                       .FirstOrDefault()?.Name;
                            }

                            foreach (var inner in ParentList)
                            {
                                var innerParent = await this.db.MAppuserinfos.FirstOrDefaultAsync(w => w.Id == inner.Appuserid);
                                var relationId = db.MParentchildmappings.FirstOrDefault(w => w.Appuserid == inner.Appuserid)?.Relationtypeid;
                                var childfirstname = db.MChildinfos.FirstOrDefault(w => w.Id == inner.Childid)?.Firstname;
                                var childlastname = db.MChildinfos.FirstOrDefault(w => w.Id == inner.Childid)?.Lastname;

                                var perPar = new ParticularParent
                                {
                                    ParentName = innerParent?.Firstname + " " + innerParent?.Lastname,
                                    Relation = relationId,
                                    ParentId = inner.Appuserid,
                                    ChildId = inner.Childid,
                                    ChildName = childfirstname + " " + childlastname
                                };

                                schMsg.Parents.Add(perPar);
                            }

                            schMsg.ParCount = ParentList.Count();
                            schoolmessage.Add(schMsg);
                        }
                        else if (item.Standardsectionmappingid != null && !(item.IsParticularParent ?? false))
                        {
                            var allPar = new GetSchoolMessage
                            {
                                Id = item.Id,
                                Attachments = item.Attachments,
                                schoolusername = schooluser?.Username,
                                Datetimesent = item.Createddate,
                                IsEntireSchool = item.IsEntireSchool,
                                IsParticularClass = item.IsParticularClass,
                                IsParticularSection = item.IsParticularSection,
                                Message = item.Message,
                                SchoolUserId = (int)item.Schooluserid,
                                Subject = item.Subject,
                                IsParticularParent = item.IsParticularParent,
                                SMS = item.Sms,
                                isemail = item.Isemail,
                                IsPriority = item.Ispriority,
                                OneSignal = item.OneSignal
                            };

                            if (item.Standardsectionmapping != null)
                            {
                                allPar.StandardId = item.Standardsectionmappingid;
                                allPar.SectionId = null;
                                allPar.standardname = db.MStandardsectionmappings
                                                        .Where(w => w.Id == item.Standardsectionmappingid)
                                                        .FirstOrDefault()?.Name;
                                allPar.sectionname = null;
                            }
                            else
                            {
                                allPar.StandardId = db.MStandardsectionmappings
                                                      .Where(w => w.Id == item.Standardsectionmappingid)
                                                      .FirstOrDefault()?.Parentid;
                                allPar.SectionId = item.Standardsectionmappingid;
                                allPar.standardname = db.MStandardsectionmappings
                                                        .Where(w => w.Id == allPar.StandardId)
                                                        .FirstOrDefault()?.Name;
                                allPar.sectionname = db.MStandardsectionmappings
                                                       .Where(w => w.Id == item.Standardsectionmappingid)
                                                       .FirstOrDefault()?.Name;
                            }


                            allPar.ParCount = db.MChildschoolmappings
                                                .Where(w => w.Standardsectionmapping.Branch.Schoolid == messageId)
                                                .Distinct()
                                                .Count();
                            schoolmessage.Add(allPar);
                        }
                        else if (item.Standardsectionmappingid == null)
                        {
                            var entSchool = new GetSchoolMessage
                            {
                                Id = item.Id,
                                Attachments = item.Attachments,
                                StandardId = item.Standardsectionmappingid,
                                SectionId = item.Standardsectionmappingid,
                                schoolusername = schooluser?.Username,
                                Datetimesent = item.Createddate,
                                IsEntireSchool = item.IsEntireSchool,
                                IsParticularClass = item.IsParticularClass,
                                IsParticularSection = item.IsParticularSection,
                                Message = item.Message,
                                SchoolUserId = (int)item.Schooluserid,
                                Subject = item.Subject,
                                IsParticularParent = item.IsParticularParent,
                                SMS = item.Sms,
                                isemail = item.Isemail,
                                IsPriority = item.Ispriority,
                                OneSignal = item.OneSignal,
                                ParCount = db.MChildschoolmappings
                                            .Where(w => w.Standardsectionmapping.Branch.Schoolid == messageId)
                                            .Distinct()
                                            .Count()
                            };
                            schoolmessage.Add(entSchool);
                        }
                    }
                }

                return new
                {
                    Items = schoolmessage
                };
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<IQueryable<GetParentsSP>> GetParentsNB(int schoolid, string standard, string section)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetParentsSP>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetParentSP, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                command.Parameters["@schoolid"].Value = schoolid;
                command.Parameters.Add(new SqlParameter("@standard", SqlDbType.NVarChar));
                command.Parameters["@standard"].Value = standard;
                command.Parameters.Add(new SqlParameter("@section", SqlDbType.NVarChar));
                command.Parameters["@section"].Value = section;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new GetParentsSP()
                            {
                                Parentid = reader["id"] != DBNull.Value ? (int?)reader["id"] : null,
                                ParentName = reader["ParentName"].ToString(),
                                RelationId = reader["RelationId"] != DBNull.Value ? (int?)reader["RelationId"] : null,
                                ChildId = reader["ChildId"] != DBNull.Value ? (int?)reader["ChildId"] : null,
                                ChildSchoolMappingId = reader["ChildSchoolMappingId"] != DBNull.Value ? (int?)reader["ChildSchoolMappingId"] : null,
                                ChildName = reader["ChildName"].ToString(),
                                ChildEmail = reader["ChildEmail"].ToString(),
                                SectionId = reader["SectionId"] != DBNull.Value ? (int?)reader["SectionId"] : null,
                                SectionName = reader["SectionName"].ToString(),
                                StandardId = reader["StandardId"] != DBNull.Value ? (int?)reader["StandardId"] : null,
                                StandardName = reader["StandardName"].ToString(),
                                RegistrationNumber = reader["RegistrationNumber"].ToString(),
                                StatusId = reader["StatusId"] != DBNull.Value ? (int?)reader["StatusId"] : null, // 27/2/2024 Sanduni
                            }));

                        }
                    }
                }
                return res.AsQueryable();
            }
        }

        //public async Task<IQueryable<GetParentsSP>> GetParentsNB(int schoolid, string standard, string section)
        //{
        //    var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        var res = new List<GetParentsSP>();
        //        connection.Open();
        //        SqlCommand command = new SqlCommand(ApplicationConstants.GetParentSP, connection);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
        //        command.Parameters["@schoolid"].Value = schoolid;
        //        command.Parameters.Add(new SqlParameter("@standard", SqlDbType.NVarChar));
        //        command.Parameters["@standard"].Value = standard;
        //        command.Parameters.Add(new SqlParameter("@section", SqlDbType.NVarChar));
        //        command.Parameters["@section"].Value = section;
        //        using (SqlDataReader reader = command.ExecuteReader())
        //        {
        //            if (reader.HasRows)
        //            {
        //                while (reader.Read())
        //                {
        //                    await Task.Run(() => res.Add(new GetParentsSP()
        //                    {
        //                        Parentid = reader["id"] != DBNull.Value ? (int?)reader["id"] : null,
        //                        ParentName = reader["ParentName"].ToString(),
        //                        RelationId = reader["RelationId"] != DBNull.Value ? (int?)reader["RelationId"] : null,
        //                        ChildId = reader["ChildId"] != DBNull.Value ? (int?)reader["ChildId"] : null,
        //                        ChildSchoolMappingId = reader["ChildSchoolMappingId"] != DBNull.Value ? (int?)reader["ChildSchoolMappingId"] : null,
        //                        ChildName = reader["ChildName"].ToString(),
        //                        ChildEmail = reader["ChildEmail"].ToString(),
        //                        SectionId = reader["SectionId"] != DBNull.Value ? (int?)reader["SectionId"] : null,
        //                        SectionName = reader["SectionName"].ToString(),
        //                        StandardId = reader["StandardId"] != DBNull.Value ? (int?)reader["StandardId"] : null,
        //                        StandardName = reader["StandardName"].ToString(),
        //                        RegistrationNumber = reader["RegistrationNumber"].ToString()
        //                    }));

        //                }
        //            }
        //        }
        //        return res.AsQueryable();
        //    }
        //}


        public IQueryable<TNoticeboardmessage> GetNoticeboardmessagesBySchoolID(int userid, int schoolid, string searchstring)
        {
            try
            {
                var nbmsg = tpContext.TNoticeboardmessages.Where(x => x.Schooluserid.Equals(userid) && x.Branch.Schoolid.Equals(schoolid) && x.Statusid == 1 && ((x.Message.Contains(searchstring)) || (x.Sms.Contains(searchstring)) || (x.Subject.Contains(searchstring)))).Include(x => x.Standardsectionmapping).Include(x => x.Branch);
                if (nbmsg.Count() > 0)
                {
                    return nbmsg;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        //public async Task<object> GetSchoolMessagesBySchoolIdForAPIOld(int userId, int schoolId, string searchString = "", int PageSize = 10, int pageNumber = 1)
        //{
        //    try
        //    {
        //        List<GetSchoolMessage> schoolmessage = new List<GetSchoolMessage>();
        //        //ObjectParameter totalCount = new ObjectParameter("totalCount", typeof(int));
        //        IQueryable<TNoticeboardmessage> objresult = GetNoticeboardmessagesBySchoolID(userId, schoolId, searchString);

        //        if (objresult != null)
        //        {

        //            foreach (var item in objresult.ToList())
        //            {
        //                var schooluser = await this.db.MSchooluserinfos.Where(w => w.Id == item.Schooluserid).FirstOrDefaultAsync();

        //                if (item.Standardsectionmappingid != null && item.IsParticularParent == true)
        //                {
        //                    var _count = 0;
        //                    var ParentList = db.TNoticeboardmappings.Where(w => w.Noticeboardmsgid == item.Id).ToList();
        //                    var schMsg = new GetSchoolMessage();
        //                    schMsg.Id = item.Id;
        //                    schMsg.Attachments = item.Attachments;
        //                    schMsg.schoolusername = schooluser.Username; //need to check

        //                    if (item.Standardsectionmapping.Parentid == null)
        //                    {
        //                        schMsg.StandardId = item.Standardsectionmappingid;
        //                        schMsg.SectionId = null;
        //                        schMsg.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
        //                        schMsg.sectionname = null;
        //                    }
        //                    else
        //                    {
        //                        schMsg.StandardId = item.Standardsectionmapping.Parentid;
        //                        schMsg.SectionId = item.Standardsectionmappingid;
        //                        schMsg.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmapping.Parentid).FirstOrDefault().Name;
        //                        schMsg.sectionname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
        //                    }

        //                    schMsg.Datetimesent = item.Createddate;
        //                    schMsg.IsEntireSchool = item.IsEntireSchool;
        //                    schMsg.IsParticularClass = item.IsParticularClass;
        //                    schMsg.IsParticularSection = item.IsParticularSection;
        //                    schMsg.Message = item.Message;
        //                    schMsg.SchoolUserId = (int)item.Schooluserid;
        //                    schMsg.Subject = item.Subject;
        //                    schMsg.IsParticularParent = item.IsParticularParent;
        //                    schMsg.SMS = item.Sms;
        //                    schMsg.IsPriority = item.Ispriority;
        //                    schMsg.OneSignal = item.OneSignal;


        //                    schMsg.Parents = new List<ParticularParent>();
        //                    foreach (var inner in ParentList)
        //                    {
        //                        var innerParent = await this.db.MAppuserinfos.Where(w => w.Id == inner.Appuserid).FirstOrDefaultAsync();
        //                        var relationId = db.MParentchildmappings.Where(w => w.Appuserid == inner.Appuserid).FirstOrDefault().Relationtypeid;
        //                        var perPar = new ParticularParent();
        //                        if (innerParent != null && relationId != null)
        //                        {

        //                            perPar.ParentName = innerParent.Firstname + " " + innerParent.Lastname;
        //                            perPar.Relation = relationId;

        //                        }
        //                        perPar.ParentId = inner.Appuserid;
        //                        perPar.ChildId = inner.Childid;
        //                        schMsg.Parents.Add(perPar);

        //                    }
        //                    _count = ParentList.Count();
        //                    schMsg.ParCount = _count;
        //                    schoolmessage.Add(schMsg);


        //                }
        //                else if (item.Standardsectionmappingid != null && item.IsParticularParent == false)
        //                {
        //                    var _count = 0;
        //                    var allPar = new GetSchoolMessage();
        //                    allPar.Id = item.Id;
        //                    allPar.Attachments = item.Attachments;
        //                    allPar.schoolusername = schooluser.Username; //need to check


        //                    if (item.Standardsectionmapping.Parentid == null)
        //                    {
        //                        allPar.StandardId = item.Standardsectionmappingid;
        //                        allPar.SectionId = null;
        //                        allPar.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
        //                        allPar.sectionname = null;
        //                    }
        //                    else
        //                    {
        //                        allPar.StandardId = item.Standardsectionmapping.Parentid;
        //                        allPar.SectionId = item.Standardsectionmappingid;
        //                        allPar.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmapping.Parentid).FirstOrDefault().Name;
        //                        allPar.sectionname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
        //                    }

        //                    //allPar.StandardId = item.StandardId;
        //                    //allPar.standardname = db.Standards.Where(w => w.Id == item.StandardId).FirstOrDefault().Name;
        //                    //allPar.sectionname = db.Sections.Where(w => w.Id == item.SectionId).FirstOrDefault().Name;
        //                    //allPar.SectionId = item.SectionId;


        //                    allPar.Datetimesent = item.Createddate;
        //                    allPar.IsEntireSchool = item.IsEntireSchool;
        //                    allPar.IsParticularClass = item.IsParticularClass;
        //                    allPar.IsParticularSection = item.IsParticularSection;
        //                    allPar.Message = item.Message;
        //                    allPar.SchoolUserId = (int)item.Schooluserid;

        //                    allPar.Subject = item.Subject;
        //                    allPar.IsParticularParent = item.IsParticularParent;
        //                    allPar.SMS = item.Sms;
        //                    allPar.IsPriority = item.Ispriority;
        //                    allPar.OneSignal = item.OneSignal;
        //                    //var _classes = db.Standards.Where(w => w.Id == item.StandardId).FirstOrDefault().Id;
        //                    //var _sections = db.Sections.Where(w => w.Id == item.SectionId).FirstOrDefault().Id;
        //                    var childmap = db.MChildschoolmappings.Where(w => w.Standardsectionmappingid.Equals(item.Standardsectionmappingid)).ToList();
        //                    foreach (var child in childmap)
        //                    {
        //                        var _chinfo = db.MChildinfos.Where(w => w.Id.Equals(child.Childid)).FirstOrDefault();
        //                        var _parid = db2.MParentchildmappings.Where(w => w.Childid.Equals(_chinfo.Id)).FirstOrDefault().Appuserid;
        //                        var _parent = db.MAppuserinfos.Where(w => w.Id.Equals(_parid)).Count();
        //                        _count = _count + _parent;
        //                    }
        //                    allPar.ParCount = _count;
        //                    schoolmessage.Add(allPar);
        //                }
        //                else if (item.Standardsectionmappingid == null)
        //                {
        //                    var _count = 0;
        //                    var entSchool = new GetSchoolMessage();
        //                    entSchool.Id = item.Id;
        //                    entSchool.Attachments = item.Attachments;

        //                    //entSchool.StandardId = item.StandardId;
        //                    //entSchool.SectionId = item.SectionId;
        //                    entSchool.StandardId = item.Standardsectionmappingid;
        //                    entSchool.SectionId = item.Standardsectionmappingid;

        //                    entSchool.schoolusername = schooluser.Username; //need to check

        //                    entSchool.Datetimesent = item.Createddate;
        //                    entSchool.IsEntireSchool = item.IsEntireSchool;
        //                    entSchool.IsParticularClass = item.IsParticularClass;
        //                    entSchool.IsParticularSection = item.IsParticularSection;
        //                    entSchool.Message = item.Message;
        //                    entSchool.SchoolUserId = (int)item.Schooluserid;
        //                    entSchool.Subject = item.Subject;
        //                    entSchool.IsParticularParent = item.IsParticularParent;
        //                    entSchool.SMS = item.Sms;
        //                    entSchool.IsPriority = item.Ispriority;
        //                    entSchool.OneSignal = item.OneSignal;
        //                    var childmap = db.MChildschoolmappings.Where(w => w.Standardsectionmapping.Branch.Schoolid == schoolId).ToList();
        //                    foreach (var child in childmap)
        //                    {
        //                        var map = await this.db.MParentchildmappings.Where(w => w.Childid == child.Childid).FirstOrDefaultAsync();
        //                        if (map != null)
        //                        {
        //                            var parmap = map.Appuserid;
        //                            var parents = db.MAppuserinfos.Where(w => w.Id == parmap).Distinct().Count();
        //                            _count = _count + parents;
        //                        }

        //                    }
        //                    entSchool.ParCount = _count;
        //                    schoolmessage.Add(entSchool);
        //                }
        //                else
        //                {
        //                    GetSchoolMessage def = new GetSchoolMessage();
        //                    def.Id = item.Id;
        //                    def.Attachments = item.Attachments;

        //                    if (item.Standardsectionmapping.Parentid == null)
        //                    {
        //                        def.StandardId = item.Standardsectionmappingid;
        //                        def.SectionId = null;
        //                    }
        //                    else
        //                    {
        //                        def.StandardId = item.Standardsectionmapping.Parentid;
        //                        def.SectionId = item.Standardsectionmappingid;
        //                    }
        //                    def.schoolusername = schooluser.Username; //need to check
        //                    def.Datetimesent = item.Createddate;

        //                    def.IsEntireSchool = item.IsEntireSchool;
        //                    def.IsParticularClass = item.IsParticularClass;
        //                    def.IsParticularSection = item.IsParticularSection;
        //                    def.Message = item.Message;
        //                    def.SchoolUserId = (int)item.Schooluserid;
        //                    def.Subject = item.Subject;
        //                    def.IsParticularParent = item.IsParticularParent;
        //                    def.SMS = item.Sms;
        //                    def.IsPriority = item.Ispriority;
        //                    def.OneSignal = item.OneSignal;
        //                    schoolmessage.Add(def);

        //                }

        //            }
        //        }

        //        int count = schoolmessage.Count;
        //        int CurrentPage = pageNumber;
        //        int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
        //        var items = schoolmessage.OrderByDescending(x => x.Datetimesent).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        //        var previousPage = CurrentPage > 1 ? "Yes" : "No";
        //        var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

        //        var obj = new
        //        {
        //            TotalPages = TotalPages,
        //            items = items
        //        };
        //        return (obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.ToString();
        //    }
        //}
        public async Task<object> GetSchoolMessagesBySchoolIdForAPI(int userId, int schoolId, int? standardId = null, int? sectionId = null, int? childId = null, string searchString = "", int PageSize = 10, int pageNumber = 1, int isSelectAll = 0)
        {
            try
            {
                List<GetSchoolMessage> schoolmessage = new List<GetSchoolMessage>();
                //ObjectParameter totalCount = new ObjectParameter("totalCount", typeof(int));
                // IQueryable<TNoticeboardmessage> objresult = GetNoticeboardmessagesBySchoolID(userId, schoolId, searchString);

                Task <IQueryable<TNoticeboardmessage>> task = GetSp_GetNoticeBoardMsgBySchoolId(schoolId, userId, standardId, sectionId, searchString, PageSize, pageNumber, isSelectAll);
                task.Wait(); // This will synchronously wait for the task to complete
                IQueryable<TNoticeboardmessage> objresult = task.Result;
                var nbcount = 0;
               //Task<IQueryable<TNoticeboardmessage>> task2 = GetSp_GetNoticeBoardMsgBySchoolId2(schoolId, userId, PageSize, pageNumber);
               var roleid = db.MRoles.Where(w => w.Schoolid == schoolId && w.Statusid == 1 && w.Rank == 1).Select(w => w.Id).FirstOrDefault();
                var status = await (from schoolUserRole in db.MSchooluserroles join category in db.MCategories on schoolUserRole.Categoryid equals category.Id 
                                                 where category.Roleid == roleid && category.Statusid == 1 && schoolUserRole.Statusid == 1 && schoolUserRole.Schooluserid == userId
                                                 select schoolUserRole.Id).FirstOrDefaultAsync();
                var branchid1 = db.MBranches.Where(w => w.Schoolid == schoolId && w.Statusid ==1).Select(w => w.Id).FirstOrDefault();
                if (status != 0)
                { 
                    if(sectionId != null)
                    {
                        //nbcount = await db.TNoticeboardmessages.Where(w => w.Branchid == branchid1 && w.Standardsectionmappingid == sectionId && w.Subject == searchString).CountAsync();
                        IQueryable<TNoticeboardmessage> query = db.TNoticeboardmessages.Where(w => w.Branchid == branchid1 && w.Standardsectionmappingid == sectionId);
                        if (!string.IsNullOrEmpty(searchString))
                        {
                            query = query.Where(w => w.Subject == searchString);
                        }
                        nbcount = await query.CountAsync();
                    }
                    else if (standardId != null)
                    {
                        var Sectionlist = db.MStandardsectionmappings.Where(w => w.Parentid == standardId).Select(w => w.Id).ToList();
                        IQueryable<TNoticeboardmessage> query = db.TNoticeboardmessages .Where(w => w.Branchid == branchid1 && Sectionlist.Contains((int)w.Standardsectionmappingid));

                        if (!string.IsNullOrEmpty(searchString))
                        {
                            query = query.Where(w => w.Subject == searchString);
                        }

                        nbcount = await query.CountAsync();
                        //nbcount = await db.TNoticeboardmessages.Where(w => w.Branchid == branchid1 && Sectionlist.Contains((int)w.Standardsectionmappingid)&& w.Subject == searchString).CountAsync();
                    }
                    else
                    {
                        IQueryable<TNoticeboardmessage> query = db.TNoticeboardmessages.Where(w => w.Branchid == branchid1);

                        if (!string.IsNullOrEmpty(searchString))
                        {
                            query = query.Where(w => w.Subject == searchString);
                        }

                        nbcount = await query.CountAsync();
                        //nbcount = await db.TNoticeboardmessages.Where(w => w.Branchid == branchid1 && Sectionlist.Contains((int)w.Standardsectionmappingid)&& w.Subject == searchString).CountAsync();
                    }

                }
                else
                {
                    //nbcount = await  db.TNoticeboardmessages.Where(w => w.Branchid == branchid1 && w.Schooluserid == userId).CountAsync();
                    if (sectionId != null)
                    {
                        //nbcount = await db.TNoticeboardmessages.Where(w => w.Branchid == branchid1 && w.Schooluserid == userId && w.Standardsectionmappingid == sectionId && w.Subject == searchString ).CountAsync();
                        IQueryable<TNoticeboardmessage> query = db.TNoticeboardmessages.Where(w => w.Branchid == branchid1 && w.Schooluserid == userId && w.Standardsectionmappingid == sectionId);
                        if (!string.IsNullOrEmpty(searchString))
                        {
                            query = query.Where(w => w.Subject == searchString);
                        }

                        nbcount = await query.CountAsync();
                    }
                    if (standardId != null)
                    {
                        var Sectionlist = db.MStandardsectionmappings.Where(w => w.Parentid == standardId).Select(w => w.Id).ToList();
                        IQueryable<TNoticeboardmessage> query = db.TNoticeboardmessages.Where(w => w.Branchid == branchid1 && w.Schooluserid == userId && Sectionlist.Contains((int)w.Standardsectionmappingid));

                        if (!string.IsNullOrEmpty(searchString))
                        {
                            query = query.Where(w => w.Subject == searchString);
                        }

                        nbcount = await query.CountAsync();
                        //nbcount = await db.TNoticeboardmessages.Where(w => w.Branchid == branchid1 && Sectionlist.Contains((int)w.Standardsectionmappingid)&& w.Subject == searchString).CountAsync();
                    }
                    else
                    {
                        IQueryable<TNoticeboardmessage> query = db.TNoticeboardmessages.Where(w => w.Branchid == branchid1 && w.Schooluserid == userId);

                        if (!string.IsNullOrEmpty(searchString))
                        {
                            query = query.Where(w => w.Subject == searchString);
                        }

                        nbcount = await query.CountAsync();
                        //nbcount = await db.TNoticeboardmessages.Where(w => w.Branchid == branchid1 && Sectionlist.Contains((int)w.Standardsectionmappingid)&& w.Subject == searchString).CountAsync();
                    }
                }



               // task2.Wait(); // This will synchronously wait for the task to complete
               // IQueryable<TNoticeboardmessage> objresult2 = task2.Result;

                if (objresult != null)
                {

                    foreach (var item in objresult.ToList())
                    {
                        var schooluser = await this.db.MSchooluserinfos.Where(w => w.Id == item.Schooluserid).FirstOrDefaultAsync();
                       if(schooluser == null)
                        {
                            schooluser = await this.db.MSchooluserinfos.Where(w => w.Id == userId).FirstOrDefaultAsync();
                            schooluser.Username = schooluser.Username;
                            schooluser.Id = schooluser.Id;
                            item.Schooluserid = schooluser.Id;

                        }
                        
                        if (item.Standardsectionmappingid != null && item.IsParticularParent == true)
                        {
                            var _count = 0;
                            var ParentList = db.TNoticeboardmappings.Where(w => w.Noticeboardmsgid == item.Id).ToList();
                            var schMsg = new GetSchoolMessage();
                            schMsg.Id = item.Id;
                            schMsg.Attachments = item.Attachments;
                            schMsg.schoolusername = schooluser.Username; //need to check

                            //if (item.Standardsectionmapping.Parentid == null)
                            if (item.Standardsectionmapping != null)
                            {
                                schMsg.StandardId = item.Standardsectionmappingid;
                                schMsg.SectionId = null;
                                schMsg.standardname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
                                schMsg.sectionname = null;
                            }
                            else
                            {
                                // schMsg.StandardId = item.Standardsectionmapping.Parentid;
                                schMsg.StandardId = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Parentid;
                                schMsg.SectionId = item.Standardsectionmappingid;
                                schMsg.standardname = db.MStandardsectionmappings.Where(w => w.Id == schMsg.StandardId).FirstOrDefault().Name;
                                schMsg.sectionname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
                            }

                            schMsg.Datetimesent = item.Createddate;
                            schMsg.IsEntireSchool = item.IsEntireSchool;
                            schMsg.IsParticularClass = item.IsParticularClass;
                            schMsg.IsParticularSection = item.IsParticularSection;
                            schMsg.Message = item.Message;
                            schMsg.SchoolUserId = (int)item.Schooluserid;
                            schMsg.Subject = item.Subject;
                            schMsg.IsParticularParent = item.IsParticularParent;
                            schMsg.SMS = item.Sms;
                            schMsg.isemail = item.Isemail;//Sanduni July 29 2024
                            schMsg.IsPriority = item.Ispriority;
                            schMsg.OneSignal = item.OneSignal;


                            schMsg.Parents = new List<ParticularParent>();
                            //foreach (var inner in ParentList) // 29/9/2024
                            //{
                            //    var innerParent = await this.db.MAppuserinfos.FirstOrDefaultAsync(w => w.Id == inner.Appuserid);
                            //    var relationId = db.MParentchildmappings.FirstOrDefault(w => w.Appuserid == inner.Appuserid)?.Relationtypeid;
                            //    var childfirstname = db.MChildinfos.FirstOrDefault(w => w.Id == inner.Childid)?.Firstname;
                            //    var childlastname = db.MChildinfos.FirstOrDefault(w => w.Id == inner.Childid)?.Lastname;
                            //    var perPar = new ParticularParent();
                            //    if (innerParent != null && relationId != null)
                            //    {

                            //        perPar.ParentName = innerParent.Firstname + " " + innerParent.Lastname;
                            //        perPar.Relation = relationId;

                            //    }
                            //    perPar.ParentId = inner.Appuserid;
                            //    perPar.ChildId = inner.Childid;
                            //    perPar.ChildName = childfirstname +" "+ childlastname;
                            //    schMsg.Parents.Add(perPar);

                            //}
                            //_count = ParentList.Count();
                            //schMsg.ParCount = _count;
                            schoolmessage.Add(schMsg);


                        }
                        else if (item.Standardsectionmappingid != null && item.IsParticularParent == false)
                        {
                            var _count = 0;
                            var allPar = new GetSchoolMessage();
                            allPar.Id = item.Id;
                            allPar.Attachments = item.Attachments;
                            allPar.schoolusername = schooluser.Username; //need to check


                            // if (item.Standardsectionmapping.Parentid == null) sanduni 22/4/2024
                             if (item.Standardsectionmapping != null)
                            {
                                allPar.StandardId = item.Standardsectionmappingid;
                                allPar.SectionId = null;
                                allPar.standardname = db.MStandardsectionmappings.FirstOrDefault(w => w.Id == item.Standardsectionmappingid)?.Name;
                                allPar.sectionname = null;
                            }
                            else
                            {
                                // schMsg.StandardId = item.Standardsectionmapping.Parentid;
                                allPar.StandardId = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Parentid;
                                allPar.SectionId = item.Standardsectionmappingid;
                                allPar.standardname = db.MStandardsectionmappings.Where(w => w.Id == allPar.StandardId).FirstOrDefault().Name;
                                allPar.sectionname = db.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefault().Name;
                            }

                            //allPar.StandardId = item.StandardId;
                            //allPar.standardname = db.Standards.Where(w => w.Id == item.StandardId).FirstOrDefault().Name;
                            //allPar.sectionname = db.Sections.Where(w => w.Id == item.SectionId).FirstOrDefault().Name;
                            //allPar.SectionId = item.SectionId;


                            allPar.Datetimesent = item.Createddate;
                            allPar.IsEntireSchool = item.IsEntireSchool;
                            allPar.IsParticularClass = item.IsParticularClass;
                            allPar.IsParticularSection = item.IsParticularSection;
                            allPar.Message = item.Message;
                            allPar.SchoolUserId = (int)item.Schooluserid;
                            

                            allPar.Subject = item.Subject;
                            allPar.IsParticularParent = item.IsParticularParent;
                            allPar.SMS = item.Sms;
                            allPar.isemail = item.Isemail; //29/7/2024
                            allPar.IsPriority = item.Ispriority;
                            allPar.OneSignal = item.OneSignal;
                            //var _classes = db.Standards.Where(w => w.Id == item.StandardId).FirstOrDefault().Id;
                            //var _sections = db.Sections.Where(w => w.Id == item.SectionId).FirstOrDefault().Id;
                            _count = db.MChildschoolmappings.Where(w => w.Standardsectionmapping.Branch.Schoolid == schoolId).ToList().Distinct().Count();
                            //var childmap = db.MChildschoolmappings.Where(w => w.Standardsectionmappingid.Equals(item.Standardsectionmappingid)).ToList();
                            //foreach (var child in childmap)
                            //{
                            //    var _chinfo = db.MChildinfos.FirstOrDefault(w => w.Id.Equals(child.Childid));
                            //    var _parid = db2.MParentchildmappings.FirstOrDefault(w => w.Childid.Equals(_chinfo.Id))?.Appuserid;
                            //    var _parent = db.MAppuserinfos.Where(w => w.Id.Equals(_parid)).Count();
                            //    _count = _count + _parent;
                            //}
                            allPar.ParCount = _count;
                            schoolmessage.Add(allPar);
                        }
                        else if (item.Standardsectionmappingid == null)
                        {
                            var _count = 0;
                            var entSchool = new GetSchoolMessage();
                            entSchool.Id = item.Id;
                            entSchool.Attachments = item.Attachments;

                            //entSchool.StandardId = item.StandardId;
                            //entSchool.SectionId = item.SectionId;
                            entSchool.StandardId = item.Standardsectionmappingid;
                            entSchool.SectionId = item.Standardsectionmappingid;

                            entSchool.schoolusername = schooluser.Username; //need to check

                            entSchool.Datetimesent = item.Createddate;
                            entSchool.IsEntireSchool = item.IsEntireSchool;
                            entSchool.IsParticularClass = item.IsParticularClass;
                            entSchool.IsParticularSection = item.IsParticularSection;
                            entSchool.Message = item.Message;
                            entSchool.SchoolUserId = (int)item.Schooluserid;
                            entSchool.Subject = item.Subject;
                            entSchool.IsParticularParent = item.IsParticularParent;
                            entSchool.SMS = item.Sms;
                            entSchool.isemail = item.Isemail;//Sanduni July 29 2024
                            entSchool.IsPriority = item.Ispriority;
                            entSchool.OneSignal = item.OneSignal;
                            _count = db.MChildschoolmappings.Where(w => w.Standardsectionmapping.Branch.Schoolid == schoolId).ToList().Distinct().Count();

                            //  var childmap = db.MChildschoolmappings.Where(w => w.Standardsectionmapping.Branch.Schoolid == schoolId).ToList(); //sanduni may27 2024
                            // foreach (var child in childmap)
                            // {
                            //    var map = await this.db.MParentchildmappings.Where(w => w.Childid == child.Childid).FirstOrDefaultAsync();
                            //    if (map != null)
                            //  {
                            //     var parmap = map.Appuserid;
                            //      var parents = db.MAppuserinfos.Where(w => w.Id == parmap).Distinct().Count();
                            //   _count = _count + parents;
                            //  }

                            //  }
                            entSchool.ParCount = _count;
                            schoolmessage.Add(entSchool);
                        }
                        else
                        {
                            GetSchoolMessage def = new GetSchoolMessage();
                            def.Id = item.Id;
                            def.Attachments = item.Attachments;

                            //if (item.Standardsectionmapping.Parentid == null) sanduni
                             if (item.Standardsectionmapping != null)
                            {
                                def.StandardId = item.Standardsectionmappingid;
                                def.SectionId = null;
                            }
                            else
                            {
                                def.StandardId = item.Standardsectionmapping.Parentid;
                                def.SectionId = item.Standardsectionmappingid;
                            }
                            def.schoolusername = schooluser.Username; //need to check
                            def.Datetimesent = item.Createddate;

                            def.IsEntireSchool = item.IsEntireSchool;
                            def.IsParticularClass = item.IsParticularClass;
                            def.IsParticularSection = item.IsParticularSection;
                            def.Message = item.Message;
                            def.SchoolUserId = (int)item.Schooluserid;
                            def.Subject = item.Subject;
                            def.IsParticularParent = item.IsParticularParent;
                            def.SMS = item.Sms;
                            def.isemail = item.Isemail;
                            def.IsPriority = item.Ispriority;
                            def.OneSignal = item.OneSignal;
                            schoolmessage.Add(def);

                        }

                    }
                }

                //  int count = objresult2.ToList().Count();
                int count = nbcount;
                int CurrentPage = pageNumber;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                // var items = schoolmessage.OrderByDescending(x => x.Datetimesent).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                var items = schoolmessage; //.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                var obj = new
                {
                    TotalPages = TotalPages,
                    items = items
                };
                return (obj);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public async Task<IQueryable<TNoticeboardmessage>> GetSp_GetNoticeBoardMsgBySchoolId2(int schoolid, int userid, int? pagesize, int? pagenumber)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<TNoticeboardmessage>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetNoticeBoardMsgBySchoolId2, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                command.Parameters["@schoolid"].Value = schoolid;
                command.Parameters.Add(new SqlParameter("@userid", SqlDbType.Int));
                command.Parameters["@userid"].Value = userid;
                command.Parameters.Add(new SqlParameter("@pagesize", SqlDbType.Int));
                command.Parameters["@pagesize"].Value = pagesize;
                command.Parameters.Add(new SqlParameter("@pagenumber", SqlDbType.Int));
                command.Parameters["@pagenumber"].Value = pagenumber;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new TNoticeboardmessage()
                            {
                                Id = (int)reader["Id"],
                                Attachments = reader["attachments"].ToString(),
                                Standardsectionmappingid = reader["standardsectionmappingid"] != DBNull.Value ? (int?)reader["standardsectionmappingid"] : null,
                                IsEntireSchool = reader["IsEntireSchool"] != DBNull.Value ? (bool?)reader["IsEntireSchool"] : null,
                                IsParticularSection = reader["IsParticularSection"] != DBNull.Value ? (bool?)reader["IsParticularSection"] : null,
                                IsParticularClass = reader["IsParticularClass"] != DBNull.Value ? (bool?)reader["IsParticularClass"] : null,
                                IsParticularParent = reader["IsParticularParent"] != DBNull.Value ? (bool?)reader["IsParticularParent"] : null,
                                Message = reader["Message"].ToString(),
                                Subject = reader["Subject"].ToString(),
                                OneSignal = reader["OneSignal"].ToString(),
                                Sms = reader["SMS"].ToString(),
                                Ispriority = reader["IsPriority"] != DBNull.Value ? (bool?)reader["IsPriority"] : null,
                                Schooluserid = reader["SchoolUserId"] != DBNull.Value ? (int?)reader["SchoolUserId"] : null, // 27/2/2024 Sanduni
                            }));

                        }
                    }
                }
                return res.AsQueryable();
            }
        }
        public async Task<IQueryable<TNoticeboardmessage>> GetSp_GetNoticeBoardMsgBySchoolId(int schoolid, int userid, int? standardId, int? sectionId, string searchString, int? pagesize, int? pagenumber,int isSelectAll)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<TNoticeboardmessage>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetNoticeBoardMsgBySchoolIdandSchooluser, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                command.Parameters["@schoolid"].Value = schoolid;
                command.Parameters.Add(new SqlParameter("@userid", SqlDbType.Int));
                command.Parameters["@userid"].Value = userid;
                command.Parameters.Add(new SqlParameter("@StandardId", SqlDbType.Int));
                command.Parameters["@StandardId"].Value = (object)standardId ?? DBNull.Value;

                command.Parameters.Add(new SqlParameter("@SectionId", SqlDbType.Int));
                command.Parameters["@SectionId"].Value = (object)sectionId ?? DBNull.Value;

                command.Parameters.Add(new SqlParameter("@searchString", SqlDbType.NVarChar));
                command.Parameters["@searchString"].Value = searchString;

                command.Parameters.Add(new SqlParameter("@pagesize", SqlDbType.Int));
                command.Parameters["@pagesize"].Value = pagesize;
                command.Parameters.Add(new SqlParameter("@pagenumber", SqlDbType.Int));
                command.Parameters["@pagenumber"].Value = pagenumber;
                command.Parameters.Add(new SqlParameter("@isSelectAll", SqlDbType.Int));
                command.Parameters["@isSelectAll"].Value = isSelectAll;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new TNoticeboardmessage()
                            {
                                Id = (int)reader["Id"],
                                Attachments = reader["attachments"].ToString(),
                                Createddate = (DateTime)reader["createddate"],
                                Standardsectionmappingid = reader["standardsectionmappingid"] != DBNull.Value ? (int?)reader["standardsectionmappingid"] : null,
                                IsEntireSchool = reader["IsEntireSchool"] != DBNull.Value ? (bool?)reader["IsEntireSchool"] : null,
                                IsParticularSection = reader["IsParticularSection"] != DBNull.Value ? (bool?)reader["IsParticularSection"] : null,
                                IsParticularClass = reader["IsParticularClass"] != DBNull.Value ? (bool?)reader["IsParticularClass"] : null,
                                IsParticularParent = reader["IsParticularParent"] != DBNull.Value ? (bool?)reader["IsParticularParent"] : null,
                                Message = reader["Message"].ToString(),
                                Subject = reader["Subject"].ToString(),
                                OneSignal = reader["OneSignal"].ToString(),
                                Sms = reader["SMS"].ToString(),
                                Isemail = reader["isemail"] != DBNull.Value ? (bool?)reader["isemail"] : null,
                                Ispriority = reader["IsPriority"] != DBNull.Value ? (bool?)reader["IsPriority"] : null,
                                Schooluserid = reader["SchoolUserId"] != DBNull.Value ? (int?)reader["SchoolUserId"] : null, // 27/2/2024 Sanduni
                            }));

                        }
                    }
                }
                return res.AsQueryable();
            }
        }

      






        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }

    }

    //sanduni15march2024
        public class GetSchooluserids
    {
        public int? Parentid { get; set; }
        public string ParentName { get; set; }
        public int? RelationId { get; set; }
        public int? ChildId { get; set; }
    }
    public class GetNoticeboardmsgs
    {
        public int? Id { get; set; }
        public string attachments { get; set; }
        public int? standardsectionmappingid { get; set; }
        public DateTime? createddate { get; set; }
        public int? IsEntireSchool { get; set; }
        public int? IsParticularClass { get; set; }
        public int? IsParticularSection { get; set; }
        public int? IsParticularParent { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int? SchoolUserId { get; set; }
        public string OneSignal { get; set; }
        public string SMS { get; set; }
        public int? IsPriority { get; set; }
        public string ParentName { get; set; }
        public int? RelationId { get; set; }
        public int? ChildId { get; set; }
    }
    public class GetParentsSP
    {
        public int? Parentid { get; set; }
        public string ParentName { get; set; }
        public int? RelationId { get; set; }
        public int? ChildId { get; set; }
        public string ChildEmail { get; set; }
        public int? ChildSchoolMappingId { get; set; }
        public string ChildName { get; set; }
        public string ChildFirstName { get; set; }
        public string ChildLastName { get; set; }
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public int? StandardId { get; set; }
        public string StandardName { get; set; }
        public int SecondParent { get; set; }
        public string RegistrationNumber { get; set; }
        public int? DRCEnable1 { get; set; }
        public int? DRCEnable2 { get; set; }
        public int? DRCEnable3 { get; set; }
        public int? StatusId { get; set; } // 27/2/2024 Sanduni


    }



}
