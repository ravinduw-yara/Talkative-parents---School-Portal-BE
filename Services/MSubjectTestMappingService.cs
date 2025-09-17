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
//using static Services.MSubjectService;

namespace Services
{
    public interface IMSubjectTestMappingService : ICommonService
    {
        Task<int> AddEntity(MSubjecttestmapping entity);
        Task<int> DeleteEntity(MSubjecttestmapping entity);
        Task<MSubjecttestmapping> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MSubjecttestmapping entity);
        Task<object> PostTestSections(MSubjectTestMappingModel model);
        Task<object> AddSubjectTestMappingData(SubTestMapModel model);
        Task<List<GetSubjectTestMappingModel>> GetSubjectTestMappingData(int schoolid);
    }
    public class MSubjectTestMappingService : IMSubjectTestMappingService
    {
        private readonly IRepository<MSubjecttestmapping> repository;
        private DbSet<MSubjecttestmapping> localDBSet;
        private readonly TpContext db = new TpContext();
        private readonly IConfiguration configuration;

        public MSubjectTestMappingService(IRepository<MSubjecttestmapping> repository, IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        #region base services
        private async Task AllEntityValue() => localDBSet = (DbSet<MSubjecttestmapping>)await this.repository.GetAll();

        private static Object Mapper(MSubjecttestmapping x) => new
        {
            x.Id,
            x.SubjectSectionMappingId,
            x.TestId,
            x.MaxMarks,
            x.Statusid
        };


        private async Task<IQueryable<MSubjecttestmapping>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet;
            //.Include(x => x.CreatedbyNavigation)
            //.Include(x => x.ModifiedbyNavigation)
            //.Include(x => x.Status);
        }



        public async Task<int> AddEntity(MSubjecttestmapping entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MSubjecttestmapping> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();


        public async Task<int> UpdateEntity(MSubjecttestmapping entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MSubjecttestmapping entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }

        public async Task<object> PostTestSections(MSubjectTestMappingModel model) // Post/Update 
        {
            try
            {
                foreach (var test in model.Tests)
                {
                    foreach (var sec in model.Sections)
                    {
                        var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand(ApplicationConstants.PostTestSectionMapping, connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@TestId", SqlDbType.Int));
                            command.Parameters["@TestId"].Value = test.TestId;
                            command.Parameters.Add(new SqlParameter("@SectionId", SqlDbType.Int));
                            command.Parameters["@SectionId"].Value = sec.SectionId;
                            command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int));
                            command.Parameters["@CreatedBy"].Value = model.Createdby;
                            command.Parameters.Add(new SqlParameter("@ModifiedBy", SqlDbType.Int));
                            command.Parameters["@ModifiedBy"].Value = model.Modifiedby;
                            command.ExecuteNonQuery();
                        }

                        var subsecids = db.MSubjectsectionmappings.Where(x => x.SectionId == sec.SectionId).Select(w => w.Id).ToList();
                        
                        if (subsecids.Count() > 0)
                        {
                            foreach (var subsec in subsecids)
                            {
                                //new august 15 2024
                                var SubjectId =  db.MSubjectsectionmappings.Where(x => x.SectionId == subsec).Select(w => w.SubjectId).FirstOrDefault();
                                var totalMaxMarks = db.MSubSubjects.Where(m => SubjectId == m.SubjectId).Sum(m => m.SubMaxMarks);
                                //new
                                var temp = db.MSubjecttestmappings.Where(x => x.SubjectSectionMappingId == subsec && x.TestId == test.TestId).FirstOrDefault();
                               
                                if (temp == null)
                                {
                                    await this.AddEntity(new MSubjecttestmapping
                                    {
                                        SubjectSectionMappingId = subsec,
                                        TestId = test.TestId,
                                        MaxMarks = totalMaxMarks,
                                        Createddate = DateTime.UtcNow,
                                        Modifieddate = DateTime.UtcNow,
                                        Createdby = model.Createdby,
                                        Modifiedby = model.Modifiedby,
                                        Statusid = model.Statusid
                                    });
                                }
                            }
                        }
                    }
                }
                return ("Section Tests Mapped Successfully");
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }

        public async Task<object> AddSubjectTestMappingData(SubTestMapModel model) // Post/Update 
        {
            try
            {
                foreach (var item in model.Sections.ToList())
                {
                    var subsecids = db.MSubjectsectionmappings.Where(x => x.SectionId == item.SectionId).Select(w => w.Id).ToList();
                    foreach (var subsecid in subsecids)
                    {
                        foreach (var tests in model.Tests.ToList())
                        {
                            var temp = db.MSubjecttestmappings.Where(w => w.SubjectSectionMappingId == subsecid && w.TestId == tests.TestId).FirstOrDefault(); //checks if it's already mapped
                            if (temp == null)
                            {
                                await this.AddEntity(new MSubjecttestmapping
                                {
                                    SubjectSectionMappingId = subsecid,
                                    TestId = (int)tests.TestId,
                                    MaxMarks = tests.MaxMakrs,
                                    Createddate = DateTime.UtcNow,
                                    Modifieddate = DateTime.UtcNow,
                                    Createdby = model.Createdby,
                                    Modifiedby = model.Modifiedby,
                                    Statusid = model.Statusid
                                });
                            }
                        }
                    }
                }
                return ("Subject Tests mapped succesfully");
            }
            catch (Exception ex)
            {
                return (new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }
        }

        public async Task<List<GetSubjectTestMappingModel>> GetSubjectTestMappingData(int schoolid)
        {
            try
            {
                List<GetSubjectTestMappingModel> gssi = new List<GetSubjectTestMappingModel>();
                var res = await Task.FromResult(from sec in db.MStandardsectionmappings
                          join ssm in db.MSubjectsectionmappings on sec.Id equals ssm.SectionId
                          join s in db.MSubjects on ssm.SubjectId equals s.Id
                          join sut in db.MSubjecttestmappings on ssm.Id equals sut.SubjectSectionMappingId
                          join stm in db.MSemestertestsmappings on sut.TestId equals stm.Id
                          join br in db.MBranches on stm.BranchId equals br.Id
                          where br.Schoolid == schoolid
                          select new { standardId = sec.Parentid, sectionId = sec.Id, sectionName = sec.Name, subjectId = s.Id, subjectName = s.Name, sut.TestId, testName = stm.Name, stm.SemesterId });

                if (res.ToList().Count() > 0)
                {
                    var subs = res.Select(x => new { x.subjectId, x.subjectName }).Distinct().ToList().OrderBy(w => w.subjectName);
                    foreach (var data in subs.ToList())
                    {
                        GetSubjectTestMappingModel xRow = new GetSubjectTestMappingModel();
                        xRow.SubjectId = data.subjectId;
                        xRow.SubjectName = data.subjectName;

                        var res2 = res.Where(w => w.subjectId == data.subjectId).Select(w => new { w.standardId }).Distinct().ToList();

                        foreach (var item in res2)
                        {
                            SubStandardModel xRow1 = new SubStandardModel();
                            xRow1.StandardId = (int)item.standardId;
                            var stdName = from stm in db.MStandardsectionmappings.Where(x => x.Id == item.standardId)
                                               select new { stm.Name };
                            foreach (var data5 in stdName)
                            {
                                xRow1.StandardName = data5.Name;
                            }

                            var res3 = res.Where(w => w.standardId == item.standardId && w.subjectId == data.subjectId).Select(w => new { w.sectionId, w.sectionName }).Distinct().ToList().OrderBy(w => w.sectionName);
                            foreach (var item2 in res3)
                            {
                                SubSectionDisplayModel xRow2 = new SubSectionDisplayModel();
                                xRow2.SectionId = item2.sectionId;
                                xRow2.SectionName = item2.sectionName;
                                xRow1.Sections.Add(xRow2);

                                var res4 = res.Where(w => w.sectionId == item2.sectionId && w.subjectId == data.subjectId && w.standardId == item.standardId).Select(w => new { w.SemesterId }).Distinct().ToList();
                                foreach (var item3 in res4)
                                {
                                    SubSemesterModel xRow4 = new SubSemesterModel();
                                    xRow4.SemesterId = (int)item3.SemesterId;
                                    var semesterName = from stm in db.MSemestertestsmappings.Where(x => x.Id == item3.SemesterId)
                                                       select new { stm.Name };
                                    foreach (var data2 in semesterName)
                                    {
                                        xRow4.SemesterName = data2.Name;
                                    }
                                    xRow2.Semesters.Add(xRow4);

                                    var res5 = res.Where(w => w.SemesterId == item3.SemesterId && w.subjectId == data.subjectId && w.sectionId == item2.sectionId && w.standardId == item.standardId).Select(w => new { w.TestId, w.testName }).Distinct().ToList().OrderBy(w => w.testName);

                                    foreach (var item4 in res5)
                                    {
                                        SubTestsModel xRow3 = new SubTestsModel();
                                        xRow3.TestId = item4.TestId;
                                        xRow3.TestName = item4.testName;
                                        xRow4.Tests.Add(xRow3);
                                    }
                                }
                            }
                            xRow.Standards.Add(xRow1);
                        }
                        gssi.Add(xRow);
                    }
                    return (gssi);
                }
                return null; ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
            #endregion

        }
}

