namespace Nyan.Misc;

internal sealed class DefaultManager : BotManager
{
    private readonly CancellationTokenSource _cancellationToken = new();

    protected override void Stop()
    {
        _cancellationToken.Cancel();
    }

    protected override async Task Run()
    {
        this.RegisterDefaultCommands();
        _ = Bot.Connect().ConfigureAwait(false);
        Bot.RegisterPlugins();
        await Task.Delay(-1, _cancellationToken.Token);
    }
}