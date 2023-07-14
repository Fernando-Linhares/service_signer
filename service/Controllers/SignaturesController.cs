using Service.Network;
using System.Security.Cryptography;
using System.Text;
using System;

namespace Service.Controllers;

public class SignaturesController: Controller
{
    private Logger _logger { get; set; }

    public SignaturesController()
    {
        var env = new Env();

        string logPath = env.Get("LOGS_PATH") ?? "";

        _logger = new Logger(logPath);
    }

    public async Task<Response> Sign()
    {
        var env = new Env();

        try
        {
            string filePath = await MountFile(Form["FileName"], Form["FileContent"]);

            int cert = int.Parse(Form["CertId"]);

            string password = Form["Password"];

            var result = await Signer.Sign(cert, password, filePath);

            if(!env.KeyIsEmpty("WEBHOOK"))
                await Signer.UpdateStatus(result.Status, Form["FileName"], env.Get("WEBHOOK"), env.Get("LOG_PATH"));
            
            return await Send(result);
        }
        catch (System.Exception exception)
        {
            _logger.Write(exception.Message);

            throw;
        }
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