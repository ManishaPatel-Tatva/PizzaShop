
using System.Linq.Expressions;

namespace PizzaShop.Repository.Interfaces;

public interface IGenericRepository<T>
    where T : class
{
    Task<bool> AddAsync(T entity);

    IEnumerable<T> GetAll();

    IEnumerable<T> GetByCondition(Expression<Func<T, bool>> predicate);

    (IEnumerable<T> records, int totalRecord) GetPagedRecords
    (
        int pageSize,
        int pageNumber,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy
    );

    (IEnumerable<T> records, int totalRecord) GetPagedRecords
    (
        int pageSize,
        int pageNumber,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        Expression<Func<T, bool>>? filter = null,
        List<Expression<Func<T, object>>>? includes = null
    );

    public Task<(IEnumerable<T> items, int totalCount)> GetPagedRecordsAsync
    (
        int pageSize,
        int pageNumber,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<Expression<Func<T, object>>>? includes = null
    );

    Task<T?> GetByIdAsync(long id);

    Task<T?> GetByStringAsync(Expression<Func<T, bool>> predicate);

    Task<bool> UpdateAsync(T entity);

    Task DeleteAsync(int id);

    Task DeleteAssociatedEntitiesAsync(Expression<Func<T, bool>> predicate);

    Task<int> GetCount(Expression<Func<T, bool>>? predicate = null);
    
}
