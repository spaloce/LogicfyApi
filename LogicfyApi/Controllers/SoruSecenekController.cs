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
    public class SoruSecenekController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SoruSecenekController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("soru/{soruId}")]
        public IActionResult GetBySoruId(int soruId)
        {
            var secenekler = _context.SoruSecenekleri
                .Where(x => x.SoruId == soruId)
                .Select(x => new
                {
                    x.Id,
                    x.SoruId,
                    x.SecenekMetni,
                    x.CreatedAt
                })
                .ToList();

            if (!secenekler.Any())
                return NotFound(new { message = "Bu soru için seçenek bulunamadı" });

            return Ok(secenekler);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSoruSecenekRequest request)
        {
            var soru = _context.Sorular.Find(request.SoruId);
            if (soru == null)
                return BadRequest(new { message = "Soru bulunamadı" });

            var secenek = new SoruSecenek
            {
                SoruId = request.SoruId,
                SecenekMetni = request.SecenekMetni
            };

            _context.SoruSecenekleri.Add(secenek);
            await _context.SaveChangesAsync();

            // Eğer bu doğru cevapsa, Soru'da DogruCevapId'yi güncelle
            if (request.IsDogruCevap)
            {
                soru.DogruCevapId = secenek.Id;
                _context.Sorular.Update(soru);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetById), new { id = secenek.Id }, new
            {
                secenek.Id,
                secenek.SoruId,
                secenek.SecenekMetni,
                secenek.CreatedAt
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var secenek = _context.SoruSecenekleri.Find(id);
            if (secenek == null)
                return NotFound(new { message = "Seçenek bulunamadı" });

            return Ok(new
            {
                secenek.Id,
                secenek.SoruId,
                secenek.SecenekMetni,
                secenek.CreatedAt
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSoruSecenekRequest request)
        {
            var secenek = _context.SoruSecenekleri.Find(id);
            if (secenek == null)
                return NotFound(new { message = "Seçenek bulunamadı" });

            if (!string.IsNullOrWhiteSpace(request.SecenekMetni))
                secenek.SecenekMetni = request.SecenekMetni;

            _context.SoruSecenekleri.Update(secenek);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                secenek.Id,
                secenek.SecenekMetni,
                message = "Seçenek güncellendi"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var secenek = _context.SoruSecenekleri.Find(id);
            if (secenek == null)
                return NotFound(new { message = "Seçenek bulunamadı" });

            _context.SoruSecenekleri.Remove(secenek);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Seçenek silindi" });
        }
    }

}
