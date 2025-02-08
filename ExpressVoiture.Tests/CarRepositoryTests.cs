using ExpressVoiture.Tests.Config;
using ExpressVoitures.Data;
using ExpressVoitures.Models;
using ExpressVoitures.Repository;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Tests
{
    public class CarRepositoryTests
    {
        // Méthode utilitaire pour obtenir un contexte en mémoire unique par test
        private TestApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TestApplicationDbContext(options);
        }




        [Fact]
        public void AddCar_ShouldAddCarToDatabase()
        {
            // Arrange
            var dbName = nameof(AddCar_ShouldAddCarToDatabase);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            // Création des entités liées nécessaires
            var brand = new CarBrand { CarBrandId = 1, CarBrandName = "TestBrand" };
            var model = new CarModel { CarModelId = 1, CarModelName = "TestModel" };
            context.CarBrands.Add(brand);
            context.CarModels.Add(model);
            var brandModel = new CarBrandModel
            {
                CarBrandModelId = 1,
                CarBrandId = brand.CarBrandId,
                CarModelId = model.CarModelId,
                CarBrand = brand,
                CarModel = model
            };
            context.CarBrandModels.Add(brandModel);
            context.SaveChanges();

            var car = new Car
            {
                CarId = 1,
                CarBrandModelId = brandModel.CarBrandModelId,
                Year = 2020,
                SellingPrice = 15000,
                ImageUrl = "test.png",
                CarBrandModel = brandModel
            };

            // Act
            repository.AddCar(car);

            // Assert
            var carFromDb = context.Cars.Find(1);
            Assert.NotNull(carFromDb);
            Assert.Equal(15000, carFromDb.SellingPrice);
        }

        [Fact]
        public void GetBrandModelId_ShouldReturnCorrectBrandModel()
        {
            // Arrange
            var dbName = nameof(GetBrandModelId_ShouldReturnCorrectBrandModel);
            using var context = GetDbContext(dbName);

            // Création des entités nécessaires
            var brand = new CarBrand { CarBrandId = 1, CarBrandName = "Brand1" };
            var model = new CarModel { CarModelId = 1, CarModelName = "Model1" };
            context.CarBrands.Add(brand);
            context.CarModels.Add(model);
            // Supposons ici que le contexte est configuré avec une clé composite (CarBrandId, CarModelId)
            var brandModel = new CarBrandModel
            {
                CarBrandModelId = 1, // ou bien, si la clé composite est configurée, cette propriété peut être ignorée
                CarBrandId = brand.CarBrandId,
                CarModelId = model.CarModelId,
                CarBrand = brand,
                CarModel = model
            };
            context.CarBrandModels.Add(brandModel);
            context.SaveChanges();

            var repository = new CarRepository(context);

            // Act
            var result = repository.GetBrandModelId(brand.CarBrandId, model.CarModelId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(brand.CarBrandId, result.Result.CarBrandId);
            Assert.Equal(model.CarModelId, result.Result.CarModelId);
        }

        [Fact]
        public void DeleteCar_ShouldRemoveCarFromDatabase()
        {
            // Arrange
            var dbName = nameof(DeleteCar_ShouldRemoveCarFromDatabase);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            // Création des entités liées
            var brand = new CarBrand { CarBrandId = 1, CarBrandName = "BrandDel" };
            var model = new CarModel { CarModelId = 1, CarModelName = "ModelDel" };
            context.CarBrands.Add(brand);
            context.CarModels.Add(model);
            var brandModel = new CarBrandModel
            {
                CarBrandModelId = 1,
                CarBrandId = brand.CarBrandId,
                CarModelId = model.CarModelId,
                CarBrand = brand,
                CarModel = model
            };
            context.CarBrandModels.Add(brandModel);
            context.SaveChanges();

            var car = new Car
            {
                CarId = 1,
                CarBrandModelId = brandModel.CarBrandModelId,
                Year = 2019,
                SellingPrice = 12000,
                ImageUrl = "delete.png",
                CarBrandModel = brandModel
            };
            context.Cars.Add(car);
            context.SaveChanges();

            // Act
            repository.DeleteCar(car.CarId);

            // Assert
            var deletedCar = context.Cars.Find(car.CarId);
            Assert.Null(deletedCar);
        }

        [Fact]
        public void GetAllCars_ShouldReturnAllCarsWithBrandModelIncluded()
        {
            // Arrange
            var dbName = nameof(GetAllCars_ShouldReturnAllCarsWithBrandModelIncluded);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            // Création des entités liées
            var brand = new CarBrand { CarBrandId = 1, CarBrandName = "BrandAll" };
            var model = new CarModel { CarModelId = 1, CarModelName = "ModelAll" };
            context.CarBrands.Add(brand);
            context.CarModels.Add(model);
            var brandModel = new CarBrandModel
            {
                CarBrandModelId = 1,
                CarBrandId = brand.CarBrandId,
                CarModelId = model.CarModelId,
                CarBrand = brand,
                CarModel = model
            };
            context.CarBrandModels.Add(brandModel);
            context.SaveChanges();

            var car1 = new Car
            {
                CarId = 1,
                CarBrandModelId = brandModel.CarBrandModelId,
                Year = 2018,
                SellingPrice = 10000,
                ImageUrl = "car1.png",
                CarBrandModel = brandModel
            };
            var car2 = new Car
            {
                CarId = 2,
                CarBrandModelId = brandModel.CarBrandModelId,
                Year = 2020,
                SellingPrice = 20000,
                ImageUrl = "car2.png",
                CarBrandModel = brandModel
            };
            context.Cars.AddRange(car1, car2);
            context.SaveChanges();

            // Act
            var cars = repository.GetAllCars().Result.ToList();

            // Assert
            Assert.Equal(2, cars.Count);
            Assert.NotNull(cars[0].CarBrandModel);
            Assert.NotNull(cars[1].CarBrandModel);
        }

        [Fact]
        public void GetBrandByName_ShouldReturnBrand_WhenExists()
        {
            // Arrange
            var dbName = nameof(GetBrandByName_ShouldReturnBrand_WhenExists);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            var brand = new CarBrand { CarBrandId = 1, CarBrandName = "ExistingBrand" };
            context.CarBrands.Add(brand);
            context.SaveChanges();

            // Act
            var result = repository.GetBrandByName("ExistingBrand");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ExistingBrand", result.Result.CarBrandName);
        }

        [Fact]
        public async Task GetBrandByName_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var dbName = nameof(GetBrandByName_ShouldReturnNull_WhenNotExists);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            // Act
            var result = await repository.GetBrandByName("NonExistentBrand");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetModelByName_ShouldReturnModel_WhenExists()
        {
            // Arrange
            var dbName = nameof(GetModelByName_ShouldReturnModel_WhenExists);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            var model = new CarModel { CarModelId = 1, CarModelName = "ExistingModel" };
            context.CarModels.Add(model);
            context.SaveChanges();

            // Act
            var result = repository.GetModelByName("ExistingModel");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ExistingModel", result.Result.CarModelName);
        }

        [Fact]
        public async Task GetModelByName_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var dbName = nameof(GetModelByName_ShouldReturnNull_WhenNotExists);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            // Act
            var result = await repository.GetModelByName("NonExistentModel");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetCarById_ShouldReturnCar_WhenExists()
        {
            // Arrange
            var dbName = nameof(GetCarById_ShouldReturnCar_WhenExists);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            // Création des entités liées
            var brand = new CarBrand { CarBrandId = 1, CarBrandName = "BrandGetCar" };
            var model = new CarModel { CarModelId = 1, CarModelName = "ModelGetCar" };
            context.CarBrands.Add(brand);
            context.CarModels.Add(model);
            var brandModel = new CarBrandModel
            {
                CarBrandModelId = 1,
                CarBrandId = brand.CarBrandId,
                CarModelId = model.CarModelId,
                CarBrand = brand,
                CarModel = model
            };
            context.CarBrandModels.Add(brandModel);
            context.SaveChanges();

            var car = new Car
            {
                CarId = 1,
                CarBrandModelId = brandModel.CarBrandModelId,
                Year = 2021,
                SellingPrice = 18000,
                ImageUrl = "getcar.png",
                CarBrandModel = brandModel
            };
            context.Cars.Add(car);
            context.SaveChanges();

            // Act
            var result = repository.GetCarById(car.CarId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2021, result.Result.Year);
        }

        [Fact]
        public void GetCarById_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var dbName = nameof(GetCarById_ShouldReturnNull_WhenNotExists);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            // Act
            var result = repository.GetCarById(999);

            // Assert
            Assert.Null(result.Result);
        }

        [Fact]
        public void UpdateCar_ShouldModifyCarDetails()
        {
            // Arrange
            var dbName = nameof(UpdateCar_ShouldModifyCarDetails);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            // Création des entités liées
            var brand = new CarBrand { CarBrandId = 1, CarBrandName = "BrandUpdate" };
            var model = new CarModel { CarModelId = 1, CarModelName = "ModelUpdate" };
            context.CarBrands.Add(brand);
            context.CarModels.Add(model);
            var brandModel = new CarBrandModel
            {
                CarBrandModelId = 1,
                CarBrandId = brand.CarBrandId,
                CarModelId = model.CarModelId,
                CarBrand = brand,
                CarModel = model
            };
            context.CarBrandModels.Add(brandModel);
            context.SaveChanges();

            var car = new Car
            {
                CarId = 1,
                CarBrandModelId = brandModel.CarBrandModelId,
                Year = 2017,
                SellingPrice = 11000,
                ImageUrl = "old.png",
                CarBrandModel = brandModel
            };
            context.Cars.Add(car);
            context.SaveChanges();

            // Act
            car.SellingPrice = 13000;
            car.ImageUrl = "updated.png";
            repository.UpdateCar(car);

            // Assert
            var updatedCar = context.Cars.Find(car.CarId);
            Assert.Equal(13000, updatedCar.SellingPrice);
            Assert.Equal("updated.png", updatedCar.ImageUrl);
        }

        [Fact]
        public void AddBrand_ShouldAddBrandToDatabase()
        {
            // Arrange
            var dbName = nameof(AddBrand_ShouldAddBrandToDatabase);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            var brand = new CarBrand { CarBrandId = 1, CarBrandName = "NewBrand" };

            // Act
            repository.AddBrand(brand);

            // Assert
            var brandFromDb = context.CarBrands.Find(brand.CarBrandId);
            Assert.NotNull(brandFromDb);
            Assert.Equal("NewBrand", brandFromDb.CarBrandName);
        }

        [Fact]
        public void AddModel_ShouldAddModelToDatabase()
        {
            // Arrange
            var dbName = nameof(AddModel_ShouldAddModelToDatabase);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            var model = new CarModel { CarModelId = 1, CarModelName = "NewModel" };

            // Act
            repository.AddModel(model);

            // Assert
            var modelFromDb = context.CarModels.Find(model.CarModelId);
            Assert.NotNull(modelFromDb);
            Assert.Equal("NewModel", modelFromDb.CarModelName);
        }

        [Fact]
        public void AddBrandModel_ShouldAddBrandModelToDatabase()
        {
            // Arrange
            var dbName = nameof(AddBrandModel_ShouldAddBrandModelToDatabase);
            using var context = GetDbContext(dbName);
            var repository = new CarRepository(context);

            // Création des entités liées
            var brand = new CarBrand { CarBrandId = 1, CarBrandName = "BrandForBrandModel" };
            var model = new CarModel { CarModelId = 1, CarModelName = "ModelForBrandModel" };
            context.CarBrands.Add(brand);
            context.CarModels.Add(model);
            context.SaveChanges();

            var brandModel = new CarBrandModel
            {
                CarBrandModelId = 1,
                CarBrandId = brand.CarBrandId,
                CarModelId = model.CarModelId,
                CarBrand = brand,
                CarModel = model
            };

            // Act
            repository.AddBrandModel(brandModel);

            // Assert
            var brandModelFromDb = context.CarBrandModels.Find(brandModel.CarBrandModelId);
            Assert.NotNull(brandModelFromDb);
            Assert.Equal(brand.CarBrandId, brandModelFromDb.CarBrandId);
            Assert.Equal(model.CarModelId, brandModelFromDb.CarModelId);
        }
    }
}
