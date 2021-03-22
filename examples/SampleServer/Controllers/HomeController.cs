using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;

namespace SampleServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IUsersGetter _client;

        public HomeController(IUsersGetter client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult> GetUsersAsync()
        {
            var response = await _client.GetUsersAsync(string.Empty);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
    }
}