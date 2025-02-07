namespace ExpressVoitures.ViewModels
{
    public class CarViewModel
    {
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public required int Year { get; set; }
        public string ImageUrl { get; set; }
        public required int SellingPrice { get; set; }

    }
}
