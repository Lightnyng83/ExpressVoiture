using ExpressVoitures.Models;

namespace ExpressVoitures.Services.Interfaces
{
    public interface IFrontService
    {
        Task<List<CarBrand>> GetBrand();
        Task<List<CarModel>> GetModel();
    }
}
