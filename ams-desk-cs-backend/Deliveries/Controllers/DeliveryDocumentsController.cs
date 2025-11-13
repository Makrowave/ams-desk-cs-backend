using ams_desk_cs_backend.Deliveries.Dtos;
using ams_desk_cs_backend.Deliveries.Interfaces;
using ams_desk_cs_backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Deliveries.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class DeliveryDocumentsController(IDeliveryDocumentService deliveryDocumentService) : ErrorOrController
{
    [HttpGet("{deliveryId:int}")]
    public async Task<IActionResult> GetDeliveryDocuments(int deliveryId)
    {
        var result = await deliveryDocumentService.GetDeliveryDocumentsByDeliveryIdAsync(deliveryId);
        return ErrorOrToResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddDeliveryDocument([FromBody] NewDeliveryDocumentDto value)
    {
        var result = await deliveryDocumentService.CreateDeliveryDocumentAsync(value);
        return ErrorOrToResponse(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateDeliveryDocument([FromBody] DeliveryDocumentDto value)
    {
        var result = await deliveryDocumentService.UpdateDeliveryDocumentAsync(value);
        return ErrorOrToResponse(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDeliveryDocument(int id)
    {
        var result = await deliveryDocumentService.DeleteDeliveryDocumentAsync(id);
        return ErrorOrToResponse(result);    
    }
}