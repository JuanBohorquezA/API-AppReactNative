using API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class ControllerParent : ControllerBase
    {
        public async Task<ObjectResult> GetResponseAsync<T>(HttpStatusCode statusCode, string message, T response)
        {
            return await Task.Run(() =>
            {
                var objectResult = new HttpResponse<T>((int)statusCode, message, response);
                return new ObjectResult(objectResult)
                {
                    StatusCode = (int)statusCode
                };
            });
        }

    }
}
