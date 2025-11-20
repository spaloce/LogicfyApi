using LogicfyApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogicfyApi.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    //[Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminDashboardController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        //  ANA DASHBOARD ENDPOINT
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            // --------------------------
            // 1) Genel Sayısal Veriler
            // --------------------------
            var totalLanguages = await _context.ProgramlamaDilleri.CountAsync();
            var totalUnits = await _context.Uniteler.CountAsync();
            var totalKisim = await _context.Kisimlar.CountAsync();
            var totalLessons = await _context.Dersler.CountAsync();
            var totalQuestions = await _context.Sorular.CountAsync();

            // --------------------------
            // 2) Ders & Ünite Takip Bilgileri
            // --------------------------
            var mostFollowedLesson = await _context.DersTakipler
                .Include(x => x.Ders)
                .OrderByDescending(x => x.TakipEdenKullaniciSayisi)
                .Select(x => new
                {
                    x.DersId,
                    x.TakipEdenKullaniciSayisi,
                    Baslik = x.Ders.Baslik
                })
                .FirstOrDefaultAsync();

            var mostFollowedUnit = await _context.UniteTakipler
                .Include(x => x.Unite)
                .OrderByDescending(x => x.TakipEdenKullaniciSayisi)
                .Select(x => new
                {
                    x.UniteId,
                    x.TakipEdenKullaniciSayisi,
                    Baslik = x.Unite.Baslik
                })
                .FirstOrDefaultAsync();

            // --------------------------
            // 3) Soru Çözüm Performansı
            // --------------------------
            var soruAnalitik = await _context.SoruAnalitikler
                .GroupBy(x => 1)
                .Select(g => new
                {
                    OrtalamaDogruOrani =
                        g.Sum(x => x.DogruSayisi) == 0 ? 0 :
                        (double)g.Sum(x => x.DogruSayisi) /
                        (g.Sum(x => x.DogruSayisi) + g.Sum(x => x.YanlisSayisi)) * 100,

                    OrtalamaCevaplanmaSuresi = g.Average(x => x.OrtalamaSure),

                    ToplamCevaplanma = g.Sum(x => x.CevaplanmaSayisi),
                })
                .FirstOrDefaultAsync();

            // --------------------------
            // 4) DersPerformans Analitik Ortalamaları
            // --------------------------
            var dersPerformans = await _context.DersPerformansAnalitikler
                .GroupBy(x => 1)
                .Select(g => new
                {
                    OrtalamaTamamlama = g.Average(x => x.OrtalamaTamamlamaSuresi),
                    OrtalamaDogru = g.Average(x => x.OrtalamaDogruOrani),
                })
                .FirstOrDefaultAsync();

            // --------------------------
            // 5) Zorluk Seviyesi Dağılımı (Ders)
            // --------------------------
            var difficultyStats = await _context.Dersler
                .GroupBy(x => x.ZorlukSeviyesi)
                .Select(g => new
                {
                    Zorluk = g.Key,
                    DersSayisi = g.Count()
                })
                .OrderBy(x => x.Zorluk)
                .ToListAsync();

            // --------------------------
            // 6) Programlama Dillerine Göre Ünite & Ders Dağılımı
            // --------------------------
            var languageStats = await _context.ProgramlamaDilleri
                .Include(d => d.Uniteler)
                .ThenInclude(u => u.Kisimlar)
                .ThenInclude(k => k.Dersler)
                .Select(d => new
                {
                    d.Id,
                    d.Ad,
                    UniteSayisi = d.Uniteler.Count,
                    KisimSayisi = d.Uniteler.Sum(u => u.Kisimlar.Count),
                    DersSayisi = d.Uniteler.Sum(u => u.Kisimlar.Sum(k => k.Dersler.Count)),
                    SoruSayisi = d.Uniteler.Sum(
                        u => u.Kisimlar.Sum(
                            k => k.Dersler.Sum(
                                ders => ders.Sorular.Count)))
                })
                .ToListAsync();

            // --------------------------
            // 7) Son 7 Gün İçinde Eklenen İçerikler
            // --------------------------
            var oneWeekAgo = DateTime.Now.AddDays(-7);

            var last7days = new
            {
                YeniDiller = await _context.ProgramlamaDilleri.CountAsync(x => x.CreatedAt >= oneWeekAgo),
                YeniUniteler = await _context.Uniteler.CountAsync(x => x.CreatedAt >= oneWeekAgo),
                YeniKisimlar = await _context.Kisimlar.CountAsync(x => x.CreatedAt >= oneWeekAgo),
                YeniDersler = await _context.Dersler.CountAsync(x => x.CreatedAt >= oneWeekAgo),
                YeniSorular = await _context.Sorular.CountAsync(x => x.CreatedAt >= oneWeekAgo),
            };

            return Ok(new
            {
                Genel = new
                {
                    ToplamDil = totalLanguages,
                    ToplamUnite = totalUnits,
                    ToplamKisim = totalKisim,
                    ToplamDers = totalLessons,
                    ToplamSoru = totalQuestions
                },

                Takip = new
                {
                    EnCokTakipEdilenDers = mostFollowedLesson,
                    EnCokTakipEdilenUnite = mostFollowedUnit
                },

                SoruAnaliz = soruAnalitik,
                DersAnaliz = dersPerformans,
                ZorlukDagilim = difficultyStats,
                DilIstatistikleri = languageStats,
                Son7Gun = last7days
            });
        }

        // ============================================================
        //  En zor 10 soru
        // ============================================================
        [HttpGet("sorular/en-zor")]
        public async Task<IActionResult> GetHardestQuestions()
        {
            var hardest = await _context.SoruAnalitikler
                .Include(x => x.Soru)
                .OrderByDescending(x => x.OrtalamaSure * 0.6 +
                                        (1 - ((double)x.DogruSayisi / Math.Max(1, x.DogruSayisi + x.YanlisSayisi))) * 0.4)
                .Take(10)
                .Select(x => new
                {
                    x.SoruId,
                    x.CevaplanmaSayisi,
                    x.DogruSayisi,
                    x.YanlisSayisi,
                    x.OrtalamaSure,
                    SoruMetni = x.Soru.SoruMetni
                })
                .ToListAsync();

            return Ok(hardest);
        }




        // ============================================================
        //  En popüler 10 ders
        // ============================================================
        [HttpGet("dersler/en-populer")]
        public async Task<IActionResult> GetMostFollowedLessons()
        {
            var dersler = await _context.DersTakipler
                .Include(x => x.Ders)
                .OrderByDescending(x => x.TakipEdenKullaniciSayisi)
                .Take(10)
                .Select(x => new
                {
                    x.DersId,
                    x.TakipEdenKullaniciSayisi,
                    Baslik = x.Ders.Baslik,
                })
                .ToListAsync();

            return Ok(dersler);
        }




        // ============================================================
        //  Programlama dili detayı
        // ============================================================
        [HttpGet("dil/{id}")]
        public async Task<IActionResult> GetLanguageDetails(int id)
        {
            var dil = await _context.ProgramlamaDilleri
                .Include(d => d.Uniteler)
                .ThenInclude(u => u.Kisimlar)
                .ThenInclude(k => k.Dersler)
                .ThenInclude(ders => ders.Sorular)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dil == null)
                return NotFound("Dil bulunamadı");

            var detay = new
            {
                Dil = new { dil.Id, dil.Ad, dil.Kod, dil.IkonUrl },

                UniteSayisi = dil.Uniteler.Count,

                KisimSayisi = dil.Uniteler.Sum(u => u.Kisimlar.Count),

                DersSayisi = dil.Uniteler.Sum(u => u.Kisimlar.Sum(k => k.Dersler.Count)),

                SoruSayisi = dil.Uniteler.Sum(u =>
                    u.Kisimlar.Sum(k =>
                        k.Dersler.Sum(ders => ders.Sorular.Count))),

                Uniteler = dil.Uniteler.Select(u => new
                {
                    u.Id,
                    u.Baslik,
                    u.Sira,
                    Kisimlar = u.Kisimlar.Select(k => new
                    {
                        k.Id,
                        k.Baslik,
                        k.Sira,
                        Dersler = k.Dersler.Select(d => new
                        {
                            d.Id,
                            d.Baslik,
                            d.ZorlukSeviyesi,
                            SoruSayisi = d.Sorular.Count
                        })
                    })
                })
            };

            return Ok(detay);
        }




        // ============================================================
        //  Haftalık soru cevaplama grafiği (line chart)
        // ============================================================
        [HttpGet("grafik/haftalik-soru")]
        public async Task<IActionResult> GetWeeklyQuestionActivity()
        {
            var oneWeekAgo = DateTime.Now.AddDays(-6).Date;

            var data = await _context.SoruAnalitikler
                .GroupBy(x => x.UpdatedAt.Value.Date)
                .Where(g => g.Key >= oneWeekAgo)
                .Select(g => new
                {
                    Tarih = g.Key,
                    CevapSayisi = g.Sum(x => x.CevaplanmaSayisi)
                })
                .OrderBy(x => x.Tarih)
                .ToListAsync();

            // Eksik günleri dolduralım (grafikte boşluk olmasın)
            var full = Enumerable.Range(0, 7)
                .Select(i => oneWeekAgo.AddDays(i))
                .GroupJoin(
                    data,
                    tarih => tarih,
                    item => item.Tarih,
                    (tarih, group) => new
                    {
                        Tarih = tarih.ToString("yyyy-MM-dd"),
                        CevapSayisi = group.Select(x => x.CevapSayisi).FirstOrDefault()
                    }
                );

            return Ok(full);
        }

    }
}
