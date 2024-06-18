using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GMCopilot.AccessApi.Controllers;
using GMCopilot.Core.Models;
using GMCopilot.Data.Repositories;
using GMCopilot.ApiCore.Services;
using System.Security.Claims;

namespace Tests.Unit.GMCopilot.AccessApi.Test
{
    [TestClass]
    public class AccountControllerTests
    {
        private Mock<ILogger<AccountController>> _loggerMock;
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<ITenantRepository> _tenantRepositoryMock;
        private Mock<IAuthorizationService> _authorizationServiceMock;
        private AccountController _controller;
        private Guid _testUserId = new Guid();
        private string _testUserName = "testUserName";

        [TestInitialize]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<AccountController>>();
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _tenantRepositoryMock = new Mock<ITenantRepository>();
            _authorizationServiceMock = new Mock<IAuthorizationService>();

            _controller = new AccountController(
                _loggerMock.Object,
                _accountRepositoryMock.Object,
                _tenantRepositoryMock.Object,
                _authorizationServiceMock.Object);

            // Mock HttpContext to simulate user claims
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString()),
                new Claim(ClaimTypes.Name, _testUserName)
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
        }

        [TestMethod]
        public async Task InitializeAccount_AccountAndTenantExistsSuccessfulCreation_ReturnsOk()
        {
            // Arrange
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(_testUserId);
            _authorizationServiceMock.Setup(x => x.GetUserName(It.IsAny<HttpContext>())).Returns(_testUserName);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(It.IsAny<Guid>(), It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(null)); // Simulate account not found
            _tenantRepositoryMock.Setup(x => x.TryFindByIdAsync(It.IsAny<Guid>(), It.IsAny<Action<Tenant?>>()))
                .Callback<Guid, Action<Tenant?>>((id, callback) => callback(null)); // Simulate tenant not found

            // Act
            var result = await _controller.InitializeAccount();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            _accountRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Account>()), Times.Once);
            _tenantRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Tenant>()), Times.Once);
        }

        [TestMethod]
        public async Task InitializeAccount_TenantExistsAccountMissingSuccessfulCreation_ReturnsOk()
        {
            // Arrange
            var existingTenant = new Tenant(new Guid(), "testTenant", _testUserId, TenantType.Individual);
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(_testUserId);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(It.IsAny<Guid>(), It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(null)); // Simulate account not found
            _tenantRepositoryMock.Setup(x => x.TryFindByIdAsync(It.IsAny<Guid>(), It.IsAny<Action<Tenant?>>()))
                .Callback<Guid, Action<Tenant?>>((id, callback) => callback(existingTenant)); // Simulate tenant found

            // Act
            var result = await _controller.InitializeAccount();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            _accountRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Account>()), Times.Once);
            _tenantRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Tenant>()), Times.Never);
        }


        [TestMethod]
        public async Task InitializeAccount_AccountExists_ReturnsOk()
        {
            // Arrange
            var existingAccount = new Account(_testUserId, _testUserName, new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(_testUserId);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(It.IsAny<Guid>(), It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(existingAccount)); // Simulate account found

            // Act
            var result = await _controller.InitializeAccount();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            _accountRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Account>()), Times.Never);
        }

        [TestMethod]
        public async Task InitializeAccount_ErrorOccurs_ReturnsInternalServerError()
        {
            // Arrange
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Throws(new Exception("Test exception"));

            // Act
            var result = await _controller.InitializeAccount();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(StatusCodeResult));
            var statusCodeResult = result.Result as StatusCodeResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }
    }
}