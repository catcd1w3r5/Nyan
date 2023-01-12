using Nyan.Misc;

try
{
    await BotInstanceUtils.RunAsync(new DefaultManager());
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();