using Service.Network;
using Service.SignatureUsage;

namespace Service.MVVM.Models;

public class Signature: BaseModel
{
    private Logger _logger { get; set; }

    public Signature()
    {
        var env = new Env();

        string logPath = env.Get("LOGS_PATH") ?? "";

        _logger = new Logger(logPath);
    }

    public async Task<Response> Sign()
    {
        var env = new Env();

        Statement result = null;

        try
        {
            string filePath = await MountFile(Form["filename"], Form["filecontent"]);

            int cert = int.Parse(Form["certid"]);

            string password = Form["password"];

            result = await Signer.Sign(cert, password, filePath);
                        
            if(!env.KeyIsEmpty("WEBHOOK"))
                await Signer.UpdateStatus(result.Status, Form["filename"], env.Get("WEBHOOK"), env.Get("LOG_PATH"));
        }
        catch (System.Exception exception)
        {
            _logger.Write(exception.Message);
            Console.WriteLine(exception.Message);
            throw;
        }

        if(result is null){
            return await Error(new {
                Error="Internal error",
                Code=1
            });
        }
        return await Send(result);
    }

    private async Task<string> MountFile(string filename, string filecontent)
    {
        var env = new Env();

        string path = $"{env.Get("PDFS_PATH")}/{filename}";

        try
        {
            using var fs = File.Create(path);

            fs.Close();

            var content = Convert.FromBase64String(filecontent);

            await File.WriteAllBytesAsync(path, content);
        }
        catch (System.Exception exception)
        {
            _logger.Write(exception.Message);

            throw;
        }

        return path;
    }
}