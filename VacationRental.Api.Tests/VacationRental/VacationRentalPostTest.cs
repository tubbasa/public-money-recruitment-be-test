using System;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests.VacationRental
{
    [Collection("Integration")]
    public class VacationRentalTest
    {
        private readonly HttpClient _client;

        public VacationRentalTest(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostVacationRental_ThenAGetReturnsTheCreatedRentalId()
        {
            var vacationRentalResult = new RentalBindingModel
            {
                Units = 4,
                PreparationTimeInDays = 2
            };

            ResourceIdViewModel postVacationRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/VacationRental/rentals", vacationRentalResult))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postVacationRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }
        }
    }
}