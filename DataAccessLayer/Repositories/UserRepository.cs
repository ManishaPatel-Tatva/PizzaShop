using System.Threading.Tasks;
using DataAccessLayer.Models;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PizzaShopContext _context;

    public UserRepository(PizzaShopContext context)
    {
        _context = context;
    }

   public async Task<List<User>> GetAllUsersAsync()
   {
        return await _context.Users.ToListAsync();
   }

    public async Task<List<UserInfoViewModel>> GetUsersInfoAsync()
    {
        var user = await _context.Users
        .Include(u => u.Role)             // Ensure Role data is fetched
        .Select(u => new UserInfoViewModel
        {
            UserId = u.Id,
            ProfileImageUrl = u.ProfileImg,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role.Name,          //  RoleName is in Role model
            Status = u.IsActive
        })
        .ToListAsync();

        return user;  
    }

     public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(m => m.Email == email);
    }

    public async Task<User?> GetUserByIdAsync(long id)
    {
        return await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Role?> GetUserRoleAsync(long roleId)
    {
        return await _context.Roles.SingleOrDefaultAsync(u => u.Id == roleId);
    }


    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    // public (IEnumerable<User> users, int totalRecords) GetPagedRecordsAsync(int pageSize, int pageNumber)
    // {
    //     IQueryable<User> query = _context.Users;
    //     return (
    //         query.OrderBy(p => p.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
    //         query.Count()
    //     );
    // }


}

