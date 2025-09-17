using Microsoft.EntityFrameworkCore;
using Repository.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public interface IRepository<T> where T:BaseEntity
    {
        Task<IQueryable<T>> GetAll();

        Task<bool> Insert(T entity);
        Task<bool> Update(T entity);
        Task<int> GetCount();
        Task<bool> BulkInsert(List<T> entities);
        Task<bool> BulkUpdate(List<T> entities);
        Task<int> Delete(T entity);
        Task<int> DeleteRange(List<T> entity);
    }
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly TpContext tpContext;

        private readonly DbSet<T> entities;
        public Repository(TpContext _tpContext)
        {
            this.tpContext = _tpContext;
            this.entities = this.tpContext.Set<T>();
        }


        void EntityNullCheck(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
        }

        public async Task<bool> BulkInsert(List<T> entities)
        {
            this.entities.AddRange(entities);
            return await this.tpContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> BulkUpdate(List<T> entities)
        {
            this.entities.UpdateRange(entities);
            return await this.tpContext.SaveChangesAsync() > 0;
        }

        public async Task<int> Delete(T entity)
        {
            this.entities.Remove(entity);
            return await this.tpContext.SaveChangesAsync();
        }

        public async Task<int> GetCount()
        {
            return await Task.Run(() => this.entities.Count());
        }

        public async Task<int> DeleteRange(List<T> entity)
        {
            this.entities.RemoveRange(entity);
            return await this.tpContext.SaveChangesAsync();
        }

        public async Task<IQueryable<T>> GetAll()
        {
            return await Task.Run(() => this.entities);
        }

        public async Task<bool> Insert(T entity)
        {
            EntityNullCheck(entity);
            entities.Add(entity);
            return await this.tpContext.SaveChangesAsync() > 0;

        }

        public async Task<bool> Update(T entity)
        {
            EntityNullCheck(entity);
            entities.Update(entity);
            return await this.tpContext.SaveChangesAsync() > 0;
        }

    }
}
