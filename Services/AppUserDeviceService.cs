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
    public interface IAppuserdeviceService : ICommonService
    {
        Task<int> AddEntity(Appuserdevice entity);
        Task<Appuserdevice> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(Appuserdevice entity);

    }

    public class AppuserdeviceService : IAppuserdeviceService
    {
        private readonly IRepository<Appuserdevice> repository;
        private DbSet<Appuserdevice> localDBSet;
        public AppuserdeviceService(IRepository<Appuserdevice> _repository)
        {
            this.repository = _repository;
        }
        private async Task AllEntityValue() => localDBSet = (DbSet<Appuserdevice>)await this.repository.GetAll();

        private static Object Mapper(Appuserdevice x) => new
        {
            x.Id,
            x.Version,
            x.Deviceid,
            x.Devicetype,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            },
            Appuser = new
            {
                Schooluser = x.Appuser != null ? x.Appuser.Firstname : string.Empty,
                x.Appuserid
            },
            //Group = new
            //{
            //    Module = x.Group != null ? x.Group.Name : string.Empty,
            //    x.Groupid
            //}
        };
        private async Task<IQueryable<Appuserdevice>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet.Include(x => x.Status)
           .Include(x => x.CreatedbyNavigation)
           .Include(x => x.ModifiedbyNavigation);
        }
        public async Task<int> AddEntity(Appuserdevice entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<IQueryable<Appuserdevice>> GetAllAppuserdevices()
         => await this.repository.GetAll();


        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<Appuserdevice> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID)
         => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<int> UpdateEntity(Appuserdevice entity)
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

