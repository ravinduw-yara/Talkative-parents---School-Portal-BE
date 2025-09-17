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
    public interface IMGenderService : ICommonService
    {
        Task<int> AddEntity(MGender entity);
        Task<MGender> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MGender entity);
        Task<IQueryable<object>> GetEntityByType(string EntityType);
    }

    public class MGenderService : IMGenderService
    {
        private readonly IRepository<MGender> repository;
        private DbSet<MGender> localDBSet;

        public MGenderService(IRepository<MGender> repository)
        {
            this.repository = repository;
        }
        public async Task<int> AddEntity(MGender entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MGender>)await this.repository.GetAll();

        private static Object Mapper(MGender x) => new
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

        private async Task<IQueryable<MGender>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MGender> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        //MGender does not have name
        //public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));
        public async Task<IQueryable<object>> GetEntityByType(string EntityType) => (await this.GetAllEntitiesPvt()).Where(x => x.Type.Equals(EntityType.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(MGender entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        //MGender does not have name
        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }

    }
}
