using Service.Network;

namespace Service.Controllers;

public class CertificatesController : Controller
{
    public async Task<Response> Store()
    {
        var result = Signer.AddCertificate(new CertificateIn
        {
            FileContent = Convert.FromBase64String(Form["FileContent"]),
            Password = Form["Password"]
        });

       return await Send(result);
    }

    public async Task<Response> Index()
    {
        if(Form.Keys.Contains("Id"))
            return await Send(new
            {
                certificates = Signer.FindCert(int.Parse(Form["id"]))
            });

        return await Send(new 
        {
            certificates = Signer.ListCertificates()
        });
    }
}