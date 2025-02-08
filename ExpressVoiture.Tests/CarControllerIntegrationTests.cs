using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExpressVoitures.Data;
using ExpressVoitures.Models;
using ExpressVoitures.Repository.Interfaces;
using ExpressVoitures.Repository;
using ExpressVoitures.Services;
using ExpressVoitures.Services.Interfaces;
using ExpressVoitures.ViewModels;
using ExpressVoitures.Views.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ExpressVoiture.Tests.Config;
using Microsoft.AspNetCore.Http;

namespace ExpressVoitures.IntegrationTests
{
    public class CarControllerIntegrationTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ICarService _carService;
        private readonly CarController _controller;
        private readonly string _tempImagesFolder;

        public CarControllerIntegrationTests()
        {
            // Configurez le DbContext en mémoire
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TestApplicationDbContext(options);
            _context.Database.EnsureCreated();

            // On instancie le repository et le service réels.
            ICarRepository repository = new CarRepository(_context);
            _carService = new CarService(repository);

            // Utilisation d'un dossier temporaire pour simuler WebRootPath
            _tempImagesFolder = Path.Combine(Path.GetTempPath(), "ExpressVoituresTestImages");
            if (!Directory.Exists(_tempImagesFolder))
            {
                Directory.CreateDirectory(_tempImagesFolder);
            }
            // Créez une implémentation simulée de IWebHostEnvironment
            var hostEnvMock = new Moq.Mock<IWebHostEnvironment>();
            hostEnvMock.Setup(h => h.WebRootPath).Returns(_tempImagesFolder);

            _controller = new CarController(_carService, hostEnvMock.Object);
        }

        #region Méthode utilitaire pour insérer des entités valides

        /// <summary>
        /// Insère en base une marque, un modèle, l'association BrandModel et un véhicule valide.
        /// </summary>
        /// <param name="carId">ID du véhicule (par exemple 1).</param>
        /// <param name="year">Année du véhicule.</param>
        /// <param name="price">Prix de vente.</param>
        /// <param name="imageUrl">URL de l'image.</param>
        /// <returns>Le véhicule inséré.</returns>
        private async Task<Car> InsertValidCar(int carId, int year, int price, string imageUrl)
        {
            // Créez les entités nécessaires
            var brand = new CarBrand { CarBrandId = 1, CarBrandName = "Toyota" };
            var model = new CarModel { CarModelId = 1, CarModelName = "Corolla" };
            var brandModel = new CarBrandModel
            {
                CarBrandModelId = 1,
                CarBrandId = brand.CarBrandId,
                CarModelId = model.CarModelId,
                CarBrand = brand,
                CarModel = model
            };

            // Ajoute si non déjà présents dans le contexte
            if (!_context.CarBrands.Any(b => b.CarBrandId == brand.CarBrandId))
                _context.CarBrands.Add(brand);
            if (!_context.CarModels.Any(m => m.CarModelId == model.CarModelId))
                _context.CarModels.Add(model);
            if (!_context.CarBrandModels.Any(bm => bm.CarBrandModelId == brandModel.CarBrandModelId))
                _context.CarBrandModels.Add(brandModel);

            var car = new Car
            {
                CarId = carId,
                CarBrandModelId = brandModel.CarBrandModelId,
                Year = year,
                SellingPrice = price,
                ImageUrl = imageUrl,
                CarBrandModel = brandModel
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return car;
        }

        #endregion

        #region Index & Details

        [Fact]
        public async Task Index_ReturnsView_WithAllCars()
        {
            // Arrange : insérer un véhicule valide
            await InsertValidCar(1, 2020, 15000, "default.jpg");

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Car>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_NullId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_CarNotFound_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_CarFound_ReturnsViewWithCar()
        {
            // Arrange
            var insertedCar = await InsertValidCar(1, 2020, 15000, "default.jpg");

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var car = Assert.IsType<Car>(viewResult.Model);
            Assert.Equal(insertedCar.CarId, car.CarId);
        }

        #endregion

        #region Create

        [Fact]
        public async Task Create_Get_ReturnsViewWithCarCreateViewModel()
        {
            // Arrange : Insérer quelques marques et modèles dans le contexte
            _context.CarBrands.Add(new CarBrand { CarBrandId = 1, CarBrandName = "Toyota" });
            _context.CarModels.Add(new CarModel { CarModelId = 1, CarModelName = "Corolla" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<CarCreateViewModel>(viewResult.Model);
            Assert.Single(viewModel.BrandList);
            Assert.Single(viewModel.ModelList);
        }

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex_AndCreatesCar()
        {
            // Arrange : Insérer marque et modèle
            _context.CarBrands.Add(new CarBrand { CarBrandId = 1, CarBrandName = "Toyota" });
            _context.CarModels.Add(new CarModel { CarModelId = 1, CarModelName = "Corolla" });
            await _context.SaveChangesAsync();

            var carViewModel = new CarViewModel
            {
                Brand = "Toyota",
                Model = "Corolla",
                Year = 2020,
                SellingPrice = 15000,
                Image = null // Pas de fichier image pour ce test.
            };

            // Act
            var result = await _controller.Create(carViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Vérifier qu'une voiture a bien été insérée en base
            var carInDb = await _context.Cars.FirstOrDefaultAsync();
            Assert.NotNull(carInDb);
            Assert.Equal(2020, carInDb.Year);
        }

        #endregion

        #region Edit

        [Fact]
        public async Task Edit_Get_CarNotFound_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_CarFound_ReturnsViewWithCarEditViewModel()
        {
            // Arrange
            await InsertValidCar(1, 2020, 15000, "default.jpg");

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<CarEditViewModel>(viewResult.Model);
            Assert.Equal(2020, viewModel.Year);
            Assert.Equal("Toyota", viewModel.Brand);
            Assert.Equal("Corolla", viewModel.Model);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_RedirectsToIndex_AndUpdatesCar()
        {
            // Arrange
            await InsertValidCar(1, 2020, 15000, "old.jpg");

            var carEditionViewModel = new CarEditionViewModel
            {
                Brand = "Toyota",
                Model = "Corolla",
                Year = 2021,       // mise à jour
                SellingPrice = 16000,
                Image = new FormFile(Stream.Null, 0, 0, null, null)
            };

            // Act
            var result = await _controller.Edit(carEditionViewModel, 1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Vérifier la mise à jour en base
            var updatedCar = await _context.Cars.FindAsync(1);
            Assert.NotNull(updatedCar);
            Assert.Equal(2021, updatedCar.Year);
            Assert.Equal(16000, updatedCar.SellingPrice);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_Get_NullId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_CarNotFound_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Delete(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_CarFound_ReturnsViewWithCar()
        {
            // Arrange : insérer un véhicule valide
            var insertedCar = await InsertValidCar(1, 2020, 15000, "default.jpg");

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var car = Assert.IsType<Car>(viewResult.Model);
            Assert.Equal(insertedCar.CarId, car.CarId);
        }

        [Fact]
        public async Task Delete_Post_ValidId_RedirectsToIndex_AndDeletesCar()
        {
            // Arrange : insérer un véhicule valide
            await InsertValidCar(1, 2020, 15000, "default.jpg");

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            var carInDb = await _context.Cars.FindAsync(1);
            Assert.Null(carInDb);
        }

        #endregion

        public void Dispose()
        {
            // Supprimer la base en mémoire
            _context.Database.EnsureDeleted();
            _context.Dispose();

            // Supprimer le dossier temporaire s'il existe
            if (Directory.Exists(_tempImagesFolder))
            {
                Directory.Delete(_tempImagesFolder, true);
            }
        }
    }
}
