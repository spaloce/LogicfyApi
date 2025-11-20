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
    public class ProgramlamaDiliController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProgramlamaDiliController(AppDbContext context)
        {
            _context = context;
        }

        // ===== GET - TÜM PROGRAMLAMA DİLLERİ =====

        [HttpGet]
        public IActionResult GetAll()
        {
            var diller = _context.ProgramlamaDilleri
                .Include(x => x.Uniteler)
                .Select(x => new
                {
                    x.Id,
                    x.Ad,
                    x.Kod,
                    x.IkonUrl,
                    x.AktifMi,
                    x.CreatedAt,
                    x.UpdatedAt,
                    UniteSayisi = x.Uniteler.Count,
                    AktifUniteSayisi = x.Uniteler.Count
                })
                .ToList();

            return Ok(diller);
        }

        // ===== GET - AKTİF PROGRAMLAMA DİLLERİ =====

        [HttpGet("aktif")]
        public IActionResult GetActive()
        {
            var diller = _context.ProgramlamaDilleri
                .Where(x => x.AktifMi)
                .Include(x => x.Uniteler)
                .Select(x => new
                {
                    x.Id,
                    x.Ad,
                    x.Kod,
                    x.IkonUrl,
                    UniteSayisi = x.Uniteler.Count
                })
                .ToList();

            if (!diller.Any())
                return NotFound(new { message = "Aktif programlama dili bulunamadı" });

            return Ok(diller);
        }

        // ===== GET - SİNGLE DİL =====

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var dil = _context.ProgramlamaDilleri
                .Include(x => x.Uniteler)
                .ThenInclude(u => u.Kisimlar)
                .FirstOrDefault(x => x.Id == id);

            if (dil == null)
                return NotFound(new { message = "Programlama dili bulunamadı" });

            return Ok(new
            {
                dil.Id,
                dil.Ad,
                dil.Kod,
                dil.IkonUrl,
                dil.AktifMi,
                dil.CreatedAt,
                dil.UpdatedAt,
                Uniteler = dil.Uniteler.Select(u => new
                {
                    u.Id,
                    u.Baslik,
                    u.Sira,
                    u.Aciklama,
                    KisimSayisi = u.Kisimlar.Count
                }).OrderBy(u => u.Sira),
                UniteSayisi = dil.Uniteler.Count,
                ToplamKisim = dil.Uniteler.Sum(u => u.Kisimlar.Count)
            });
        }

        // ===== GET - DİL DETAYLARI (Uniteler + Kisimlar) =====

        [HttpGet("{id}/detay")]
        public IActionResult GetDilDetay(int id)
        {
            var dil = _context.ProgramlamaDilleri
                .Include(x => x.Uniteler)
                .ThenInclude(u => u.Kisimlar)
                .ThenInclude(k => k.Dersler)
                .FirstOrDefault(x => x.Id == id);

            if (dil == null)
                return NotFound(new { message = "Programlama dili bulunamadı" });

            return Ok(new
            {
                Dil = new
                {
                    dil.Id,
                    dil.Ad,
                    dil.Kod,
                    dil.IkonUrl,
                    dil.AktifMi,
                    dil.CreatedAt
                },
                Uniteler = dil.Uniteler.Select(u => new
                {
                    u.Id,
                    u.Baslik,
                    u.Sira,
                    u.Aciklama,
                    Kisimlar = u.Kisimlar.Select(k => new
                    {
                        k.Id,
                        k.Baslik,
                        k.Sira,
                        k.DersSayisiCache,
                        DersSayisi = k.Dersler.Count
                    }).OrderBy(k => k.Sira),
                    KisimSayisi = u.Kisimlar.Count,
                    ToplamDers = u.Kisimlar.Sum(k => k.Dersler.Count)
                }).OrderBy(u => u.Sira),
                Ozet = new
                {
                    UniteSayisi = dil.Uniteler.Count,
                    ToplamKisim = dil.Uniteler.Sum(u => u.Kisimlar.Count),
                    ToplamDers = dil.Uniteler.Sum(u => u.Kisimlar.Sum(k => k.Dersler.Count))
                }
            });
        }

        // ===== GET - DİL ISTATISTIKLERI =====

        [HttpGet("{id}/istatistik")]
        public IActionResult GetDilIstatistik(int id)
        {
            var dil = _context.ProgramlamaDilleri
                .Include(x => x.Uniteler)
                .ThenInclude(u => u.Kisimlar)
                .ThenInclude(k => k.Dersler)
                .ThenInclude(d => d.Sorular)
                .FirstOrDefault(x => x.Id == id);

            if (dil == null)
                return NotFound(new { message = "Programlama dili bulunamadı" });

            var toplamSoru = dil.Uniteler.Sum(u => u.Kisimlar.Sum(k => k.Dersler.Sum(d => d.Sorular.Count)));
            var toplamDers = dil.Uniteler.Sum(u => u.Kisimlar.Sum(k => k.Dersler.Count));
            var ortalamaSure = toplamDers > 0
                ? dil.Uniteler.Sum(u => u.Kisimlar.Sum(k => k.Dersler.Sum(d => d.TahminiSure))) / (double)toplamDers
                : 0;

            return Ok(new
            {
                DilId = dil.Id,
                DilAdi = dil.Ad,
                DilKodu = dil.Kod,
                AktifMi = dil.AktifMi,
                UniteSayisi = dil.Uniteler.Count,
                ToplamKisim = dil.Uniteler.Sum(u => u.Kisimlar.Count),
                ToplamDers = toplamDers,
                ToplamSoru = toplamSoru,
                OrtalamaSure = Math.Round(ortalamaSure, 2)
            });
        }

        // ===== GET - KODU GÖRE DİL =====

        [HttpGet("kod/{kod}")]
        public IActionResult GetByKod(string kod)
        {
            if (string.IsNullOrWhiteSpace(kod))
                return BadRequest(new { message = "Dil kodu gereklidir" });

            var dil = _context.ProgramlamaDilleri
                .Include(x => x.Uniteler)
                .FirstOrDefault(x => x.Kod.ToLower() == kod.ToLower());

            if (dil == null)
                return NotFound(new { message = "Programlama dili bulunamadı" });

            return Ok(new
            {
                dil.Id,
                dil.Ad,
                dil.Kod,
                dil.IkonUrl,
                dil.AktifMi,
                UniteSayisi = dil.Uniteler.Count
            });
        }

        // ===== GET - DİL ARAMA =====

        [HttpGet("ara/{arama}")]
        public IActionResult Search(string arama)
        {
            if (string.IsNullOrWhiteSpace(arama))
                return BadRequest(new { message = "Arama terimi gereklidir" });

            var sonuclar = _context.ProgramlamaDilleri
                .Where(x => x.Ad.ToLower().Contains(arama.ToLower()) ||
                            x.Kod.ToLower().Contains(arama.ToLower()))
                .Include(x => x.Uniteler)
                .Select(x => new
                {
                    x.Id,
                    x.Ad,
                    x.Kod,
                    x.IkonUrl,
                    x.AktifMi,
                    UniteSayisi = x.Uniteler.Count
                })
                .ToList();

            if (!sonuclar.Any())
                return NotFound(new { message = "Arama sonucu bulunamadı" });

            return Ok(sonuclar);
        }

        // ===== CREATE - DİL OLUŞTURMA =====

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProgramlamaDiliRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Ad) || string.IsNullOrWhiteSpace(request.Kod))
                return BadRequest(new { message = "Dil adı ve kodu gereklidir" });

            var existing = _context.ProgramlamaDilleri.FirstOrDefault(x => x.Kod.ToLower() == request.Kod.ToLower());
            if (existing != null)
                return BadRequest(new { message = "Bu koda sahip bir dil zaten var" });

            var dil = new ProgramlamaDili
            {
                Ad = request.Ad,
                Kod = request.Kod.ToLower(),
                IkonUrl = request.IkonUrl,
                AktifMi = request.AktifMi ?? true
            };

            _context.ProgramlamaDilleri.Add(dil);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = dil.Id }, new
            {
                dil.Id,
                dil.Ad,
                dil.Kod,
                dil.IkonUrl,
                dil.AktifMi,
                dil.CreatedAt,
                message = "Programlama dili başarıyla oluşturuldu"
            });
        }

        // ===== UPDATE - DİL GÜNCELLEME =====

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProgramlamaDiliRequest request)
        {
            var dil = _context.ProgramlamaDilleri.Find(id);
            if (dil == null)
                return NotFound(new { message = "Programlama dili bulunamadı" });

            if (!string.IsNullOrWhiteSpace(request.Ad))
                dil.Ad = request.Ad;

            if (!string.IsNullOrWhiteSpace(request.Kod))
            {
                var existing = _context.ProgramlamaDilleri
                    .FirstOrDefault(x => x.Kod.ToLower() == request.Kod.ToLower() && x.Id != id);
                if (existing != null)
                    return BadRequest(new { message = "Bu koda sahip başka bir dil zaten var" });

                dil.Kod = request.Kod.ToLower();
            }

            if (!string.IsNullOrWhiteSpace(request.IkonUrl))
                dil.IkonUrl = request.IkonUrl;

            if (request.AktifMi.HasValue)
                dil.AktifMi = request.AktifMi.Value;

            dil.UpdatedAt = DateTime.Now;

            _context.ProgramlamaDilleri.Update(dil);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                dil.Id,
                dil.Ad,
                dil.Kod,
                dil.IkonUrl,
                dil.AktifMi,
                dil.UpdatedAt,
                message = "Programlama dili başarıyla güncellendi"
            });
        }

        // ===== UPDATE - DURUM DEĞIŞTIRME (Aktif/Pasif) =====

        [HttpPut("{id}/durum/{aktif}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDurum(int id, bool aktif)
        {
            var dil = _context.ProgramlamaDilleri.Find(id);
            if (dil == null)
                return NotFound(new { message = "Programlama dili bulunamadı" });

            dil.AktifMi = aktif;
            dil.UpdatedAt = DateTime.Now;

            _context.ProgramlamaDilleri.Update(dil);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                dil.Id,
                dil.Ad,
                dil.AktifMi,
                message = $"Dil {(aktif ? "aktif" : "pasif")} duruma getirildi"
            });
        }

        // ===== DELETE - DİL SİLME =====

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var dil = _context.ProgramlamaDilleri
                .Include(x => x.Uniteler)
                .FirstOrDefault(x => x.Id == id);

            if (dil == null)
                return NotFound(new { message = "Programlama dili bulunamadı" });

            if (dil.Uniteler.Any())
                return BadRequest(new { message = "Bu dile ait ünitelerin olduğu için silemezsiniz. Lütfen önce üniteleri silin" });

            _context.ProgramlamaDilleri.Remove(dil);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Programlama dili başarıyla silindi" });
        }

        // ===== GET - DIL ÜNİTELERİ =====

        [HttpGet("{id}/uniteler")]
        public IActionResult GetDilUniteler(int id)
        {
            var dil = _context.ProgramlamaDilleri
                .Include(x => x.Uniteler)
                .ThenInclude(u => u.Kisimlar)
                .FirstOrDefault(x => x.Id == id);

            if (dil == null)
                return NotFound(new { message = "Programlama dili bulunamadı" });

            var uniteler = dil.Uniteler
                .Select(u => new
                {
                    u.Id,
                    u.Baslik,
                    u.Sira,
                    u.Aciklama,
                    KisimSayisi = u.Kisimlar.Count
                })
                .OrderBy(u => u.Sira)
                .ToList();

            if (!uniteler.Any())
                return NotFound(new { message = "Bu dile ait ünite bulunamadı" });

            return Ok(new
            {
                DilId = dil.Id,
                DilAdi = dil.Ad,
                ToplamUnite = uniteler.Count,
                Uniteler = uniteler
            });
        }

        // ===== GET - TÜM DİLLER HİYERARŞİK =====

        [HttpGet("hiyerarsi/tumun")]
        public IActionResult GetHierarchical()
        {
            var diller = _context.ProgramlamaDilleri
                .Include(x => x.Uniteler)
                .ThenInclude(u => u.Kisimlar)
                .Select(d => new
                {
                    d.Id,
                    d.Ad,
                    d.Kod,
                    d.IkonUrl,
                    d.AktifMi,
                    Uniteler = d.Uniteler.Select(u => new
                    {
                        u.Id,
                        u.Baslik,
                        u.Sira,
                        u.Aciklama,
                        Kisimlar = u.Kisimlar.Select(k => new
                        {
                            k.Id,
                            k.Baslik,
                            k.Sira,
                            k.DersSayisiCache
                        }).OrderBy(k => k.Sira).ToList()
                    }).OrderBy(u => u.Sira).ToList()
                })
                .ToList();

            if (!diller.Any())
                return NotFound(new { message = "Programlama dili bulunamadı" });

            return Ok(diller);
        }

        // ===== POST - DİLLER TOPLU İMPORT =====

        [HttpPost("toplu-ekle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkCreate([FromBody] List<CreateProgramlamaDiliRequest> requests)
        {
            if (requests == null || !requests.Any())
                return BadRequest(new { message = "En az bir dil gereklidir" });

            var eklinenler = new List<object>();
            var hatalar = new List<string>();

            foreach (var request in requests)
            {
                if (string.IsNullOrWhiteSpace(request.Ad) || string.IsNullOrWhiteSpace(request.Kod))
                {
                    hatalar.Add($"Eksik bilgi: Ad={request.Ad}, Kod={request.Kod}");
                    continue;
                }

                var existing = _context.ProgramlamaDilleri
                    .FirstOrDefault(x => x.Kod.ToLower() == request.Kod.ToLower());

                if (existing != null)
                {
                    hatalar.Add($"{request.Ad} ({request.Kod}) zaten var");
                    continue;
                }

                var dil = new ProgramlamaDili
                {
                    Ad = request.Ad,
                    Kod = request.Kod.ToLower(),
                    IkonUrl = request.IkonUrl,
                    AktifMi = request.AktifMi ?? true
                };

                _context.ProgramlamaDilleri.Add(dil);
                eklinenler.Add(new { dil.Id, dil.Ad, dil.Kod });
            }

            if (eklinenler.Any())
                await _context.SaveChangesAsync();

            return Ok(new
            {
                ToplamIstek = requests.Count,
                BasarilanSayisi = eklinenler.Count,
                HataSayisi = hatalar.Count,
                Eklenenler = eklinenler,
                Hatalar = hatalar,
                message = $"{eklinenler.Count} dil başarıyla eklendi"
            });
        }
    }

}
