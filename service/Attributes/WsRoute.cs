namespace Service.Attributes;

public class WsRoute: Attribute
{
    public string Content { get; set; }

    public WsRoute(string content)
    {
        Content = content;
    }
}