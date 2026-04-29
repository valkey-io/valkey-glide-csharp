# Documentation Guidelines

This document defines guidelines for documentation in the Valkey GLIDE C# client.

## General

### XML Documentation Requirements

- All public and protected members must have XML documentation comments.
- The compiler should produce zero XML documentation warnings (`CS1591`).

### Language and Tone

- Use clear, concise, simple language.
- Describe the member's purpose or actions.
- Avoid redundant or verbose phrases like "This method..." or "the key of the list".
- Do not repeat information that is already available elsewhere (e.g. method signatures,
  type definitions, or the other reference).
- Terminate sentences with a period.

### Formatting Conventions

- Use `<see cref="..."/>` for all type and member references.
- Use `<see langword="..." />` for language keywords (`true`, `false`, `null`).
- Use `<paramref name="..." />` when referring to parameters within descriptions.
- Use `<br/>` for line breaks within an XML element.
- Keep line lengths reasonable (≤ 120 characters within the `///` comment block where practical).

## Command Methods

This section covers documentation specific to methods that implement Valkey commands.
It applies to the shared (`I*Commands`), Valkey GLIDE (`IBaseClient.*`) and the
StackExchange.Redis compatibility (`IDatabaseAsync.*`, `IServer`) interfaces.

### Format

Command method documentation format:

```xml
/// <summary>...</summary>
/// <seealso href="https://valkey.io/commands/{command}/">Valkey commands – {COMMAND}</seealso>
/// <note>...</note>
/// <param name="...">...</param>
/// <returns>...</returns>
/// <exception cref="...">...</exception>
/// <remarks>
///   <example>
///     <code>...</code>
///   </example>
/// </remarks>
```

### Content

Guidelines for command method documentation content:

1. **`<summary>`** — Required. Single occurrence.
   - Describe what the command does in one or two sentences.
   - Keep it concise — do not duplicate information already covered by other tags or
     the linked Valkey command reference.
   - For commands that map to multiple Valkey commands, describe the unified behavior.
   - Use third-person present tense (e.g. "Returns", "Sets", "Removes").

2. **`<seealso>`** — Required. One or more occurrences.
   - Link to the corresponding Valkey command documentation.
   - Format: `<seealso href="https://valkey.io/commands/{command}/">Valkey commands – {COMMAND}</seealso>`
   - If a method maps to multiple Valkey commands, include a `<seealso>` for each.

3. **`<note>`** — Optional. Zero or more occurrences.
   - **Version requirements**: When a command requires a minimum Valkey version
     (e.g., `Since Valkey 6.2.0.`).
   - **Cluster mode behavior**: When a multi-key command has non-atomic behavior across hash slots.
   - **Slot constraints**: When keys must reside in the same hash slot
     (e.g., `When in cluster mode, both key and newKey must map to the same hash slot.`).

4. **`<param>`** — Required for each parameter. One occurrence per parameter.
   - Document every parameter, including those with default values.
   - Do not restate default values already visible in the method signature.
   - Be specific about what the parameter represents in the context of the Valkey command.

5. **`<returns>`** — Required (unless the method returns `void` or `Task`). Single occurrence.
   - Clearly describe the return value and its type.
   - Document the behavior when the key does not exist (e.g., returns `ValkeyValue.Null`).

6. **`<exception>`** — Optional. Zero or more occurrences.
   - Document exceptions that callers should be aware of.
   - Use `<exception cref="...">` with a description of when the exception is thrown.

7. **`<remarks>` / `<example>` / `<code>`** — Required. One or more `<example>` blocks inside a single `<remarks>`.
   - Examples should be **self-contained**:they should include any setup needed to determine the expected return
   value from the example alone; this should include populating any relevant keys first (e.g., call `SetAsync` before `GetAsync`).
   - Examples should follow code format and style conventions from this project.
   - For methods with notable edge cases, include multiple `<example>` blocks.
   - Use descriptive variable names; avoid generic names like `result`.
   - **Expected return**: annotate the returned value with an inline comment:
     - strings: `// "value"`
     - numbers: `// 0`
     - lists/arrays: `// ["value1", "value2"]`
     - dictionaries: `// {key1: "value1", key2: "value2"}`
     - sets: `// {"value1", "value2"}`
   - **When the return value cannot be determine** (e.g., latency, server time) or is
     impractical to set up, use `Console.WriteLine` to show how it would be consumed —
     for example, `Console.WriteLine($"Received response after {latency.TotalSeconds} seconds")`.
   - **Be concise**: use `var`, collection expressions (`["a", "b"]`), and other modern
     C# features to keep examples short without sacrificing clarity.

