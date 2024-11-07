﻿using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Application.Interfaces
{
    public interface IPlacesService
    {
        public Task<ServiceResult<IEnumerable<PlaceDto>>> GetPlaces();
    }
}
