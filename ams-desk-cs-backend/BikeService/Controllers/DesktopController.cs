using ams_desk_cs_backend.BikeService.Dtos;
using ams_desk_cs_backend.BikeService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Text.RegularExpressions;

namespace ams_desk_cs_backend.BikeService.Controllers
{

    [Authorize(Policy = "AccessToken")]
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
        public async Task<IActionResult> Assemble(int id, short employeeId)
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
            bike.ForEach(bi => { bi.StatusId = 2; bi.AssembledBy = employeeId; });
            _context.SaveChanges();
            return NoContent();
        }
        [HttpPut("AddLink/{id}")]
        public async Task<IActionResult> AddLink(int id, string link)
        {
            if (!ModelExists(id))
            {
                return NotFound();
            }
            var model = await _context.Models.Where(mo => mo.ModelId == id).ToListAsync();
            if(!(link.StartsWith("https://") || !link.StartsWith("http://"))) 
            {
                return BadRequest();
            }
            model.ForEach(mo => mo.Link = link);
            _context.SaveChanges();
            return NoContent();
        }
        [HttpPut("AddEan/{id}")]
        public async Task<IActionResult> AddEan(int id, string ean)
        {

            if (!Regex.IsMatch(ean, "^[0-9]{13}$"))
            {
                return BadRequest("Zły format EAN");
            }

            if (!ModelExists(id))
            {
                return NotFound();
            }
            var model = await _context.Models.Where(mo => mo.ModelId == id).ToListAsync();
            
            model.ForEach(mo => mo.EanCode = ean);
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
        public async Task<IActionResult> Move(int id, short placeId)
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
        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(int id, short statusId)
        {
            if (!BikeExists(id))
            {
                return NotFound();
            }
            var bike = await _context.Bikes.Where(bi => bi.BikeId == id).ToListAsync();
            if (bike.Any(bi => bi.StatusId == statusId))
            {
                return BadRequest();
            }
            bike.ForEach(bi => { bi.StatusId = statusId; bi.AssembledBy = null;});
            _context.SaveChanges();
            return NoContent();
        }
        [HttpPut("ChangeColor/{id}")]
        public async Task<IActionResult> ChangeColor(int id, string primaryColor, string secondaryColor)
        {
            primaryColor = "#" + primaryColor; 
            secondaryColor = "#" + secondaryColor;
            if (!ModelExists(id))
            {
                return NotFound();
            }
            if(!isValidHex(primaryColor) || !isValidHex(secondaryColor))
            {
                return BadRequest();
            }
            var model = await _context.Models.Where(m => m.ModelId == id).ToListAsync();
            model.ForEach(m => { m.PrimaryColor = primaryColor; m.SecondaryColor = secondaryColor; });
            _context.SaveChanges();
            return NoContent();
        }
        [HttpPut("ChangeColorMain/{id}")]
        public async Task<IActionResult> ChangeColorMain(int id, short colorId)
        {
            if (!ModelExists(id))
            {
                return NotFound();
            }
            if(!_context.Colors.Any(c => c.ColorId == colorId))
            {
                return BadRequest();
            }

            var model = await _context.Models.Where(m => m.ModelId == id).ToListAsync();
            model.ForEach(m => m.ColorId = colorId);
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
        [HttpPost("AddModel")]
        public async Task<IActionResult> AddModel(AddModelDto model)
        {
            var postModel = new Model
            {
                ProductCode = model.ProductCode,
                EanCode = model.EanCode,
                ModelName = model.ModelName,
                FrameSize = model.FrameSize,
                IsWoman = model.IsWoman,
                WheelSize = model.WheelSize,
                ManufacturerId = model.ManufacturerId,
                ColorId = model.ColorId,
                CategoryId = model.CategoryId,
                PrimaryColor = model.PrimaryColor,
                SecondaryColor = model.SecondaryColor,
                Price = model.Price,
                IsElectric = model.IsElectric,
                InsertionDate = DateOnly.FromDateTime(DateTime.Today),
            };
            _context.Add(postModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                actionName: nameof(BikesController.GetBike),
                controllerName: "Bikes",
                routeValues: new { id = postModel.ModelId },
                value: postModel
            );
        }

        // GET: api/Models
        [HttpGet("GetBikes")]
        public async Task<ActionResult<IEnumerable<ModelRecordDTO>>> GetModelsJoinBikes(
            bool avaible,
            bool electric,
            int? manufacturerId,
            int? wheelSize,
            int? frameSize,
            int? statusId,
            int minPrice,
            int maxPrice,
            string? name,
            bool? isWoman,
            bool isKids,
            int? categoryId,
            int? colorId,
            int? placeId
            )
        {
            var bikes = _context.Models
                .GroupJoin(
                    _context.Bikes.Where(bi => bi.StatusId != 3 && (placeId == 0 || bi.PlaceId == placeId)),
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
                        r.mo.IsElectric,
                        r.mo.CategoryId, 
                        r.mo.ColorId,
                        r.mo.PrimaryColor,
                        r.mo.SecondaryColor,
                        r.mo.Link,
                    }
                );
            bikes = bikes.Where(
                g => g.Key.Price >= minPrice && g.Key.Price <= maxPrice
            );

            if(categoryId != null)
            {
                bikes = bikes.Where(
                    g => g.Key.CategoryId == categoryId
                );
            }
            if(colorId != null)
            {
                bikes = bikes.Where(
                    g => g.Key.ColorId == colorId
                );
            }
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
            if (statusId != null)
            {
                bikes = bikes.Where(
                    g => g.Count(r => r.bi != null && r.bi.StatusId == statusId) > 0
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
                //Find just first word in name so query returns less for later step (don't really know if it's optimal)
                var words = name.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                bikes = bikes.Where(
                    g => g.Key.ModelName.ToLower().Contains(words[0])
                );
            }

            var result = await bikes.OrderBy(g => g.Key.ModelId)
                .Select(g => new ModelRecordDTO
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
                    PrimaryColor = g.Key.PrimaryColor,
                    SecondaryColor = g.Key.SecondaryColor,
                    CategoryId = g.Key.CategoryId,
                    ColorId = g.Key.ColorId,
                    Link = g.Key.Link,
                    BikeCount = g.Count(r => r.bi != null),
                    PlaceBikeCount = g.Where(r => r.bi != null)
                                        .GroupBy(r => new { r.bi!.PlaceId })
                                        .Select(d => new PlaceBikeCountDto
                                        {
                                            PlaceId = d.Key.PlaceId,
                                            Count = d.Count()
                                        })

                }).ToListAsync();
            //Checks for words from input string in model name in order but with gaps
            //For example input "Bike XL blue" will match with "Bike 4.0 XL blue" and "...Bike XL blue 4.0..."
            //but not with "Bike 4.0 blue XL"
            //And input "Bike XL XL" will not match with "Bike XL"
            if (name != null)
            {
                 var words = name.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    result = result.FindAll(model => {
                        string modelName = model.ModelName.ToLower();
                        foreach(var word in words)
                        {
                            if(!modelName.Contains(word))
                            {
                                return false;
                            }
                            //Shorten string so it won't match second occurence in input to first in model name
                            modelName = modelName.Substring(modelName.IndexOf(word) + word.Length);
                        }
                        return true;
                    });
            }
            return result;
        }


        

        private bool BikeExists(int id)
        {
            return _context.Bikes.Any(e => e.BikeId == id);
        }
        private bool ModelExists(int id)
        {
            return _context.Models.Any(e => e.ModelId == id);
        }
        private bool isValidHex(string code)
        {
            return Regex.IsMatch(code, "^#([a-fA-F0-9]{6})$");
        }
    }
}
