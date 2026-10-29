# Structure Guidelines

This document defines guidelines for organizing code in the Valkey GLIDE C# client.
The goal is to organize code elements consistently so that the codebase is as easy
as possible for developers to read and understand.

## Lists

Order lists and other sequences of items to maximize readability.

Within a list:

1. Group related items together, and separate groups with a blank line or comment.
2. Order items using a logical order when one exists. For example:
   - Reference: match the order of a related definition (e.g. glide-core, an interface, or StackExchange.Redis).
   - Lifecycle: order items in the sequence they occur (e.g. create, use, then dispose).
   - Magnitude: order items by numeric value, size, or severity.
   - Dependency: place items before the items that depend on them.
3. If there is no other logical order, consider ordering items alphabetically (e.g. a set of command names).

> [!WARNING]
> Do not reorder items when it would change behavior or cause a breaking change. For example,
> reordering enum members without explicit values changes their values, and reordering overlapping
> switch arms changes which arm matches.

## Members

Order the members of a type consistently to make it easier for developers to find what they're looking for.

Members should be ordered first by kind, next by access level, and finally by modifier. This is enforced automatically by [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) rules. Within these groups, order methods following the [Lists](#lists) guidelines.

## Regions

Use region directives (`#region`) to make large types easier to read and maintain by breaking them into manageable sections.

Guidelines for using regions:

- Regions are an organizational aid, not a requirement. They should only be used when
  they make the type easier to read and/or maintain. Small types usually don't need regions.
- Name each region after what its members have in common (e.g. `Public Methods` or `Subscriber Commands`).
- Leave a blank line after `#region` ([RCS0002](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0002))
  and before `#endregion` ([RCS0005](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS0005)).

Some common patterns:

- **Public types** (e.g. `CircuitBreakerConfig.cs`): match regions to the kind and access level groups in [Members](#members) (e.g. `Public Methods`).
- **Command partials** (e.g. `BaseClient.*.cs`): one region per command or related commands (e.g. `SubscribeCommands`).
- **Builders and converters** (e.g. `Request.*.cs`): one region per purpose (e.g. `Command Builders`, `Argument Builders`, `Response Converters`).
- **Tests** (e.g. `HashCommandTests.cs`): regions for the parts of a test class (e.g. `Constants`, `Test Data`, `Tests`, `Helpers`), or separate regions for each command or feature under test (e.g. `HashSetAsync`).

## Example

The following class applies the guidelines from each section:

- [Lists](#lists): `Commands` uses logical groups and order, and `ReadOnlyCommands` uses alphabetical order.
- [Members](#members): members are ordered by kind (fields, constructors, properties, methods),
  then by access level, then by modifier (constants before other fields, and static before instance members).
  Public methods follow lifecycle order, and overloads are ordered from fewest to most parameters.
- [Regions](#regions): regions match the kind and access level groups.

```csharp
public sealed class Subscriber : IDisposable
{
    #region Constants

    public const int DefaultTimeoutMs = 1000;

    private const string ChannelPrefix = "glide:";

    #endregion
    #region Private Fields

    // Logical groups in lifecycle order (subscribe, unsubscribe),
    // with items in reference order (channel, pattern, shard).
    private static readonly string[] Commands =
    [
        // Subscribe
        "SUBSCRIBE",
        "PSUBSCRIBE",
        "SSUBSCRIBE",

        // Unsubscribe
        "UNSUBSCRIBE",
        "PUNSUBSCRIBE",
        "SUNSUBSCRIBE",
    ];

    // Alphabetical order, where there is no other logical order.
    private static readonly string[] ReadOnlyCommands =
    [
        "GET",
        "HGET",
        "HGETALL",
        "LRANGE",
        "STRLEN",
    ];

    private readonly BaseClient client;
    private readonly HashSet<string> channels = [];

    #endregion
    #region Constructors

    public Subscriber(BaseClient client) { ... }

    #endregion
    #region Public Properties

    public int ChannelCount => channels.Count;

    #endregion
    #region Public Methods

    public static Subscriber Create(BaseClient client) { ... }

    public Task SubscribeAsync(string channel) { ... }

    public Task SubscribeAsync(string channel, TimeSpan timeout) { ... }

    public Task UnsubscribeAsync(string channel) { ... }

    public void Dispose() { ... }

    #endregion
    #region Internal Methods

    internal bool IsSubscribed(string channel) { ... }

    #endregion
    #region Private Methods

    private static string ToKey(string channel) { ... }

    private Task SendAsync(string command, string channel) { ... }

    #endregion
}
```
