using System.Net;
using System.Net.WebSockets;
using System.Threading;

namespace Service.Network.Server;

public class WebsocketServer
{
    public async Task BootServer()
    {
        var config = new Credentials();

        var definition = new DefinitionLayer();

        await definition.StartAsync();
    
        Console.WriteLine($"Server Listen - {config.Prefix} | {DateTime.UtcNow.ToString("MM-dd-yyy H:mm:ss")}");

        await BootServer();
    }

    public void Listen()
    {
        BootServer().Wait();
    }
}