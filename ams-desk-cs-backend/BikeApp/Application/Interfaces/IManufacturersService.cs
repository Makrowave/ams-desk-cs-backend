﻿using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Application.Interfaces
{
    public interface IManufacturersService
    {
        public Task<ServiceResult<IEnumerable<ManufacturerDto>>> GetManufacturers();
        public Task<ServiceResult> PostManufacturer(ManufacturerDto manufacturer);
        public Task<ServiceResult> UpdateManufacturer(short id, ManufacturerDto manufacturer);
        public Task<ServiceResult> DeleteManufacturer(short id);
    }
}
