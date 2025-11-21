using LogicfyApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogicfyApi.Controllers.Frontend
{
    [ApiController]
    [Route("api/frontend")]
    public class FrontendController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FrontendController(AppDbContext context)
        {
            _context = context;
        }

        // ================================================
        // 1) Kısım Sayfası için Tek Uç (Full Chain Data)
        // ================================================
        [HttpGet("kisim/{kisimId}/full")]
        public async Task<IActionResult> GetFullSectionData(int kisimId)
        {
            var kisim = await _context.Kisimlar
                .Include(k => k.Unite)
                .ThenInclude(u => u.ProgramlamaDili)
                .FirstOrDefaultAsync(k => k.Id == kisimId);

            if (kisim == null)
                return NotFound("Kısım bulunamadı.");

            // İçerik
            var icerik = await _context.KisimIcerikler
                .FirstOrDefaultAsync(i => i.KisimId == kisimId);

            // Dersler
            var dersler = await _context.Dersler
                .Where(d => d.KisimId == kisimId)
                .OrderBy(d => d.Sira)
                .Select(d => new
                {
                    d.Id,
                    d.Baslik,
                    d.Sira,
                    SoruSayisi = d.SoruSayisiCache
                })
                .ToListAsync();

            return Ok(new
            {
                programlamaDili = new
                {
                    kisim.Unite.ProgramlamaDili.Id,
                    kisim.Unite.ProgramlamaDili.Ad,
                    kisim.Unite.ProgramlamaDili.Kod,
                    kisim.Unite.ProgramlamaDili.IkonUrl
                },
                unite = new
                {
                    kisim.Unite.Id,
                    kisim.Unite.Baslik,
                    kisim.Unite.Sira
                },
                kisim = new
                {
                    kisim.Id,
                    kisim.Baslik,
                    kisim.Sira
                },
                icerik = icerik == null ? null : new
                {
                    icerik.Baslik,
                    icerik.AciklamaHtml,
                    icerik.OrnekKod,
                    icerik.EkstraJson
                },
                dersler
            });
        }

        // ================================================
        // 2) Programlama dili listesi (Frontend için sade)
        // ================================================
        [HttpGet("diller")]
        public async Task<IActionResult> GetLanguages()
        {
            var list = await _context.ProgramlamaDilleri
                .Where(x => x.AktifMi)
                .Select(x => new
                {
                    x.Id,
                    x.Ad,
                    x.Kod,
                    x.IkonUrl
                })
                .ToListAsync();

            return Ok(list);
        }

        // ================================================
        // 3) Bir dile ait üniter
        // ================================================
        [HttpGet("dil/{dilId}/uniteler")]
        public async Task<IActionResult> GetUnits(int dilId)
        {
            var list = await _context.Uniteler
                .Where(x => x.ProgramlamaDiliId == dilId)
                .OrderBy(x => x.Sira)
                .Select(x => new
                {
                    x.Id,
                    x.Baslik,
                    x.Sira,
                    x.Aciklama
                })
                .ToListAsync();

            return Ok(list);
        }

        // ================================================
        // 4) Üniteye ait kısımlar
        // ================================================
        [HttpGet("unite/{uniteId}/kisimlar")]
        public async Task<IActionResult> GetSections(int uniteId)
        {
            var list = await _context.Kisimlar
                .Where(k => k.UniteId == uniteId)
                .OrderBy(k => k.Sira)
                .Select(k => new
                {
                    k.Id,
                    k.Baslik,
                    k.Sira,
                    k.DersSayisiCache
                })
                .ToListAsync();

            return Ok(list);
        }

        // ================================================
        // 5) Kısıma ait dersler
        // ================================================
        [HttpGet("kisim/{kisimId}/dersler")]
        public async Task<IActionResult> GetLessons(int kisimId)
        {
            var list = await _context.Dersler
                .Where(d => d.KisimId == kisimId)
                .OrderBy(d => d.Sira)
                .Select(d => new
                {
                    d.Id,
                    d.Baslik,
                    d.Sira,
                    d.ZorlukSeviyesi,
                    d.TahminiSure,
                    d.SoruSayisiCache
                })
                .ToListAsync();

            return Ok(list);
        }

        // ================================================
        // 6) Derse ait sorular
        // ================================================
        [HttpGet("ders/{dersId}/sorular")]
        public async Task<IActionResult> GetQuestions(int dersId)
        {
            var questions = await _context.Sorular
                .Where(s => s.DersId == dersId)
                .Select(s => new
                {
                    s.Id,
                    s.SoruTipi,
                    s.SoruMetni,
                    s.KodMetni,
                    s.Seviye,
                    s.DogruCevapId,
                    s.EkVeriJson,
                    Secenekler = s.Secenekler.Select(c => new
                    {
                        c.Id,
                        c.SecenekMetni
                    }),
                    s.KelimeBloklar,
                    s.FonksiyonCozumler,
                    s.CanliPreviews
                })
                .ToListAsync();

            return Ok(questions);
        }
    }
}
