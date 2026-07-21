// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Collections.Concurrent;

namespace Valkey.Glide.Internals;

internal class MessageContainer(BaseClient client)
{
    #region Private Fields

    private readonly List<Message> _messages = [];
    private readonly ConcurrentQueue<Message> _availableMessages = new();

#pragma warning disable IDE0052
    private readonly BaseClient _client = client;
#pragma warning restore IDE0052

    #endregion
    #region Internal Methods

    internal void DisposeWithError()
    {
        lock (_messages)
        {
            List<Message> incompleteMessages = [.. _messages.Where(message => !message.IsCompleted)];

            if (incompleteMessages.Count > 0)
            {
                Logger.Log(Level.Error, GetType().Name, $"Client is closing, but there are {incompleteMessages.Count} ongoing requests");
            }

            foreach (Message message in incompleteMessages)
            {
                try
                {
                    message.SetException(new TaskCanceledException($"Client {_client} closed"));
                }
                catch (Exception) { }
            }
        }
    }

    internal Message GetMessage(int index) => _messages[index];

    internal Message GetMessageForCall()
    {
        Message message = GetFreeMessage();
        message.SetupTask(_client);
        return message;
    }

    internal void ReturnFreeMessage(Message message)
        => _availableMessages.Enqueue(message);

    #endregion
    #region Private Methods

    private Message GetFreeMessage()
    {
        if (!_availableMessages.TryDequeue(out Message? message))
        {
            lock (_messages)
            {
                int index = _messages.Count;
                message = new Message(index, this);
                _messages.Add(message);
            }
        }
        return message;
    }

    #endregion
}
