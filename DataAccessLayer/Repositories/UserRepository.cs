using System.Threading.Tasks;
using DataAccessLayer.Models;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.ViewModels;
using DataAccessLayer.Repositories;

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

    /*-------------------------------------------------User List------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<UsersListViewModel> GetUsersInfoAsync(int pageNumber, int pageSize, string search)
    {
        var user = _context.Users
        .Where(u => u.IsDeleted == false)
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
        });

        var usersQuery =  user.AsQueryable();

        var totalRecords = usersQuery.Count();

        var usersList = await usersQuery
        .OrderBy(u => u.UserId)
        .Skip((pageSize-1)*pageSize)
        .Take(pageSize)
        .ToListAsync();

        var viewModel = new UsersListViewModel
        {
            User = usersList,
            TotalRecords = totalRecords
        };

        return viewModel;
    }

    public List<Role> GetRoles()
    {
        return _context.Roles.ToList();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(m => m.Email == email);
    }

    public async Task<Role?> GetUserRoleAsync(long roleId)
    {
        return await _context.Roles.SingleOrDefaultAsync(u => u.Id == roleId);
    }


    public async Task<bool> AddUserAsync(AddUserViewModel model, string createrEmail)
    {
        var creater = await _context.Users.SingleOrDefaultAsync(m => m.Email == createrEmail);

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Username = model.UserName,
            RoleId = model.RoleId,
            Email = model.Email,
            Password = model.Password,
            Phone = model.Phone,
            CountryId = model.CountryId,
            StateId = model.StateId,
            CityId = model.CityId,
            Address = model.Address,
            ZipCode = model.ZipCode,
            CreatedBy = creater.Id
        };

        // Handle Image Upload
        if (model.Image != null)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{Guid.NewGuid()}_{model.Image.FileName}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(stream);
            }

            user.ProfileImg = $"/uploads/{fileName}";
        }

        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch(Exception)
        {
            return false;
        }
        
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    /*--------------------------------------------View Data for Edit User -----------------------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------*/
    public EditUserViewModel GetUserByIdAsync(long id)
    {
        var user = _context.Users
        .Where(u => u.Id == id)             // Ensure Role data is fetched
        .Select(user => new EditUserViewModel
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.Username,
            RoleId = user.RoleId,
            Email = user.Email,
            Status = user.IsActive,
            Phone = user.Phone,
            CountryId = user.CountryId,
            StateId = user.StateId,
            CityId = user.CityId,
            Address = user.Address,
            ZipCode = user.ZipCode,
            ProfileImageUrl = user.ProfileImg,
        })
        .FirstOrDefault();

        return user;
    }

    /*--------------------------------------------Update Data for Edit User -----------------------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------*/

    public async Task<bool> UpdateUser(EditUserViewModel model)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == model.UserId);

        try
        {
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Username = model.UserName;
            user.RoleId = model.RoleId;
            user.IsActive = model.Status;
            user.CountryId = model.CountryId;
            user.StateId = model.StateId;
            user.CityId = model.CityId;
            user.ZipCode = model.ZipCode;
            user.Address = model.Address;
            user.Phone = model.Phone;
            // Handle Image Upload
            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = $"{Guid.NewGuid()}_{model.Image.FileName}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                user.ProfileImg = $"/uploads/{fileName}";
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }


    /*--------------------------------------------Update Data for Edit User -----------------------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------*/
    #region SoftDelete

    public async Task<bool> SoftDeleteUser(long id)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);

        try
        {
            user.IsDeleted = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    #endregion




    // public (IEnumerable<User> users, int totalRecords) GetPagedRecordsAsync(int pageSize, int pageNumber)
    // {
    //     IQueryable<User> query = _context.Users;
    //     return (
    //         query.OrderBy(p => p.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
    //         query.Count()
    //     );
    // }


}

