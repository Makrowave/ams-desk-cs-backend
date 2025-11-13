using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ams_desk_cs_backend.Deliveries.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Deliveries.Services;

public class DeliveryDocumentService(BikesDbContext dbContext) : IDeliveryDocumentService
{
    public async Task<ErrorOr<List<DeliveryDocumentDto>>> GetDeliveryDocumentsByDeliveryIdAsync(int deliveryId)
    {
        return await dbContext.DeliveryDocuments.Where(d => d.DeliveryId == deliveryId)
            .Select(doc => new DeliveryDocumentDto(doc)).ToListAsync();
    }

    public async Task<ErrorOr<DeliveryDocumentDto>> UpdateDeliveryDocumentAsync(DeliveryDocumentDto deliveryDocumentDto)
    {
        var deliveryDocument = await dbContext.DeliveryDocuments.Include(doc => doc.DeliveryItems)
            .FirstOrDefaultAsync(doc => doc.DeliveryId == deliveryDocumentDto.Id);

        if (deliveryDocument == null)
        {
            return Error.NotFound("Nie znaleziono dokumentu.");
        }

        deliveryDocument.Name = deliveryDocumentDto.Name;
        
        await dbContext.SaveChangesAsync();
        
        return new DeliveryDocumentDto(deliveryDocument);
        
    }

    public async Task<ErrorOr<DeliveryDocumentDto>> CreateDeliveryDocumentAsync(NewDeliveryDocumentDto deliveryDocumentDto)
    {
        var document = new DeliveryDocument
        {
            DeliveryId = deliveryDocumentDto.DeliveryId,
            Name = deliveryDocumentDto.Name,
            DocumentDate = DateTime.UtcNow
        };
        
        await dbContext.DeliveryDocuments.AddAsync(document);
        await dbContext.SaveChangesAsync();
        
        return new DeliveryDocumentDto(document);
    }

    public async Task<ErrorOr<Success>> DeleteDeliveryDocumentAsync(int id)
    {
        var deliveryDocument = await dbContext.DeliveryDocuments.Include(doc => doc.DeliveryItems)
            .FirstOrDefaultAsync(doc => doc.Id == id);

        if (deliveryDocument == null)
        {
            return Error.NotFound("Nie znaleziono dokumentu.");
        }

        if (deliveryDocument.DeliveryItems.Count != 0)
        {
            return Error.Validation("Nie można usunąć dokumentu z przypisanymi rowerami");
        }
        dbContext.DeliveryDocuments.Remove(deliveryDocument);
        await dbContext.SaveChangesAsync();
        return new Success();
    }
}