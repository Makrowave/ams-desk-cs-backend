using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ams_desk_cs_backend.BikeService.Dtos;
using ams_desk_cs_backend.BikeService.Models;

namespace ams_desk_cs_backend.BikeService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikesController : ControllerBase
    {
        private readonly BikesDbContext _context;

        public BikesController(BikesDbContext context)
        {
            _context = context;
        }

        // GET: api/Bikes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bike>>> GetBikes()
        {
            return await _context.Bikes.ToListAsync();
        }

        // GET: api/Bikes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bike>> GetBike(int id)
        {
            var bike = await _context.Bikes.FindAsync(id);

            if (bike == null)
            {
                return NotFound();
            }

            return bike;
        }

        // GET: api/Bikes
        [HttpGet("bikesByModelId/{modelId}")]
        public async Task<ActionResult<IEnumerable<BikeSubRecordDto>>> GetBikesByModel(int modelId, int placeId)
        {
            var maxPlaceId = placeId;
            if (placeId == 0)
            {
                maxPlaceId = 999;
            }
            //Find more elegant solution to placeId
            var bikes = _context.Bikes.Where(bi => bi.ModelId == modelId && bi.StatusId != 3 && bi.PlaceId >= placeId && bi.PlaceId <= maxPlaceId)
                .GroupJoin(
                    _context.Places,
                    bi => bi.PlaceId,
                    pl => pl.PlaceId,
                    (bi, pl) => new { bi, pl }
                )
                .SelectMany(
                    g => g.pl.DefaultIfEmpty(),
                    (g1, pl) => new { g1.bi, pl }
                )
                .GroupJoin(
                    _context.Statuses,
                    g => g.bi.StatusId,
                    st => st.StatusId,
                    (g, st) => new { g.bi, g.pl, st }
                )
                .SelectMany(
                    g => g.st.DefaultIfEmpty(),
                    (g, st) => new BikeSubRecordDto
                    {
                        Id = g.bi.BikeId,
                        Place = g.pl.PlaceName,
                        Status = st.StatusName,
                    }
                ).ToListAsync();

            return await bikes;
        }

        // PUT: api/Bikes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBike(int id, Bike bike)
        {
            if (id != bike.BikeId)
            {
                return BadRequest();
            }

            _context.Entry(bike).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BikeExists(id))
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

        // POST: api/Bikes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bike>> PostBike(Bike bike)
        {
            _context.Bikes.Add(bike);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBike", new { id = bike.BikeId }, bike);
        }

        // DELETE: api/Bikes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBike(int id)
        {
            var bike = await _context.Bikes.FindAsync(id);
            if (bike == null)
            {
                return NotFound();
            }

            _context.Bikes.Remove(bike);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BikeExists(int id)
        {
            return _context.Bikes.Any(e => e.BikeId == id);
        }
    }
}
