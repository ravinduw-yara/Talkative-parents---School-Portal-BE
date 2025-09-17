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
    public interface IMUsermodulemappingService : ICommonService
    {
        Task<int> AddEntity(MUsermodulemapping entity);
        Task<MUsermodulemapping> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MUsermodulemapping entity);

    }

    public class MUsermodulemappingService : IMUsermodulemappingService
    {
        private readonly IRepository<MUsermodulemapping> repository;
        private DbSet<MUsermodulemapping> localDBSet;
        public MUsermodulemappingService(IRepository<MUsermodulemapping> _repository)
        {
            this.repository = _repository;
        }
        private async Task AllEntityValue() => localDBSet = (DbSet<MUsermodulemapping>)await this.repository.GetAll();

        private static Object Mapper(MUsermodulemapping x) => new
        {
            x.Id,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            },
            Schooluser = new
            {
                Schooluser = x.Schooluser != null ? x.Schooluser.Firstname : string.Empty,
                x.Schooluserid
            },
            Module = new
            {
                Module = x.Module != null ? x.Module.Name : string.Empty,
                x.Moduleid
            }
        };
        private async Task<IQueryable<MUsermodulemapping>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet.Include(x => x.Status)
           .Include(x => x.CreatedbyNavigation)
           .Include(x => x.Module)
           .Include(x => x.Schooluser)
           .Include(x => x.ModifiedbyNavigation);
        }
        public async Task<int> AddEntity(MUsermodulemapping entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<IQueryable<MUsermodulemapping>> GetAllMUsermodulemappings()
         => await this.repository.GetAll();


        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MUsermodulemapping> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID)
         => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<int> UpdateEntity(MUsermodulemapping entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }
    }
}

