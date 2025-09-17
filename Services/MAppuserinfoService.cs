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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Services.MAppuserinfoService;

namespace Services
{
    public interface IMAppuserinfoService : ICommonService
    {
        Task<int> AddEntity(MAppuserinfo entity);
        Task<MAppuserinfo> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MAppuserinfo entity);
    }
    public class MAppuserinfoService : IMAppuserinfoService
    {
        private readonly IRepository<MAppuserinfo> repository;
        private DbSet<MAppuserinfo> localDBSet;
        private readonly IConfiguration configuration;
        readonly TpContext db = new TpContext();

        public MAppuserinfoService(IRepository<MAppuserinfo> repository, IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MAppuserinfo>)await this.repository.GetAll();

        private static Object Mapper(MAppuserinfo x) => new
        {
            x.Id,
            x.Code,
            x.Dob,
            x.Firstname,
            x.Middlename,
            x.Lastname,
            Gender = x.Genderid,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };


        private async Task<IQueryable<MAppuserinfo>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.MParentchildmappings)
            .Include(x => x.Gender)
            .Include(x => x.MFeatureCreatedbyNavigations)
            .Include(x => x.TSoundingboardmessageAppuserinfos)
            .Include(x => x.TEmaillogCreatedbyNavigations)
            .Include(x => x.TNoticeboardmappings)
            .Include(x => x.AppuserdeviceAppusers)
            .Include(x => x.Status);
        }



        public async Task<int> AddEntity(MAppuserinfo entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MAppuserinfo> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Firstname.Equals(EntityName.Trim())).Select(x => Mapper(x));


        public async Task<int> UpdateEntity(MAppuserinfo entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
    }
}