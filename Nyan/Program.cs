using System.Diagnostics;
using System.Text;

// ReSharper disable MemberCanBePrivate.Global

namespace Nyan;

public class ConsoleApplication
{
    private bool _isRunning;
    private NyanBot? _bot;
    public readonly DynamicCommands commands;

    public ConsoleApplication()
    {
        _isRunning = true;
        commands = new DynamicCommands();
    }

    public static ConsoleApplication? Instance { get; private set; }

    public static async Task Main(string[] args)
    {
        try
        {
            Instance = new ConsoleApplication();
            await Instance.Run();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private async Task Run()
    {
        _bot = await Utils.CreateNyanBot();

        DefaultCommands();
        Debug.Assert(_bot != null, nameof(_bot) + " != null");
        _ = _bot.Connect().ConfigureAwait(false);
        _ = _bot.RegisterPlugins().ConfigureAwait(false);
        while (_isRunning)
        {
            var input = Console.ReadLine();
            if (input != null) await commands.CallCommand(input);
        }
    }

    public void DefaultCommands()
    {
        commands.Response += Console.WriteLine;
        Debug.Assert(_bot != null, nameof(_bot) + " != null");
        commands.RegisterCommand("stop", async (args, response) =>
        {
            await _bot.Disconnect();
            _isRunning = false;
            if (args.Length == 1) Console.WriteLine("exiting application");
            else response("exiting application " + string.Join(' ', args.Split(' ').Skip(1)));
        });
        commands.RegisterCommand("crash", async (args, response) =>
        {
            await _bot.Disconnect();
            _isRunning = false;
            if (args.Length == 1) Console.WriteLine("application crashed");
            else response("application crashed, reason: " + string.Join(' ', args.Split(' ').Skip(1)));
        });
        commands.RegisterCommand("reconnect", async (args, response) =>
        {
            response("disconnecting");
            await _bot.Disconnect();
            _ = _bot.Connect().ConfigureAwait(false);
            response("reconnected");
        });
        commands.RegisterCommand("reload", async (_, response) =>
        {
            await _bot.UnregisterPlugins();
            commands.Clear();
            DefaultCommands();
            _bot = Utils.CreateNyanBot(_bot);
            await _bot.RegisterPlugins();
            response("reloading plugins :" + _bot.Plugins.Length + " plugins loaded");
        });
        commands.RegisterCommand("help", (_, response) =>
        {
            var sb = new StringBuilder();
            sb.AppendLine("Available commands (" + commands.commandDic.Count + "):");
            foreach (var command in commands.commandDic)
            {
                sb.AppendLine($"{command.Key}");
            }

            response(sb.ToString());
            return Task.CompletedTask;
        });
    }
}