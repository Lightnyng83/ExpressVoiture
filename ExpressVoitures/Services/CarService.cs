using ExpressVoitures.Models;
using ExpressVoitures.ViewModels;

namespace ExpressVoitures.Services
{
    public class CarService
    {
        private readonly Repository.Interfaces.ICarRepository _carRepository;

        public CarService(Repository.Interfaces.ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public void AddCar(CarViewModel car)
        {
            var brand = _carRepository.GetAllCars().Where(x => x.CarBrandModel.CarBrand.CarBrandName==car.Brand);
            if (brand == null)
            {
                _carRepository.AddBrand(new Models.CarBrand { CarBrandName = car.Brand });
            }

            var model = _carRepository.GetAllCars().Where(x => x.CarBrandModel.CarModel.CarModelName == car.Model);
            if (model == null)
            {
                _carRepository.AddModel(new Models.CarModel { CarModelName = car.Model });
            }

            var brandModel = _carRepository.GetBrandModelId(_carRepository.GetBrandByName(car.Brand).CarBrandId, _carRepository.GetModelByName(car.Model).CarModelId);
            if (brandModel == null)
            {
                _carRepository.AddBrandModel(new Models.CarBrandModel { CarBrandId = _carRepository.GetBrandByName(car.Brand).CarBrandId, CarModelId = _carRepository.GetModelByName(car.Model).CarModelId });
            }

            var newCar = new Models.Car
            {
                CarBrandModelId = brandModel!.CarBrandModelId,
                Year = car.Year,
                SellingPrice = car.SellingPrice
            };

            _carRepository.AddCar(newCar);


        }
    }
}
