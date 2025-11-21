using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace LogicfyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize(Roles = "Admin")]
    public class FileController : ControllerBase
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp" };
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Dosya seçilmedi" });

            if (file.Length > _maxFileSize)
                return BadRequest(new { message = "Dosya boyutu 5MB'dan küçük olmalıdır" });

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Sadece JPG, PNG, GIF, SVG ve WebP formatları desteklenir" });

            try
            {
                // wwwroot/uploads klasörüne kaydet
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUrl = $"/uploads/{uniqueFileName}";

                return Ok(new
                {
                    success = true,
                    fileUrl,
                    fileName = uniqueFileName,
                    message = "Dosya başarıyla yüklendi"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Dosya yüklenirken bir hata oluştu", error = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteFile([FromBody] DeleteFileRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FileUrl))
                return BadRequest(new { message = "Dosya URL'si gereklidir" });

            try
            {
                var fileName = Path.GetFileName(request.FileUrl);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok(new { success = true, message = "Dosya başarıyla silindi" });
                }

                return NotFound(new { message = "Dosya bulunamadı" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Dosya silinirken bir hata oluştu", error = ex.Message });
            }
        }
    }

    public class DeleteFileRequest
    {
        public string FileUrl { get; set; }
    }
}