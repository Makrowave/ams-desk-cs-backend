using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ams_desk_cs_backend.Deliveries.Enums;
using ams_desk_cs_backend.Deliveries.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Deliveries.Services;

public class DeliveryService(BikesDbContext dbContext, IDeliveryItemService deliveryItemService) : IDeliveryService
{
    public async Task<ErrorOr<List<DeliverySummaryDto>>> GetDeliveries()
    {
        var result = await dbContext.Deliveries.Include(delivery => delivery.Place)
            .Include(delivery => delivery.Invoice)
            .OrderByDescending(delivery => delivery.PlannedArrivalDate)
            .Select(delivery => new DeliverySummaryDto(delivery)).ToListAsync();
        return result;
    }

    public async Task<ErrorOr<DeliveryDto>> GetDelivery(int deliveryId)
    {
        var result = await GetCompleteDeliveryAsync(deliveryId);
        return result == null ? Error.NotFound(description: "Nie znaleziono dostawy") : new DeliveryDto(result);
    }

    public async Task<ErrorOr<DeliveryDto>> UpdateDelivery(int id, DeliveryDto deliveryDto)
    {
        var delivery = await dbContext.Deliveries.FirstOrDefaultAsync(delivery => delivery.Id == id);
        if (delivery == null) return Error.NotFound(description: "Nie znaleziono dostawy");
        delivery.Place = deliveryDto.Place;
        delivery.PlaceId = deliveryDto.PlaceId;
        delivery.PlannedArrivalDate = deliveryDto.PlannedArrivalDate;
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
        
        var invoice = await dbContext.Invoices.FirstOrDefaultAsync(invoice => invoice.Id == delivery.InvoiceId);
        if (invoice == null) return Error.NotFound(description: "Nie znaleziono faktury");
        
        invoice.DeliveryId = delivery.Id;
        await dbContext.SaveChangesAsync();
        
        return new DeliveryDto(delivery);
    }

    public async Task<ErrorOr<DeliveryDto>> StartDelivery(int id)
    {
        var delivery = await dbContext.Deliveries.FirstOrDefaultAsync(delivery => delivery.Id == id);
        if (delivery == null) return Error.NotFound(description: "Nie znaleziono dostawy");
        if (delivery.Status != (int)DeliveryStatus.Pending) return Error.Validation(description: "Nie można rozpocząć dostawy");
        delivery.Status = (int)DeliveryStatus.Started;
        delivery.StartDate = DateTime.UtcNow;
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
        delivery.FinishDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        var modelsResolved = await ResolveTemporaryModels(delivery);

        if (modelsResolved.IsError) return modelsResolved.FirstError; 
        return new DeliveryDto(delivery);
    }

    public async Task<ErrorOr<DeliveryDto>> CancelDelivery(int id)
    {
        var delivery = await dbContext.Deliveries.FirstOrDefaultAsync(delivery => delivery.Id == id);
        if (delivery == null) return Error.NotFound(description: "Nie znaleziono dostawy");
        if (delivery.Status == (int)DeliveryStatus.Finished) return Error.Validation(description: "Nie można anulować dostawy");;
        delivery.Status = (int)DeliveryStatus.Cancelled;
        delivery.FinishDate = DateTime.UtcNow;
        
        
        var invoice = await dbContext.Invoices.FirstOrDefaultAsync(invoice => invoice.Id == delivery.InvoiceId);

        if (invoice == null) return Error.NotFound(description: "Nie znaleziono faktury");

        invoice.DeliveryId = null;
        delivery.InvoiceId = null;
        
        await dbContext.SaveChangesAsync();
        return new DeliveryDto(delivery);
    }

    public async Task<ErrorOr<Delivery?>> ResolveTemporaryModels(Delivery delivery)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var documents = delivery.DeliveryDocuments.ToList();

            var deliveryItems = documents
                .SelectMany(document => document.DeliveryItems)
                .ToList();

            // Validate XOR constraint
            foreach (var item in deliveryItems)
            {
                var hasTemp = item.TemporaryModelId.HasValue;
                var hasModel = item.ModelId.HasValue;
                if (hasTemp == hasModel)
                    return Error.Validation(description: "Item must have either ModelId or TemporaryModelId");
            }

