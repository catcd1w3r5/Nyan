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

    internal Task RegisterPlugins() => Task.WhenAll(Plugins.Select(plugin => plugin.Register(this)));
    internal Task UnregisterPlugins() => Task.WhenAll(Plugins.Select(plugin => plugin.Unregister(this)));

    internal Task Connect() => Client.ConnectAsync();

    public Task Disconnect() => Client.DisconnectAsync();
}