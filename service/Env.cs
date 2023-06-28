using Service.Serialization;
using System.Xml.Serialization;
using System.Net;

namespace Service;

public class Env
{
    public Dictionary<string, string?> environments = new Dictionary<string, string?>();

    public Env()
    {
        string? key, value, filecontent;

        filecontent = File.ReadAllText(".env");

        foreach(string raw in filecontent.Split("\n"))
        {
            string[] rowSplited = raw.Split("=");

            key = raw.Split("=")[0];

            value = (rowSplited.Length > 1) ? raw.Split("=")[1] : "";
    
            environments[key] = value;
        }
    }

    public string? Get(string key)
    {
        return environments[key];
    }

    public void Set(string key, string value)
    {
        environments[key] = value;
    }

    public bool KeyIsEmpty(string key)
    {
        return String.IsNullOrEmpty(environments[key]);
    }
}