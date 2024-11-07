using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ams_desk_cs_backend.BikeApp.Api.Controllers
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
        /*
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
        */
     
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
