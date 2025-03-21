﻿using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.BikeApp.Controllers
{
    [Authorize(Policy = "AccessToken")]
    [Route("api/[controller]")]
    [ApiController]
    public class PartTypesController : ControllerBase
    {
        private readonly IPartTypesService _partTypesService;
        public PartTypesController(IPartTypesService partTypesService)
        {
            _partTypesService = partTypesService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<PartTypeDto>>> GetPartTypes(short id)
        {
            var result = await _partTypesService.GetPartTypes(id);
            return Ok(result.Data);
        }
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<PartCategoryDto>>> GetCategories()
        {
            var result = await _partTypesService.GetPartCategories();
            return Ok(result.Data);
        }
    }
}
