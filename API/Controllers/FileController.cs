using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly FileService _fileService;

    public FileController(FileService fileService)
    {
        _fileService = fileService;
    }

    // 1. DOSYA YÜKLEME
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromQuery] string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return BadRequest("Dosya yolu boş.");
        try
        {
            await _fileService.UploadFileAsync(filePath);
            return Ok(new { message = "Başarıyla yüklendi", path = filePath });
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    // 2. GÖRÜNTÜLEME LİNKİ AL (Presigned URL)
    [HttpGet("view-link")]
    public async Task<IActionResult> GetViewLink([FromQuery] string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return BadRequest("Dosya adı boş.");
        try
        {
            var url = await _fileService.GetPresignedUrlAsync(fileName);
            return Ok(new { downloadUrl = url });
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    // 3. DOĞRUDAN İNDİRME (Stream)
    [HttpGet("download-direct/{fileName}")]
    public async Task<IActionResult> DownloadDirect(string fileName)
    {
        try
        {
            var stream = await _fileService.GetFileAsync(fileName);
            // Dosyayı tarayıcıya akış olarak gönderiyoruz
            return File(stream, "application/octet-stream", fileName);
        }
        catch (Exception ex) { return NotFound(ex.Message); }
    }
}