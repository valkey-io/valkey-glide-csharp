// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal class ValkeyTransaction : ValkeyBatch, ITransaction
{
    private readonly List<ConditionResult> _conditionResults = [];

    public ValkeyTransaction(BaseClient client) : base(client)
    {
        _isAtomic = true;
    }

    public ConditionResult AddCondition(Condition condition)
    {
        ConditionResult res = new(condition);
        _conditionResults.Add(res);
        return res;
    }

    public bool Execute(CommandFlags flags = CommandFlags.None)
        => ExecuteAsync(flags).GetAwaiter().GetResult();

    public async Task<bool> ExecuteAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        // Evaluate all conditions asynchronously before submitting the transaction.
        // This replaces the former sync-over-async PreExecCheck() override in the
        // base class, keeping transaction-specific logic here rather than leaking
        // it into ValkeyBatch.
        foreach (var conditionResult in _conditionResults)
        {
            var condition = conditionResult.Condition;

            // Execute the condition commands in a non-atomic batch to check the
            // current state of the watched keys.
            var batch = new ValkeyTransaction(_client) { _isAtomic = false };
            batch._commands.AddRange(condition.CreateCommands());
            await batch.ExecuteImpl();

            // At least one condition failed — cancel the transaction without
            // submitting the MULTI/EXEC block.
            conditionResult.WasSatisfied = condition.Validate(batch._tcs.Task.Result);

            if (!conditionResult.WasSatisfied)
            {
                // Complete the transaction before clearing the WATCH set by the condition commands.
                // This ensures that the transacrion result is set even if the WATCH command throws.
                _ = _tcs.TrySetResult(null);
                _ = await _client.Command(Request.Unwatch());

                return false;
            }
        }

        // If there are conditions but no queued commands, no EXEC is sent (GLIDE
        // rejects an empty transaction), so clear the WATCH set by the conditions.
        if (_conditionResults.Count > 0 && _commands.Count == 0)
        {
            _ = _tcs.TrySetResult([]);
            _ = await _client.Command(Request.Unwatch());

            return true;
        }

        await ExecuteImpl();
        return _tcs.Task.Result is not null;
    }
}
