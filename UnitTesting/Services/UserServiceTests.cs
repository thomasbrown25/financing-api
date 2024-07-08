using financing_api.Data;
using financing_api.DataAccess.UserDA;
using financing_api.Dtos.User;
using financing_api.Services.UserService;
using Moq;
using NUnit.Framework;

namespace financing_api.UnitTesting.Services
{
    public class UserServiceTests
    {
        private Mock<IUserService> _userService;
        private Mock<IUserDataAccess> _userDataAccess;

        [SetUp]
        public void Setup()
        {
            _userService = new Mock<IUserService>();
            _userDataAccess = new Mock<IUserDataAccess>();
        }

        [Test]
        public void RegisterUser_Test()
        {
            // Arrange
            RegisterUserDto user = new() { Email = "admin@gmail.com", Firstname = "admin", Lastname = "lastname", Password = "mypassword" };

            _userDataAccess.Setup(x => x.UserExists(user.Email)).Returns(Task.FromResult(true));

            // Act
            //var service = new UserService(_userDataAccess.Object);


            // Assert
            Assert.Pass();
        }
    }
}