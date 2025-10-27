// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Valkey.Glide;

/// <summary>
/// Utility for parsing and mapping named parameters in Lua scripts.
/// Supports StackExchange.Redis-style @parameter syntax.
/// </summary>
internal static class ScriptParameterMapper
{
    private static readonly Regex ParameterRegex = new(@"@([a-zA-Z_][a-zA-Z0-9_]*)",
        RegexOptions.Compiled);

    /// <summary>
    /// Prepares a script by extracting parameters and converting to KEYS/ARGV syntax.
    /// </summary>
    /// <param name="script">The script with @parameter syntax.</param>
    /// <returns>A tuple containing the original script, executable script, and parameter names.</returns>
    internal static (string OriginalScript, string ExecutableScript, string[] Parameters) PrepareScript(string script)
    {
        if (string.IsNullOrEmpty(script))
        {
            throw new ArgumentException("Script cannot be null or empty", nameof(script));
        }

        var parameters = new List<string>();
        var parameterIndices = new Dictionary<string, int>();

        // Extract unique parameters in order of first appearance
        foreach (Match match in ParameterRegex.Matches(script))
        {
            string paramName = match.Groups[1].Value;
            if (!parameterIndices.ContainsKey(paramName))
            {
                parameterIndices[paramName] = parameters.Count;
                parameters.Add(paramName);
            }
        }

        // Convert @param to placeholder for later substitution
        // We use placeholders because we don't know yet if parameters are keys or arguments
        string executableScript = ParameterRegex.Replace(script, match =>
        {
            string paramName = match.Groups[1].Value;
            int index = parameterIndices[paramName];
            return $"{{PARAM_{index}}}";
        });

        return (script, executableScript, parameters.ToArray());
    }

