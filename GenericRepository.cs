using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WMS_DAL.Models;

namespace WMS_DAL.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private WMSContext _context;
        protected WMSContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public GenericRepository(WMSContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }
        public async Task<IEnumerable<VM>> FindAsync<VM>(Expression<Func<T, bool>> predicate, Expression<Func<T, VM>> columns)
        {
            return await _context.Set<T>().Where(predicate).Select<T, VM>(columns).ToListAsync();
            //return _context.Set<T>().Where(predicate).ToList();
        }

        public virtual async Task<T> FindByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }



        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null,
                                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                          string includeProperties = "")
        {
            IQueryable<T> query = _context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }
        public async Task<IEnumerable<VM>> GetSelectedAllAsync<VM>(Expression<Func<T, bool>> predicate,
                                        Expression<Func<T, VM>> columns,
                                        Func<IQueryable<VM>, IOrderedQueryable<VM>> orderBy = null)
        {
            IQueryable<VM> query = _context.Set<T>().Where(predicate).Select(columns);
            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }


        }
        public async Task<IEnumerable<VM>> GetPagedListAsync<VM>(Expression<Func<T, bool>> predicate,
                                         Expression<Func<T, VM>> columns,
                                         Func<IQueryable<VM>, IOrderedQueryable<VM>> orderBy = null,
                                         int size = 10,
                                         int pageIndex = 0,
                                         int sortOrder = 0)
        {
            IQueryable<VM> query = _context.Set<T>().Where(predicate).Select(columns);
            if (orderBy != null)
            {
                return await orderBy(query).Skip(size * pageIndex).Take(size).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }


        }

        //public IEnumerable<VM> GetSelectedAll<VM>(Func<Expression<Func<T, bool>>> isAny1, Func<Expression<Func<T, VM>>> isAny2, Func<Func<IQueryable<VM>, IOrderedQueryable<VM>>> isAny3)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
