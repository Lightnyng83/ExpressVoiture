using ExpressVoitures.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ExpressVoitures.ViewModels
{
    public class CarEditionViewModel : ICarViewModel
    {
        public int CarId { get; set; }

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
        //public Dictionary<string, IEnumerable<SelectListItem>> GroupedModelList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

}
