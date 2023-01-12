using System.Reflection;
using Nyan.Plugins;

namespace Nyan;

internal static class NyanBotPluginUtil
{
    private const string TokenFile = "token.config";
    private const string PluginPath = ".\\Plugins";


    internal static string GetToken()
    {
        if (!File.Exists(TokenFile)) File.Create(TokenFile);
        using var reader = new StreamReader(TokenFile);
        var token = reader.ReadLine();
        if (token == null) throw new ArgumentNullException(nameof(token));
        return token;
    }

    internal static Memory<NyanPlugin> GetPlugins()
    {
        if (!Directory.Exists(PluginPath)) Directory.CreateDirectory(PluginPath);

        var pluginFiles = Directory.GetFiles(PluginPath, "*.dll");
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
                    Console.WriteLine("ReadBytesFailed");
                }

                try
                {
                    return Assembly.Load(AssemblyName.GetAssemblyName(path));
                }
                catch (Exception)
                {
                    // ignored
                    Console.WriteLine("GetAssemblyNameFailed");
                }

                try
                {
                    return Assembly.LoadFrom(path);
                }
                catch (Exception)
                {
                    // ignored
                    Console.WriteLine("LoadFromFailed");
                }

                Console.Out.WriteLine($"Failed to load plugin | Path: {path}");
                return null;
            })
            .Where(x => x != null)
            .Select(x => x!)
            .ToList();

        pluginAssemblies.Add(Assembly.GetExecutingAssembly());
        return Utils.GetAllOfType<NyanPlugin>(pluginAssemblies);
    }

    #region Plugin Management

    internal static Task RegisterPlugins(this NyanBot bot)
    {
        var plugins = bot.Plugins.ToArray();
        //sort plugins by dependencies
        var registeredPluginsTypes = new List<Type>(); 
        var unregisteredPlugins = plugins
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
                using var task = nyanPlugin.Register(bot);
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

        var tasks = new Task[plugins.Length];
        for (var index = 0; index < plugins.Length; index++)
        {
            tasks[index] = plugins[index].Finalise(bot);
        }

        return Task.WhenAll(tasks);
    }

    internal static Task UnregisterPlugins(this NyanBot bot)
    {
        var plugins = bot.Plugins;
        var tasks = new Task[plugins.Length];
        for (var index = 0; index < plugins.Length; index++)
        {
            tasks[index] = plugins[index].Unregister(bot);
        }

        return Task.WhenAll(tasks);
    }

    #endregion
}