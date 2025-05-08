using System.Linq.Expressions;
using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Reflection;

namespace PizzaShop.Repository.Repositories;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{
    private readonly PizzaShopContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(PizzaShopContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    #region C : Create
    /*------------------------------adds a new entity (record) to the database----------------------------------------
    -------------------------------------------------------------------------------------------------------*/
    public async Task<bool> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<long> AddAsyncReturnId(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        PropertyInfo? idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null)
        {
            return (long)idProperty.GetValue(entity);
        }

        return 0;
    }
    #endregion C : Create

    #region R : Read
    /*----------------------------To Get the all the values from table----------------------------------------
    -------------------------------------------------------------------------------------------------------*/
    public IEnumerable<T> GetAll() => _dbSet;

    public async Task<IEnumerable<T>> GetByCondition(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<Expression<Func<T, object>>>? includes = null,
        List<Func<IQueryable<T>, IQueryable<T>>>? thenIncludes = null)
    {

        IQueryable<T> query = _dbSet;

        //Apply Filters
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        //Order By
        if (orderBy != null)
        {
            query = orderBy(query);
        }

        // Apply Includes (First-level navigation properties)
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        // Apply ThenIncludes (Deeper navigation properties)
        if (thenIncludes != null)
        {
            foreach (var thenInclude in thenIncludes)
            {
                query = thenInclude(query);
            }
        }

        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<T> items, int totalCount)> GetPagedRecords
    (
        int pageSize,
        int pageNumber,
        IEnumerable<T>? items = null
    )
    {
        int totalCount = items.Count();

        items = items
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }

    /*----------------------retrieves a single record from the database by its primary key (id)----------------------------------------
    -------------------------------------------------------------------------------------------------------*/
    public async Task<T?> GetByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }


    /*----------------------fetches a single record from the database based on a given condition----------------------------------------
    -------------------------------------------------------------------------------------------------------*/
    public async Task<T?> GetByStringAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    #endregion R : Read

    #region U : Update
    /*------------------------------updates an existing entity in the database----------------------------------------
    -------------------------------------------------------------------------------------------------------*/
    public async Task<bool> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion U : Update

    #region D : Delete
    /*------------------Only Soft Delete is allowed so no remove method------------------------------------
    -------------------------------------------------------------------------------------------------------*/
    #endregion D : Delete

    #region Common
    /*---------------Counts the number of records in a table, with an optional filter (predicate)-------------
    -------------------------------------------------------------------------------------------------------*/
    public async Task<int> GetCount(Expression<Func<T, bool>>? predicate)
    {
        if (predicate is not null)
        {
            return await _dbSet.CountAsync(predicate);
        }
        return await _dbSet.CountAsync();
    }

    #endregion Common
}

