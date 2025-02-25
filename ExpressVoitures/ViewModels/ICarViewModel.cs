using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpressVoitures.ViewModels
{
    public interface ICarViewModel
    {
        //int SelectedCarBrandId { get; set; }
        //int SelectedCarBrandModelId { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        int Year { get; set; }
        int SellingPrice { get; set; }
        string? Finition { get; set; }

    }

}
