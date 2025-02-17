﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;
using ams_desk_cs_backend.BikeApp.Dtos;
using ams_desk_cs_backend.BikeApp.Interfaces;

namespace ams_desk_cs_backend.BikeApp.Controllers
{
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
        public async Task<IActionResult> PostModel(ModelDto model)
        {
            var result = await _modelsService.AddModel(model);
            if (result.Status == ServiceStatus.BadRequest)
            {
                return BadRequest(result.Message);
            }
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModel(int id, ModelDto model)
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
            return NoContent();

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
            return NoContent();
        }
    }
}
