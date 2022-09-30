namespace Nyan.Types;

public interface IStringCommands
{
    #region Input

    public Task RegisterCommand(ReadOnlySpan<char> command, Command? callback);
    public Task CallCommand(ReadOnlySpan<char> command, ReadOnlySpan<char> args);
    public Task UnregisterCommand(ReadOnlySpan<char> command);
    public Task UnregisterAllCommands();

    #endregion

    #region Output

    public Task RegisterOutput(Response response);
    public Task RemoveOutput(Response response);
    public Task RemoveAllOutput();

    #endregion

    #region Misc
    public int GetCommandCount();
    public void InitializeCommands();

    #endregion
}