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
            .Include(delivery => delivery.DeliveryItems)
            .FirstOrDefaultAsync(delivery => delivery.Id == deliveryId);
        return result == null ? null : new DeliveryDto(result);
    }

    public async Task<DeliveryDto?> UpdateDelivery(DeliveryDto deliveryDto)
    {
        var delivery = await dbContext.Deliveries.FirstOrDefaultAsync(delivery => delivery.Id == deliveryDto.Id);
        if (delivery == null) return null;
        delivery.DeliveryDocument = deliveryDto.DeliveryDocument;
        delivery.Place = deliveryDto.Place;
        delivery.StatusId = (int)deliveryDto.StatusId;
        delivery.InvoiceNumber = deliveryDto.InvoiceNumber;
        delivery.PlaceId = deliveryDto.PlaceId;
        await dbContext.SaveChangesAsync();
        return new DeliveryDto(delivery);
    }

    public async Task<DeliveryDto> AddDelivery(NewDeliveryDto deliveryDto)
    {
        var delivery = new Delivery()
        {
            InvoiceNumber = deliveryDto.InvoiceNumber,
            PlaceId = deliveryDto.PlaceId
        };
        dbContext.Add(delivery);
        await dbContext.SaveChangesAsync();
        return new DeliveryDto(delivery);
    }

    public async Task<DeliveryDto?> StartDelivery(int deliveryId)
    {
        var delivery = await dbContext.Deliveries.FirstOrDefaultAsync(delivery => delivery.Id == deliveryId);
        if (delivery == null) return null;
        if(delivery.StatusId != (int)DeliveryStatus.Pending) {}
    }

    public async Task<DeliveryDto> FinishDelivery(int deliveryId)
    {
        throw new NotImplementedException();
    }

    public async Task<DeliveryDto> CancelDelivery(int deliveryId)
    {
        throw new NotImplementedException();
    }

    public async Task ResolveTemporaryModels(int deliveryId)
    {
        throw new NotImplementedException();
    }
}