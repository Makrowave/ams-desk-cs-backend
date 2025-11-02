using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Deliveries.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class DeliveryItemController : ControllerBase
{
    [HttpPost("{deliveryId}")]
    public async Task<IActionResult> PostNewItem(int deliveryId, [FromBody] string ean)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost("increment/{id:int}")]
    public async Task<IActionResult> Increment(int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("decrement/{id:int}")]
    public async Task<IActionResult> Decrement(int id)
    {
        throw new NotImplementedException();
    }
}