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

        public async Task<ServiceResult<ModelRecordDto>> AddModel(ModelDto modelDto)
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
            var result = new ModelRecordDto(model, 0, []);
            return new ServiceResult<ModelRecordDto>(ServiceStatus.Ok, string.Empty, result);
        }

        public async Task<ServiceResult<IEnumerable<ModelRecordDto>>> GetModelRecords(ModelFilter filter)
        {
            var models = _context.Models.Include(model => model.Bikes
                .Where(bi =>
                    bi.StatusId != (short)BikeStatus.Sold
                    && (filter.PlaceId == 0 || bi.PlaceId == filter.PlaceId)))
                .Where(
                    model => model.Price >= filter.MinPrice!.Value && model.Price <= filter.MaxPrice!.Value
                );

            if (filter.CategoryId.HasValue)
            {
                models = models.Where(
                    model => model.CategoryId == filter.CategoryId.Value
                );
            }
            if (!filter.ProductCode.IsNullOrEmpty())
            {
                models = models.Where(
                    model => model.ProductCode != null && model.ProductCode.Contains(filter.ProductCode!)
                );
            }
            if (filter.ColorId.HasValue)
            {
                models = models.Where(
                    model => model.ColorId == filter.ColorId
                );
            }
            if (filter.Avaible.HasValue && filter.Avaible.Value)
            {
                models = models.Where(
                    model => model.Bikes.Any()
                );
            }
            if (filter.IsWoman.HasValue)
            {
                models = models.Where(
                     model => model.IsWoman == filter.IsWoman.Value
                );
            }
            if (filter.IsKids.HasValue && filter.IsKids.Value)
            {
                models = models.Where(
                    model => model.WheelSizeId <= 24
                );
            }
            if (filter.StatusId.HasValue)
            {
                models = models.Where(
                    model => model.Bikes.Count(bike => bike.StatusId == filter.StatusId) > 0
                    
                // model => model.Bikes.Count(bike => bike != null && bike.StatusId == filter.StatusId) > 0
                );
            }
            //All bikes or only electric
            if (filter.Electric.HasValue && filter.Electric.Value)
            {
                models = models.Where(
                    model => model.IsElectric == true
                );
            }
            if (filter.ManufacturerId.HasValue)
            {
                models = models.Where(
                    model => model.ManufacturerId == filter.ManufacturerId.Value
                );
            }
            if (filter.WheelSize.HasValue)
            {
                models = models.Where(
                    model => model.WheelSizeId == filter.WheelSize.Value
                );
            }
            if (filter.FrameSize.HasValue)
            {
                models = models.Where(
                    model => model.FrameSize == filter.FrameSize.Value
                );
            }
            if (filter.NoColor.HasValue && filter.NoColor.Value)
            {
                models = models.Where(
                    model => model.PrimaryColor == null || model.SecondaryColor == null
                );
            }
            if (filter.NoColorGroup.HasValue && filter.NoColorGroup.Value)
            {
                models = models.Where(
                    model => model.ColorId == null
                );
            }
            if (filter.NoEan.HasValue && filter.NoEan.Value)
            {
                models = models.Where(
                    model => model.EanCode == null
                );
            }
            if (filter.NoProductCode.HasValue && filter.NoProductCode.Value)
            {
                models = models.Where(
                    model => model.ProductCode == null
                );
            }
            if (filter.Name != null)
            {
                //Find just first word in name so query returns less for later step (don't really know if it's optimal)
                var words = filter.Name.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                models = models.Where(
                    model => model.ModelName.ToLower().Contains(words[0])
                );
            }
            
            var places = await _context.Places.OrderBy(place => place.PlaceId).ToListAsync();
            var selectedModels = await models.ToListAsync();

            var result = selectedModels.OrderBy(model => model.ModelId)
                .Select(model =>
                {
                    var placeBikeCount = places.Select(
                            place => new PlaceBikeCountDto
                            {
                                PlaceId = place.PlaceId,
                                Count = model.Bikes.Count(bike => bike.PlaceId == place.PlaceId),
                                IsAvailable = model.Bikes.Any(bike => bike.PlaceId == place.PlaceId
                                                                      && bike.StatusId == (short)BikeStatus.Assembled),
                            }
                        )
                        .OrderBy(place => place.PlaceId)
                        .ToList();
                    return new ModelRecordDto(model, model.Bikes.Count(), placeBikeCount);
                }).ToList();
            result = result.Where(model => model.BikeCount != 0).ToList();
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

        public async Task<ServiceResult<ModelRecordDto>> UpdateModel(int id, ModelDto newModel)
        {
            var oldModel = await _context.Models.FindAsync(id);
            if (oldModel == null)
            {
                return ServiceResult<ModelRecordDto>.NotFound("Nie znaleziono roweru");
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

            await _context.SaveChangesAsync();
            var result = new ModelRecordDto(oldModel, 0, []);
            return new ServiceResult<ModelRecordDto>(ServiceStatus.Ok, string.Empty, result);
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

        public async Task<ServiceResult<bool>> SetFavorite(int id, bool favorite)
        {
            var model = await _context.Models.FindAsync(id);
            if (model == null)
            {
                return ServiceResult<bool>.NotFound("Nie znaleziono modelu");
            }
            model.Favorite = favorite;
            await _context.SaveChangesAsync();
            return new ServiceResult<bool>(ServiceStatus.Ok,string.Empty, model.Favorite);
        }

        public async Task<ServiceResult<IEnumerable<FavoriteModelDto>>> GetLowFavorites()
        {
            var result = await _context.Models
                .Where(model => model.Favorite)
                .Where(model => model.Bikes.Count <= 3)
                .OrderBy(model => model.Bikes.Count)
                .Include(model => model.Bikes.Where(bike => bike.StatusId != (short)BikeStatus.Sold))
                .Include(model => model.Manufacturer)
                .Select(model => new FavoriteModelDto(model))
                .ToListAsync();
            return new ServiceResult<IEnumerable<FavoriteModelDto>>(ServiceStatus.Ok, string.Empty, result);
        }
    }
}
