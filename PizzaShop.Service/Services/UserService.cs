using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using System.Linq.Expressions;
using PizzaShop.Service.Common;

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

    #region Get
    /*----------------------------------------------------------------Get User By Email--------------------------------------------------------------------------------------------
    -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<User> Get(string email)
    {
        User? user = await _userRepository.GetByStringAsync(u => u.Email == email && !u.IsDeleted);
        return user;
    }

    /*----------------------------------------------------------------Get Users Pagination List with filters--------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<UserPaginationViewModel> Get(FilterViewModel filter)
    {
        filter.Search = string.IsNullOrEmpty(filter.Search) ? "" : filter.Search.Replace(" ", "");

        //For sorting the column according to order
        Func<IQueryable<User>, IOrderedQueryable<User>>? sortingColumn = q => q.OrderBy(u => u.Id);
        if (!string.IsNullOrEmpty(filter.Column))
        {
            switch (filter.Column.ToLower())
            {
                case "name":
                    sortingColumn = filter.Sort == "asc" ? q => q.OrderBy(u => u.FirstName) : q => q.OrderByDescending(u => u.FirstName);
                    break;
                case "role":
                    sortingColumn = filter.Sort == "asc" ? q => q.OrderBy(u => u.Role.Name) : q => q.OrderByDescending(u => u.Role.Name);
                    break;
                default:
                    break;
            }
        }

        (IEnumerable<User> users, int totalRecord) = await _userRepository.GetPagedRecordsAsync(
            filter.PageSize,
            filter.PageNumber,
            predicate: u => !u.IsDeleted &&
                         (string.IsNullOrEmpty(filter.Search.ToLower()) ||
                            u.Role.Name.ToLower() == filter.Search.ToLower() ||
                            u.FirstName.ToLower().Contains(filter.Search.ToLower()) ||
                            u.LastName.ToLower().Contains(filter.Search.ToLower())),
            orderBy: sortingColumn,
            includes: new List<Expression<Func<User, object>>> { u => u.Role }
        );

        UserPaginationViewModel model = new()
        {
            Page = new(),
            Users = users.Select(u => new UserInfoViewModel()
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role.Name,
                Status = u.IsActive,
                UserId = u.Id,
                ProfileImageUrl = u.ProfileImg
            }).ToList()
        };

        model.Page.SetPagination(totalRecord, filter.PageSize, filter.PageNumber);
        return model;
    }

    /*---------------------------------------------------------------Get User by Id-------------------------------------------------------------------------------------------------
    -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<EditUserViewModel> Get(long userId)
    {
        User? user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        EditUserViewModel userVM = new()
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

        return userVM;
    }

    /*---------------------------------------------------------------For getting the countries/roles in Add User  --------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<AddUserViewModel> Get()
    {
        return new AddUserViewModel
        {
            Countries = _addressService.GetCountries(),
            Roles = _roleRepository.GetAll().ToList()
        };
    }
    #endregion Get

    #region Add
    /*----------------------------------------------------------------Add User--------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<ResponseViewModel> Add(AddUserViewModel model, string createrEmail)
    {
        model.Email = model.Email.ToLower();

        //Checking if email already existed
        User existingEmail = await _userRepository.GetByStringAsync(u => u.Email.ToLower() == model.Email && u.IsDeleted == false);
        if (existingEmail != null)
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.AlreadyExisted.Replace("{0}", "Email")
            };
        }

        //Checking if User Name already existed
        User existingUserName = await _userRepository.GetByStringAsync(u => u.Username == model.UserName && u.IsDeleted == false);
        if (existingUserName != null)
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.AlreadyExisted.Replace("{0}", "User Name")
            };
        }

        User? creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail && !u.IsDeleted);

        //Hashing the simple password
        string simplePassword = model.Password;
        model.Password = PasswordHelper.HashPassword(simplePassword);

        //Setting values into View Model
        User user = new()
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
            CreatedBy = creater.Id,
            UpdatedBy = creater.Id,
            UpdatedAt = DateTime.Now
        };

        // Handle Image Upload
        if (model.Image != null)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = $"{Guid.NewGuid()}_{model.Image.FileName}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (FileStream? stream = new(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(stream);
            }

            user.ProfileImg = $"/uploads/{fileName}";
        }

        //Send Email if user added succesfully
        if (await _userRepository.AddAsync(user))
        {
            string? body = EmailTemplateHelper.GetNewPasswordEmail(simplePassword);
            await _emailService.SendEmailAsync(model.Email, "New User", body);
            return new ResponseViewModel
            {
                Success = true,
                Message = NotificationMessages.Added.Replace("{0}", "User")
            };
        }

        return new ResponseViewModel
        {
            Success = false,
            Message = NotificationMessages.AddedFailed.Replace("{0}", "User")
        };
    }
    #endregion

    #region Update
    /*----------------------------------------------------------------Update User--------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<ResponseViewModel> Update(EditUserViewModel model, string createrEmail)
    {
        User existingUserName = await _userRepository.GetByStringAsync(u => u.Username == model.UserName && u.Email != model.Email && u.IsDeleted == false);
        if (existingUserName != null)
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.AlreadyExisted.Replace("{0}", "User Name")
            };
        }

        User user = await _userRepository.GetByIdAsync(model.UserId);
        if (user == null)
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.NotFound.Replace("{0}", "User")
            };
        }

        User? creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail && !u.IsDeleted);

        //Updating Values    
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
        user.UpdatedBy = creater.Id;
        user.UpdatedAt = DateTime.Now;

        // Handle Image Upload
        if (model.Image != null)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string fileName = $"{Guid.NewGuid()}_{model.Image.FileName}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(stream);
            }

            user.ProfileImg = $"/uploads/{fileName}";
        }

        if (await _userRepository.UpdateAsync(user))
        {
            return new ResponseViewModel
            {
                Success = true,
                Message = NotificationMessages.Updated.Replace("{0}", "User")
            };
        }

        return new ResponseViewModel
        {
            Success = false,
            Message = NotificationMessages.UpdatedFailed.Replace("{0}", "User")
        };
    }
    #endregion

    #region Delete
    /*----------------------------------------------------------------Delete User--------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> Delete(long id)
    {
        User user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.IsDeleted = true;
        return await _userRepository.UpdateAsync(user);
    }
    #endregion

}
