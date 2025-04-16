using Bim2Gltf.Core;
using Microsoft.AspNetCore.Mvc;

namespace Bim2Gltf.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversionController : ControllerBase
    {
        private readonly string _tempFolder = Path.Combine(Path.GetTempPath(), "Bim2Gltf");

        public ConversionController()
        {
            Directory.CreateDirectory(_tempFolder);
        }

        [HttpPost]
        public async Task<IActionResult> Convert([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (!file.FileName.EndsWith(".ifc", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid file format. Only .ifc is supported.");
            }

            string inputFilePath = Path.Combine(_tempFolder, $"{Guid.NewGuid()}.ifc");

            try
            {
                using (var stream = new FileStream(inputFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string outputGlbFilePath = GltfHelper.ConvertIfc(inputFilePath);

                var glbBytes = await System.IO.File.ReadAllBytesAsync(outputGlbFilePath);

                return File(glbBytes, "model/gltf-binary", Path.GetFileName(outputGlbFilePath));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error. Failed conversion: {ex.Message}");
            }
        }
    }
}
