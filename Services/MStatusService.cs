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
    public interface IMStatusService : ICommonService
    {
        Task<int> AddEntity(MStatus entity);
        Task<MStatus> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MStatus entity);

    }
    public class MStatusService : IMStatusService
    {
        private readonly IRepository<MStatus> repository;
        private DbSet<MStatus> localDBSet;

        public MStatusService(IRepository<MStatus> repository)
        {
            this.repository = repository;
        }

        public async Task<int> AddEntity(MStatus entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MStatus>)await this.repository.GetAll();

        private static Object Mapper(MStatus x) => new
        {
            x.Id,
            x.Name,
            x.Description,
            x.Isactive,
            StatusType = new
            {
                Status = x.Statustype != null ? x.Statustype.Name : string.Empty,
                x.Statustypeid,
            }
            //CreatedBy = new
            //{
            //    Id = x.Createdby,
            //    Firstname = x.CreatedbyNavigation != null ? x.CreatedbyNavigation.Firstname : string.Empty,
            //    Middlename = x.CreatedbyNavigation != null ? x.CreatedbyNavigation.Middlename : string.Empty,
            //    Lastname = x.CreatedbyNavigation != null ? x.CreatedbyNavigation.Lastname : string.Empty
            //},
            //ModifiedBy = new
            //{
            //    Id = x.Modifiedby,
            //    Firstname = x.ModifiedbyNavigation != null ? x.ModifiedbyNavigation.Firstname : string.Empty,
            //    Middlename = x.ModifiedbyNavigation != null ? x.ModifiedbyNavigation.Middlename : string.Empty,
            //    Lastname = x.ModifiedbyNavigation != null ? x.ModifiedbyNavigation.Lastname : string.Empty
            //}
        };

        private async Task<IQueryable<MStatus>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Where(x => x.Statustype != null);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MStatus> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(MStatus entity)
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
