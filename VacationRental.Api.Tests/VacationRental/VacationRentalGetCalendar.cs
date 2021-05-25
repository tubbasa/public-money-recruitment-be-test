using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests.VacationRental
{
    [Collection("Integration")]
    public class VacationRentalGetCalendar
    {
        private readonly HttpClient _client;

        public VacationRentalGetCalendar(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostVacationRental_ThenAGetReturnsTheCreatedRentalId()
        {
            var postRentalRequest = new RentalBindingModel
            {
                Units = 2,
                PreparationTimeInDays =  3
            };

            ResourceIdViewModel postRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/VacationRental/rentals", postRentalRequest))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBookingRequest = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 01)
            };

            ResourceIdViewModel postBookingResult;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBookingResult = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            ResourceIdViewModel postVacationRentalResult;
            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/VacationRental/calendar"))
            {
                var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<List<CalendarDateViewModel>>();

                Assert.True(getCalendarResponse.IsSuccessStatusCode);
                Assert.Equal(new DateTime(2000, 01, 01), getCalendarResult.FirstOrDefault(cl=>postBookingResult.Id == cl.Bookings.FirstOrDefault(x=>x.Id==postBookingResult.Id).Id).Date);
                Assert.IsType<List<CalendarBookingViewModel>>(getCalendarResult.FirstOrDefault().Bookings);
                Assert.IsType<List<PreparationTimes>>(getCalendarResult.FirstOrDefault().PreparationTimes);
            }
            
        }
    }
}