// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Diagnostics;

namespace Nyan;

public abstract class NyanPlugin
{
    protected NyanPlugin(string id)
    {
        Id = id;
    }

    public string Id { get; }
    public string Name { get; init; } = "Unnamed Plugin";
    public string Description { get; init; } = "No description provided";
    public string Website { get; init; } = "";
    public string ImageUrl { get; init; } = "";
    public string Author { get; init; } = "Unknown";
    public string GitHub { get; init; } = "";
    public Version VersionObj { get; init; } = new(0, 0, 0, 0);
    public ReleaseVersion ReleaseVersion { get; init; } = ReleaseVersion.Development;

    public string Version
    {
        get => $"{VersionObj} ({ReleaseVersion})";
        init
        {
            //spilt at first space
            var parts = value.Split(' ', 2);
            VersionObj = new Version(parts[0]);
            switch (parts[1].ToLower())
            {
                case "a":
                case "alpha":
                    ReleaseVersion = ReleaseVersion.Alpha;
                    break;
                case "b":
                case "beta":
                    ReleaseVersion = ReleaseVersion.Beta;
                    break;
                case "":
                case "release":
                case "final":
                    ReleaseVersion = ReleaseVersion.Release;
                    break;
                case "d":
                case "dev":
                case "development":
                    ReleaseVersion = ReleaseVersion.Development;
                    break;
                case "rc":
                case "r-c":
                case "release-candidate":
                // ReSharper disable once StringLiteralTypo
                case "releasecandidate":
                case "release candidate":
                    ReleaseVersion = ReleaseVersion.ReleaseCandidate;
                    break;
            }
        }
    }

    /**
     * This is the main entry point for your plugin.
     * It is called when the plugin is loaded.
     * All other plugins listed in the dependencies will be loaded before this is called.
     */
    protected virtual Task OnRegister(NyanBot nyanBot) => Task.CompletedTask;

    /**
     * This is called when all plugins have been loaded.
     * This is the last step before the bot is ready.
     * This is the best place to do anything involving optional dependencies.
     */
    protected virtual Task OnFinalise(NyanBot nyanBot) => Task.CompletedTask;

    /**
     * This is called when the bot is unregistered,
     * This is called when plugins are getting reloaded or when the bot is shutting down.
     */
    protected virtual Task OnUnregister(NyanBot nyanBot) => Task.CompletedTask;

    internal virtual async  Task Register(NyanBot nyanBot) => await OnRegister(nyanBot);
    internal virtual async Task Finalise(NyanBot nyanBot) => await OnFinalise(nyanBot);
    internal virtual async Task Unregister(NyanBot nyanBot) => await OnUnregister(nyanBot);
}