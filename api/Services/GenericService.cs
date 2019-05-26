using Common;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Services
{

    public class GenericService : IGenericService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeService _dateTimeService;
        private readonly MakingSenseDbContext _ctx;
        private readonly ClaimsPrincipal _principal;

        public GenericService(IUnitOfWork unitOfWork, IDateTimeService dateTimeService, IPrincipal principal)
        {
            _unitOfWork = unitOfWork;
            _ctx = unitOfWork.Context;
            _dateTimeService = dateTimeService;
            _principal = principal as ClaimsPrincipal;
        }


        public virtual Task<T> GetAsync<T>(int id, string includeProperties = "") where T : BaseEntity
        {
            IQueryable<T> query = _ctx.Set<T>();

            query = query.Where(x => !x.IsDisabled.HasValue || !x.IsDisabled.Value);

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return query.SingleOrDefaultAsync(x => x.Id == id);
        }

        public virtual Task<int> CreateAsync<T>(T entity) where T : BaseEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.Created = _dateTimeService.UtcNow();
            entity.CreatedBy = GetCurrentUserId();
            _ctx.Set<T>().Add(entity);
            return _unitOfWork.CommitAsync();
        }

        public virtual Task<int> CreateRangeAsync<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var list = entities.ToList();
            foreach (var entity in list)
            {
                entity.Created = DateTime.UtcNow;
                entity.CreatedBy = GetCurrentUserId();
            }

            _ctx.Set<T>().AddRange(list);
            return _unitOfWork.CommitAsync();
        }

        public virtual Task<int> UpdateAsync<T>(T entity) where T : BaseEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.IsDisabled.HasValue && entity.IsDisabled.Value)
            {
                throw new ArgumentException("entity is disabled");
            }

            entity.Updated = _dateTimeService.UtcNow();
            entity.UpdatedBy = GetCurrentUserId();
            _ctx.Entry(entity).State = EntityState.Modified;
            return _unitOfWork.CommitAsync();
        }

        public virtual Task<int> UpsertAsync<T>(T entity) where T : BaseEntity
        {
            InnerUpsert(entity);
            return _unitOfWork.CommitAsync();
        }

        public virtual Task<int> CountAsync<T>(Expression<Func<T, bool>> filter = null) where T : BaseEntity
        {
            var query = _ctx.Set<T>().AsNoTracking();

            query = query.Where(x => !x.IsDisabled.HasValue || !x.IsDisabled.Value);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.CountAsync();
        }

        public virtual Task<int> DeleteAsync<T>(T entity) where T : BaseEntity
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _ctx.Set<T>().Remove(entity);
            return _unitOfWork.CommitAsync();
        }

        public virtual Task<int> DeleteRangeAsync<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            _ctx.Set<T>().RemoveRange(entities);
            return _unitOfWork.CommitAsync();
        }

        public virtual int SoftDelete<T>(T entity) where T : BaseEntity
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            entity.Updated = _dateTimeService.UtcNow();
            entity.UpdatedBy = GetCurrentUserId();
            entity.IsDisabled = true;
            _ctx.Entry(entity).State = EntityState.Modified;
            return _unitOfWork.Commit();
        }

        public virtual Task<T> Single<T>(Expression<Func<T, bool>> filter = null, bool noTrack = false, params Expression<Func<T, object>>[] includes) where T : BaseEntity
        {
            var query = noTrack ? _ctx.Set<T>().AsNoTracking() : _ctx.Set<T>();

            query = query.Where(x => !x.IsDisabled.HasValue || !x.IsDisabled.Value);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return query.FirstOrDefaultAsync();
        }

        public virtual Task<List<T>> FindAsync<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, bool noTrack = false, params Expression<Func<T, object>>[] includes) where T : BaseEntity
        {
            var query = noTrack ? _ctx.Set<T>().AsNoTracking() : _ctx.Set<T>();

            query = query.Where(x => !x.IsDisabled.HasValue || !x.IsDisabled.Value);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return orderBy != null ? orderBy(query).ToListAsync() : query.ToListAsync();
        }

        public virtual Task<List<T>> GetAllAsync<T>(bool noTrack = true, params Expression<Func<T, object>>[] includes) where T : BaseEntity
        {
            var query = noTrack ? _ctx.Set<T>().AsNoTracking() : _ctx.Set<T>();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return query
                .Where(x => !x.IsDisabled.HasValue || !x.IsDisabled.Value)
                .ToListAsync();

        }

        private void InnerUpsert<T>(T entity) where T : BaseEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.IsDisabled.HasValue && entity.IsDisabled.Value)
            {
                throw new ArgumentException("entity is disabled");
            }

            if (entity.Id == 0)
            {
                entity.Created = _dateTimeService.UtcNow();
                entity.CreatedBy = GetCurrentUserId();
                _ctx.Entry(entity).State = EntityState.Added;
                _ctx.Set<T>().Add(entity);
            }
            else
            {
                entity.Updated = _dateTimeService.UtcNow();
                entity.UpdatedBy = GetCurrentUserId();
                _ctx.Entry(entity).State = EntityState.Modified;
                _ctx.Set<T>().Update(entity);
            }
        }

        private string GetCurrentUserId()
        {
            return _principal.Claims.FirstOrDefault(x => x.Type == Constants.JwtClaimIdentifiers.Id).Value;
        }
    }
}
