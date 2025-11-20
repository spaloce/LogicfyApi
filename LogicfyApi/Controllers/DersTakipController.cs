using LogicfyApi.Data;
using LogicfyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogicfyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DersTakipController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DersTakipController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: TÜM DERS TAKİPLERİ
        // =====================================================

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _context.DersTakipler
                .Include(x => x.Ders)
                .Select(x => new
                {
                    x.Id,
                    x.DersId,
                    x.TakipEdenKullaniciSayisi,
                    Ders = new { x.Ders.Id, x.Ders.Baslik },
                    x.CreatedAt,
                    x.UpdatedAt
                })
                .ToList();

            return Ok(list);
        }

        // =====================================================
        // GET: DERS ID'YE GÖRE TAKİP
        // =====================================================

        [HttpGet("ders/{dersId}")]
        public IActionResult GetByDersId(int dersId)
        {
            var takip = _context.DersTakipler
                .Include(x => x.Ders)
                .FirstOrDefault(x => x.DersId == dersId);

            if (takip == null)
                return NotFound(new { message = "Bu derse ait takip kaydı bulunamadı" });

            return Ok(new
            {
                takip.Id,
                takip.DersId,
                takip.TakipEdenKullaniciSayisi,
                Ders = new
                {
                    takip.Ders.Id,
                    takip.Ders.Baslik,
                    takip.Ders.Sira
                }
            });
        }

        // =====================================================
        // POST: TAKİP ARTTIRMA
        // =====================================================

        [HttpPost("takip-arttir/{dersId}")]
        public async Task<IActionResult> Increment(int dersId)
        {
            var takip = _context.DersTakipler.FirstOrDefault(x => x.DersId == dersId);

            if (takip == null)
            {
                takip = new DersTakip
                {
                    DersId = dersId,
                    TakipEdenKullaniciSayisi = 1
                };

                _context.DersTakipler.Add(takip);
            }
            else
            {
                takip.TakipEdenKullaniciSayisi++;
                takip.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Ok(takip);
        }

        // =====================================================
        // POST: TAKİP AZALTMA
        // =====================================================

        [HttpPost("takip-azalt/{dersId}")]
        public async Task<IActionResult> Decrement(int dersId)
        {
            var takip = _context.DersTakipler.FirstOrDefault(x => x.DersId == dersId);

            if (takip == null)
                return NotFound(new { message = "Takip kaydı bulunamadı" });

            if (takip.TakipEdenKullaniciSayisi > 0)
                takip.TakipEdenKullaniciSayisi--;

            takip.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(takip);
        }

        // =====================================================
        // DELETE
        // =====================================================

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var takip = _context.DersTakipler.Find(id);
            if (takip == null)
                return NotFound(new { message = "Takip kaydı bulunamadı" });

            _context.DersTakipler.Remove(takip);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Takip kaydı silindi" });
        }
    }
}
