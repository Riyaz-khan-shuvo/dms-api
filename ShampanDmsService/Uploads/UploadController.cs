using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShampanDmsService.uploads;

namespace ShampanDmsService.Uploads
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpPost]
        public IActionResult UploadFile(IFormFile file) {
        return Ok(new UploadHandler().Upload(file));
        }
    }
}
