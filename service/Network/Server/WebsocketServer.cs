using System.Net;
using System.Net.WebSockets;
using System.Threading;

namespace Service.Network.Server;

public class WebsocketServer
{
    public async Task BootServer()
    {
        var config = new Credentials();

        var listener = new HttpListener();

        listener.Prefixes.Add(config.Prefix);

        listener.Start();

        Console.WriteLine($"Server Listen - {config.Prefix} | {DateTime.UtcNow.ToString("MM-dd-yyy H:mm:ss")}");

        await RequestExecute(listener);

        listener.Abort();

        await BootServer();
    }

    public void Listen()
    {
        BootServer().Wait();
    }

    public async Task RequestExecute(HttpListener listener)
    {
        var context = await listener.GetContextAsync();

        if(context.Request.IsWebSocketRequest)
        {
            await ProcessWebSocketRequest(context);
        }
    }

    private async Task ProcessWebSocketRequest(HttpListenerContext context)
    {
        IRouter router = new RouteLayer();

        router.UseContext(context);

        await router.Dispatch();
    }
}