using System.Net.WebSockets;
using System.Text;
using Service.Network;
using Newtonsoft.Json;

namespace Service.Models;

public class Application: BaseModel
{
    private WebSocket _websocket;

    private Logger _logger;

    private readonly Env _env = new Env();

    public Application()
    {
        var env = new Env();

        string logPath = env.Get("LOGS_PATH") ?? "";

        _logger = new Logger(logPath);
    }

    public async Task<string> Flush(string request)
    {
        var defaultMessage = ExecuteMessage(new Dictionary<string, string>{
            ["command"] = "default"
        });

        try
        {
            var strMessageByClient = Decode(request);

            var body = JsonConvert.DeserializeObject<Dictionary<string, string>>(strMessageByClient);

            var response = await ExecuteMessage(body);

            var responseContent = response.ToString();

            string encodedResponse = Encode(responseContent);

            if(response.Code == 1)
            {
                _logger.Write($"{DateTime.Now.ToString("MM-dd-yyyy H:mm:s")}| ERROR - {response.ToString()}");
            }

            Console.WriteLine($"EXECUTION COMMAND - {body["command"]} | TIME - {DateTime.Now.ToString("MM-dd-yyyy H:mm:ss")}");

            return encodedResponse;
        }
        catch (System.Exception exception)
        {
            var internalError = Send(new { InternalError=exception.Message});

            string errorString = internalError.ToString();

            byte[] errorBytes = Encoding.UTF8.GetBytes(errorString);

            await _websocket.SendAsync(new ArraySegment<byte>(errorBytes), WebSocketMessageType.Text, true, CancellationToken.None);

            _logger.Write($"{DateTime.Now.ToString("MM-dd-yyyy H:mm:s")}| ERROR - {exception.Message} - Raw {exception.StackTrace}");
        }

        return Encode(defaultMessage.ToString());
    }

    private string Decode(string content)
    {
        string decodedString = content;

        if(_env.Get("APP_ENV") == "production")
        {
            var base64bytes =  Convert.FromBase64String(decodedString);

            decodedString =  Convert.ToBase64String(base64bytes);
        }

        return decodedString;
    }

    private string Encode(string content)
    {
        string encodedString = content;

        if(_env.Get("APP_ENV") == "production")
        {
            var base64bytes =  Convert.FromBase64String(encodedString);

            encodedString = Convert.ToBase64String(base64bytes);
        }

        return encodedString;
    }

    private async Task<Response> ExecuteMessage(Dictionary<string, string> body)
    {
        return body["command"] switch
        {
            "sign.file" => await SingatureCommand(body),
            "list.certificates" => await ListCertificatesCommand(),
            "setup.environment" => await SetupEnvironmentCommand(),
            "default" => await NotFoundCommand(),
            _ => await NotFoundCommand()
        };
    }

    private async Task<Response> SingatureCommand(Dictionary<string, string> body)
    {
        var signature = new Signature();

        signature.Form = body;

        return await signature.Sign();
    }

    private async Task<Response> ListCertificatesCommand()
    {
        var certificates = new Certificate();

        return await certificates.List();
    }

    private async Task<Response> SetupEnvironmentCommand()
    {
        var environment = new Environment();

        return await environment.Setup();
    }

    private async Task<Response> NotFoundCommand()
    {
        return await Send(new {
            Code=1,
            Error="Not Found Command"
        });
    }
}