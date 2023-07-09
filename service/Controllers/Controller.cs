using Service.Network;

namespace Service.Controllers;

public abstract class Controller
{
   public Dictionary<string, string>? Form { get; set; }

   public async Task<Response> Send(Dictionary<string, string> data)
   {
        var response = new Response();

        response.Attributes = data;

        return response;
   }
}