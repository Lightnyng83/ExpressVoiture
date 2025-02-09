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


        public async Task AddCar(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
        }

        public async Task<CarBrandModel?> GetBrandModelId(int brandId, int modelId)
        {
            return await _context.CarBrandModels.FirstOrDefaultAsync(b => b.CarBrandId == brandId && b.CarModelId == modelId);
        }

        public async Task DeleteCar(int carId)
        {
            var car = await _context.Cars.FindAsync(carId);
            _context.Cars.Remove(car!);
            _context.SaveChanges();
        }

        public async Task<List<Car>> GetAllCars()
        {
            return await _context.Cars.Include(b =>b.CarBrandModel.CarModel).Include(b => b.CarBrandModel.CarBrand).ToListAsync();
        }

        public async  Task<CarBrand?> GetBrandByName (string brandName)
        {
            return await _context.CarBrands.FirstOrDefaultAsync(b => b.CarBrandName == brandName);
        }

        public async Task<CarModel?> GetModelByName(string modelName)
        {
            return await _context.CarModels.FirstOrDefaultAsync(m => m.CarModelName == modelName);
        }

        public async Task<Car?> GetCarById(int? carId)
        {
            return await _context.Cars.Include(b => b.CarBrandModel.CarModel).Include(b => b.CarBrandModel.CarBrand).FirstOrDefaultAsync(x => x.CarId==carId);
        }

        public async Task UpdateCar(Car car)
        {
            // Vérifier si le contexte suit déjà une instance avec le même identifiant
            var local = _context.Cars.Local.FirstOrDefault(c => c.CarId == car.CarId);
            if (local != null)
            {
                // Détacher l'instance locale
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }


        public async Task AddBrand(CarBrand brand)
        {
            _context.CarBrands.Add(brand);
            await _context.SaveChangesAsync();
        }

        public async Task AddModel(CarModel model)
        {
            _context.CarModels.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddBrandModel(CarBrandModel brandModel)
        {
            _context.CarBrandModels.Add(brandModel);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CarBrand>> GetBrand()
        {
            return await _context.CarBrands.ToListAsync();
        }

        public async Task<List<CarModel>> GetModel()
        {
            return await _context.CarModels.ToListAsync();
        }
    }
}
