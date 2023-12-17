using Microsoft.AspNetCore.Mvc;
using delivery.Helpers;


namespace delivery.Controllers;

[ApiController]
[Route("media")]
public class MediaController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    private ILogger<ProductController> _logger;

    public MediaController(ILogger<ProductController> logger, IWebHostEnvironment environment)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetImage([FromQuery] string path)
    {;   
        FileStream fileStream = new FileStream(
            Path.Combine(
                _environment.WebRootPath,
                path
            ),
            FileMode.Open
        );

        string extension = Path.GetExtension(path);
        return new FileStreamResult(fileStream, GetContentType(extension));
    }

    public string GetContentType(string extension)
    {
        switch (Path.GetExtension(extension))
        {
            case ".bmp": return "Image/bmp";
            case ".gif": return "Image/gif";
            case ".jpg": return "Image/jpeg";
            case ".png": return "Image/png";
            default: break;
        }
        return "";
    }
}