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
    public interface IMChildschoolmappingService : ICommonService
    {
        Task<int> AddEntity(MChildschoolmapping entity);
        Task<MChildschoolmapping> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MChildschoolmapping entity);
        Task<object> GetChildrenV2MappingForAPI(int appUserId);
    }

    public class MChildschoolmappingService : IMChildschoolmappingService
    {
        private readonly IRepository<MChildschoolmapping> repository;
        private DbSet<MChildschoolmapping> localDBSet;
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext db3 = new TpContext();

        public MChildschoolmappingService(IRepository<MChildschoolmapping> _repository)
        {
            this.repository = _repository;
        }
        private async Task AllEntityValue() => localDBSet = (DbSet<MChildschoolmapping>)await this.repository.GetAll();

        private static Object Mapper(MChildschoolmapping x) => new
        {
            x.Id,          
            x.Childid,
            x.Standardsectionmappingid,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            },
        };
        private async Task<IQueryable<MChildschoolmapping>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet.Include(x => x.Status)
           .Include(x => x.CreatedbyNavigation)
           .Include(x => x.Status)
           .Include(x => x.Standardsectionmapping)
           .Include(x => x.Child)
           .Include(x => x.ModifiedbyNavigation);
        }
        public async Task<int> AddEntity(MChildschoolmapping entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<IQueryable<MChildschoolmapping>> GetAllMChildschoolmappings()
         => await this.repository.GetAll();


        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        //public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt())
        //.Where(x => (x.Status != null && x.Status.Referencesid != (int)ApplicationConstants.ActionStatus.Delete))
        //.Select(x => Mapper(x));

        public async Task<MChildschoolmapping> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<object> GetEntityByID(int entityID)
         => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<int> UpdateEntity(MChildschoolmapping entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public Task<IQueryable<object>> GetEntityByName(string EntityName)
        {
            throw new NotImplementedException();
        }


        //APP Services
        public async Task<object> GetChildrenV2MappingForAPI(int appUserId)
        {
            List<ChildModel> childrenList = new List<ChildModel>();
            try
            {
                //var appUserId = 1; ////for testing without token
                var appUser = await db.MAppuserinfos.Where(w => w.Id == appUserId).FirstOrDefaultAsync();
                if (appUser == null)
                {
                    return ("Unauthorized");
                }
                var children = db.MParentchildmappings.Where(c => c.Appuserid == appUser.Id).ToList();

                foreach (var eachChild1 in children)
                {
                   //Get Academicyearid
                    var Standardsectionmappingid = db.MChildschoolmappings.Where(c => c.Childid == eachChild1.Childid).Select(t => t.Standardsectionmappingid).FirstOrDefault();
                    var Branchid = db.MStandardsectionmappings.Where(c => c.Id == Standardsectionmappingid).Select(t => t.Branchid).FirstOrDefault();
                    var Schoolid = db.MBranches.Where(c => c.Id == Branchid).Select(t => t.Schoolid).FirstOrDefault();
                    var academicyearid = db.MAcademicyeardetails.Where(c => c.SchoolId == Schoolid && c.Currentyear == 1).Select(t => t.Id).FirstOrDefault();

                    var eachChild = await db.MChildinfos.Where(c => c.Id == eachChild1.Childid).FirstOrDefaultAsync();
                    var cChildSchool = await db.MChildschoolmappings.Where(c => c.Childid == eachChild.Id && c.AcademicYearId == academicyearid && c.Statusid == 1).Include(x => x.Standardsectionmapping.Branch).FirstOrDefaultAsync();

                    if (cChildSchool != null)
                    {
                        var SbCount = db.TSoundingboardmessages.Where(w => w.Childinfoid == eachChild.Id && w.Appuserinfoid == appUser.Id && w.Didread == false && w.Isparentreplied == false).Count();

                        ChildModel childModel = new ChildModel();
                        childModel.ChildId = eachChild.Id;
                        childModel.DateOfBirth = eachChild.Dob.ToString();
                        childModel.Gender = eachChild.Genderid.HasValue ? eachChild.Genderid.Value : 1;
                        childModel.FirstName = eachChild.Firstname;
                        childModel.LastName = eachChild.Lastname;

                        //childModel.Picture = eachChild.Picture;

                        childModel.RelationId = (int)eachChild1.Relationtypeid;

                        var ssm = await db2.MStandardsectionmappings.Where(x => x.Id == cChildSchool.Standardsectionmappingid).Include(w => w.Branch.School).FirstOrDefaultAsync();

                        if (ssm.Parentid == null)
                        {
                            childModel.StandardId = ssm.Id;
                            childModel.SectionId = 0;
                            childModel.SectionName = ssm.Name;
                            childModel.StandardName = null;
                        }
                        else
                        {
                            childModel.StandardId = (int)ssm.Parentid;
                            childModel.SectionId = ssm.Id;
                            childModel.SectionName = await db3.MStandardsectionmappings.Where(a => a.Id == ssm.Parentid).Select(b => b.Name).FirstOrDefaultAsync();
                            childModel.StandardName = ssm.Name;
                        }

                        var brch = await db2.MBranches.Where(x => x.Id == cChildSchool.Standardsectionmapping.Branchid).Include(a => a.School).FirstOrDefaultAsync();

                        childModel.SchoolId = (int)ssm.Branch.Schoolid;
                        childModel.SchoolName = brch.School.Name;
                        childModel.SBNotificationCount = SbCount.ToString();
                        childModel.Logo = brch.School.Logo;
                        childModel.Tag = childModel.SchoolName + "-" + childModel.StandardName + " " + childModel.SectionName;

                        childModel.CityId = (int)(ssm.Branch.Locaionid != null ? ssm.Branch.Locaionid : 0);
                        childModel.StateId = (int)(ssm.Branch.Pincode != null ? ssm.Branch.Pincode : 0);
                        //childModel.CountryId = (int)ssm.Branch.;

                        childrenList.Add(childModel);
                    }
                }
                return (childrenList);
            }

            catch (Exception ex)
            {
                return null;
            }

        }


    }
}

