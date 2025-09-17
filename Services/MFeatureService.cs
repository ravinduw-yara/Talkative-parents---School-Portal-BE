using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Confirm
namespace Services
{
    public interface IMFeatureService : ICommonService
    {
        Task<int> AddEntity(MFeature entity);
        Task<MFeature> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MFeature entity);

    }
    public class MFeatureService : IMFeatureService
    {
        private readonly IRepository<MFeature> repository;
        private DbSet<MFeature> localDBSet;

        public MFeatureService(IRepository<MFeature> repository)
        {
            this.repository = repository;
        }

        public async Task<int> AddEntity(MFeature entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MFeature>)await this.repository.GetAll();

        private static Object Mapper(MFeature x) => new
        {
            x.Id,
            x.Schoolid,
            x.Maxmsgcount,
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

        private async Task<IQueryable<MFeature>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MFeature> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        //Confirm - Is it required?? No, as Name field is not available
        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Schoolid.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(MFeature entity)
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
