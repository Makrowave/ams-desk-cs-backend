using ams_desk_cs_backend.Deliveries.Dtos;
using ErrorOr;

namespace ams_desk_cs_backend.Deliveries.Interfaces;

public interface IInvoiceService
{
    Task<ErrorOr<InvoiceDto>> GetInvoice(int invoiceId);
    Task<ErrorOr<InvoiceDto[]>> GetInvoices();
    Task<ErrorOr<InvoiceDto>> CreateInvoice(NewInvoiceDto invoiceDto);
    Task<ErrorOr<InvoiceDto>> UpdateInvoice(InvoiceDto invoiceDto);
    Task<ErrorOr<Success>> DeleteInvoice(int invoiceId);
}