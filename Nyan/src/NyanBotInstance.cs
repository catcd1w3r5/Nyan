using System.Diagnostics;
using System.Text;

namespace Nyan;

public class NyanBotInstance
{
    private readonly CancellationTokenSource _cancellationToken = new();

    public static NyanBotInstance Instance
    {
        get => _instance ?? throw new InvalidOperationException();
        private set => _instance = value;
    }

    public void Kill()
    {
        _cancellationToken.Cancel();
    }

    private NyanBot? _bot;
    public readonly DynamicCommands commands = new();
    private static NyanBotInstance? _instance;

    internal async Task Run()
    {
        if (_instance != null) throw new InvalidOperationException("Instance already exists");
        Instance = this;
        _bot = await Utils.CreateNyanBot();
        Instance = this;

        DefaultCommands();
        Debug.Assert(_bot != null, nameof(_bot) + " != null");
        _ = _bot.Connect().ConfigureAwait(false);
        _bot.RegisterPlugins();
        await Task.Delay(-1, _cancellationToken.Token);
        // if (!_bot.TryGetPlugin<ConsoleLib>(out var console))
        //     throw new InvalidOperationException("Console Lib plugin not found");
        // while (!_cancellationToken.Token.IsCancellationRequested)
        // {
        //     var input = Console.ReadLine();
        //     if (input != null) _ = console.SendMessage(input).ConfigureAwait(false);
        // }\

        Instance = null!;
    }

    private void DefaultCommands()
    {
        Debug.Assert(_bot != null, nameof(_bot) + " != null");
        commands.RegisterCommand("stop", async (args, response) =>
        {
            await _bot.Disconnect();
            Kill();
            if (args.Length == 1) Console.WriteLine("exiting application");
            else response("exiting application " + string.Join(' ', args.Split(' ').Skip(1)));
        });
        commands.RegisterCommand("crash", async (args, response) =>
        {
            await _bot.Disconnect();
            Kill();
            if (args.Length == 1) Console.WriteLine("application crashed");
            else response("application crashed, reason: " + string.Join(' ', args.Split(' ').Skip(1)));
        });
        commands.RegisterCommand("reconnect", async (args, response) =>
        {
            response("disconnecting");
            await _bot.Disconnect();
            await _bot.Connect();
            response("reconnected");
        });
        commands.RegisterCommand("reload", (_, response) =>
        {
            _bot.UnregisterPlugins();
            commands.Clear();
            DefaultCommands();
            _bot = Utils.CreateNyanBot(_bot);
            _bot.RegisterPlugins();
            response("reloading plugins :" + _bot.Plugins.Length + " plugins loaded");
            return Task.CompletedTask;
        });
        commands.RegisterCommand("help", (_, response) =>
        {
            var sb = new StringBuilder();
            sb.AppendLine("Available commands (" + commands.commandDic.Count + "):");
            foreach (var command in commands.commandDic)
            {
                sb.AppendLine(command.Key);
            }

            response(sb.ToString());
            return Task.CompletedTask;
        });
    }
}