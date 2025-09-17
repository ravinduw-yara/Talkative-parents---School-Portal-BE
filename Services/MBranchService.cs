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
    public interface IMBranchService : ICommonService
    {
        Task<int> AddEntity(MBranch entity);
        Task<MBranch> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MBranch entity);

    }
    public class MBranchService : IMBranchService
    {
        private readonly IRepository<MBranch> repository;
        private DbSet<MBranch> localDBSet;

        public MBranchService(IRepository<MBranch> repository)
        {
            this.repository = repository;
        }

        public async Task<int> AddEntity(MBranch entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MBranch>)await this.repository.GetAll();

        private static Object Mapper(MBranch x) => new
        {
            x.Id,
            x.Code,
            x.Name,
            x.Description,
            x.Address,
            x.Principalname,
            x.Pincode,
            Locaion = new
            {
                Locaion = x.Locaion != null ? x.Locaion.Name : string.Empty,
                x.Locaionid
            },
            School = new
            {
                School = x.School != null ? x.School.Name : string.Empty,
                x.Schoolid
            },
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };

        private async Task<IQueryable<MBranch>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.School)
            .Include(x => x.Locaion)
            .Include(x => x.MSchooluserinfos)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MBranch> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(MBranch entity)
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
