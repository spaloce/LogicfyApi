using LogicfyApi.Data;
using LogicfyApi.Models;
using LogicfyApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogicfyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UniteController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        //  GET: TÜM ÜNİTELER
        // ============================================================

        [HttpGet]
        public IActionResult GetAll()
        {
            var uniteler = _context.Uniteler
                .Include(u => u.Kisimlar)
                .Select(u => new
                {
                    u.Id,
                    u.Baslik,
                    u.Sira,
                    u.Aciklama,
                    u.ProgramlamaDiliId,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    KisimSayisi = u.Kisimlar.Count
                })
                .OrderBy(u => u.Sira)
                .ToList();

            return Ok(uniteler);
        }

        // ============================================================
        //  GET: TEK ÜNİTE + KISIMLAR
        // ============================================================

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var unite = _context.Uniteler
                .Include(u => u.Kisimlar)
                .ThenInclude(k => k.Dersler)
                .FirstOrDefault(u => u.Id == id);

            if (unite == null)
                return NotFound(new { message = "Ünite bulunamadı" });

            return Ok(new
            {
                unite.Id,
                unite.Baslik,
                unite.Sira,
                unite.Aciklama,
                unite.ProgramlamaDiliId,
                unite.CreatedAt,
                unite.UpdatedAt,
                Kisimlar = unite.Kisimlar
                    .Select(k => new
                    {
                        k.Id,
                        k.Baslik,
                        k.Sira,
                        k.DersSayisiCache,
                        DersSayisi = k.Dersler.Count
                    })
                    .OrderBy(k => k.Sira),
                Ozet = new
                {
                    KisimSayisi = unite.Kisimlar.Count,
                    ToplamDers = unite.Kisimlar.Sum(k => k.Dersler.Count)
                }
            });
        }

        // ============================================================
        //  GET: ÜNİTE DETAY (KISIM + DERS + SORULAR)
        // ============================================================

        [HttpGet("{id}/detay")]
        public IActionResult GetDetail(int id)
        {
            var unite = _context.Uniteler
                .Include(u => u.Kisimlar)
                .ThenInclude(k => k.Dersler)
                .ThenInclude(d => d.Sorular)
                .FirstOrDefault(u => u.Id == id);

            if (unite == null)
                return NotFound(new { message = "Ünite bulunamadı" });

            return Ok(new
            {
                Unite = new
                {
                    unite.Id,
                    unite.Baslik,
                    unite.Sira,
                    unite.Aciklama,
                    unite.ProgramlamaDiliId
                },
                Kisimlar = unite.Kisimlar.Select(k => new
                {
                    k.Id,
                    k.Baslik,
                    k.Sira,
                    Dersler = k.Dersler.Select(d => new
                    {
                        d.Id,
                        d.Baslik,
                        d.Sira,
                        SoruSayisi = d.Sorular.Count
                    }).OrderBy(d => d.Sira)
                })
                .OrderBy(k => k.Sira),
                Ozet = new
                {
                    KisimSayisi = unite.Kisimlar.Count,
                    ToplamDers = unite.Kisimlar.Sum(k => k.Dersler.Count),
                    ToplamSoru = unite.Kisimlar.Sum(k => k.Dersler.Sum(d => d.Sorular.Count))
                }
            });
        }

        // ============================================================
        //  GET: PROGRAMLAMA DİLİNE AİT ÜNİTELER
        // ============================================================

        [HttpGet("dil/{dilId}")]
        public IActionResult GetByLanguage(int dilId)
        {
            var uniteler = _context.Uniteler
                .Where(u => u.ProgramlamaDiliId == dilId)
                .Include(u => u.Kisimlar)
                .OrderBy(u => u.Sira)
                .Select(u => new
                {
                    u.Id,
                    u.Baslik,
                    u.Sira,
                    u.Aciklama,
                    KisimSayisi = u.Kisimlar.Count
                })
                .ToList();

            return Ok(uniteler);
        }

        // ============================================================
        //  SEARCH
        // ============================================================

        [HttpGet("ara/{metin}")]
        public IActionResult Search(string metin)
        {
            var uniteler = _context.Uniteler
                .Where(u => u.Baslik.ToLower().Contains(metin.ToLower()) ||
                            u.Aciklama.ToLower().Contains(metin.ToLower()))
                .Include(u => u.Kisimlar)
                .Select(u => new
                {
                    u.Id,
                    u.Baslik,
                    u.Sira,
                    u.Aciklama,
                    KisimSayisi = u.Kisimlar.Count
                })
                .ToList();

            return Ok(uniteler);
        }

        // ============================================================
        //  CREATE
        // ============================================================

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateUniteRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Baslik))
                return BadRequest(new { message = "Başlık gereklidir" });

            var unite = new Unite
            {
                Baslik = request.Baslik,
                Sira = request.Sira,
                Aciklama = request.Aciklama,
                ProgramlamaDiliId = request.ProgramlamaDiliId
            };

            _context.Uniteler.Add(unite);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = unite.Id }, unite);
        }

        // ============================================================
        //  UPDATE
        // ============================================================

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUniteRequest request)
        {
            var unite = _context.Uniteler.Find(id);
            if (unite == null)
                return NotFound(new { message = "Ünite bulunamadı" });

            if (!string.IsNullOrWhiteSpace(request.Baslik))
                unite.Baslik = request.Baslik;

            if (!string.IsNullOrWhiteSpace(request.Aciklama))
                unite.Aciklama = request.Aciklama;

            if (request.Sira.HasValue)
                unite.Sira = request.Sira.Value;

            unite.UpdatedAt = DateTime.Now;

            _context.Uniteler.Update(unite);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                unite.Id,
                unite.Baslik,
                unite.Sira,
                unite.Aciklama,
                unite.UpdatedAt,
                message = "Ünite başarıyla güncellendi"
            });
        }

        // ============================================================
        //  DELETE
        // ============================================================

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var unite = _context.Uniteler
                .Include(u => u.Kisimlar)
                .FirstOrDefault(u => u.Id == id);

            if (unite == null)
                return NotFound(new { message = "Ünite bulunamadı" });

            if (unite.Kisimlar.Any())
                return BadRequest(new { message = "Bu üniteye ait kısımlar olduğu için silemezsiniz. Önce kısımları silin." });

            _context.Uniteler.Remove(unite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ünite başarıyla silindi" });
        }

        // ============================================================
        //  TÜM ÜNİTELER HİYERARŞİK
        // ============================================================

        [HttpGet("hiyerarsi/tumun")]
        public IActionResult GetHierarchical()
        {
            var uniteler = _context.Uniteler
                .Include(u => u.Kisimlar)
                .ThenInclude(k => k.Dersler)
                .Select(u => new
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
                    }).OrderBy(k => k.Sira)
                })
                .OrderBy(u => u.Sira)
                .ToList();

            return Ok(uniteler);
        }
    }
}
