namespace Service.Attributes;

public class WsMethod: Attribute
{
    public string Content { get; set; }

    public WsMethod(string content)
    {
        Content = content;
    }
}