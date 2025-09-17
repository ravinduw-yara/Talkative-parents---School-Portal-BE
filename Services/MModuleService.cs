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
    public interface IMModuleService : ICommonService
    {
        Task<int> AddEntity(MModule entity);
        Task<MModule> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MModule entity);

    }
    public class MModuleService : IMModuleService
    {
        private readonly IRepository<MModule> repository;
        private DbSet<MModule> localDBSet;

        public MModuleService(IRepository<MModule> repository)
        {
            this.repository = repository;
        }

        public async Task<int> AddEntity(MModule entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MModule>)await this.repository.GetAll();

        private static Object Mapper(MModule x) => new
        {
            x.Id,
            x.State,
            x.Name,
            x.Icon,
            x.Selected,
            x.Type,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };

        private async Task<IQueryable<MModule>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.MUsermodulemappings)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MModule> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(MModule entity)
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
