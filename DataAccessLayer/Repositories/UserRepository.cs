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

    public IEnumerable<User> GetAll() => _context.Users;

     public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(m => m.Email == email);
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

