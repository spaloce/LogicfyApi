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
    public class SoruFonksiyonCozumController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SoruFonksiyonCozumController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("soru/{soruId}")]
        public IActionResult GetBySoruId(int soruId)
        {
            var cozumler = _context.SoruFonksiyonCozumler
                .Where(x => x.SoruId == soruId)
                .Select(x => new
                {
                    x.Id,
                    x.SoruId,
                    x.CozumKod,
                    x.CreatedAt
                })
                .ToList();

            if (!cozumler.Any())
                return NotFound(new { message = "Bu soru için çözüm bulunamadı" });

            return Ok(cozumler);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSoruFonksiyonCozumRequest request)
        {
            var soru = _context.Sorular.Find(request.SoruId);
            if (soru == null)
                return BadRequest(new { message = "Soru bulunamadı" });

            var cozum = new SoruFonksiyonCozum
            {
                SoruId = request.SoruId,
                CozumKod = request.CozumKod
            };

            _context.SoruFonksiyonCozumler.Add(cozum);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = cozum.Id }, new
            {
                cozum.Id,
                cozum.SoruId,
                cozum.CozumKod,
                cozum.CreatedAt
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var cozum = _context.SoruFonksiyonCozumler.Find(id);
            if (cozum == null)
                return NotFound(new { message = "Çözüm bulunamadı" });

            return Ok(new
            {
                cozum.Id,
                cozum.SoruId,
                cozum.CozumKod,
                cozum.CreatedAt
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSoruFonksiyonCozumRequest request)
        {
            var cozum = _context.SoruFonksiyonCozumler.Find(id);
            if (cozum == null)
                return NotFound(new { message = "Çözüm bulunamadı" });

            if (!string.IsNullOrWhiteSpace(request.CozumKod))
                cozum.CozumKod = request.CozumKod;

            _context.SoruFonksiyonCozumler.Update(cozum);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                cozum.Id,
                cozum.CozumKod,
                message = "Çözüm güncellendi"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var cozum = _context.SoruFonksiyonCozumler.Find(id);
            if (cozum == null)
                return NotFound(new { message = "Çözüm bulunamadı" });

            _context.SoruFonksiyonCozumler.Remove(cozum);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Çözüm silindi" });
        }
    }
}
