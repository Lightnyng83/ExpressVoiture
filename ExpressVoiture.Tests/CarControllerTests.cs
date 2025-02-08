using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExpressVoitures.Models;
using ExpressVoitures.Services.Interfaces;
using ExpressVoitures.ViewModels;
using ExpressVoitures.Views.Admin; // ou le namespace exact de votre CarController
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Xunit;

namespace ExpressVoitures.Tests
{
    public class CarControllerTests
    {
        private readonly Mock<ICarService> _mockCarService;
        private readonly Mock<IWebHostEnvironment> _mockHostEnvironment;
        private readonly CarController _controller;

        public CarControllerTests()
        {
            _mockCarService = new Mock<ICarService>();
            _mockHostEnvironment = new Mock<IWebHostEnvironment>();
            // Pour la méthode SavePicture, on simule un chemin webroot
            _mockHostEnvironment.Setup(x => x.WebRootPath).Returns(Directory.GetCurrentDirectory());

            _controller = new CarController(_mockCarService.Object, _mockHostEnvironment.Object);
        }

        #region Index & Details

        [Fact]
        public async Task Index_ReturnsViewWithAllCars()
        {
            // Arrange
            var cars = new List<Car>
            {
                new Car { CarId = 1, Year = 2020, SellingPrice = 10000 }
            };
            _mockCarService.Setup(s => s.GetAllCars()).ReturnsAsync(cars);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(cars, viewResult.Model);
        }

        [Fact]
        public async Task Details_IdIsNull_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_CarNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockCarService.Setup(s => s.GetCarById(It.IsAny<int>())).ReturnsAsync((Car)null);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_CarFound_ReturnsViewWithCar()
        {
            // Arrange
            var car = new Car { CarId = 1, Year = 2020, SellingPrice = 10000 };
            _mockCarService.Setup(s => s.GetCarById(1)).ReturnsAsync(car);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(car, viewResult.Model);
        }

        #endregion

        #region Create

        [Fact]
        public async Task Create_Get_ReturnsViewWithViewModel()
        {
            // Arrange
            var brands = new List<CarBrand>
            {
                new CarBrand { CarBrandId = 1, CarBrandName = "Toyota" }
            };
            var models = new List<CarModel>
            {
                new CarModel { CarModelId = 1, CarModelName = "Corolla" }
            };
            _mockCarService.Setup(s => s.GetBrand()).ReturnsAsync(brands);
            _mockCarService.Setup(s => s.GetModel()).ReturnsAsync(models);

            // Act
            var result = await _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<CarCreateViewModel>(viewResult.Model);
            Assert.Single(viewModel.BrandList);
            Assert.Single(viewModel.ModelList);
        }

