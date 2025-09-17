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
using static Services.MSubSubjectService;


namespace Services
{
    public interface IMSubSubjectService : ICommonService
    {
        Task<int> AddEntity(MSubSubject entity);
        Task<object> GetEntityBySchoolID6(int EntityID);
        Task<int> DeleteEntity(MSubSubject entity);
        Task<MSubSubject> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MSubSubject entity);
        Task<List<SubSubjectModel>> GetEntityBySchoolID5(int? entityID);
    }
    public class MSubSubjectService : IMSubSubjectService
    {
        private readonly IRepository<MSubSubject> repository;
        private DbSet<MSubSubject> localDBSet;

        public MSubSubjectService(IRepository<MSubSubject> repository)
        {
            this.repository = repository;
        }

        #region base SubSubjects
        private async Task AllEntityValue() => localDBSet = (DbSet<MSubSubject>)await this.repository.GetAll();

        private static Object Mapper(MSubSubject x) => new
        {
            x.Id,
            x.SubSubject,
            x.SubjectId,
            x.Precentage,
            x.SubMaxMarks
        };

        private async Task<IQueryable<MSubSubject>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.SubjectId);

        }



        public async Task<int> AddEntity(MSubSubject entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MSubSubject> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.SubSubject.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<object> GetEntityBySchoolID6(int entityID) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.SubjectId == entityID).Select(x => Mapper(x));



        public async Task<int> UpdateEntity(MSubSubject entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MSubSubject entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<List<SubSubjectModel>> GetEntityBySchoolID5(int? entityID)
        {
            IQueryable<MSubSubject> entities = await GetAllEntitiesPvt();

            List<MSubSubject> classList = entities.Where(a => a.SubjectId == entityID).ToList();

            if (classList.Count != 0)
            {
                var SubSubjectLists = classList;
                List<SubSubjectModel> SubSubjects = new List<SubSubjectModel>();

                SubSubjectLists.ForEach(a =>
                {

                    SubSubjectModel SubSubject = new SubSubjectModel();
                    SubSubject.Id = a.Id;
                    SubSubject.SubSubject = a.SubSubject;
                    SubSubject.Percentage = a.Precentage;
                    SubSubject.SubMaxMarks = a.SubMaxMarks;
                    SubSubjects.Add(SubSubject);
                });
                return SubSubjects;
            }
            return null;
            #endregion
        }

        public class SubSubjectModel
        {
            public int Id { get; set; }
            public string SubSubject { get; set; }
            public int? Percentage { get; set; }
            public int? SubMaxMarks { get; set; }


        }



    }
}

