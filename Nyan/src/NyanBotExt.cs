using Nyan.Plugins;

namespace Nyan;

public static class NyanBotExt
{
    public static bool TryGetPlugin<T>(this NyanBot bot, out T plugin) where T : NyanPlugin
    {
        foreach (var p in bot.Plugins)
        {
            if (p is not T nyanPlugin) continue;
            plugin = nyanPlugin;
            return true;
        }

        plugin = default!;
        return false;
    }

    public static ReadOnlySpan<T> GetPlugins<T>(this NyanBot bot) where T : NyanPlugin
    {
        var plugins = bot.Plugins;
        var result = new T[plugins.Length];
        var i = 0;
        foreach (var p in plugins)
        {
            if (p is not T nyanPlugin) continue;
            result[i++] = nyanPlugin;
        }

        return result.AsSpan(0, i);
    }

    public static Task Connect(this NyanBot bot) => bot.Client.ConnectAsync();

    public static Task Disconnect(this NyanBot bot) => bot.Client.DisconnectAsync();
}