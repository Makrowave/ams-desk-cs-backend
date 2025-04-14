using System.IdentityModel.Tokens.Jwt;
using ams_desk_cs_backend.Bikes.Dtos;
using ams_desk_cs_backend.Bikes.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Bikes.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class BikesController : ControllerBase
{
    private readonly IBikesService _bikesService;

    public BikesController(IBikesService bikesService)
    {
        _bikesService = bikesService;
    }

    // GET: api/Bikes
    [HttpGet("bikesByModelId/{modelId}")]
    public async Task<ActionResult<IEnumerable<BikeSubRecordDto>>> GetBikes(int modelId, short placeId)
    {
        var result = await _bikesService.GetBikes(modelId, placeId);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }

    // PUT: api/Bikes/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<ActionResult<BikeSubRecordDto>> PutBike(int id, BikeDto bike)
    {
        var result = await _bikesService.PutBike(id, bike);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }

    // POST: api/Bikes
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<BikeSubRecordDto>> PostBike(BikeDto bike)
    {
        var result = await _bikesService.PostBike(bike);
        if (result.Status == ServiceStatus.BadRequest)
        {
            return BadRequest(result.Message);
        }
        return Ok(result.Data);
    }
    // DELETE: api/Bikes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBike(int id)
    {
        var result = await _bikesService.DeleteBike(id);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return NoContent();
    }

    [HttpPut("Sell/{id}")]
    public async Task<IActionResult> SellBike(int id, int price)
    {
        var result = await _bikesService.SellBike(id, price);
        return result.Status switch
        {
            ServiceStatus.Ok => Ok(new {PlaceId = result.Data.PlaceId, ModelId = result.Data.ModelId}),
            ServiceStatus.NotFound => NotFound(result.Message),
            _ => BadRequest(result.Message)
        };
    }
    [HttpPut("Move/{id}")]
    public async Task<IActionResult> MoveBike(int id, short placeId)
    {
        var result = await _bikesService.MoveBike(id, placeId);
        return result.Status switch
        {
            ServiceStatus.Ok => Ok(result.Data),
            ServiceStatus.NotFound => NotFound(result.Message),
            _ => BadRequest(result.Message)
        };
    }
    [HttpPut("AssembleMobile/{id}")]
    public async Task<IActionResult> AssembleBikeMobile(int id)
    {
        //Temporary solution
        string authHeader = Request.Headers["Authorization"];
        short employeeId = -1;
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            string token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token); 
        
            var claims = jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);
            claims.TryGetValue("employeeId", out var employeeIdStr);
            short.TryParse(employeeIdStr, out employeeId);
        }

        if (employeeId == -1)
        {
            return BadRequest("Nie przypisano pracownika do użytkownika - powiadom administratora");
        }
        var result = await _bikesService.AssembleBikeMobile(id, employeeId);
        return result.Status switch
        {
            ServiceStatus.Ok => Ok(result.Data),
            ServiceStatus.NotFound => NotFound(result.Message),
            _ => BadRequest(result.Message)
        };
    }
}