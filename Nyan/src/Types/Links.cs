namespace Nyan.Types;

public readonly struct Links
{
    public Links()
    {
    }

    private Dictionary<string,string> LinksDict { get; init; } = new();
    
    public string this[string websiteName]
    {
        get => LinksDict[websiteName];
        init => LinksDict[websiteName] = value;
    }
}