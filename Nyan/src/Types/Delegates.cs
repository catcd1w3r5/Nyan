namespace Nyan.Types;

public delegate Task Command(ReadOnlySpan<char>  args, Response response);
public delegate void Response(ReadOnlySpan<char>  args);