    /// <summary>
    /// Replaces parameter placeholders in the executable script with KEYS/ARGV references.
    /// </summary>
    /// <param name="executableScript">The script with {PARAM_i} placeholders.</param>
    /// <param name="parameterNames">The parameter names in order.</param>
    /// <param name="parameters">The parameter object.</param>
    /// <returns>The script with placeholders replaced by KEYS[i] and ARGV[i] references.</returns>
    internal static string ReplacePlaceholders(string executableScript, string[] parameterNames, object parameters)
    {
        Type paramType = parameters.GetType();

        // Build a mapping from parameter index to KEYS/ARGV reference
        var replacements = new Dictionary<int, string>();
        int keyIndex = 1; // Lua arrays are 1-based
        int argIndex = 1;

        for (int i = 0; i < parameterNames.Length; i++)
        {
            string paramName = parameterNames[i];

            // Get the parameter's type
            var property = paramType.GetProperty(paramName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var field = paramType.GetField(paramName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            Type memberType = property?.PropertyType ?? field!.FieldType;

            // Determine if this is a key or argument based on type
            if (IsKeyType(memberType))
            {
                replacements[i] = $"KEYS[{keyIndex++}]";
            }
            else
            {
                replacements[i] = $"ARGV[{argIndex++}]";
            }
        }

        // Replace placeholders
        foreach (var kvp in replacements)
        {
            executableScript = executableScript.Replace($"{{PARAM_{kvp.Key}}}", kvp.Value);
        }

        return executableScript;
    }

    /// <summary>
    /// Replaces parameter placeholders using a heuristic to determine which are keys.
    /// Parameters named "key", "keys", or starting with "key" (case-insensitive) are treated as keys.
    /// </summary>
    /// <param name="executableScript">The script with {PARAM_i} placeholders.</param>
    /// <param name="parameterNames">The parameter names in order.</param>
    /// <returns>The script with placeholders replaced by KEYS[i] and ARGV[i] references.</returns>
    internal static string ReplacePlaceholdersWithHeuristic(string executableScript, string[] parameterNames)
    {
        var replacements = new Dictionary<int, string>();
        int keyIndex = 1; // Lua arrays are 1-based
        int argIndex = 1;

        for (int i = 0; i < parameterNames.Length; i++)
        {
            string paramName = parameterNames[i].ToLowerInvariant();

            // Heuristic: parameters named "key", "keys", or starting with "key" are keys
            bool isKey = paramName == "key" || paramName == "keys" || paramName.StartsWith("key");

            if (isKey)
            {
                replacements[i] = $"KEYS[{keyIndex++}]";
            }
            else
            {
                replacements[i] = $"ARGV[{argIndex++}]";
            }
        }

        // Replace placeholders
        foreach (var kvp in replacements)
        {
            executableScript = executableScript.Replace($"{{PARAM_{kvp.Key}}}", kvp.Value);
        }

        return executableScript;
    }

    /// <summary>
    /// Validates that a parameter object has all required properties and they are of valid types.
    /// </summary>
    /// <param name="type">The type of the parameter object.</param>
    /// <param name="parameterNames">The required parameter names.</param>
    /// <param name="missingMember">Output parameter for the first missing member name.</param>
    /// <param name="badTypeMember">Output parameter for the first member with invalid type.</param>
    /// <returns>True if all parameters are valid, false otherwise.</returns>
    internal static bool IsValidParameterHash(Type type, string[] parameterNames,
        out string? missingMember, out string? badTypeMember)
    {
        missingMember = null;
        badTypeMember = null;

        foreach (string paramName in parameterNames)
        {
            var property = type.GetProperty(paramName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var field = type.GetField(paramName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (property == null && field == null)
            {
                missingMember = paramName;
                return false;
            }

            Type memberType = property?.PropertyType ?? field!.FieldType;
            if (!IsValidParameterType(memberType))
            {
                badTypeMember = paramName;
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Generates a function to extract parameters from an object.
    /// Uses expression trees for efficient parameter extraction.
    /// </summary>
    /// <param name="type">The type of the parameter object.</param>
    /// <param name="parameterNames">The parameter names to extract.</param>
    /// <returns>A function that extracts parameters from an object and returns keys and arguments.</returns>
    internal static Func<object, ValkeyKey?, (ValkeyKey[] Keys, ValkeyValue[] Args)> GetParameterExtractor(
        Type type, string[] parameterNames)
    {
        // Build expression tree for efficient parameter extraction
        var paramObj = Expression.Parameter(typeof(object), "obj");
        var keyPrefix = Expression.Parameter(typeof(ValkeyKey?), "prefix");
        var typedObj = Expression.Variable(type, "typedObj");

        var assignments = new List<Expression>
        {
            Expression.Assign(typedObj, Expression.Convert(paramObj, type))
        };

        // Extract keys and values
        var keysList = new List<Expression>();
        var valuesList = new List<Expression>();

        foreach (string paramName in parameterNames)
        {
            var property = type.GetProperty(paramName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var field = type.GetField(paramName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            MemberExpression member;
            Type memberType;

            if (property != null)
            {
                member = Expression.Property(typedObj, property);
                memberType = property.PropertyType;
            }
            else if (field != null)
            {
                member = Expression.Field(typedObj, field);
                memberType = field.FieldType;
            }
            else
            {
                throw new ArgumentException($"Parameter '{paramName}' not found on type {type.Name}");
            }

            // Determine if this is a key (ValkeyKey type) or argument
            if (IsKeyType(memberType))
            {
                // Convert to ValkeyKey
                var keyValue = Expression.Convert(member, typeof(ValkeyKey));

                // Apply prefix if provided
                // WithPrefix expects (byte[]? prefix, ValkeyKey value)
                // We need to convert ValkeyKey? to byte[]?
                var prefixAsBytes = Expression.Convert(
                    Expression.Convert(keyPrefix, typeof(ValkeyKey)),
                    typeof(byte[]));

                var prefixedKey = Expression.Condition(
                    Expression.Property(keyPrefix, "HasValue"),
                    Expression.Call(
                        typeof(ValkeyKey).GetMethod("WithPrefix", BindingFlags.NonPublic | BindingFlags.Static)!,
                        prefixAsBytes,
                        keyValue),
                    keyValue);

                keysList.Add(prefixedKey);
            }
            else
            {
                // Convert to ValkeyValue
                var valueExpr = Expression.Convert(member, typeof(ValkeyValue));
                valuesList.Add(valueExpr);
            }
        }

        // Create arrays
        var keysArray = Expression.NewArrayInit(typeof(ValkeyKey), keysList);
        var valuesArray = Expression.NewArrayInit(typeof(ValkeyValue), valuesList);

        // Create tuple
        var tupleType = typeof((ValkeyKey[], ValkeyValue[]));
        var tupleConstructor = tupleType.GetConstructor([typeof(ValkeyKey[]), typeof(ValkeyValue[])])!;
        var result = Expression.New(tupleConstructor, keysArray, valuesArray);

        // Build the lambda
        var blockExpressions = new List<Expression>(assignments)
        {
            result
        };
        var body = Expression.Block(
            [typedObj],
            blockExpressions
        );

        var lambda = Expression.Lambda<Func<object, ValkeyKey?, (ValkeyKey[], ValkeyValue[])>>(
            body, paramObj, keyPrefix);

        return lambda.Compile();
    }

    /// <summary>
    /// Checks if a type is valid for use as a script parameter.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is valid, false otherwise.</returns>
    internal static bool IsValidParameterType(Type type)
    {
        // Unwrap nullable types
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        // Check for key types
        if (IsKeyType(underlyingType))
        {
            return true;
        }

        // Check for value types
        if (underlyingType == typeof(string) ||
            underlyingType == typeof(byte[]) ||
            underlyingType == typeof(int) ||
            underlyingType == typeof(long) ||
            underlyingType == typeof(uint) ||
            underlyingType == typeof(ulong) ||
            underlyingType == typeof(double) ||
            underlyingType == typeof(float) ||
            underlyingType == typeof(bool) ||
            underlyingType == typeof(ValkeyValue) ||
            underlyingType == typeof(GlideString))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if a type should be treated as a key (vs an argument).
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is a key type, false otherwise.</returns>
    private static bool IsKeyType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        return underlyingType == typeof(ValkeyKey);
    }
}
