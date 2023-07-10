using Service.Network;
using System.Security.Cryptography;
using System.Text;
using System;

namespace Service.Controllers;

public class SignaturesController: Controller
{
    public async Task<Response> Sign()
    {
        string filePath = await MountFile(Form["FileName"], Form["FileContent"]);

        int cert = int.Parse(Form["CertId"]);

        string password = Form["Password"];

        var result = await Signer.Sign(cert, password, filePath);

        var env = new Env();

        if(!env.KeyIsEmpty("WEBHOOK"))
            await Signer.UpdateStatus(result.Status, Form["FileName"], env.Get("WEBHOOK"), env.Get("LOG_PATH"));

        return await Send(result);
    }

    private async Task<string> MountFile(string filename, string filecontent)
    {
        var env = new Env();

        string path = $"{env.Get("PDFS_PATH")}/{filename}";

        using var fs = File.Create(path);

        fs.Close();

        var content = Convert.FromBase64String(filecontent);

        await File.WriteAllBytesAsync(path, content);

        return path;
    }
}