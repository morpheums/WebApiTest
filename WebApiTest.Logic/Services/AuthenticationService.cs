using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Helpers;
using WebApiTest.Data.Definitions;
using WebApiTest.Data.Entities;
using WebApiTest.Logic.Definitions;
using WebApiTest.Logic.Exceptions;
using WebApiTest.Logic.Models.Authentication;
using WebApiTest.Logic.Services.Base;

namespace WebApiTest.Logic.Services
{
    public class AuthenticationService : ServiceBase, IAuthenticationService
    {
        public AuthenticationService(IGenericUoW genericUoW, IMapper mapper) : base(genericUoW, mapper)
        {
        }

        public async Task ChangePasswordAsync(ChangePasswordDto changePasswordInfo)
        {
            //Validating DTO
            Validator.ValidateObject(changePasswordInfo, new System.ComponentModel.DataAnnotations.ValidationContext(changePasswordInfo), true);

            //Validating if user exists
            var user = await _genericUoW.Repository<User>().FirstOrDefaultAsync(r => r.Username == changePasswordInfo.Username);

            if (user == null)
            {
                throw new NotFoundException("Invalid Username or Password!");
            }

            //Validating if old password is correct
            var saltedPasswordOld = changePasswordInfo.OldPassword + user.PasswordSalt;
            var passwordHashOld = Crypto.Hash(saltedPasswordOld);

            if (passwordHashOld != user.PasswordHash)
            {
                throw new NotFoundException("Invalid Username or Password!");
            }

            //Generating new password salt and hash
            var newPasswordSalt = Crypto.GenerateSalt(32);
            var newSaltedPassword = changePasswordInfo.NewPassword + newPasswordSalt;
            var newPasswordHash = Crypto.Hash(newSaltedPassword);

            user.PasswordSalt = newPasswordSalt;
            user.PasswordHash = newPasswordHash;
            await _genericUoW.SaveChangesAsync();
        }

        public async Task<bool> LoginAsync(LoginDto userCredentials)
        {
            //Validating DTO
            Validator.ValidateObject(userCredentials, new System.ComponentModel.DataAnnotations.ValidationContext(userCredentials), true);

            //Validating if user exists
            var user = await _genericUoW.Repository<User>().FirstOrDefaultAsync(r => r.Username == userCredentials.Username);

            if (user == null)
            {
                throw new NotFoundException("Invalid Username or Password!");
            }

            //Validating if password is correct
            var saltedPassword = userCredentials.Password + user.PasswordSalt;
            var passwordHash = Crypto.Hash(saltedPassword);

            if (passwordHash != user.PasswordHash)
            {
                throw new NotFoundException("Invalid Username or Password!");
            }

            return true;
        }

        public async Task RegisterAsync(RegisterUserDto userToRegister)
        {
            //Validating DTO
            Validator.ValidateObject(userToRegister, new System.ComponentModel.DataAnnotations.ValidationContext(userToRegister), true);

            //Validating user with this username does not exists already
            var userByUsername = await _genericUoW.Repository<User>().FirstOrDefaultAsync(r => r.Username == userToRegister.Username);
            if (userByUsername != null)
            {
                throw new DuplicateEntityException("Username already exists, please use a different one!");
            }

            //Validating user with this email does not exists already
            var userByEmail = await _genericUoW.Repository<User>().FirstOrDefaultAsync(r => r.Email == userToRegister.Email);
            if (userByEmail != null)
            {
                throw new DuplicateEntityException("Email already registered, please use a different one!");
            }

            //Generating pasword salt and hash
            var passwordSalt = Crypto.GenerateSalt(32);
            var saltedPassword = userToRegister.Password + passwordSalt;
            var hashedPassword = Crypto.Hash(saltedPassword);

            //Saving new user information
            var newUser = new User()
            {
                Username = userToRegister.Username,
                Email = userToRegister.Email,
                PasswordHash = hashedPassword,
                PasswordSalt = passwordSalt
            };

            _genericUoW.Repository<User>().Insert(newUser);
            await _genericUoW.SaveChangesAsync();
        }
    }
}
