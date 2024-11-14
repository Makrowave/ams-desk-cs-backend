using ams_desk_cs_backend.BikeApp.Api.Dtos;
using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Application.Interfaces.Validators;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models;
using ams_desk_cs_backend.BikeApp.Infrastructure.Enums;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Xml.Linq;

namespace ams_desk_cs_backend.BikeApp.Application.Services
{
    public class ModelsService : IModelsService
    {
        private readonly BikesDbContext _context;
        private readonly IModelValidator _validator;
        public ModelsService(BikesDbContext context, IModelValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<ServiceResult> AddModel(ModelDto modelDto)
        {
            if(!_validator.ValidateModel(modelDto)) {
                return new ServiceResult(ServiceStatus.BadRequest, "Dane nie przeszły walidacji");
            }
            var model = new Model
            {
                ModelName = modelDto.ModelName!,
                ProductCode = modelDto.ProductCode,
                EanCode = modelDto.EanCode,
                FrameSize = modelDto.FrameSize!.Value,
                WheelSizeId = modelDto.WheelSize!.Value,
                ManufacturerId = modelDto.ManufacturerId!.Value,
                ColorId = modelDto.ColorId!.Value,
                CategoryId = modelDto.CategoryId!.Value,
                PrimaryColor = modelDto.PrimaryColor,
                SecondaryColor = modelDto.SecondaryColor,
                Price = modelDto.Price!.Value,
                IsElectric = modelDto.IsElectric!.Value,
                IsWoman = modelDto.IsWoman!.Value,
                InsertionDate = DateOnly.FromDateTime(DateTime.Today)
            };
            _context.Add(model);
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, String.Empty);
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

        public async Task<ServiceResult> UpdateModel(int id, ModelDto model)
        {
            var existingModel = await _context.Models.FindAsync(id);
            bool isValidated = true;
            if (existingModel == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono roweru");
            }
            //these 2 ommited for now - maybe should make IncompleteModelsService.cs or sth
            if (model.ProductCode != null)
            {
                if (!_validator.ValidateProductCode(model.ProductCode))
                    isValidated = false;
                existingModel.ProductCode = model.ProductCode;
            }
            if (model.EanCode != null)
            {
                if (!_validator.ValidateEanCode(model.EanCode))
                    isValidated = false;
                existingModel.EanCode = model.EanCode;
            }
            if (model.ModelName != null)
            {
                if (!_validator.ValidateModelName(model.ModelName))
                    isValidated = false;
                existingModel.ModelName = model.ModelName;
            }
            if (model.FrameSize.HasValue)
            {
                if (!_validator.ValidateFrameSize(model.FrameSize))
                    isValidated = false;
                existingModel.FrameSize = model.FrameSize.Value;
            }
            if (model.WheelSize.HasValue)
            {
                if (!_validator.ValidateWheelSize(model.WheelSize))
                    isValidated = false;
                existingModel.WheelSizeId = model.WheelSize.Value;
            }
            if (model.IsWoman.HasValue)
            {
                existingModel.IsWoman = model.IsWoman.Value;
            }
            if (model.ManufacturerId.HasValue)
            {
                if (!_context.Manufacturers.Any(m => m.ManufacturerId == model.ManufacturerId))
                    isValidated = false;
                existingModel.ManufacturerId = model.ManufacturerId.Value;
            }
            if (model.ColorId.HasValue)
            {
                if (!_context.Colors.Any(c => c.ColorId == model.ColorId))
                    isValidated = false;
                existingModel.ColorId = model.ColorId.Value;
            }
            if (model.CategoryId.HasValue)
            {
                if(!_context.Categories.Any(c => c.CategoryId == model.CategoryId))
                    isValidated = false;
                existingModel.CategoryId = model.CategoryId.Value;
            }
            if (model.PrimaryColor != null)
            {
                if (!_validator.ValidateColor(model.PrimaryColor))
                    isValidated = false;
                existingModel.PrimaryColor = model.PrimaryColor;
            }
            if (model.SecondaryColor != null)
            {
                if (!_validator.ValidateColor(model.SecondaryColor))
                    isValidated = false;
                existingModel.SecondaryColor = model.SecondaryColor;
            }
            if (model.Price.HasValue)
            {
                if (!_validator.ValidatePrice(model.Price))
                    isValidated = false;
                existingModel.Price = model.Price.Value;
            }
            if (model.IsElectric.HasValue)
            {
                existingModel.IsElectric = model.IsElectric.Value;
            }
            if (model.Link != null)
            {
                if (!_validator.ValidateLink(model.Link))
                    isValidated = false;
                existingModel.Link = model.Link;
            }
            if(!isValidated)
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Dane nie przeszły walidacji");
            }
            _context.SaveChanges();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }
    }
}
