using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ams_desk_cs_backend.Deliveries.Enums;
using ams_desk_cs_backend.Deliveries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Deliveries.Services;

public class DeliveryService(BikesDbContext dbContext) : IDeliveryService
{
    public async Task<List<DeliverySummaryDto>> GetDeliveries()
    {
        var result = await dbContext.Deliveries.Include(delivery => delivery.Place)
            .Select(delivery => new DeliverySummaryDto(delivery)).ToListAsync();
        return result;
    }

    public async Task<DeliveryDto?> GetDelivery(int deliveryId)
    {
        var result = await dbContext.Deliveries.Include(delivery => delivery.Place)
            .Include(delivery => delivery.DeliveryDocuments)
            .ThenInclude(document => document.DeliveryItems)
            .FirstOrDefaultAsync(delivery => delivery.Id == deliveryId);
        return result == null ? null : new DeliveryDto(result);
    }

    public async Task<DeliveryDto?> UpdateDelivery(DeliveryDto deliveryDto)
    {
        var delivery = await dbContext.Deliveries.FirstOrDefaultAsync(delivery => delivery.Id == deliveryDto.Id);
        if (delivery == null) return null;
        delivery.Place = deliveryDto.Place;
        delivery.Status = (int)deliveryDto.Status;
        delivery.PlaceId = deliveryDto.PlaceId;
        await dbContext.SaveChangesAsync();
        return new DeliveryDto(delivery);
    }

    public async Task<DeliveryDto> AddDelivery(NewDeliveryDto deliveryDto)
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

    public async Task<DeliveryDto?> StartDelivery(int deliveryId)
    {
        var delivery = await dbContext.Deliveries.FirstOrDefaultAsync(delivery => delivery.Id == deliveryId);
        if (delivery == null) return null;
        if (delivery.Status != (int)DeliveryStatus.Pending) return null;
        delivery.Status = (int)DeliveryStatus.Started;
        delivery.StartDate = DateTime.Now;
        await dbContext.SaveChangesAsync();
        return new DeliveryDto(delivery);
    }

    public async Task<DeliveryDto?> FinishDelivery(int deliveryId)
    {
        var delivery = await dbContext.Deliveries.FirstOrDefaultAsync(delivery => delivery.Id == deliveryId);
        if (delivery == null) return null;
        if (delivery.Status != (int)DeliveryStatus.Started) return null;
        delivery.Status = (int)DeliveryStatus.Finished;
        delivery.FinishDate = DateTime.Now;
        await dbContext.SaveChangesAsync();
        var modelsResolved = await ResolveTemporaryModels(delivery);
        
        if (!modelsResolved) return null;
        return new DeliveryDto(delivery);
    }

    public async Task<DeliveryDto?> CancelDelivery(int deliveryId)
    {
        var delivery = await dbContext.Deliveries.FirstOrDefaultAsync(delivery => delivery.Id == deliveryId);
        if (delivery == null) return null;
        if (delivery.Status != (int)DeliveryStatus.Finished) return null;
        delivery.Status = (int)DeliveryStatus.Cancelled;
        delivery.FinishDate = DateTime.Now;
        await dbContext.SaveChangesAsync();
        return new DeliveryDto(delivery);
    }

    public async Task<bool> ResolveTemporaryModels(Delivery delivery)
    {
        throw new NotImplementedException();
    }
}