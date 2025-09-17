using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IMStandardgroupmappingService : ICommonService
    {
        Task<int> AddEntity(MStandardgroupmapping entity);
        Task<MStandardgroupmapping> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MStandardgroupmapping entity);
        Task<int> DeleteEntity(MStandardgroupmapping entity);
        Task<MStandardgroupmapping> GetEntityIDForDelete(int entityID);
        Task<MStandardgroupmapping> GetGroupIDForUpdate(int entityID);

        List<MStandardgroupmapping> GetGroupIDForUpdateV2(int groupid);

        Task<IQueryable<MStandardgroupmapping>> GetGroupIDForBulk(int entityID);

        Task<bool> BulkUpdateEntity(List<MStandardgroupmapping> entity);

    }
    public class MStandardgroupmappingService : IMStandardgroupmappingService
    {
        private readonly IRepository<MStandardgroupmapping> repository;
        private DbSet<MStandardgroupmapping> localDBSet;
        private readonly TpContext db = new TpContext();
        //private readonly MGroupService mGroupServices;

        public MStandardgroupmappingService(IRepository<MStandardgroupmapping> repository/*, MGroupService mGroupServices*/)
        {
            this.repository = repository;
            //this.mGroupServices = mGroupServices;
        }

        public async Task<int> AddEntity(MStandardgroupmapping entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MStandardgroupmapping>)await this.repository.GetAll();

        private static Object Mapper(MStandardgroupmapping x) => new
        {
            x.Id,
            x.Groupid,
            x.Standardsectionmappingid,
            Status = x.Status != null ? x.Status.Name : string.Empty,
            x.Statusid
        };

        private async Task<IQueryable<MStandardgroupmapping>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Group)
            .Include(x => x.Standardsectionmapping)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MStandardgroupmapping> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<MStandardgroupmapping> GetGroupIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.FirstOrDefault(x => x.Groupid.Equals(entityID)));

        public async Task<IQueryable<MStandardgroupmapping>> GetGroupIDForBulk(int entityID) => await Task.Run(() => this.repository.GetAll().Result.Where(x => x.Id.Equals(entityID)));

        public async Task<MStandardgroupmapping> GetEntityIDForDelete(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Groupid.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        //public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(MStandardgroupmapping entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<bool> BulkUpdateEntity(List<MStandardgroupmapping> entity)
        {
            var temp = await this.repository.BulkUpdate(entity);
            if (temp)
            {
                return temp;
            }
            return false;
        }

        public async Task<int> DeleteEntity(MStandardgroupmapping entity)
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

        public List<MStandardgroupmapping> GetGroupIDForUpdateV2(int groupid)
        {
            return db.MStandardgroupmappings.Where(x => x.Groupid.Equals(groupid)).ToList();
        }
    }
}
