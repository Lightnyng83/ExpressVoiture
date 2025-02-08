using ExpressVoitures.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Data
{
    public class SeedData
    {
        private static readonly IPasswordHasher<IdentityUser> _passwordHasher = new PasswordHasher<IdentityUser>();
        private static readonly string adminEmail = "admin@example.com";
        private static readonly string adminPassword = "Admin@123";
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            #region ----- INITIALIZATION -----
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();

            #endregion ----- INITIALIZATION -----

            #region ----- ADDING USERS -----

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();



            if (await userManager.FindByEmailAsync("admin@example.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };
                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    Console.WriteLine("Admin user created successfully.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }

            #endregion ----- ADDING USERS -----

            #region ----- ADDING CAR DATA -----

            // Liste des voitures issues du fichier Excel
            var cars = new List<(string brand, string model, int year, int price)>
    {
        ("Mazda", "Miata", 2019, 9900),
        ("Jeep", "Liberty", 2007, 5350),
        ("Renault", "Scénic", 2007, 2990),
        ("Ford", "Explorer", 2017, 25950),
        ("Honda", "Civic", 2008, 4975)
    };

            foreach (var (brand, model, year, price) in cars)
            {

                var carBrand = dbContext.CarBrands.FirstOrDefault(x => x.CarBrandName == brand);
                if (carBrand == null)
                {
                    carBrand = new CarBrand { CarBrandName = brand };
                    dbContext.CarBrands.Add(carBrand);
                    dbContext.SaveChanges();
                }

                var carModel = dbContext.CarModels.FirstOrDefault(x => x.CarModelName == model);
                if (carModel == null)
                {
                    carModel = new CarModel { CarModelName = model };
                    dbContext.CarModels.Add(carModel);
                    dbContext.SaveChanges();
                }

                var carBrandModel = dbContext.CarBrandModels.FirstOrDefault(x => x.CarBrandId == carBrand.CarBrandId && x.CarModelId == carModel.CarModelId);
                if (carBrandModel == null)
                {
                    carBrandModel = new CarBrandModel { CarBrandId = carBrand.CarBrandId, CarModelId = carModel.CarModelId };
                    dbContext.CarBrandModels.Add(carBrandModel);
                    dbContext.SaveChanges();
                }

                var car = dbContext.Cars.FirstOrDefault(x => x.CarBrandModelId == carBrandModel.CarBrandModelId && x.Year == year);
                if (car == null)
                {
                    dbContext.Cars.Add(new Car
                    {
                        CarBrandModelId = carBrandModel.CarBrandModelId,
                        Year = year,
                        SellingPrice = price,
                        ImageUrl = "default.jpg"
                    });
                    dbContext.SaveChanges();
                }

            }

            #endregion ----- ADDING CAR DATA -----
        }

    }
}
