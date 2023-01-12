using Nyan.Types;

namespace Nyan.Plugins;

public interface IPluginDescription
{
    /// <summary>
    /// ID of the plugin
    /// It is used to identify the plugin. <b>They must be unique.</b>
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The name of the plugin
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The description of the plugin
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// The version of the plugin
    /// </summary>
    public Versioning Version { get; }
    
    /// <summary>
    /// Author of the plugin
    /// </summary>
    public string Author { get; }

    /// <summary>
    /// Link to the plugin's image
    /// (Optional)
    /// </summary>
    public string ImageUrl { get; }
    
    public Links Links { get; }
}