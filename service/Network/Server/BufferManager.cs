using System.Net.WebSockets;
using System.Text;

namespace Service.Network.Server;

public class BufferManager
{
    public WebSocket WsAdmin { get; set; }

    private byte[] _buffer;

    private Logger _logger;

    public readonly int bufferSize = 14969112;

    public readonly int limitSize = 100;

    public BufferManager()
    {
        var env  = new Env();

        _logger = new Logger(env.Get("LOGS_PATH"));
    }

    public async Task<byte[]> ResolveReceive()
    {
        using var memoryStream = new MemoryStream();

        try
        {
            _buffer = new byte[bufferSize];

            var result = await WsAdmin.ReceiveAsync(new ArraySegment<byte>(_buffer),  CancellationToken.None);

            memoryStream.Write(_buffer, 0, result.Count);

            string contentAvaliable = Encoding.UTF8.GetString(_buffer);

            int lastindex = limitSize;
            
            while(!result.EndOfMessage)
            {
                result = await WsAdmin.ReceiveAsync(new ArraySegment<byte>(_buffer),  CancellationToken.None);

                memoryStream.Write(_buffer, lastindex, result.Count);
            }

            return memoryStream.ToArray();
        }
        catch(Exception exception)
        {
            _logger.Write($"{DateTime.Now.ToString("MM-dd-yyyy H:mm:s")}| ERROR - {exception.Message}");

            return memoryStream.ToArray();
        }
        finally
        {
            memoryStream.Close();
        }
    }
}