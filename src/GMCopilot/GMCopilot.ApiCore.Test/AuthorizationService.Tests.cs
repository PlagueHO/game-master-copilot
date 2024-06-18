using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using GMCopilot.ApiCore.Services;
using System.Security.Claims;

namespace Tests.Unit.GMCopilot.ApiCore.Services
{
    [TestClass]
    public class AuthorizationServiceTests
    {
        private Mock<ILogger<AuthorizationService>> _loggerMock;
        private AuthorizationService _authorizationService;
        private Mock<HttpContext> _httpContextMock;
        private ClaimsPrincipal _user;

        [TestInitialize]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<AuthorizationService>>();
            _authorizationService = new AuthorizationService(_loggerMock.Object);
            _httpContextMock = new Mock<HttpContext>();
            _user = new ClaimsPrincipal();
            _httpContextMock.Setup(x => x.User).Returns(_user);
        }

        [TestMethod]
        public void GetUserId_ValidOidClaim_ReturnsUserId()
        {
            // Arrange
            var expectedUserId = Guid.NewGuid();
            _user.AddIdentity(new(new Claim[] { new("http://schemas.microsoft.com/identity/claims/objectidentifier", expectedUserId.ToString()) }));

            // Act
            var result = _authorizationService.GetUserId(_httpContextMock.Object);

            // Assert
            result.Should().Be(expectedUserId);
        }

        [TestMethod]
        public void GetUserId_NoOidClaim_ThrowsInvalidOperationException()
        {
            // Arrange
            // No OID claim added to the user

            // Act
            Action act = () => _authorizationService.GetUserId(_httpContextMock.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("No oid claim!");
        }

        [TestMethod]
        public void GetUserName_ValidNameClaim_ReturnsUserName()
        {
            // Arrange
            var expectedUserName = "TestUser";
            _user.AddIdentity(new(new Claim[] { new(ClaimTypes.Name, expectedUserName) }));

            // Act
            var result = _authorizationService.GetUserName(_httpContextMock.Object);

            // Assert
            result.Should().Be(expectedUserName);
        }

        [TestMethod]
        public void IsAppMakingRequest_WithAppClaim_ReturnsTrue()
        {
            // Arrange
            _user.AddIdentity(new(new Claim[] { new("idtyp", "app") }));

            // Act
            var result = _authorizationService.IsAppMakingRequest(_httpContextMock.Object);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void AppHasPermission_WithCorrectRoleClaim_ReturnsTrue()
        {
            // Arrange
            var permission = "expected.permission";
            _user.AddIdentity(new(new Claim[] { new("idtyp", "app"), new("roles", permission) }));

            // Act
            var result = _authorizationService.AppHasPermission(_httpContextMock.Object, permission);

            // Assert
            result.Should().BeTrue();
        }
    }
}