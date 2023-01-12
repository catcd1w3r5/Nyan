namespace Nyan.Types;

public readonly struct Versioning
{
    public Versioning()
    {
    }

    public Version VersionObj { get; init; } = new(0, 0, 0, 0);
    public ReleaseVersion ReleaseVersion { get; init; } = ReleaseVersion.Development;

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

    public override string ToString() => Version;

    public static implicit operator Versioning(string version) => new() {Version = version};

    public static implicit operator string(Versioning version) => version.Version;

    public static implicit operator Version(Versioning version) => version.VersionObj;

    public static implicit operator ReleaseVersion(Versioning version) => version.ReleaseVersion;
}