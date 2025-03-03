
// using PizzaShop.Entity.Models;
// using PizzaShop.Repository.Interfaces;
// using PizzaShop.Service.Helpers;
// using PizzaShop.Service.Interfaces;
// using PizzaShop.Entity.ViewModels;
// using System.Linq.Expressions;
// using System.Threading.Tasks;

// namespace PizzaShop.Service.Services;

// public class UserService : IUserService
// {
//     private readonly IGenericRepository<User> _userRepository;
//     private readonly IGenericRepository<Role> _roleRepository;
//     private readonly IAddressService _addressService;
//     private readonly IEmailService _emailService;

//     public UserService(IGenericRepository<User> userRepository, IGenericRepository<Role> roleRepository, IAddressService addressService, IEmailService emailService)
//     {
//         _userRepository = userRepository;
//         _roleRepository = roleRepository;
//         _addressService = addressService;
//         _emailService = emailService;
//     }

//     /*------------------------------------------------------------Display User List
//     --------------------------------------------------------------*/
//     public  UsersListViewModel GetUsersListAsync(int pageNumber, int pageSize, string search)
//     {
//         var (users, totalRecords) =  _userRepository.GetPagedRecordsAsync(
//             pageSize,
//             pageNumber,
//             filter: u => !u.IsDeleted && 
//                          (string.IsNullOrEmpty(search) || 
//                           u.FirstName.Contains(search) || 
//                           u.LastName.Contains(search) || 
//                           u.Email.Contains(search)),
//             orderBy: q => q.OrderBy(u => u.Id),
//             includes: new List<Expression<Func<User, object>>> { u => u.Role }
//         );

//         var userViewModels = users.Select(u => new UserInfoViewModel
//         {
//             UserId = u.Id,
//             ProfileImageUrl = u.ProfileImg,
//             FirstName = u.FirstName,
//             LastName = u.LastName,
//             Email = u.Email,
//             Phone = u.Phone,
//             Role = u.Role.Name,
//             Status = u.IsActive
//         }).ToList();

//         return new UsersListViewModel
//         {
//             User = userViewModels,
//             Page = totalRecords
//         };
//     }

//     /*------------------------------------------------------------ Add User
//     --------------------------------------------------------------*/
//     public async Task<AddUserViewModel> GetAddUserAsync()
//     {
//         return new AddUserViewModel
//         {
//             Countries = _addressService.GetCountries(),
//             Roles = _roleRepository.GetAll().ToList()
//         };
//     }

//     public async Task<bool> AddUserAsync(AddUserViewModel model, string creatorEmail)
//     {
//         var creater = _userRepository.GetByStringAsync(u => u.Email == creatorEmail);

//         var password = model.Password;
//         model.Password = PasswordHelper.HashPassword(password);

//         await _userRepository.AddAsync(new User
//         {
//             FirstName = model.FirstName,
//             LastName = model.LastName,
//             Email = model.Email,
//             Password = model.Password,
//             CountryId = model.CountryId,
//             StateId = model.StateId,
//             CityId = model.CityId,
//             RoleId = model.RoleId,
//             CreatedBy = createrEmail.Id
//         });

//         string body = $@"
//             <div style='background-color: #F2F2F2;'>
//                 <div style='background-color: #0066A8; color: white; height: 90px; font-size: 40px; font-weight: 600; text-align: center; padding-top: 40px; margin-bottom: 0px;'>PIZZASHOP</div>
//                 <div style='font-family:Verdana, Geneva, Tahoma, sans-serif; margin-top: 0px; font-size: 20px; padding: 10px;'>
//                     <p>Pizza shop,</p>
//                     <h3>Your Password is : {password}</h3>
//                     <p>If you encounter any issues or have any question, please do not hesitate to contact our support team.</p>
//                 </div>
//             </div>";

//         await _emailService.SendEmailAsync(model.Email, "New User", body);

//         return true;
//     }

//     /*------------------------------------------------------------ Edit User
//     --------------------------------------------------------------*/
//     public async Task<EditUserViewModel?> GetUserByIdAsync(long id)
//     {
//         var user = await _userRepository.GetByIdAsync(id);
//         if (user == null) return null;

//         return new EditUserViewModel
//         {
//             Id = user.Id,
//             Name = user.Name,
//             Email = user.Email,
//             CountryId = user.CountryId,
//             StateId = user.StateId,
//             CityId = user.CityId,
//             RoleId = user.RoleId,
//             Roles = _roleRepository.GetAll().ToList(),
//             Countries = _addressService.GetCountries(),
//             States = _addressService.GetStates(user.CountryId),
//             Cities = _addressService.GetCities(user.StateId)
//         };
//     }

//     public async Task<bool> UpdateUserAsync(EditUserViewModel model)
//     {
//         var user = await _userRepository.GetByIdAsync(model.Id);
//         if (user == null) return false;

//         user.Name = model.Name;
//         user.Email = model.Email;
//         user.CountryId = model.CountryId;
//         user.StateId = model.StateId;
//         user.CityId = model.CityId;
//         user.RoleId = model.RoleId;

//         await _userRepository.UpdateAsync(user);
//         return true;
//     }

//     /*------------------------------------------------------------ Soft Delete User
//     --------------------------------------------------------------*/
//     public async Task<bool> SoftDeleteUserAsync(long id)
//     {
//         var user = await _userRepository.GetByIdAsync(id);
//         if (user == null) return false;

//         user.IsDeleted = true;
//         await _userRepository.UpdateAsync(user);
//         return true;
//     }

//     /*------------------------------------------------------------ Common Methods
//     --------------------------------------------------------------*/
//     public async Task<User?> GetUserByEmailAsync(string email)
//     {
//         return await _userRepository.GetByStringAsync(u => u.Email == email);
//     }

//     public List<Role> GetRoles()
//     {
//         return _roleRepository.GetAll().ToList();
//     }
// }
