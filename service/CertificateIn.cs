using System;

namespace Service;

public class CertificateIn
{
    public byte[] FileContent { get; set; }

    public string Password { get; set; }
}