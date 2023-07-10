using System.Security.Cryptography;
using iTextSharp.text.pdf.security;
using System.Security.Cryptography.X509Certificates;
using Certificate = System.Security.Cryptography.X509Certificates.X509Certificate2;

namespace Service.Signature;

public class SignatureExt : IExternalSignature
{
    private Certificate _cert;

    public SignatureExt(Certificate cert)
    {
        _cert = cert;
    }

    public virtual byte[] Sign(byte[] message)
    {
        return SignatureComplex(message);
    }

    public byte[] SignatureComplex(byte[]  message)
    {
        return _cert
            .GetRSAPrivateKey()?
            .SignData(
                message,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            )
            ??
            new byte[0];
    }

    public virtual string GetHashAlgorithm()
    {
        return "SHA-256";
    }

    public virtual string GetEncryptionAlgorithm()
    {
        return "RSA";
    }
}