        [Fact]
        public async Task Create_Post_InvalidModelState_RedirectsToIndex()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "invalid");
            var carViewModel = new CarViewModel();
            // Act
            var result = await _controller.Create(carViewModel);
            // Ici, votre code redirige vers Index même si le modèle est invalide
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_ValidModel_CreatesCarAndRedirects()
        {
            // Arrange
            var carViewModel = new CarViewModel
            {
                Brand = "Toyota",
                Model = "Corolla",
                Year = 2020,
                SellingPrice = 15000,
                Image = null  // pour ce test, pas d'image
            };

            // Simuler que GetBrandByName et GetModelByName renvoient null lors du premier appel (marque/modèle à créer)
            _mockCarService.SetupSequence(s => s.GetBrandByName("Toyota"))
                           .ReturnsAsync((CarBrand)null)
                           .ReturnsAsync(new CarBrand { CarBrandId = 1, CarBrandName = "Toyota" });
            _mockCarService.SetupSequence(s => s.GetModelByName("Corolla"))
                           .ReturnsAsync((CarModel)null)
                           .ReturnsAsync(new CarModel { CarModelId = 1, CarModelName = "Corolla" });
            _mockCarService.Setup(s => s.AddBrand(It.IsAny<CarBrand>())).Returns(Task.CompletedTask);
            _mockCarService.Setup(s => s.AddModel(It.IsAny<CarModel>())).Returns(Task.CompletedTask);
            _mockCarService.Setup(s => s.GetBrandModelId(1, 1))
                           .ReturnsAsync(new CarBrandModel { CarBrandModelId = 1, CarBrandId = 1, CarModelId = 1 });
            _mockCarService.Setup(s => s.AddBrandModel(It.IsAny<CarBrandModel>())).Returns(Task.CompletedTask);
            _mockCarService.Setup(s => s.AddCar(It.IsAny<Car>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(carViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockCarService.Verify(s => s.AddCar(It.IsAny<Car>()), Times.Once);
        }

        #endregion

        #region Edit

        [Fact]
        public async Task Edit_Get_CarNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockCarService.Setup(s => s.GetCarById(It.IsAny<int>())).ReturnsAsync((Car)null);
            // Act
            var result = await _controller.Edit(1);
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_CarFound_ReturnsViewWithViewModel()
        {
            // Arrange
            var car = new Car
            {
                CarId = 1,
                Year = 2020,
                SellingPrice = 15000,
                CarBrandModel = new CarBrandModel
                {
                    CarBrandModelId = 1,
                    CarBrand = new CarBrand { CarBrandId = 1, CarBrandName = "Toyota" },
                    CarModel = new CarModel { CarModelId = 1, CarModelName = "Corolla" }
                }
            };
            _mockCarService.Setup(s => s.GetCarById(1)).ReturnsAsync(car);
            _mockCarService.Setup(s => s.GetBrand()).ReturnsAsync(new List<CarBrand>
            {
                new CarBrand { CarBrandId = 1, CarBrandName = "Toyota" }
            });
            _mockCarService.Setup(s => s.GetModel()).ReturnsAsync(new List<CarModel>
            {
                new CarModel { CarModelId = 1, CarModelName = "Corolla" }
            });

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<CarEditViewModel>(viewResult.Model);
            Assert.Equal(2020, viewModel.Year);
            Assert.Equal(15000, viewModel.SellingPrice);
            Assert.Equal("Toyota", viewModel.Brand);
            Assert.Equal("Corolla", viewModel.Model);
        }

        [Fact]
        public async Task Edit_Post_InvalidModelState_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "invalid");
            var carEditionViewModel = new CarEditionViewModel
            {
                Brand = "Toyota",
                Model = "Corolla",
                Year = 2020,
                SellingPrice = 15000,
                Image = null
            };
            // Act
            var result = await _controller.Edit(carEditionViewModel, 1);
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(carEditionViewModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_UpdatesCarAndRedirects()
        {
            // Arrange
            var existingCar = new Car
            {
                CarId = 1,
                Year = 2020,
                SellingPrice = 15000,
                ImageUrl = "old.jpg",
                CarBrandModel = new CarBrandModel
                {
                    CarBrandModelId = 1,
                    CarBrand = new CarBrand { CarBrandId = 1, CarBrandName = "Toyota" },
                    CarModel = new CarModel { CarModelId = 1, CarModelName = "Corolla" }
                }
            };
            _mockCarService.Setup(s => s.GetCarById(1)).ReturnsAsync(existingCar);
            // Pour ce test, aucun nouveau fichier n'est envoyé
            var carEditionViewModel = new CarEditionViewModel
            {
                Brand = "Toyota",
                Model = "Corolla",
                Year = 2021, // valeur mise à jour
                SellingPrice = 16000,
                Image = new FormFile(Stream.Null, 0, 0, null, null)
            };
            _mockCarService.SetupSequence(s => s.GetBrandByName("Toyota"))
                .ReturnsAsync(new CarBrand { CarBrandId = 1, CarBrandName = "Toyota" });
            _mockCarService.SetupSequence(s => s.GetModelByName("Corolla"))
                .ReturnsAsync(new CarModel { CarModelId = 1, CarModelName = "Corolla" });
            _mockCarService.Setup(s => s.GetBrandModelId(1, 1))
                .ReturnsAsync(new CarBrandModel { CarBrandModelId = 1, CarBrandId = 1, CarModelId = 1 });
            _mockCarService.Setup(s => s.UpdateCar(It.IsAny<Car>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(carEditionViewModel, 1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockCarService.Verify(s => s.UpdateCar(It.Is<Car>(c => c.Year == 2021 && c.SellingPrice == 16000)), Times.Once);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_Get_IdNull_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Delete(null);
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_CarNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockCarService.Setup(s => s.GetCarById(It.IsAny<int>())).ReturnsAsync((Car)null);
            // Act
            var result = await _controller.Delete(1);
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_CarFound_ReturnsViewWithCar()
        {
            // Arrange
            var car = new Car { CarId = 1 };
            _mockCarService.Setup(s => s.GetCarById(1)).ReturnsAsync(car);
            // Act
            var result = await _controller.Delete(1);
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(car, viewResult.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_DeletesCarAndRedirects()
        {
            // Arrange
            var car = new Car { CarId = 1 };
            _mockCarService.Setup(s => s.GetCarById(1)).ReturnsAsync(car);
            _mockCarService.Setup(s => s.DeleteCar(1)).Returns(Task.CompletedTask);
            // Act
            var result = await _controller.DeleteConfirmed(1);
            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockCarService.Verify(s => s.DeleteCar(1), Times.Once);
        }

        #endregion
    }
}
