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


namespace Services
{
    public interface IMHiEduSubjectExamMappingService : ICommonService
    {
        Task<int> AddEntity(HiEdu_SubjectExamMapping entity);
        Task<object> GetEntityBySubjectExamMappingID6(int EntityID);
        Task<int> DeleteEntity(HiEdu_SubjectExamMapping entity);
        Task<HiEdu_SubjectExamMapping> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(HiEdu_SubjectExamMapping entity);
        Task<List<HieduSubjectExamMappingModel>> GetEntityBySubjectExamMappingID5(int? entityID);
        //ask<IQueryable<HiEdu_SubjectExamMapping>> GetAllEntities(int? entityID);
        //Task<int> GetSemesterCourseMappingId(int id);
        Task<List<SemesterModuleModel>> GetSemesterModuleByCourseAndSemesterIdAsync(int courseId, int semesterId, int examId);

    }
    public class MHiEduSubjectExamMappingService : IMHiEduSubjectExamMappingService
    {
        private readonly IRepository<HiEdu_SubjectExamMapping> repository;
        private DbSet<HiEdu_SubjectExamMapping> localDBSet;
        private readonly IConfiguration configuration;
        readonly TpContext db = new TpContext();

        public MHiEduSubjectExamMappingService(
           IRepository<HiEdu_SubjectExamMapping> repository,
           IConfiguration configuration
           )
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        #region base SubjectCourseSemesterMapping
        private async Task AllEntityValue() => localDBSet = (DbSet<HiEdu_SubjectExamMapping>)await this.repository.GetAll();

        private static Object Mapper(HiEdu_SubjectExamMapping x) => new
        {
            x.Id,
            x.Subject,
            x.CourseSemesterExamId

        };

        private async Task<IQueryable<HiEdu_SubjectExamMapping>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CourseSemesterExamId);

        }
        // public async Task<int> GetSemesterCourseMappingId(int id)
        //{
        //  var semesterCourseMappingId = await localDBSet
        //   .Where(a => a.Id == id)
        //    .Select(b => b.Id)
        //    .FirstOrDefaultAsync();

        // return semesterCourseMappingId;
        // }




        public async Task<int> AddEntity(HiEdu_SubjectExamMapping entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        //public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<HiEdu_SubjectExamMapping> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        //public async Task<object> GetEntityByID(int entityID)
        //=> (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        //public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
        //    (await this.GetAllEntitiesPvt()).Where(x => x.Subject.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<object> GetEntityBySubjectExamMappingID6(int entityID) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.CourseSemesterExamId == entityID).Select(x => Mapper(x));



        public async Task<int> UpdateEntity(HiEdu_SubjectExamMapping entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(HiEdu_SubjectExamMapping entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<List<HieduSubjectExamMappingModel>> GetEntityBySubjectExamMappingID5(int? entityID)
        {
            IQueryable<HiEdu_SubjectExamMapping> entities = await GetAllEntitiesPvt();

            List<HiEdu_SubjectExamMapping> classList = entities.Where(a => a.CourseSemesterExamId == entityID).ToList();

            if (classList.Count != 0)
            {
                var MHieduSubjectExamMappingLists = classList;
                List<HieduSubjectExamMappingModel> Hiedusubjects = new List<HieduSubjectExamMappingModel>();

                MHieduSubjectExamMappingLists.ForEach(a =>
                {

                    HieduSubjectExamMappingModel Hiedusubject = new HieduSubjectExamMappingModel();
                    Hiedusubject.Subject = a.Subject;
                    Hiedusubjects.Add(Hiedusubject);
                });
                return Hiedusubjects;
            }
            return null;

        }
        #endregion
        public class HieduSubjectExamMappingModel
        {

            public string Subject { get; set; }
            public int CourseSemesterExamId { get; set; }
        }

        public async Task<List<SemesterModuleModel>> GetSemesterModuleByCourseAndSemesterIdAsync(int courseId, int semesterId,int examId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                var res = new List<SemesterModuleModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.HiEduGetSemesterModule, connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@CourseId", SqlDbType.Int));
                command.Parameters["@CourseId"].Value = courseId;
                command.Parameters.Add(new SqlParameter("@SemesterId", SqlDbType.NVarChar));
                command.Parameters["@SemesterId"].Value = semesterId;
                command.Parameters.Add(new SqlParameter("@ExamId", SqlDbType.Int));
                command.Parameters["@ExamId"].Value = examId;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new SemesterModuleModel
                            {
                                SubjectName = reader["SubjectName"].ToString(),
                                SubjectId = (int)reader["SubjectId"],
                            }));
                        }
                    }
                    return res;
                }
            }
        }

        public Task<IQueryable<object>> GetAllEntities()
        {
            throw new NotImplementedException();
        }

        public Task<object> GetEntityByID(int entityID)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }

        public class SemesterModuleModel
        {

            public string SubjectName { get; set; }
            public int SubjectId { get; set; }

        }


    }
}
