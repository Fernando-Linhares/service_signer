using Newtonsoft.Json;

namespace Service.Network;

public class Response
{
    public object Attributes { get; set; }

    public string ToString()
    {
        return JsonConvert.SerializeObject(Attributes);
    }
}