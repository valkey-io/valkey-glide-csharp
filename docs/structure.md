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

### Examples

Example of logical groups and order. The groups follow lifecycle order (subscribe, unsubscribe),
and the items within each group follow the same reference order (channel, pattern, shard):

```csharp
string[] commands =
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
```

Example of alphabetical order, where there is no other logical order:

```csharp
string[] readOnlyCommands =
[
    "GET",
    "HGET",
    "HGETALL",
    "LRANGE",
    "STRLEN",
];
```
