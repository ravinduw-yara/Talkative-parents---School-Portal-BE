using CommonUtility.RequestModels;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public interface ITEmaillogService : ICommonService
    {
        Task<int> AddEntity(TEmaillog entity);
        Task<TEmaillog> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(TEmaillog entity);
        
    }
    public class TEmaillogService : ITEmaillogService
    {
        private readonly IRepository<TEmaillog> repository;
        private DbSet<TEmaillog> localDBSet;

        private readonly TpContext tpContext;

        public TEmaillogService(IRepository<TEmaillog> repository, TpContext tpContext)
        {
            this.repository = repository;
            this.tpContext = tpContext;
        }

        public async Task<int> AddEntity(TEmaillog entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<TEmaillog>)await this.repository.GetAll();

        private static Object Mapper(TEmaillog x) => new
        {
            x.Id,
            x.Fromemailid,
            x.Noticeboardmsgid,
            x.Emailcount,
            x.Toemailid,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };

        private async Task<IQueryable<TEmaillog>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<TEmaillog> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<int> UpdateEntity(TEmaillog entity)
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
