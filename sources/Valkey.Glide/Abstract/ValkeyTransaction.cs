// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal class ValkeyTransaction : ValkeyBatch, ITransaction
{
    private readonly List<ConditionResult> _conditions = [];

    public ValkeyTransaction(BaseClient client) : base(client)
    {
        _isAtomic = true;
    }

    public ConditionResult AddCondition(Condition condition)
    {

        ConditionResult res = new(condition);
        _conditions.Add(res);
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
        foreach (ConditionResult condition in _conditions)
        {
            // Execute the condition commands in a non-atomic batch to check the
            // current state of the watched keys.
            ValkeyTransaction conditionBatch = new(_client)
            {
                _isAtomic = false
            };
            conditionBatch._commands.AddRange(condition.Condition.CreateCommands());
            await conditionBatch.ExecuteImpl();

            condition.WasSatisfied = condition.Condition.Validate(conditionBatch._tcs.Task.Result);
            if (!condition.WasSatisfied)
            {
                // At least one condition failed — cancel the transaction without
                // submitting the MULTI/EXEC block.
                _tcs.SetResult(null);
                return false;
            }
        }

        await ExecuteImpl();
        return _tcs.Task.Result is not null;
    }
}
