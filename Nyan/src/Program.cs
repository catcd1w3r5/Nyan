using Nyan.Misc;

try
{
    await BotInstanceUtils.RunAsync(new DefaultInstance());
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();