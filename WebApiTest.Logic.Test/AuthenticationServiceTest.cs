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
using WebApiTest.Logic.Models.Authentication;
using WebApiTest.Logic.Services;

namespace WebApiTest.Logic.Tests
{
    [TestClass]
    public class AuthenticationServiceTest
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;
        private Mock<IGenericUoW> _genericUoW;
        private Mock<IGenericRepository<User>> _genericRepository;

        //Data Persistence Mock
        private List<User> usersList = new List<User>();
        private readonly RegisterUserDto mockedUser;

        public AuthenticationServiceTest()
        {
            SetupMocks();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<AutoMappperProfile>());
            _mapper = new Mapper(mapperConfig);
            _authenticationService = new AuthenticationService(_genericUoW.Object, _mapper);
            mockedUser = new RegisterUserDto()
            {
                Username = "TestUser",
                Email = "test@test.com",
                Password = "Pass123",
                PasswordConfirmation = "Pass123"
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

        #region Register Tests
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "A null User was inappropriately allowed.")]
        public async Task Should_FailToRegister_When_UserIsNull()
        {
            await _authenticationService.RegisterAsync(null);
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_UsernameIsEmpty()
        {
            try
            {
                mockedUser.Username = "";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with an empty Username was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Username is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_UsernameIsNull()
        {
            try
            {
                mockedUser.Username = null;
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with a null Username was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Username is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_UsernameHasWhiteSpaces()
        {
            try
            {
                mockedUser.Username = "test user";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with an Username with white spaces was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Username cannot have white spaces.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_UsernameDoesntMeetMinLength()
        {
            try
            {
                mockedUser.Username = "Te";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with an Username that does not meet the min length was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Username must be between 4 and 16 characters.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_UsernameDoesntMeetMaxLength()
        {
            try
            {
                mockedUser.Username = "Teeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeet";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with an Username that does not meet the max length was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Username must be between 4 and 16 characters.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_UsernameAlreadyExists()
        {
            try
            {
                await _authenticationService.RegisterAsync(mockedUser);
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An user with the same Username was inappropriately allowed.");
            }
            catch (DuplicateEntityException e)
            {
                Assert.AreEqual("Username already exists, please use a different one!", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_EmailIsEmpty()
        {
            try
            {
                mockedUser.Email = "";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with an empty Email was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Email is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_EmailIsNull()
        {
            try
            {
                mockedUser.Email = null;
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with a null Email was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Email is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_InvalidEmail()
        {
            try
            {
                mockedUser.Email = "Test Email";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An object with an invalid Email was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Provided Email is not a valid e-mail address.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_EmailDoesntMeetMinLength()
        {
            try
            {
                mockedUser.Email = "T@t";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with an Email that does not meet the min length was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Email must be between 7 and 50 characters", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_EmailDoesntMeetMaxLength()
        {
            try
            {
                mockedUser.Email = "Teeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeet@test.com";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with an Email that does not meet the max length was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Email must be between 7 and 50 characters", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_EmailAlreadyExists()
        {
            try
            {
                await _authenticationService.RegisterAsync(mockedUser);
                mockedUser.Username = "Admin";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An user with the same Email was inappropriately allowed.");
            }
            catch (DuplicateEntityException e)
            {
                Assert.AreEqual("Email already registered, please use a different one!", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_PasswordIsEmpty()
        {
            try
            {
                mockedUser.Password = "";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with an empty Password was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Password is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_PasswordIsNull()
        {
            try
            {
                mockedUser.Password = null;
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with a null Password was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Password is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_PasswordDoesntMeetMinLength()
        {
            try
            {
                mockedUser.Password = "Pas1";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with a Password that does not meet the min length was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Password must be at least 6 characters long.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_PasswordlDoesntHaveNumbers()
        {
            try
            {
                mockedUser.Password = "PasswordTest";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with a Password that does not have a number was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Password must contain at least one uppercase letter and one number.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_PasswordlDoesntHaveUppercase()
        {
            try
            {
                mockedUser.Password = "pass1234";
                await _authenticationService.RegisterAsync(mockedUser);
                Assert.Fail("An User with a Password that does not have an uppercase was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Password must contain at least one uppercase letter and one number.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToRegister_When_PasswordsDontMatch()
        {
            try
            {
                mockedUser.Password = "Pass123";
                mockedUser.PasswordConfirmation = "Pass12";

                await _authenticationService.RegisterAsync(mockedUser);
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
        public async Task Should_SucceedToRegister_When_IsNotNullAndHaveValidValues()
        {
            await _authenticationService.RegisterAsync(mockedUser);
            Assert.IsTrue(usersList.Any());
        }
        #endregion

        #region Login Tests
        [TestMethod]
        public async Task Should_SucceedToLogin_When_Exists()
        {
            await _authenticationService.RegisterAsync(mockedUser);

            var validLogin = await _authenticationService.LoginAsync(new LoginDto() { Username = mockedUser.Username, Password = mockedUser.Password });
            Assert.IsTrue(validLogin);
        }

        [TestMethod]
        public async Task Should_FailToLogin_When_NotExists()
        {
            try
            {
                await _authenticationService.LoginAsync(new LoginDto() { Username = mockedUser.Username, Password = mockedUser.Password });
                Assert.Fail("An user with invalid credentials was inappropriately allowed.");
            }
            catch (NotFoundException e)
            {
                Assert.AreEqual("Invalid Username or Password!", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToLogin_When_WrongPassword()
        {
            try
            {
                await _authenticationService.RegisterAsync(mockedUser);
                await _authenticationService.LoginAsync(new LoginDto() { Username = mockedUser.Username, Password = "TestWrongPassword123" });
            }
            catch (NotFoundException e)
            {
                Assert.AreEqual("Invalid Username or Password!", e.Message);
            }
        }
        #endregion

        #region ChangePassword Tests
        [TestMethod]
        public async Task Should_SucceedToChangePassword_When_ValidInfo()
        {
            var newPassword = "NewPass123";

            await _authenticationService.RegisterAsync(mockedUser);

            await _authenticationService.ChangePasswordAsync(
                new ChangePasswordDto()
                {
                    Username = mockedUser.Username,
                    OldPassword = mockedUser.Password,
                    NewPassword = newPassword,
                    PasswordConfirmation = newPassword
                });

            var validLogin = await _authenticationService.LoginAsync(
                new LoginDto()
                {
                    Username = mockedUser.Username,
                    Password = newPassword
                });

            Assert.IsTrue(validLogin);
        }

        [TestMethod]
        public async Task Should_FailToChangePassword_When_NotExists()
        {
            var newPassword = "NewPass123";

            try
            {
                await _authenticationService.ChangePasswordAsync(
                    new ChangePasswordDto()
                    {
                        Username = mockedUser.Username,
                        OldPassword = mockedUser.Password,
                        NewPassword = newPassword,
                        PasswordConfirmation = newPassword
                    });

                Assert.Fail("An user with invalid credentials was inappropriately allowed.");
            }
            catch (NotFoundException e)
            {
                Assert.AreEqual("Invalid Username or Password!", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToChangePassword_When_WrongOldPassword()
        {
            var newPassword = "NewPass123";

            try
            {
                await _authenticationService.RegisterAsync(mockedUser);

                await _authenticationService.ChangePasswordAsync(
                    new ChangePasswordDto()
                    {
                        Username = mockedUser.Username,
                        OldPassword = "wrongOldPassword12",
                        NewPassword = newPassword,
                        PasswordConfirmation = newPassword
                    });
            }
            catch (NotFoundException e)
            {
                Assert.AreEqual("Invalid Username or Password!", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToChangePassword_When_UsernameIsEmpty()
        {
            try
            {
                await _authenticationService.ChangePasswordAsync(new ChangePasswordDto()
                {
                    Username = "",
                    OldPassword = "Pass12",
                    NewPassword = "Pass123",
                    PasswordConfirmation = "Pass123"
                });
                Assert.Fail("An User with an empty Username was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Username is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToChangePassword_When_UsernameIsNull()
        {
            try
            {
                await _authenticationService.ChangePasswordAsync(new ChangePasswordDto()
                {
                    Username = null,
                    OldPassword = "Pass12",
                    NewPassword = "Pass123",
                    PasswordConfirmation = "Pass123"
                });
                Assert.Fail("An User with an empty Username was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Username is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToChangePassword_When_OldPasswordIsEmpty()
        {
            try
            {
                await _authenticationService.ChangePasswordAsync(new ChangePasswordDto()
                {
                    Username = "testUser",
                    OldPassword = "",
                    NewPassword = "Pass123",
                    PasswordConfirmation = "Pass123"
                });
                Assert.Fail("An User with an empty Password was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Old password is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToChangePassword_When_OldPasswordIsNull()
        {
            try
            {
                await _authenticationService.ChangePasswordAsync(new ChangePasswordDto()
                {
                    Username = "testUser",
                    OldPassword = null,
                    NewPassword = "Pass123",
                    PasswordConfirmation = "Pass123"
                });
                Assert.Fail("An User with an empty Password was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Old password is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToChangePassword_When_NewPasswordIsEmpty()
        {
            try
            {
                await _authenticationService.ChangePasswordAsync(new ChangePasswordDto()
                {
                    Username = "testUser",
                    OldPassword = "Pass12",
                    NewPassword = "",
                    PasswordConfirmation = "Pass123"
                });
                Assert.Fail("An User with an empty Password was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("New password is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToChangePassword_When_NewPasswordIsNull()
        {
            try
            {
                await _authenticationService.ChangePasswordAsync(new ChangePasswordDto()
                {
                    Username = "testUser",
                    OldPassword = "Pass12",
                    NewPassword = null,
                    PasswordConfirmation = "Pass123"
                });
                Assert.Fail("An User with an empty Password was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("New password is required.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToChangePassword_When_NewPasswordDoesntMeetMinLength()
        {
            try
            {
                await _authenticationService.ChangePasswordAsync(new ChangePasswordDto()
                {
                    Username = "testUser",
                    OldPassword = "Pass12",
                    NewPassword = "Pas1",
                    PasswordConfirmation = "Pas1"
                });
                Assert.Fail("An User with a Password that does not meet the min length was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("New password must be at least 6 characters long.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToChangePassword_When_NewPasswordlDoesntHaveNumbers()
        {
            try
            {
                await _authenticationService.ChangePasswordAsync(new ChangePasswordDto()
                {
                    Username = "testUser",
                    OldPassword = "Pass12",
                    NewPassword = "PasswordTest",
                    PasswordConfirmation = "PasswordTest"
                });
                Assert.Fail("An User with a Password that does not have a number was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("New password must contain at least one uppercase letter and one number.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToChangePassword_When_NewPasswordlDoesntHaveUppercase()
        {
            try
            {
                await _authenticationService.ChangePasswordAsync(new ChangePasswordDto()
                {
                    Username = "testUser",
                    OldPassword = "Pass12",
                    NewPassword = "password12",
                    PasswordConfirmation = "password12"
                });
                Assert.Fail("An User with a Password that does not have an uppercase was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("New password must contain at least one uppercase letter and one number.", e.Message);
            }
        }

        [TestMethod]
        public async Task Should_FailToChangePassword_When_PasswordsDontMatch()
        {
            try
            {
                await _authenticationService.ChangePasswordAsync(new ChangePasswordDto()
                {
                    Username = "testUser",
                    OldPassword = "Pass12",
                    NewPassword = "Pass1234",
                    PasswordConfirmation = "Pass12345"
                });
                Assert.Fail("An user with unmatched passwords was inappropriately allowed.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual("Passwords do not match.", e.Message);
            }
        }
        #endregion
    }
}
