using HotelListingApi.Data;
using HotelListingApi.Data.Interfaces;
using HotelListingApi.HotelListResourceParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HotelListingApi.Repository
{
    public class GenericRepsitory<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _db;

        public GenericRepsitory(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _db = context.Set<T>();
        }
        public async Task CreateAsync(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task CreateRangeAsync(IEnumerable<T> enttities)
        {
           await  _db.AddRangeAsync(enttities);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.FindAsync(id);
            if (id == null) throw new ArgumentNullException("No id matching this");
            _db.Remove(entity);
        }

        public void DeleteRangeAsync(IEnumerable<T> enttities)
        {
            _db.RemoveRange(enttities);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderdBy = null, List<string> includes = null)
        {
            IQueryable<T> query = _db;

            if(expression != null)
            {
                query = query.Where(expression);
            }

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            if(orderdBy != null)
            {
                query = orderdBy(query);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<IPagedList<T>> GetAllAsync(Hotel_List_Resource_Parameters resourceParameters, List<string> includes = null)
        {
            IQueryable<T> query = _db;

            if (resourceParameters == null)
            {
                throw new ArgumentNullException(nameof(resourceParameters));
            }

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }
            return await query.AsNoTracking().ToPagedListAsync(resourceParameters.PageNumber , resourceParameters.PageSize);
        }

        public async Task<T> GetByIdAsync(Expression<Func<T, bool>> expression, List<string> includes = null)
        {
            IQueryable<T> query = _db;
            if(includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }

            }

            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }


        public void UpdateAsync(T entity)
        {
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
