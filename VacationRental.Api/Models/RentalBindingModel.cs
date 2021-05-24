using FluentValidation;

namespace VacationRental.Api.Models
{
    public class RentalBindingModel
    {
        public int Units { get; set; }
        public int PreparationTimeInDays { get; set; }
    }
    
    public class RentalBindingModelValidator : AbstractValidator<RentalBindingModel>
    {
        public RentalBindingModelValidator()
        {
            RuleFor(r => r.Units).GreaterThan(0)
                .WithMessage($"{nameof(RentalBindingModel.Units)}_should_not_greater_than_zero");
            RuleFor(r => r.Units).GreaterThan(0)
                .WithMessage($"{nameof(RentalBindingModel.PreparationTimeInDays)}_should_not_greater_than_zero");
        }
    }
}
