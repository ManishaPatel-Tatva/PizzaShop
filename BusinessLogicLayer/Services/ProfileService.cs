using BusinessLogicLayer.Interfaces;
using DataAccessLayer.ViewModels;
using DataAccessLayer.Interfaces;


namespace BusinessLogicLayer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICountryRepository _countryRepository;

        public ProfileService(IUserRepository userRepository, ICountryRepository countryRepository)
        {
            _userRepository = userRepository;
            _countryRepository = countryRepository;
        }

/*-----------------------------------------------------------------My Profile---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region My Profile

        public async Task<MyProfileViewModel> GetMyProfileAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return null;

            var role = await _userRepository.GetUserRoleAsync(user.RoleId);

            MyProfileViewModel userProfile =  new MyProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role.Name,
                UserName = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                CountryId = user.CountryId,
                StateId = user.StateId,
                CityId = user.CityId,
                Address = user.Address,
                ZipCode = user.ZipCode,
                ProfileImageUrl = user.ProfileImg,
                Countries = _countryRepository.GetCountries(),
                States = _countryRepository.GetStates(user.CountryId),
                Cities = _countryRepository.GetCities(user.StateId)
            };

            return userProfile;
        }

#endregion

/*---------------------------------------------------------------Update Profile---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Update Profile

        public async Task<bool> UpdateProfileAsync(MyProfileViewModel model)
        {
            var user = await _userRepository.GetUserByEmailAsync(model.Email);
            
            if (user == null) 
                return false;

            //for updating the user, if there is any exception then return false
            try
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Username = model.UserName;
                user.Phone = model.Phone;
                user.CountryId = model.CountryId;
                user.StateId = model.StateId;
                user.CityId = model.CityId;
                user.Address = model.Address;
                user.ZipCode = model.ZipCode;

                 // Handle Image Upload
                if (model.image != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string fileName = $"{Guid.NewGuid()}_{model.image.FileName}";
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.image.CopyToAsync(stream);
                    }

                    user.ProfileImg = $"/uploads/{fileName}";
                }

                await _userRepository.UpdateAsync(user);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

#endregion

/*---------------------------------------------------------------Change Password---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Change Password

        public async Task<bool> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var user = await _userRepository.GetUserByEmailAsync(model.Email);

            if (user == null) 
                return false;

            bool verified = BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password);
            if (!verified)
                return false;

            //for updating the user, if there is any exception then return false
            try
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                await _userRepository.UpdateAsync(user);
                return true;
            }
            catch(Exception)
            {
               return false;
            }

             
        }
#endregion

    }
}
