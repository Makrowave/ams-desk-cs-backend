using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.BikeFilters.Enums;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ams_desk_cs_backend.Deliveries.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Deliveries.Services;

public class DeliveryItemService(ITemporaryModelService temporaryModelService, BikesDbContext dbContext) : IDeliveryItemService
{
    public async Task<ErrorOr<DeliveryItemDto>> AddDeliveryItemAsync(NewDeliveryItemDto deliveryItemDto)
    {
        if (deliveryItemDto.ModelId is not null && deliveryItemDto.Ean is not null) return Error.Validation();
        if (!(deliveryItemDto.ModelId is not null || deliveryItemDto.Ean is not null)) return Error.Validation();

        //If model exists get it
        if (deliveryItemDto.ModelId.HasValue)
        {
            var model = await dbContext.Models.FindAsync(deliveryItemDto.ModelId.Value);
            
            if (model is null) return Error.NotFound("Nie znaleziono modelu");
            
            var modelDeliveryItem = new DeliveryItem
            {
                DeliveryDocumentId = deliveryItemDto.DeliveryDocumentId,
                ModelId = deliveryItemDto.ModelId.Value,
                Model = model,
                Count = 0,
                StorageCount = 0,
            };
            dbContext.DeliveryItems.Add(modelDeliveryItem);
            await dbContext.SaveChangesAsync();
            return new DeliveryItemDto(modelDeliveryItem);
        }

        if (deliveryItemDto.Ean is null) return Error.Validation();
        
        var temporaryModelExists = await dbContext.TemporaryModels
            .AnyAsync(model => model.EanCode == deliveryItemDto.Ean);

        // For mobile = if temporary model exists then increment
        if (temporaryModelExists)
        {
            var deliveryItem = await dbContext.DeliveryItems
                .Include(deliveryItem => deliveryItem.TemporaryModel)
                .FirstOrDefaultAsync(item => item.TemporaryModel != null && item.TemporaryModel.EanCode == deliveryItemDto.Ean);

            if(deliveryItem is null) return Error.Validation();
            
            var count = await IncrementAsync(deliveryItem);
            
            return new DeliveryItemDto(deliveryItem);
        }
        
        // Create new temporary model
        var temporaryModel = await temporaryModelService.CreateTemporaryModelAsync(deliveryItemDto.Ean);
        if (temporaryModel.IsError) return temporaryModel.Errors;

        var temporaryModelDeliveryItem = new DeliveryItem
        {
            DeliveryDocumentId = deliveryItemDto.DeliveryDocumentId,
            TemporaryModelId = temporaryModel.Value.Id,
            TemporaryModel = temporaryModel.Value,
            Count = 0,
            StorageCount = 0,
        };
        
        dbContext.DeliveryItems.Add(temporaryModelDeliveryItem);
        await dbContext.SaveChangesAsync();
        
        return new DeliveryItemDto(temporaryModelDeliveryItem);
    }

    public async Task<ErrorOr<Success>> DeleteDeliveryItemAsync(int id)
    {
        var deliveryItem = await dbContext.DeliveryItems.Include(item => item.TemporaryModel)
            .FirstOrDefaultAsync(item => item.Id == id);
        if(deliveryItem is null) return Error.NotFound("Nie znaleziono modelu");
        
        if(deliveryItem.TemporaryModel != null) dbContext.TemporaryModels.Remove(deliveryItem.TemporaryModel);
        
        dbContext.DeliveryItems.Remove(deliveryItem);
        
        await dbContext.SaveChangesAsync();
        return new Success();
        
    }

    public async Task<ErrorOr<int>> IncrementCountAsync(int id)
    {
        var deliveryItem = await dbContext.DeliveryItems.FindAsync(id);

        if (deliveryItem == null) return Error.NotFound("Nie znaleziono modelu");
        

        return await IncrementAsync(deliveryItem);
    }

    public async Task<ErrorOr<int>> DecrementCountAsync(int id)
    {
        var deliveryItem = await dbContext.DeliveryItems.FindAsync(id);

        if (deliveryItem == null) return Error.NotFound("Nie znaleziono modelu");

        if (deliveryItem.Count <= 0) return deliveryItem.Count;
        
        deliveryItem.Count -= 1;
        await dbContext.SaveChangesAsync();
        return deliveryItem.Count;
    }

    public async Task<ErrorOr<DeliveryItemDto>> MoveToStorageAsync(int deliveryItemId)
    {
        var deliveryItem = await dbContext.DeliveryItems.Include(di => di.DeliveryDocument)
            .ThenInclude(dd => dd!.Delivery).FirstOrDefaultAsync(di => di.Id == deliveryItemId);
        
        if (deliveryItem is null) return Error.NotFound("Nie znaleziono przedmiotu");

        try
        {
            var bikes = CreateBikesToStore(deliveryItem);
            dbContext.Bikes.AddRange(bikes);
            deliveryItem.StorageCount = deliveryItem.Count;
            await dbContext.SaveChangesAsync();

            return new DeliveryItemDto(deliveryItem);
        }
        catch
        {
            return Error.Validation("Nastąpił błąd przy dodawaniu");
        }
    }

    public async Task<ErrorOr<DeliveryDocument>> MoveMultipleToStorageAsync(int deliveryDocumentId)
    {
        throw new NotImplementedException();
    }

    private IEnumerable<Bike> CreateBikesToStore(DeliveryItem deliveryItem)
    {
        if (deliveryItem.ModelId is null) throw new ValidationException("No modelId in deliveryItem");
        
        if (deliveryItem.Count < deliveryItem.StorageCount) throw new ValidationException("Delivery count is less than storage count");
        
        if(deliveryItem.DeliveryDocument?.Delivery?.PlaceId == null) throw new ArgumentException("Provided delivery document does not have required navigation properties.");
        
        return Enumerable.Range(0, deliveryItem.Count - deliveryItem.StorageCount)
            .Select(_ => new Bike
            {
                ModelId = deliveryItem.ModelId.Value,
                PlaceId = deliveryItem.DeliveryDocument.Delivery.PlaceId,
                StatusId = (short)BikeStatus.NotAssembled,
                InsertionDate = DateOnly.FromDateTime(DateTime.Now),
                PurchaseCost = deliveryItem.PurchaseCost,
                InternetSale = false,
            });
    }

    private async Task<int> IncrementAsync(DeliveryItem deliveryItem)
    {
        deliveryItem.Count++;
        
        await dbContext.SaveChangesAsync();
        
        return deliveryItem.Count;
    }
}