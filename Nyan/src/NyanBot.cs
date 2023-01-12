using DSharpPlus;
using Nyan.Plugins;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Nyan;

public sealed class NyanBot
{
    private readonly Memory<NyanPlugin> _plugins;

    public DiscordClient Client { get; }
    public ReadOnlySpan<NyanPlugin> Plugins => _plugins.Span;

    public NyanBot()
    {
        Client = new DiscordClient(new DiscordConfiguration
        {
            Token = NyanBotPluginUtil.GetToken(),
            TokenType = TokenType.Bot
        });

        _plugins = NyanBotPluginUtil.GetPlugins();
    }

    public NyanBot(NyanBot old)
    {
        Client = old.Client;
        _plugins = NyanBotPluginUtil.GetPlugins();
    }

    public void Dispose()
    {
        Client.Dispose();
        this.Disconnect();
        this.UnregisterPlugins().Wait();
    }
}