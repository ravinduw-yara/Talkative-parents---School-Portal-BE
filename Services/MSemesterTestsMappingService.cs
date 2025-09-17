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
    public interface IMSemesterTestsMappingService : ICommonService
    {
        Task<int> AddEntity(MSemestertestsmapping entity);
        Task<int> DeleteEntity(MSemestertestsmapping entity);
        Task<IQueryable<object>> GetEntityByBranchID(int entityID);
        Task<IQueryable<object>> GetEntityBySemesterID(int entityID);
        Task<MSemestertestsmapping> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MSemestertestsmapping entity);
        Task<List<SemesterModel>> GetSemesterTestMappingData(int schoolid, int levelid);
    }
    public class MSemesterTestsMappingService : IMSemesterTestsMappingService
    {
        private readonly IRepository<MSemestertestsmapping> repository;
        private DbSet<MSemestertestsmapping> localDBSet;
        private readonly TpContext db = new TpContext();
        private readonly TpContext db1 = new TpContext();
        private readonly IConfiguration configuration;

        public MSemesterTestsMappingService(IRepository<MSemestertestsmapping> repository, IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        #region base services
        private async Task AllEntityValue() => localDBSet = (DbSet<MSemestertestsmapping>)await this.repository.GetAll();

        private static Object Mapper(MSemestertestsmapping x) => new
        {
            x.Id,
            x.Name,
            x.Description,
            x.SemesterId,
            Branch = new
            {
                Branch = x.Branch != null ? x.Branch.Name : string.Empty,
                x.BranchId
            },
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };


        private async Task<IQueryable<MSemestertestsmapping>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Branch)
            .Include(x => x.Status);
        }



        public async Task<int> AddEntity(MSemestertestsmapping entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MSemestertestsmapping> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<IQueryable<object>> GetEntityByBranchID(int entityID)
            => (await this.GetAllEntitiesPvt()).Where(x => x.BranchId.Equals(entityID)).Select(x => Mapper(x));

        public async Task<IQueryable<object>> GetEntityBySemesterID(int entityID)
            => (await this.GetAllEntitiesPvt()).Where(x => x.SemesterId.Equals(entityID)).Select(x => Mapper(x));


        public async Task<int> UpdateEntity(MSemestertestsmapping entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MSemestertestsmapping entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<List<SemesterModel>> GetSemesterTestMappingData(int schoolid,int levelid)
        {
            try
            {
                
                List<SemesterModel> gssi = new List<SemesterModel>();
                var res = await Task.FromResult((from stm in db.MSemestertestsmappings
                                                 join tsm in db.MTestsectionmappings on stm.Id equals tsm.TestId
                                                 join sec in db.MStandardsectionmappings on tsm.SectionId equals sec.Id
                                                 join br in db.MBranches on stm.BranchId equals br.Id
                                                 where br.Schoolid == schoolid
                                                 select new { TestId = tsm.TestId, TestName = stm.Name, SectionId = tsm.SectionId, SectionName = sec.Name, stm.SemesterId, sec.Parentid }).Distinct().ToList());

                if (res.Count() > 0)
                {
                    var semid = res.Select(x => new { x.SemesterId }).Distinct().ToList();
                    foreach (var item in semid)
                    {
                        SemesterModel xRow4 = new SemesterModel();
                        xRow4.SemesterId = (int)item.SemesterId;
                        var semesterName = from stm in db1.MSemestertestsmappings.Where(x => x.Id == item.SemesterId)
                                           select new { stm.Name };
                        foreach (var data in semesterName)
                        {
                            xRow4.SemesterName = data.Name;
                        }

                        var yeardata = (from sym in db.MSemesteryearmappings.Where(x => x.SemesterId == item.SemesterId) select new { sym.AcademicYearId }).ToList();
                        foreach (var year in yeardata)
                        {
                            dataAYear xRow5 = new dataAYear();
                            xRow5.YearId = year.AcademicYearId;
                            xRow5.AcademicYear = db.MAcademicyeardetails.Where(a => a.Id == year.AcademicYearId).Select(b => b.YearName).FirstOrDefault();
                            xRow4.Years.Add(xRow5);
                        }

                        var testdata = res.Where(y => y.SemesterId == item.SemesterId).Select(w => new { w.TestId, w.TestName }).Distinct().ToList().OrderBy(o => o.TestName);
                        foreach (var item1 in testdata)
                        {
                            TestModel xRow3 = new TestModel();
                            xRow3.TestId = item1.TestId;
                            xRow3.TestName = item1.TestName;
                            xRow4.Tests.Add(xRow3);

                            var stddata = res.Where(r => r.SemesterId == item.SemesterId && r.TestId == item1.TestId).Select(w => new { w.Parentid }).Distinct().ToList();
                            foreach (var item2 in stddata)
                            {
                                StdModel xRow2 = new StdModel();
                                xRow2.StandardId = (int)item2.Parentid;
                                foreach (var datas in stddata)
                                {
                                    var standardName = from stm in db1.MStandardsectionmappings.Where(x => x.Id == datas.Parentid)
                                                       select new { stm.Name };
                                    foreach (var data7 in standardName)
                                    {
                                        xRow2.StandardName = data7.Name;
                                    }
                                }
                                xRow3.Stds.Add(xRow2);

                                var secdata = res.Where(t => t.SemesterId == item.SemesterId && t.TestId == item1.TestId && t.Parentid == item2.Parentid).Select(w => new { w.SectionId, w.SectionName }).Distinct().ToList().OrderBy(o => o.SectionName);
                                foreach (var item3 in secdata)
                                {
                                    SecModel xRow1 = new SecModel();
                                    xRow1.SectionId = item3.SectionId;
                                    xRow1.SectionName = item3.SectionName;
                                    xRow2.Sections.Add(xRow1);
                                }
                            }
                        }
                        gssi.Add(xRow4);
                    }
                    return (gssi);
                }
                return (gssi);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

    }
}
