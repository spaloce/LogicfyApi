using LogicfyApi.Data;
using LogicfyApi.Models;
using LogicfyApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LogicfyApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class SoruCanliPreviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SoruCanliPreviewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("soru/{soruId}")]
        public IActionResult GetBySoruId(int soruId)
        {
            var previews = _context.SoruCanliPreviews
                .Where(x => x.SoruId == soruId)
                .Select(x => new
                {
                    x.Id,
                    x.SoruId,
                    x.DogruHtml,
                    x.DogruCss,
                    x.GerekenEtiketlerJson,
                    x.GerekenStillerJson,
                    x.CreatedAt
                })
                .ToList();

            if (!previews.Any())
                return NotFound(new { message = "Bu soru için preview bulunamadı" });

            return Ok(previews);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSoruCanliPreviewRequest request)
        {
            var soru = _context.Sorular.Find(request.SoruId);
            if (soru == null)
                return BadRequest(new { message = "Soru bulunamadı" });

            var preview = new SoruCanliPreview
            {
                SoruId = request.SoruId,
                DogruHtml = request.DogruHtml,
                DogruCss = request.DogruCss,
                GerekenEtiketlerJson = request.GerekenEtiketlerJson,
                GerekenStillerJson = request.GerekenStillerJson
            };

            _context.SoruCanliPreviews.Add(preview);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = preview.Id }, new
            {
                preview.Id,
                preview.SoruId,
                preview.CreatedAt
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var preview = _context.SoruCanliPreviews.Find(id);
            if (preview == null)
                return NotFound(new { message = "Preview bulunamadı" });

            return Ok(new
            {
                preview.Id,
                preview.SoruId,
                preview.DogruHtml,
                preview.DogruCss,
                preview.GerekenEtiketlerJson,
                preview.GerekenStillerJson,
                preview.CreatedAt
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSoruCanliPreviewRequest request)
        {
            var preview = _context.SoruCanliPreviews.Find(id);
            if (preview == null)
                return NotFound(new { message = "Preview bulunamadı" });

            if (!string.IsNullOrWhiteSpace(request.DogruHtml))
                preview.DogruHtml = request.DogruHtml;

            if (!string.IsNullOrWhiteSpace(request.DogruCss))
                preview.DogruCss = request.DogruCss;

            if (!string.IsNullOrWhiteSpace(request.GerekenEtiketlerJson))
                preview.GerekenEtiketlerJson = request.GerekenEtiketlerJson;

            if (!string.IsNullOrWhiteSpace(request.GerekenStillerJson))
                preview.GerekenStillerJson = request.GerekenStillerJson;

            _context.SoruCanliPreviews.Update(preview);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                preview.Id,
                message = "Preview güncellendi"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var preview = _context.SoruCanliPreviews.Find(id);
            if (preview == null)
                return NotFound(new { message = "Preview bulunamadı" });

            _context.SoruCanliPreviews.Remove(preview);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Preview silindi" });
        }
    }

}
