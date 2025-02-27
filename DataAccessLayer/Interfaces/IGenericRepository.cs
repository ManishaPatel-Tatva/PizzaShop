using System.Linq.Expressions;

namespace DataAccessLayer.Interfaces;

public interface IGenericRepository<T>
    where T : class
{
    IEnumerable<T> GetAll();
    (IEnumerable<T> records, int totalRecord) GetPagedRecords(
        int pageSize,
        int pageNumber,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy
    );
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<int> GetCount(Expression<Func<T, bool>>? predicate = null);
    Task DeleteAssociatedEntitiesAsync(Expression<Func<T, bool>> predicate);
}
