// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

using Nyan.Types;

namespace Nyan.Plugins;

public abstract class NyanPlugin
{
    #region Constructors

    protected NyanPlugin(string id)
    {
        Id = id;
    }

    #endregion

    #region Properties

    /// <summary>
    /// ID of the plugin
    /// It is used to identify the plugin. <b>They must be unique.</b>
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The name of the plugin
    /// </summary>
    public string Name { get; init; } = "Unnamed Plugin";

    /// <summary>
    /// The description of the plugin
    /// </summary>
    public string Description { get; init; } = "No description provided";

    /// <summary>
    /// Website to the plugin or the author
    /// (Optional)
    /// </summary>
    public string Website { get; init; } = "";

    /// <summary>
    /// Link to the plugin's image
    /// (Optional)
    /// </summary>
    public string ImageUrl { get; init; } = "";

    /// <summary>
    /// Author of the plugin
    /// </summary>
    public string Author { get; init; } = "Unknown";

    /// <summary>
    /// GitHub repository of the plugin
    /// (Optional)
    /// </summary>
    public string GitHub { get; init; } = "";

    /// <summary>
    /// The first part of the version of the plugin
    /// Please set the <see cref="ReleaseVersion"/> property as it is the second part of the version
    /// Or use the <see cref="Version"/> property instead
    /// </summary>
    public Version VersionObj { get; init; } = new(0, 0, 0, 0);

    /// <summary>
    /// The second part of the version of the plugin
    /// Please set the <see cref="VersionObj"/> property as it is the first part of the version
    /// Or use the <see cref="Version"/> property instead
    /// </summary>
    public ReleaseVersion ReleaseVersion { get; init; } = ReleaseVersion.Development;

    /// <summary>
    /// The version of the plugin
    /// Use <b>Either</b> this <b>or</b> both <see cref="VersionObj"/> and <see cref="ReleaseVersion"/> to set the version
    /// </summary>
    public string Version
    {
        get => $"{VersionObj} ({ReleaseVersion})";
        init
        {
            var parts = value.Split(' ', 2);
            VersionObj = new Version(parts[0]);
            ReleaseVersion = Utils.ParseVersion(parts[1]);
        }
    }

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

    internal virtual async Task Register(NyanBot nyanBot) => await OnRegister(nyanBot);
    internal virtual async Task Finalise(NyanBot nyanBot) => await OnFinalise(nyanBot);
    internal virtual async Task Unregister(NyanBot nyanBot) => await OnUnregister(nyanBot);

    #endregion
}