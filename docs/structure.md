# Structure Guidelines

This document defines guidelines for organizing code in the Valkey GLIDE C# client.
The goal is to organize code elements consistently so that the codebase is as easy
as possible for developers to read and understand.

## General

- Group related elements together, and order them so that a reader can find what they are looking for.
- Prefer a logical order when one exists. Otherwise, order alphabetically (case-insensitive).

## Lists

Order lists and other sequences of items to maximize readability.

This includes:

- Collection and array initializers.
- Switch cases and pattern-matching arms.
- Enum members.

Within a list:

1. Group related items together, and separate groups with a blank line or comment.
2. Order items using a logical order when one exists. For example:
   - Reference: match the order of a related definition (e.g. glide-core, an interface, or StackExchange.Redis).
   - Lifecycle: order items in the sequence they occur (e.g. create, use, then dispose).
   - Magnitude: order items by numeric value, size, or severity.
   - Dependency: place items before the items that depend on them.
3. If there is no other logical order, consider ordering items alphabetically (e.g. command declarations).

> [!WARNING]
> Do not re-order items when it would change behaviour or cause a breaking change. For example,
> reordering enum members without explicit values changes their values, and reordering overlapping
> switch arms changes which arm matches.
