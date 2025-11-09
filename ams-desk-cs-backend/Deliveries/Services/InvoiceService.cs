using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ams_desk_cs_backend.Deliveries.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Deliveries.Services;

public class InvoiceService(BikesDbContext context): IInvoiceService
{
    public async Task<ErrorOr<InvoiceDto>> GetInvoice(int invoiceId)
    {
        var invoice = await context.Invoices.FirstOrDefaultAsync(invoice => invoice.Id == invoiceId);
        if (invoice == null) return Error.NotFound(description: "Nie znaleziono faktury");
        return new InvoiceDto(invoice);
    }

    public async Task<ErrorOr<InvoiceDto[]>> GetInvoices()
    {
        var invoices = await context.Invoices.ToListAsync();
        return invoices.Select(invoice => new InvoiceDto(invoice)).ToArray();
    }

    public async Task<ErrorOr<InvoiceDto>> CreateInvoice(NewInvoiceDto invoiceDto)
    {
        var invoice = new Invoice
        {
            InvoiceNumber = invoiceDto.InvoiceNumber,
            IssueDate = invoiceDto.IssueDate,
            PaymentDate = invoiceDto.PaymentDate,
            IssuerName = invoiceDto.IssuerName,
            IssuerAddress = invoiceDto.IssuerAddress,
            NettoAmount = invoiceDto.NettoAmount,
            BruttoAmount = invoiceDto.BruttoAmount,
        };
        
        await context.AddAsync(invoice);
        await context.SaveChangesAsync();
        
        return new InvoiceDto(invoice);
    }

    public async Task<ErrorOr<InvoiceDto>> UpdateInvoice(InvoiceDto invoiceDto)
    {
        var invoice = await context.Invoices.FirstOrDefaultAsync(invoice => invoice.Id == invoiceDto.Id);
        if (invoice == null) return Error.NotFound(description: "Nie znaleziono faktury");
        
        invoice.InvoiceNumber = invoiceDto.InvoiceNumber;
        invoice.IssueDate = invoiceDto.IssueDate;
        invoice.PaymentDate = invoiceDto.PaymentDate;
        invoice.IssuerName = invoiceDto.IssuerName;
        invoice.IssuerAddress = invoiceDto.IssuerAddress;
        invoice.NettoAmount = invoiceDto.NettoAmount;
        invoice.BruttoAmount = invoiceDto.BruttoAmount;
        invoice.DeliveryId = invoiceDto.DeliveryId;
        
        await context.SaveChangesAsync();
        
        return new InvoiceDto(invoice);
        
    }

    public async Task<ErrorOr<Success>> DeleteInvoice(int invoiceId)
    {
        var invoice = await context.Invoices.FirstOrDefaultAsync(invoice => invoice.Id == invoiceId);
        
        if(invoice == null) return Error.NotFound(description: "Nie znaleziono faktury");

        if (invoice.DeliveryId.HasValue) return Error.Validation("Nie można usunąć faktury powiązanej z dostawą");
        
        context.Remove(invoice);
        await context.SaveChangesAsync();
        
        return new Success();
    }
}