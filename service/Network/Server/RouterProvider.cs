using System.Net.WebSockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Service.Network.Server;

public class RouterProvider: IRouter
{
    private HttpListenerContext _context;

    public void UseContext(HttpListenerContext context)
    {
        _context = context;
    }

    public async Task Dispatch()
    {
        HttpListenerWebSocketContext webSocketContext = await _context.AcceptWebSocketAsync(null);

        WebSocket webSocket = webSocketContext.WebSocket;

        byte[] buffer = new byte[90024];

        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

            var request = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string,string>>>>(receivedMessage);

            var routes = new Routes();

            foreach(var controller in routes.EnabledControllers)
            {
                string className = controller.GetType().Name;

                string routeName = className.Replace("Controller", "");

                if(request.Keys.Contains(routeName))
                {
                    var env = new Env();

                    string logPath = env.Get("LOGS_PATH") ?? "";

                    var now = DateTime.Now.ToString("MM-dd-yyyy");

                    string logFileName = $"{logPath}/{now}.log";

                    if(!File.Exists(logFileName))
                    {
                        using var fileLog = File.Create(logFileName);

                        fileLog.Close();
                    }

                    try
                    {
                        var matchAttrs = request[routeName];

                        var listMethodInfo = controller.GetType().GetMethods().ToList();

                        foreach(var methodInfo in listMethodInfo)
                        {
                            var nameMethod = methodInfo.Name;

                            if(matchAttrs.Keys.Contains(nameMethod))
                            {
                                var attrs = matchAttrs[nameMethod];

                                controller.Form = attrs;

                                var response = await (Task<Response>?) methodInfo.Invoke(controller, null);

                                var responseContent = response.ToString();

                                byte[] responseBytes = Encoding.UTF8.GetBytes(responseContent);

                                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                                buffer = new byte[90024];

                                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                                File.AppendAllText(logFileName, $"\n{DateTime.Now.ToString("MM-dd-yyyy H:mm:ss")}| OK - {routeName}Controller->{nameMethod}");
                            }
                        }

                        Console.WriteLine($"REQUEST - {routeName} | TIME - {DateTime.Now.ToString("MM-dd-yyyy H:mm:ss")}");
                    }
                    catch (System.Exception exception)
                    {
                        File.AppendAllText(logFileName, $"\n{DateTime.Now.ToString("MM-dd-yyyy H:mm:s")}| ERROR - {exception.Message}");

                        var response = new Response
                        {
                            Attributes = new
                            {
                                Error = exception.Message,
                            }
                        };

                        var responseContent = response.ToString();

                        byte[] responseBytes = Encoding.UTF8.GetBytes(responseContent);

                        await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                        buffer = new byte[90024];

                        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    }
                }
            }
        }
    
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    public Response ErrorNotFound()
    {
        var response = new Response();
        
        response.Attributes = new {
            Error = "Route not found"
        };

        return response;
    }
}