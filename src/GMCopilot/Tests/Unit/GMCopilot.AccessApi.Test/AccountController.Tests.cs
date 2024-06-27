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
using FluentAssertions;

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
                new(ClaimTypes.NameIdentifier, _testUserId.ToString()),
                new(ClaimTypes.Name, _testUserName)
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
        }

        // InitializeAccount tests
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
            Assert.IsNotNull(result.Result);
            var statusCodeResult = result.Result as StatusCodeResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult?.StatusCode);
        }

        [TestMethod]
        public async Task InitializeAccount_AppRequest_ReturnsUnauthorized()
        {
            // Arrange
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(true);

            // Act
            var result = await _controller.InitializeAccount();

            // Assert
            Assert.IsNotNull(result.Result);
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("Application request not permitted.", unauthorizedResult.Value);
        }

        // CreateAccount tests
        [TestMethod]
        public async Task CreateAccount_WithValidAccount_ReturnsCreatedAtRouteResult()
        {
            // Arrange
            var newAccount = new Account(Guid.NewGuid(), "newUser", new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), newAccount.Id)).Returns(true);

            // Act
            var result = await _controller.CreateAccount(newAccount);

            // Assert
            var createdAtRouteResult = result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRouteResult);
            Assert.AreEqual("GetAccountById", createdAtRouteResult.RouteName);
            Assert.AreEqual(newAccount.Id, createdAtRouteResult.RouteValues["id"]);
            _accountRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Account>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateAccount_WithNullAccount_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CreateAccount(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task CreateAccount_WhenUserCannotAccess_ReturnsUnauthorized()
        {
            // Arrange
            var newAccount = new Account(Guid.NewGuid(), "newUser", new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), newAccount.Id)).Returns(false);

            // Act
            var result = await _controller.CreateAccount(newAccount);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public async Task CreateAccount_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var newAccount = new Account(Guid.NewGuid(), "newUser", new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), newAccount.Id)).Returns(true);
            _accountRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Account>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.CreateAccount(newAccount);

            // Assert
            var statusCodeResult = result as StatusCodeResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        // GetAccountById tests
        [TestMethod]
        public async Task GetAccountById_UserHasAccess_ReturnsOkWithAccount()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new Account(accountId, "TestUser", new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), accountId))
                .Returns(true);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(accountId, It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(account));

            // Act
            var result = await _controller.GetAccountById(accountId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(account);
        }

        [TestMethod]
        public async Task GetAccountById_UserHasNoAccess_ReturnsUnauthorized()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), accountId))
                .Returns(false);

            // Act
            var result = await _controller.GetAccountById(accountId);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [TestMethod]
        public async Task GetAccountById_AccountNotFound_ReturnsNotFound()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), accountId))
                .Returns(true);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(accountId, It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(null));

            // Act
            var result = await _controller.GetAccountById(accountId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task GetAccountById_ApplicationHasAccess_ReturnsOkWithAccount()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new Account(accountId, "TestUser", new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), accountId))
                .Returns(true);
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>()))
                .Returns(true);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(accountId, It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(account));

            // Act
            var result = await _controller.GetAccountById(accountId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(account);
        }

        // UpdateAccountById tests
        [TestMethod]
        public async Task UpdateAccountById_WithValidAccount_ReturnsOk()
        {
            // Arrange
            var account = new Account(_testUserId, _testUserName, new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), _testUserId)).Returns(true);
            _accountRepositoryMock.Setup(x => x.UpsertAsync(account)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateAccountById(_testUserId, account);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [TestMethod]
        public async Task UpdateAccountById_WithNullAccount_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UpdateAccountById(_testUserId, null);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task UpdateAccountById_WhenUserCannotAccess_ReturnsUnauthorized()
        {
            // Arrange
            var account = new Account(_testUserId, _testUserName, new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), _testUserId)).Returns(false);

            // Act
            var result = await _controller.UpdateAccountById(_testUserId, account);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [TestMethod]
        public async Task UpdateAccountById_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var account = new Account(_testUserId, _testUserName, new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), _testUserId)).Returns(true);
            _accountRepositoryMock.Setup(x => x.UpsertAsync(account)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.UpdateAccountById(_testUserId, account);

            // Assert
            var statusCodeResult = result as StatusCodeResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        // DeleteAccountById tests
        [TestMethod]
        public async Task DeleteAccountById_UserHasAccess_ReturnsOk()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new Account(accountId, "TestUser", new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), accountId))
                .Returns(true);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(accountId, It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(account));
            _accountRepositoryMock.Setup(x => x.DeleteAsync(account))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAccountById(accountId);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [TestMethod]
        public async Task DeleteAccountById_UserHasNoAccess_ReturnsUnauthorized()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), accountId))
                .Returns(false);

            // Act
            var result = await _controller.DeleteAccountById(accountId);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [TestMethod]
        public async Task DeleteAccountById_AccountNotFound_ReturnsNotFound()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), accountId))
                .Returns(true);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(accountId, It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(null));

            // Act
            var result = await _controller.DeleteAccountById(accountId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task DeleteAccountById_ApplicationHasAccess_ReturnsOk()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new Account(accountId, "TestUser", new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), accountId))
                .Returns(true);
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>()))
                .Returns(true);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(accountId, It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(account));
            _accountRepositoryMock.Setup(x => x.DeleteAsync(account))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAccountById(accountId);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [TestMethod]
        public async Task DeleteAccountById_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new Account(accountId, "TestUser", new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.RequestCanAccessUser(It.IsAny<HttpContext>(), accountId))
                .Returns(true);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(accountId, It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(account));
            _accountRepositoryMock.Setup(x => x.DeleteAsync(account))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.DeleteAccountById(accountId);

            // Assert
            var statusCodeResult = result as StatusCodeResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }
    }
}