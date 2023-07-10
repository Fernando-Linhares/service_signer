using System.Net;

namespace Service.Network.Server;

public class WebsocketServer
{
    public void BootServer()
    {
         var config = new Credentials();

        var listener = new HttpListener();

        listener.Prefixes.Add(config.Prefix);

        listener.Start();

        Console.WriteLine($"Server is Running in - {config.Prefix} | {DateTime.UtcNow.ToString("MM-dd-yyy H:mm:ss")}");

        NonBreakableLoop(listener).Wait();
    }

    public void Listen()
    {
        BootServer();
    }

    public async Task NonBreakableLoop(HttpListener listener)
    {
        var context = await listener.GetContextAsync();

        if(context.Request.IsWebSocketRequest)
        {
            await ProcessWebSocketRequest(context);
        }

        await NonBreakableLoop(listener);
    }

    private async Task ProcessWebSocketRequest(HttpListenerContext context)
    {
        var router = new RouterProvider();

        router.UseContext(context);

        await router.Dispatch();
    }
}