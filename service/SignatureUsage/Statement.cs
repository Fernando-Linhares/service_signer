namespace Service.SignatureUsage;

public class Statement
{
    public string Status { get; set; }

    public string Time { get; set; }

    public string FileName { get; set; }

    public string FileContent { get; set; }
    
    public string CertName { get; set; }

    public string Message { get; set; }

    public int StatusCode { get; set; }
}