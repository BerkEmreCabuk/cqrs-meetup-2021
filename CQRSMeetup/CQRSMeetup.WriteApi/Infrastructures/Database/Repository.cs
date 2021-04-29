using CQRSMeetup.Core.Enums;
using CQRSMeetup.Core.RabbitMq;
using CQRSMeetup.WriteDomain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CQRSMeetup.WriteApi.Infrastructures.Database
{
    public interface IRepository
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        IQueryable<T> Query<T>() where T : BaseEntity;
        Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate, bool isTracking = false, params Expression<Func<T, object>>[] includeEntities) where T : BaseEntity;
        Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate, bool isTracking = false, params Expression<Func<T, object>>[] includeEntities) where T : BaseEntity;
        Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeEntities) where T : BaseEntity;
        Task<bool> ExistAsync<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity;
        Task<List<T>> GetListAsync<T>(bool isTracking = false) where T : BaseEntity;
        Task<T> GetByIdAsync<T>(long id, bool isTracking = false) where T : BaseEntity;
        T Add<T>(T model) where T : BaseEntity;
        T Update<T>(T model) where T : BaseEntity;
        void Delete<T>(T model) where T : BaseEntity;
        void Remove<T>(T model) where T : BaseEntity;
    }
    public class Repository : IRepository
    {
        private readonly CQRSMeetupDbContext _context;
        private readonly IRabbitMqService _rabbitMqService;
        public Repository(CQRSMeetupDbContext context, IRabbitMqService rabbitMqService)
        {
            _context = context;
            _rabbitMqService = rabbitMqService;
        }

        public virtual IQueryable<T> Query<T>() where T : BaseEntity
        {
            return _context.Set<T>().AsQueryable();
        }

        public virtual Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeEntities) where T : BaseEntity
        {
            return GetListAsync<T>(predicate, false, includeEntities);
        }

        public virtual Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate, bool isTracking = false, params Expression<Func<T, object>>[] includeEntities) where T : BaseEntity
        {
            var result = Query<T>();
            foreach (var include in includeEntities)
            {
                result = result.Include(include);
            }
            return isTracking
                ? result.Where(predicate).ToListAsync<T>()
                : result.AsNoTracking().Where(predicate).ToListAsync<T>();
        }

        public virtual Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeEntities) where T : BaseEntity
        {
            return FindAsync<T>(predicate, false, includeEntities);
        }

        public virtual Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate, bool isTracking = false, params Expression<Func<T, object>>[] includeEntities) where T : BaseEntity
        {
            var result = Query<T>();
            foreach (var include in includeEntities)
            {
                result = result.Include(include);
            }
            return isTracking
                ? result.FirstOrDefaultAsync(predicate)
                : result.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public virtual Task<bool> ExistAsync<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            return Query<T>().AnyAsync<T>(predicate);
        }

        public virtual Task<T> GetByIdAsync<T>(long id, bool isTracking = false) where T : BaseEntity
        {
            var result = Query<T>();
            return isTracking
                ? result.FirstOrDefaultAsync(x => x.Id == id && x.Status == RecordStatuses.ACTIVE)
                : result.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.Status == RecordStatuses.ACTIVE);
        }

        public virtual Task<List<T>> GetListAsync<T>(bool isTracking = false) where T : BaseEntity
        {
            var result = Query<T>();
            return isTracking
                ? result.Where(x => x.Status == RecordStatuses.ACTIVE).ToListAsync()
                : result.AsNoTracking().Where(x => x.Status == RecordStatuses.ACTIVE).ToListAsync();
        }
        public virtual T Add<T>(T model) where T : BaseEntity
        {
            model.Add();
            return _context.Set<T>().Add(model).Entity;
        }

        public virtual T Update<T>(T model) where T : BaseEntity
        {
            model.Update();
            return _context.Set<T>().Update(model).Entity;
        }

        public virtual void Delete<T>(T model) where T : BaseEntity
        {
            model.Delete();
            _context.Set<T>().Update(model);
        }

        public virtual void Remove<T>(T model) where T : BaseEntity
        {
            _context.Set<T>().Remove(model);
        }

        public virtual int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            var entries = _context.ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted).ToList();
            var result= await _context.SaveChangesAsync();
            foreach (var entry in entries)
            {
                if (entry.Entity.GetType() == typeof(StockEntity))
                    _rabbitMqService.Publish("StockQueue", entry.Entity);
            }
            return result;
        }
    }
}
