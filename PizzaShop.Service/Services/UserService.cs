using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BusinessLogicLayer.Helpers;

namespace PizzaShop.Service.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IAddressService _addressService;
    private readonly IEmailService _emailService;

    public UserService(IGenericRepository<User> userRepository, IGenericRepository<Role> roleRepository, IAddressService addressService, IEmailService emailService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _addressService = addressService;
        _emailService = emailService;
    }

#region Display User List
/*----------------------------------------------------------------Display User List--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public async Task<UsersListViewModel> GetPagedRecords(int pageSize, int pageNumber, string search)
    {
        var (users, totalRecord) = await _userRepository.GetPagedRecordsAsync(
            pageSize,
            pageNumber,
            filter: u => !u.IsDeleted && 
                         (string.IsNullOrEmpty(search.ToLower()) ||  
                            u.Role.Name.ToLower() == search.ToLower() ||
                            u.FirstName.ToLower().Contains(search.ToLower()) || 
                            u.LastName.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id), 
            includes: new List<Expression<Func<User, object>>> { u => u.Role }
        );

        UsersListViewModel model = new() { Page = new() };

        model.Users = users.Select(u => new UserInfoViewModel()
        {
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role.Name,
            Status = u.IsActive,
            UserId = u.Id,
            ProfileImageUrl = u.ProfileImg
        }).ToList();

        model.Page.SetPagination(totalRecord, pageSize, pageNumber);
        return model;
    }
#endregion Display User List

#region Add User
/*----------------------------------------------------------------Add User--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/

    //This method is used for getting the countries in 
    public async Task<AddUserViewModel> GetAddUser()
    {
        AddUserViewModel newUser = new AddUserViewModel
        {
            Countries = _addressService.GetCountries(),
            Roles = _roleRepository.GetAll().ToList()
        };

        return newUser;
    }

    public async Task<(bool success, string? message)> AddUserAsync(AddUserViewModel model, string createrEmail)
    {
        model.Email = model.Email.ToLower();

        User existingEmail = await _userRepository.GetByStringAsync(u => u.Email == model.Email && u.IsDeleted == false);
        if (existingEmail != null)
        {
            return(false, "User with same email already existed!");
        }

        User existingUserName = await _userRepository.GetByStringAsync(u => u.Username == model.UserName && u.IsDeleted == false);
         if (existingUserName != null)
        {
            return(false, "User name already existed!");
        }
    
        var creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);

        string simplePassword = model.Password;
        model.Password = PasswordHelper.HashPassword(simplePassword);

        User user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Username = model.UserName,
            RoleId = model.RoleId,
            Email = model.Email,
            Password = model.Password,
            ProfileImg = model.ProfileImageUrl,
            CountryId = model.CountryId,
            StateId = model.StateId,
            CityId = model.CityId,
            ZipCode = model.ZipCode,
            Address = model.Address,
            Phone = model.Phone,
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

        bool success = await _userRepository.AddAsync(user);

        if (success)
        {
            var body = EmailTemplateHelper.GetNewPasswordEmail(simplePassword);
            await _emailService.SendEmailAsync(model.Email, "New User", body);
        }

        return (success,"");
    }
    #endregion

#region Edit User
/*----------------------------------------------------------------Edit User--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<EditUserViewModel> GetUserAsync(long userId)
    {
        User user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return null;

        EditUserViewModel model = new EditUserViewModel
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.Username,
            Email = user.Email,
            Status = user.IsActive,
            Address = user.Address,
            ZipCode = user.ZipCode,
            Phone = user.Phone,
            ProfileImageUrl = user.ProfileImg,
            CountryId = user.CountryId,
            StateId = user.StateId,
            CityId = user.CityId,
            RoleId = user.RoleId,
            Roles = _roleRepository.GetAll().ToList(),
            Countries = _addressService.GetCountries(),
            States = _addressService.GetStates(user.CountryId),
            Cities = _addressService.GetCities(user.StateId)
        };

        return model;
    }

    public async Task<(bool success, string? message)> UpdateUser(EditUserViewModel model)
    {
        User existingUserName = await _userRepository.GetByStringAsync(u => u.Username == model.UserName && u.Email != model.Email && u.IsDeleted == false);
         if (existingUserName != null)
        {
            return(false, "User name already existed!");
        }

        User user = await _userRepository.GetByIdAsync(model.UserId);

        if (user == null)
            return (false,"User Not found");

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Username = model.UserName;
        user.RoleId = model.RoleId;
        user.IsActive = model.Status;
        user.Phone = model.Phone;
        user.CountryId = model.CountryId;
        user.StateId = model.StateId;
        user.CityId = model.CityId;
        user.Address = model.Address;
        user.ZipCode = model.ZipCode;

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

        bool success = await _userRepository.UpdateAsync(user);

        return (success,"");

    }

    #endregion

#region Soft Delete
/*----------------------------------------------------------------Soft Delete User--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public async Task<bool> SoftDeleteUser(long id)
    {
        User user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return false;

        user.IsDeleted = true;

        bool success = await _userRepository.UpdateAsync(user);

        return success;


    }

    #endregion

}
