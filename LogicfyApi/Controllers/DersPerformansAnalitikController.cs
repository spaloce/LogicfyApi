using LogicfyApi.Data;
using LogicfyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogicfyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DersPerformansAnalitikController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DersPerformansAnalitikController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: TÜM DERS ANALİTİKLERİ
        // =====================================================

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _context.DersPerformansAnalitikler
                .Include(x => x.Ders)
                .Select(x => new
                {
                    x.Id,
                    x.DersId,
                    x.OrtalamaTamamlamaSuresi,
                    x.OrtalamaDogruOrani,
                    x.EnZorSoruId,
                    Ders = new { x.Ders.Id, x.Ders.Baslik },
                    x.CreatedAt,
                    x.UpdatedAt
                })
                .ToList();

            return Ok(list);
        }

        // =====================================================
        // GET: DERS ID’YE GÖRE ANALİTİK
        // =====================================================

        [HttpGet("ders/{dersId}")]
        public IActionResult GetByDersId(int dersId)
        {
            var analitik = _context.DersPerformansAnalitikler
                .Include(x => x.Ders)
                .FirstOrDefault(x => x.DersId == dersId);

            if (analitik == null)
                return NotFound(new { message = "Bu derse ait performans analitiği bulunamadı" });

            return Ok(new
            {
                analitik.Id,
                analitik.DersId,
                analitik.OrtalamaTamamlamaSuresi,
                analitik.OrtalamaDogruOrani,
                analitik.EnZorSoruId,
                Ders = new
                {
                    analitik.Ders.Id,
                    analitik.Ders.Baslik,
                    analitik.Ders.Sira
                }
            });
        }

        // =====================================================
        // POST: ANALİTİK OLUŞTURMA / GÜNCELLEME
        // =====================================================

        [HttpPost("kaydet")]
        public async Task<IActionResult> Upsert([FromBody] DersPerformansAnalitik request)
        {
            if (request == null || request.DersId <= 0)
                return BadRequest(new { message = "DersId gereklidir" });

            var existing = _context.DersPerformansAnalitikler
                .FirstOrDefault(x => x.DersId == request.DersId);

            if (existing == null)
            {
                var yeni = new DersPerformansAnalitik
                {
                    DersId = request.DersId,
                    OrtalamaTamamlamaSuresi = request.OrtalamaTamamlamaSuresi,
                    OrtalamaDogruOrani = request.OrtalamaDogruOrani,
                    EnZorSoruId = request.EnZorSoruId
                };

                _context.DersPerformansAnalitikler.Add(yeni);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Analitik oluşturuldu", yeni });
            }
            else
            {
                existing.OrtalamaTamamlamaSuresi = request.OrtalamaTamamlamaSuresi;
                existing.OrtalamaDogruOrani = request.OrtalamaDogruOrani;
                existing.EnZorSoruId = request.EnZorSoruId;
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
            var analitik = _context.DersPerformansAnalitikler.Find(id);
            if (analitik == null)
                return NotFound(new { message = "Analitik kaydı bulunamadı" });

            _context.DersPerformansAnalitikler.Remove(analitik);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Analitik silindi" });
        }
    }
}
