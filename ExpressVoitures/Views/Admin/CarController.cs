using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpressVoitures.Data;
using ExpressVoitures.Models;
using ExpressVoitures.Services.Interfaces;
using ExpressVoitures.ViewModels;
using System.Runtime.ConstrainedExecution;

namespace ExpressVoitures.Views.Admin
{
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CarController(ICarService carService, IWebHostEnvironment hostEnvironment)
        {
            _carService = carService;
            _hostEnvironment = hostEnvironment;
        }


        //GET: Car
        public async Task<IActionResult> Index()
        {
            return View(await _carService.GetAllCars());
        }

        // GET: Car/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _carService.GetCarById(id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Car/Create
        public async Task<IActionResult> Create()
        {
            var brands = await _carService.GetBrand(); // Renvoie par exemple List<CarBrand>
            var models = await _carService.GetModel();   // Renvoie par exemple List<CarModel>

            var viewModel = new CarCreateViewModel
            {
                BrandList = brands.Select(b => new SelectListItem
                {
                    Value = b.CarBrandName, // Utilisez ici le nom, car c'est ce qui sera affiché et comparé
                    Text = b.CarBrandName
                }),
                ModelList = models.Select(m => new SelectListItem
                {
                    Value = m.CarModelName,
                    Text = m.CarModelName
                })
            };

            return View(viewModel);
        }


        // POST: Car/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarViewModel carViewModel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = "";

                switch (carViewModel.Image)
                {
                    case not null:
                        uniqueFileName = await SavePicture(carViewModel.Image);
                        break;
                    default:
                        break;
                }
                
                if (carViewModel.Brand != null &&( await _carService.GetBrandByName(carViewModel.Brand) == null))
                {
                        await _carService.AddBrand(new CarBrand { CarBrandName = carViewModel.Brand });
                }

                if (carViewModel.Model != null && await _carService.GetModelByName(carViewModel.Model) == null)
                {
                    await _carService.AddModel(new CarModel { CarModelName = carViewModel.Model });
                }

                CarBrandModel? brandModel = await _carService.GetBrandModelId(_carService.GetBrandByName(carViewModel.Brand).Result.CarBrandId, _carService.GetModelByName(carViewModel.Model).Result.CarModelId);
                if (brandModel == null)
                {
                    await _carService.AddBrandModel(new CarBrandModel { CarBrandId = _carService.GetBrandByName(carViewModel.Brand).Result.CarBrandId, CarModelId = _carService.GetModelByName(carViewModel.Model).Result.CarModelId });
                }
                    
                var newcar = new Car
                {
                    CarBrandModelId = brandModel != null ? brandModel.CarBrandModelId : _carService.GetBrandModelId(_carService.GetBrandByName(carViewModel.Brand).Result.CarBrandId, _carService.GetModelByName(carViewModel.Model).Result.CarModelId).Result.CarBrandModelId,
                    Year = carViewModel.Year,
                    SellingPrice = carViewModel.SellingPrice,
                    ImageUrl = uniqueFileName, // Par exemple "GUID_NomFichier.ext"
                    Finition = carViewModel.Finition
                };
                await _carService.AddCar(newcar);                    
                
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Car/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var car = await _carService.GetCarById(id);
            if (car == null)
            {
                return NotFound();
            }

            var viewModel = new CarEditViewModel
            {
                CarId = car.CarId,
                Year = car.Year,
                SellingPrice = car.SellingPrice,
                Brand = car.CarBrandModel.CarBrand.CarBrandName,
                Model = car.CarBrandModel.CarModel.CarModelName,
                BrandList = (await _carService.GetBrand()).Select(b => new SelectListItem
                {
                    Value = b.CarBrandName,
                    Text = b.CarBrandName
                }),
                ModelList = (await _carService.GetModel()).Select(m => new SelectListItem
                {
                    Value = m.CarModelName,
                    Text = m.CarModelName
                })
            };

            return View(viewModel);
        }


        // POST: Car/Edit/5
        //To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] CarEditionViewModel carViewModel, int id)
        {
            if (ModelState.IsValid)
            {
                var existingCar = await _carService.GetCarById(id);
                string uniqueFileName = "default.jpg";
                if(carViewModel.Image != null && carViewModel.Image.Length>0)
                {
                    uniqueFileName = await SavePicture(carViewModel.Image);
                }
                else if (existingCar.ImageUrl != uniqueFileName && carViewModel.Image.Length == 0)
                {
                    uniqueFileName = existingCar.ImageUrl;
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
                        CarBrandModelId = _carService.GetBrandModelId(_carService.GetBrandByName(carViewModel.Brand).Result.CarBrandId, _carService.GetModelByName(carViewModel.Model).Result.CarModelId).Result.CarBrandModelId,
                        Year = carViewModel.Year,
                        SellingPrice = carViewModel.SellingPrice,
                        ImageUrl = uniqueFileName // Par exemple "GUID_NomFichier.ext"

                    };

                    await _carService.UpdateCar(updatecar).ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(carViewModel);
        }

        // GET: Car/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _carService.GetCarById(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Car/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _carService.GetCarById(id);
            if (car != null)
            {
                if(car.ImageUrl!=null)
                { 
                    await DeletePicture(car.ImageUrl); 
                }

                await _carService.DeleteCar(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _carService.GetAllCars().Result.Any(e => e.CarId == id);
        }
        private async Task<string> SavePicture (IFormFile formFile)
        {
            string uniqueFileName = "";

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

        private  async Task DeletePicture(string url)
        {
            const string uniqueFileName = "default.jpg";

            string imageUrl = Path.Combine(_hostEnvironment.WebRootPath, "images");
            string filePath = Path.Combine(imageUrl, url);
            if (url != uniqueFileName)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    fileStream.Close();
                    System.IO.File.Delete(filePath);
                }
            }
        }
    } 
}
