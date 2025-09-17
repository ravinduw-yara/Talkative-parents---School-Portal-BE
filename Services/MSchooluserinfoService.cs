
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IMSchooluserinfoService : ICommonService
    {
        Task<int> AddEntity(MSchooluserinfo entity);
        Task<MSchooluserinfo> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MSchooluserinfo entity);
        Task<IQueryable<object>> GetEntityByUserName(string EntityName);
        Task<object> PostSchoolUserForAPI(SchoolUserModel model);
        Task<MSchooluserinfo> GetEntityIDForDelete(int entityID);
        Task<object> GetSchoolUserByidForAPI(int id, int schoolid);
        Task<object> GetSchoolUserOldWithSP(Guid token, string searchString, int PageSize, int pageNumber);
        Task<IQueryable<GetSbAccessPermissionForSP>> GetAllUsersBySP(int id);
        Task<List<UserDetailsModel>> GetSchoolUserDetails(int schoolId, int userId);
        Task<bool> DeleteSchoolUserAsync(int userId);

    }
    public class MSchooluserinfoService : IMSchooluserinfoService
    {
        private readonly IRepository<MSchooluserinfo> repository;
        private DbSet<MSchooluserinfo> localDBSet;
        private readonly TpContext db = new TpContext();
        private readonly TpContext db2 = new TpContext();
        private readonly TpContext db3 = new TpContext();
        private readonly IMGroupService mGroupService;
        private readonly IMStandardgroupmappingService mStandardgroupmapping;
        private readonly IMUsermodulemappingService mUsermodulemappingService;
        private readonly IMSchooluserroleService mSchooluserroleService;
        private readonly IMModuleService mModuleService;
        private readonly IMUsermodulemappingService mUsermodulemapping;
        private readonly IConfiguration configuration;
        private readonly string _connectionString;

        public MSchooluserinfoService(
            IRepository<MSchooluserinfo> repository,
            IMGroupService _mGroupService,
            IMStandardgroupmappingService _mStandardgroupmapping,
            IMUsermodulemappingService _mUsermodulemappingService,
            IMSchooluserroleService _mSchooluserroleService,
            IMModuleService _mModuleService,
            IMUsermodulemappingService _mUsermodulemapping,
            IConfiguration configuration
            )
        {
            this.repository = repository;
            mGroupService = _mGroupService;
            mStandardgroupmapping = _mStandardgroupmapping;
            mUsermodulemappingService = _mUsermodulemappingService;
            mSchooluserroleService = _mSchooluserroleService;
            mModuleService = _mModuleService;
            mUsermodulemapping = _mUsermodulemapping;
            _connectionString = configuration.GetConnectionString("TpConnectionString");
            this.configuration = configuration;
        }

        private async Task AllEntityValue() => localDBSet = (DbSet<MSchooluserinfo>)await this.repository.GetAll();

        private static Object Mapper(MSchooluserinfo x) => new
        {
            x.Id,
            x.Salutation,
            x.Firstname,
            x.Middlename,
            x.Lastname,
            x.Code,
            x.Emailid,
            x.Genderid,
            x.Username,
            x.Password,
            x.Branchid,
            x.Phonenumber,
            x.Profilephoto,
            x.Statusid  
        };


        private async Task<IQueryable<MSchooluserinfo>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.TNoticeboardmessageSchoolusers)
            .Include(x => x.TCalendereventdetailCreatedbyNavigations)
            .Include(x => x.MAppuserinfoCreatedbyNavigations)
            .Include(x => x.MCategoryCreatedbyNavigations)
            .Include(x => x.MChildinfoCreatedbyNavigations)
            .Include(x => x.MGroupCreatedbyNavigations)
            .Include(x => x.MModuleCreatedbyNavigations)
            .Include(x => x.MParentchildmappingCreatedbyNavigations)
            .Include(x => x.MRoleCreatedbyNavigations)
            .Include(x => x.MSchoolCreatedbyNavigations)
            .Include(x => x.MUsermodulemappingCreatedbyNavigations)

            .Include(x => x.Branch)
            .Include(x => x.Gender)
            .Include(x => x.Status);
        }



        public async Task<int> AddEntity(MSchooluserinfo entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MSchooluserinfo> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        public async Task<MSchooluserinfo> GetEntityIDForDelete(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID) && x.Statusid == 1));

        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        public async Task<IQueryable<object>> GetEntityByName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.Firstname.Equals(EntityName.Trim())).Select(x => Mapper(x));
        public async Task<IQueryable<object>> GetEntityByUserName(string EntityName) =>
            (await this.GetAllEntitiesPvt()).Where(x => x.Username.Equals(EntityName.Trim())).Select(x => Mapper(x));


        public async Task<int> UpdateEntity(MSchooluserinfo entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        private string Sha256Encryption(string rawpassword)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {

                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawpassword));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private async Task<IQueryable<MSchooluserinfo>> GetAllSchoolUsers(Guid token, string searchString)
        {
            var userid = await this.db.TTokens.Where(x => x.Id.Equals(token) && x.Statusid == 1).Select(x => x.Referenceid).FirstOrDefaultAsync();
            if (userid != null)
            {
                    var schoolid = await this.db.MSchooluserinfos.Where(x => x.Id.Equals(userid)).Select(x => x.Branch.Schoolid).FirstOrDefaultAsync();
                    var res = db.MSchooluserinfos.Where(x => (x.Branch.Schoolid.Equals(schoolid)) && x.Statusid == 1 && ((x.Firstname.Contains(searchString)) || (x.Lastname.Contains(searchString)) || (x.Middlename.Contains(searchString)) || (x.Emailid.Contains(searchString)) || (x.Username.Contains(searchString)) || (x.Phonenumber.Contains(searchString)))).Include(x => x.Branch);
                    return res;
            }
            return null;
        }


        public async Task<object> GetSchoolUserOldWithSP(Guid token, string searchString, int PageSize, int pageNumber)
        {
            try
            {
                var objresult = await GetAllSchoolUsers(token, searchString);
                if (objresult != null)
                {
                    List<GetSchoolUser> user = new List<GetSchoolUser>();
                    foreach (var item in objresult)
                    {
                        GetSchoolUser unitSchooluser = new GetSchoolUser();

                        //admin check
                        var std_check = await this.db3.MSchooluserroles.Where(x => x.Schooluserid == item.Id).Include(x=>x.Schooluser.Branch).Include(w=>w.Category).FirstOrDefaultAsync();

                        if(std_check != null && std_check.Standardsectionmappingid == null)
                        {

                            GetSbAccessPermission getAccessPermission = new GetSbAccessPermission();
                            getAccessPermission.Id = std_check.Id;
                            getAccessPermission.schoolid = std_check.Schooluser.Branch.Schoolid;
                            getAccessPermission.sectionalGradeId = std_check.Groupid;
                            getAccessPermission.schooluserid = std_check.Schooluserid;
                            getAccessPermission.sectionid = null;
                            getAccessPermission.standardid = null;
                            getAccessPermission.UserSpecializationCategoryId = std_check.Categoryid;
                            getAccessPermission.Rank = std_check.Category.Roleid;

                            unitSchooluser.selectedSbAccessPermission.Add(getAccessPermission);
                        }
                        else 
                        { 
                            var res = await this.GetAllUsersBySP(item.Id);
                            foreach (var item2 in res)
                            {
                                    GetSbAccessPermission getAccessPermission = new GetSbAccessPermission();
                                    getAccessPermission.Id = item2.Id;
                                    getAccessPermission.schoolid = item2.schoolid;
                                    getAccessPermission.sectionalGradeId = item2.sectionalGradeId;
                                    getAccessPermission.schooluserid = item2.schooluserid;

                                    if (item2.parentid == null)
                                    {
                                        getAccessPermission.sectionid = null;
                                        getAccessPermission.standardid = item2.standandardsectionmappingid;
                                    }
                                    else
                                    {
                                        getAccessPermission.sectionid = item2.standandardsectionmappingid;
                                        getAccessPermission.standardid = item2.parentid;
                                    }
                                    getAccessPermission.UserSpecializationCategoryId = item2.UserSpecializationCategoryId;
                                    getAccessPermission.Rank = item2.Rank;

                                    unitSchooluser.selectedSbAccessPermission.Add(getAccessPermission);
                            }
                        }

                        var res2 = db2.MUsermodulemappings.Where(x => x.Schooluserid.Equals(item.Id)).Include(a => a.Module);
                        foreach (var item3 in res2)
                        {
                            GetModulePermissions getmod = new GetModulePermissions();
                            getmod.id = (int)item3.Moduleid;
                            getmod.name = item3.Module.Name;
                            getmod.type = item3.Module.Type;
                            getmod.icon = item3.Module.Icon;
                            getmod.state = item3.Module.State;
                            getmod.selected = (bool)item3.Module.Selected;

                            var res3 = db3.MModules.Where(x => x.Parentid.Equals(item3.Moduleid));
                            foreach(var item4 in res3)
                            {
                                GetModuleChildren getch = new GetModuleChildren();
                                getch.name = item4.Name;
                                getch.state = item4.State;
                                getmod.children.Add(getch);
                            }

                            unitSchooluser.selectedClass.Add(getmod);
                        }

                        unitSchooluser.Id = item.Id;
                        unitSchooluser.Username = item.Username;
                        unitSchooluser.Password = item.Password;
                        unitSchooluser.Code = item.Code;
                        unitSchooluser.BranchId = item.Branchid;
                        unitSchooluser.Salutation = item.Salutation;
                        unitSchooluser.FirstName = item.Firstname;
                        unitSchooluser.MiddleName = item.Middlename;
                        unitSchooluser.LastName = item.Lastname;
                        unitSchooluser.PhoneNumber = item.Phonenumber;
                        unitSchooluser.EmailAddress = item.Emailid;
                        unitSchooluser.Enddate = item.Enddate;

                        user.Add(unitSchooluser);
                    }
                    int count = user.Count();
                    int CurrentPage = pageNumber;
                    int TotalCount = count;
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                    var items = user.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                    var previousPage = CurrentPage > 1 ? "Yes" : "No";
                    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                    var obj = new
                    {
                        TotalPages = TotalPages,
                        items = items
                    };
                    return (obj);
                }
                return (new { 
                   Msg = "No  data found",
                   StatusCode = HttpStatusCode.NotFound
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


        public async Task<object> GetSchoolUserByidForAPI(int id, int schoolid)
        {
            try
            {
                var objresult = await this.db.MSchooluserinfos.Where(x => x.Id.Equals(id) && x.Branch.Schoolid.Equals(schoolid)).FirstOrDefaultAsync();
                if (objresult != null)
                {
                    GetSchoolUser getSchooluser = null;
                    getSchooluser = new GetSchoolUser
                    {
                        Id = objresult.Id,
                        Username = objresult.Username,
                        FirstName = objresult.Firstname,
                        LastName = objresult.Lastname,
                        MiddleName = objresult.Middlename,
                        Salutation = objresult.Salutation,
                        PhoneNumber = objresult.Phonenumber,
                        EmailAddress = objresult.Emailid,
                        Password = objresult.Password,
                        Code = objresult.Code,
                        BranchId = objresult.Branchid
                    };
                    var res = db.MSchooluserroles.Where(x => x.Schooluserid.Equals(id)).Include(x => x.Category).Include(x => x.Group).Include(x => x.Category.Role).Include(x => x.Schooluser).Include(x => x.Schooluser.Branch).Include(x => x.Standardsectionmapping);
                    foreach (var item in res)
                    {
                        GetSbAccessPermission getAccessPermission = new GetSbAccessPermission();
                        getAccessPermission.Id = item.Id;
                        getAccessPermission.schoolid = item.Schooluser.Branch.Schoolid;
                        getAccessPermission.sectionalGradeId = item.Groupid;
                        getAccessPermission.schooluserid = item.Schooluserid;

                        if (item.Standardsectionmapping == null)
                        {
                            getAccessPermission.sectionid = null;
                            getAccessPermission.standardid = null;
                        }
                        else if (item.Standardsectionmapping.Parentid == null)
                        {
                            getAccessPermission.sectionid = null;
                            getAccessPermission.standardid = item.Standardsectionmappingid;
                        }
                        else
                        {
                            getAccessPermission.sectionid = item.Standardsectionmappingid;
                            getAccessPermission.standardid = item.Standardsectionmapping.Parentid;
                        }

                        getAccessPermission.UserSpecializationCategoryId = item.Categoryid;
                        getAccessPermission.Rank = item.Category.Role.Rank;
                        getSchooluser.selectedSbAccessPermission.Add(getAccessPermission);
                    }
                    var res2 = db2.MUsermodulemappings.Where(x => x.Schooluserid.Equals(id)).Include(a => a.Module);
                    foreach (var item3 in res2)
                    {
                        GetModulePermissions getmod = new GetModulePermissions();
                        getmod.id = (int)item3.Moduleid;
                        getmod.name = item3.Module.Name;
                        getmod.type = item3.Module.Type;
                        getmod.icon = item3.Module.Icon;
                        getmod.state = item3.Module.State;
                        getmod.selected = (bool)item3.Module.Selected;

                        var res3 = db3.MModules.Where(x => x.Parentid.Equals(item3.Moduleid));
                        foreach (var item4 in res3)
                        {
                            GetModuleChildren getch = new GetModuleChildren();
                            getch.name = item4.Name;
                            getch.state = item4.State;
                            getmod.children.Add(getch);
                        }

                        //firebase
                        getSchooluser.selectedClass.Add(getmod);
                    }
                    if (getSchooluser != null)
                    {
                        return (getSchooluser);
                    }
                    else
                    {
                        return (new
                        {
                            Msg = "User either deactviated or not found",
                            StatusCode = HttpStatusCode.NotFound
                        });
                    }
                }
                return (new
                {
                    Msg = "No  data found",
                    StatusCode = HttpStatusCode.NotFound
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

        public async Task<object> PostSchoolUserForAPI(SchoolUserModel model)
        {
            try
            {
                    var temp = await this.GetEntityIDForUpdate(model.Id);
                    if (temp != null)
                    {
                        if (!string.IsNullOrEmpty(model.Salutation))
                            temp.Salutation = model.Salutation;
                        if (!string.IsNullOrEmpty(model.FirstName))
                            temp.Firstname = model.FirstName;
                        if (!string.IsNullOrEmpty(model.MiddleName))
                            temp.Middlename = model.MiddleName;
                        if (!string.IsNullOrEmpty(model.LastName))
                            temp.Lastname = model.LastName;
                        if (!string.IsNullOrEmpty(model.Code))
                            temp.Code = model.Code;
                        if (!string.IsNullOrEmpty(model.EmailAddress))
                            temp.Emailid = model.EmailAddress;
                        if (model.Gender.HasValue)
                            temp.Genderid = model.Gender;
                        if (model.Gender.HasValue)
                            temp.Genderid = model.Gender;
                        if (!string.IsNullOrEmpty(model.Username))
                            temp.Username = model.Username;

                        var pwd = await this.db3.MSchooluserinfos.Where(x => x.Id.Equals(model.Id)).Select(y => y.Password).FirstOrDefaultAsync();
                        if (model.Password != pwd)
                            temp.Password = Sha256Encryption(model.Password);

                        //if (!string.IsNullOrEmpty(model.Password))
                        //    temp.Password = Sha256Encryption(model.Password);

                        if (!string.IsNullOrEmpty(model.PhoneNumber))
                            temp.Phonenumber = model.PhoneNumber;
                        if (!string.IsNullOrEmpty(model.Profilephoto))
                            temp.Profilephoto = model.Profilephoto;
                        if (model.Enddate.HasValue)
                            temp.Enddate = model.Enddate;
                        #region can be used later
                        if (model.Statusid.HasValue)
                            temp.Statusid = model.Statusid;
                        if (model.BranchId.HasValue)
                            temp.Branchid = model.BranchId;
                        //if (model.Modifiedby.HasValue)
                        //    temp.Modifiedby = model.Modifiedby;
                        //if (model.Createdby.HasValue)
                        //    temp.Createdby = model.Createdby;
                        #endregion
                        temp.Modifieddate = DateTime.Now;
                        await this.UpdateEntity(temp);

                        //Updating MUsermodulemapping
                        List<MUsermodulemapping> temp3 = db.MUsermodulemappings.AsNoTracking().Where(x => x.Schooluserid.Equals(model.Id)).ToList();
                        db.MUsermodulemappings.RemoveRange(temp3);
                        db.SaveChanges();

                        foreach (var item in model.selectedPermission)
                        {
                            await this.mUsermodulemapping.AddEntity(new MUsermodulemapping
                            {
                                Schooluserid = temp.Id,
                                Moduleid = item.id,
                                #region can be used later
                                //Createdby = model.Createdby,
                                Modifiedby = temp.Id,
                                #endregion
                                Statusid = model.Statusid,
                                Createddate = DateTime.Now,
                                Modifieddate = DateTime.Now,
                            });
                        }

                        //Updating MSchooluserrole
                        List<MSchooluserrole> temp4 = db.MSchooluserroles.AsNoTracking().Where(x => x.Schooluserid.Equals(model.Id)).ToList();
                        db.MSchooluserroles.RemoveRange(temp4);
                        db.SaveChanges();

                        foreach (var item in model.selectedClass)
                        {
                            MSchooluserrole sur = new MSchooluserrole();
                            sur.Schooluserid = temp.Id;

                            var std_sec = await this.db.MStandardsectionmappings.Where(x => x.Id.Equals(item.SectionId)).FirstOrDefaultAsync();
                            if (std_sec == null)
                            {
                                sur.Standardsectionmappingid = item.StandardId;
                            }
                            else
                            {
                                sur.Standardsectionmappingid = item.SectionId;
                            }

                            sur.Enddate = model.Enddate;
                            sur.Categoryid = item.UserSpecializationCategoryId;
                            sur.Groupid = item.SectionalGradeId;
                            sur.Modifiedby = temp.Id;
                            sur.Statusid = 1;
                            sur.Createddate = DateTime.Now;
                            sur.Modifieddate = DateTime.Now;
                            await this.mSchooluserroleService.AddEntity(sur);
                        }

                    return temp.Id;
                    }
                var temp2 = await this.GetEntityByUserName(model.Username);
                if (temp2.Count() < 1)
                {
                    var res = await this.AddEntity(new MSchooluserinfo
                    {
                        Salutation = model.Salutation,
                        Firstname = model.FirstName,
                        Lastname = model.LastName,
                        Middlename = model.MiddleName,
                        Code = model.Code,
                        Branchid = model.BranchId,
                        Emailid = model.EmailAddress,
                        Genderid = model.Gender,
                        Username = model.Username,
                        Password = Sha256Encryption(model.Password),
                        Phonenumber = model.PhoneNumber,
                        Profilephoto = model.Profilephoto,
                        Enddate = model.Enddate,
                        #region can be used later
                        //Createdby = model.Createdby,
                        //Modifiedby = model.Createdby,
                        #endregion
                        Statusid = 1,
                        Createddate = DateTime.Now,
                        Modifieddate = DateTime.Now,
                    });

                    foreach (var item in model.selectedPermission)
                    {
                        await this.mUsermodulemapping.AddEntity(new MUsermodulemapping
                        {
                            Schooluserid = res,
                            Moduleid = item.id,
                            #region can be used later
                            Createdby = res,
                            Modifiedby = res,
                            #endregion
                            Statusid = 1,
                            Createddate = DateTime.Now,
                            Modifieddate = DateTime.Now,
                        });
                    }
                    foreach (var item in model.selectedClass)
                    {
                        MSchooluserrole sur = new MSchooluserrole();
                        sur.Schooluserid = res;

                        var std_sec = await this.db.MStandardsectionmappings.Where(x => x.Id.Equals(item.SectionId)).FirstOrDefaultAsync();
                        if(std_sec == null)
                        {
                            sur.Standardsectionmappingid = item.StandardId;
                        }
                        else
                        {
                            sur.Standardsectionmappingid = item.SectionId;
                        }

                        sur.Enddate = model.Enddate;
                        sur.Categoryid = item.UserSpecializationCategoryId;
                        sur.Groupid = item.SectionalGradeId;
                        sur.Createdby = res;
                        sur.Modifiedby = res;
                        sur.Statusid = 1;
                        sur.Createddate = DateTime.Now;
                        sur.Modifieddate = DateTime.Now;
                        await this.mSchooluserroleService.AddEntity(sur);
                    }
                    return res;
                }
                return (new
                {
                    Msg = "Username already exists",
                    StatusCode = HttpStatusCode.NotFound
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



        public async Task<IQueryable<GetSbAccessPermissionForSP>> GetAllUsersBySP(int id)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<GetSbAccessPermissionForSP>();
                connection.Open();
                SqlCommand command = new SqlCommand(ApplicationConstants.GetAccessPermissionBySchoolUserId, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                command.Parameters["@id"].Value = id;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            await Task.Run(() => res.Add(new GetSbAccessPermissionForSP()
                            {
                                Id = (int)reader["id"],
                                schoolid = reader["schoolid"] != DBNull.Value ? (int?)reader["schoolid"] : null,
                                sectionalGradeId = reader["sectionalGradeId"] != DBNull.Value ? (int?)reader["sectionalGradeId"] : null,
                                schooluserid = reader["schooluserid"] != DBNull.Value ? (int?)reader["schooluserid"] : null,
                                UserSpecializationCategoryId = reader["UserSpecializationCategoryId"] != DBNull.Value ? (int?)reader["UserSpecializationCategoryId"] : null,
                                Rank = reader["rank"] != DBNull.Value ? (int?)reader["rank"] : null,
                                parentid = reader["parentid"] != DBNull.Value ? (int?)reader["parentid"] : null,
                                standandardsectionmappingid = reader["standandardsectionmappingid"] != DBNull.Value ? (int?)reader["standandardsectionmappingid"] : null,
                            }));

                        }
                    }
                }
                return res.AsQueryable();
            }
        }
        //nwq 21 delete user
        public async Task<bool> DeleteSchoolUserAsync(int userId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(ApplicationConstants.DeleteSchoolUser, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters with type safety
                    command.Parameters.Add(new SqlParameter("@schooluserid", SqlDbType.Int) { Value = userId });

                    // Execute the command and check rows affected
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    // Return true if the operation affected at least one row
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<List<UserDetailsModel>> GetSchoolUserDetails(int schoolId, int userId)
        {
            var connectionString = configuration.GetConnectionString(ApplicationConstants.TPConnectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var res = new List<UserDetailsModel>();
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand(ApplicationConstants.GetSchoolUserDetails, connection);
                command.CommandType = CommandType.StoredProcedure;
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@schoolid", schoolId);
                    command.Parameters.AddWithValue("@id", userId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                res.Add(new UserDetailsModel
                                {
                                    UserId = (int)reader["UserID"],
                                    UserName = reader["UserName"].ToString(),
                                    UserRole = reader["UserRole"].ToString(),
                                    RoleID = (int)reader["RoleID"],
                                    UserSpecializationCategoryId = (int)reader["UserSpecializationCategoryId"],
                                    Rank = (int)reader["Rank"],
                                    RankName = reader["RankName"].ToString()
                                });
                            }
                        }
                    }
                }

                return res;
            }
        }

    }
}
