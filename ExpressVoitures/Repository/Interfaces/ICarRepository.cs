using ExpressVoitures.Models;

namespace ExpressVoitures.Repository.Interfaces
{
    public interface ICarRepository
    {
        void AddCar(Car car);
        void DeleteCar(int carId);
        IEnumerable<Car> GetAllCars();
        Car GetCarById(int carId);
        void UpdateCar(Car car);
        void AddBrand(CarBrand brand);
        void AddModel(CarModel model);
        void AddBrandModel(CarBrandModel brandModel);
        CarBrandModel GetBrandModelId(int brandId, int modelId);
        CarBrand GetBrandByName(string brandName);
        CarModel GetModelByName(string modelName);



    }
}
