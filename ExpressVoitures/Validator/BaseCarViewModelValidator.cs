using FluentValidation;
using ExpressVoitures.ViewModels;
using System;

public abstract class BaseCarViewModelValidator<T> : AbstractValidator<T> where T : ICarViewModel
{
    protected BaseCarViewModelValidator()
    {
       RuleFor(x => x.Year)
            .InclusiveBetween(1990, DateTime.Now.Year)
            .WithMessage($"L'année doit être comprise entre 1990 et {DateTime.Now.Year}.");

        RuleFor(x => x.SellingPrice)
            .GreaterThan(0)
            .WithMessage("Le prix de vente doit être supérieur à 0.");

        RuleFor(x => x.Finition)
            .NotEmpty()
            .WithMessage("Vous devez saisir une finition.");

        RuleFor(x => x.Brand)
            .NotEmpty()
            .WithMessage("Vous devez saisir ou selectionner une marque");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Vous devez saisir ou selectionner une marque");
    }
}
