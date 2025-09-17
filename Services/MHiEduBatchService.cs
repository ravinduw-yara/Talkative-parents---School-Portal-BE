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
using static Services.MHiEduBatchService;


namespace Services
{
    public interface IMHiEduBatchService : ICommonService
    {
        Task<int> AddEntity(MHiEduBatch entity);
        Task<object> GetEntityByBatchID6(int EntityID);
        Task<int> DeleteEntity(MHiEduBatch entity);
        Task<MHiEduBatch> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MHiEduBatch entity);
        Task<List<HiEduBatchModel>> GetEntityByBatchID5(int? entityID);
        Task<List<BatchDetailsModel>> GetHiEduBatchs(int courseId, int? createdYear, int? createdMonth = null, int? createdDate = null, int? StudentCount = null, string Status = null);
    }
    public class MHiEduBatchService : IMHiEduBatchService
    {
        private readonly IRepository<MHiEduBatch> repository;
        private DbSet<MHiEduBatch> localDBSet;
        private readonly IConfiguration configuration;
        readonly TpContext db = new TpContext();

        public MHiEduBatchService(
            IRepository<MHiEduBatch> repository,
            IConfiguration configuration
            )
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        #region base HiEduBatchs
        private async Task AllEntityValue() => localDBSet = (DbSet<MHiEduBatch>)await this.repository.GetAll();

        private static Object Mapper(MHiEduBatch x) => new
        {
            x.Id,
            x.CourseId,
            x.Batch,
            x.Created_Year,
            x.Created_Month,
            x.Created_Date,
            x.Batch_Created_Date,
            x.StudentCount,
            x.Status,



        };

        private async Task<IQueryable<MHiEduBatch>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.Course);

        }



        public async Task<int> AddEntity(MHiEduBatch entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MHiEduBatch> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.Batch.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<object> GetEntityByBatchID6(int entityID) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.Course.Id == entityID).Select(x => Mapper(x));



        public async Task<int> UpdateEntity(MHiEduBatch entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MHiEduBatch entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<List<HiEduBatchModel>> GetEntityByBatchID5(int? entityID)
        {
            IQueryable<MHiEduBatch> entities = await GetAllEntitiesPvt();

            List<MHiEduBatch> classList = entities.Where(a => a.Course.Id == entityID).ToList();

            if (classList.Count != 0)
            {
                var HiedubatchLists = classList;
                List<HiEduBatchModel> Hiedubatchs = new List<HiEduBatchModel>();

                HiedubatchLists.ForEach(a =>
                {

                    HiEduBatchModel Hiedubatch = new HiEduBatchModel();
                    Hiedubatch.Batch = a.Batch;
                    Hiedubatch.Batch_Created_Date = a.Batch_Created_Date;

                    Hiedubatchs.Add(Hiedubatch);
                });
                return Hiedubatchs;
            }
            return null;
            #endregion
        }

        public class HiEduBatchModel
        {

            public string Batch { get; set; }
            public int CourseId { get; set; }
            public int Created_Year { get; set; }
            public int Created_Month { get; set; }
            public int Created_Date { get; set; }
            public DateTime Batch_Created_Date { get; set; }
            public int StudentCount { get; set; }
            public string Status { get; set; }
        }

        public async Task<List<BatchDetailsModel>> GetHiEduBatchs(int courseId, int? createdYear, int? createdMonth = null, int? createdDate = null, int? StudentCount = null, string Status = null)
        {


            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<BatchDetailsModel>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetHiEduBatch, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@CourseId", SqlDbType.Int));
                command.Parameters["@CourseId"].Value = courseId;

                command.Parameters.Add(new SqlParameter("@Created_Year", SqlDbType.Int));
                command.Parameters["@Created_Year"].Value = createdYear;
                command.Parameters.Add(new SqlParameter("@Created_Month", SqlDbType.Int));
                command.Parameters["@Created_Month"].Value = createdMonth;
                command.Parameters.Add(new SqlParameter("@Created_Date", SqlDbType.Int));
                command.Parameters["@Created_Date"].Value = createdDate;

                //if (createdYear.HasValue)
                //    command.Parameters.AddWithValue("@Created_Year", createdYear.Value);
                //else
                //    command.Parameters.AddWithValue("@Created_Year", DBNull.Value);

                //if (createdMonth.HasValue)
                //    command.Parameters.AddWithValue("@Created_Month", createdMonth.Value);
                //else
                //    command.Parameters.AddWithValue("@Created_Month", DBNull.Value);

                //if (createdDate.HasValue)
                //    command.Parameters.AddWithValue("@Created_Date", createdDate.Value);
                //else
                //    command.Parameters.AddWithValue("@Created_Date", DBNull.Value);

                //if (StudentCount.HasValue)
                //    command.Parameters.AddWithValue("@StudentCount", StudentCount.Value);
                //else
                //    command.Parameters.AddWithValue("@StudentCount", DBNull.Value);

                //if (Status != "")
                //    command.Parameters.AddWithValue("@Status", Status);
                //else
                //    command.Parameters.AddWithValue("@Status", DBNull.Value);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new BatchDetailsModel
                            {
                                Id = (int)reader["Id"],
                                Batch = reader["Batch"].ToString(),
                                CourseId = (int)reader["CourseId"],
                                Created_Year = (int)reader["Created_Year"],
                                Created_Month = (int)reader["Created_Month"],
                                Created_Date = (int)reader["Created_Date"],
                                Batch_Created_Date = (DateTime)reader["Batch_Created_Date"],
                                StudentCount = (int)reader["StudentCount"],
                                Status = (string)reader["Status"]
                            }));
                        }
                    }
                }
                return res;
            }
        }


    }
}

