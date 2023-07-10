using Service.Controllers;

namespace Service;

public class Routes
{
    public List<Controller> EnabledControllers = new List<Controller>
    {
        new EnvironmentController(),
        new CertificatesController(),
        new SignaturesController()
    };
}