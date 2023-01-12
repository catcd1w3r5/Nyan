namespace Nyan.Misc;

public static class BotInstanceUtils
{
    public static void Run(BotManager manager) => manager.Run_Internal().GetAwaiter().GetResult();

    public static Task RunAsync(BotManager manager) => manager.Run_Internal();
    
    public static void Stop(BotManager manager) => manager.Stop_Internal();
}