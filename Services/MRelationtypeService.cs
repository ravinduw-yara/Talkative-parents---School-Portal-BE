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
    public interface IMRelationtypeService : ICommonService
    {
        Task<int> AddEntity(MRelationtype entity);
        Task<MRelationtype> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MRelationtype entity);
        Task<IQueryable<object>> GetEntityByType(string EntityType);
    }
    public class MRelationtypeService : IMRelationtypeService
    {
        private readonly IRepository<MRelationtype> repository;
        private DbSet<MRelationtype> localDBSet;

        public MRelationtypeService(IRepository<MRelationtype> repository)
        {
            this.repository = repository;
        }
        public async Task<int> AddEntity(MRelationtype entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MRelationtype>)await this.repository.GetAll();

        private static Object Mapper(MRelationtype x) => new
        {
            x.Id,
            x.Type,
            x.Icon,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };

        private async Task<IQueryable<MRelationtype>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MRelationtype> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByType(string EntityType) => (await this.GetAllEntitiesPvt()).Where(x => x.Type.Equals(EntityType.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(MRelationtype entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        //MRelationtype does not have name
        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }
    }
}
