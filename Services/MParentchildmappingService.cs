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
    public interface IMParentchildmappingService : ICommonService
    {
        Task<int> AddEntity(MParentchildmapping entity);
        Task<MParentchildmapping> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MParentchildmapping entity);

    }

    public class MParentchildmappingService : IMParentchildmappingService
    {
        private readonly IRepository<MParentchildmapping> repository;
        private DbSet<MParentchildmapping> localDBSet;
        public MParentchildmappingService(IRepository<MParentchildmapping> _repository)
        {
            this.repository = _repository;
        }
        private async Task AllEntityValue() => localDBSet = (DbSet<MParentchildmapping>)await this.repository.GetAll();

        private static Object Mapper(MParentchildmapping x) => new
        {
            x.Id,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            },
            Child = new
            {
                Child = x.Child != null ? x.Child.Firstname : string.Empty,
                x.Childid
            },
            Appuser = new
            {
                Appuser = x.Appuser != null ? x.Appuser.Firstname : string.Empty,
                x.Appuserid
            },
            Relationtype = new
            {
                Relationtype = x.Relationtype != null ? x.Relationtype.Type : string.Empty,
                x.Relationtypeid
            }
        };
        private async Task<IQueryable<MParentchildmapping>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet.Include(x => x.Status)
           .Include(x => x.CreatedbyNavigation)
           .Include(x => x.Appuser)
           .Include(x => x.Child)
           .Include(x => x.Relationtype)
           .Include(x => x.ModifiedbyNavigation);
        }
        public async Task<int> AddEntity(MParentchildmapping entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<IQueryable<MParentchildmapping>> GetAllMParentchildmappings()
         => await this.repository.GetAll();


        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MParentchildmapping> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID)
         => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<int> UpdateEntity(MParentchildmapping entity)
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

