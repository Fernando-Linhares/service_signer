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

            if(
                parameters.ContainsKey("filepath") &&
                parameters.ContainsKey("certpath") &&
                parameters.ContainsKey("password")
            )
                Signer.Sign(parameters["certpath"], parameters["password"], parameters["filepath"]);
        }
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