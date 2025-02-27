using BusinessLogicLayer.Helpers;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;
    private readonly ICountryRepository _countryRepository;
    public UserService(IUserRepository userRepository, JwtService jwtService, ICountryRepository countryRepository)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _countryRepository = countryRepository;
    }

#region Display User List

    public async Task<UsersListViewModel> GetUsersListAsync()
    {
        var users = await _userRepository.GetUsersInfoAsync();
        return new UsersListViewModel { User = users };
    }

#endregion

#region  Add User

    public async Task<AddUserViewModel> GetAddUser()
    {
        AddUserViewModel newUser = new AddUserViewModel
        {
            Countries = _countryRepository.GetCountries()
        };

        return newUser;
    }

    public async Task AddUserAsync(AddUserViewModel model, string token)
    {
        if (string.IsNullOrEmpty(token)) 
            throw new UnauthorizedAccessException("Invalid token.");

        var email = _jwtService.GetClaimValue(token, "email");
        var creater = await GetUserByEmailAsync(email);

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Username = model.UserName,
            RoleId = model.RoleId,
            Email = model.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
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

        await _userRepository.AddAsync(user);
    }
#endregion



    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        return user;
    }

    // public async Task<User?> GetUserByIdAsync(long id)
    // {
    //     var user = await _userRepository.GetUserByIdAsync(id);
    //     return user;
    // }

    public async Task<List<UserInfoViewModel>> GetUserInfoAsync()
    {
        var userList = await _userRepository.GetUsersInfoAsync();
        return userList;
    }


    public async Task UpdateUserAsync(User user)
    {
        try{
            await _userRepository.UpdateAsync(user);
            
        }
        catch(Exception)
        {
            
        }
        

    }
}
