using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace APinI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _uploadsPath;

    public FileController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
    {
        _webHostEnvironment = webHostEnvironment;
        
        // Use external uploads path from configuration or default to a persistent location
        var uploadsBasePath = configuration["FileUpload:BasePath"] ?? 
                             Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        
        _uploadsPath = Path.Combine(uploadsBasePath, "APinI_Uploads");
        
        // Ensure uploads directory exists
        if (!Directory.Exists(_uploadsPath))
        {
            Directory.CreateDirectory(_uploadsPath);
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string? subfolder = null)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided or file is empty.");
        }

        // Validate file size (limit to 50MB)
        if (file.Length > 50 * 1024 * 1024)
        {
            return BadRequest("File size exceeds 50MB limit.");
        }

        // Get file extension and validate
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".mp3", ".wav", ".flac", ".aac", ".ogg", ".pdf", ".txt", ".doc", ".docx" };
        
        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest($"File type {fileExtension} is not allowed.");
        }

        try
        {
            // Create subfolder path if specified
            var targetDirectory = _uploadsPath;
            if (!string.IsNullOrEmpty(subfolder))
            {
                // Sanitize subfolder name
                subfolder = string.Join("_", subfolder.Split(Path.GetInvalidFileNameChars()));
                targetDirectory = Path.Combine(_uploadsPath, subfolder);
                
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
            }

            // Generate unique filename to avoid conflicts
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(targetDirectory, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative path for accessing the file
            var relativePath = string.IsNullOrEmpty(subfolder) 
                ? fileName 
                : $"{subfolder}/{fileName}";

            return Ok(new
            {
                message = "File uploaded successfully",
                fileName = fileName,
                originalFileName = file.FileName,
                size = file.Length,
                path = relativePath,
                accessUrl = $"{Request.Scheme}://{Request.Host}/files/{relativePath}"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error uploading file: {ex.Message}");
        }
    }

    [HttpGet("list")]
    public IActionResult ListFiles([FromQuery] string? subfolder = null)
    {
        try
        {
            var targetDirectory = string.IsNullOrEmpty(subfolder) 
                ? _uploadsPath 
                : Path.Combine(_uploadsPath, subfolder);

            if (!Directory.Exists(targetDirectory))
            {
                return NotFound("Directory not found.");
            }

            var files = Directory.GetFiles(targetDirectory)
                .Select(f => new
                {
                    fileName = Path.GetFileName(f),
                    size = new FileInfo(f).Length,
                    lastModified = new FileInfo(f).LastWriteTime,
                    path = string.IsNullOrEmpty(subfolder) 
                        ? Path.GetFileName(f) 
                        : $"{subfolder}/{Path.GetFileName(f)}",
                    accessUrl = $"{Request.Scheme}://{Request.Host}/files/{(string.IsNullOrEmpty(subfolder) ? Path.GetFileName(f) : $"{subfolder}/{Path.GetFileName(f)}")}"
                })
                .ToList();

            return Ok(files);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error listing files: {ex.Message}");
        }
    }

    [HttpDelete("{*filePath}")]
    public IActionResult DeleteFile(string filePath)
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

            System.IO.File.Delete(fullPath);
            return Ok(new { message = "File deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting file: {ex.Message}");
        }
    }
}
