using CommonUtility.RequestModels;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITClassCalendereventsService : ICommonService
    {
        Task<int> AddEntity(TClassCalenderevents entity);
        Task<int> UpdateEntity(TClassCalenderevents entity);
        Task<int> DeleteEntity(TClassCalenderevents entity); 

    }
    public class TClassCalendereventsService : ITClassCalendereventsService
    {
        private readonly IRepository<TClassCalenderevents> repository;

        public TClassCalendereventsService(IRepository<TClassCalenderevents> repository, TpContext tpContext)
        {
            this.repository = repository;

            this.tpContext = tpContext;
        }

        private DbSet<TClassCalenderevents> localDBSet;
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext tpContext;

        public async Task<int> AddEntity(TClassCalenderevents entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<TClassCalenderevents>)await this.repository.GetAll();

        private static Object Mapper(TClassCalenderevents x) => new
        {
            x.Id,
            x.schoolid,
            x.sectionId,
            x.calendereventid
        };

      



        private async Task<IQueryable<TClassCalenderevents>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet;
        }


        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        //public async Task<TClassCalenderevents> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) =>
            (await this.GetAllEntitiesPvt())
            .Where(x => x.Id.Equals(entityID))
            .Select(x => Mapper(x))
            .SingleOrDefault();



        public async Task<int> UpdateEntity(TClassCalenderevents entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(TClassCalenderevents entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
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
