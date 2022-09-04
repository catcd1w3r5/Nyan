using Nyan;

try
{
    await new NyanBotInstance().Run();
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();