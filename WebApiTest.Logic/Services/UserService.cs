using AutoMapper;
using WebApiTest.Data.Definitions;
using WebApiTest.Data.Entities;
using WebApiTest.Logic.Definitions;
using WebApiTest.Logic.Services.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;
using WebApiTest.Logic.Models.User;
using System.Threading.Tasks;
using WebApiTest.Logic.Exceptions;

namespace WebApiTest.Logic.Services
{
    public class UserService : ServiceBase, IUserService
    {
        public UserService(IGenericUoW genericUoW, IMapper mapper) : base(genericUoW, mapper)
        {
        }

        public async Task DeleteAsync(int id)
        {
            var userToDelete = await _genericUoW.Repository<User>().GetByIdAsync(id);

            if (userToDelete == null)
                throw new NotFoundException("User was not found!");

            _genericUoW.Repository<User>().Delete(userToDelete);
            await _genericUoW.SaveChangesAsync();
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _genericUoW.Repository<User>().GetByIdAsync(id);

            var mappedUser = _mapper.Map<UserDto>(user);
            return mappedUser;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var usersList = await _genericUoW.Repository<User>().GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(usersList);
        }

        public async Task<int> InsertAsync(InsertUserDto newUser)
        {
            Validator.ValidateObject(newUser, new System.ComponentModel.DataAnnotations.ValidationContext(newUser), true);

            //Validating user with this username does not exists already
            var userByUsername = await _genericUoW.Repository<User>().FirstOrDefaultAsync(r => r.Username == newUser.Username);
            if (userByUsername != null)
            {
                throw new DuplicateEntityException("Username already exists, please use a different one!");
            }

            //Validating user with this email does not exists already
            var userByEmail = await _genericUoW.Repository<User>().FirstOrDefaultAsync(r => r.Email == newUser.Email);
            if (userByEmail != null)
            {
                throw new DuplicateEntityException("Email already registered, please use a different one!");
            }

            //Generating pasword salt and hash
            var passwordSalt = Crypto.GenerateSalt(32);
            var saltedPassword = newUser.Password + passwordSalt;
            var hashedPassword = Crypto.HashPassword(saltedPassword);

            //Setting up new user
            var mappedUser = _mapper.Map<User>(newUser);
            mappedUser.PasswordHash = hashedPassword;
            mappedUser.PasswordSalt = passwordSalt;

            //Adding new user
            _genericUoW.Repository<User>().Insert(mappedUser);
            await _genericUoW.SaveChangesAsync();

            return mappedUser.Id;
        }

        public async Task UpdateAsync(UpdateUserDto user)
        {
            Validator.ValidateObject(user, new System.ComponentModel.DataAnnotations.ValidationContext(user), true);

            var userToUpdate = await _genericUoW.Repository<User>().GetByIdAsync(user.Id);
            if (userToUpdate == null)
            {
                throw new NotFoundException("User was not found!");
            }

            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.Phone = user.Phone;
           
            if (user.Address != null)
            {
                if (userToUpdate.Address == null)
                {
                    userToUpdate.Address = new Address();
                }
                userToUpdate.Address.UserId = user.Id;
                userToUpdate.Address.State = user.Address.State;
                userToUpdate.Address.City = user.Address.City;
                userToUpdate.Address.Street = user.Address.Street;
                userToUpdate.Address.ZipCode = user.Address.ZipCode;
            }
       
            await _genericUoW.SaveChangesAsync();
        }
    }
}
