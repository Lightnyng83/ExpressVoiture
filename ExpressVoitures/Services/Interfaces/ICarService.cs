using ExpressVoitures.Models;
using ExpressVoitures.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExpressVoitures.Services.Interfaces
{
    public interface ICarService
    {
        Task AddCar(CarCreateViewModel carViewModel);
        Task DeleteCar(int carId);
        Task<List<Car>> GetAllCars();
        Task<Car?> GetCarById(int? carId);
        Task<IActionResult> UpdateCar(CarEditionViewModel car, int id);
        Task<CarBrandModel?> GetBrandModelId(int brandId, int modelId);
        Task<CarBrand?> GetBrandByName(string brandName);
        Task<CarModel?> GetModelByName(string modelName);
        Task AddBrand(CarBrand brand);
        Task AddModel(CarModel model);
        Task AddBrandModel(CarBrandModel brandModel);
        Task<List<CarBrand>> GetBrand();
        Task<List<CarModel>> GetModel();
    }
}
