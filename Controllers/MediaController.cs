using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MediaController : ControllerBase
{
    private const string MimeType = "image/jpeg";
    private readonly string _uploadPath;
    string basePath;
    public MediaController(IConfiguration configuration)
    {
        string upload = configuration.GetSection("Upload-Path").Get<string>() ?? throw new Exception("Upload Path Doesn't Exists in appsettings.json");

        basePath = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? throw new Exception("Error in Find Base Directory");
        _uploadPath = Path.Combine(basePath, upload);
    }

    [SwaggerOperation("View Default Image", "Return a Default Image Named gojo.jpg")]
    [SwaggerResponse(200, "Successfully", typeof(PhysicalFileResult))]
    [SwaggerResponse(404, "Failed, File not found.")]
    [HttpGet]
    public IActionResult ViewMedia()
    {
        string filePath = Path.Combine(_uploadPath, "gojo.jpg");

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        return PhysicalFile(filePath, MimeType);
    }


    [SwaggerOperation("View Image By Name", "Return a Image Based on Name from upload folder")]
    [SwaggerResponse(200, "Successfully", typeof(PhysicalFileResult))]
    [SwaggerResponse(404, "Failed, File not found.")]
    [HttpGet("{id}")]
    public IActionResult ViewProductImage(string id)
    {
        string filePath = Path.Combine(_uploadPath, id);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File not found.");
        }

        return PhysicalFile(filePath, MimeType);
    }
}
