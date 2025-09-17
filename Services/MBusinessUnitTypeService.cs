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
    public interface IMBusinessUnitTypeService : ICommonService
    {
        Task<int> AddEntity(MBusinessunittype entity);
        Task<MBusinessunittype> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MBusinessunittype entity);
        Task<IQueryable<object>> GetEntityByType(string EntityType);

    }
    public class MBusinessUnitTypeService : IMBusinessUnitTypeService
    {
        private readonly IRepository<MBusinessunittype> repository;
        private DbSet<MBusinessunittype> localDBSet;

        public MBusinessUnitTypeService(IRepository<MBusinessunittype> repository)
        {
            this.repository = repository;
        }

        public async Task<int> AddEntity(MBusinessunittype entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MBusinessunittype>)await this.repository.GetAll();

        private static Object Mapper(MBusinessunittype x) => new
        {
            x.Id,
            x.Type,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };

        private async Task<IQueryable<MBusinessunittype>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MBusinessunittype> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        //Buisnessunittype does not have a name
        //public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));
        public async Task<IQueryable<object>> GetEntityByType(string EntityType) => (await this.GetAllEntitiesPvt()).Where(x => x.Type.Equals(EntityType.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(MBusinessunittype entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        //Buisnessunittype does not have a name
        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }
    }

}
