using ExpressVoitures.Models;

namespace ExpressVoitures.Repository.Interfaces
{
    public interface ICarRepository
    {
        Task AddCar(Car car);
        Task DeleteCar(int carId);
        Task UpdateCar(Car car);
        Task<List<Car>> GetAllCars();
        Task<Car?> GetCarById(int? carId);
        Task<CarBrand?> GetBrandByName(string brandName);
        Task<CarModel?> GetModelByName(string modelName);
        Task<CarBrandModel?> GetBrandModelId(int brandId, int modelId);
        Task AddBrand(CarBrand brand);
        Task AddModel(CarModel model);
        Task AddBrandModel(CarBrandModel brandModel);
        Task<List<CarBrand>> GetBrand();
        Task<List<CarModel>> GetModel();


    }
}
