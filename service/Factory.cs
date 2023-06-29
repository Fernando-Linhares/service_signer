using Service.Serialization;
using System.Xml.Serialization;
using System.Net;

namespace Service;

public class Factory
{
    private Env _env = new Env();

    public Apparence Apparence()
    {
        string? storage = _env.Get("STORAGE_PATH");

        string preferenciesFile = storage + @"/preferencies/Apparece.xml";

        if(!File.Exists(preferenciesFile))
            CreateFilePreferencies(preferenciesFile);

        using var fileStream = File.Open(preferenciesFile , FileMode.Open);

        Apparence? preferencies = new Apparence();

        var serializer = new XmlSerializer(typeof(Apparence));

        preferencies = (Apparence?) serializer.Deserialize(fileStream);

        fileStream.Close();

        return preferencies ?? new Apparence();
    }

    public void CreateFilePreferencies(string filepath)
    {
        using var fileStream = File.Open(filepath, FileMode.Create);

        if(!_env.KeyIsEmpty("PREFERENCIES"))
        {
            fileStream.Close();

            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(_env.Get("PREFERENCIES"));

            request.Method = "GET";

            String responseBody = String.Empty;

            using HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            Stream dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            responseBody = reader.ReadToEnd();

            reader.Close();

            dataStream.Close();

            File.WriteAllText(filepath, responseBody);
        }
        else
        {
            var serializer = new XmlSerializer(typeof(Apparence));

            serializer.Serialize(fileStream, new Apparence());

            fileStream.Close();
        }
    }
}