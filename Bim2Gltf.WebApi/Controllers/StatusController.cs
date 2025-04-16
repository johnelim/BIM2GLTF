using Microsoft.AspNetCore.Mvc;

namespace Bim2Gltf.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index() => Ok("Welcome to BIM2GLTF converter!");
    }
}
