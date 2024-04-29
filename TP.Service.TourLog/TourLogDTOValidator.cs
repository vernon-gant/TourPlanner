using FluentValidation;

namespace TP.Service.TourLog;

using TourLog = Domain.TourLog;

public class TourLogDTOValidator : AbstractValidator<TourLogDTO>
{
    public TourLogDTOValidator()
    {
        RuleFor(tourLog => tourLog.UserName).NotEmpty().MaximumLength(TourLog.MaxUserNameLength).WithMessage($"User name must be maximum {TourLog.MaxUserNameLength} characters");

        RuleFor(tourLog => tourLog.Comment).NotEmpty();

        RuleFor(tourLog => tourLog.Difficulty).NotEmpty();

        RuleFor(tourLog => tourLog.TotalDistanceMeters).NotEmpty();

        RuleFor(tourLog => tourLog.TotalTime).NotEmpty();

        RuleFor(tourLog => tourLog.Rating).InclusiveBetween(TourLog.MinRating,TourLog.MaxRating);
    }
}