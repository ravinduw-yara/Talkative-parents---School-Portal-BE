using CommonUtility.RequestModels;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IMGroupService : ICommonService
    {
        Task<int> AddEntity(MGroup entity);
        Task<MGroup> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MGroup entity);
        Task<int> DeleteEntity(MGroup entity);
        Task<object> GetEntityByID(int entityID);

        Task<object> PostSectionalGradeForAPI(MGroupModel model);
        object GetSectionalGradeForAPI(int schoolid, int PageSize = 10, int pageNumber = 1, string searchString = "");

    }
    public class MGroupService : IMGroupService
    {
        private readonly IRepository<MGroup> repository;
        private DbSet<MGroup> localDBSet;
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly IMStandardgroupmappingService mStandardgroupmappingService;

        public MGroupService(IRepository<MGroup> repository, IMStandardgroupmappingService mStandardgroupmappingService)
        {
            this.repository = repository;
            this.mStandardgroupmappingService = mStandardgroupmappingService;
        }

        public async Task<int> AddEntity(MGroup entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MGroup>)await this.repository.GetAll();

        private static Object Mapper(MGroup x) => new
        {
            x.Id,
            x.Name,
            x.Schoolid,
            Status = x.Status != null ? x.Status.Name : string.Empty,
            x.Statusid
        };

        private async Task<IQueryable<MGroup>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.School)
            .Include(x => x.MStandardgroupmappings)
            .Include(x => x.TSoundingboardmessages)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MGroup> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<int> UpdateEntity(MGroup entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MGroup entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<object> PostSectionalGradeForAPI(MGroupModel model) // Post/Update Group, update
        {
            try
            {
                var temp = await this.GetEntityIDForUpdate(model.Id);
                if (temp != null)
                {
                    if (!string.IsNullOrEmpty(model.Name))
                        temp.Name = model.Name;
                    #region can be used later
                    //if (model.Statusid.HasValue)
                    //    temp.Statusid = model.Statusid;
                    //if (model.Modifiedby.HasValue)
                    //    temp.Modifiedby = model.Modifiedby;
                    //if (model.Createdby.HasValue)
                    //    temp.Createdby = model.Createdby;
                    #endregion
                    if (model.SchoolId.HasValue)
                        temp.Schoolid = model.SchoolId;
                    temp.Modifieddate = DateTime.UtcNow;
                    await this.UpdateEntity(temp);

                    //Updating MStandardgroupmapping
                    List<MStandardgroupmapping> temp2 = db.MStandardgroupmappings.AsNoTracking().Where(x => x.Groupid.Equals(model.Id)).ToList();
                    db.MStandardgroupmappings.RemoveRange(temp2);
                    db.SaveChanges();

                    foreach (var item in model.SelectedGrades)
                    {
                        await this.mStandardgroupmappingService.AddEntity(new MStandardgroupmapping
                        {
                            Groupid = model.Id,
                            Standardsectionmappingid = item.StandardId,
                            #region can be used later
                            //Statusid = model.Statusid,
                            //Modifiedby = model.Createdby,
                            #endregion
                            Createddate = DateTime.UtcNow,
                            Modifieddate = DateTime.UtcNow,
                        });
                    }

                    return (temp.Id);
                }
                var res = await this.AddEntity(new MGroup
                {
                    Name = model.Name,
                    Schoolid = model.SchoolId,
                    #region can be used later
                    //Createdby = model.Createdby,
                    //Statusid = model.Statusid,
                    //Modifiedby = model.Createdby,
                    #endregion
                    Createddate = DateTime.UtcNow,
                    Modifieddate = DateTime.UtcNow,
                });

                foreach (var item in model.SelectedGrades)
                {
                    await this.mStandardgroupmappingService.AddEntity(new MStandardgroupmapping
                    {
                        Groupid = res,
                        Standardsectionmappingid = item.StandardId,
                        #region can be used later
                        //Createdby = model.Createdby,
                        //Statusid = model.Statusid,
                        //Modifiedby = model.Createdby,
                        #endregion
                        Createddate = DateTime.UtcNow,
                        Modifieddate = DateTime.UtcNow,
                    });
                }
                return (res);
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

        public object GetSectionalGradeForAPI(int schoolid, int PageSize = 10, int pageNumber = 1, string searchString = "")
        {
            try
            {
                List<SectionalGradeModelGet> sectionalGradeList = new List<SectionalGradeModelGet>();

                var sectionalGrade_Results = db.MGroups.Where(x => (x.Schoolid.Equals(schoolid)) && (x.Name.Contains(searchString))).OrderByDescending(x => x.Modifieddate);
                foreach (var section in sectionalGrade_Results)
                {
                    SectionalGradeModelGet sectionalGrade = new SectionalGradeModelGet();

                    sectionalGrade.Id = section.Id;
                    sectionalGrade.Name = section.Name;
                    sectionalGrade.SchoolId = section.Schoolid;

                    var mapping = db2.MStandardgroupmappings.Where(x => (x.Groupid.Equals(section.Id)) && (x.Group.Name.Contains(searchString))).Include(x => x.Standardsectionmapping).Include(x => x.Group).OrderBy(x => x.Standardsectionmappingid);
                    foreach (var item in mapping)
                    {
                        SectionalGradeMappingModelGet grademapping = new SectionalGradeMappingModelGet();
                        grademapping.Id = item.Id;
                        grademapping.SectionalGradeId = item.Groupid;
                        grademapping.StandardId = item.Standardsectionmappingid;
                        grademapping.StandardName = item.Standardsectionmapping.Name;
                        sectionalGrade.SelectedGrades.Add(grademapping);
                    }
                    sectionalGradeList.Add(sectionalGrade);
                }

                int count = sectionalGradeList.Count();
                int CurrentPage = pageNumber;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                var items = sectionalGradeList.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

                var obj = new
                {
                    TotalPages = TotalPages,
                    items = items
                };

                return obj;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


    }
}
