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
    public interface ITTokenService : ICommonService
    {
        Task<int> AddEntity(TToken entity);
        Task<TToken> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(TToken entity);
        Task<Guid> AddToken(TToken entity);
        Task<Guid> UpdateToken(TToken entity);
        Task<TToken> GetTokenIDForUpdate(Guid entityID);
        Task<Guid> DeleteToken(TToken entity);

    }
    public class TTokenService : ITTokenService
    {
        private readonly IRepository<TToken> repository;
        private DbSet<TToken> localDBSet;

        public TTokenService(IRepository<TToken> repository)
        {
            this.repository = repository;
        }

        public async Task<Guid> AddToken(TToken entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return Guid.Empty;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<TToken>)await this.repository.GetAll();

        private static Object Mapper(TToken x) => new
        {
            x.Id,
            x.Referenceid,
            x.Usertype,
            x.Ttl,
            x.Ipaddress,
            x.Statusid,
        };

        private async Task<IQueryable<TToken>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<TToken> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));
        public async Task<TToken> GetTokenIDForUpdate(Guid entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();


        public async Task<Guid> UpdateToken(TToken entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return Guid.Empty;
        }

        public async Task<Guid> DeleteToken(TToken entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return Guid.Empty;
        }




        Task<int> ITTokenService.AddEntity(TToken entity)
        {
            throw new NotImplementedException();
        }

        Task<int> ITTokenService.UpdateEntity(TToken entity)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }
    }
}
