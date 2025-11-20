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
    public class KisimController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KisimController(AppDbContext context)
        {
            _context = context;
        }

        // ===== GET - TÜM KISIMLAR =====

        [HttpGet]
        public IActionResult GetAll()
        {
            var kisimlar = _context.Kisimlar
                .Include(x => x.Unite)
                .Include(x => x.Dersler)
                .Select(x => new
                {
                    x.Id,
                    x.UniteId,
                    x.Baslik,
                    x.Sira,
                    x.DersSayisiCache,
                    x.CreatedAt,
                    x.UpdatedAt,
                    Unite = new
                    {
                        x.Unite.Id,
                        x.Unite.Baslik
                    },
                    DersSayisi = x.Dersler.Count
                })
                .OrderBy(x => x.Sira)
                .ToList();

            return Ok(kisimlar);
        }

        // ===== GET - SİNGLE KISIM =====

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var kisim = _context.Kisimlar
                .Include(x => x.Unite)
                .Include(x => x.Dersler)
                .FirstOrDefault(x => x.Id == id);

            if (kisim == null)
                return NotFound(new { message = "Kısım bulunamadı" });

            return Ok(new
            {
                kisim.Id,
                kisim.UniteId,
                kisim.Baslik,
                kisim.Sira,
                kisim.DersSayisiCache,
                kisim.CreatedAt,
                kisim.UpdatedAt,
                Unite = new
                {
                    kisim.Unite.Id,
                    kisim.Unite.Baslik
                },
                Dersler = kisim.Dersler.Select(d => new
                {
                    d.Id,
                    d.Baslik,
                    d.Sira,
                    d.TahminiSure,
                    d.ZorlukSeviyesi,
                    d.SoruSayisiCache
                }).OrderBy(d => d.Sira),
                DersSayisi = kisim.Dersler.Count
            });
        }

        // ===== GET - ÜNİTEYE GÖRE KISIMLAR =====

        [HttpGet("unite/{uniteId}")]
        public IActionResult GetByUniteId(int uniteId)
        {
            var unite = _context.Uniteler.Find(uniteId);
            if (unite == null)
                return NotFound(new { message = "Ünite bulunamadı" });

            var kisimlar = _context.Kisimlar
                .Where(x => x.UniteId == uniteId)
                .Include(x => x.Dersler)
                .Select(x => new
                {
                    x.Id,
                    x.Baslik,
                    x.Sira,
                    x.DersSayisiCache,
                    DersSayisi = x.Dersler.Count
                })
                .OrderBy(x => x.Sira)
                .ToList();

            if (!kisimlar.Any())
                return NotFound(new { message = "Bu üniteye ait kısım bulunamadı" });

            return Ok(new
            {
                UniteId = uniteId,
                UniteAdi = unite.Baslik,
                Kisimlar = kisimlar
            });
        }

        // ===== GET - KISIM İSTATİSTİKLERİ =====

        [HttpGet("{id}/istatistik")]
        public IActionResult GetKisimIstatistik(int id)
        {
            var kisim = _context.Kisimlar
                .Include(x => x.Dersler)
                .ThenInclude(d => d.Sorular)
                .FirstOrDefault(x => x.Id == id);

            if (kisim == null)
                return NotFound(new { message = "Kısım bulunamadı" });

            var toplamDers = kisim.Dersler.Count;
            var toplamSoru = kisim.Dersler.Sum(d => d.Sorular.Count);
            var ortalamaSure = toplamDers > 0
                ? kisim.Dersler.Average(d => d.TahminiSure)
                : 0;
            var ortalamaZorluk = toplamDers > 0
                ? kisim.Dersler.Average(d => d.ZorlukSeviyesi)
                : 0;

            var dersZorlukDagitimi = kisim.Dersler
                .GroupBy(d => d.ZorlukSeviyesi)
                .Select(g => new
                {
                    ZorlukSeviyesi = g.Key,
                    DersSayisi = g.Count()
                })
                .ToList();

            return Ok(new
            {
                KisimId = kisim.Id,
                KisimBaslik = kisim.Baslik,
                ToplamDers = toplamDers,
                ToplamSoru = toplamSoru,
                OrtalamaSure = Math.Round(ortalamaSure, 2),
                OrtalamaZorluk = Math.Round(ortalamaZorluk, 2),
                DersZorlukDagitimi = dersZorlukDagitimi
            });
        }

        // ===== GET - KISIM DERSLERİ =====

        [HttpGet("{id}/dersler")]
        public IActionResult GetKisimDersler(int id)
        {
            var kisim = _context.Kisimlar
                .Include(x => x.Dersler)
                .FirstOrDefault(x => x.Id == id);

            if (kisim == null)
                return NotFound(new { message = "Kısım bulunamadı" });

            var dersler = kisim.Dersler
                .Select(d => new
                {
                    d.Id,
                    d.Baslik,
                    d.Sira,
                    d.TahminiSure,
                    d.SoruSayisiCache,
                    d.ZorlukSeviyesi
                })
                .OrderBy(d => d.Sira)
                .ToList();

            if (!dersler.Any())
                return NotFound(new { message = "Bu kısıma ait ders bulunamadı" });

            return Ok(new
            {
                KisimId = kisim.Id,
                KisimBaslik = kisim.Baslik,
                ToplamDers = dersler.Count,
                Dersler = dersler
            });
        }

        // ===== CREATE - KISIM OLUŞTURMA =====

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateKisimRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Baslik))
                return BadRequest(new { message = "Kısım başlığı gereklidir" });

            var unite = _context.Uniteler.Find(request.UniteId);
            if (unite == null)
                return BadRequest(new { message = "Ünite bulunamadı" });

            // Sıra numarası otomatik atanıyor
            var sonSira = _context.Kisimlar
                .Where(x => x.UniteId == request.UniteId)
                .Max(x => (int?)x.Sira) ?? 0;

            var kisim = new Kisim
            {
                UniteId = request.UniteId,
                Baslik = request.Baslik,
                Sira = sonSira + 1,
                DersSayisiCache = 0
            };

            _context.Kisimlar.Add(kisim);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = kisim.Id }, new
            {
                kisim.Id,
                kisim.Baslik,
                kisim.Sira,
                kisim.UniteId,
                kisim.CreatedAt,
                message = "Kısım başarıyla oluşturuldu"
            });
        }

        // ===== UPDATE - KISIM GÜNCELLEME =====

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateKisimRequest request)
        {
            var kisim = _context.Kisimlar.Find(id);
            if (kisim == null)
                return NotFound(new { message = "Kısım bulunamadı" });

            if (!string.IsNullOrWhiteSpace(request.Baslik))
                kisim.Baslik = request.Baslik;

            if (request.Sira > 0)
                kisim.Sira = request.Sira;

            kisim.UpdatedAt = DateTime.Now;

            _context.Kisimlar.Update(kisim);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                kisim.Id,
                kisim.Baslik,
                kisim.Sira,
                kisim.UpdatedAt,
                message = "Kısım başarıyla güncellendi"
            });
        }

        // ===== DELETE - KISIM SİLME =====

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var kisim = _context.Kisimlar
                .Include(x => x.Dersler)
                .FirstOrDefault(x => x.Id == id);

            if (kisim == null)
                return NotFound(new { message = "Kısım bulunamadı" });

            if (kisim.Dersler.Any())
                return BadRequest(new { message = "Bu kısımın dersleri var. Lütfen önce dersleri silin" });

            _context.Kisimlar.Remove(kisim);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kısım başarıyla silindi" });
        }

        // ===== UPDATE - DERS SAYISI CACHE'İ GÜNCELLE =====

        [HttpPut("{id}/update-ders-cache")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDersCache(int id)
        {
            var kisim = _context.Kisimlar
                .Include(x => x.Dersler)
                .FirstOrDefault(x => x.Id == id);

            if (kisim == null)
                return NotFound(new { message = "Kısım bulunamadı" });

            kisim.DersSayisiCache = kisim.Dersler.Count;
            _context.Kisimlar.Update(kisim);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                KisimId = kisim.Id,
                DersSayisi = kisim.DersSayisiCache,
                message = "Ders sayısı cache'i güncellendi"
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
                var kisim = _context.Kisimlar.Find(req.KisimId);
                if (kisim != null)
                {
                    kisim.Sira = req.YeniSira;
                    _context.Kisimlar.Update(kisim);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Sıralar başarıyla güncellendi" });
        }

        // ===== GET - KISIM ARAMA =====

        [HttpGet("ara/{arama}")]
        public IActionResult Search(string arama)
        {
            if (string.IsNullOrWhiteSpace(arama))
                return BadRequest(new { message = "Arama terimi gereklidir" });

            var sonuclar = _context.Kisimlar
                .Where(x => x.Baslik.ToLower().Contains(arama.ToLower()))
                .Include(x => x.Unite)
                .Select(x => new
                {
                    x.Id,
                    x.Baslik,
                    x.Sira,
                    x.DersSayisiCache,
                    Unite = new
                    {
                        x.Unite.Id,
                        x.Unite.Baslik
                    }
                })
                .ToList();

            if (!sonuclar.Any())
                return NotFound(new { message = "Arama sonucu bulunamadı" });

            return Ok(sonuclar);
        }

        // ===== GET - KISIM DETAYLI BİLGİ (Dersler dahil) =====

        [HttpGet("{id}/detay")]
        public IActionResult GetKisimDetay(int id)
        {
            var kisim = _context.Kisimlar
                .Include(x => x.Unite)
                .Include(x => x.Dersler)
                .ThenInclude(d => d.Sorular)
                .FirstOrDefault(x => x.Id == id);

            if (kisim == null)
                return NotFound(new { message = "Kısım bulunamadı" });

            return Ok(new
            {
                Kisim = new
                {
                    kisim.Id,
                    kisim.Baslik,
                    kisim.Sira,
                    kisim.DersSayisiCache,
                    kisim.CreatedAt,
                    kisim.UpdatedAt,
                    Unite = new { kisim.Unite.Id, kisim.Unite.Baslik }
                },
                DersDetaylari = kisim.Dersler.Select(d => new
                {
                    d.Id,
                    d.Baslik,
                    d.Sira,
                    d.TahminiSure,
                    d.ZorlukSeviyesi,
                    d.SoruSayisiCache,
                    SoruSayisi = d.Sorular.Count,
                    SoruDagitimi = new
                    {
                        Kolay = d.Sorular.Count(s => s.Seviye == 1),
                        Orta = d.Sorular.Count(s => s.Seviye == 2),
                        Zor = d.Sorular.Count(s => s.Seviye == 3)
                    }
                }).OrderBy(d => d.Sira),
                Ozet = new
                {
                    ToplamDers = kisim.Dersler.Count,
                    ToplamSoru = kisim.Dersler.Sum(d => d.Sorular.Count),
                    OrtalamaSure = kisim.Dersler.Any() ? Math.Round(kisim.Dersler.Average(d => d.TahminiSure), 2) : 0,
                    OrtalemaZorluk = kisim.Dersler.Any() ? Math.Round(kisim.Dersler.Average(d => d.ZorlukSeviyesi), 2) : 0
                }
            });
        }

        // ===== GET - TÜM KISIMLAR (Hiyerarşik) =====

        [HttpGet("hiyerarsi/tumun")]
        public IActionResult GetHierarchical()
        {
            var uniteler = _context.Uniteler
                .Include(x => x.Kisimlar)
                .ThenInclude(k => k.Dersler)
                .Select(u => new
                {
                    u.Id,
                    u.Baslik,
                    u.Sira,
                    Kisimlar = u.Kisimlar.Select(k => new
                    {
                        k.Id,
                        k.Baslik,
                        k.Sira,
                        k.DersSayisiCache,
                        Dersler = k.Dersler.Select(d => new
                        {
                            d.Id,
                            d.Baslik,
                            d.Sira,
                            d.TahminiSure,
                            d.ZorlukSeviyesi
                        }).OrderBy(d => d.Sira).ToList()
                    }).OrderBy(k => k.Sira).ToList()
                })
                .OrderBy(u => u.Sira)
                .ToList();

            if (!uniteler.Any())
                return NotFound(new { message = "Ünite bulunamadı" });

            return Ok(uniteler);
        }

        // ===== GET - KOMPLEKS ARAMA (ADVANCE SEARCH) =====

        [HttpGet("advanced-arama")]
        public IActionResult AdvancedSearch([FromQuery] string baslik, [FromQuery] int? uniteId)
        {
            var query = _context.Kisimlar.Include(x => x.Unite);

            if (!string.IsNullOrWhiteSpace(baslik))
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Kisim, Unite>)query.Where(x => x.Baslik.ToLower().Contains(baslik.ToLower()));

            if (uniteId.HasValue)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Kisim, Unite>)query.Where(x => x.UniteId == uniteId.Value);

            var sonuclar = query
                .Select(x => new
                {
                    x.Id,
                    x.Baslik,
                    x.Sira,
                    x.DersSayisiCache,
                    Unite = new { x.Unite.Id, x.Unite.Baslik }
                })
                .OrderBy(x => x.Sira)
                .ToList();

            if (!sonuclar.Any())
                return NotFound(new { message = "Kısım bulunamadı" });

            return Ok(sonuclar);
        }
    }

}
