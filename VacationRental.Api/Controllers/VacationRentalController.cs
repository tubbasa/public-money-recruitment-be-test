using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/VacationRental")]
    [ApiController]
    public class VacationRentalController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rental;
        private readonly IDictionary<int, BookingViewModel> _bookings;
        private readonly IDictionary<int, CalendarDateViewModel> _calendarDate;

        public VacationRentalController(IDictionary<int, RentalViewModel> rental,IDictionary<int, BookingViewModel> bookings)
        {
            _rental = rental;
            _bookings = bookings;
        }

        [HttpGet]
        [Route("calendar")]
        public List<CalendarDateViewModel> Get()
        {
            if (_rental.Count==0)
                throw new ApplicationException("There is no data in CalendarDate.");
            List<CalendarDateViewModel> viewModel =  new List<CalendarDateViewModel>();
            #region PreparingCalendarViewModel
            foreach (var booking in _bookings)
            {
                for (int i = 0; i < booking.Value.Nights; i++)
                {
                    if(i==0)
                    {
                        CalendarDateViewModel calendar = new CalendarDateViewModel();
                        calendar.Date = booking.Value.Start;
                        calendar.Bookings = new List<CalendarBookingViewModel>(){new CalendarBookingViewModel{Id= booking.Value.Id,Unit = 1}};   //Unit line always be 1 because: 'One booking always occupies only one unit.'
                        calendar.PreparationTimes = new List<PreparationTimes>();
                        viewModel.Add(calendar);
                    }
                    else
                    {
                        CalendarDateViewModel calendar = new CalendarDateViewModel();
                        calendar.Date = booking.Value.Start.Date.AddDays(i);
                        calendar.Bookings = new List<CalendarBookingViewModel>(){new CalendarBookingViewModel{Id= booking.Value.Id,Unit = 1}};   //Unit line always be 1 because: 'One booking always occupies only one unit.'
                        calendar.PreparationTimes = new List<PreparationTimes>();
                        viewModel.Add(calendar);
                    }
                }

                var rentsPreparationTime = _rental.FirstOrDefault(rent => rent.Value.Id == booking.Value.RentalId).Value
                    .PreparationTimeInDays;
                for (int i = 0; i < rentsPreparationTime; i++)
                {
                    CalendarDateViewModel calendar = new CalendarDateViewModel();
                    calendar.Date = (booking.Value.Start.Date.AddDays(booking.Value.Nights)).Date.AddDays(i);
                    calendar.Bookings = new List<CalendarBookingViewModel>();   //Unit line always be 1 because: 'One booking always occupies only one unit.'
                    calendar.PreparationTimes= new List<PreparationTimes>(){new PreparationTimes(){Unit = 1}}; //It will always be 1 because: 'One booking always occupies only one unit.' and 'PreparationTime occupies the same Unit number as the Booking'
                    viewModel.Add(calendar);

                }
            }
            #endregion
            return viewModel;
        }

        [HttpPost]
        [Route("rentals")]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            var key = new ResourceIdViewModel { Id = _rental.Keys.Count + 1 };

            _rental.Add(key.Id, new RentalViewModel
            {
                Id = key.Id,
                PreparationTimeInDays = model.PreparationTimeInDays,
                Units = model.Units
                
            });

            return key;
        }
        
        [HttpPut]
        [Route("rentals/{id}")]
        public RentalViewModel Put(RentalBindingModel model,[FromRoute] int id )
        {
            var existingRental = _rental.FirstOrDefault(x => x.Value.Id == id);
             if (existingRental.Equals(null))
             {
                 throw new ApplicationException("There is no rental belongs to this Id.");
             }

             var bookingsBelongTheRental = _bookings.Where(book => book.Value.RentalId == id);  //The bookings belongs to given rental id
             
             var oldPreparationTime = existingRental.Value.PreparationTimeInDays;
             var newPreparationTime = model.PreparationTimeInDays;
             var neededToAdd = newPreparationTime - oldPreparationTime;  // we have to add additional preparingDates for eachbooking.
             foreach (var booking in bookingsBelongTheRental)
             {
                 var startDate = booking.Value.Start;
                 var endDate = booking.Value.Start.Date.AddDays(booking.Value.Nights);
                 var preparingEndDate = endDate.Date.AddDays(existingRental.Value.PreparationTimeInDays);

                 var conflictingUnits = bookingsBelongTheRental.Count(bk => bk.Value.Start >= startDate && bk.Value.Start <= endDate &&bk.Value.Id != booking.Value.Id); //conflicting booking units
                 if (model.Units < conflictingUnits)
                 {
                     throw new ApplicationException("You can not change unit because booking table has unit more than new unit ");
                 }
                 
                 
                 var conflictingPreparationTimes = bookingsBelongTheRental.Count(bk => bk.Value.Start >= endDate && bk.Value.Start <= preparingEndDate); //conflicting booking preprationTimes
                 if (conflictingPreparationTimes>0)
                 {
                     throw new ApplicationException("You can not change preparationTime because booking table has scheduled booking in the new preparatime");
                 }
             }

         
             existingRental.Value.Units = model.Units;
             existingRental.Value.PreparationTimeInDays = model.PreparationTimeInDays;
             return existingRental.Value;
        }
    }
}