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
    public interface IMStatustypeService : ICommonService
    {
        Task<int> AddEntity(MStatustype entity);
        Task<MStatustype> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MStatustype entity);

    }
    public class MStatustypeService : IMStatustypeService
    {
        private readonly IRepository<MStatustype> repository;
        private DbSet<MStatustype> localDBSet;

        public MStatustypeService(IRepository<MStatustype> repository)
        {
            this.repository = repository;
        }

        public async Task<int> AddEntity(MStatustype entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MStatustype>)await this.repository.GetAll();

        private static Object Mapper(MStatustype x) => new
        {
            x.Id,
            x.Name,
            x.Description,
            x.Enddate,
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
            //},
        };

        public async Task<MStatustype> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        private async Task<IQueryable<MStatustype>> GetAllMStatustypes()
        {
            await AllEntityValue();
            return this.localDBSet.Include(x => x.CreatedbyNavigation)
               .Include(x => x.ModifiedbyNavigation);
        }
        public async Task<object> GetEntityByID(int entityID) =>
        (await this.GetAllMStatustypes()).Where(x => x.Id.Equals((short)entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllMStatustypes()).Select(x => Mapper(x));

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllMStatustypes()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));


        public async Task<int> UpdateEntity(MStatustype entity)
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
