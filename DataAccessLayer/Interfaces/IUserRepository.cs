using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Interfaces;
public interface IUserRepository
{
    public Task<List<User>> GetAllUsersAsync();

    // (IEnumerable<User> users, int totalRecords) GetPagedRecordsAsync(
    //     int pageSize,
    //     int pageNumber
    // );

    public List<Role> GetRoles();

    public Task<UsersListViewModel> GetUsersInfoAsync(int pageNumber, int pageSize, string search);
    
    public Task<bool> AddUserAsync(AddUserViewModel model, string createrEmail);

    public Task<User?> GetUserByEmailAsync(string email);

    public EditUserViewModel GetUserByIdAsync(long id);

    public Task<Role?> GetUserRoleAsync(long roleId);
    
    public Task UpdateAsync(User user);

    public Task<bool> UpdateUser(EditUserViewModel model);
    
    public Task<bool> SoftDeleteUser(long id);
}


