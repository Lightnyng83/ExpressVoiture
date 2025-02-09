using ExpressVoitures.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpressVoitures.ViewModels
{
    public class CarEditViewModel
    {
        public int CarId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int SellingPrice { get; set; }

        public IFormFile? Image { get; set; }

        // Valeurs saisies par l'utilisateur (marque et modèle)
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Finition { get; set; }

        // Listes pour les suggestions
        public IEnumerable<SelectListItem> BrandList { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ModelList { get; set; } = Enumerable.Empty<SelectListItem>();
    }

}
