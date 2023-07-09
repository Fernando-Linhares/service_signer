using Service.Network.Server;

namespace Service;

public class Program
{
    public static void Main(string[] args)
    {
       var server = new WebsocketServer();

       server.Listen();
    }
}