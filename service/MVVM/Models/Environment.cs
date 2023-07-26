using Service.Network;

namespace Service.MVVM.Models;

public class Environment: BaseModel
{
    private readonly Env _env = new Env();

    public async Task<Response> Setup()
    {
        return await Send(new
        {
            AppName = _env.Get("APP_NAME"),
            Version = _env.Get("APP_VERSION"),
            Environment =  _env.Get("APP_ENV"),
            Auth =  _env.Get("AUTH")
        });
    }
}