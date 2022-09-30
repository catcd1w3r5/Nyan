namespace Nyan.Misc;

public static class BotInstanceUtils
{
    public static void Run(BotInstance instance) => instance.Run_Internal().GetAwaiter().GetResult();

    public static Task RunAsync(BotInstance instance) => instance.Run_Internal();
    
    public static void Stop(BotInstance instance) => instance.Stop_Internal();
}