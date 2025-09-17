using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Services.MLevelService;

namespace Services
{
    //dinidu
    public interface IMLevelService : ICommonService
    {
        Task<int> AddEntity(MLevel entity);
        Task<object> GetEntityBySchoolID1(int EntityID);
        Task<int> DeleteEntity(MLevel entity);
        Task<MLevel> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MLevel entity);
        Task<List<LevelModel>> GetEntityBySchoolID3(int? entityID);
    }
    public class MLevelService : IMLevelService
    {
        private readonly IRepository<MLevel> repository;
        private DbSet<MLevel> localDBSet;

        public MLevelService(IRepository<MLevel> repository)
        {
            this.repository = repository;
        }

        #region base levels
        private async Task AllEntityValue() => localDBSet = (DbSet<MLevel>)await this.repository.GetAll();

        private static Object Mapper(MLevel x) => new
        {
            x.Id,
            x.levels,
            x.schoolid
        };

        private async Task<IQueryable<MLevel>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.School);

        }



        public async Task<int> AddEntity(MLevel entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MLevel> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.levels.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<object> GetEntityBySchoolID1(int entityID) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.schoolid == entityID).Select(x => Mapper(x));


        public async Task<int> UpdateEntity(MLevel entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MLevel entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<List<LevelModel>> GetEntityBySchoolID3(int? entityID)
        {
            IQueryable<MLevel> entities = await GetAllEntitiesPvt();

            List<MLevel> classList = entities.Where(a => a.schoolid == entityID).ToList();

            if (classList.Count != 0)
            {
                var levelsLists = classList;
                List<LevelModel> levels = new List<LevelModel>();

                levelsLists.ForEach(a =>
                {

                    LevelModel level = new LevelModel();
                    level.Id = a.Id;
                    level.levelsname = a.levels;
                    levels.Add(level);
                });
                return levels;
            }
            return null;
            #endregion
        }
        public class LevelModel
        {
            public int Id { get; set; }
            public string levelsname { get; set; }


        }
    }
}