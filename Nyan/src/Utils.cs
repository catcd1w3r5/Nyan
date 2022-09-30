using System.Collections.Immutable;
using System.Reflection;
using DSharpPlus;

namespace Nyan;

internal static class Utils
{
    private static List<T> GetAllOfType<T>(IEnumerable<Assembly> assemblies)
    {
        var definitionsInstances = new List<T>();
        foreach (var assembly in assemblies)
        {
            //lots of allocations here, but it's not supposed to run frequently
            var definitions = assembly
                .GetTypes()
                .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<T>()
                .Where(instance => instance != null);
            definitionsInstances.AddRange(definitions);
        }

        return definitionsInstances;
    }

    internal static async Task<NyanBot?> CreateNyanBot()
    {
        var client = new DiscordClient(new DiscordConfiguration
        {
            Token = await GetToken(),
            TokenType = TokenType.Bot
        });
        return new NyanBot
        {
            Client = client,
            Plugins = GetPlugins()
        };
    }

    internal static NyanBot CreateNyanBot(NyanBot old) => new()
    {
        Client = old.Client,
        Plugins = GetPlugins()
    };

    private static async Task<string> GetToken()
    {
        if (!File.Exists("token.config")) File.Create("token.config");
        using var reader = new StreamReader("token.config");
        var token = await reader.ReadLineAsync();
        if (token == null) throw new ArgumentNullException(nameof(token));
        return token;
    }

    private static ImmutableArray<NyanPlugin> GetPlugins()
    {
        if (!Directory.Exists(".\\Plugins")) Directory.CreateDirectory(".\\Plugins");

        var pluginFiles = Directory.GetFiles(".\\Plugins", "*.dll");
        var pluginAssemblies = pluginFiles.Select(Assembly.LoadFrom).ToList();
        pluginAssemblies.Add(Assembly.GetExecutingAssembly());
        return GetAllOfType<NyanPlugin>(pluginAssemblies).ToImmutableArray();
    }
}