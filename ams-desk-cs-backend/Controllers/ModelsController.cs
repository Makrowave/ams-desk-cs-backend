using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ams_desk_cs_backend.Models;
using Microsoft.EntityFrameworkCore.Internal;
using ams_desk_cs_backend.Dtos;
using System.Numerics;
using System.Text.RegularExpressions;

namespace ams_desk_cs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private readonly BikesDbContext _context;

        public ModelsController(BikesDbContext context)
        {
            _context = context;
        }

        // GET: api/Models
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Model>>> GetModels()
        {
            return await _context.Models.ToListAsync();
        }

        // GET: api/Models
        [HttpGet("notSold")]
        public async Task<ActionResult<IEnumerable<BikeRecordDto>>> GetModelsJoinBikes(bool avaible, bool ready, bool electric, int? manufacturer_id, int? wheel_size, int? frame_size, string? name)
        {
            var bikes = _context.Models
                .GroupJoin(
                    _context.Bikes.Where(bi => bi.StatusId != 3),
                    mo => mo.ModelId,
                    bi => bi.ModelId,
                    (mo, bi) => new { mo, bi }
                )
                .SelectMany(
                    r => r.bi.DefaultIfEmpty(),
                    (r, bi) => new { r.mo, bi }
                )
                .GroupBy(
                    r => new
                    {
                        r.mo.ModelId,
                        r.mo.ProductCode,
                        r.mo.EanCode,
                        r.mo.ModelName,
                        r.mo.FrameSize,
                        r.mo.WheelSize,
                        r.mo.ManufacturerId,
                        r.mo.Price,
                        r.mo.IsWoman,
                        r.mo.IsElectric
                    }
                );
            if(avaible)
            {
                bikes = bikes.Where(
                    g => g.Count(r => r.bi != null) > 0
                );
            }
            if (ready)
            {
                bikes = bikes.Where(
                    g => g.Count(r => r.bi != null && r.bi.StatusId == 2) > 0
                );
            }
            if (electric)
            {
                bikes = bikes.Where(
                    g => g.Key.IsElectric == true
                );
            }
            if (manufacturer_id != null)
            {
                bikes = bikes.Where(
                    g => g.Key.ManufacturerId == manufacturer_id
                );
            }
            if (wheel_size != null)
            {
                bikes = bikes.Where(
                    g => g.Key.WheelSize == wheel_size
                );
            }
            if (frame_size != null)
            {
                bikes = bikes.Where(
                    g => g.Key.FrameSize == frame_size
                );
            }
            if (name != null)
            {
                bikes = bikes.Where(
                    g => g.Key.ModelName.ToLower().Contains(name)
                );
            }

            var result = await bikes.OrderBy(g => g.Key.ModelId)
                .Select(g => new BikeRecordDto
                {
                    ModelId = g.Key.ModelId,
                    ProductCode = g.Key.ProductCode,
                    EanCode = g.Key.EanCode,
                    ModelName = g.Key.ModelName,
                    FrameSize = g.Key.FrameSize,
                    WheelSize = g.Key.WheelSize,
                    ManufacturerId = g.Key.ManufacturerId,
                    Price = g.Key.Price,
                    IsWoman = g.Key.IsWoman,
                    IsElectric = g.Key.IsElectric,
                    BikeCount = g.Count(r => r.bi != null),
                    PlaceBikeCount = g.Where(r => r.bi != null && r.bi.PlaceId != null)
                                        .GroupBy(r => new { r.bi.PlaceId })
                                        .Select(d => new PlaceBikeCountDto
                                        {
                                            PlaceId = d.Key.PlaceId,
                                            Count = d.Count()
                                        })

                }).ToListAsync();

            return result;
            //return await _context.Models.ToListAsync();
        }

        // GET: api/Models/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Model>> GetModel(int id)
        {
            var model = await _context.Models.FindAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return model;
        }

        // PUT: api/Models/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModel(int id, Model model)
        {
            if (id != model.ModelId)
            {
                return BadRequest();
            }

            _context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Models
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Model>> PostModel(Model model)
        {
            _context.Models.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModel", new { id = model.ModelId }, model);
        }

        // DELETE: api/Models/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModel(int id)
        {
            var model = await _context.Models.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            _context.Models.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ModelExists(int id)
        {
            return _context.Models.Any(e => e.ModelId == id);
        }
    }
}
