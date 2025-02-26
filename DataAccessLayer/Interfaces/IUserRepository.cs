using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Interfaces;
public interface IUserRepository
{
    public IEnumerable<User> GetAll();

    // (IEnumerable<User> users, int totalRecords) GetPagedRecordsAsync(
    //     int pageSize,
    //     int pageNumber
    // );

    public Task AddAsync(User user);

    public Task<User?> GetUserByEmailAsync(string email);

    public Task<Role?> GetUserRoleAsync(long roleId);
    
    public Task UpdateAsync(User user);
}


