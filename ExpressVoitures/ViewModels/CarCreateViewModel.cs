using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ExpressVoitures.ViewModels
{
    public class CarCreateViewModel
    {
        // Propriétés pour stocker les valeurs sélectionnées
        [Required]
        public int SelectedCarBrandId { get; set; }

        [Required]
        public int SelectedCarBrandModelId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int SellingPrice { get; set; }
        public string Finition { get; set; }
        public IFormFile Image { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        // Propriétés pour alimenter les listes déroulantes
        public IEnumerable<SelectListItem> BrandList { get; set; }
        public IEnumerable<SelectListItem> ModelList { get; set; }
    }
}
