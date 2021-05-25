namespace VacationRental.Api.Models
{
    public class RentalViewModel
    {
        public int Id { get; set; }
        
        //It is quantity of this rental space: For example lets say it is a room, if we declare 2 to Units; It means we have 2 similar units. 
        public int Units { get; set; }
        
        //It is a preparation time for each units.
        public int PreparationTimeInDays { get; set; }
    }
}
