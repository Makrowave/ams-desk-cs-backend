using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ErrorOr;

namespace ams_desk_cs_backend.Deliveries.Interfaces;

public interface ITemporaryModelService
{
    Task<ErrorOr<TemporaryModel>> CreateTemporaryModelAsync(string ean);
    Task<ErrorOr<TemporaryModelDto>> UpdateTemporaryModelAsync(TemporaryModelDto deliveryModelDto);
    Task<ErrorOr<Success>> DeleteTemporaryModelAsync(int id);
}