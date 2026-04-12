namespace RentFlowApi.DTO
{
    public class CreateBookingRequest
    {
        public int PropertyId { get; set; }
        public DateOnly ArrivalDate { get; set; }
        public DateOnly DepartureDate { get; set; }
        public int GuestsCount { get; set; }
        public bool NeedsContactlessCheckin { get; set; }
    }
}
