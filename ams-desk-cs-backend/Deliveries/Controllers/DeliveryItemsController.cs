using ams_desk_cs_backend.Deliveries.Dtos;
using ams_desk_cs_backend.Deliveries.Interfaces;
using ams_desk_cs_backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Deliveries.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class DeliveryItemsController(IDeliveryItemService deliveryItemService) : ErrorOrController
{
    [HttpPost]
    public async Task<IActionResult> PostNewItem(NewDeliveryItemDto item)
    {
        return ErrorOrToResponse(await deliveryItemService.AddDeliveryItemAsync(item));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        return ErrorOrToResponse(await deliveryItemService.DeleteDeliveryItemAsync(id));
    }

    [HttpPost("increment/{id:int}")]
    public async Task<IActionResult> Increment(int id)
    {
        return ErrorOrToResponse(await deliveryItemService.IncrementCountAsync(id));
    }
    
    [HttpPost("decrement/{id:int}")]
    public async Task<IActionResult> Decrement(int id)
    {
        return ErrorOrToResponse(await deliveryItemService.DecrementCountAsync(id));
    }

    [HttpPost("addToStorage/{id:int}")]
    public async Task<IActionResult> AddToStorage(int id)
    {
        return ErrorOrToResponse(await deliveryItemService.MoveToStorageAsync(id));
    }
}