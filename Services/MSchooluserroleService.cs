using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Services.MSchooluserroleService;

namespace Services
{
    public interface IMSchooluserroleService : ICommonService
    {
        Task<int> AddEntity(MSchooluserrole entity);
        Task<MSchooluserrole> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MSchooluserrole entity);
        Task<bool> ValidateTeacher(int schoolid, int teacherid, int sectionid);

    }
    public class MSchooluserroleService : IMSchooluserroleService
    {
        private readonly IRepository<MSchooluserrole> repository;
        private DbSet<MSchooluserrole> localDBSet;
        private TpContext db = new TpContext();

        public MSchooluserroleService(IRepository<MSchooluserrole> repository)
        {
            this.repository = repository;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MSchooluserrole>)await this.repository.GetAll();

        private static Object Mapper(MSchooluserrole x) => new
        {
            x.Id,
            x.Schooluserid,
            x.Standardsectionmappingid,
            x.Categoryid,
            x.Statusid
        };


        private async Task<IQueryable<MSchooluserrole>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Standardsectionmapping)
            .Include(x => x.Schooluser)
            .Include(x => x.Category)
            .Include(x => x.Group)
            .Include(x => x.Status);
        }



        public async Task<int> AddEntity(MSchooluserrole entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MSchooluserrole> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        //public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
        //    (await this.GetAllEntitiesPvt()).Where(x => x.Firstname.Equals(EntityName.Trim())).Select(x => Mapper(x));


        public async Task<int> UpdateEntity(MSchooluserrole entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }


        // For APP
        public async Task<bool> ValidateTeacher(int schoolid, int teacherid, int sectionid)
        {
            try
            {
                //var ssm = await db.MStandardsectionmappings.Where(x => x.Branch.Schoolid == schoolid && ).Select(w => w.Id).FirstOrDefaultAsync();
                var sur = await db.MSchooluserroles.Where(x => x.Standardsectionmappingid == sectionid && x.Schooluserid == teacherid).Select(w => w.Categoryid).SingleOrDefaultAsync();
                if(sur != null)
                {
                    var cat = await db.MCategories.Where(a => a.Id == sur).Select(w => w.Roleid).SingleOrDefaultAsync();
                    if(cat == 4)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }
    }
}
