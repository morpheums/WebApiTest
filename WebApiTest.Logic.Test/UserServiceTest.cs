using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApiTest.Data.Definitions;
using WebApiTest.Data.Entities;
using WebApiTest.Logic.Configuration;
using WebApiTest.Logic.Definitions;
using WebApiTest.Logic.Exceptions;
using WebApiTest.Logic.Models.User;
using WebApiTest.Logic.Services;

namespace WebApiTest.Logic.Tests
{
    [TestClass]
    public class UserServiceTest
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private Mock<IGenericUoW> _genericUoW;
        private Mock<IGenericRepository<User>> _genericRepository;

        //Data Persistence Mock
        private List<User> usersList = new List<User>();
        private readonly InsertUserDto mockedUser;

        public UserServiceTest()
        {
            SetupMocks();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<AutoMappperProfile>());
            _mapper = new Mapper(mapperConfig);
            _userService = new UserService(_genericUoW.Object, _mapper);
            mockedUser = new InsertUserDto()
            {
                Username = "TestUser",
                Phone = "809-999-9999",
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "Test",
                Password = "Pass123",
                PasswordConfirmation = "Pass123",
                Address = new UserAddressDto()
                {
                    State = "Test State",
                    City = "Test City",
                    Street = "Test Street",
                    ZipCode = "Test Zipcode"
                }
            };
        }

        private void SetupMocks()
        {
            _genericRepository = new Mock<IGenericRepository<User>>();
            _genericRepository.Setup(svc => svc.Insert(It.IsAny<User>())).Callback((User user) =>
            {
                user.Id = usersList.Count() + 1;
                usersList.Add(user);
            });

            _genericRepository.Setup(svc => svc.Delete(It.IsAny<User>())).Callback((User user) =>
              {
                  usersList.Remove(user);
              });

            _genericRepository.Setup(svc => svc.GetByIdAsync(It.IsAny<int>())).Returns((int id) => Task<User>.Factory.StartNew(() => usersList.FirstOrDefault(r => r.Id == id)));
            _genericRepository.Setup(svc => svc.Count()).Returns(() => usersList.Count);
            _genericRepository.Setup(svc => svc.GetAllAsync()).Returns(() => Task<List<User>>.Factory.StartNew(() => usersList.ToList()));
            _genericRepository.Setup(svc => svc.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>())).Returns((Expression<Func<User, bool>> expression) => Task<User>.Factory.StartNew(() => usersList.AsQueryable().FirstOrDefault(expression)));

            _genericUoW = new Mock<IGenericUoW>();
            _genericUoW.Setup(service => service.Repository<User>()).Returns(() => _genericRepository.Object);
        }

        #region Insert Tests
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "A null User was inappropriately allowed.")]
        public async Task Should_FailToInsert_When_UserIsNull()
        {
            await _userService.InsertAsync(null);
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_UsernameIsEmpty()
        {
            try
            {
                mockedUser.Username = "";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with an empty Username was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Username is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_UsernameIsNull()
        {
            try
            {
                mockedUser.Username = null;
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with a null Username was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Username is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_UsernameHasWhiteSpaces()
        {
            try
            {
                mockedUser.Username = "test user";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with an Username with white spaces was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Username cannot have white spaces.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_UsernameDoesntMeetMinLength()
        {
            try
            {
                mockedUser.Username = "Te";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with an Username that does not meet the min length was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Username must be between 4 and 16 characters.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_UsernameDoesntMeetMaxLength()
        {
            try
            {
                mockedUser.Username = "Teeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeet";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with an Username that does not meet the max length was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Username must be between 4 and 16 characters.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_UsernameAlreadyExists()
        {
            try
            {
                await _userService.InsertAsync(mockedUser);
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An user with the same Username was inappropriately allowed.");
            }
            catch (DuplicateEntityException e)
            {
                Assert.AreEqual("Username already exists, please use a different one!", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_EmailIsEmpty()
        {
            try
            {
                mockedUser.Email = "";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with an empty Email was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Email is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_EmailIsNull()
        {
            try
            {
                mockedUser.Email = null;
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with a null Email was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Email is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_InvalidEmail()
        {
            try
            {
                mockedUser.Email = "Test Email";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An object with an invalid Email was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Provided Email is not a valid e-mail address.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_EmailDoesntMeetMinLength()
        {
            try
            {
                mockedUser.Email = "T@t";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with an Email that does not meet the min length was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Email must be between 7 and 50 characters", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_EmailDoesntMeetMaxLength()
        {
            try
            {
                mockedUser.Email = "Teeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeet@test.com";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with an Email that does not meet the max length was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Email must be between 7 and 50 characters", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_EmailAlreadyExists()
        {
            try
            {
                await _userService.InsertAsync(mockedUser);
                mockedUser.Username = "Admin";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An user with the same Email was inappropriately allowed.");
            }
            catch (DuplicateEntityException e)
            {
                Assert.AreEqual("Email already registered, please use a different one!", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_PasswordIsEmpty()
        {
            try
            {
                mockedUser.Password = "";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with an empty Password was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Password is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_PasswordIsNull()
        {
            try
            {
                mockedUser.Password = null;
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with a null Password was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Password is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_PasswordDoesntMeetMinLength()
        {
            try
            {
                mockedUser.Password = "Pas1";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with a Password that does not meet the min length was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Password must be at least 6 characters long.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_PasswordlDoesntHaveNumbers()
        {
            try
            {
                mockedUser.Password = "PasswordTest";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with a Password that does not have a number was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Password must contain at least one uppercase letter and one number.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_PasswordlDoesntHaveUppercase()
        {
            try
            {
                mockedUser.Password = "pass1234";
                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An User with a Password that does not have an uppercase was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Password must contain at least one uppercase letter and one number.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToInsert_When_PasswordsDontMatch()
        {
            try
            {
                mockedUser.Password = "Pass123";
                mockedUser.PasswordConfirmation = "Pass12";

                await _userService.InsertAsync(mockedUser);
                Assert.Fail("An user with unmatched passwords was inappropriately allowed.");
            }
            catch (ValidationException ae)
            {
                Assert.AreEqual("Passwords do not match.", ae.Message);
            }
            catch (Exception e)
            {
                Assert.Fail(
                     string.Format("Unexpected exception of type {0} caught: {1}", e.GetType(), e.Message)
                );
            }
        }

        [TestMethod]
        public async Task Should_SucceedToInsert_When_IsNotNullAndHaveValidValues()
        {
            await _userService.InsertAsync(mockedUser);
            Assert.IsTrue(usersList.Any());
        }
        #endregion

        #region Get Tests
        [TestMethod]
        public async Task Should_SucceedToGet_AllUsers()
        {
            await _userService.InsertAsync(mockedUser);
            var data = await _userService.GetAllAsync();
            Assert.AreEqual(usersList.Count(), data.Count());
        }

        [TestMethod]
        public async Task Should_SucceedToGet_UserById()
        {
            var userId = await _userService.InsertAsync(mockedUser);
            var user = await _userService.GetByIdAsync(userId);
            Assert.IsNotNull(user);
        }

        #endregion

        #region Delete Tests
        [TestMethod]
        public async Task Should_SucceedToDeleteUser_When_Exists()
        {
            var userId = await _userService.InsertAsync(mockedUser);
            await _userService.DeleteAsync(userId);
            Assert.AreEqual(usersList.Count(), 0);
        }

        [TestMethod]
        public async Task Should_FailToDelete_When_NotExists()
        {
            try
            {
                await _userService.DeleteAsync(123);
                Assert.Fail("An object with an invalid Id was inappropriately deleted.");
            }
            catch (NotFoundException e)
            {
                Assert.AreEqual("User was not found!", e.Message);
            }
        }
        #endregion

        #region Update Tests
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "A null User was inappropriately updated.")]
        public async Task Should_FailToUpdate_When_UserIsNull()
        {
            await _userService.UpdateAsync(null);
        }

        [TestMethod]
        public async Task Should_FailToUpdate_When_NotExists()
        {
            try
            {
                await _userService.UpdateAsync(new UpdateUserDto() { Id = 123 });
                Assert.Fail("An User with an empty Id was inappropriately updated.");
            }
            catch (NotFoundException e)
            {
                Assert.AreEqual("User was not found!", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_SucceedToUpdate_When_Exists()
        {
            var userId = await _userService.InsertAsync(mockedUser);
            var newFirstName = "Updated Name";

            await _userService.UpdateAsync(new UpdateUserDto() { Id = userId, FirstName = newFirstName });
            var user = await _userService.GetByIdAsync(userId);
            Assert.AreEqual(newFirstName, user.FirstName);
        }
        #endregion
    }
}
