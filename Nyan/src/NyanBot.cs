using System.Collections.Immutable;
using System.Reflection;
using DSharpPlus;
using Nyan.Plugins;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Nyan;

public sealed partial class NyanBot : IDisposable
{
    public NyanBot()
    {
        Client = new DiscordClient(new DiscordConfiguration
        {
            Token = GetToken(),
            TokenType = TokenType.Bot
        });
        Plugins = GetPlugins();
    }

    public NyanBot(NyanBot old)
    {
        Client = old.Client;
        Plugins = GetPlugins();
    }

    public ImmutableArray<NyanPlugin> Plugins { get; }

#pragma warning disable CS8618
    public DiscordClient Client { get; }
#pragma warning restore CS8618
    public Task Connect() => Client.ConnectAsync();

    public Task Disconnect() => Client.DisconnectAsync();

    public bool TryGetPlugin<T>(out T plugin) where T : NyanPlugin
    {
#pragma warning disable CS8601
        plugin = Plugins.FirstOrDefault(x => x is T) as T;
#pragma warning restore CS8601
        return plugin != null;
    }


    public void Dispose()
    {
        Client.Dispose();
        Disconnect();
        UnregisterPlugins();
    }
}

public sealed partial class NyanBot
{
    private static string GetToken()
    {
        if (!File.Exists("token.config")) File.Create("token.config");
        using var reader = new StreamReader("token.config");
        var token = reader.ReadLine();
        if (token == null) throw new ArgumentNullException(nameof(token));
        return token;
    }

    private static ImmutableArray<NyanPlugin> GetPlugins()
    {
        if (!Directory.Exists(".\\Plugins")) Directory.CreateDirectory(".\\Plugins");

        var pluginFiles = Directory.GetFiles(".\\Plugins", "*.dll");
        var pluginAssemblies = pluginFiles
            .Select(path =>
            {
                try
                {
                    return Assembly.Load(File.ReadAllBytes(path));
                }
                catch (Exception)
                {
                    // ignored
                }

                try
                {
                    return Assembly.Load(AssemblyName.GetAssemblyName(path));
                }
                catch (Exception)
                {
                    // ignored
                }

                try
                {
                    return Assembly.LoadFrom(path);
                }
                catch (Exception)
                {
                    // ignored
                }

                Console.Out.WriteLine($"Failed to load plugin | Path: {path}");
                return null;
            })
            .Where(x => x != null)
            .Select(x => x!)
            .ToList();

        pluginAssemblies.Add(Assembly.GetExecutingAssembly());
        return Utils.GetAllOfType<NyanPlugin>(pluginAssemblies).ToImmutableArray();
    }

    #region Plugin Management

    internal void RegisterPlugins()
    {
        //sort plugins by dependencies
        var registeredPluginsTypes = new List<Type>();
        var unregisteredPlugins = Plugins
            .Select(plugin =>
            {
                var dependencies = plugin
                    .GetType()
                    .GetInterfaces()
                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequirePlugin<>))
                    .Select(x => x.GetGenericArguments()[0])
                    .ToArray();
                return new Tuple<NyanPlugin, Type[]>(plugin, dependencies);
            })
            .ToList();
        //sort plugins by number of dependencies (least to most)
        unregisteredPlugins.Sort((x, y) => x.Item2.Length.CompareTo(y.Item2.Length));

        while (unregisteredPlugins.Count > 0)
        {
            var progress = false;
            for (var index = 0; index < unregisteredPlugins.Count; index++)
            {
                var plugin = unregisteredPlugins[index];
                var (nyanPlugin, dependencies) = plugin;
                var unregisteredDeps = dependencies.Count(x => !registeredPluginsTypes.Contains(x));
                if (unregisteredDeps != 0) continue;
                using var task = nyanPlugin.Register(this);
                //load plugins synchronously for now
                task.Wait();
                registeredPluginsTypes.Add(nyanPlugin.GetType());
                unregisteredPlugins.Remove(plugin);
                progress = true;
            }

            if (progress) continue;
            //check if circular dependency or missing dependency
            var missingDeps = unregisteredPlugins
                .SelectMany(x => x.Item2)
                .Where(x => !registeredPluginsTypes.Contains(x))
                .Select(x => x.Name)
                .ToArray();

            if (missingDeps.Length == 0) throw new("Circular dependency detected");
            throw new Exception($"Missing dependencies: {string.Join(", ", missingDeps)}");
        }

        Task.WhenAll(Plugins.Select(plugin => plugin.Finalise(this))).Wait();
    }

    internal void UnregisterPlugins() => Task.WhenAll(Plugins.Select(plugin => plugin.Unregister(this))).Wait();

    #endregion
}