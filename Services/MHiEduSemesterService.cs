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
using static Services.MHiEduSemesterService;


namespace Services
{
    public interface IMHiEduSemesterService : ICommonService
    {
        Task<int> AddEntity(MHiEduSemester entity);
        Task<object> GetEntityBySchoolID1(int EntityID);
        Task<int> DeleteEntity(MHiEduSemester entity);
        Task<MHiEduSemester> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MHiEduSemester entity);
        Task<List<SemestersModels>> GetEntityBySchoolID4(int? entityID);
        // Task<List<MHieduSemesterModel>> GetEntityBySchoolID3(int? entityID);
    }
    public class MHiEduSemesterService : IMHiEduSemesterService
    {
        private readonly IRepository<MHiEduSemester> repository;
        private DbSet<MHiEduSemester> localDBSet;

        public MHiEduSemesterService(IRepository<MHiEduSemester> repository)
        {
            this.repository = repository;
        }

        #region base levels
        private async Task AllEntityValue() => localDBSet = (DbSet<MHiEduSemester>)await this.repository.GetAll();

        private static Object Mapper(MHiEduSemester x) => new
        {
            x.Id,
            x.SemesterName,
            x.CourseId
        };

        private async Task<IQueryable<MHiEduSemester>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CourseId);

        }



        public async Task<int> AddEntity(MHiEduSemester entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MHiEduSemester> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.SemesterName.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<object> GetEntityBySchoolID1(int entityID) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.CourseId == entityID).Select(x => Mapper(x));


        public async Task<int> UpdateEntity(MHiEduSemester entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MHiEduSemester entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<List<SemestersModels>> GetEntityBySchoolID4(int? entityID)
        {
            IQueryable<MHiEduSemester> entities = await GetAllEntitiesPvt();

            List<MHiEduSemester> classList = entities.Where(a => a.CourseId == entityID).ToList();

            if (classList.Count != 0)
            {
                var levelsLists = classList;
                List<SemestersModels> levels = new List<SemestersModels>();

                levelsLists.ForEach(a =>
                {

                    SemestersModels level = new SemestersModels();
                    level.semesterId = a.Id;
                    level.semestersname = a.SemesterName;
                    levels.Add(level);
                });
                return levels;
            }
            return null;
            #endregion
        }

        public class SemestersModels
        {
            public int semesterId { get; set; }
            public string semestersname { get; set; }


        }
    }
}
