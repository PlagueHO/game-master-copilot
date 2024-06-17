using FluentAssertions;
using GMCopilot.Core.Models;
using GMCopilot.Data.Repositories;
using Moq;

namespace GMCopilot.Tests.Repositories
{
    [TestClass]
    public class AccountRepositoryTests
    {
        private readonly Mock<IStorageContext<Account>> _mockStorageContext;
        private readonly AccountRepository _accountRepository;

        public AccountRepositoryTests()
        {
            _mockStorageContext = new Mock<IStorageContext<Account>>();
            _accountRepository = new AccountRepository(_mockStorageContext.Object);
        }

        [TestMethod]
        public async Task CreateAsync_ShouldInvokeCreateAsyncOnStorageContext()
        {
            // Arrange
            Account account = GenerateTestAccount();

            // Act
            await _accountRepository.CreateAsync(account);

            // Assert
            _mockStorageContext.Verify(x => x.CreateAsync(account), Times.Once);
        }

        [TestMethod]
        public async Task FindByIdAsync_ShouldReturnAccount_WhenAccountExists()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var expectedAccount = GenerateTestAccount(accountId);
            _mockStorageContext.Setup(x => x.ReadAsync(accountId)).ReturnsAsync(expectedAccount);

            // Act
            var result = await _accountRepository.FindByIdAsync(accountId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedAccount);
        }

        [TestMethod]
        public async Task FindByIdAsync_ShouldThrowException_WhenAccountDoesNotExist()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            _mockStorageContext.Setup(x => x.ReadAsync(accountId)).ThrowsAsync(new KeyNotFoundException());

            // Act
            Func<Task> act = async () => await _accountRepository.FindByIdAsync(accountId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [TestMethod]
        public async Task TryFindByIdAsync_ShouldReturnFalse_WhenAccountDoesNotExist()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            _mockStorageContext.Setup(x => x.ReadAsync(accountId)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _accountRepository.TryFindByIdAsync(accountId, _ => { });

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task TryFindByIdAsync_ShouldReturnTrue_WhenAccountExists()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var expectedAccount = GenerateTestAccount(accountId);
            _mockStorageContext.Setup(x => x.ReadAsync(accountId)).ReturnsAsync(expectedAccount);

            // Act
            var result = await _accountRepository.TryFindByIdAsync(accountId, _ => { });

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldInvokeDeleteAsyncOnStorageContext()
        {
            // Arrange
            var account = GenerateTestAccount();

            // Act
            await _accountRepository.DeleteAsync(account);

            // Assert
            _mockStorageContext.Verify(x => x.DeleteAsync(account), Times.Once);
        }

        [TestMethod]
        public async Task UpsertAsync_ShouldInvokeUpsertAsyncOnStorageContext()
        {
            // Arrange
            var account = GenerateTestAccount();

            // Act
            await _accountRepository.UpsertAsync(account);

            // Assert
            _mockStorageContext.Verify(x => x.UpsertAsync(account), Times.Once);
        }

        // Helper methods
        private static Account GenerateTestAccount(Guid accountId)
        {
            string userName = "testUser";
            List<AccountTenantRole> tenantRoles = new List<AccountTenantRole>
            {
                new AccountTenantRole(accountId, TenantType.Individual, TenantRole.Owner)
            };

            var account = new Account(accountId, userName, tenantRoles);
            return account;
        }

        private static Account GenerateTestAccount()
        {
            return GenerateTestAccount(Guid.NewGuid()); 
        }

    }
}
