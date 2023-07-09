using Service.Attributes;
using Service.Network;

namespace Service.Controllers;

[WsRoute("certificates")]
public class CertificatesController : Controller
{
    // public async Task<Response> Store()
    // {

    // }

    public async Task<Response> Index()
    {
        var result = new Dictionary<string, string>
        {
            ["certificates"] = Signer.ListCertificates()
        };

        return await Send(result);
    }

    // public async Task<Response> Update()
    // {

    // }

    // public async Task<Response> Delete()
    // {

    // }
}