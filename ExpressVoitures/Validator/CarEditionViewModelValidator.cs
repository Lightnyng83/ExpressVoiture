using FluentValidation;
using ExpressVoitures.ViewModels;

public class CarEditionViewModelValidator : BaseCarViewModelValidator<CarEditionViewModel>
{
    public CarEditionViewModelValidator() : base()
    {
        RuleFor(x => x.CarId)
            .NotEmpty()
            .WithMessage("L'identifiant de la voiture est requis.");
    }
}