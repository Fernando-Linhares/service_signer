using Service.Network;

namespace Service.Models;

public class Certificate : BaseModel
{
    public async Task<Response> Store()
    {
        var result = Signer.AddCertificate(new CertificateIn
        {
            FileContent = Convert.FromBase64String(Form["filecontent"]),
            Password = Form["password"]
        });

       return await Send(result);
    }

    public async Task<Response> List()
    {
        return await Send(new 
        {
            certificates = Signer.ListCertificates()
        });
    }
}