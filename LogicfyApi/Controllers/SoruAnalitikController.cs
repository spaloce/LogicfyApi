using LogicfyApi.Data;
using LogicfyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogicfyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SoruAnalitikController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SoruAnalitikController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: TÜM SORU ANALİTİKLERİ
        // =====================================================

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _context.SoruAnalitikler
                .Include(x => x.Soru)
                .Select(x => new
                {
                    x.Id,
                    x.SoruId,
                    x.CevaplanmaSayisi,
                    x.DogruSayisi,
                    x.YanlisSayisi,
                    x.OrtalamaSure,
                    Soru = new { x.Soru.Id, x.Soru.SoruMetni },
                    x.CreatedAt,
                    x.UpdatedAt
                })
                .ToList();

            return Ok(list);
        }

        // =====================================================
        // GET: SORU ID’YE GÖREA ANALİTİK
        // =====================================================

        [HttpGet("soru/{soruId}")]
        public IActionResult GetBySoruId(int soruId)
        {
            var analitik = _context.SoruAnalitikler
                .Include(x => x.Soru)
                .FirstOrDefault(x => x.SoruId == soruId);

            if (analitik == null)
                return NotFound(new { message = "Bu soruya ait analitik bulunamadı" });

            return Ok(new
            {
                analitik.Id,
                analitik.SoruId,
                analitik.CevaplanmaSayisi,
                analitik.DogruSayisi,
                analitik.YanlisSayisi,
                analitik.OrtalamaSure,
                Soru = new { analitik.Soru.Id, analitik.Soru.SoruMetni }
            });
        }

        // =====================================================
        // POST: ANALİTİK OLUŞTURMA / GÜNCELLEME
        // =====================================================

        [HttpPost("kaydet")]
        public async Task<IActionResult> Upsert([FromBody] SoruAnalitik request)
        {
            if (request == null || request.SoruId <= 0)
                return BadRequest(new { message = "SoruId gereklidir" });

            var existing = _context.SoruAnalitikler
                .FirstOrDefault(x => x.SoruId == request.SoruId);

            if (existing == null)
            {
                var yeni = new SoruAnalitik
                {
                    SoruId = request.SoruId,
                    CevaplanmaSayisi = request.CevaplanmaSayisi,
                    DogruSayisi = request.DogruSayisi,
                    YanlisSayisi = request.YanlisSayisi,
                    OrtalamaSure = request.OrtalamaSure
                };

                _context.SoruAnalitikler.Add(yeni);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Analitik oluşturuldu", yeni });
            }
            else
            {
                existing.CevaplanmaSayisi = request.CevaplanmaSayisi;
                existing.DogruSayisi = request.DogruSayisi;
                existing.YanlisSayisi = request.YanlisSayisi;
                existing.OrtalamaSure = request.OrtalamaSure;
                existing.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Analitik güncellendi", existing });
            }
        }

        // =====================================================
        // DELETE
        // =====================================================

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var analitik = _context.SoruAnalitikler.Find(id);
            if (analitik == null)
                return NotFound(new { message = "Analitik kaydı bulunamadı" });

            _context.SoruAnalitikler.Remove(analitik);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Analitik silindi" });
        }
    }
}
