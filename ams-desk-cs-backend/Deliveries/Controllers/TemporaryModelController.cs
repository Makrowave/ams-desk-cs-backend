using ams_desk_cs_backend.Deliveries.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Deliveries.Controllers;


[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class TemporaryModelController : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] TemporaryModelDto temporaryModelDto)
    {
        throw new NotImplementedException();
    }
    
}