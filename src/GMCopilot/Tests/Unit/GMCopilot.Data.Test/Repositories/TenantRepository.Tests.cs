using FluentAssertions;
using GMCopilot.Core.Models;
using GMCopilot.Data.Repositories;
using Moq;

namespace GMCopilot.Tests.Repositories
{
    [TestClass]
    public class TenantRepositoryTests
    {
        private readonly Mock<IStorageContext<Tenant>> _mockStorageContext;
        private readonly TenantRepository _TenantRepository;

        public TenantRepositoryTests()
        {
            _mockStorageContext = new Mock<IStorageContext<Tenant>>();
            _TenantRepository = new TenantRepository(_mockStorageContext.Object);
        }

        [TestMethod]
        public async Task CreateAsync_ShouldInvokeCreateAsyncOnStorageContext()
        {
            // Arrange
            Tenant Tenant = GenerateTestTenant();

            // Act
            await _TenantRepository.CreateAsync(Tenant);

            // Assert
            _mockStorageContext.Verify(x => x.CreateAsync(Tenant), Times.Once);
        }

        [TestMethod]
        public async Task FindByIdAsync_ShouldReturnTenant_WhenTenantExists()
        {
            // Arrange
            var TenantId = Guid.NewGuid();
            var expectedTenant = GenerateTestTenant(TenantId);
            _mockStorageContext.Setup(x => x.ReadAsync(TenantId)).ReturnsAsync(expectedTenant);

            // Act
            var result = await _TenantRepository.FindByIdAsync(TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTenant);
        }

        [TestMethod]
        public async Task TryFindByIdAsync_ShouldReturnFalse_WhenTenantDoesNotExist()
        {
            // Arrange
            var TenantId = Guid.NewGuid();
            _mockStorageContext.Setup(x => x.ReadAsync(TenantId)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _TenantRepository.TryFindByIdAsync(TenantId, _ => { });

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task TryFindByIdAsync_ShouldReturnTrue_WhenTenantExists()
        {
            // Arrange
            var TenantId = Guid.NewGuid();
            var expectedTenant = GenerateTestTenant(TenantId);
            _mockStorageContext.Setup(x => x.ReadAsync(TenantId)).ReturnsAsync(expectedTenant);

            // Act
            var result = await _TenantRepository.TryFindByIdAsync(TenantId, tenant =>
            {
                tenant.Should().NotBeNull();
                tenant.Should().BeEquivalentTo(expectedTenant);
            });

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldInvokeDeleteAsyncOnStorageContext()
        {
            // Arrange
            var Tenant = GenerateTestTenant();

            // Act
            await _TenantRepository.DeleteAsync(Tenant);

            // Assert
            _mockStorageContext.Verify(x => x.DeleteAsync(Tenant), Times.Once);
        }

        [TestMethod]
        public async Task UpsertAsync_ShouldInvokeUpsertAsyncOnStorageContext()
        {
            // Arrange
            var Tenant = GenerateTestTenant();

            // Act
            await _TenantRepository.UpsertAsync(Tenant);

            // Assert
            _mockStorageContext.Verify(x => x.UpsertAsync(Tenant), Times.Once);
        }

        // Helper methods
        private static Tenant GenerateTestTenant(Guid tenantId)
        {
            // Assuming default values for the test tenant
            string name = "Test Tenant";
            Guid ownerAccount = Guid.NewGuid(); // Simulate an owner account with a new Guid
            TenantType type = TenantType.Individual; // Use Individual as the default type for simplicity

            return new Tenant(tenantId, name, ownerAccount, type);
        }

        private static Tenant GenerateTestTenant()
        {
            return GenerateTestTenant(Guid.NewGuid());
        }

    }
}
