using CommonUtility.RequestModels;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITGclvedioclassService : ICommonService
    {
        Task<int> AddEntity(TGclvedioclass entity);
        Task<TGclvedioclass> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(TGclvedioclass entity);
        Task<int> DeleteEntity(TGclvedioclass entity);
        Task<IQueryable<TGclvedioclass>> getVideoClasses(int schoolId, int appuserid);
    }
    public class TGclvedioclassService : ITGclvedioclassService
    {
        private readonly IRepository<TGclvedioclass> repository;
        private DbSet<TGclvedioclass> localDBSet;
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly IMStandardgroupmappingService mStandardgroupmappingService;

        public TGclvedioclassService(IRepository<TGclvedioclass> repository, IMStandardgroupmappingService mStandardgroupmappingService)
        {
            this.repository = repository;
            this.mStandardgroupmappingService = mStandardgroupmappingService;
        }

        public async Task<int> AddEntity(TGclvedioclass entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<TGclvedioclass>)await this.repository.GetAll();

        private static Object Mapper(TGclvedioclass x) => new
        {
            x.Id,
            x.Name,
            x.Standardsectionmapping,
            x.Meetinglink,
            x.Description,
            x.Startdate,
            x.Enddate,
            x.Createdby,
            x.Statusid
        };

        private async Task<IQueryable<TGclvedioclass>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Standardsectionmapping)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<TGclvedioclass> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(TGclvedioclass entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(TGclvedioclass entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<IQueryable<TGclvedioclass>> getVideoClasses(int schoolId, int appuserid)
        {
            try
            {

                var rank = await this.db.MSchooluserroles.Where(a => a.Schooluserid.Equals(appuserid)).Select(a => a.Category.Roleid).MaxAsync();

                if(rank == 1)
                {
                    return (this.db.TGclvedioclasses);
                }
                else
                {
                    return (this.db.TGclvedioclasses.Where(x => x.Createdby.Equals(appuserid)));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