### Inheritdoc

`<inheritdoc>` should be used to avoid duplicating documentation across related methods.

#### Prefer Explicit Referemces

Always prefer explicit `<inheritdoc cref="..."/>` over bare `<inheritdoc/>`:

```csharp
/// <inheritdoc cref="IHashBaseCommands.HashGetAsync(ValkeyKey, ValkeyValue)"/>
public async Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField) { ... }
```

Explicit `cref` improves resolution and makes the inheritance target unambiguous when a type
implements multiple interfaces with similarly-named members.

#### Use Path Filter Syntax

A `path` attribute can be used to restricts which tags are inherited. Its value is an XPath expression
evaluated against the referenced method's XML doc. To avoid complex path expressions, the codebase
tends to use the following two patterns:

- **Inherit a single tag**: `path="/summary"` inherits only `<summary>`; every other tag is
  redocumented inline.
- **Inherit everything except one or more tags**: `path="/*[not(self::returns)]"` inherits
  every top-level tag except `<returns>`. Extend the predicate with `and` to exclude more
  tags, e.g. `path="/*[not(self::returns) and not(self::note)]"`.

### StackExchange.Redis Compatibility

Whenever a StackExchange.Redis compatibility-layer method accepts a `CommandFlags` parameter,
the following tags are **always** present inline:

- `<param name="flags">Command flags (currently not supported by GLIDE).</param>`
- `<exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>`

## Examples

### Valkey GLIDE Command Methods

Basic command method:

```csharp
/// <summary>
/// Returns the values of keys.
/// </summary>
/// <seealso href="https://valkey.io/commands/mget/">Valkey commands – MGET</seealso>
/// <param name="keys">The keys to retrieve.</param>
/// <returns>An array with the value for each key, or <see cref="ValkeyValue.Null"/> if it does not exist.
/// </returns>
/// <remarks>
/// <example>
/// <code>
/// await client.SetAsync("key", "hello");
/// var values = await client.GetAsync(["key", "nonexistent"]);  // ["hello", ValkeyValue.Null]
/// </code>
/// </example>
/// </remarks>
Task<ValkeyValue[]> GetAsync(IEnumerable<ValkeyKey> keys);
```

Command method overload with `inheritdoc`

```csharp
/// <inheritdoc cref="StreamReadGroupAsync(StreamPosition, ValkeyValue, ValkeyValue)" path="/*[not(self::returns)]"/>
/// <param name="options">Options including count, block timeout, and noAck.</param>
/// <returns>An array of <see cref="StreamEntry"/> values read from the stream.</returns>
Task<StreamEntry[]> StreamReadGroupAsync(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options);
```

### StackExchange.Redis Command Methods

When the StackExchange.Redis method simply wraps a corresponding shared or Valkey GLIDE method, use `<inheritdoc>` to avoid duplication:

```csharp
/// <inheritdoc cref="IBaseClient.GetAsync(ValkeyKey)"/>
/// <param name="flags">Command flags (currently not supported by GLIDE).</param>
/// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);
```

When the StackExchange.Redis method has some elements that differ from the shared method, use `path` filtering
to exclude inherited params and redocument them:

```csharp
/// <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue, SetOptions)" path="/summary"/>
/// <param name="key">The key to store.</param>
/// <param name="value">The value to store.</param>
/// <param name="expiry">The expiry to set. <see langword="null"/> means no expiry.</param>
/// <param name="keepTtl">Whether to retain the existing TTL.</param>
/// <param name="when">The condition under which the key should be set.</param>
/// <param name="flags">Command flags (currently not supported by GLIDE).</param>
/// <returns><see langword="true"/> if the key was set, <see langword="false"/> if the condition was not met.</returns>
/// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None);
```

When `<inheritdoc>` is not suitable, write the documentation inline using StackExchange.Redis wording with
type substitutions:

```csharp
/// <summary>
/// Get the value of key. If the key does not exist the special value <see cref="ValkeyValue.Null"/> is returned.
/// An error is returned if the value stored at key is not a string, because GET only handles string values.
/// </summary>
/// <param name="key">The key of the string.</param>
/// <param name="flags">Command flags (currently not supported by GLIDE).</param>
/// <returns>The value of key, or <see cref="ValkeyValue.Null"/> when key does not exist.</returns>
/// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);
```
