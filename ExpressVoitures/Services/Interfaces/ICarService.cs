using ExpressVoitures.Models;
using ExpressVoitures.ViewModels;

namespace ExpressVoitures.Services.Interfaces
{
    public interface ICarService
    {
        Task AddCar(Car car);
        Task DeleteCar(int carId);
        Task<List<Car>> GetAllCars();
        Task<Car?> GetCarById(int? carId);
        Task UpdateCar(Car car);
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
