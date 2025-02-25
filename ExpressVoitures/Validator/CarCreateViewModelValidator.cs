using FluentValidation;
using ExpressVoitures.ViewModels;

public class CarCreateViewModelValidator : BaseCarViewModelValidator<CarCreateViewModel>
{
    public CarCreateViewModelValidator() : base()
    {
    }
}
