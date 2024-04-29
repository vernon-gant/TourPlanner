using FluentValidation;

namespace TP.Service.Tour;

using Tour = Domain.Tour;

public class TourDTOValidator : AbstractValidator<TourDTO>
{
    public TourDTOValidator()
    {
        RuleFor(tour => tour.Name).NotEmpty().WithMessage("Tour name is required.").MaximumLength(Tour.MaxNameLength).WithMessage($"Tour name must be maximum {Tour.MaxNameLength} characters");

        RuleFor(tour => tour.Description).NotEmpty().WithMessage("Tour description is required");

        RuleFor(tour => tour.Start).NotEmpty().WithMessage("Start description is required")
            .MaximumLength(Tour.MaxPointDescriptionLength).WithMessage($"Start description cannot be more than {Tour.MaxPointDescriptionLength} characters.");

        RuleFor(tour => tour.End).NotEmpty().WithMessage("End description is required")
            .MaximumLength(Tour.MaxPointDescriptionLength).WithMessage($"End description cannot be more than {Tour.MaxPointDescriptionLength} characters.");
    }
}
