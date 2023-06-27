using System;
using System.Security.Cryptography.X509Certificates;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Parser = Org.BouncyCastle.X509.X509CertificateParser;
using BouncyCert = Org.BouncyCastle.X509.X509Certificate;
using CryptoException = System.Security.Cryptography.CryptographicException;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Service;

public class Signer
{
    public X509Certificate2 GetCert(string certpath, string password)
    {

        return new X509Certificate2(certpath, password);
    }

    public static void Sign(string certpath, string password, string filepath)
    {
        var bytes = GetBytes(certpath);

        using var cert =  GetCert(certpath, password);

        var parser = new Parser();

        using var reader = new PdfReader(filepath);

        using var stream = File.Open(filepath.Substring(0, filepath.Length -4) + "-signed.pdf", FileMode.Create);

        using var stamper = PdfStamper.CreateSignature(reader, stream, '0', null, true);

        var bouncyCert = new BouncyCert[]{ parser.ReadCertificate(cert.RawData) };

        var certName = CertificateInfo.GetSubjectFields(bouncyCert[0]).GetField("CN");

        var directory = new PdfSignature(PdfName.ADOBE_PPKLITE, PdfName.ADBE_PKCS7_DETACHED);

        directory.Name = certName;

        directory.Date = new PdfDate(DateTime.Now);

        var apparence = stamper.SignatureAppearance;

        var preferencies =

        apparence.Certificate = bouncyCert[0];

        apparence.CryptoDictionary = directory;

        apparence.Reason = preferencies.Reason;

        apparence.Location =  preferencies.Location;

        string content = "";

        content += Regex.Replace(certName, @"@cert_name@", preferencies.Content);

        content += Regex.Replace(apparence.SignDate, @"@signature_date@", content);

        apparence.Layer2Text = content;

        if(preferencies.Visible)
        {
            apparence.SignatureRenderingMode = preferencies.Type switch
            {
                "DESCRIPTION" => PdfSignatureAppearance.RenderingMode.DESCRIPTION,
            };

            var rec = new Rectangle(
                preferencies.Dimensions[0],
                preferencies.Dimensions[0],
                preferencies.Dimensions[0],
                preferencies.Dimensions[0]
            );

            apparence.SetVisibleSignature(rec, "Signature");
        }

        apparence.Acro6Layers = true;

        IExternalSignature external = new SignatureExt(cert);

        MakeSignature.SignDetached(apparence, external, bouncyCert, null, null, null, 0, CryptoStandard.CMS);

        cert.Close();

        reader.Close();

        stream.Close();

        stamper.Close();
    }

    public byte[] GetBytes(string filepath)
    {
        return File.ReadAllBytes(filepath);
    }
}