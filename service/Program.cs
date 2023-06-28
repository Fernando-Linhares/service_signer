using System;
using System.Security.Cryptography.X509Certificates;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Parser = Org.BouncyCastle.X509.X509CertificateParser;
using BouncyCert = Org.BouncyCastle.X509.X509Certificate;
using CryptoException = System.Security.Cryptography.CryptographicException;
using System.Collections.Generic;

namespace Service;

public class Program
{
    public static void Main(string[] args)
    {
        if(args.Length > 0)
        {
            var parameters = Args(args);

            if(Determinate("signature-1", parameters))
            {
                string[] keys = Flush(parameters);

                Signer.Sign(
                    parameters["c"],
                    parameters[""],
                    parameters[""]
                );
            }

            if(Determinate("signature-2", parameters))
            {
                string[] keys = Flush(parameters);

                Signer.Sign(
                    parameters["cert-path"],
                    parameters["password"],
                    parameters["file-path"]
                );
            }

            if(Determinate("select-1", parameters))
            {
                string[] keys = Flush(parameters);

                Signer.SelectCertificate(parameters["i"]);
            }

            if(Determinate("select-2", parameters))
            {
                string[] keys = Flush(parameters);

                Signer.SelectCertificate(parameters["index-cert"]);
            }

            if(Determinate("list-1", parameters) || Determinate("list-2", parameters))
            {
                string[] keys = Flush(parameters);

                Signer.ListCertificates();
            }
        }
    }

    public static string[] Flush(Dictionary<string, string> parameters)
    {
        return parameters.Keys.ToArray();
    }


    public static bool Determinate(string pattern ,Dictionary<string, string> parameters)
    {
        if("signature-1" == pattern)
            return (
                parameters.ContainsKey("f") &&
                parameters.ContainsKey("c") &&
                parameters.ContainsKey("p")
            );

        if("signature-2" == pattern)
            return (
                parameters.ContainsKey("file-path") &&
                parameters.ContainsKey("cert-path") &&
                parameters.ContainsKey("password")
            );

        if("list-1" == pattern)
            return parameters.ContainsKey("l");

        if("list-2" == pattern)
            return parameters.ContainsKey("list-cert");

        if("select-1" == pattern)
            return (
                parameters.ContainsKey("s") &&
                parameters.ContainsKey("i")
            );

        if("select-2" == pattern)
            return (
                parameters.ContainsKey("select-cert") &&
                parameters.ContainsKey("index-cert")
            );

        return false;
    }

    public static Dictionary<string, string> Args(string[] args)
    {
        var dict = new Dictionary<string, string>();

        foreach(string arg in args) {

            var key = arg.Split("=")[0].Substring(2);

            dict[key] = arg.Split("=")[1];
        }

        return dict;
    }
}