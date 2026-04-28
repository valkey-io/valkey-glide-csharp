# Documentation Guidelines

This document defines guidelines for documentation in the Valkey GLIDE C# client.

## General

### XML Documentation Requirements

- All public and protected members **must** have XML documentation comments.
- All parameters must be documented with `<param>` tags.
- All methods that return a value (i.e., not `void` or `Task`) must include a `<returns>` tag.
- The compiler should produce zero XML documentation warnings (`CS1591`).

### Language and Tone

- Use clear, concise language.
- Terminate sentences with a period.
- Write from the perspective of the member's purpose or actions.
- Avoid redundant phrases like "This method..." or "Use this to...", or
  verbose phrases like "the key of the list" instead of "the list key".

### Formatting Conventions

- Use `<see cref="..."/>` for all type and member references.
- Use `<see langword="..." />` for language keywords (`true`, `false`, `null`).
- Use `<paramref name="..." />` when referring to parameters within descriptions.
- Use `<br/>` for line breaks within an XML element.
- Keep line lengths reasonable (≤ 120 characters within the `///` comment block where practical).

---

## Command Methods

This section covers documentation specific to methods that implement Valkey commands.
It applies to the shared (`I*Commands`), Valkey GLIDE (`IBaseClient.*`) and the
StackExchange.Redis compatibility (`IDatabaseAsync.*`, `IServer`) interfaces.

### Format

XML doc tags for command methods should appear in the following order. Each tag's requirements
are described inline:

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
     (e.g., `Since Valkey 6.2.0 and above.`).
   - **Cluster mode behavior**: When a multi-key command has non-atomic behavior across hash slots.
   - **Slot constraints**: When keys must reside in the same hash slot
     (e.g., `When in cluster mode, both key and newKey must map to the same hash slot.`).

4. **`<param>`** — Required for each parameter. One occurrence per parameter.
   - Document every parameter, including those with default values.
   - Mention the default value in the description when applicable.
   - Be specific about what the parameter represents in the context of the Valkey command.

5. **`<returns>`** — Required (unless the method returns `void` or `Task`). Single occurrence.
   - Clearly describe the return value and its type.
   - Document the behavior when the key does not exist (e.g., returns `ValkeyValue.Null`).

6. **`<exception>`** — Optional. Zero or more occurrences.
   - Document exceptions that callers should be aware of.
   - Use `<exception cref="...">` with a description of when the exception is thrown.

7. **`<remarks>` / `<example>` / `<code>`** — Required. Zero or more `<example>` blocks inside a single `<remarks>`.
   - Examples must be self-contained and demonstrate the most common usage.
   - Examples should follow code format and style conventions from this project.
   - For methods with notable edge cases, include multiple `<example>` blocks.
   - Prefer `var` for variables types for more concise examples.
   - Use descriptive variables names to improve readabibility; avoid generic names like `result`.
   - Where helpful, examples may use a comment or `Console.WriteLine` to show expected results.

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

#### StackExchange.Redis Compatibility Layer

The compatibility layer (`IDatabaseAsync.*`, `Database.*`) has additional conventions:

##### Inheriting Documentation (`<inheritdoc>`)

Use `<inheritdoc cref="..." />` to inherit the summary from the corresponding `IBaseClient` method
when the behavior is identical or nearly identical.

```xml
/// <inheritdoc cref="IBaseClient.GetSetAsync(ValkeyKey, ValkeyValue, SetOptions)" path="/summary"/>
```

##### CommandFlags

- All compatibility-layer methods accept a `CommandFlags` parameter.
- Document it consistently:

```xml
/// <param name="flags">Command flags (currently not supported by GLIDE).</param>
/// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
```

---

## Examples

### GLIDE Interface Method

```csharp
/// <summary>
/// Gets the value of a key.
/// </summary>
/// <seealso href="https://valkey.io/commands/get/">Valkey commands – GET</seealso>
/// <param name="key">The key to retrieve.</param>
/// <returns>The value of the key, or <see cref="ValkeyValue.Null"/> if it doesn't exist.
/// </returns>
/// <remarks>
/// <example>
/// <code>
/// await client.SetAsync("key", "hello");
/// var value = await client.GetAsync("key");  // "hello"
/// </code>
/// </example>
/// <example>
/// <code>
/// var missing = await client.GetAsync("nonexistent");  // ValkeyValue.Null
/// </code>
/// </example>
/// </remarks>
Task<ValkeyValue> GetAsync(ValkeyKey key);

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

### StackExchange.Redis Compatibility Layer Method

```csharp
/// <summary>
/// Gets the value of a key.
/// </summary>
/// <seealso href="https://valkey.io/commands/get/">Valkey commands – GET</seealso>
/// <param name="key">The key to retrieve.</param>
/// <param name="flags">Command flags (currently not supported by GLIDE).</param>
/// <returns>The value of the key, or <see cref="ValkeyValue.Null"/> if it doesn't exist.
/// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
/// </returns>
/// <remarks>
/// <example>
/// <code>
/// await client.StringSetAsync("key", "hello");
/// var value = await client.StringGetAsync("key");  // "hello"
/// </code>
/// </example>
/// <example>
/// <code>
/// var missing = await client.StringGetAsync("nonexistent");  // ValkeyValue.Null
/// </code>
/// </example>
/// </remarks>
Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

/// <summary>
/// Returns the values of keys.
/// </summary>
/// <seealso href="https://valkey.io/commands/mget/">Valkey commands – MGET</seealso>
/// <param name="keys">The keys to retrieve.</param>
/// <param name="flags">Command flags (currently not supported by GLIDE).</param>
/// <returns>An array with the value for each key, or <see cref="ValkeyValue.Null"/> if it does not exist.
/// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
/// </returns>
/// <remarks>
/// <example>
/// <code>
/// await client.StringSetAsync("key", "hello");
/// var values = await client.StringGetAsync(["key", "nonexistent"]);  // ["hello", ValkeyValue.Null]
/// </code>
/// </example>
/// </remarks>
Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);
```

---
