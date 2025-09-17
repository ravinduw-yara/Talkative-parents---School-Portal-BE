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
using static Services.MHiEduSubjectExamMappingService;
using static Services.MHiEduSubSubjectService;


namespace Services
{
    public interface IMHiEduSubSubjectService : ICommonService
    {
        Task<int> AddEntity(MHiEduSubSubject entity);
        Task<object> GetEntityBySubSubJectID6(int EntityID);
        Task<int> DeleteEntity(MHiEduSubSubject entity);
        Task<MHiEduSubSubject> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MHiEduSubSubject entity);
        Task<List<HieduSubSubjectModel>> GetEntityBySubSubJectID5(int? entityID);
        //Task<List<SubSubjectDetailsModel>> GetSubSubjectNameByCourseSemesterSubjectSP(int courseId, int batchId, int subjectId);
        Task<List<SubSubjectDetailsModel>> GetSubSubjectNames(int courseId, int semesterId, int subjectId);

    }
    public class MHiEduSubSubjectService : IMHiEduSubSubjectService
    {
        private readonly IRepository<MHiEduSubSubject> repository;
        private DbSet<MHiEduSubSubject> localDBSet;
        private readonly IConfiguration configuration;
        readonly TpContext db = new TpContext();

        public MHiEduSubSubjectService(
            IRepository<MHiEduSubSubject> repository,
            IConfiguration configuration
            )
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        #region base HiEduSubSubjects
        private async Task AllEntityValue() => localDBSet = (Microsoft.EntityFrameworkCore.DbSet<MHiEduSubSubject>)await this.repository.GetAll();
        // private async Task AllEntityValue() => localDBSet = (DbSet<MHiEduSubSubject>)await this.repository.GetAll();
        private static Object Mapper(MHiEduSubSubject x) => new
        {
            x.Id,
            x.SubSubjectName,
            x.SubjectId


        };

        private async Task<IQueryable<MHiEduSubSubject>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.SubjectId);

        }



        public async Task<int> AddEntity(MHiEduSubSubject entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MHiEduSubSubject> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.SubSubjectName.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<object> GetEntityBySubSubJectID6(int entityID) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.Id == entityID).Select(x => Mapper(x));



        public async Task<int> UpdateEntity(MHiEduSubSubject entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MHiEduSubSubject entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<List<HieduSubSubjectModel>> GetEntityBySubSubJectID5(int? entityID)
        {
            IQueryable<MHiEduSubSubject> entities = await GetAllEntitiesPvt();

            List<MHiEduSubSubject> classList = entities.Where(a => a.Id == entityID).ToList();

            if (classList.Count != 0)
            {
                var HiedusubsubjectLists = classList;
                List<HieduSubSubjectModel> Hiedusubsubjects = new List<HieduSubSubjectModel>();

                HiedusubsubjectLists.ForEach(a =>
                {

                    HieduSubSubjectModel Hiedusubsubject = new HieduSubSubjectModel();
                    Hiedusubsubject.SubSubjectName = a.SubSubjectName;
                    Hiedusubsubject.SubjectId = a.SubjectId;
                    Hiedusubsubjects.Add(Hiedusubsubject);
                });
                return Hiedusubsubjects;
            }
            return null;
            #endregion
        }

        public class HieduSubSubjectModel
        {

            public string SubSubjectName { get; set; }
            public int SubjectId { get; set; }

        }
        //11/04/2023--- jaliya-----------------
 

        public class SubSubjectDetailsModel
        {
            public string SubSubjectName { get; set; }
            public int SubSubjectId { get; set; }
        }

        public async Task<List<SubSubjectDetailsModel>> GetSubSubjectNames(int courseId, int semesterId, int subjectId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                var res = new List<SubSubjectDetailsModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.HiEduGetSubSubject, connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@CourseId", SqlDbType.Int));
                command.Parameters["@CourseId"].Value = courseId;
                command.Parameters.Add(new SqlParameter("@SemesterId", SqlDbType.Int));
                command.Parameters["@SemesterId"].Value = semesterId;
                command.Parameters.Add(new SqlParameter("@SubjectId", SqlDbType.Int));
                command.Parameters["@SubjectId"].Value = subjectId;


                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new SubSubjectDetailsModel
                            {
                                SubSubjectName = reader["SubSubjectName"].ToString(),
                                SubSubjectId = (int)reader["Id"],
                            }));
                        }

                    }
                    return res;

                }
            }
        }

    }
}

