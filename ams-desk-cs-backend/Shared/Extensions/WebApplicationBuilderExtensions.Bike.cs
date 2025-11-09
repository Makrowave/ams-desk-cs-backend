using ams_desk_cs_backend.BikeFilters.Interfaces;
using ams_desk_cs_backend.BikeFilters.Services;
using ams_desk_cs_backend.Bikes.Interfaces;
using ams_desk_cs_backend.Bikes.Services;
using ams_desk_cs_backend.Deliveries.Interfaces;
using ams_desk_cs_backend.Deliveries.Services;
using ams_desk_cs_backend.Employees.Interfaces;
using ams_desk_cs_backend.Employees.Services;
using ams_desk_cs_backend.Models.Interfaces;
using ams_desk_cs_backend.Models.Services;
using ams_desk_cs_backend.Places.Interfaces;
using ams_desk_cs_backend.Places.Services;
using ams_desk_cs_backend.Repairs.Interfaces;
using ams_desk_cs_backend.Repairs.Services;

namespace ams_desk_cs_backend.Shared.Extensions
{
    public static partial class WEbApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddBikeServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IBikesService, BikesService>();
            builder.Services.AddScoped<IColorsService, ColorsService>();
            builder.Services.AddScoped<IStatusService, StatusService>();
            builder.Services.AddScoped<ICategoriesService, CategoriesService>();
            builder.Services.AddScoped<IEmployeesService, EmployeesService>();
            builder.Services.AddScoped<IManufacturersService, ManufacturersService>();
            builder.Services.AddScoped<IPlacesService, PlaceService>();
            builder.Services.AddScoped<IModelsService, ModelsService>();
            builder.Services.AddScoped<IWheelSizesService, WheelSizesService>();
            
            builder.Services.AddScoped<IRepairsService, RepairsService>();
            builder.Services.AddScoped<IServicesService, ServicesService>();
            builder.Services.AddScoped<IPartsService, PartsService>();
            builder.Services.AddScoped<IPartTypesService, PartTypesService>();
            builder.Services.AddScoped<IUnitsService, UnitsService>();

            builder.Services.AddScoped<IDeliveryService, DeliveryService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<ITemporaryModelService, TemporaryModelService>();
            builder.Services.AddScoped<IDeliveryItemService, DeliveryItemService>();
            
            return builder;
        }
    }
}
