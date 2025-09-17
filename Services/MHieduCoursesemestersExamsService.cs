using CommonUtility;
using CommonUtility.RequestModels;
using Microsoft.Extensions.Configuration;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace.Objects.Monitoring;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Services.MHiEduCoursesemestersExamsService;
using static System.Collections.Specialized.BitVector32;

namespace Services
{

    public interface IMHiEduCoursesemestersExamsService : ICommonService
    {
        Task<int> AddEntity(MHiEduCourseSemesterExam entity);
        Task<MHiEduCourseSemesterExam> GetEntityIDForUpdate(int entityID);

        Task<int> UpdateEntity(MHiEduCourseSemesterExam entity);

        // Task<object> GetCourseListForAPI(int SchoolId);
        Task<List<CourseSemesteexamModel>> GetCourseSemesterexamAPI(int courseId, int semesterId);


    }
    public class MHiEduCoursesemestersExamsService : IMHiEduCoursesemestersExamsService
    {
        private readonly IRepository<MHiEduCourseSemesterExam> repository;
        private DbSet<MHiEduCourseSemesterExam> localDBSet;
        private readonly IConfiguration configuration;
        readonly TpContext db = new TpContext();

        public MHiEduCoursesemestersExamsService(
            IRepository<MHiEduCourseSemesterExam> repository,
            IConfiguration configuration
            )
        {
            this.repository = repository;
            this.configuration = configuration;
        }
        #region base services
        private async Task AllEntityValue() => localDBSet = (DbSet<MHiEduCourseSemesterExam>)await this.repository.GetAll();

        private static Object Mapper(MHiEduCourseSemesterExam x) => new
        {
            x.Id,
            x.Exam,
            x.SemesterCourseMappingId,

        };
        public async Task<int> UpdateEntity(MHiEduCourseSemesterExam entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MHiEduCourseSemesterExam entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> AddEntity(MHiEduCourseSemesterExam entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }


        public class CourseSemesteexamModel
        {

            public string ExamName { get; set; }
            public int examid { get; set; }

        }
        public async Task<List<CourseSemesteexamModel>> GetCourseSemesterexamAPI(int courseId, int semesterId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                var res = new List<CourseSemesteexamModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.HiEduGetCourseSemesterexam, connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@CourseId", SqlDbType.Int));
                command.Parameters["@CourseId"].Value = courseId;
                command.Parameters.Add(new SqlParameter("@SemesterId", SqlDbType.NVarChar));
                command.Parameters["@SemesterId"].Value = semesterId;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new CourseSemesteexamModel
                            {

                                examid = (int)reader["ID"],
                                ExamName = reader["EXAM"].ToString(),
                            }));
                        }
                    }
                    return res;
                }
            }
        }

        //Dinidu get 
        /* public async Task<List<CoursesemesterExamsModel>> GetEntityByCoursesemseterexam(int? entityID)
         {
             IQueryable<MHieduCoursesemesterExam> entities = await GetAllEntitiesPvt();

             List<MHieduCoursesemesterExam> classList = entities.Where(a => a.SemesterCourseMappingId == entityID).ToList();

             if (classList.Count != 0)
             {
                 var subjectsLists = classList;
                 List<CoursesemesterExamsModel> acadamics = new List<CoursesemesterExamsModel>();

                 subjectsLists.ForEach(a =>
                 {

                     CoursesemesterExamsModel acadamicy = new CoursesemesterExamsModel();
                     acadamicy.acadamicyeartId = a.Id;
                     acadamicy.acadamicyearName = a.YearName;
                     acadamics.Add(acadamicy);
                 });
                 return acadamics;
             }
             return null;
             #endregion
         }

         public class CoursesemesterExamsModel
         {
             public int acadamicyeartId { get; set; }
             public string acadamicyearName { get; set; }


         }
 */

        private async Task<IQueryable<MHiEduCourseSemesterExam>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
           .Include(x => x.Id)
            .Include(x => x.Exam)
             .Include(x => x.SemesterCourseMappingId);


        }



        Task<int> IMHiEduCoursesemestersExamsService.UpdateEntity(MHiEduCourseSemesterExam entity)
        {
            throw new NotImplementedException();
        }





        #endregion
        /*   private async Task<IQueryable<MHieducoursesModel>> GetCourseListSP(int schoolid)
           {
               var res = this.GetCourseList(schoolid);
               return res.AsQueryable().OrderByDescending(x => x.Course);
           }*/

        /* private List<MHieducoursesModel> GetCourseList(int schoolid)
         {
             try
             {
                 List<MHieducoursesModel> res = new List<MHieducoursesModel>();

                 if (schoolid == null)
                 {
                     return null;
                 }
                var res1 = (from ur in db.MHieducourses
                           where ur.SchoolId == schoolid
                           select new
                    {
                        id = ur.Id,
                               Course = ur.Course,
                               SchoolId = ur.SchoolId,
                               DepartmentId = ur.DepartmentId,
                               Duration = ur.Duration
                           }).ToList();
                 if (res1 != null)
                 {
                     foreach (var item in res1)
                     {
                         MHieducoursesModel gus = new MHieducoursesModel();
                         gus.SchoolId = item.SchoolId;
                         gus.Course = item.Course;
                         gus.SchoolId = item.SchoolId;
                         gus.DepartmentId = item.DepartmentId;
                         gus.Duration = item.Duration;
                         res.Add(gus);
                     }
                     return res;
                 }
                 return null;
             }
             catch (Exception ex)
             {

                 throw ex;
             }
         }*/

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MHiEduCourseSemesterExam> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.Exam.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<object> GetEntityBySchoolID(int entityID) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.SemesterCourseMappingId == entityID).Select(x => Mapper(x));


        /*  public async Task<object> GetCourseListForAPI(int SchoolId)
          {

              try
              {
                  // var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();

                  var objresult = await this.GetCourseListSP(SchoolId);
                  if (objresult != null)
                  {
                      int count = objresult.Count();
                      //int CurrentPage = 10;
                      //int PageSize = 2;
                      //int TotalCount = count;
                      //int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                      var items = objresult.ToList();
                      // var previousPage = CurrentPage > 1 ? "Yes" : "No";
                      // var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                      var obj = new
                      {
                          // TotalPages = TotalPages,
                          items = items
                      };
                      return obj;
                  }
                  else
                  {
                      return (new
                      {
                          Message = "No data found",

                      });
                  }

              }
              catch (Exception ex)
              {
                  return (new
                  {
                      Data = ex.Message,
                  });
              }
          }*/



    }
}
