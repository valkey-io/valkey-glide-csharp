// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Stream entry response from the <c>XRANGE</c> command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xrange/"/>
/// <remarks>
/// Creates an stream entry.
/// </remarks>
/// <param name="id">The ID assigned to the message.</param>
/// <param name="values">The values contained within the message.</param>
public readonly struct StreamEntry(ValkeyValue id, NameValueEntry[] values)
{
    #region Constants

    /// <summary>
    /// A null stream entry.
    /// </summary>
    public static StreamEntry Null { get; } = new StreamEntry(ValkeyValue.Null, []);

    #endregion
    #region Public Properties

    /// <summary>
    /// The ID assigned to the message.
    /// </summary>
    public ValkeyValue Id { get; } = id;

    /// <summary>
    /// The values contained within the message.
    /// </summary>
    public NameValueEntry[] Values { get; } = values;

    /// <summary>
    /// Search for a specific field by name, returning the value.
    /// </summary>
    /// <param name="fieldName">The name of the field to search for.</param>
    public ValkeyValue this[ValkeyValue fieldName]
    {
        get
        {
            var values = Values;
            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i].Name == fieldName)
                    {
                        return values[i].Value;
                    }
                }
            }

            return ValkeyValue.Null;
        }
    }

    /// <summary>
    /// Indicates that the entry is null.
    /// </summary>
    public bool IsNull
        => Id == ValkeyValue.Null && Values == Array.Empty<NameValueEntry>();

    #endregion
}
