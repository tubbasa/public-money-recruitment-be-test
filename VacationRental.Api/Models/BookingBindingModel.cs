using System;
using FluentValidation;

namespace VacationRental.Api.Models
{
    public class BookingBindingModel
    {
        public int RentalId { get; set; }

        public DateTime Start
        {
            get => _startIgnoreTime;
            set => _startIgnoreTime = value.Date;
        }

        private DateTime _startIgnoreTime;
        public int Nights { get; set; }
    }

    public class BookingBindingModelValidator : AbstractValidator<BookingBindingModel>
    {
        public BookingBindingModelValidator()
        {
            RuleFor(r => r.Nights).GreaterThan(0)
                .WithMessage($"{nameof(BookingBindingModel.Nights)}_should_not_greater_than_zero");
            RuleFor(r => r.RentalId).GreaterThan(0)
                .WithMessage($"{nameof(BookingBindingModel.RentalId)}_should_not_greater_than_zero");
        }
    }
}