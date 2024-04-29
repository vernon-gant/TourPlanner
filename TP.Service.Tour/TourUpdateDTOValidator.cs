using FluentValidation;

namespace TP.Service.Tour;

using Tour = Domain.Tour;

public class TourUpdateDTOValidator : AbstractValidator<TourUpdateDTO>
{
    public TourUpdateDTOValidator()
    {
        RuleFor(tour => tour.Name).NotEmpty().WithMessage("Tour name is required.").MaximumLength(Tour.MaxNameLength).WithMessage($"Tour name must be maximum {Tour.MaxNameLength} characters");

        RuleFor(tour => tour.Description).NotEmpty().WithMessage("Tour description is required");
    }
}