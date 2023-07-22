using System.Net;
using System.Net.WebSockets;
using System.Text;
using Service.Models;

namespace Service.Network.Server;

public class DefinitionLayer
{
    private const int MaxBufferSize = 4024;

    private readonly HttpListener _httpListener;

    private WebSocket _webSocket;

    private StringBuilder _messageBuilder = new StringBuilder();

    public async Task StartAsync()
    {
        var listener = new HttpListener();

        var config = new Credentials();

        listener.Prefixes.Add(config.Prefix);

        listener.Start();

        HttpListenerContext context = await listener.GetContextAsync();

        if (context.Request.IsWebSocketRequest)
        {
            await ProcessWebSocketRequest(context);
        }
        else
        {
            context.Response.StatusCode = 400;

            context.Response.Close();
        }

        listener.Abort();

        await StartAsync();
    }

    private async Task ProcessWebSocketRequest(HttpListenerContext context)
    {
        HttpListenerWebSocketContext webSocketContext = null;

        try
        {
            webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);

            _webSocket = webSocketContext.WebSocket;

            await ReceiveMessagesAsync();
        }
        catch (Exception ex)
        {
            webSocketContext?.WebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Internal Server Error", CancellationToken.None);
        }
    }

    private async Task ReceiveMessagesAsync()
    {
        while (_webSocket.State == WebSocketState.Open)
        {
            try
            {
                byte[] buffer = new byte[MaxBufferSize];

                WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string messageChunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    
                    _messageBuilder.Append(messageChunk);

                    if(result.EndOfMessage)
                    {
                        string messageRquest = _messageBuilder.ToString();
                        
                        var app = new Application();

                        var messageResponse = await app.Flush(messageRquest);

                        await SendStringAsync(messageResponse);

                        _messageBuilder.Clear();
                    }                    
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the server", CancellationToken.None);
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("WebSocket error: " + ex.Message);
                _webSocket.Dispose();
                break;
            }
        }        
    }

    private async Task SendStringAsync(string message)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        
        await _webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}
