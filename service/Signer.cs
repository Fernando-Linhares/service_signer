using System.Security.Cryptography.X509Certificates;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Parser = Org.BouncyCastle.X509.X509CertificateParser;
using BouncyCert = Org.BouncyCastle.X509.X509Certificate;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Service;

public class Signer
{
    public static void Sign(CertificateIn certificate, string password, string filepath)
    {
        var env = new Env();

        string status = "pendente";

        string fileName = "Not Found";

        string certName = "Not Found";

        try
        {
            status = "assinando";

            var bytes = File.ReadAllBytes(filepath);

            X509Certificate2? cert = null;

            if(certificate?.Type == "local")
            {
                cert = FindCert(certificate?.Index ?? "");
            }

            if(certificate?.Type == "external")
            {
                cert = new X509Certificate2(certificate?.Path ?? "", password);
            }

            if(cert is null)
                throw new Exception("Certificate type not found");

            var parser = new Parser();

            using var reader = new PdfReader(filepath);

            using var stream = File.Open(filepath.Substring(0, filepath.Length -4) + "-signed.pdf", FileMode.Create);

            fileName = Path.GetFileName(stream.Name);

            var stamper = PdfStamper.CreateSignature(reader, stream, '0', null, true);

            var bouncyCert = new BouncyCert[]{ parser.ReadCertificate(cert.RawData) };

            certName = CertificateInfo.GetSubjectFields(bouncyCert[0]).GetField("CN");

            var directory = new PdfSignature(PdfName.ADOBE_PPKLITE, PdfName.ADBE_PKCS7_DETACHED);

            directory.Name = certName;

            directory.Date = new PdfDate(DateTime.Now);

            PdfSignatureAppearance apparence = stamper.SignatureAppearance;

            var factory = new Factory();

            var preferencies = factory.Apparence();

            apparence.Certificate = bouncyCert[0];

            apparence.CryptoDictionary = directory;

            apparence.Reason = preferencies.Reason;

            apparence.Location =  preferencies.Location;

            string content = "";

            content += Regex.Replace(certName, @"@cert_name@", preferencies.Content);

            content += Regex.Replace(apparence.SignDate.ToString(), @"@signature_date@", content);

            apparence.Layer2Text = content;

            if(preferencies.Visible)
            {
                apparence.SignatureRenderingMode = preferencies.Type switch
                {
                    "DESCRIPTION" => PdfSignatureAppearance.RenderingMode.DESCRIPTION,
                    "GRAPHIC" => PdfSignatureAppearance.RenderingMode.GRAPHIC,
                    "GRAPHIC_AND_DESCRIPTION" => PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION,
                    "NAME_AND_DESCRIPTION" => PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION,
                    _ => PdfSignatureAppearance.RenderingMode.DESCRIPTION
                };

                var rec = new Rectangle(
                    preferencies.Dimensions[0],
                    preferencies.Dimensions[1],
                    preferencies.Dimensions[2],
                    preferencies.Dimensions[3]
                );

                apparence.SetVisibleSignature(rec, 1, "Signature");
            }

            apparence.Acro6Layers = true;

            IExternalSignature external = new SignatureExt(cert);

            MakeSignature.SignDetached(apparence, external, bouncyCert, null, null, null, 0, CryptoStandard.CMS);

            reader.Close();

            stream.Close();
        }
        catch(iTextSharp.text.DocumentException document)
        {
            status = "falha";

            Console.WriteLine(document.Message);
        }
        catch(System.Security.Cryptography.CryptographicException cryptographic)
        {
            status = "falha";

            Console.WriteLine(cryptographic.InnerException);
        }
        catch(System.IO.IOException io)
        {
            status = "falha";

            Console.WriteLine(io.InnerException);
        }
        catch (System.Exception generic)
        {
            status = "falha";

            Console.WriteLine(generic.InnerException);
        }
        finally
        {
            status = "assinado";
        }

        if(!env.KeyIsEmpty("WEBHOOK"))
            UpdateStatus(status, fileName, env.Get("WEBHOOK"), env.Get("LOG_PATH")).Wait();

        Console.WriteLine($"{DateTime.Now}| By {certName}| File: {fileName} | {status.ToUpper()}");
    }

    public static async Task UpdateStatus(string status, string filename, string? url, string? logpath)
    {
        var client = new HttpClient();

        var body = new { DateTime = DateTime.Now, FileName = filename, Status = status };

        var json = JsonConvert.SerializeObject(body);

        var content = new StringContent(json);

        var response = await client.PostAsync(url, content);

        using var streamWriter = new StreamWriter(File.Open(logpath, FileMode.OpenOrCreate));

        string responseContent = await response.Content.ReadAsStringAsync();

        streamWriter.WriteLine(responseContent);

        streamWriter.Close();
    }

    public static void ListCertificates()
    {
        var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

        store.Open(OpenFlags.ReadOnly);

        var certList = new List<Dictionary<string, string>>();

        var parser = new Parser();

        var getNameByCert = (X509Certificate2 cert) =>
        {
            var bouncyCert =  parser.ReadCertificate(cert.RawData);

            return CertificateInfo.GetSubjectFields(bouncyCert).GetField("CN");
        };

        foreach(X509Certificate2 cert in store.Certificates)
        {
            var name = getNameByCert(cert);

            certList.Add(new Dictionary<string, string>{
                ["Subject"] = cert.Subject,
                ["Issuer"] = cert.Issuer,
                ["Thumbprint"] = cert.Thumbprint,
                ["Name"] = name,
                ["Location"] = $"{store.Location}/{name}"
            });
        }

        string jsonContent = JsonConvert.SerializeObject(certList);

        Console.WriteLine(jsonContent);
    }

    public static void AddCertificate(string filepath, string password)
    {
        try
        {
            var cert = new X509Certificate2(filepath, password);

            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            store.Open(OpenFlags.ReadWrite);

            store.Add(cert);

            store.Close();

            Console.WriteLine($"Certificate added at - {DateTime.Now}");
        }
        catch (System.Exception exception)
        {
            Console.WriteLine("Error on add certificate");
            Console.WriteLine(exception.Message);
        }
    }

    private static X509Certificate2 FindCert(string index)
    {
        var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

        store.Open(OpenFlags.ReadWrite);

        var list = new List<X509Certificate2>();

        foreach(var cert in store.Certificates)
        {
            list.Add(cert);
        }

        store.Close();

        return list[int.Parse(index)];
    }
}