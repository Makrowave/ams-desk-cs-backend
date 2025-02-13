using ams_desk_cs_backend.BikeApp.Dtos;
using ams_desk_cs_backend.BikeApp.Data;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Enums;
using ams_desk_cs_backend.BikeApp.Data.Models;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ams_desk_cs_backend.BikeApp.Services
{
    public class ModelsService : IModelsService
    {
        private readonly BikesDbContext _context;
        public ModelsService(BikesDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult> AddModel(ModelDto modelDto)
        {
            var model = new Model
            {
                ModelName = modelDto.ModelName,
                ProductCode = modelDto.ProductCode,
                EanCode = modelDto.EanCode,
                FrameSize = modelDto.FrameSize,
                WheelSizeId = modelDto.WheelSize,
                ManufacturerId = modelDto.ManufacturerId,
                ColorId = modelDto.ColorId,
                CategoryId = modelDto.CategoryId,
                PrimaryColor = modelDto.PrimaryColor,
                SecondaryColor = modelDto.SecondaryColor,
                Price = modelDto.Price,
                IsElectric = modelDto.IsElectric,
                IsWoman = modelDto.IsWoman,
                InsertionDate = DateOnly.FromDateTime(DateTime.Today)
            };
            _context.Add(model);
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult<IEnumerable<ModelRecordDto>>> GetModelRecords(ModelFilter filter)
        {
            var bikes = _context.Models
            .GroupJoin(
                    _context.Bikes.Where(bi => bi.StatusId != (short)BikeStatus.Sold &&
                        (filter.PlaceId == 0 || bi.PlaceId == filter.PlaceId)),
                    mo => mo.ModelId,
                    bi => bi.ModelId,
                    (mo, bi) => new { mo, bi }
                )
                .SelectMany(
                    r => r.bi.DefaultIfEmpty(),
                    (r, bi) => new { r.mo, bi }
                )
                .GroupBy(
                    r => new
                    {
                        r.mo.ModelId,
                        r.mo.ProductCode,
                        r.mo.EanCode,
                        r.mo.ModelName,
                        r.mo.FrameSize,
                        r.mo.WheelSizeId,
                        r.mo.ManufacturerId,
                        r.mo.Price,
                        r.mo.IsWoman,
                        r.mo.IsElectric,
                        r.mo.CategoryId,
                        r.mo.ColorId,
                        r.mo.PrimaryColor,
                        r.mo.SecondaryColor,
                        r.mo.Link,
                    }
                );
            //Should be always supplied in my case
            bikes = bikes.Where(
                g => g.Key.Price >= filter.MinPrice!.Value && g.Key.Price <= filter.MaxPrice!.Value
            );

            if (filter.CategoryId.HasValue)
            {
                bikes = bikes.Where(
                    g => g.Key.CategoryId == filter.CategoryId.Value
                );
            }
            if (!filter.ProductCode.IsNullOrEmpty())
            {
                bikes = bikes.Where(
                    g => g.Key.ProductCode != null && g.Key.ProductCode.Contains(filter.ProductCode!)
                );
            }
            if (filter.ColorId.HasValue)
            {
                bikes = bikes.Where(
                    g => g.Key.ColorId == filter.ColorId
                );
            }
            if (filter.Avaible.HasValue && filter.Avaible.Value)
            {
                bikes = bikes.Where(
                    g => g.Count(r => r.bi != null) > 0
                );
            }
            if (filter.IsWoman.HasValue)
            {
                bikes = bikes.Where(
                     g => g.Key.IsWoman == filter.IsWoman.Value
                );
            }
            if (filter.IsKids.HasValue && filter.IsKids.Value)
            {
                bikes = bikes.Where(
                    g => g.Key.WheelSizeId <= 24
                );
            }
            if (filter.StatusId.HasValue)
            {
                bikes = bikes.Where(
                    g => g.Count(r => r.bi != null && r.bi.StatusId == filter.StatusId) > 0
                );
            }
            //All bikes or only electric
            if (filter.Electric.HasValue && filter.Electric.Value)
            {
                bikes = bikes.Where(
                    g => g.Key.IsElectric == true
                );
            }
            if (filter.ManufacturerId.HasValue)
            {
                bikes = bikes.Where(
                    g => g.Key.ManufacturerId == filter.ManufacturerId.Value
                );
            }
            if (filter.WheelSize.HasValue)
            {
                bikes = bikes.Where(
                    g => g.Key.WheelSizeId == filter.WheelSize.Value
                );
            }
            if (filter.FrameSize.HasValue)
            {
                bikes = bikes.Where(
                    g => g.Key.FrameSize == filter.FrameSize.Value
                );
            }
            if (filter.NoColor.HasValue && filter.NoColor.Value)
            {
                bikes = bikes.Where(
                    g => g.Key.PrimaryColor == null || g.Key.SecondaryColor == null
                );
            }
            if (filter.NoColorGroup.HasValue && filter.NoColorGroup.Value)
            {
                bikes = bikes.Where(
                    g => g.Key.ColorId == null
                );
            }
            if (filter.NoEan.HasValue && filter.NoEan.Value)
            {
                bikes = bikes.Where(
                    g => g.Key.EanCode == null
                );
            }
            if (filter.NoProductCode.HasValue && filter.NoProductCode.Value)
            {
                bikes = bikes.Where(
                    g => g.Key.ProductCode == null
                );
            }
            if (filter.Name != null)
            {
                //Find just first word in name so query returns less for later step (don't really know if it's optimal)
                var words = filter.Name.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                bikes = bikes.Where(
                    g => g.Key.ModelName.ToLower().Contains(words[0])
                );
            }

            var result = await bikes.OrderBy(g => g.Key.ModelId)
                .Select(g => new ModelRecordDto
                {
                    ModelId = g.Key.ModelId,
                    ProductCode = g.Key.ProductCode,
                    EanCode = g.Key.EanCode,
                    ModelName = g.Key.ModelName,
                    FrameSize = g.Key.FrameSize,
                    WheelSize = g.Key.WheelSizeId,
                    ManufacturerId = g.Key.ManufacturerId,
                    Price = g.Key.Price,
                    IsWoman = g.Key.IsWoman,
                    IsElectric = g.Key.IsElectric,
                    PrimaryColor = g.Key.PrimaryColor,
                    SecondaryColor = g.Key.SecondaryColor,
                    CategoryId = g.Key.CategoryId,
                    ColorId = g.Key.ColorId,
                    Link = g.Key.Link,
                    BikeCount = g.Count(r => r.bi != null),
                    PlaceBikeCount = g.Where(r => r.bi != null)
                                        .GroupBy(r => new { r.bi!.PlaceId })
                                        .Select(d => new PlaceBikeCountDto
                                        {
                                            PlaceId = d.Key.PlaceId,
                                            Count = d.Count()
                                        })

                }).ToListAsync();
            //Checks for words from input string in model name in order but with gaps
            //For example input "Bike XL blue" will match with "Bike 4.0 XL blue" and "...Bike XL blue 4.0..."
            //but not with "Bike 4.0 blue XL"
            //And input "Bike XL XL" will not match with "Bike XL"
            if (filter.Name != null)
            {
                var words = filter.Name.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                result = result.FindAll(model =>
                {
                    string modelName = model.ModelName.ToLower();
                    foreach (var word in words)
                    {
                        if (!modelName.Contains(word))
                        {
                            return false;
                        }
                        //Shorten string so it won't match second occurence in input to first in model name
                        modelName = modelName.Substring(modelName.IndexOf(word) + word.Length);
                    }
                    return true;
                });
            }
            return new ServiceResult<IEnumerable<ModelRecordDto>>(ServiceStatus.Ok, string.Empty, result);
        }

        public async Task<ServiceResult> UpdateModel(int id, ModelDto newModel)
        {
            var oldModel = await _context.Models.FindAsync(id);
            if (oldModel == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono roweru");
            }

            oldModel.ProductCode = newModel.ProductCode;
            oldModel.EanCode = newModel.EanCode;
            oldModel.ModelName = newModel.ModelName;
            oldModel.FrameSize = newModel.FrameSize;
            oldModel.WheelSizeId = newModel.WheelSize;
            oldModel.IsWoman = newModel.IsWoman;
            oldModel.ManufacturerId = newModel.ManufacturerId;
            oldModel.ColorId = newModel.ColorId;
            oldModel.CategoryId = newModel.CategoryId;
            oldModel.PrimaryColor = newModel.PrimaryColor;
            oldModel.SecondaryColor = newModel.SecondaryColor;
            oldModel.Price = newModel.Price;
            oldModel.IsElectric = newModel.IsElectric;
            oldModel.Link = newModel.Link;

            _context.SaveChanges();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult> DeleteModel(int id)
        {
            try
            {
                var existingModel = await _context.Models.FindAsync(id);
                if (existingModel == null)
                {
                    return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono modelu");
                }
                _context.Models.Remove(existingModel);
                await _context.SaveChangesAsync();
                return new ServiceResult(ServiceStatus.Ok, string.Empty);
            }
            catch (Exception ex)
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Istnieją rowery przypisane to tego modelu");
            }
        }
    }
}
