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
    public interface IMLocationtypeService : ICommonService
    {
        Task<int> AddEntity(MLocationtype entity);
        Task<MLocationtype> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MLocationtype entity);
        Task<IQueryable<object>> GetEntityByType(string EntityType);

    }
    public class MLocationtypeService : IMLocationtypeService
    {
        private readonly IRepository<MLocationtype> repository;
        private DbSet<MLocationtype> localDBSet;

        public MLocationtypeService(IRepository<MLocationtype> repository)
        {
            this.repository = repository;
        }
        public async Task<int> AddEntity(MLocationtype entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MLocationtype>)await this.repository.GetAll();

        private static Object Mapper(MLocationtype x) => new
        {
            x.Id,
            x.Type,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };

        private async Task<IQueryable<MLocationtype>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MLocationtype> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByType(string EntityType) => (await this.GetAllEntitiesPvt()).Where(x => x.Type.Equals(EntityType.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(MLocationtype entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        //MLocationtype does not have name
        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }

    }
}
