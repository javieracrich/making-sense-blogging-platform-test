using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Services
{
    public interface IGenericService
    {

        Task<T> GetAsync<T>(int id, string includeProperties = "") where T : BaseEntity;

        Task<int> CreateAsync<T>(T entity) where T : BaseEntity;

        Task<int> CreateRangeAsync<T>(IEnumerable<T> entities) where T : BaseEntity;

        Task<int> UpdateAsync<T>(T entity) where T : BaseEntity;

        Task<int> UpsertAsync<T>(T entity) where T : BaseEntity;

        Task<int> DeleteAsync<T>(T entity) where T : BaseEntity;

        Task<int> DeleteRangeAsync<T>(IEnumerable<T> entities) where T : BaseEntity;

        Task<int> CountAsync<T>(Expression<Func<T, bool>> filter = null) where T : BaseEntity;

        Task<List<T>> GetAllAsync<T>(bool noTrack = true, params Expression<Func<T, object>>[] includes) where T : BaseEntity;

        Task<List<T>> FindAsync<T>(Expression<Func<T, bool>> filter = null,
                                   Func<IQueryable<T>,
                                   IOrderedQueryable<T>> orderBy = null,
                                   bool noTrack = false,
                                   params Expression<Func<T, object>>[] includes)
            where T : BaseEntity;

        Task<T> Single<T>(Expression<Func<T, bool>> filter = null,
                          bool noTrack = false,
                          params Expression<Func<T, object>>[] includes)
                          where T : BaseEntity;

    }
}