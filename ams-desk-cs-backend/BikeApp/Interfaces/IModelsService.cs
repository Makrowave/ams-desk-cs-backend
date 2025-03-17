using ams_desk_cs_backend.BikeApp.Dtos;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.LoginApp.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Interfaces
{
    public interface IModelsService
    {
        public Task<ServiceResult<IEnumerable<ModelRecordDto>>> GetModelRecords(ModelFilter filter);
        public Task<ServiceResult<ModelRecordDto>> AddModel(ModelDto model);
        public Task<ServiceResult<ModelRecordDto>> UpdateModel(int id, ModelDto model);
        public Task<ServiceResult> DeleteModel(int id);
        public Task<ServiceResult<bool>> SetFavorite(int id, bool favorite);
        public Task<ServiceResult<IEnumerable<FavoriteModelDto>>> GetLowFavorites();
    }
}
