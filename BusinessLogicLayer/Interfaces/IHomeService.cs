using DataAccessLayer.Models;

namespace BusinessLogicLayer.Interfaces;

public class IHomeService
{
    public Task<User> GetOneByEmailAsync(int id, string operationType);
}
