using System.Net.WebSockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Service.Network.Server;

public class RouteLayer: IRouter
{
    private HttpListenerContext _context;

    private Logger _logger;

    private readonly Env _env = new Env();

    public int BufferSize { get; set; }

    public void UseContext(HttpListenerContext context)
    {
        _context = context;

        string logPath = _env.Get("LOGS_PATH") ?? "";

        BufferSize = 14969112;// int.Parse(_env.Get("LIMIT_SIZE_BYTES_REQUEST"));

        _logger = new Logger(logPath);
    }

    public async Task Dispatch()
    {
        var env = new Env();

        HttpListenerWebSocketContext webSocketContext = await _context.AcceptWebSocketAsync(null);

        WebSocket webSocket = webSocketContext.WebSocket;

        byte[] inputBuffer = new byte[1000];

        try
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(inputBuffer),  CancellationToken.None);

            while (webSocket.State == WebSocketState.Open)
            {
                var routes = new Routes();

                var uri = webSocketContext.RequestUri.ToString();

                var controllers = (
                    from c in routes.EnabledControllers
                    where c.GetType().Name == UriFromControllerName(uri)
                    select c
                ).ToList();

                if(controllers.Count > 0)
                {
                    var methodUri = UriFromMethodName(uri);

                    var methods = controllers[0].GetType().GetMethods().ToList();

                    var methodMatch = (
                        from m in methods
                        where m.Name == methodUri
                        select m
                    ).ToList();

                    if(methodMatch.Count > 0)
                    {
                        var controller = controllers[0];

                        var form = await Message(webSocket);

                        controller.Form = form;

                        var response = await (Task<Response>?) methodMatch[0].Invoke(controller, null);

                        var responseContent = response.ToString();

                        byte[] responseBytes = Encoding.UTF8.GetBytes(responseContent);

                        Console.WriteLine(responseContent);
                        await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                        _logger.Write($"{DateTime.Now.ToString("MM-dd-yyyy H:mm:ss")}| OK - {UriFromControllerName(uri)} -> {UriFromMethodName(uri)}");
                    }
                }

                Console.WriteLine($"REQUEST - {uri} | TIME - {DateTime.Now.ToString("MM-dd-yyyy H:mm:ss")}");

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(inputBuffer),  CancellationToken.None);;
            }


            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        catch (System.Exception exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.Source);
            Console.WriteLine(exception.StackTrace);

            _logger.Write($"{DateTime.Now.ToString("MM-dd-yyyy H:mm:s")}| ERROR - {exception.Message}");
        }
    }

    public async Task<Dictionary<string, string>> Message(WebSocket webSocket)
    {
        var receiver = new BufferManager();

        receiver.WsAdmin = webSocket;

        var buffer = await receiver.ResolveReceive();

        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

        return JsonConvert.DeserializeObject<Dictionary<string, string>>(receivedMessage) ?? new Dictionary<string, string>{};
    }

    public Response ErrorNotFound()
    {
        var response = new Response();
        
        response.Attributes = new {
            Error = "Route not found"
        };

        return response;
    }

    public string UriFromControllerName(string uriname)
    {
        var config = new Credentials();

        var uriOnly = uriname.Replace(config.Prefix, "");

        return uriOnly.Split("/")[0] + "Controller";
    }

    public string UriFromMethodName(string uriname)
    {
        var config = new Credentials();

        var uriOnly = uriname.Replace(config.Prefix, "");

        return uriOnly.Split("/")[1];
    }
}