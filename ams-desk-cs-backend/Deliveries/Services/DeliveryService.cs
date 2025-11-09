using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ams_desk_cs_backend.Deliveries.Enums;
using ams_desk_cs_backend.Deliveries.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Deliveries.Services;

public class DeliveryService(BikesDbContext dbContext) : IDeliveryService
{
    public async Task<ErrorOr<List<DeliverySummaryDto>>> GetDeliveries()
    {
        var result = await dbContext.Deliveries.Include(delivery => delivery.Place)
            .Select(delivery => new DeliverySummaryDto(delivery)).ToListAsync();
        return result;
    }

    public async Task<ErrorOr<DeliveryDto>> GetDelivery(int deliveryId)
    {
        var result = await dbContext.Deliveries.Include(delivery => delivery.Place)
            .Include(delivery => delivery.DeliveryDocuments)
            .ThenInclude(document => document.DeliveryItems)
            .FirstOrDefaultAsync(delivery => delivery.Id == deliveryId);
        return result == null ? Error.NotFound(description: "Nie znaleziono dostawy") : new DeliveryDto(result);
    }

    public async Task<ErrorOr<DeliveryDto>> UpdateDelivery(int id, DeliveryDto deliveryDto)
    {
        var delivery = await dbContext.Deliveries.FirstOrDefaultAsync(delivery => delivery.Id == id);
        if (delivery == null) return Error.NotFound(description: "Nie znaleziono dostawy");
        delivery.Place = deliveryDto.Place;
        delivery.Status = (int)deliveryDto.Status;
        delivery.PlaceId = deliveryDto.PlaceId;
        await dbContext.SaveChangesAsync();
        return new DeliveryDto(delivery);
    }

    public async Task<ErrorOr<DeliveryDto>> AddDelivery(NewDeliveryDto deliveryDto)
    {
        var delivery = new Delivery()
        {
            InvoiceId = deliveryDto.InvoiceId,
            PlaceId = deliveryDto.PlaceId,
            PlannedArrivalDate = deliveryDto.PlannedArrivalDate,
            Status = (int)DeliveryStatus.Pending,
        };
        dbContext.Add(delivery);
        await dbContext.SaveChangesAsync();
        return new DeliveryDto(delivery);
    }

    public async Task<ErrorOr<DeliveryDto>> StartDelivery(int id)
    {
        var delivery = await dbContext.Deliveries.FirstOrDefaultAsync(delivery => delivery.Id == id);
        if (delivery == null) return Error.NotFound(description: "Nie znaleziono dostawy");
        if (delivery.Status != (int)DeliveryStatus.Pending) return Error.Validation(description: "Nie można rozpocząć dostawy");
        delivery.Status = (int)DeliveryStatus.Started;
        delivery.StartDate = DateTime.Now;
        await dbContext.SaveChangesAsync();
        return new DeliveryDto(delivery);
    }

    public async Task<ErrorOr<DeliveryDto>> FinishDelivery(int id)
    {
        var delivery = await dbContext.Deliveries
            .Include(d => d.DeliveryDocuments)
            .ThenInclude(dd => dd.DeliveryItems)
            .ThenInclude(di => di.Model)
            .Include(d => d.DeliveryDocuments)
            .ThenInclude(dd => dd.DeliveryItems)
            .ThenInclude(di => di.TemporaryModel)
            .FirstOrDefaultAsync(delivery => delivery.Id == id);
        if (delivery == null) return Error.NotFound(description: "Nie znaleziono dostawy");
        if (delivery.Status != (int)DeliveryStatus.Started) return Error.Validation(description: "Nie można zakończyć dostawy");;
        delivery.Status = (int)DeliveryStatus.Finished;
        delivery.FinishDate = DateTime.Now;
        await dbContext.SaveChangesAsync();
        var modelsResolved = await ResolveTemporaryModels(delivery);

        if (modelsResolved.IsError) return modelsResolved.FirstError; 
        return new DeliveryDto(delivery);
    }

    public async Task<ErrorOr<DeliveryDto>> CancelDelivery(int id)
    {
        var delivery = await dbContext.Deliveries.FirstOrDefaultAsync(delivery => delivery.Id == id);
        if (delivery == null) return Error.NotFound(description: "Nie znaleziono dostawy");
        if (delivery.Status != (int)DeliveryStatus.Finished) return Error.Validation(description: "Nie można anulować dostawy");;
        delivery.Status = (int)DeliveryStatus.Cancelled;
        delivery.FinishDate = DateTime.Now;
        await dbContext.SaveChangesAsync();
        return new DeliveryDto(delivery);
    }

    public async Task<ErrorOr<Delivery>> ResolveTemporaryModels(Delivery delivery)
    {
        var documents = delivery.DeliveryDocuments.ToList();
        
        var unresolvedDeliveryItems = documents
            .SelectMany(document => document.DeliveryItems)
            .ToList();

        foreach (var item in unresolvedDeliveryItems)
        {
            if (item.TemporaryModelId.HasValue && item.ModelId.HasValue)
            {
                return Error.Validation();
            }
            
            if (!(item.TemporaryModelId.HasValue || item.ModelId.HasValue))
            {
                return Error.Validation();
            }
        }

        var temporaryModels =  unresolvedDeliveryItems.Where(item => item.TemporaryModel != null && item.TemporaryModelId.HasValue)
            .Select(item => item.TemporaryModel!).ToList();
        
        // Add deduplication

        var models = await dbContext.Models.Where(m => m.EanCode != null).ToListAsync();

        var temporaryModelsNotInserted = temporaryModels
            .Where(temp => models.All(m => m.EanCode != temp.EanCode));
        
        var resolvedModels = temporaryModels
            .Select(Model.ModelFromTemporaryModel).ToList();
        
        if(resolvedModels.Contains(null)) return Error.Validation();
        
        dbContext.Models.AddRange(resolvedModels!);
        await dbContext.SaveChangesAsync();
        
        unresolvedDeliveryItems.ForEach(item =>
        {
            item.TemporaryModelId = null;
            item.ModelId = resolvedModels.Find(model => model!.EanCode == item.TemporaryModel!.EanCode)?.Id;
            item.TemporaryModel = null;
        });
        
        dbContext.DeliveryItems.UpdateRange(unresolvedDeliveryItems);
        
        dbContext.TemporaryModels.RemoveRange(temporaryModels);
        
        await dbContext.SaveChangesAsync();
        
        var updatedDelivery =  (await dbContext.Deliveries
            .Include(d => d.DeliveryDocuments)
            .ThenInclude(dd => dd.DeliveryItems)
            .ThenInclude(di => di.Model)
            .Include(d => d.DeliveryDocuments)
            .ThenInclude(dd => dd.DeliveryItems)
            .ThenInclude(di => di.TemporaryModel)
            .FirstOrDefaultAsync(d => d.Id == delivery.Id))!;

        return updatedDelivery;
    }
}