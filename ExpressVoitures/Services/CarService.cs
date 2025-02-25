using ExpressVoitures.Models;
using ExpressVoitures.Services.Interfaces;
using ExpressVoitures.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ExpressVoitures.Services
{
    public class CarService : ICarService
    {
        private readonly Repository.Interfaces.ICarRepository _carRepository;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CarService(Repository.Interfaces.ICarRepository carRepository, IWebHostEnvironment hostEnvironment)
        {
            _carRepository = carRepository;
            _hostEnvironment = hostEnvironment;
        }

        public async Task AddCar(CarCreateViewModel carViewModel)
        {
            string uniqueFileName = "";

            uniqueFileName = await SavePicture(carViewModel!.Image!);

            if (carViewModel.Brand != null && (await GetBrandByName(carViewModel.Brand) == null))
            {
                await AddBrand(new CarBrand { CarBrandName = carViewModel.Brand });
            }

            if (carViewModel.Model != null && await GetModelByName(carViewModel.Model) == null)
            {
                await AddModel(new CarModel { CarModelName = carViewModel.Model });
            }
            CarBrandModel? brandModel = await GetBrandModelId(GetBrandByName(carViewModel!.Brand!).Result!.CarBrandId, GetModelByName(carViewModel.Model!).Result!.CarModelId);
            if (brandModel == null)
            {
                await AddBrandModel(new CarBrandModel { CarBrandId = GetBrandByName(carViewModel.Brand!).Result!.CarBrandId, CarModelId = GetModelByName(carViewModel.Model!).Result!.CarModelId });
            }
            var newcar = new Car
            {
                CarBrandModelId = brandModel != null ? brandModel.CarBrandModelId : GetBrandModelId(GetBrandByName(carViewModel.Brand!).Result!.CarBrandId, GetModelByName(carViewModel.Model!).Result!.CarModelId).Result!.CarBrandModelId,
                Year = carViewModel.Year,
                SellingPrice = carViewModel.SellingPrice,
                ImageUrl = uniqueFileName, // Par exemple "GUID_NomFichier.ext"
                Finition = carViewModel.Finition
            };
            await _carRepository.AddCar(newcar);
        }
        public async Task DeleteCar(int carId)
        {
            var car = await GetCarById(carId);

            if (car.ImageUrl != null)
            {
                DeletePicture(car.ImageUrl);
                await _carRepository.DeleteCar(carId);
            }
        }

        public async Task<List<Car>> GetAllCars()
        {
            return await _carRepository.GetAllCars();
        }

        public async Task<Car?> GetCarById(int? carId)
        {
            return await _carRepository.GetCarById(carId);
        }
       
        // Inside the CarService class
        public async Task<IActionResult> UpdateCar(CarEditionViewModel carViewModel, int id) // Change return type to IActionResult
        {
            var existingCar = await GetCarById(id);
            string uniqueFileName = "default.jpg";
            if (carViewModel.Image != null && carViewModel.Image.Length > 0)
            {
                uniqueFileName = await SavePicture(carViewModel.Image);
            }
            else if (existingCar!.ImageUrl != uniqueFileName && carViewModel.Image!.Length == 0)
            {
                uniqueFileName = existingCar.ImageUrl!;
            }
            else
            {
                uniqueFileName = "default.jpg";
            }
            try
            {
                var updatecar = new Car
                {
                    CarId = id,
                    CarBrandModelId = GetBrandModelId(GetBrandByName(carViewModel.Brand!).Result!.CarBrandId!, GetModelByName(carViewModel.Model!).Result!.CarModelId).Result!.CarBrandModelId,
                    Year = carViewModel.Year,
                    SellingPrice = carViewModel.SellingPrice,
                    ImageUrl = uniqueFileName, // Par exemple "GUID_NomFichier.ext"
                    Finition = carViewModel.Finition
                };

                await _carRepository.UpdateCar(updatecar);
                return new OkResult(); // Return OK if update is successful
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
                {
                    return new NotFoundResult(); // Return NotFound if car does not exist
                }
                else
                {
                    throw;
                }
            }
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
        /// <summary>
        /// Permet d'ajouter l'image de l'utilisateur à une voiture, si l'utilisateur ne donne pas d'image, permet d'appliquer une image par défaut.
        /// </summary>
        /// <param name="formFile">Image sans type particulier pour le moment</param>
        /// <returns></returns>
        private async Task<string> SavePicture(IFormFile formFile)
        {
            string uniqueFileName = "default.jpg";

            if (formFile != null && formFile.Length > 0)
            {
                // Détermine le dossier wwwroot/images
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");

                // Assurez-vous que le dossier existe (sinon, créez-le)
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Générez un nom unique pour éviter les conflits
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(formFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(fileStream);
                }
                return uniqueFileName;
            }
            return uniqueFileName;
        }
        /// <summary>
        /// Permet de supprimer une image de la voiture à l'exception de l'image par défaut
        /// </summary>
        /// <param name="imageName">nom de l'image</param>
        /// <returns></returns>
        private void DeletePicture(string imageName)
        {
            const string uniqueFileName = "default.jpg";

            string imageUrl = Path.Combine(_hostEnvironment.WebRootPath, "images");
            string filePath = Path.Combine(imageUrl, imageName);
            if (imageName != uniqueFileName)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    fileStream.Close();
                    System.IO.File.Delete(filePath);
                }
            }
        }

        private bool CarExists(int id)
        {
            return GetAllCars().Result.Any(e => e.CarId == id);
        }

        
    }
}
