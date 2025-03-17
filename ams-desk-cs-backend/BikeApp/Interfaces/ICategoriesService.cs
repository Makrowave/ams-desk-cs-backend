﻿using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Interfaces
{
    public interface ICategoriesService
    {
        public Task<ServiceResult<IEnumerable<CategoryDto>>> GetCategories();
        public Task<ServiceResult<CategoryDto>> PostCategory(CategoryDto category);
        public Task<ServiceResult<CategoryDto>> UpdateCategory(short id, CategoryDto category);
        public Task<ServiceResult> ChangeOrder(short firstId, short lastId);
        public Task<ServiceResult> DeleteCategory(short id);

    }
}
