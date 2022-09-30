using System.Reflection;
using Nyan.Types;

namespace Nyan;

internal static class Utils
{
    internal static List<T> GetAllOfType<T>(List<Assembly> assemblies)
    {
        var definitionsInstances = new List<T>();
        foreach (var assembly in assemblies)
        {
            //lots of allocations here, but it's not supposed to run frequently
            var definitions = assembly
                .GetTypes()
                .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsAbstract)
                .Distinct()
                .Select(Activator.CreateInstance)
                .Cast<T>()
                .Where(instance => instance != null);
            definitionsInstances.AddRange(definitions);
        }

        return definitionsInstances;
    }

    internal static ReleaseVersion ParseVersion(string version)
    {
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
            case "r-c":
            case "release-candidate":
            // ReSharper disable once StringLiteralTypo
            case "releasecandidate":
            case "release candidate":
                return ReleaseVersion.ReleaseCandidate;
        }

        throw new ArgumentException("Invalid version");
    }

    public static void RegisterDefaultCommands(this BotInstance botInstance)
    {
        var commands = botInstance.commands;
        var bot = botInstance.Bot;
        commands.RegisterCommand("stop", (args, response) =>
        {
            bot.Disconnect();
            botInstance.Stop_Internal();
            if (args.Length == 1) Console.WriteLine("exiting application");
            else response("exiting application " + args.ToString());
            return Task.CompletedTask;
        });
        commands.RegisterCommand("crash", (args, response) =>
        {
            bot.Disconnect();
            botInstance.Stop_Internal();
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
            botInstance.RegisterDefaultCommands();
            bot = new NyanBot(bot);
            bot.RegisterPlugins();
            botInstance.Bot = bot;
            response("reloading plugins :" + bot.Plugins.Length + " plugins loaded");
            return Task.CompletedTask;
        });
    }
}