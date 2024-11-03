using ams_desk_cs_backend.BikeApp.Api.Dtos;
using ams_desk_cs_backend.BikeApp.Application.Results;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;

namespace ams_desk_cs_backend.BikeApp.Application.Interfaces
{
    public interface IModelsService
    {
        public Task<ServiceResult<IEnumerable<ModelRecordDto>>> GetModelRecords(ModelFilter filter);
        public Task<ServiceResult> AddModel(ModelDto model);
        public Task<ServiceResult> UpdateModel(int id, ModelDto model);
    }
}
