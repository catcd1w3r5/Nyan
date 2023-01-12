using System.Reflection;
using Nyan.Types;

namespace Nyan;

internal static class Utils
{
    internal static Memory<T> GetAllOfType<T>(List<Assembly> assemblies)
    {
        var definitionsInstances = new List<T>();
        foreach (var definitions in assemblies.Select(assembly => assembly
                     .GetTypes()
                     .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsAbstract)
                     .Distinct()
                     .Select(Activator.CreateInstance)
                     .Cast<T>()
                     .Where(instance => instance != null)))
        {
            definitionsInstances.AddRange(definitions);
        }

        var memory = new Memory<T>(definitionsInstances.ToArray());

        return memory;
    }

    internal static ReleaseVersion ParseVersion(string version)
    {
        //change remove - _ ' ' and .
        var versionString = version
            .Replace("-", "")
            .Replace("_", "")
            .Replace(" ", "")
            .Replace(".", "");
        switch (version.ToLower())
        {
            case "a":
            case "alpha":
                return ReleaseVersion.Alpha;
            case "b":
            case "beta":
                return ReleaseVersion.Beta;
            case "":
            case "release":
            case "final":
                return ReleaseVersion.Release;
            case "d":
            case "dev":
            case "development":
                return ReleaseVersion.Development;
            case "rc":
            // ReSharper disable once StringLiteralTypo
            case "releasecandidate":
                return ReleaseVersion.ReleaseCandidate;
        }

        throw new ArgumentException("Invalid version");
    }

    public static void RegisterDefaultCommands(this BotManager botManager)
    {
        var commands = botManager.commands;
        var bot = botManager.Bot;
        commands.RegisterCommand("stop", (args, response) =>
        {
            bot.Disconnect();
            botManager.Stop_Internal();
            if (args.Length == 1) Console.WriteLine("exiting application");
            else response("exiting application " + args.ToString());
            return Task.CompletedTask;
        });
        commands.RegisterCommand("crash", (args, response) =>
        {
            bot.Disconnect();
            botManager.Stop_Internal();
            if (args.Length == 1) Console.WriteLine("application crashed");
            else response("application crashed, reason: " + args.ToString());
            return Task.CompletedTask;
        });
        commands.RegisterCommand("reconnect", (_, response) =>
        {
            response("disconnecting");
            bot.Disconnect();
            bot.Connect();
            response("reconnected");
            return Task.CompletedTask;
        });
        commands.RegisterCommand("reload", (_, response) =>
        {
            bot.UnregisterPlugins();
            commands.UnregisterAllCommands();
            commands.RemoveAllOutput();
            botManager.RegisterDefaultCommands();
            bot = new NyanBot(bot);
            bot.RegisterPlugins();
            botManager.Bot = bot;
            response("reloading plugins :" + bot.Plugins.Length + " plugins loaded");
            return Task.CompletedTask;
        });
    }
}