// ReSharper disable MemberCanBePrivate.Global

namespace Nyan.Plugins;

/**
 * Only use this class if to make your plugin a Singleton.
 * Do not use it if not needed.
 */
public abstract class NyanPluginSingleton<T> : NyanPlugin where T : NyanPluginSingleton<T>
{
    #region Singleton

    private static T? _instance;

    /// <summary>
    /// Instance of the plugin.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static T Instance => _instance ?? throw new InvalidOperationException();

    /// <summary>
    ///  Called when the plugin is loaded.
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="InvalidOperationException"></exception>
    /// 

    #endregion

    #region Constructor

    protected NyanPluginSingleton(string id) : base(id)
    {
        if (Instance != null)
            throw new InvalidOperationException("Cannot create multiple instances of a singleton plugin.");
    }

    #endregion

    #region Internal Methods

    /// <summary>
    ///  Called when the plugin is loaded.
    /// </summary>
    /// <param name="nyanBot"></param>
    /// <returns></returns>
    internal override Task Register(NyanBot nyanBot)
    {
        _instance = (T) this;
        return base.Register(nyanBot);
    }

    /// <summary>
    /// Called when the plugin is unloaded.
    /// </summary>
    /// <param name="nyanBot"></param>
    /// <returns></returns>
    internal override Task Unregister(NyanBot nyanBot)
    {
        _instance = null;
        return base.Unregister(nyanBot);
    }

    #endregion
}