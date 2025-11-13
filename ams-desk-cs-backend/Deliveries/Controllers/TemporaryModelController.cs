using ams_desk_cs_backend.Deliveries.Dtos;
using ams_desk_cs_backend.Deliveries.Interfaces;
using ams_desk_cs_backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Deliveries.Controllers;


[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class TemporaryModelController(ITemporaryModelService temporaryModelService) : ErrorOrController
{
    [HttpPatch]
    public async Task<IActionResult> HttpPatch([FromBody] TemporaryModelDto temporaryModelDto)
    {
        return ErrorOrToResponse(await temporaryModelService.UpdateTemporaryModelAsync(temporaryModelDto));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return ErrorOrToResponse(await temporaryModelService.DeleteTemporaryModelAsync(id));
    }
}