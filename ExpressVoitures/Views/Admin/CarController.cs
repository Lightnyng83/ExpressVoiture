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
using Microsoft.AspNetCore.Mvc.ModelBinding;

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


        #region ----- Read -----

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

        #endregion

        #region ----- Create -----

        #region ----- Version 1 -----

        public async Task<IActionResult> Create()
        {
            // Récupérer la liste des marques et la liste des associations (table de jonction)
            var brands = await _carService.GetBrand(); // Retourne List<CarBrand>
            var brandModels = await _carService.GetModel(); // Retourne List<CarBrandModel>

            var viewModel = new CarCreateViewModel
            {
                // Préparer la liste déroulante des marques
                BrandList = brands.Select(b => new SelectListItem
                {
                    Value = b.CarBrandName,
                    Text = b.CarBrandName
                }),
                // Regrouper les modèles par marque en se basant sur la table de jonction
                ModelList = brandModels.Select(m => new SelectListItem
                {
                    Value = m.CarModelName,
                    Text = m.CarModelName
                })
            };

            return View(viewModel);
        }


        #endregion

        #region ----- Version 2 -----
        // GET: Car/Create
        //public async Task<IActionResult> Create()
        //{
        //    var brands = await _carService.GetBrand(); // Renvoie par exemple List<CarBrand>
        //    var models = await _carService.GetModel();   // Renvoie par exemple List<CarModel>

        //    var viewModel = new CarCreateViewModel
        //    {
        //        BrandList = brands.Select(b => new SelectListItem
        //        {
        //            Value = b.CarBrandName, 
        //            Text = b.CarBrandName
        //        }),
        //        ModelList = models.Select(m => new SelectListItem
        //        {
        //            Value = m.CarModelName,
        //            Text = m.CarModelName
        //        })
        //    };

        //    return View(viewModel);
        //}


        #endregion


        // POST: Car/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarCreateViewModel carViewModel)
        {
            if (ModelState.IsValid)
            {
                await _carService.AddCar(carViewModel);
                return RedirectToAction(nameof(Completed));
            }

            var brands = await _carService.GetBrand(); // Liste de CarBrand
            var models = await _carService.GetModel();   // Liste de CarModel

            var viewModel = new CarCreateViewModel
            {
                BrandList = brands.Select(b => new SelectListItem
                {
                    Value = b.CarBrandName,
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
        public IActionResult Completed()
        {
            return View();
        }

        #endregion

        #region ----- Edit ----- 

        // GET: Car/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var car = await _carService.GetCarById(id);
            if (car == null)
            {
                return NotFound();
            }

            var viewModel = new CarEditionViewModel
            {
                CarId = car.CarId,
                Year = car.Year,
                SellingPrice = car.SellingPrice,
                Brand = car.CarBrandModel.CarBrand.CarBrandName,
                Model = car.CarBrandModel.CarModel.CarModelName,
                Finition = car.Finition!,
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
                await _carService.UpdateCar(carViewModel, id).ConfigureAwait(false);
                
                
                return RedirectToAction(nameof(Index));
            }
            // Repopuler les listes déroulantes
            carViewModel.BrandList = (await _carService.GetBrand())
                .Select(b => new SelectListItem { Value = b.CarBrandName, Text = b.CarBrandName });
            carViewModel.ModelList = (await _carService.GetModel())
                .Select(m => new SelectListItem { Value = m.CarModelName, Text = m.CarModelName });

            return View(carViewModel);
        }

        #endregion

        #region ----- Delete -----

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
                await _carService.DeleteCar(id);
            }

            return RedirectToAction(nameof(Deleted), new { brand = car!.CarBrandModel.CarBrand.CarBrandName, model = car.CarBrandModel.CarModel.CarModelName, year = car.Year, finition = car.Finition });
        }

        public IActionResult Deleted(string brand, string model, int year, string finition)
        {
            var carViewModel = new CarViewModel
            {
                Brand = brand,
                Model = model,
                Year = year,
                Finition = finition
            };
            return View(carViewModel);
        }

        #endregion

        #region ----- PRIVATE METHODS -----


        /// <summary>
        /// Permet de valider les données du formulaire (Remplacer par FluentApi
        /// </summary>
        /// <param name="carViewModel">objet dynamique pour s'adapter a un Create ou un Edit</param>
        /// <returns></returns>
        //private async Task<IActionResult> ValidateForm(dynamic carViewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        if (carViewModel.BrandList == null && carViewModel.ModelList == null)
        //        {
        //            // Repopuler les listes déroulantes
        //            carViewModel.BrandList = (await _carService.GetBrand())
        //                .Select(b => new SelectListItem { Value = b.CarBrandName, Text = b.CarBrandName });
        //            carViewModel.ModelList = (await _carService.GetModel())
        //                .Select(m => new SelectListItem { Value = m.CarModelName, Text = m.CarModelName });
        //            ModelState.Clear();
        //            TryValidateModel(carViewModel);
        //        }

        //        if (carViewModel.Image == null)
        //        {
        //            carViewModel.Image = new FormFile(Stream.Null, 0, 0, null!, null!);
        //            ModelState.Clear();
        //            TryValidateModel(carViewModel);
        //        }
        //    }


        //    if (carViewModel.Year < 1990 || carViewModel.Year > (int)DateTime.Now.Year)
        //    {
        //        ModelState.AddModelError(nameof(carViewModel.Year), $"L'année doit être comprise entre 1990 et {(int)DateTime.Now.Year}");
        //    }
        //    if (carViewModel.SellingPrice <= 0)
        //    {
        //        ModelState.AddModelError(nameof(carViewModel.SellingPrice), "Vous devez saisir un prix de vente supérieur à 0");
        //    }
        //    if (carViewModel.SelectedCarBrandModelId == 0 && carViewModel.Model == null)
        //    {
        //        ModelState.AddModelError(nameof(carViewModel.Model), "Vous devez saisir ou sélectionner un modèle");
        //    }
        //    if (carViewModel.SelectedCarBrandId == 0 && carViewModel.Brand == null)
        //    {
        //        ModelState.AddModelError(nameof(carViewModel.Brand), "Vous devez saisir ou sélectionner une marque");
        //    }
        //    if (string.IsNullOrEmpty(carViewModel.Finition))
        //    {
        //        ModelState.Remove(nameof(carViewModel.Finition));
        //        ModelState.AddModelError(nameof(carViewModel.Finition), "Vous devez saisir une finition");
        //    }

        //    if (ModelState.ErrorCount > 0)
        //    {
        //        if (carViewModel is CarCreateViewModel createViewModel)
        //        {
        //            return View(createViewModel);
        //        }
        //        else if (carViewModel is CarEditionViewModel editionViewModel)
        //        {
        //            return View(editionViewModel);
        //        }

        //    }
        //    return Ok();
        //}

        #endregion
    }
}
