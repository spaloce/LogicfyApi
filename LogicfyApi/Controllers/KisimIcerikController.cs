using LogicfyApi.Data;
using LogicfyApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogicfyApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/kisimicerik")]
    public class KisimIcerikController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KisimIcerikController(AppDbContext context)
        {
            _context = context;
        }

        // -----------------------------------------------------
        // 1) Listeleme
        // -----------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.KisimIcerikler
                .Include(i => i.ProgramlamaDili)
                .Include(i => i.Unite)
                .Include(i => i.Kisim)
                .Select(i => new
                {
                    i.Id,
                    i.Baslik,
                    i.ProgramlamaDiliId,
                    ProgramlamaDili = i.ProgramlamaDili.Ad,
                    i.UniteId,
                    Unite = i.Unite.Baslik,
                    i.KisimId,
                    Kisim = i.Kisim.Baslik
                })
                .ToListAsync();

            return Ok(list);
        }

        // -----------------------------------------------------
        // 2) Tek Kayıt Getirme
        // -----------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.KisimIcerikler
                .Include(i => i.ProgramlamaDili)
                .Include(i => i.Unite)
                .Include(i => i.Kisim)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return NotFound("Kayıt bulunamadı.");

            return Ok(item);
        }

        // -----------------------------------------------------
        // 3) Ekleme
        // -----------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KisimIcerik model)
        {
            var exists = await _context.KisimIcerikler
                .AnyAsync(x => x.KisimId == model.KisimId);

            if (exists)
                return BadRequest("Bu kısım için zaten bir içerik mevcut.");

            _context.KisimIcerikler.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // -----------------------------------------------------
        // 4) Güncelleme
        // -----------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] KisimIcerik model)
        {
            var item = await _context.KisimIcerikler.FindAsync(id);

            if (item == null)
                return NotFound("Kayıt bulunamadı.");

            item.ProgramlamaDiliId = model.ProgramlamaDiliId;
            item.UniteId = model.UniteId;
            item.KisimId = model.KisimId;
            item.Baslik = model.Baslik;
            item.AciklamaHtml = model.AciklamaHtml;
            item.OrnekKod = model.OrnekKod;
            item.EkstraJson = model.EkstraJson;

            await _context.SaveChangesAsync();

            return Ok(item);
        }

        // -----------------------------------------------------
        // 5) Silme
        // -----------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.KisimIcerikler.FindAsync(id);

            if (item == null)
                return NotFound("Kayıt bulunamadı.");

            _context.KisimIcerikler.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Silindi");
        }
    }
}
