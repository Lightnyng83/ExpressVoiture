using ExpressVoitures.Data;
using ExpressVoitures.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Repository
{
    public class CarRepository : Interfaces.ICarRepository
    {
        private readonly ApplicationDbContext _context;

        public CarRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddCar(Car car)
        {
            _context.Cars.Add(car);
            _context.SaveChanges();
        }

        public CarBrandModel GetBrandModelId(int brandId, int modelId)
        {
            return _context.CarBrandModels.Find(brandId, modelId);
        }

        public void DeleteCar(int carId)
        {
            var car = _context.Cars.Find(carId);
            _context.Cars.Remove(car);
            _context.SaveChanges();
        }

        public IEnumerable<Car> GetAllCars()
        {
            return _context.Cars.Include(b =>b.CarBrandModel);
        }

        public CarBrand GetBrandByName (string brandName)
        {
            return _context.CarBrands.FirstOrDefault(b => b.CarBrandName == brandName);
        }

        public CarModel GetModelByName(string modelName)
        {
            return _context.CarModels.FirstOrDefault(m => m.CarModelName == modelName);
        }

        public Car GetCarById(int carId)
        {
            return _context.Cars.Find(carId);
        }

        public void UpdateCar(Car car)
        {
            _context.Cars.Update(car);
            _context.SaveChanges();
        }

        public void AddBrand(CarBrand brand)
        {
            _context.CarBrands.Add(brand);
            _context.SaveChanges();
        }

        public void AddModel(CarModel model)
        {
            _context.CarModels.Add(model);
            _context.SaveChanges();
        }

        public void AddBrandModel(CarBrandModel brandModel)
        {
            _context.CarBrandModels.Add(brandModel);
            _context.SaveChanges();
        }
    }
}
