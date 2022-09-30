namespace Nyan.Plugins;
/// <summary>
/// A interface that tells the plugin loader that this plugin depends on another plugin.
/// </summary>
/// <typeparam name="T">The other plugin</typeparam>
public interface IRequirePlugin<T> where T : NyanPlugin
{
}