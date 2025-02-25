using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces;
public interface IHomeRepository
{
    Task<User> GetOneByEmailAsync(string email);
};