            var temporaryModels = deliveryItems
                .Where(item => item.TemporaryModel != null && item.TemporaryModelId.HasValue)
                .Select(item => item.TemporaryModel!)
                .ToList();

            if (temporaryModels.Count == 0)
            {
                return delivery;
            }

            // Get all existing models with EAN codes that match our temporary models
            var tempEanCodes = temporaryModels
                .Where(tm => !string.IsNullOrEmpty(tm.EanCode))
                .Select(tm => tm.EanCode!)
                .ToList();

            var existingModels = await dbContext.Models
                .Where(m => m.EanCode != null && tempEanCodes.Contains(m.EanCode))
                .ToListAsync();

            var existingEanCodes = existingModels.Select(m => m.EanCode).ToHashSet();

            // Only create models for temporary models that don't already exist
            var temporaryModelsToInsert = temporaryModels
                .Where(tm => string.IsNullOrEmpty(tm.EanCode) || !existingEanCodes.Contains(tm.EanCode))
                .ToList();

            // Create models from temporary models
            var resolvedModelsOptional = temporaryModelsToInsert
                .Select(Model.ModelFromTemporaryModel)
                .ToList();

            if (resolvedModelsOptional.Contains(null))
                return Error.Validation(description: "Cannot create model from temporary model");

            var newModels = resolvedModelsOptional.Where(m => m != null).ToList();

            // Insert only new models
            if (newModels.Count > 0)
            {
                dbContext.Models.AddRange(newModels!);
                await dbContext.SaveChangesAsync(); // Get IDs assigned
            }

            // Combine existing and newly created models
            var allResolvedModels = existingModels.Concat(newModels).ToList();

            // Update delivery items - map to either existing or newly created models
            foreach (var item in deliveryItems.Where(i => i.TemporaryModelId.HasValue))
            {
                var tempModel = item.TemporaryModel;
                if (tempModel == null) continue;

                // Find matching model by EAN code
                var matchingModel = allResolvedModels.FirstOrDefault(m =>
                    !string.IsNullOrEmpty(m.EanCode) &&
                    m.EanCode == tempModel.EanCode);

                if (matchingModel != null)
                {
                    item.ModelId = matchingModel.Id;
                    item.TemporaryModelId = null;
                    item.TemporaryModel = null;
                }
                else
                {
                    // This shouldn't happen, but handle gracefully
                    return Error.Validation(
                        description: $"Could not resolve temporary model with EAN: {tempModel.EanCode}");
                }
            }

            dbContext.DeliveryItems.UpdateRange(deliveryItems);
            dbContext.TemporaryModels.RemoveRange(temporaryModels);

            await dbContext.SaveChangesAsync();
            
            
            var moveAllToStorageResult = await deliveryItemService.MoveMultipleToStorageAsync(delivery);

            if (moveAllToStorageResult.IsError)
            {
                return Error.Conflict();
            }

            await transaction.CommitAsync();
            
            return delivery;
        }
        catch
        {
            await transaction.RollbackAsync();
            return Error.Validation(description: "Nie udało się ukończyć zadania");
        }
        
    }


    private async Task<Delivery?> GetCompleteDeliveryAsync(int deliveryId)
    {
        return await dbContext.Deliveries.Include(delivery => delivery.Place)
            .Include(delivery => delivery.DeliveryDocuments)
            .ThenInclude(document => document.DeliveryItems)
            .ThenInclude(deliveryItem => deliveryItem.TemporaryModel)
            // Model then color include
            .Include(delivery => delivery.DeliveryDocuments)
            .ThenInclude(document => document.DeliveryItems)
            .ThenInclude(deliveryItem => deliveryItem.Model)
            .ThenInclude(model => model.Color)
            // Manufacturer
            .Include(delivery => delivery.DeliveryDocuments)
            .ThenInclude(document => document.DeliveryItems)
            .ThenInclude(deliveryItem => deliveryItem.Model)
            .ThenInclude(model => model.Manufacturer)
            // Category
            .Include(delivery => delivery.DeliveryDocuments)
            .ThenInclude(document => document.DeliveryItems)
            .ThenInclude(deliveryItem => deliveryItem.Model)
            .ThenInclude(model => model.Category)
            // Invoice
            .Include(delivery => delivery.Invoice)
            .FirstOrDefaultAsync(delivery => delivery.Id == deliveryId);
    }
}