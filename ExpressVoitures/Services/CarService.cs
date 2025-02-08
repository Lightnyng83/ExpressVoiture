using ExpressVoitures.Models;
using ExpressVoitures.ViewModels;
using Microsoft.Extensions.Hosting;

namespace ExpressVoitures.Services
{
    public class CarService : Interfaces.ICarService
    {
        private readonly Repository.Interfaces.ICarRepository _carRepository;

        public CarService(Repository.Interfaces.ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task AddCar(Car car)
        {   
            await _carRepository.AddCar(car);
        }

        public async Task DeleteCar(int carId)
        {
            await _carRepository.DeleteCar(carId);
        }

        public async Task<List<Car>> GetAllCars()
        {
            return await _carRepository.GetAllCars();
        }

        public async Task<Car?> GetCarById(int? carId)
        {
            return await _carRepository.GetCarById(carId);
        }

        public async Task UpdateCar(Car car)
        {
            await _carRepository.UpdateCar(car);
        }

        public async Task<CarBrandModel?> GetBrandModelId(int brandId, int modelId)
        {
            return await _carRepository.GetBrandModelId(brandId, modelId);
        }

        public async Task<CarBrand?> GetBrandByName(string brandName)
        {
            return await _carRepository.GetBrandByName(brandName);
        }

        public async Task<CarModel?> GetModelByName(string modelName)
        {
            return await _carRepository.GetModelByName(modelName);
        }

        public async Task AddBrand(CarBrand brand)
        {
            await _carRepository.AddBrand(brand);
        }

        public async Task AddModel(CarModel model)
        {
            await _carRepository.AddModel(model);
        }

        public async Task AddBrandModel(CarBrandModel brandModel)
        {
            await _carRepository.AddBrandModel(brandModel);
        }

        public async Task<List<CarBrand>> GetBrand()
        {
            return await _carRepository.GetBrand();
        }

        public async Task<List<CarModel>> GetModel()
        {
            return await _carRepository.GetModel();
        }

    }
}
