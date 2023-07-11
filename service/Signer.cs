using System.Security.Cryptography.X509Certificates;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Parser = Org.BouncyCastle.X509.X509CertificateParser;
using BouncyCert = Org.BouncyCastle.X509.X509Certificate;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Service.Signature;
using System.Text;

namespace Service;

public class Signer
{
    public static async Task<Statement> Sign(int CertificateId, string password, string filepath)
    {
        var env = new Env();

        string status = "pending";

        string message = "Pending signature";

        string fileName = "Not Found";

        string certName = "Not Found";

        string signedfilename = "Not Found";

        int statusCode = 404;

        try
        {
            status = "assinando";

            var bytes = File.ReadAllBytes(filepath);

            var cert = FindCert(CertificateId);

            var parser = new Parser();

            using var reader = new PdfReader(filepath);

            signedfilename = filepath.Substring(0, filepath.Length -4) + "-signed.pdf";

            using var stream = File.Open(signedfilename, FileMode.Create);

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

            status = "signed";

            message = "signed successfully";

            statusCode = 200;
        }
        catch(iTextSharp.text.DocumentException document)
        {
            status = "fail";

            message = document.Message;

            statusCode = 404;

            Console.WriteLine(document.Message);
        }
        catch(System.Security.Cryptography.CryptographicException cryptographic)
        {
            statusCode = 500;

            status = "fail";

            message = cryptographic.Message;
        }
        catch(System.IO.IOException io)
        {
            statusCode = 500;

            status = "fail";

            message = io.Message;
        }
        catch (System.Exception generic)
        {
            statusCode = 500;

            status = "fail";
        }

        byte[] signedBytes = await File.ReadAllBytesAsync(signedfilename);

        string signedContent = Convert.ToBase64String(signedBytes);

        return new Statement
        {
            Time = DateTime.UtcNow.ToString("mm-dd-yyyy h:i:s"),
            CertName = certName,
            FileName = fileName,
            FileContent = signedContent,
            Status = status.ToUpper()
        };
    }

    public static async Task UpdateStatus(string status, string filename, string? url, string? logpath)
    {
        var env = new Env();

        string logPath = env.Get("LOGS_PATH");

        var now = DateTime.Now.ToString("MM-dd-yyyy");

        string logFileName = $"{logPath}/{now}.log";

        if(!File.Exists(logFileName))
        {
            using var fileLog = File.Create(logFileName);
            fileLog.Close();
        }

        try
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
        catch (System.Exception exception)
        {
            File.AppendAllText(logFileName, $"\n{DateTime.Now.ToString("mm-dd-yyyy H:m:s")}| ERROR - {exception.InnerException}");
        }
    }

    public static List<object> ListCertificates()
    {
        var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

        store.Open(OpenFlags.ReadOnly);

        var certList = new List<object>();

        var parser = new Parser();

        var getNameByCert = (X509Certificate2 cert) =>
        {
            var bouncyCert =  parser.ReadCertificate(cert.RawData);

            return CertificateInfo.GetSubjectFields(bouncyCert).GetField("CN");
        };

        foreach(X509Certificate2 cert in store.Certificates)
        {
            var name = getNameByCert(cert);

            certList.Add(new {
                Subject = cert.Subject,
                Issuer = cert.Issuer,
                Thumbprint = cert.Thumbprint,
                Name = name,
                Location = $"{store.Location}/{name}"
            });
        }

        return certList;
    }

    public static object AddCertificate(string filepath, string password)
    {
        try
        {
            var cert = new X509Certificate2(filepath, password);

            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            store.Open(OpenFlags.ReadWrite);

            store.Add(cert);

            store.Close();

            return new
            {
                Certificate = cert
            };
        }
        catch (System.Exception exception)
        {
            Console.WriteLine("Error on add certificate");

            return new 
            {
                Error = exception.InnerException
            };
        }
    }

    public static object AddCertificate(CertificateIn certificate)
    {
        try
        {
            var cert = new X509Certificate2(certificate.FileContent, certificate.Password);

            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            store.Open(OpenFlags.ReadWrite);

            store.Add(cert);

            store.Close();

            return new
            {
                Certificate = cert
            };
        }
        catch (System.Exception exception)
        {
            return new 
            {
                Error = exception.InnerException
            };
        }
    }

    public static X509Certificate2 FindCert(int id)
    {
        var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

        store.Open(OpenFlags.ReadWrite);

        var list = new List<X509Certificate2>();

        foreach(var cert in store.Certificates)
        {
            list.Add(cert);
        }

        store.Close();

        int index = id - 1;

        return list[index];
    }
}