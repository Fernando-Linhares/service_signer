namespace Service.Network.Server;

public class Credentials
{
    public string Host = "localhost";

    public int Port = 2514;

    public string Protocolo = "http";

    public bool Authentitable = false;

    public string Prefix
    {
        get => $"{Protocolo}://{Host}:{Port}/";
    }
}
