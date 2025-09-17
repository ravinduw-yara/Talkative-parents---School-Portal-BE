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
    public interface IMTeacherSubjectMappingService : ICommonService
    {
        Task<int> AddEntity(MTeachersubjectmapping entity);
        Task<int> DeleteEntity(MTeachersubjectmapping entity);
        Task<MTeachersubjectmapping> GetEntityIDForUpdate(int entityID);
        Task<int> UpdateEntity(MTeachersubjectmapping entity);
    }
    public class MTeacherSubjectMappingService : IMTeacherSubjectMappingService
    {
        private readonly IRepository<MTeachersubjectmapping> repository;
        private DbSet<MTeachersubjectmapping> localDBSet;

        public MTeacherSubjectMappingService(IRepository<MTeachersubjectmapping> repository)
        {
            this.repository = repository;
        }

        #region base services
        private async Task AllEntityValue() => localDBSet = (DbSet<MTeachersubjectmapping>)await this.repository.GetAll();

        private static Object Mapper(MTeachersubjectmapping x) => new
        {
            x.Id,
            x.TeacherId,
            x.SubjectSectionId,
            x.AcademicYearID,
            Status = new
            {
                Status = x.Status != null ? x.Status.Name : string.Empty,
                x.Statusid
            }
        };


        private async Task<IQueryable<MTeachersubjectmapping>> GetAllEntitiesPvt()
        {
            await AllEntityValue();
            return this.localDBSet
            .Include(x => x.CreatedbyNavigation)
            .Include(x => x.ModifiedbyNavigation)
            .Include(x => x.Status);
        }



        public async Task<int> AddEntity(MTeachersubjectmapping entity)
        {
            var temp = await this.repository.Insert(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }
        public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        public async Task<MTeachersubjectmapping> GetEntityIDForUpdate(int entityID)
       => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));


        public async Task<object> GetEntityByID(int entityID)
        => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();


        public async Task<int> UpdateEntity(MTeachersubjectmapping entity)
        {
            var temp = await this.repository.Update(entity);
            if (temp)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteEntity(MTeachersubjectmapping entity)
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
        #endregion

    }
}
