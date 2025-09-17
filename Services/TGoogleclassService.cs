using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITGoogleclassService : ICommonService
    {
        Task<int> AddEntity(TGoogleclass entity);
        Task<TGoogleclass> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(TGoogleclass entity);
        Task<int> DeleteEntity(TGoogleclass entity);
        Task<IQueryable<TGoogleclass>> getGcl(int schoolId, int appuserid);
        Task<IQueryable<techclassModel>> getTeachClassSP(int schoolid);
        Task<List<TC_GCL_Res>> GetGCLClassSumamryForAPI(int SchoolId, int appuserid);

    }
    public class TGoogleclassService : ITGoogleclassService
    {
        private readonly IRepository<TGoogleclass> repository;
        private DbSet<TGoogleclass> localDBSet;
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly IMStandardgroupmappingService mStandardgroupmappingService;
        private readonly IConfiguration configuration;

        public TGoogleclassService(IRepository<TGoogleclass> repository, IMStandardgroupmappingService mStandardgroupmappingService, IConfiguration configuration)
        {
            this.repository = repository;
            this.mStandardgroupmappingService = mStandardgroupmappingService;
            this.configuration = configuration;
        }

        public async Task<int> AddEntity(TGoogleclass entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<TGoogleclass>)await this.repository.GetAll();

        private static Object Mapper(TGoogleclass x) => new
        {
            x.Id,
            x.Name,
            x.Standardsectionmapping,
            x.Gcourseid,
            x.Gownerid,
            Status = x.Status != null ? x.Status.Name : string.Empty,
            x.Statusid
        };

        private async Task<IQueryable<TGoogleclass>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Standardsectionmapping)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<TGoogleclass> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(TGoogleclass entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(TGoogleclass entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<IQueryable<TGoogleclass>> getGcl(int schoolId, int appuserid)
        {
            try
            {

                var rank = await this.db.MSchooluserroles.Where(a => a.Schooluserid.Equals(appuserid)).Include(a => a.Category).FirstOrDefaultAsync();

                var rankid = await this.db.MRoles.Where(x => x.Id == rank.Category.Roleid).Select(x => x.Rank).FirstOrDefaultAsync();

                if (rankid  == 1)
                {
                    return (this.db.TGoogleclasses);
                }
                else if(rankid == 4)
                {
                    return (this.db.TGoogleclasses.Where(x => x.Standardsectionmappingid.Equals(rank.Standardsectionmappingid)));
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

        //Alternative service, unused for now. Only use if there is some problem in the future. Serevice call also commented in the API
        public async Task<IQueryable<techclassModel>> getTeachClassSP(int schoolid)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<techclassModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.getTeachClasSP, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolid", SqlDbType.Int));
                command.Parameters["@schoolid"].Value = schoolid;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new techclassModel()
                            {
                                Id = (int)reader["Id"],
                                Name = reader["name"].ToString(),
                                IsApproved = reader["IsApproved"] != DBNull.Value ? (bool?)reader["IsApproved"] : false,
                                branchid = reader["branchid"] != DBNull.Value ? (int?)reader["branchid"] : 0,
                                Standardsectionmappingid = reader["Standardsectionmappingid"] != DBNull.Value ? (int?)reader["Standardsectionmappingid"] : 0,
                                Parentid = reader["Parentid"] != DBNull.Value ? (int?)reader["Parentid"] : 0,
                                TeacherId = reader["TeacherId"] != DBNull.Value ? (int?)reader["TeacherId"] : 0
                            }));
                        }
                    }
                }
                return res.AsQueryable();
            }
        }

        public async Task<List<TC_GCL_Res>> GetGCLClassSumamryForAPI(int SchoolId, int appuserid)
        {
            var rank = await db.MSchooluserroles.Where(x => x.Schooluserid == appuserid).Select(w => w.Category.Role.Rank).FirstOrDefaultAsync();
            List<TC_GCL_Res> res = new List<TC_GCL_Res>();
            if (rank == 1)
            {
                var Tres = db.TGclteacherclasses.Where(x => x.Branchid == SchoolId);
                if (Tres != null)
                {
                    foreach (var item in Tres)
                    {
                        TC_GCL_Res data = new TC_GCL_Res();
                        data.isApproved = (bool)item.IsApproved;

                        var ssm = await db2.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefaultAsync();

                        if (ssm.Parentid == null)
                        {
                            data.standardId = (int)ssm.Id;
                            data.stdName = ssm.Name;
                            data.sectionId = 0;
                            data.sectionName = null;
                        }
                        else
                        {
                            data.standardId = (int)ssm.Parentid;
                            data.stdName = await db2.MStandardsectionmappings.Where(n => n.Id == ssm.Parentid).Select(m => m.Name).FirstOrDefaultAsync(); ;
                            data.sectionId = ssm.Id;
                            data.sectionName = ssm.Name;
                        }
                        res.Add(data);
                    }
                    //return (IQueryable<TC_GCL_Res>)res;
                }
            }
            else if (rank == 4)
            {
                int secid = (int)await db.MSchooluserroles.Where(x => x.Schooluserid == appuserid).Select(w => w.Standardsectionmappingid).FirstOrDefaultAsync();
                var Tres = db.TGclteacherclasses.Where(x => x.Standardsectionmappingid == secid);
                if (Tres != null)
                {
                    foreach (var item in Tres)
                    {
                        TC_GCL_Res data = new TC_GCL_Res();
                        data.isApproved = (bool)item.IsApproved;

                        var ssm = await db2.MStandardsectionmappings.Where(w => w.Id == item.Standardsectionmappingid).FirstOrDefaultAsync();

                        if (ssm.Parentid == null)
                        {
                            data.standardId = (int)ssm.Id;
                            data.stdName = ssm.Name;
                            data.sectionId = 0;
                            data.sectionName = null;
                        }
                        else
                        {
                            data.standardId = (int)ssm.Parentid;
                            data.stdName = await db2.MStandardsectionmappings.Where(n => n.Id == ssm.Parentid).Select(m => m.Name).FirstOrDefaultAsync(); ;
                            data.sectionId = ssm.Id;
                            data.sectionName = ssm.Name;
                        }
                        res.Add(data);
                    }
                }
            }
            return res;
        }

    }
}
