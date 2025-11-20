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
    public class SoruKelimeBlokController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SoruKelimeBlokController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("soru/{soruId}")]
        public IActionResult GetBySoruId(int soruId)
        {
            var kelimeBloklar = _context.SoruKelimeBloklar
                .Where(x => x.SoruId == soruId)
                .Select(x => new
                {
                    x.Id,
                    x.SoruId,
                    x.DogruKod,
                    x.KelimelerJson,
                    x.CreatedAt
                })
                .ToList();

            if (!kelimeBloklar.Any())
                return NotFound(new { message = "Bu soru için kelime blok bulunamadı" });

            return Ok(kelimeBloklar);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSoruKelimeBlokRequest request)
        {
            var soru = _context.Sorular.Find(request.SoruId);
            if (soru == null)
                return BadRequest(new { message = "Soru bulunamadı" });

            var kelimeBlok = new SoruKelimeBlok
            {
                SoruId = request.SoruId,
                DogruKod = request.DogruKod,
                KelimelerJson = request.KelimelerJson
            };

            _context.SoruKelimeBloklar.Add(kelimeBlok);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = kelimeBlok.Id }, new
            {
                kelimeBlok.Id,
                kelimeBlok.SoruId,
                kelimeBlok.DogruKod,
                kelimeBlok.CreatedAt
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var kelimeBlok = _context.SoruKelimeBloklar.Find(id);
            if (kelimeBlok == null)
                return NotFound(new { message = "Kelime blok bulunamadı" });

            return Ok(new
            {
                kelimeBlok.Id,
                kelimeBlok.SoruId,
                kelimeBlok.DogruKod,
                kelimeBlok.KelimelerJson,
                kelimeBlok.CreatedAt
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSoruKelimeBlokRequest request)
        {
            var kelimeBlok = _context.SoruKelimeBloklar.Find(id);
            if (kelimeBlok == null)
                return NotFound(new { message = "Kelime blok bulunamadı" });

            if (!string.IsNullOrWhiteSpace(request.DogruKod))
                kelimeBlok.DogruKod = request.DogruKod;

            if (!string.IsNullOrWhiteSpace(request.KelimelerJson))
                kelimeBlok.KelimelerJson = request.KelimelerJson;

            _context.SoruKelimeBloklar.Update(kelimeBlok);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                kelimeBlok.Id,
                kelimeBlok.DogruKod,
                message = "Kelime blok güncellendi"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var kelimeBlok = _context.SoruKelimeBloklar.Find(id);
            if (kelimeBlok == null)
                return NotFound(new { message = "Kelime blok bulunamadı" });

            _context.SoruKelimeBloklar.Remove(kelimeBlok);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kelime blok silindi" });
        }
    }
}
