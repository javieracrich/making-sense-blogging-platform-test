using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly DbContext _ctx;

        public UserService(IUnitOfWork unitOfWork)
        {
            _ctx = unitOfWork.Context;
        }

        public virtual int Count(Expression<Func<User, bool>> filter = null)
        {
            IQueryable<User> query = _ctx.Set<User>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Count();
        }

        public Task<T> Single<T>(Expression<Func<T, bool>> filter = null, string includeProperties = "") where T : IdentityUser
        {
            IQueryable<T> query = _ctx.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return query.FirstOrDefaultAsync();
        }


        public Task<List<T>> FindAsync<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, bool noTrack = false, string includeProperties = "") where T : IdentityUser
        {
            var query = noTrack ? _ctx.Set<T>().AsNoTracking() : _ctx.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return orderBy != null ? orderBy(query).ToListAsync() : query.ToListAsync();
        }

        public virtual IEnumerable<User> GetAll()
        {
            return _ctx
                .Set<User>()
                .AsEnumerable();
        }
        public virtual IEnumerable<User> GetAllAsNoTracking()
        {
            return _ctx
                .Set<User>()
                .AsNoTracking()
                .AsEnumerable();
        }

        /// <summary>
        /// THIS METHOD IS INTENDED TO USED EXCLUSIVELY IN THE TESTUSERSTORE<USER>. DO NOT USE ELSEWHERE.
        /// IF YOU NEED TO CREATE A NEW USER, USE THE USERMANAGER
        /// </summary>
        /// <param name="user"></param>
        public virtual Task<int> CreateAsync(User user)
        {
            _ctx.Add(user);
            return _ctx.SaveChangesAsync();
        }


        public virtual Task<int> UpdateAsync(User user)
        {
            _ctx.Update(user);
            return _ctx.SaveChangesAsync();
        }

  
    }


    public interface IUserService
    {
        int Count(Expression<Func<User, bool>> filter = null);
        IEnumerable<User> GetAll();
        IEnumerable<User> GetAllAsNoTracking();

        Task<List<T>> FindAsync<T>(Expression<Func<T, bool>> filter = null,
                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                bool noTrack = false,
                                string includeProperties = "")
                                where T : IdentityUser;

        /// <summary>
        /// THIS METHOD IS INTENDED TO USED EXCLUSIVELY IN THE TESTUSERSTORE<USER>. DO NOT USE ELSEWHERE.
        /// IF YOU NEED TO CREATE A NEW USER, USE THE USERMANAGER
        /// </summary>
        /// <param name="user"></param>
        Task<int> CreateAsync(User user);

        Task<int> UpdateAsync(User user);

        Task<T> Single<T>(Expression<Func<T, bool>> filter = null, string includeProperties = "")
            where T : IdentityUser;
    }
}
