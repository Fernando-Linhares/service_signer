// using System;
// using System.Security.Cryptography.X509Certificates;
// using iTextSharp.text.pdf;
// using iTextSharp.text.pdf.security;
// using Parser = Org.BouncyCastle.X509.X509CertificateParser;
// using BouncyCert = Org.BouncyCastle.X509.X509Certificate;
// using CryptoException = System.Security.Cryptography.CryptographicException;
// using System.Collections.Generic;

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
                   new CertificateIn {
                        Path = parameters["c"],
                        Type="external"
                    },
                    parameters["p"],
                    parameters["f"]
                );

                Environment.Exit(0);
            }

            if(Determinate("signature-2", parameters))
            {
                string[] keys = Flush(parameters);

                Signer.Sign(
                    new CertificateIn {
                        Path = parameters["cert-path"],
                        Type="external"
                    },
                    parameters["password"],
                    parameters["file-path"]
                );

                Environment.Exit(0);
            }

             if(Determinate("signature-3", parameters))
            {
                string[] keys = Flush(parameters);

                Signer.Sign(
                   new CertificateIn {
                        Index = parameters["i"],
                        Type = "local"
                    },
                    parameters["p"],
                    parameters["f"]
                );
            
                Environment.Exit(0);
            }

            if(Determinate("signature-4", parameters))
            {
                string[] keys = Flush(parameters);

                Signer.Sign(
                    new CertificateIn {
                        Index = parameters["cert-index"],
                        Type = "local"
                    },
                    parameters["password"],
                    parameters["file-path"]
                );

                Environment.Exit(0);
            }

            if(Determinate("list-1", parameters) || Determinate("list-2", parameters))
            {
                Signer.ListCertificates();

                Environment.Exit(0);
            }

            if(Determinate("add-1", parameters))
            {
                Signer.AddCertificate(parameters["c"], parameters["p"]);

                Environment.Exit(0);
            }

            if(Determinate("add-2", parameters))
            {
                Signer.AddCertificate(parameters["cert-path"], parameters["password"]);

                Environment.Exit(0);
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

        if("signature-3" == pattern)
            return (
                parameters.ContainsKey("i") &&
                parameters.ContainsKey("f") &&
                parameters.ContainsKey("p")
            );

        if("signature-4" == pattern)
            return (
                parameters.ContainsKey("file-path") &&
                parameters.ContainsKey("cert-index") &&
                parameters.ContainsKey("password")
            );

        if("list-1" == pattern)
            return parameters.ContainsKey("l");

        if("list-2" == pattern)
            return parameters.ContainsKey("list");

        if("add-1" == pattern)
            return (
                parameters.ContainsKey("p") &&
                parameters.ContainsKey("c") 
            );
    
        if("add-2" == pattern)
            return (
                parameters.ContainsKey("password") &&
                parameters.ContainsKey("cert-path")
            );

        return false;
    }

    public static Dictionary<string, string> Args(string[] args)
    {
        var dict = new Dictionary<string, string>();

        foreach(string arg in args)
        {
            string[] keyValue = arg.Split("=");

            var key = keyValue[0].Substring(2);

            dict[key] = keyValue.Length > 1 ? keyValue[1] : "";
        }

        return dict;
    }
}