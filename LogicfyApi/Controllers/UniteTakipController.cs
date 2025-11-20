using LogicfyApi.Data;
using LogicfyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogicfyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniteTakipController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UniteTakipController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: TÜM ÜNİTE TAKİPLERİ
        // =====================================================

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _context.UniteTakipler
                .Include(x => x.Unite)
                .Select(x => new
                {
                    x.Id,
                    x.UniteId,
                    x.TakipEdenKullaniciSayisi,
                    Unite = new { x.Unite.Id, x.Unite.Baslik, x.Unite.Sira },
                    x.CreatedAt,
                    x.UpdatedAt
                })
                .ToList();

            return Ok(list);
        }

        // =====================================================
        // GET: ÜNİTE ID'YE GÖRE TAKİP
        // =====================================================

        [HttpGet("unite/{uniteId}")]
        public IActionResult GetByUniteId(int uniteId)
        {
            var takip = _context.UniteTakipler
                .Include(x => x.Unite)
                .FirstOrDefault(x => x.UniteId == uniteId);

            if (takip == null)
                return NotFound(new { message = "Bu üniteye ait takip kaydı bulunamadı" });

            return Ok(new
            {
                takip.Id,
                takip.UniteId,
                takip.TakipEdenKullaniciSayisi,
                Unite = new
                {
                    takip.Unite.Id,
                    takip.Unite.Baslik,
                    takip.Unite.Sira
                }
            });
        }

        // =====================================================
        // POST: TAKİP ARTTIRMA
        // =====================================================

        [HttpPost("takip-arttir/{uniteId}")]
        public async Task<IActionResult> Increment(int uniteId)
        {
            var takip = _context.UniteTakipler.FirstOrDefault(x => x.UniteId == uniteId);

            if (takip == null)
            {
                takip = new UniteTakip
                {
                    UniteId = uniteId,
                    TakipEdenKullaniciSayisi = 1
                };

                _context.UniteTakipler.Add(takip);
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

        [HttpPost("takip-azalt/{uniteId}")]
        public async Task<IActionResult> Decrement(int uniteId)
        {
            var takip = _context.UniteTakipler.FirstOrDefault(x => x.UniteId == uniteId);

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
            var takip = _context.UniteTakipler.Find(id);
            if (takip == null)
                return NotFound(new { message = "Takip kaydı bulunamadı" });

            _context.UniteTakipler.Remove(takip);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Takip kaydı silindi" });
        }
    }
}
