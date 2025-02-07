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
        private static readonly string adminName = "Jacques";
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

            var carBrand = dbContext.CarBrands.FirstOrDefault(x => x.CarBrandName == "Mazda");
            if (carBrand == null)
            {
                dbContext.CarBrands.Add(new CarBrand
                {
                    CarBrandName = "Mazda"
                });
                dbContext.SaveChanges();
            }

            var carModel = dbContext.CarModels.FirstOrDefault(x => x.CarModelName == "Miata");
            if (carModel == null)
            {
                dbContext.CarModels.Add(new CarModel
                {
                    CarModelName = "Miata"
                });
                dbContext.SaveChanges();
            }
            carBrand = dbContext.CarBrands.FirstOrDefault(x => x.CarBrandName == "Mazda");
            carModel = dbContext.CarModels.FirstOrDefault(x => x.CarModelName == "Miata");
            var carBrandModel = dbContext.CarBrandModelIds.FirstOrDefault(x => x.CarBrandId == carBrand.CarBrandId && x.CarModelId == carModel.CarModelId);
            if (carBrandModel == null)
            {
                dbContext.CarBrandModelIds.Add(new CarBrandModelId
                {
                    CarBrandId = carBrand.CarBrandId,
                    CarModelId = carModel.CarModelId
                });
                dbContext.SaveChanges();
            }

            var car = dbContext.Cars.FirstOrDefault(x => x.CarBrandModelId == carBrandModel.CarBrandModelId1);
            if (car == null) {
                dbContext.Cars.Add(new Car
                {
                    CarBrandModelId = carBrandModel.CarBrandModelId1,
                    Year = 2019,
                    SellingPrice = 9900,
                });
                dbContext.SaveChanges();
            }
            #endregion
        }
    }
}
