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

        // GetAccount tests
        [TestMethod]
        public async Task GetAccount_AccountExists_ReturnsOkWithAccount()
        {
            // Arrange
            var userId = _testUserId; // Assuming _testUserId is the user ID you want to test with
            var expectedAccount = new Account(userId, _testUserName, new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(userId);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(userId, It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(expectedAccount));

            // Act
            var result = await _controller.GetAccount();

            // Assert
            Assert.IsNotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var accountResult = okResult.Value as Account;
            Assert.IsNotNull(accountResult);
            Assert.AreEqual(userId, accountResult.Id);
        }

        [TestMethod]
        public async Task GetAccount_AccountDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var userId = _testUserId;
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(userId);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(userId, It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(null)); // Simulate account not found

            // Act
            var result = await _controller.GetAccount();

            // Assert
            Assert.IsNotNull(result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual($"Account with ID {userId} not found.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetAccount_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Throws(new Exception("Test exception"));

            // Act
            var result = await _controller.GetAccount();

            // Assert
            Assert.IsNotNull(result);
            var statusCodeResult = result.Result as StatusCodeResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [TestMethod]
        public async Task GetAccount_AppRequest_ReturnsUnauthorized()
        {
            // Arrange
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(true);

            // Act
            var result = await _controller.GetAccount();

            // Assert
            Assert.IsNotNull(result);
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("Application request not permitted.", unauthorizedResult.Value);
        }

        // CreateAccount tests
        [TestMethod]
        public async Task CreateAccount_SuccessfulCreation_ReturnsOk()
        {
            // Arrange
            var newAccount = new Account(_testUserId, _testUserName, new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(_testUserId);
            _accountRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Account>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateAccount(newAccount);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateAccount_NullAccount_ReturnsBadRequest()
        {
            // Arrange
            Account nullAccount = null;

            // Act
            var result = await _controller.CreateAccount(nullAccount);

            // Assert
            Assert.IsNotNull(result);
            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestObjectResult);
            Assert.AreEqual(400, badRequestObjectResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateAccount_UnauthorizedCreationAttempt_ReturnsUnauthorized()
        {
            // Arrange
            var unauthorizedAccount = new Account(Guid.NewGuid(), "UnauthorizedUser", new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(_testUserId);

            // Act
            var result = await _controller.CreateAccount(unauthorizedAccount);

            // Assert
            Assert.IsNotNull(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("Creating an account for another user not permitted.", unauthorizedResult.Value);
        }

        [TestMethod]
        public async Task CreateAccount_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var newAccount = new Account(_testUserId, _testUserName, new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Throws(new Exception("Test exception"));

            // Act
            var result = await _controller.CreateAccount(newAccount);

            // Assert
            Assert.IsNotNull(result);
            var statusCodeResult = result as StatusCodeResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult?.StatusCode);
        }

        [TestMethod]
        public async Task CreateAccount_AppRequest_ReturnsUnauthorized()
        {
            // Arrange
            var newAccount = new Account(_testUserId, _testUserName, new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(true);

            // Act
            var result = await _controller.CreateAccount(newAccount);

            // Assert
            Assert.IsNotNull(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("Application request not permitted.", unauthorizedResult.Value);
        }

        // UpdateAccount tests
        [TestMethod]
        public async Task UpdateAccount_SuccessfulUpdate_ReturnsOk()
        {
            // Arrange
            var updatedAccount = new Account(_testUserId, _testUserName, new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(_testUserId);
            _accountRepositoryMock.Setup(x => x.UpsertAsync(It.IsAny<Account>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateAccount(updatedAccount);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateAccount_NullAccount_ReturnsBadRequest()
        {
            // Arrange
            Account nullAccount = null;

            // Act
            var result = await _controller.UpdateAccount(nullAccount);

            // Assert
            Assert.IsNotNull(result);
            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestObjectResult);
            Assert.AreEqual(400, badRequestObjectResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateAccount_UnauthorizedUpdateAttempt_ReturnsUnauthorized()
        {
            // Arrange
            var unauthorizedAccount = new Account(Guid.NewGuid(), "UnauthorizedUser", new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(_testUserId);

            // Act
            var result = await _controller.UpdateAccount(unauthorizedAccount);

            // Assert
            Assert.IsNotNull(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("Updating the account of another user not permitted.", unauthorizedResult.Value);
        }

        [TestMethod]
        public async Task UpdateAccount_AppRequest_ReturnsUnauthorized()
        {
            // Arrange
            var updatedAccount = new Account(_testUserId, _testUserName, new List<AccountTenantRole>());
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(true);

            // Act
            var result = await _controller.UpdateAccount(updatedAccount);

            // Assert
            Assert.IsNotNull(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("Application request not permitted.", unauthorizedResult.Value);
        }

        // DeleteAccount tests
        [TestMethod]
        public async Task DeleteAccount_SuccessfulDeletion_ReturnsOk()
        {
            // Arrange
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(_testUserId);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(_testUserId, It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(new Account(_testUserId, _testUserName, new List<AccountTenantRole>())));
            _accountRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Account>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAccount();

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteAccount_AccountDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var userId = _testUserId;
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(false);
            _authorizationServiceMock.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(userId);
            _accountRepositoryMock.Setup(x => x.TryFindByIdAsync(userId, It.IsAny<Action<Account?>>()))
                .Callback<Guid, Action<Account?>>((id, callback) => callback(null)); // Simulate account not found

            // Act
            var result = await _controller.DeleteAccount();

            // Assert
            Assert.IsNotNull(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual($"Account with ID {userId} not found.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task DeleteAccount_AppRequest_ReturnsUnauthorized()
        {
            // Arrange
            _authorizationServiceMock.Setup(x => x.IsAppMakingRequest(It.IsAny<HttpContext>())).Returns(true);

            // Act
            var result = await _controller.DeleteAccount();

            // Assert
            Assert.IsNotNull(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("Application request not permitted.", unauthorizedResult.Value);
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
    }
}