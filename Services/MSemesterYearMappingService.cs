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
    public interface IMSemesterYearMappingService : ICommonService
    {
        Task<int> AddEntity(MSemesteryearmapping entity);
        Task<MSemesteryearmapping> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MSemesteryearmapping entity);

    }
    public class MSemesterYearMappingService : IMSemesterYearMappingService
    {
        private readonly IRepository<MSemesteryearmapping> repository;
        private DbSet<MSemesteryearmapping> localDBSet;

        public MSemesterYearMappingService(IRepository<MSemesteryearmapping> repository)
        {
            this.repository = repository;
        }

        public async Task<int> AddEntity(MSemesteryearmapping entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MSemesteryearmapping>)await this.repository.GetAll();

        private static Object Mapper(MSemesteryearmapping x) => new
        {
            x.Id,
            x.SemesterId,
            x.AcademicYearId,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };

        private async Task<IQueryable<MSemesteryearmapping>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MSemesteryearmapping> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateEntity(MSemesteryearmapping entity)
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
