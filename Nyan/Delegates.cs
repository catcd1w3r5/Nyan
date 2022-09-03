namespace Nyan;

public delegate Task Command(string args, Response response);
public delegate void Response(string args);