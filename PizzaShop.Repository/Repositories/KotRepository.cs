using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Repositories;

public class KotRepository : IKotRepository
{
    private readonly PizzaShopContext _context;
    private readonly IConfiguration _configuration;

    public KotRepository(PizzaShopContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // public async Task<IEnumerable<KotDbViewModel>> Get(long category_id, bool isReady)
    // {
    //     const string sql = "Select * from get_kot(@cat_id ,@is_ready )";

    //     using NpgsqlConnection? conn = (NpgsqlConnection)_context.Database.GetDbConnection();
    //     IEnumerable<KotDbViewModel> result = await conn.QueryAsync<KotDbViewModel>(sql, new
    //     {
    //         cat_id = category_id,
    //         is_ready = isReady
    //     });
    //     return result;
    // }

    public async Task<IEnumerable<KotDbViewModel>> Get(long category_id, bool isReady)
    {
        const string sql = "Select * from get_kot(@cat_id ,@is_ready )";

        using var connection = new NpgsqlConnection(_configuration.GetConnectionString("PizzaShopDbConnection"));

        IEnumerable<KotDbViewModel> result = await connection.QueryAsync<KotDbViewModel>(sql, new
        {
            cat_id = category_id,
            is_ready = isReady
        });
        return result;
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
