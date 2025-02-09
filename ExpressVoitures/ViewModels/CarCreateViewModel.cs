using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpressVoitures.ViewModels
{
    public class CarCreateViewModel : IValidatableObject
    {
        [Required]
        public int SelectedCarBrandId { get; set; }

        [Required]
        public int SelectedCarBrandModelId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int SellingPrice { get; set; }
        [Required]
        public string? Finition { get; set; }
        public IFormFile? Image { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }

        public IEnumerable<SelectListItem>? BrandList { get; set; }
        public IEnumerable<SelectListItem>? ModelList { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            int currentYear = DateTime.Now.Year;
            if (Year < 1990 || Year > currentYear)
            {
                yield return new ValidationResult($"L'année doit être comprise entre 1990 et {currentYear}.", new[] { nameof(Year) });
            }

            if (SellingPrice <= 0)
            {
                yield return new ValidationResult("Le prix de vente doit être supérieur à 0.", new[] { nameof(SellingPrice) });
            }
        }
    }
}
