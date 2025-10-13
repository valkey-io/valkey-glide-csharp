// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;

using Valkey.Glide.Internals;

using Xunit;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Mock client class for testing ClientRegistry without requiring actual server connections.
/// </summary>
internal class MockClient : BaseClient
{
    protected override Task InitializeServerVersionAsync()
    {
        return Task.CompletedTask;
    }
}

[Collection("ClientRegistry")]
public class ClientRegistryTests : IDisposable
{
    public ClientRegistryTests()
    {
        // Clear the registry before each test
        ClientRegistry.Clear();
    }

    public void Dispose()
    {
        // Clear the registry after each test
        ClientRegistry.Clear();
    }

    [Fact]
    public void RegisterClient_ValidClient_RegistersSuccessfully()
    {
        // Arrange
        var client = new MockClient();
        ulong clientPtr = 12345;

        // Act
        ClientRegistry.RegisterClient(clientPtr, client);

        // Assert
        var retrievedClient = ClientRegistry.GetClient(clientPtr);
        Assert.Same(client, retrievedClient);
        Assert.Equal(1, ClientRegistry.Count);
    }

    [Fact]
    public void RegisterClient_NullClient_ThrowsArgumentNullException()
    {
        // Arrange
        ulong clientPtr = 12345;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ClientRegistry.RegisterClient(clientPtr, null!));
    }

    [Fact]
    public void GetClient_ExistingClient_ReturnsClient()
    {
        // Arrange
        var client = new MockClient();
        ulong clientPtr = 12345;
        ClientRegistry.RegisterClient(clientPtr, client);

        // Act
        var retrievedClient = ClientRegistry.GetClient(clientPtr);

        // Assert
        Assert.Same(client, retrievedClient);
    }

    [Fact]
    public void GetClient_NonExistentClient_ReturnsNull()
    {
        // Arrange
        ulong clientPtr = 99999;

        // Act
        var retrievedClient = ClientRegistry.GetClient(clientPtr);

        // Assert
        Assert.Null(retrievedClient);
    }

    [Fact]
    public void GetClient_GarbageCollectedClient_ReturnsNullAndCleansUp()
    {
        // Arrange
        ulong clientPtr = 12345;
        RegisterClientAndForceGC(clientPtr);

        // Act
        var retrievedClient = ClientRegistry.GetClient(clientPtr);

        // Assert
        Assert.Null(retrievedClient);
        // Note: The registry may or may not have cleaned up the dead reference automatically
        // depending on GC timing, but GetClient should return null for dead references
    }

    [Fact]
    public void UnregisterClient_ExistingClient_RemovesClient()
    {
        // Arrange
        var client = new MockClient();
        ulong clientPtr = 12345;
        int initialCount = ClientRegistry.Count;
        ClientRegistry.RegisterClient(clientPtr, client);

        // Act
        bool removed = ClientRegistry.UnregisterClient(clientPtr);

        // Assert
        Assert.True(removed);
        Assert.Null(ClientRegistry.GetClient(clientPtr));
        Assert.Equal(initialCount, ClientRegistry.Count);
    }

    [Fact]
    public void UnregisterClient_NonExistentClient_ReturnsFalse()
    {
        // Arrange
        ulong clientPtr = 99999;

        // Act
        bool removed = ClientRegistry.UnregisterClient(clientPtr);

        // Assert
        Assert.False(removed);
    }

    [Fact]
    public void CleanupDeadReferences_RemovesGarbageCollectedClients()
    {
        // Arrange
        ulong clientPtr1 = 12345;
        ulong clientPtr2 = 67890;

        var client2 = new MockClient();

        RegisterClientAndForceGC(clientPtr1);
        ClientRegistry.RegisterClient(clientPtr2, client2);

        int initialCount = ClientRegistry.Count;
        Assert.True(initialCount >= 1); // At least client2 should be registered

        // Act
        ClientRegistry.CleanupDeadReferences();

        // Assert
        Assert.True(ClientRegistry.Count <= initialCount); // Count should not increase
        Assert.Null(ClientRegistry.GetClient(clientPtr1)); // Dead client should return null
        Assert.Same(client2, ClientRegistry.GetClient(clientPtr2)); // Live client should still be accessible
    }

    [Fact]
    public void Clear_RemovesAllClients()
    {
        // Arrange - Clear any existing clients from other tests
        ClientRegistry.Clear();

        var client1 = new MockClient();
        var client2 = new MockClient();

        ClientRegistry.RegisterClient(12345, client1);
        ClientRegistry.RegisterClient(67890, client2);

        Assert.Equal(2, ClientRegistry.Count);

        // Act
        ClientRegistry.Clear();

        // Assert
        Assert.Equal(0, ClientRegistry.Count);
        Assert.Null(ClientRegistry.GetClient(12345));
        Assert.Null(ClientRegistry.GetClient(67890));
    }

    [Fact]
    public void ConcurrentAccess_ThreadSafe()
    {
        // Arrange
        const int numThreads = 10;
        const int operationsPerThread = 100;
        var tasks = new Task[numThreads];

        // Act
        for (int i = 0; i < numThreads; i++)
        {
            int threadId = i;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < operationsPerThread; j++)
                {
                    ulong clientPtr = (ulong)(threadId * operationsPerThread + j);
                    var client = new MockClient();

                    // Register
                    ClientRegistry.RegisterClient(clientPtr, client);

                    // Get
                    var retrievedClient = ClientRegistry.GetClient(clientPtr);
                    Assert.Same(client, retrievedClient);

                    // Unregister
                    bool removed = ClientRegistry.UnregisterClient(clientPtr);
                    Assert.True(removed);
                }
            });
        }

        // Assert
        Task.WaitAll(tasks);
        Assert.Equal(0, ClientRegistry.Count);
    }

    private static void RegisterClientAndForceGC(ulong clientPtr)
    {
        // Create client in a separate method to ensure it goes out of scope
        CreateAndRegisterClient(clientPtr);

        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    private static void CreateAndRegisterClient(ulong clientPtr)
    {
        var client = new MockClient();
        ClientRegistry.RegisterClient(clientPtr, client);
    }
}
