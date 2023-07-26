using Service.Network;
using Newtonsoft.Json;

namespace Service.MVVM.Models;

public abstract class BaseModel
{

   public Dictionary<string, string>? Form { get; set; }

   public async Task<Response> Send(object data)
   {
      var response = new Response();

      response.Attributes = data;

      return response;
   }

   public async Task<Response> Success(object data)
   {
      var resolve =  await Send(data);

      resolve.Code = 0;

      return resolve;
   }

   public async Task<Response> Error(object data)
   {
      var resolve =  await Send(data);

      resolve.Code = 1;

      return resolve;
   }
}