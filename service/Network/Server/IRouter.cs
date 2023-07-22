using System.Net;

namespace Service.Network.Server;

public interface IDefinition
{
    public void UseContext(HttpListenerContext context);

    public Task Dispatch();

}