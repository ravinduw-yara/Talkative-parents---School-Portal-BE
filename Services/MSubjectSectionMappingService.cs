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
    public interface IMSubjectSectionMappingService : ICommonService
    {
        Task<int> AddEntity(MSubjectsectionmapping entity);
        Task<int> DeleteEntity(MSubjectsectionmapping entity);
        Task<MSubjectsectionmapping> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MSubjectsectionmapping entity);
        Task<object> AddSubjectSectionMappingData(MSubjectSectionMappingModel model);
        Task<List<SubSecMapDisplayModel>> GetSubjectSectionMappingData(int schoolid, int levelId, int standardId);
    }
    public class MSubjectSectionMappingService : IMSubjectSectionMappingService
    {
        private readonly IRepository<MSubjectsectionmapping> repository;
        private DbSet<MSubjectsectionmapping> localDBSet;
        private readonly TpContext db = new TpContext();
        private readonly IConfiguration configuration;
        private readonly IMSubjectTestMappingService mSubjectTestMappingService;

        public MSubjectSectionMappingService(IRepository<MSubjectsectionmapping> repository, IConfiguration configuration, IMSubjectTestMappingService _mSubjectTestMappingService)
        {
            this.repository = repository;
            this.configuration = configuration;
            mSubjectTestMappingService = _mSubjectTestMappingService;
        }

        #region base services
        private async Task AllEntityValue() => localDBSet = (DbSet<MSubjectsectionmapping>)await this.repository.GetAll();

        private static Object Mapper(MSubjectsectionmapping x) => new
        {
            x.Id,
            x.SubjectId,
            x.SectionId,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };


        private async Task<IQueryable<MSubjectsectionmapping>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status);
        }



        public async Task<int> AddEntity(MSubjectsectionmapping entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MSubjectsectionmapping> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();


        public async Task<int> UpdateEntity(MSubjectsectionmapping entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MSubjectsectionmapping entity)
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

        public async Task<object> AddSubjectSectionMappingData(MSubjectSectionMappingModel model) // Post/Update 
        {
            try
            {
                foreach (var item in model.Sections)
                {
                    var temp = db.MSubjectsectionmappings.Where(x => x.SectionId == item.SectionId && x.SubjectId == model.SubjectId).FirstOrDefault();
                    if (temp == null)
                    {
                        var subsecids = await this.AddEntity(new MSubjectsectionmapping
                        {
                            SubjectId = model.SubjectId,
                            SectionId = item.SectionId,
                            Createddate = DateTime.UtcNow,
                            Modifieddate = DateTime.UtcNow,
                            Createdby = model.CreatedBy,
                            Modifiedby = model.ModifiedBy,
                            Statusid = model.StatusId
                        });

                        
                        var testids = db.MTestsectionmappings.Where(x => x.SectionId == item.SectionId).Select(a => a.TestId).ToList();
                        if (testids.Count() > 0)
                        {
                            foreach (var test in testids)
                            {
                                var temp2 = db.MSubjecttestmappings.Where(x => x.SubjectSectionMappingId == subsecids && x.TestId == test).FirstOrDefault();
                                if (temp2 == null)
                                {
                                    await this.mSubjectTestMappingService.AddEntity(new MSubjecttestmapping
                                    {
                                        SubjectSectionMappingId = subsecids,
                                        TestId = (int)test,
                                        Createddate = DateTime.UtcNow,
                                        Modifieddate = DateTime.UtcNow,
                                        Createdby = model.CreatedBy,
                                        Modifiedby = model.ModifiedBy,
                                        Statusid = model.StatusId
                                    });
                                }
                            }
                        }
                        return (new
                        {
                            Message = "Subject Sections mapped/updated succesfully"
                        });
                    }
                }
                return (new
                {
                    Message = "Subject Sections mapped already exists"
                });
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

        public async Task<List<SubSecMapDisplayModel>> GetSubjectSectionMappingData(int schoolid, int levelId, int standardId)
        {
            try
            {
                List<SubSecMapDisplayModel> ssmdList = new List<SubSecMapDisplayModel>();
                var res = await Task.FromResult(from sec in db.MStandardsectionmappings
                                                join ssm in db.MSubjectsectionmappings on sec.Id equals ssm.SectionId
                                                join sub in db.MSubjects on ssm.SubjectId equals sub.Id
                                                join br in db.MBranches on sec.Branchid equals br.Id
                                                join lvl in db.MLevels on sec.LevelID equals lvl.Id
                                                where br.Schoolid == schoolid && lvl.Id == levelId && sec.Parentid == standardId
                                                select new
                                                {
                                                    SubjectSectionMappingId = ssm.Id,
                                                    SubjectId = sub.Id,
                                                    SubjectName = sub.Name,
                                                    StandardId = sec.Parentid,
                                                    SectionId = sec.Id,
                                                    SectionName = sec.Name,
                                                });

                var subs = res.Select(x => new { x.SubjectId, x.SubjectName }).Distinct().ToList().OrderBy(w => w.SubjectName);
                var allsubs = db.MSubjects.Select(w => new { SubjectId = w.Id, SubjectName = w.Name }).ToList();
                var emptySubs = allsubs.Except(subs).ToList();

                //foreach (var item4 in subs)
                //{
                //    SubSecMapDisplayModel ssmde = new SubSecMapDisplayModel();
                //    ssmde.SubjectId = item4.SubjectId;
                //    ssmde.SubjectName = item4.SubjectName;
                //    ssmdList.Add(ssmde);
                //}

                foreach (var item in subs.ToList())
                {
                    SubSecMapDisplayModel ssmd = new SubSecMapDisplayModel();
                    ssmd.SubjectId = item.SubjectId;
                    ssmd.SubjectName = item.SubjectName;
                    var res2 = res.Where(w => w.SubjectId == item.SubjectId).Select(w => new { w.StandardId }).Distinct().ToList();
                    foreach (var item2 in res2)
                    {
                        SubStandardDisplayModel ssdm = new SubStandardDisplayModel();
                        ssdm.StandardId = (int)item2.StandardId;
                        var standardName = from stm in db.MStandardsectionmappings.Where(z => z.Id == item2.StandardId)
                                           select new { stm.Name };
                        foreach (var item4 in standardName)
                        {
                            ssdm.StandardName = item4.Name;
                        }

                        var res3 = res.Where(w => w.StandardId == item2.StandardId && w.SubjectId == item.SubjectId).Select(w => new { w.SectionId, w.SectionName }).Distinct().ToList().OrderBy(w => w.SectionName);
                        foreach (var item3 in res3)
                        {
                            SubSectionsModel ssm = new SubSectionsModel();
                            ssm.SectionId = item3.SectionId;
                            ssm.SectionName = item3.SectionName;
                            ssdm.Sections.Add(ssm);
                        }
                        ssmd.Standards.Add(ssdm);
                    }
                    ssmdList.Add(ssmd);
                }
                return (ssmdList);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

    }
}

