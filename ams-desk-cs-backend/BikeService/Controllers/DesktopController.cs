using ams_desk_cs_backend.BikeService.Dtos;
using ams_desk_cs_backend.BikeService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeService.Controllers
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

        [HttpPut("Assemble/{id}")]
        public async Task<IActionResult> Assemble(int id)
        {
            if (!BikeExists(id))
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

        [HttpPut("Sell/{id}")]
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

        [HttpPut("Move/{id}")]
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



        [HttpPost("AddBike")]
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

        // GET: api/Models
        [HttpGet("GetBikes")]
        public async Task<ActionResult<IEnumerable<BikeRecordDto>>> GetModelsJoinBikes(
            bool avaible,
            bool ready,
            bool electric,
            int? manufacturerId,
            int? wheelSize,
            int? frameSize,
            string? name,
            bool? isWoman,
            bool isKids
            )
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
            if (avaible)
            {
                bikes = bikes.Where(
                    g => g.Count(r => r.bi != null) > 0
                );
            }
            if (isWoman != null)
            {
                bikes = bikes.Where(
                     g => g.Key.IsWoman == isWoman
                );
            }
            if (isKids)
            {
                bikes = bikes.Where(
                    g => g.Key.WheelSize <= 24
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
            if (manufacturerId != null)
            {
                bikes = bikes.Where(
                    g => g.Key.ManufacturerId == manufacturerId
                );
            }
            if (wheelSize != null)
            {
                bikes = bikes.Where(
                    g => g.Key.WheelSize == wheelSize
                );
            }
            if (frameSize != null)
            {
                bikes = bikes.Where(
                    g => g.Key.FrameSize == frameSize
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
        }


        [HttpGet("GetBikesByPlace/{placeId}")]
        public async Task<ActionResult<IEnumerable<BikeRecordDto>>> GetModelsJoinBikesById(
            bool avaible,
            bool ready,
            bool electric,
            int? manufacturerId,
            int? wheelSize,
            int? frameSize,
            string? name,
            bool? isWoman,
            int? placeId,
            bool isKids
            )
        {
            var bikes = _context.Models
                .GroupJoin(
                    _context.Bikes.Where(bi => bi.StatusId != 3 && bi.PlaceId == placeId),
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
            if (avaible)
            {
                bikes = bikes.Where(
                    g => g.Count(r => r.bi != null) > 0
                );
            }
            if (isWoman != null)
            {
                bikes = bikes.Where(
                     g => g.Key.IsWoman == isWoman
                );
            }
            if (ready)
            {
                bikes = bikes.Where(
                    g => g.Count(r => r.bi != null && r.bi.StatusId == 2) > 0
                );
            }
            if (isKids)
            {
                bikes = bikes.Where(
                    g => g.Key.WheelSize <= 24
                );
            }
            if (electric)
            {
                bikes = bikes.Where(
                    g => g.Key.IsElectric == true
                );
            }
            if (manufacturerId != null)
            {
                bikes = bikes.Where(
                    g => g.Key.ManufacturerId == manufacturerId
                );
            }
            if (wheelSize != null)
            {
                bikes = bikes.Where(
                    g => g.Key.WheelSize == wheelSize
                );
            }
            if (frameSize != null)
            {
                bikes = bikes.Where(
                    g => g.Key.FrameSize == frameSize
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
                    PlaceBikeCount = new List<PlaceBikeCountDto>()

                }).ToListAsync();
            return result;
        }

        private bool BikeExists(int id)
        {
            return _context.Bikes.Any(e => e.BikeId == id);
        }
    }
}
