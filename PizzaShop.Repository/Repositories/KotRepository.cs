using Microsoft.EntityFrameworkCore;
using Npgsql;
using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Repositories;

public class KotRepository : IKotRepository
{
    private readonly PizzaShopContext _context;

    public KotRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task Update(bool isReady, int quantity, long id)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "call update_kot_item(@status, @quantity, @id)",
            new NpgsqlParameter("status", isReady),
            new NpgsqlParameter("quantity", quantity),
            new NpgsqlParameter("id", id)
        );    
    }

}
