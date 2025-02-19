using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.BikeApp.Services;

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
            return builder;
        }
    }
}
