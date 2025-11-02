using ams_desk_cs_backend.Deliveries.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Deliveries.Controllers;


[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class DeliveriesController : ControllerBase
{
    /// <summary>
    /// Returns all deliveries
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliverySummaryDto>>> GetDeliveries()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns a delivery corresponding to id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<DeliveryDto>> GetDelivery(int id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Updates a delivery
    /// </summary>
    /// <param name="id"></param>
    /// <param name="deliveryDto"></param>
    /// <returns></returns>
    [HttpPut("update/{id}")]
    public async Task<ActionResult<DeliveryDto>> UpdateDelivery(int id, [FromBody] DeliveryDto deliveryDto)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a new delivery with current date and pending status
    /// </summary>
    /// <param name="deliveryDto"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<ActionResult<DeliveryDto>> PostNewDelivery([FromBody] NewDeliveryDto deliveryDto)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Starts a delivery setting its status to started and enables adding items to it
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("start/{id:int}")]
    public async Task<ActionResult<DeliveryDto>> PostStartDelivery(int id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Finishes delivery setting its status to finished and disables adding items to it
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("finish/{id:int}")]
    public async Task<ActionResult<DeliveryDto>> PostFinishDelivery(int id)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Cancels delivery setting its status to canels and disabled editing
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("cancel/{id:int}")]
    public async Task<ActionResult<DeliveryDto>> PostCancelDelivery(int id)
    {
        throw new NotImplementedException();
    }
    
}