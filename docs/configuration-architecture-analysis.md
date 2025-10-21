# Configuration Architecture Analysis

## Overview

This document analyzes the configuration architecture in the Valkey.Glide C# client, focusing on the relationship between `ConnectionConfiguration` and `ConfigurationOptions`, and how configuration changes can be made through the `ConnectionMultiplexer`.

## Configuration Classes Relationship

### ConfigurationOptions
- **Purpose**: External API configuration class that follows StackExchange.Redis compatibility patterns
- **Location**: `sources/Valkey.Glide/Abstract/ConfigurationOptions.cs`
- **Role**: User-facing configuration interface

### ConnectionConfiguration
- **Purpose**: Internal configuration classes that map to the underlying FFI layer
- **Location**: `sources/Valkey.Glide/ConnectionConfiguration.cs`
- **Role**: Internal configuration representation and builder pattern implementation

## Configuration Flow

```
ConfigurationOptions → ClientConfigurationBuilder → ConnectionConfig → FFI.ConnectionConfig
```

1. **User Input**: `ConfigurationOptions` (external API)
2. **Translation**: `ConnectionMultiplexer.CreateClientConfigBuilder<T>()` method
3. **Building**: `ClientConfigurationBuilder<T>` (internal)
4. **Internal Config**: `ConnectionConfig` record
5. **FFI Layer**: `FFI.ConnectionConfig`

## Key Components Analysis

### ConnectionMultiplexer Configuration Mapping

The `ConnectionMultiplexer.CreateClientConfigBuilder<T>()` method at line 174 performs the critical translation:

```csharp
internal static T CreateClientConfigBuilder<T>(ConfigurationOptions configuration)
    where T : ClientConfigurationBuilder<T>, new()
{
    T config = new();
    foreach (EndPoint ep in configuration.EndPoints)
    {
        config.Addresses += Utils.SplitEndpoint(ep);
    }
    config.UseTls = configuration.Ssl;
    // ... other mappings
    _ = configuration.ReadFrom.HasValue ? config.ReadFrom = configuration.ReadFrom.Value : new();
    return config;
}
```

### Configuration Builders

The builder pattern is implemented through:
- `StandaloneClientConfigurationBuilder` (line 525)
- `ClusterClientConfigurationBuilder` (line 550)

Both inherit from `ClientConfigurationBuilder<T>` which provides:
- Fluent API methods (`WithXxx()`)
- Property setters
- Internal `ConnectionConfig Build()` method

## Configuration Mutability Analysis

### Current State: Immutable After Connection

**Connection Creation**: Configuration is set once during `ConnectionMultiplexer.ConnectAsync()`:

```csharp
public static async Task<ConnectionMultiplexer> ConnectAsync(ConfigurationOptions configuration, TextWriter? log = null)
{
    // Configuration is translated and used to create the client
    StandaloneClientConfiguration standaloneConfig = CreateClientConfigBuilder<StandaloneClientConfigurationBuilder>(configuration).Build();
    // ... connection establishment
    return new(configuration, await Database.Create(config));
}
```

**Storage**: The original `ConfigurationOptions` is stored in `RawConfig` property (line 156):

```csharp
internal ConfigurationOptions RawConfig { private set; get; }
```

### Limitations for Runtime Configuration Changes

1. **No Reconfiguration API**: `ConnectionMultiplexer` doesn't expose methods to change configuration after connection
2. **Immutable Builder Chain**: Once built, the configuration flows to FFI layer and cannot be modified
3. **Connection Recreation Required**: Any configuration change requires creating a new `ConnectionMultiplexer` instance

## Potential Configuration Change Approaches

### 1. Connection Recreation (Current Pattern)
```csharp
// Current approach - requires new connection
var newConfig = oldConfig.Clone();
newConfig.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-west-2");
var newMultiplexer = await ConnectionMultiplexer.ConnectAsync(newConfig);
```

### 2. Potential Runtime Reconfiguration (Not Currently Implemented)

To enable runtime configuration changes, the following would need to be implemented:

```csharp
// Hypothetical API
public async Task ReconfigureAsync(Action<ConfigurationOptions> configure)
{
    var newConfig = RawConfig.Clone();
    configure(newConfig);

    // Would need to:
    // 1. Validate configuration changes
    // 2. Update underlying client configuration
    // 3. Potentially recreate connections
    // 4. Update RawConfig
}
```

### 3. Builder Pattern Extension

A potential approach could extend the builder pattern to support updates:

```csharp
// Hypothetical API
public async Task<bool> TryUpdateConfigurationAsync<T>(Action<T> configure)
    where T : ClientConfigurationBuilder<T>, new()
{
    // Create new builder from current configuration
    // Apply changes
    // Validate and apply if possible
}
```

## ReadFrom Configuration Specifics

### Current Implementation
- `ReadFrom` is a struct (line 74) with `ReadFromStrategy` enum and optional AZ string
- Mapped in `CreateClientConfigBuilder()` at line 199
- Flows through to FFI layer via `ConnectionConfig.ToFfi()` method

### ReadFrom Change Requirements
To change `ReadFrom` configuration at runtime would require:
1. **API Design**: Method to accept new `ReadFrom` configuration
2. **Validation**: Ensure new configuration is compatible with current connection type
3. **FFI Updates**: Update the underlying client configuration
4. **Connection Management**: Handle any required connection reestablishment

## Recommendations

### Short Term
1. **Document Current Limitations**: Clearly document that configuration changes require connection recreation
2. **Helper Methods**: Provide utility methods for common reconfiguration scenarios:
   ```csharp
   public static async Task<ConnectionMultiplexer> RecreateWithReadFromAsync(
       ConnectionMultiplexer current,
       ReadFrom newReadFrom)
   ```

### Long Term
1. **Runtime Reconfiguration API**: Implement selective runtime configuration updates for non-disruptive changes
2. **Configuration Validation**: Add validation to determine which changes require reconnection vs. runtime updates
3. **Connection Pool Management**: Consider connection pooling to minimize disruption during reconfiguration

## Conclusion

Currently, the `ConnectionMultiplexer` does not support runtime configuration changes. The architecture is designed around immutable configuration set at connection time. Any configuration changes, including `ReadFrom` strategy modifications, require creating a new `ConnectionMultiplexer` instance.

The relationship between `ConfigurationOptions` and `ConnectionConfiguration` is a translation layer where the external API (`ConfigurationOptions`) is converted to internal configuration structures (`ConnectionConfiguration`) that interface with the FFI layer.
