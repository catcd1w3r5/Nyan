// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Nyan;

public abstract class NyanPlugin
{
    public string ID { get; init; } = "com.nyan.plugin";
    public string Name { get; init; } = "Unnamed Plugin";
    public string Description { get;init; } = "No description provided";
    public string Author { get;init; } = "Unknown";
    public Version Version { get;init; } = new Version(0, 0, 0, 0);
    public ReleaseVersion ReleaseVersion { get; init; } = ReleaseVersion.Alpha;
    public string VersionString => $"{Version} ({ReleaseVersion})";  

    protected virtual Task OnRegister(NyanBot nyanBot) => Task.CompletedTask;
    protected virtual Task OnUnregister(NyanBot nyanBot) => Task.CompletedTask;

    internal async Task Register(NyanBot nyanBot) => await OnRegister(nyanBot);

    internal async Task Unregister(NyanBot nyanBot) => await OnUnregister(nyanBot);
}