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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Services.MAcadamicyearService;


namespace Services
{
    public interface IMAcadamicyearService : ICommonService
    {
        Task<int> AddEntity(MAcademicyeardetail entity);
        Task<object> GetEntityBySchoolID(int EntityID);
        Task<int> DeleteEntity(MAcademicyeardetail entity);
        Task<MAcademicyeardetail> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MAcademicyeardetail entity);
        Task<List<AcadamicModel>> GetEntityBySchoolID2(int? entityID);
        Task<object> UpdateCurrentYear(UpdateCurrentYearModel model);
    }
    public class MAcadamicyearService : IMAcadamicyearService
    {
        private readonly IRepository<MAcademicyeardetail> repository;
        private DbSet<MAcademicyeardetail> localDBSet;
        private readonly IConfiguration configuration;

        public MAcadamicyearService(IRepository<MAcademicyeardetail> repository, IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        #region base services
        private async Task AllEntityValue() => localDBSet = (DbSet<MAcademicyeardetail>)await this.repository.GetAll();

        private static Object Mapper(MAcademicyeardetail x) => new
        {
            x.Id,
            x.YearName,
            x.SchoolId,
            x.Statusid

        };

        private async Task<IQueryable<MAcademicyeardetail>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.School)
            .Include(x => x.Status);
        }



        public async Task<int> AddEntity(MAcademicyeardetail entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MAcademicyeardetail> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.YearName.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<object> GetEntityBySchoolID(int entityID) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.School.Id == entityID).Select(x => Mapper(x));


        public async Task<int> UpdateEntity(MAcademicyeardetail entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MAcademicyeardetail entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }


        public async Task<object> UpdateCurrentYear(UpdateCurrentYearModel model)
        {
            try
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    SqlCommand command = new SqlCommand(ApplicationConstants.UpdateCurrentYear, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    command.Parameters["@Id"].Value = model.Id;

                    command.Parameters.Add(new SqlParameter("@CurrentYear", SqlDbType.Int));
                    command.Parameters["@CurrentYear"].Value = model.CurrentYear;


                    await command.ExecuteNonQueryAsync();

                }

                return "CurrentYear Updated Successfully";
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        public class UpdateCurrentYearModel
        {
            public int Id { get; set; }
            public int CurrentYear { get; set; }


        }




        public async Task<List<AcadamicModel>> GetEntityBySchoolID2(int? entityID)
        {
            IQueryable<MAcademicyeardetail> entities = await GetAllEntitiesPvt();

            List<MAcademicyeardetail> classList = entities.Where(a => a.School.Id == entityID).ToList();

            if (classList.Count != 0)
            {
                var subjectsLists = classList;
                List<AcadamicModel> acadamics = new List<AcadamicModel>();

                subjectsLists.ForEach(a =>
                {
                    AcadamicModel acadamicy = new AcadamicModel();
                    acadamicy.acadamicyeartId = a.Id;
                    acadamicy.acadamicyearName = a.YearName;
                    acadamics.Add(acadamicy);
                });
                return acadamics;
            }
            return null;
            #endregion
        }

        public class AcadamicModel
        {
            public int acadamicyeartId { get; set; }
            public string acadamicyearName { get; set; }


        }
    }
}