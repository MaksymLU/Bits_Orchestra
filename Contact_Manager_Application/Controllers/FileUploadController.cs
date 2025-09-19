using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Contact_Manager_Application.Interfaces;
namespace Contact_Manager_Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileProcessingService _fileProcessingService;

        public FileUploadController(IFileProcessingService fileProcessingService)
        {
            _fileProcessingService = fileProcessingService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Success = false, Error = "No file uploaded." });

            var result = await _fileProcessingService.ProcessFileAsync(file);

            if (!result.Success)
                return BadRequest(new { Success = false, Error = result.ErrorMessage });

            return Ok(new { Success = true, Message = "File processed successfully." });
        }

    }
}
