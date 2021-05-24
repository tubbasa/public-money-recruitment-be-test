using System;
using System.Collections.Generic;

namespace VacationRental.Api.Models
{
    public class CalendarDateViewModel
    {
        public DateTime Date { get; set; }
        public List<CalendarBookingViewModel> Bookings { get; set; }
        public List<PreparationTimes> PreparationTimes { get; set; }
    }
    
    public class PreparationTimes{
        public int  Unit { get; set; }
    }
}
