using ams_desk_cs_backend.BikeService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ams_desk_cs_backend.BikeService.Controllers
{
    [Authorize(Policy = "AccessToken")]
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase

       
    {
        private readonly BikesDbContext _context;
        public ColorsController(BikesDbContext context)
        {
            _context = context;
        }

        // GET: api/<ColorsController>
        [HttpGet]
        public async Task<IEnumerable<Color>>Get()
        {
            return await _context.Colors.ToListAsync();
        }

        // GET api/<ColorsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Color>> Get(int id)
        {
            var color = _context.Colors.Find(id);
            if(color == null)
            {
                return NotFound();
            }
            return color;
        }
    }
}
