using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using net.openstack.Core.Domain;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Services.MSubjectService;

namespace Services
{
    public interface IMSubjectsService : ICommonService
    {
        Task<int> AddEntity(MSubject entity);
        Task<object> GetEntityBySchoolID(int EntityID);
        Task<int> DeleteEntity(MSubject entity);
        Task<MSubject> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MSubject entity);
        Task<List<SubjectModel>> GetEntityBySchoolID2(int? entityID); 
        Task<object> SetGradeLevelAPI(postGradeLeveModel postgradeLevemodel);
    }
    public class MSubjectService : IMSubjectsService
    {
        private readonly IRepository<MSubject> repository;
        private readonly TpContext tpContext;

        //private DbSet<GetAuthReport> localDBSet;
        
        private DbSet<MSubject> localDBSet;
        TpContext db = new TpContext();
        private readonly IConfiguration configuration;
        private readonly string _connectionString;
        public MSubjectService(IRepository<MSubject> repository, IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;

        }
        //public MSubjectService(IConfiguration configuration)
        //{
        //    _connectionString = configuration.GetConnectionString("TpConnectionString");
        //    this.configuration = configuration;
        //}
        #region base services
        private async Task AllEntityValue() => localDBSet = (DbSet<MSubject>)await this.repository.GetAll();

        private static Object Mapper(MSubject x) => new
        {
            x.Id,
            x.Name,
            x.Description,
            x.BranchId,
            x.Statusid
        };

        private async Task<IQueryable<MSubject>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Branch)
            .Include(x => x.Status);
        }



        public async Task<int> AddEntity(MSubject entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MSubject> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<object> GetEntityBySchoolID(int entityID) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.Branch.Schoolid == entityID).Select(x => Mapper(x));


        public async Task<int> UpdateEntity(MSubject entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MSubject entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }
        //assigngradelevel-sanduni
        public async Task<object> SetGradeLevelAPI(postGradeLeveModel model)
        {
            try
            {
                if (model.Levelid != 0)
                {
                    var objresult = SetGradeLevelSP(model);
                    if (objresult != null)
                    {

                        var items = objresult;
                        var obj = new
                        {
                            items = items
                        };
                        return obj;
                    }
                    else
                    {
                        return (new
                        {
                            Message = "No data found",
                            StatusCode = HttpStatusCode.NotFound

                        });
                    }

                }

                else
                {
                    return (new
                    {
                        Message = "Enter Phone Number",
                        StatusCode = HttpStatusCode.NotFound

                    });
                }
                return "Ok";
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
        public async Task<List<SubjectModel>> GetEntityBySchoolID2(int? entityID)
        {
            IQueryable<MSubject> entities = await GetAllEntitiesPvt();

            List<MSubject> classList = entities.Where(a => a.Branch.Schoolid == entityID).ToList();

            if (classList.Count != 0)
            {
                var subjectsLists = classList;
                List<SubjectModel> subjects = new List<SubjectModel>();

                subjectsLists.ForEach(a =>
                {

                    SubjectModel subject = new SubjectModel();
                    subject.subjectId = a.Id;
                    subject.subjectName = a.Name;
                    subjects.Add(subject);
                });
                return subjects;
            }
            return null;
            #endregion
        }
        //public async Task<List<MLevelModel>> GetLevelsBySchoolID(int? schoolid)
        //{
           
        //    return null;

        //}

        private async Task<Object> SetGradeLevelSP(postGradeLeveModel model)
        {
            try
            {
                postGradeLeveModel gradelevellist = new postGradeLeveModel();
                //var sectionlistcount = model.sectionidlist.Count();
                //var check1 = await this.db.MLevels.Where(x => x.levels.Equals(model.Levelid)).FirstOrDefaultAsync();
                if (model != null)
                {
                    foreach (var item in model.sectionidlist)
                    {
                        //SectionIdList sectionlist = new SectionIdList();

                        var spsectionid = item.sectionid;
                        var levelid = model.Levelid;
                        var status = "";
                        string connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {

                            //var res = new List<GetAuthReport>();
                            connection.Open();
                            SqlCommand command = new SqlCommand(ApplicationConstants.AddGradeLevelSp, connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@levelid", SqlDbType.Int));
                            command.Parameters["@levelid"].Value = levelid;
                            command.Parameters.Add(new SqlParameter("@sectionid", SqlDbType.Int));
                            command.Parameters["@sectionid"].Value = spsectionid;
                            command.Parameters.Add(new SqlParameter("@status", SqlDbType.NVarChar));
                            command.Parameters["@status"].Value = status;
                            await command.ExecuteNonQueryAsync();
                        }
                        db.SaveChanges();
                    }
                        gradelevellist.Levelid = model.Levelid;
                        
                        gradelevellist.status = "Sucessfully Mapped";

                    return (gradelevellist);

                    }
                    else
                    {
                    gradelevellist.status = "Error in Mapping";
                    return (gradelevellist);
                    }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public class SubjectModel
        {
            public int subjectId { get; set; }
            public string subjectName { get; set; }
        }
    }
}
