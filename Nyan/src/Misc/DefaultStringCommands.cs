using System.Text;
using Nyan.Types;

namespace Nyan.Misc;

internal sealed class DefaultStringCommands : IStringCommands
{
    private readonly Dictionary<string, EventContainer> _commandDic;
    private event Response? Response;
    private void InvokeResponse(ReadOnlySpan<char> args) => Response?.Invoke(args);

    public DefaultStringCommands()
    {
        _commandDic = new Dictionary<string, EventContainer>();
    }


    public Task RegisterCommand(ReadOnlySpan<char> commandSpan, Command? callback)
    {
        var command = commandSpan.ToString();
        if (!_commandDic.ContainsKey(command)) _commandDic.Add(command, new EventContainer());
        _commandDic[command].OnCommand += callback;
        return Task.CompletedTask;
    }

    public Task CallCommand(ReadOnlySpan<char> commandSpan, ReadOnlySpan<char> args)
    {
        var command = commandSpan.ToString();
        if (_commandDic.TryGetValue(command, out var commandContainer)) commandContainer.Invoke(args, InvokeResponse);
        else InvokeResponse($"Command '{command}' not found.".AsSpan());
        return Task.CompletedTask;
    }

    public Task UnregisterCommand(ReadOnlySpan<char> command)
    {
        _commandDic.Remove(command.ToString());
        return Task.CompletedTask;
    }

    public Task UnregisterAllCommands()
    {
        _commandDic.Clear();
        return Task.CompletedTask;
    }

    public Task RegisterOutput(Response response)
    {
        Response += response;
        return Task.CompletedTask;
    }

    public Task RemoveOutput(Response response)
    {
        if (Response != null) Response -= response;
        return Task.CompletedTask;
    }

    public Task RemoveAllOutput()
    {
        Response = null;
        return Task.CompletedTask;
    }

    public int GetCommandCount()
    {
        return _commandDic.Count;
    }

    public void InitializeCommands()
    {
        RegisterCommand("help", (_, response) =>
        {
            var sb = new StringBuilder();
            sb.AppendLine("Available commands (" + GetCommandCount() + "):");
            foreach (var command in _commandDic)
            {
                sb.AppendLine(command.Key);
            }

            response(sb.ToString());
            return Task.CompletedTask;
        });
    }


    private struct EventContainer
    {
        public event Command? OnCommand;

        public Task Invoke(ReadOnlySpan<char> args, Response response) =>
            OnCommand?.Invoke(args, response) ?? Task.CompletedTask;
    }
}