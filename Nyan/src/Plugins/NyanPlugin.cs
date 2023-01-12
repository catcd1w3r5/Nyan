// ReSharper disable VirtualMemberNeverOverridden.Global

using Nyan.Types;

namespace Nyan.Plugins;

public abstract class NyanPlugin : IPluginDescription
{
    #region Constructors

    protected NyanPlugin(string id)
    {
        Id = id;
    }

    #endregion

    #region Properties

    public string Id { get; }
    public string Name { get; init; } = "Unnamed Plugin";
    public string Description { get; init; } = "No description provided";
    public string Author { get; init; } = "Unknown";
    public Versioning Version { get; init; } = "0.0.0.0 dev";
    public string ImageUrl { get; init; } = "";
    public Links Links { get; init; }

    #endregion

    #region Methods

    /// <summary>
    /// This is the main entry point for your plugin.
    /// It is called when the plugin is loaded.
    /// All other plugins listed in the dependencies will be loaded before this is called.
    /// </summary>
    protected virtual Task OnRegister(NyanBot nyanBot) => Task.CompletedTask;

    /// <summary>
    /// This is called when all plugins have been loaded.
    /// This is the last step before the bot is ready.
    /// This is the best place to do anything involving optional dependencies.
    /// </summary>
    protected virtual Task OnFinalise(NyanBot nyanBot) => Task.CompletedTask;

    /// <summary>
    /// This is called when the bot is unregistered,
    /// This is called when plugins are getting reloaded or when the bot is shutting down.
    /// </summary>
    protected virtual Task OnUnregister(NyanBot nyanBot) => Task.CompletedTask;

    #endregion

    #region Internal Methods

    /// <summary>
    /// This is called in the first step of the plugin loading process.
    /// </summary>
    /// <param name="nyanBot"></param>
    internal virtual async Task Register(NyanBot nyanBot) => await OnRegister(nyanBot);
    
    /// <summary>
    /// This is called in the final step of the plugin loading process.
    /// </summary>
    /// <param name="nyanBot"></param>
    internal virtual async Task Finalise(NyanBot nyanBot) => await OnFinalise(nyanBot);
    
    /// <summary>
    /// This is called when the plugin is unloaded.
    /// </summary>
    /// <param name="nyanBot"></param>
    internal virtual async Task Unregister(NyanBot nyanBot) => await OnUnregister(nyanBot);

    #endregion
}