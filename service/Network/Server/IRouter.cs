using System.Net;

namespace Service.Network.Server;

public interface IRouter
{
    public void UseContext(HttpListenerContext context);

    public Task Dispatch();

}