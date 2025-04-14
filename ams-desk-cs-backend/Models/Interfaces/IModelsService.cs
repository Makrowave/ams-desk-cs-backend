using ams_desk_cs_backend.Models.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.Models.Interfaces;

public interface IModelsService
{
    public Task<ServiceResult<IEnumerable<ModelRecordDto>>> GetModelRecords(ModelFilter filter);
    public Task<ServiceResult<ModelRecordDto>> AddModel(ModelDto model);
    public Task<ServiceResult<ModelRecordDto>> UpdateModel(int id, ModelDto model);
    public Task<ServiceResult> DeleteModel(int id);
    public Task<ServiceResult<bool>> SetFavorite(int id, bool favorite);
    public Task<ServiceResult<IEnumerable<FavoriteModelDto>>> GetLowFavorites();
}