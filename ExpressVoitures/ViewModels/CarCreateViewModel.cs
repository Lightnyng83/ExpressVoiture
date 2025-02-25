using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpressVoitures.ViewModels
{
    public class CarCreateViewModel : ICarViewModel
    {
        //[Required]
        //public int SelectedCarBrandId { get; set; }

        //[Required]
        //public int SelectedCarBrandModelId { get; set; }

        //[Required]
        //public int Year { get; set; }

        //[Required]
        //public int SellingPrice { get; set; }

        public string? Finition { get; set; }

        //public IFormFile? Image { get; set; }
        //public string? Brand { get; set; }
        //public string? Model { get; set; }

        public IEnumerable<SelectListItem>? BrandList { get; set; }
        public IEnumerable<SelectListItem>? ModelList { get; set; }

        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public int SellingPrice { get; set; }
        public IFormFile? Image { get; set; }

        // Liste de marques pour alimenter le datalist (si nécessaire)
        //public IEnumerable<SelectListItem> BrandList { get; set; } = Enumerable.Empty<SelectListItem>();

        // Groupe de modèles : la clé correspond à la marque, la valeur est la liste des modèles pour cette marque
        public Dictionary<string, IEnumerable<SelectListItem>> GroupedModelList { get; set; } = new Dictionary<string, IEnumerable<SelectListItem>>();
    }

}
