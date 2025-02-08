using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpressVoitures.ViewModels
{
    public class CarViewModel
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public IFormFile? Image { get; set; }
        public int SellingPrice { get; set; }
        public string Finition { get; set; }
        // Propriétés pour alimenter les listes déroulantes
        public int SelectedCarBrandId { get; set; }
        public int SelectedCarBrandModelId { get; set; }

    }
}
