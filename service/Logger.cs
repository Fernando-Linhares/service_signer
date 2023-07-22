using System;

namespace Service;

public class Logger
{
    private string _path;

    public Logger(string path)
    {
        _path = path;
    }

    public void Write(string content)
    {
        var now = DateTime.Now.ToString("MM-dd-yyyy");

        string logFileName = $"{_path}/{now}.log";

        if(File.Exists(logFileName))
        {
            using var f = File.Create(logFileName);

            f.Close();
        }

        string currentContent = File.ReadAllText(logFileName);

        File.WriteAllText(logFileName, currentContent + "\n" + content);
    }
}