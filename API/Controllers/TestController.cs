using Application.Interfaces;
using Dmain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly ISmsService _service;
        public TestController(ISmsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> sms()
        {
            Phone phone = new Phone("09960357263");
            await _service.SendAsync(phone , "Send sms");
            return Ok();
        }
    }
}
