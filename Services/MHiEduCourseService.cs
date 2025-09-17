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
using static System.Collections.Specialized.BitVector32;

namespace Services
{

    public interface IMHiEduCourseService : ICommonService
    {
        Task<int> AddEntity(MHiEduCourses entity);
        Task<MHiEduCourses> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MHiEduCourses entity);
        Task<object> GetCourseListForAPI(int SchoolId); 
        Task<object> HiEduCreateCourse(MHiEduCourses course);

    }
    public class MHiEduCourseService : IMHiEduCourseService
    {
        private readonly IRepository<MHiEduCourses> repository;
        private DbSet<MHiEduCourses> localDBSet;
        private readonly IConfiguration configuration;
        readonly TpContext db = new TpContext();

        public MHiEduCourseService(
            IRepository<MHiEduCourses> repository,
            IConfiguration configuration
            )
        {
            this.repository = repository;
            this.configuration = configuration;
        }
        #region base services
        private async Task AllEntityValue() => localDBSet = (DbSet<MHiEduCourses>)await this.repository.GetAll();

        private static Object Mapper(MHiEduCourses x) => new
        {
            x.Id,
            x.Course,
            x.DepartmentId,
            x.Duration
        };


        private async Task<IQueryable<MHiEduCourses>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.Id)
            .Include(x => x.Course)
            .Include(x => x.DepartmentId)
            .Include(x => x.Duration);
        }

        Task<int> IMHiEduCourseService.AddEntity(MHiEduCourses entity)
        {
            throw new NotImplementedException();
        }

        Task<int> IMHiEduCourseService.UpdateEntity(MHiEduCourses entity)
        {
            throw new NotImplementedException();
        }
        public Task<MHiEduCourses> GetEntityIDForUpdate(int entityID)
        {
            throw new NotImplementedException();
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
        #endregion
        private async Task<IQueryable<MHiEduCoursesModel>> GetCourseListSP(int schoolid)
        {
            var res = this.GetCourseList(schoolid);
            return res.AsQueryable().OrderByDescending(x => x.Course);
        }
        private List<MHiEduCoursesModel> GetCourseList(int schoolid)
        {
            try
            {
                List<MHiEduCoursesModel> res = new List<MHiEduCoursesModel>();

                if (schoolid == null)
                {
                    return null;
                }
                var res1 = (from ur in db.MHiEduCourses
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
                        MHiEduCoursesModel gus = new MHiEduCoursesModel();
                        gus.CourseId = item.id;
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
        }
       
         public async Task<object> HiEduCreateCourse(MHiEduCourses course)
        {
            try
            {
                //  var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();
                int userid = 1;
                // if (userid is not null) // checks if records are updated

                if (userid != null)
                {


                    var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(ApplicationConstants.AddHiEduCourse, connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Course", SqlDbType.NVarChar));
                        command.Parameters["@Course"].Value = course.Course;

                        command.Parameters.Add(new SqlParameter("@SchoolId", SqlDbType.Int));
                        command.Parameters["@SchoolId"].Value = course.SchoolId;

                        command.Parameters.Add(new SqlParameter("@DepartmentId", SqlDbType.Int));
                        command.Parameters["@DepartmentId"].Value = course.DepartmentId;

                        command.Parameters.Add(new SqlParameter("@Duration", SqlDbType.NVarChar));
                        command.Parameters["@Duration"].Value = course.Duration;


                        command.ExecuteNonQuery();
                    }
                    db.SaveChanges();

                }
                else
                {

                }
                return ("Course is Created Successfully");
            }

            catch (Exception)
            {
                throw;
            }
        }

     
        //Sanduni HieduCourseList
        public async Task<object> GetCourseListForAPI(int SchoolId)
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
        }


    }
}
