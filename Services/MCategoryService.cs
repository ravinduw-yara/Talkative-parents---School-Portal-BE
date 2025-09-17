using CommonUtility.RequestModels;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IMCategoryService : ICommonService
    {
        Task<int> AddEntity(MCategory entity);
        Task<MCategory> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MCategory entity);
        Task<IQueryable<object>> GetEntityBySchoolID(int EntityID);
        Task<MCategory> GetEntityByNameForUpdate(string EntityName);
        Task<int> DeleteEntity(MCategory entity);
        object GetUserSpecializationCategoryForAPI(int schoolId, bool isList, int PageSize = 10, int pageNumber = 1, string searchString = "");
    }
    public class MCategoryService : IMCategoryService
    {
        private readonly IRepository<MCategory> repository;
        private DbSet<MCategory> localDBSet;
        private readonly TpContext db = new TpContext();

        public MCategoryService(IRepository<MCategory> repository)
        {
            this.repository = repository;
        }

        public async Task<int> AddEntity(MCategory entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MCategory>)await this.repository.GetAll();

        private static Object Mapper(MCategory x) => new
        {
            x.Id,
            x.Name,
            x.Description,
            x.Roleid,
            Status = x.Status != null ? x.Status.Name : string.Empty,
            x.Statusid
        };

        private static Object Mapper2(MCategory x) => new
        {
            id = x.Id,
            roleid = x.Roleid,
            name = x.Role != null ? x.Role.Name : string.Empty,
            remarks = x.Role != null ? x.Role.Description : string.Empty,
            rank = x.Role.Rank,
            selectiontype = 0 // not present
        };

        private async Task<IQueryable<MCategory>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Role)
            .Include(x => x.MSchooluserroles)
            .Include(x => x.TSoundingboardmessages)
            .Include(x => x.Status);
        }

        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MCategory> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Name.Equals(EntityName.Trim())).Select(x => Mapper(x));

        public async Task<MCategory> GetEntityByNameForUpdate(string EntityName) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Name.Equals(EntityName)));

        public async Task<IQueryable<object>> GetEntityBySchoolID(int EntityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Role.Schoolid.Equals(EntityID)).Select(x => Mapper2(x));


        public async Task<int> UpdateEntity(MCategory entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MCategory entity)
        {
            var temp = await this.repository.Delete(entity);
            if (temp > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        #region for GetUserSpecializationCategory
        private IQueryable<MCategory> GetEntityBySchoolIDV2(int Schoolid, string searchString) => (db.MCategories.Where(x => (x.Role.Schoolid == Schoolid) && (x.Name.Contains(searchString) || x.Role.Name.Contains(searchString) || x.Role.Description.Contains(searchString))).Include(x => x.Role));

        public object GetUserSpecializationCategoryForAPI(int schoolId, bool isList, int PageSize = 10, int pageNumber = 1, string searchString = "")
        {
            List<GetCategory> category = new List<GetCategory>();
            try
            {
                var objresult = GetEntityBySchoolIDV2(schoolId, searchString);
                if (objresult != null)
                {
                    foreach (var item in objresult)
                    {
                        category.Add(new GetCategory
                        {
                            Id = item.Id,
                            Name = item.Name,
                            SbAccessRankId = item.Role.Rank,
                            RankName = item.Role != null ? item.Role.Name : string.Empty,
                            Remarks = item.Role != null ? item.Role.Description : string.Empty,
                            SelectionType = /*item.SelectionType,*/ (int)item.Role.Rank,
                        });
                    }

                    int count = category.Count();
                    // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
                    int CurrentPage = pageNumber;
                    // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
                    // int PageSize = nuofRows;
                    // Display TotalCount to Records to User  
                    int TotalCount = count;
                    // Calculating Totalpage by Dividing (No of Records / Pagesize)  
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                    Object items;
                    if (isList)
                    {
                        // Returns List of Customer after applying Paging   
                        items = category.OrderBy(x => x.Name).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                    }
                    else
                    {
                        items = category.OrderBy(x => x.Name);
                    }
                    // if CurrentPage is greater than 1 means it has previousPage  
                    var previousPage = CurrentPage > 1 ? "Yes" : "No";
                    // if TotalPages is greater than CurrentPage means it has nextPage  
                    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                    var obj = new
                    {
                        TotalPages = TotalPages,
                        items = items
                    };
                    if (category.Count > 0)
                    {
                        return obj;
                    }
                    else
                    {
                        return ("Categories are not found ");
                    }
                }
                return ("Ok");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

    }
}
