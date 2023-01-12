using Nyan.Misc;
using Nyan.Types;

namespace Nyan;

public abstract class BotManager
{
    private static BotManager? _instance;

    public static BotManager Manager
    {
        get => _instance ?? throw new InvalidOperationException();
        private set => _instance = value;
    }

    public NyanBot Bot { get; internal set; } = new();

    public IStringCommands commands { get; protected set; } = new DefaultStringCommands();

    internal async Task Run_Internal()
    {
        if (_instance != null) throw new InvalidOperationException("Instance already exists");
        Manager = this;
        await Run();
    }
    
    internal void Stop_Internal()
    {
        _instance = null;
        Stop();
    }

    protected abstract Task Run();
    protected abstract void Stop();
}