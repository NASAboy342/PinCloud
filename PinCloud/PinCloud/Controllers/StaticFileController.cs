using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace APinI.Controllers;

[ApiController]
public class StaticFileController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _uploadsPath;

    public StaticFileController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
    {
        _webHostEnvironment = webHostEnvironment;
        
        // Use external uploads path from configuration or default to a persistent location
        var uploadsBasePath = configuration["FileUpload:BasePath"] ?? 
                             Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        
        _uploadsPath = Path.Combine(uploadsBasePath, "APinI_Uploads");
    }

    [HttpGet("files/{*filePath}")]
    public IActionResult GetFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return BadRequest("File path is required.");
        }

        try
        {
            var fullPath = Path.Combine(_uploadsPath, filePath);
            
            // Security check to prevent directory traversal
            if (!fullPath.StartsWith(_uploadsPath))
            {
                return BadRequest("Invalid file path.");
            }

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("File not found.");
            }

            // Get the content type
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fullPath, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            // Read and return the file for inline viewing
            var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var fileName = Path.GetFileName(fullPath);
            var fileExtension = Path.GetExtension(fullPath).ToLowerInvariant();

            // Set headers for inline viewing
            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");
            
            // Add cache headers for better performance
            Response.Headers.Add("Cache-Control", "public, max-age=3600");
            
            // Set specific headers for different file types to ensure proper browser handling
            switch (fileExtension)
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                case ".bmp":
                case ".webp":
                    // Images should display inline
                    break;
                case ".mp3":
                case ".wav":
                case ".ogg":
                case ".flac":
                case ".aac":
                    // Audio files should have proper MIME type for browser players
                    break;
                case ".pdf":
                    // PDFs should open in browser viewer
                    break;
                case ".txt":
                    // Text files should display as plain text
                    contentType = "text/plain; charset=utf-8";
                    break;
                default:
                    // For other files, keep the detected content type
                    break;
            }
            
            return File(fileStream, contentType);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving file: {ex.Message}");
        }
    }

    [HttpGet("{fileType}/{fileName}")]
    public IActionResult GetFileByType(string fileType, string fileName)
    {
        // This allows access like /png/image.png, /mp3/audio.mp3, etc.
        var filePath = $"{fileType}/{fileName}";
        return GetFile(filePath);
    }
}
