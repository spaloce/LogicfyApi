using LogicfyApi.Data;
using LogicfyApi.Models;
using LogicfyApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogicfyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DersController(AppDbContext context)
        {
            _context = context;
        }

        // ===== GET - TÜM DERSLER =====

        [HttpGet]
        public IActionResult GetAll()
        {
            var dersler = _context.Dersler
                .Include(x => x.Kisim)
                .Include(x => x.Sorular)
                .Select(x => new
                {
                    x.Id,
                    x.KisimId,
                    x.Baslik,
                    x.Sira,
                    x.TahminiSure,
                    x.SoruSayisiCache,
                    x.ZorlukSeviyesi,
                    x.CreatedAt,
                    x.UpdatedAt,
                    Kisim = new
                    {
                        x.Kisim.Id,
                        x.Kisim.Baslik
                    },
                    SoruSayisi = x.Sorular.Count
                })
                .OrderBy(x => x.Sira)
                .ToList();

            return Ok(dersler);
        }

        // ===== GET - SİNGLE DERS =====

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var ders = _context.Dersler
                .Include(x => x.Kisim)
                .Include(x => x.Sorular)
                .Include(x => x.DersKayitlari)
                .FirstOrDefault(x => x.Id == id);

            if (ders == null)
                return NotFound(new { message = "Ders bulunamadı" });

            return Ok(new
            {
                ders.Id,
                ders.KisimId,
                ders.Baslik,
                ders.Sira,
                ders.TahminiSure,
                ders.SoruSayisiCache,
                ders.ZorlukSeviyesi,
                ders.CreatedAt,
                ders.UpdatedAt,
                Kisim = new
                {
                    ders.Kisim.Id,
                    ders.Kisim.Baslik
                },
                Sorular = ders.Sorular.Select(s => new
                {
                    s.Id,
                    s.SoruTipi,
                    s.SoruMetni,
                    s.Seviye
                }),
                KayitliKullaniciSayisi = ders.DersKayitlari.Count,
                AktifKullaniciSayisi = ders.DersKayitlari.Count(x => x.AktifMi)
            });
        }

        // ===== GET - KISIMa GÖRE DERSLER =====

        [HttpGet("kisim/{kisimId}")]
        public IActionResult GetByKisimId(int kisimId)
        {
            var kisim = _context.Kisimlar.Find(kisimId);
            if (kisim == null)
                return NotFound(new { message = "Kısım bulunamadı" });

            var dersler = _context.Dersler
                .Where(x => x.KisimId == kisimId)
                .Include(x => x.Sorular)
                .Select(x => new
                {
                    x.Id,
                    x.Baslik,
                    x.Sira,
                    x.TahminiSure,
                    x.SoruSayisiCache,
                    x.ZorlukSeviyesi,
                    SoruSayisi = x.Sorular.Count
                })
                .OrderBy(x => x.Sira)
                .ToList();

            if (!dersler.Any())
                return NotFound(new { message = "Bu kısıma ait ders bulunamadı" });

            return Ok(dersler);
        }

        // ===== GET - DERS DETAYLARI VE İSTATİSTİKLER =====

        [HttpGet("{id}/istatistik")]
        public IActionResult GetDersIstatistik(int id)
        {
            var ders = _context.Dersler
                .Include(x => x.Sorular)
                .Include(x => x.DersKayitlari)
                .FirstOrDefault(x => x.Id == id);

            if (ders == null)
                return NotFound(new { message = "Ders bulunamadı" });

            var toplamSoru = ders.Sorular.Count;
            var kolaySoru = ders.Sorular.Count(s => s.Seviye == 1);
            var ortaSoru = ders.Sorular.Count(s => s.Seviye == 2);
            var zorkSoru = ders.Sorular.Count(s => s.Seviye == 3);

            return Ok(new
            {
                DersId = ders.Id,
                DersBaslik = ders.Baslik,
                TahminiSure = ders.TahminiSure,
                ZorlukSeviyesi = ders.ZorlukSeviyesi,
                ToplamSoru = toplamSoru,
                SoruDagitimi = new
                {
                    Kolay = kolaySoru,
                    Orta = ortaSoru,
                    Zor = zorkSoru
                },
                SoruTipDagitimi = new
                {
                    Tip1 = ders.Sorular.Count(s => s.SoruTipi == 1),
                    Tip2 = ders.Sorular.Count(s => s.SoruTipi == 2),
                    Tip3 = ders.Sorular.Count(s => s.SoruTipi == 3),
                    Tip4 = ders.Sorular.Count(s => s.SoruTipi == 4)
                },
                KayitliKullaniciSayisi = ders.DersKayitlari.Count,
                AktifKullaniciSayisi = ders.DersKayitlari.Count(x => x.AktifMi)
            });
        }

        // ===== CREATE - DERS OLUŞTURMA =====

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateDersRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Baslik))
                return BadRequest(new { message = "Ders başlığı gereklidir" });

            var kisim = _context.Kisimlar.Find(request.KisimId);
            if (kisim == null)
                return BadRequest(new { message = "Kısım bulunamadı" });

            // Sıra numarası otomatik atanıyor
            var sonSira = _context.Dersler
                .Where(x => x.KisimId == request.KisimId)
                .Max(x => (int?)x.Sira) ?? 0;

            var ders = new Ders
            {
                KisimId = request.KisimId,
                Baslik = request.Baslik,
                Sira = sonSira + 1,
                TahminiSure = request.TahminiSure,
                ZorlukSeviyesi = request.ZorlukSeviyesi,
                SoruSayisiCache = 0
            };

            _context.Dersler.Add(ders);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = ders.Id }, new
            {
                ders.Id,
                ders.Baslik,
                ders.Sira,
                ders.TahminiSure,
                ders.ZorlukSeviyesi,
                ders.CreatedAt,
                message = "Ders başarıyla oluşturuldu"
            });
        }

        // ===== UPDATE - DERS GÜNCELLEME =====

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDersRequest request)
        {
            var ders = _context.Dersler.Find(id);
            if (ders == null)
                return NotFound(new { message = "Ders bulunamadı" });

            if (!string.IsNullOrWhiteSpace(request.Baslik))
                ders.Baslik = request.Baslik;

            if (request.TahminiSure > 0)
                ders.TahminiSure = request.TahminiSure;

            if (request.ZorlukSeviyesi > 0)
                ders.ZorlukSeviyesi = request.ZorlukSeviyesi;

            if (request.Sira > 0)
                ders.Sira = request.Sira;

            ders.UpdatedAt = DateTime.Now;

            _context.Dersler.Update(ders);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ders.Id,
                ders.Baslik,
                ders.Sira,
                ders.TahminiSure,
                ders.ZorlukSeviyesi,
                ders.UpdatedAt,
                message = "Ders başarıyla güncellendi"
            });
        }

        // ===== DELETE - DERS SİLME =====

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ders = _context.Dersler
                .Include(x => x.Sorular)
                .FirstOrDefault(x => x.Id == id);

            if (ders == null)
                return NotFound(new { message = "Ders bulunamadı" });

            if (ders.Sorular.Any())
                return BadRequest(new { message = "Bu dersin soruları var. Lütfen önce soruları silin" });

            _context.Dersler.Remove(ders);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ders başarıyla silindi" });
        }

        // ===== UPDATE - SORU SAYISI CACHE'İ GÜNCELLE =====

        [HttpPut("{id}/update-soru-cache")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSoruCache(int id)
        {
            var ders = _context.Dersler
                .Include(x => x.Sorular)
                .FirstOrDefault(x => x.Id == id);

            if (ders == null)
                return NotFound(new { message = "Ders bulunamadı" });

            ders.SoruSayisiCache = ders.Sorular.Count;
            _context.Dersler.Update(ders);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                DersId = ders.Id,
                SoruSayisi = ders.SoruSayisiCache,
                message = "Soru sayısı cache'i güncellendi"
            });
        }

        // ===== UPDATE - SİRA DEĞİŞTİRME =====

        [HttpPut("sira-guncelle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSira([FromBody] List<UpdateSiraRequest> requests)
        {
            if (requests == null || !requests.Any())
                return BadRequest(new { message = "Sıra bilgisi gereklidir" });

            foreach (var req in requests)
            {
                var ders = _context.Dersler.Find(req.DersId);
                if (ders != null)
                {
                    ders.Sira = req.YeniSira;
                    _context.Dersler.Update(ders);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Sıralar başarıyla güncellendi" });
        }

        // ===== GET - DERS ARAMA =====

        [HttpGet("ara/{arama}")]
        public IActionResult Search(string arama)
        {
            if (string.IsNullOrWhiteSpace(arama))
                return BadRequest(new { message = "Arama terimi gereklidir" });

            var sonuclar = _context.Dersler
                .Where(x => x.Baslik.ToLower().Contains(arama.ToLower()))
                .Include(x => x.Kisim)
                .Select(x => new
                {
                    x.Id,
                    x.Baslik,
                    x.Sira,
                    x.TahminiSure,
                    x.ZorlukSeviyesi,
                    Kisim = new
                    {
                        x.Kisim.Id,
                        x.Kisim.Baslik
                    }
                })
                .ToList();

            if (!sonuclar.Any())
                return NotFound(new { message = "Arama sonucu bulunamadı" });

            return Ok(sonuclar);
        }

        // ===== GET - ZORLUK SEVİYESİNE GÖRE DERSLER =====

        [HttpGet("zorluk/{zorlukSeviyesi}")]
        public IActionResult GetByZorluk(int zorlukSeviyesi)
        {
            if (zorlukSeviyesi < 1 || zorlukSeviyesi > 5)
                return BadRequest(new { message = "Zorluk seviyesi 1-5 arasında olmalıdır" });

            var dersler = _context.Dersler
                .Where(x => x.ZorlukSeviyesi == zorlukSeviyesi)
                .Include(x => x.Kisim)
                .Include(x => x.Sorular)
                .Select(x => new
                {
                    x.Id,
                    x.Baslik,
                    x.Sira,
                    x.TahminiSure,
                    x.ZorlukSeviyesi,
                    Kisim = new { x.Kisim.Id, x.Kisim.Baslik },
                    SoruSayisi = x.Sorular.Count
                })
                .OrderBy(x => x.Sira)
                .ToList();

            if (!dersler.Any())
                return NotFound(new { message = "Bu zorluk seviyesinde ders bulunamadı" });

            return Ok(dersler);
        }

        // ===== GET - KISA SÜREDE BİTİRİLEBİLECEK DERSLER =====

        [HttpGet("sureli/{maxSure}")]
        public IActionResult GetByMaxDuration(int maxSure)
        {
            if (maxSure <= 0)
                return BadRequest(new { message = "Süre 0'dan büyük olmalıdır" });

            var dersler = _context.Dersler
                .Where(x => x.TahminiSure <= maxSure)
                .Include(x => x.Sorular)
                .Select(x => new
                {
                    x.Id,
                    x.Baslik,
                    x.TahminiSure,
                    x.ZorlukSeviyesi,
                    SoruSayisi = x.Sorular.Count
                })
                .OrderBy(x => x.TahminiSure)
                .ToList();

            if (!dersler.Any())
                return NotFound(new { message = $"{maxSure} dakikada tamamlanabilecek ders bulunamadı" });

            return Ok(dersler);
        }
    }
}
