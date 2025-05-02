using ams_desk_cs_backend.Models.Dtos;
using ams_desk_cs_backend.Models.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Models.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class ModelsController : ControllerBase
{
    private readonly IModelsService _modelsService;

    public ModelsController(IModelsService modelsService)
    {
        _modelsService = modelsService;
    }


    [HttpGet]
    public async Task<ActionResult<ModelRecordDto>> GetModelRecords([FromQuery] ModelFilter filter)
    {
        //Don't want return error here - empty table is desired
        var result = await _modelsService.GetModelRecords(filter);
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<ModelRecordDto>> PostModel(ModelDto model)
    {
        var result = await _modelsService.AddModel(model);
        if (result.Status == ServiceStatus.BadRequest)
        {
            return BadRequest(result.Message);
        }
        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ModelRecordDto>> UpdateModel(int id, ModelDto model)
    {
        var result = await _modelsService.UpdateModel(id, model);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        if (result.Status == ServiceStatus.BadRequest)
        {
            return BadRequest(result.Message);
        }
        return Ok(result.Data);

    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<IActionResult> DeleteModel(int id)
    {
        var result = await _modelsService.DeleteModel(id);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        if (result.Status == ServiceStatus.BadRequest)
        {
            return BadRequest(result.Message);
        }
        return Ok();
    }

    [HttpPut("favorite/{id}")]
    public async Task<ActionResult<bool>> SetFavorite(int id, [FromQuery]bool favorite)
    {
        var result = await _modelsService.SetFavorite(id, favorite);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Data);
    }

    [HttpPut("updateEan/{id}")]
    public async Task<ActionResult<ModelRecordDto>> UpdateEan(int id, [FromQuery] string ean)
    {
        var result = await _modelsService.UpdateEan(id, ean);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        if (result.Status == ServiceStatus.BadRequest)
        {
            return BadRequest(result.Message);
        }
        return Ok(result.Data);
    }

    [HttpGet("favorite")]
    public async Task<ActionResult<IEnumerable<FavoriteModelDto>>> GetLowFavorites()
    {
        return Ok((await _modelsService.GetLowFavorites()).Data);
    }
}