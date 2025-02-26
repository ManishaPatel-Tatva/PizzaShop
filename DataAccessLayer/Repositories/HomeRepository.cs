using System.Threading.Tasks;
using DataAccessLayer.ViewModels;
using DataAccessLayer.Models;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Implementation;
public class HomeRepository : IHomeRepository
{
    private readonly PizzaShopContext _context;

    public async Task<User> GetOneByEmailAsync(string email)
    {
        return await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();  
    }
}
