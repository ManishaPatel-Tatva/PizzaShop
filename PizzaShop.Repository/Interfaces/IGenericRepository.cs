
using System.Linq.Expressions;

namespace PizzaShop.Repository.Interfaces;

public interface IGenericRepository<T>
    where T : class
{
    Task AddAsync(T entity);
    Task<long> AddAsyncReturnId(T entity);

    IEnumerable<T> GetAll();

    Task<IEnumerable<T>> GetByCondition
    (
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<Expression<Func<T, object>>>? includes = null,
        List<Func<IQueryable<T>, IQueryable<T>>>? thenIncludes = null
    );

    (IEnumerable<T> items, int totalCount) GetPagedRecords
    (
        int pageSize,
        int pageNumber,
        IEnumerable<T> items
    );

    Task<T?> GetByIdAsync(long id);

    Task<T?> GetByStringAsync(Expression<Func<T, bool>> predicate);

    Task UpdateAsync(T entity);
}
