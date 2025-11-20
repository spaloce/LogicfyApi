using LogicfyApi.Data;
using LogicfyApi.Models;
using LogicfyApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogicfyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SoruController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Kullanici> _userManager;

        public SoruController(AppDbContext context, UserManager<Kullanici> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ===== GET - SORULAR LISTELEME =====

        [HttpGet]
        public IActionResult GetAll()
        {
            var sorular = _context.Sorular
                .Include(x => x.Ders)
                .Include(x => x.Secenekler)
                .Include(x => x.DogruCevap)
                .Select(x => new
                {
                    x.Id,
                    x.DersId,
                    x.SoruTipi,
                    x.SoruMetni,
                    x.KodMetni,
                    x.Seviye,
                    x.DogruCevapId,
                    x.EkVeriJson,
                    x.CreatedAt,
                    x.UpdatedAt,
                    Ders = new { x.Ders.Id, x.Ders.Baslik },
                    Secenekler = x.Secenekler.Select(s => new
                    {
                        s.Id,
                        s.SecenekMetni,
                        s.CreatedAt
                    }),
                    DogruCevap = x.DogruCevap != null ? new
                    {
                        x.DogruCevap.Id,
                        x.DogruCevap.SecenekMetni
                    } : null
                })
                .ToList();

            return Ok(sorular);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var soru = _context.Sorular
                .Include(x => x.Ders)
                .Include(x => x.Secenekler)
                .Include(x => x.DogruCevap)
                .Include(x => x.KelimeBloklar)
                .Include(x => x.FonksiyonCozumler)
                .Include(x => x.CanliPreviews)
                .FirstOrDefault(x => x.Id == id);

            if (soru == null)
                return NotFound(new { message = "Soru bulunamadı" });

            return Ok(new
            {
                soru.Id,
                soru.DersId,
                soru.SoruTipi,
                soru.SoruMetni,
                soru.KodMetni,
                soru.Seviye,
                soru.DogruCevapId,
                soru.EkVeriJson,
                soru.CreatedAt,
                soru.UpdatedAt,
                Ders = new { soru.Ders.Id, soru.Ders.Baslik },
                Secenekler = soru.Secenekler.Select(s => new
                {
                    s.Id,
                    s.SecenekMetni,
                    s.CreatedAt
                }),
                DogruCevap = soru.DogruCevap != null ? new
                {
                    soru.DogruCevap.Id,
                    soru.DogruCevap.SecenekMetni
                } : null,
                KelimeBloklar = soru.KelimeBloklar.Select(k => new
                {
                    k.Id,
                    k.DogruKod,
                    k.KelimelerJson
                }),
                FonksiyonCozumler = soru.FonksiyonCozumler.Select(f => new
                {
                    f.Id,
                    f.CozumKod
                }),
                CanliPreviews = soru.CanliPreviews.Select(c => new
                {
                    c.Id,
                    c.DogruHtml,
                    c.DogruCss,
                    c.GerekenEtiketlerJson,
                    c.GerekenStillerJson
                })
            });
        }

        [HttpGet("ders/{dersId}")]
        public IActionResult GetByDersId(int dersId)
        {
            var sorular = _context.Sorular
                .Where(x => x.DersId == dersId)
                .Include(x => x.Secenekler)
                .Include(x => x.DogruCevap)
                .Select(x => new
                {
                    x.Id,
                    x.SoruTipi,
                    x.SoruMetni,
                    x.Seviye,
                    x.CreatedAt,
                    Secenekler = x.Secenekler.Select(s => new
                    {
                        s.Id,
                        s.SecenekMetni
                    })
                })
                .ToList();

            if (!sorular.Any())
                return NotFound(new { message = "Bu derse ait soru bulunamadı" });

            return Ok(sorular);
        }

        // ===== CREATE - SORU OLUŞTURMA =====

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSoruRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.SoruMetni))
                return BadRequest(new { message = "Soru metni gereklidir" });

            var ders = _context.Dersler.Find(request.DersId);
            if (ders == null)
                return BadRequest(new { message = "Ders bulunamadı" });

            var soru = new Soru
            {
                DersId = request.DersId,
                SoruTipi = request.SoruTipi,
                SoruMetni = request.SoruMetni,
                KodMetni = request.KodMetni,
                Seviye = request.Seviye,
                EkVeriJson = request.EkVeriJson
            };

            _context.Sorular.Add(soru);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = soru.Id }, new
            {
                soru.Id,
                soru.DersId,
                soru.SoruTipi,
                soru.SoruMetni,
                soru.Seviye,
                soru.CreatedAt
            });
        }

        // ===== UPDATE - SORU GÜNCELLEME =====

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSoruRequest request)
        {
            var soru = _context.Sorular.Find(id);
            if (soru == null)
                return NotFound(new { message = "Soru bulunamadı" });

            if (!string.IsNullOrWhiteSpace(request.SoruMetni))
                soru.SoruMetni = request.SoruMetni;

            if (!string.IsNullOrWhiteSpace(request.KodMetni))
                soru.KodMetni = request.KodMetni;

            if (request.Seviye > 0)
                soru.Seviye = request.Seviye;

            if (!string.IsNullOrWhiteSpace(request.EkVeriJson))
                soru.EkVeriJson = request.EkVeriJson;

            soru.UpdatedAt = DateTime.Now;

            _context.Sorular.Update(soru);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                soru.Id,
                soru.SoruMetni,
                soru.KodMetni,
                soru.Seviye,
                soru.UpdatedAt,
                message = "Soru güncellendi"
            });
        }

        // ===== DELETE - SORU SİLME =====

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var soru = _context.Sorular.Find(id);
            if (soru == null)
                return NotFound(new { message = "Soru bulunamadı" });

            _context.Sorular.Remove(soru);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Soru silindi" });
        }
    }
}
