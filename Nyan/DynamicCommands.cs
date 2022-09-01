namespace Nyan;

public class DynamicCommands
{
    internal class EventContainer
    {
        public event Command? OnCommand;

        public Task Invoke(string args, Response response)
        {
            var commands = OnCommand?.Invoke(args, response);
            return commands ?? Task.CompletedTask;
        }
    }

    internal readonly Dictionary<string, EventContainer> commandDic;
    public event Response? Response;
    private void InvokeResponse(string args) => Response?.Invoke(args);

    public DynamicCommands()
    {
        commandDic = new Dictionary<string, EventContainer>();
    }

    public void RegisterCommand(string command, Command? callback)
    {
        if (!commandDic.ContainsKey(command)) commandDic.Add(command, new EventContainer());
        commandDic[command].OnCommand += callback;
    }

    public async Task CallCommand(string input)
    {
        var commands = input.Split(' ');
        if (commandDic.TryGetValue(commands[0], out var commandContainer))
            await commandContainer.Invoke(input, InvokeResponse);
        else Response?.Invoke("no command found for " + commands[0]);
    }

    public void Clear()
    {
        Response = null;
        commandDic.Clear();
    }
}