// ReSharper disable MemberCanBePrivate.Global

namespace Nyan;

/**
 * Only use this class if to make your plugin a Singleton.
 * Do not use it if not needed.
 */
public abstract class NyanPluginSingleton<T> : NyanPlugin where T : NyanPluginSingleton<T>
{
    private static T? _instance;

    public static T Instance => _instance ?? throw new InvalidOperationException();

    protected NyanPluginSingleton(string id) : base(id)
    {
        if (Instance != null)
            throw new InvalidOperationException("Cannot create multiple instances of a singleton plugin.");
    }

    internal override Task Register(NyanBot nyanBot)
    {
        _instance = (T) this;
        return base.Register(nyanBot);
    }

    internal override Task Unregister(NyanBot nyanBot)
    {
        _instance = null;
        return base.Unregister(nyanBot);
    }
}