using System.Net.WebSockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Service.Attributes;

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

        byte[] buffer = new byte[1024];

        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

            var request = JsonConvert.DeserializeObject<Request>(receivedMessage);

            Console.WriteLine(receivedMessage);

            var routes = new Routes();

            foreach(var controller in routes.EnabledControllers)
            {
                var propInfo = controller.GetType().GetCustomAttributes(true);

                if(propInfo is null)
                    throw new NotImplementedException(
                        $"The annotation WsRoute is not implemented on controller: {controller.GetType()}"
                    );

                WsRoute? route = (WsRoute?) propInfo[0];

                if(route is null)
                    throw new KeyNotFoundException();

                Console.WriteLine(request.Attributes);
                Console.WriteLine(request.Attributes.Keys);
                Console.WriteLine(request.Attributes.Keys.Contains(route.Content));

                if(request.Attributes.Keys.Contains(route.Content))
                {
                    var matchAttrs = request.Attributes[route.Content];

                    var listMethodInfo = controller.GetType().GetMethods().ToList();

                    Console.WriteLine("Chegou aqui 1");

                    foreach(var methodInfo in listMethodInfo)
                    {
                        var nameMethod = methodInfo.Name;

                        if(matchAttrs.Keys.Contains(nameMethod))
                        {
                            Console.WriteLine("Chegou aqui 1");

                            var attrs = matchAttrs[nameMethod];

                            controller.Form = attrs;

                            var response = await (Task<Response>?) methodInfo.Invoke(controller, null);

                            var responseContent = response.ToString();

                            byte[] responseBytes = Encoding.UTF8.GetBytes(responseContent);

                            await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                            buffer = new byte[1024];

                            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        }
                    }
                }
            }
        }

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}