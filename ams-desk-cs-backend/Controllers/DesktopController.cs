using ams_desk_cs_backend.Dtos;
using ams_desk_cs_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesktopController : ControllerBase
    {
        private readonly BikesDbContext _context;

        public DesktopController(BikesDbContext context)
        {
            _context = context;
        }

        [HttpPut("assemble/{id}")]
        public async Task<IActionResult> Assemble(int id)
        {
            if(!BikeExists(id))
            {
                return NotFound();
            }
            var bike = await _context.Bikes.Where(bi => bi.BikeId == id).ToListAsync();
            if (bike.Any(bi => bi.StatusId != 1))
            {
                return BadRequest();
            }
            bike.ForEach(bi => bi.StatusId = 2);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("sell/{id}")]
        public async Task<IActionResult> Sell(int id, int salePrice)
        {
            if (!BikeExists(id))
            {
                return NotFound();
            }
            var bike = await _context.Bikes.Where(bi => bi.BikeId == id).ToListAsync();
            if (bike.Any(bi => bi.StatusId == 3))
            {
                return BadRequest();
            }
            bike.ForEach(bi => bi.SalePrice = salePrice);
            bike.ForEach(bi => bi.StatusId = 3);
            bike.ForEach(bi => bi.SaleDate = DateOnly.FromDateTime(DateTime.Today));
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("move/{id}")]
        public async Task<IActionResult> Assemble(int id, short placeId)
        {
            if (!BikeExists(id))
            {
                return NotFound();
            }
            var bike = await _context.Bikes.Where(bi => bi.BikeId == id).ToListAsync();
            if (bike.Any(bi => bi.PlaceId == placeId))
            {
                return BadRequest();
            }
            bike.ForEach(bi => bi.PlaceId = placeId);
            _context.SaveChanges();
            return NoContent();
        }



        [HttpPost("add_bike")]
        public async Task<IActionResult> AddBike(AddBikeDto bike)
        {
            var postBike = new Bike
            {
                PlaceId = bike.PlaceId,
                StatusId = bike.StatusId,
                ModelId = bike.ModelId,
                InsertionDate = DateOnly.FromDateTime(DateTime.Today)
            };
            _context.Add(postBike);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                actionName: nameof(BikesController.GetBike),
                controllerName: "Bikes",
                routeValues: new { id = postBike.BikeId },
                value: postBike
            );
        }

        private bool BikeExists(int id)
        {
            return _context.Bikes.Any(e => e.BikeId == id);
        }
    }
}
