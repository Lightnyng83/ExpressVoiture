using ExpressVoitures.Models;

namespace ExpressVoitures.Services
{
    public class FrontService : Interfaces.IFrontService
    {
        private readonly Repository.Interfaces.ICarRepository _carRepository;

        public FrontService(Repository.Interfaces.ICarRepository carRepository)
        {
            _carRepository = carRepository;
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
