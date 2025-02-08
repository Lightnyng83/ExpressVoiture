using ExpressVoitures.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ExpressVoitures.ViewModels
{
    public class CarEditionViewModel
    {
        [Required]
        public int Year { get; set; }
        [Required]
        public int SellingPrice { get; set; }
        public IFormFile Image { get; set; } = new FormFile(Stream.Null, 0, 0, null, null);
        public string? Brand { get; set; }
        public string? Model { get; set; }
        // Propriétés pour alimenter les listes déroulantes
        public IEnumerable<SelectListItem> BrandList { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ModelList { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
