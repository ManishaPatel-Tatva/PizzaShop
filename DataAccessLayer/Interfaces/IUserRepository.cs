using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces;
public interface IUserRepository
{
    IEnumerable<User> GetAll();
    (IEnumerable<User> users, int totalRecords) GetPagedRecordsAsync(
        int pageSize,
        int pageNumber
    );
    Task AddAsync(User user);
    Task<User?> GetUserByEmailAsync(string email);
    Task<Role?> GetUserRoleAsync(long roleId);
    
    Task UpdateAsync(User user);
}


