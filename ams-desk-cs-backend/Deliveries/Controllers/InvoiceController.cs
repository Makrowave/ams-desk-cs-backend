using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ams_desk_cs_backend.Deliveries.Interfaces;
using ams_desk_cs_backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Deliveries.Controllers;


[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class InvoiceController(IInvoiceService service) : ErrorOrController
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetInvoice(int id)
    {
        return ErrorOrToResponse(await service.GetInvoice(id));
    }
    
    [HttpGet]
    public async Task<IActionResult> GetInvoices()
    {
        return ErrorOrToResponse(await service.GetInvoices());
    }
    [HttpGet]
    [Route("NotAssignedInvoice")]
    public async Task<IActionResult> GetNotAssignedInvoices([FromQuery]int? invoiceId)
    {
        return ErrorOrToResponse(await service.NotAssignedInvoices(invoiceId));
    }
    
    [HttpPost]
    public async Task<IActionResult> PostInvoice([FromBody] NewInvoiceDto invoice)
    {
        return ErrorOrToResponse(await service.CreateInvoice(invoice));
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutInvoice(int id, [FromBody] InvoiceDto invoice)
    {
        return ErrorOrToResponse(await service.UpdateInvoice(invoice));
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteInvoice(int id)
    {
        return ErrorOrToResponse(await service.DeleteInvoice(id));
    }
    
}