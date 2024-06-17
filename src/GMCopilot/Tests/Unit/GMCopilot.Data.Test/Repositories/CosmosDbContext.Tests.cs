using Azure;
using FluentAssertions;
using GMCopilot.Core.Models;
using GMCopilot.Data.Repositories;
using Microsoft.Azure.Cosmos;
using Moq;
using System.Net;

[TestClass]
public class CosmosDbContextTests
{
    private Mock<Container> _mockContainer;
    private CosmosDbContext<TestEntity> _cosmosDbContext;

    [TestInitialize]
    public void Initialize()
    {
        var mockClient = new Mock<CosmosClient>();
        _mockContainer = new Mock<Container>();
        mockClient.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(_mockContainer.Object);

        _cosmosDbContext = new CosmosDbContext<TestEntity>(mockClient.Object, "databaseName", "containerName");
    }

    [TestMethod]
    public async Task CreateAsync_ShouldInvokeCreateItemAsync()
    {
        // Arrange
        var testEntity = new TestEntity { Id = Guid.NewGuid() };

        // Act
        await _cosmosDbContext.CreateAsync(testEntity);

        // Assert
        _mockContainer.Verify(c => c.CreateItemAsync(It.Is<TestEntity>(entity => entity.Id == testEntity.Id), null, null, default), Times.Once);
    }

    [TestMethod]
    public async Task ReadAsync_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var expectedEntity = new TestEntity { Id = entityId };
        _mockContainer.Setup(c => c.ReadItemAsync<TestEntity>(entityId.ToString(), It.IsAny<PartitionKey>(), null, default))
            .ReturnsAsync(new ItemResponseFake<TestEntity>(expectedEntity));

        // Act
        var result = await _cosmosDbContext.ReadAsync(entityId);

        // Assert
        result.Should().BeEquivalentTo(expectedEntity);
    }

    [TestMethod]
    public async Task ReadAsync_ShouldThrowKeyNotFoundException_WhenEntityDoesNotExist()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        _mockContainer.Setup(c => c.ReadItemAsync<TestEntity>(entityId.ToString(), It.IsAny<PartitionKey>(), null, default))
            .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, "SomeActivityId", 0));

        // Act
        Func<Task> act = async () => await _cosmosDbContext.ReadAsync(entityId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldInvokeDeleteItemAsync()
    {
        // Arrange
        var testEntity = new TestEntity { Id = Guid.NewGuid() };

        // Act
        await _cosmosDbContext.DeleteAsync(testEntity);

        // Assert
        _mockContainer.Verify(c => c.DeleteItemAsync<TestEntity>(testEntity.Id.ToString(), It.IsAny<PartitionKey>(), null, default), Times.Once);
    }

    // Define a test entity that implements IStorageEntity for testing purposes
    private class TestEntity : IStorageEntity
    {
        public Guid Id { get; set; }
    }

    // Define a fake that implements ItemResponse<T> for testing purposes
    public class ItemResponseFake<T> : ItemResponse<T>
    {
        private readonly T _item;

        public ItemResponseFake(T item)
        {
            _item = item;
        }

        public override T Resource => _item;
    }
}
