using System.Collections.Immutable;
using DSharpPlus;

// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable MemberCanBePrivate.Global

namespace Nyan;

public sealed class NyanBot
{
    public ImmutableArray<NyanPlugin> Plugins { get; init; }

#pragma warning disable CS8618
    public DiscordClient Client { get; init; }
#pragma warning restore CS8618

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
            var circularDependency = true;
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
                circularDependency = false;
            }

            if (circularDependency)
                throw new Exception("Circular dependency in plugins detected!");
        }
        
        Task.WhenAll(Plugins.Select(plugin => plugin.Finalise(this))).Wait();
    }

    internal void UnregisterPlugins() => Task.WhenAll(Plugins.Select(plugin => plugin.Unregister(this))).Wait();

    public Task Connect() => Client.ConnectAsync();

    public Task Disconnect() => Client.DisconnectAsync();

    public bool TryGetPlugin<T>(out T plugin) where T : NyanPlugin
    {
#pragma warning disable CS8601
        plugin = Plugins.FirstOrDefault(x => x is T) as T;
#pragma warning restore CS8601
        return plugin != null;
    }
}