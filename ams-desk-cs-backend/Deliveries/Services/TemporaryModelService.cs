using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ams_desk_cs_backend.Deliveries.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Deliveries.Services;

public class TemporaryModelService(BikesDbContext dbContext) : ITemporaryModelService
{
    public async Task<ErrorOr<TemporaryModel>> CreateTemporaryModelAsync(string ean)
    {
        if (await ModelWithEanExistsAsync(ean)) return Error.Conflict("Model o danym kodzie EAN istnieje");
        
        var temporaryModel = new TemporaryModel { EanCode = ean };
        dbContext.TemporaryModels.Add(temporaryModel);
        await dbContext.SaveChangesAsync();
        
        return temporaryModel;
    }

    public async Task<ErrorOr<DeliveryModelDto>> UpdateTemporaryModelAsync(DeliveryModelDto deliveryModelDto)
    {
        if(deliveryModelDto.EanCode is null) return Error.Validation("Kod EAN musi być podany");
        
        var temporaryModel = await dbContext.TemporaryModels.FirstOrDefaultAsync(model => model.Id == deliveryModelDto.Id);
        if (temporaryModel is null) return Error.NotFound("Nie znaleziono modelu");

        if (temporaryModel.EanCode != deliveryModelDto.EanCode)
        {
            if(await ModelWithEanExistsAsync(deliveryModelDto.EanCode)) return Error.Conflict("Model o danym kodzie EAN istnieje");
        }
        
        temporaryModel.EanCode = deliveryModelDto.EanCode;
        temporaryModel.ProductCode = deliveryModelDto.ProductCode;
        temporaryModel.Name = deliveryModelDto.Name;
        temporaryModel.FrameSize = deliveryModelDto.FrameSize;
        temporaryModel.IsWoman = deliveryModelDto.IsWoman;
        temporaryModel.WheelSizeId = deliveryModelDto.WheelSizeId;
        temporaryModel.ManufacturerId = deliveryModelDto.ManufacturerId;
        temporaryModel.CategoryId = deliveryModelDto.CategoryId;
        temporaryModel.PrimaryColor = deliveryModelDto.PrimaryColor;
        temporaryModel.SecondaryColor = deliveryModelDto.SecondaryColor;
        temporaryModel.Price = deliveryModelDto.Price;
        temporaryModel.IsElectric = deliveryModelDto.IsElectric;
        temporaryModel.Link = deliveryModelDto.Link;
        
        await dbContext.SaveChangesAsync();
        
        return new DeliveryModelDto(temporaryModel);
    }

    public async Task<ErrorOr<Success>> DeleteTemporaryModelAsync(DeliveryModelDto deliveryModelDto)
    {
        await dbContext.TemporaryModels.Where(temporaryModel => temporaryModel.Id == deliveryModelDto.Id).ExecuteDeleteAsync();
        return new Success();
    }


    private async Task<bool> ModelWithEanExistsAsync(string ean)
    {
        var model = await dbContext.Models.FirstOrDefaultAsync(model => model.EanCode == ean);
        if (model is not null) return true; 
        
        var temporaryModel = await dbContext.Models.FirstOrDefaultAsync(temporaryModel => temporaryModel.EanCode == ean);
        return temporaryModel is not null;
    }
}