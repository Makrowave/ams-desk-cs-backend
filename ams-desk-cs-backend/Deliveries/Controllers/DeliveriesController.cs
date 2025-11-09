using ams_desk_cs_backend.Deliveries.Dtos;
using ams_desk_cs_backend.Deliveries.Interfaces;
using ams_desk_cs_backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Deliveries.Controllers;


[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class DeliveriesController(IDeliveryService deliveryService) : ErrorOrController
{
    /// <summary>
    /// Returns all deliveries
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetDeliveries()
    {
        return ErrorOrToResponse(await deliveryService.GetDeliveries());
    }

    /// <summary>
    /// Returns a delivery corresponding to id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDelivery(int id)
    {
        return ErrorOrToResponse(await deliveryService.GetDelivery(id));
    }

    /// <summary>
    /// Updates a delivery
    /// </summary>
    /// <param name="id"></param>
    /// <param name="deliveryDto"></param>
    /// <returns></returns>
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateDelivery(int id, [FromBody] DeliveryDto deliveryDto)
    {
        return ErrorOrToResponse(await deliveryService.UpdateDelivery(id, deliveryDto));
    }

    /// <summary>
    /// Creates a new delivery with current date and pending status
    /// </summary>
    /// <param name="deliveryDto"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<IActionResult> PostNewDelivery([FromBody] NewDeliveryDto deliveryDto)
    {
        return ErrorOrToResponse(await deliveryService.AddDelivery(deliveryDto));
    }

    /// <summary>
    /// Starts a delivery setting its status to started and enables adding items to it
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("start/{id:int}")]
    public async Task<IActionResult>PostStartDelivery(int id)
    {
        return ErrorOrToResponse(await deliveryService.StartDelivery(id));
    }

    /// <summary>
    /// Finishes delivery setting its status to finished and disables adding items to it
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("finish/{id:int}")]
    public async Task<IActionResult> PostFinishDelivery(int id)
    {
        return ErrorOrToResponse(await deliveryService.FinishDelivery(id));
    }
    /// <summary>
    /// Cancels delivery setting its status to cancels and disabled editing
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("cancel/{id:int}")]
    public async Task<IActionResult> PostCancelDelivery(int id)
    {
        return ErrorOrToResponse(await deliveryService.CancelDelivery(id));
    }
    
}